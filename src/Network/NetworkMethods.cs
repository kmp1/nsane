using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NSane.Network
{
    /// <summary>
    /// This is in charge of converting .Net types to how they should appear on
    /// the wire and sending them to the SANE daemon on the other end
    /// </summary>
    internal class NetworkMethods : DisposableObject
    {
        private readonly TcpClient _socket;
        private readonly NetworkStream _stream;

        /// <summary>
        /// Construct with the details we need for the connection
        /// </summary>
        /// <param name="hostname">The host name</param>
        /// <param name="port">The port</param>
        internal NetworkMethods(string hostname, int port)
        {
            HostName = hostname;
            Port = port;
            _socket = new TcpClient(HostName, Port);
            _stream = _socket.GetStream();
        }

        /// <summary>
        /// Gets the host name for this connection
        /// </summary>
        internal string HostName { get; private set; }

        /// <summary>
        /// Gets the port for this connection
        /// </summary>
        internal int Port { get; private set; }

        /// <summary>
        /// Dispose this object
        /// </summary>
        /// <param name="disposing"><c>true</c> if we are disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (_socket != null)
            {
                _socket.Close();
            }

            if (_stream != null)
            {
                _stream.Close();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Reads all the image data records available at this sane daemon port
        /// </summary>
        /// <returns>The data as a stream</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
                "If I did that, this whole thing would break!")]
        internal MemoryStream ReadImageRecords()
        {
            var ms = new MemoryStream();

            while (true)
            {
                bool atEnd;
                var record = ReadRecord(out atEnd);         
                if (atEnd)
                    break;
                ms.Write(record, 0, record.Length);
            }

            return ms;
        }

        /// <summary>
        /// Sends the given network command
        /// </summary>
        /// <param name="command">The command to send</param>
        internal void SendCommand(NetworkCommand command)
        {
            SendWord((int) command);
        }

        /// <summary>
        /// Sends a "word"
        /// </summary>
        /// <param name="word">THe word to send</param>
        internal void SendWord(int word)
        {
            _stream.WriteByte((byte) ((word >> 24) & 0xff));
            _stream.WriteByte((byte) ((word >> 16) & 0xff));
            _stream.WriteByte((byte) ((word >> 8) & 0xff));
            _stream.WriteByte((byte) ((word >> 0) & 0xff));
        }

        /// <summary>
        /// Reads a word
        /// </summary>
        /// <returns>The read word</returns>
        internal int ReadWord()
        {
            int value = 0;
            value += (_stream.ReadByte() << 24);
            value += (_stream.ReadByte() << 16);
            value += (_stream.ReadByte() << 8);
            value += (_stream.ReadByte() << 0);
            return value;
        }

        /// <summary>
        /// Send a string
        /// </summary>
        /// <param name="value">The string to send</param>
        internal void SendString(string value)
        {
            SendWord(255);
            for (int i = 0; i < 255; ++i)
            {
                int val = i < value.Length ? value[i] : 0;
                _stream.WriteByte((byte) val);
            }
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <returns>The string to read</returns>
        internal string ReadString()
        {
            var str = new StringBuilder();

            int size = ReadWord();
            bool add = true;

            if (size > 0)
            {
                for (int i = 0; i < size; ++i)
                {
                    int byt = _stream.ReadByte();
                    var chr = (char) byt;
                    if (byt == 0)
                    {
                        add = false;
                    }
                    if (add)
                    {
                        str.Append(chr);
                    }
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Reads a pointer array
        /// </summary>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <param name="creator">A function that is in charge of creating
        /// the type that is pointed to by the pointer</param>
        /// <returns>A collection of created items at the given pointers
        /// </returns>
        internal IEnumerable<T> ReadPointerArray<T>(Func<NetworkMethods, int, T> creator)
        {
            int size = ReadWord();

            // This is screaming out for a yield statement here and part of me
            // loved that I could with a Where (or whatever) break out early
            // here, but the problem is that if you do that SANE basically
            // freaks out - you need to get back the entire result set, hence
            // the list
            var ret = new List<T>();
            for (int i = 0; i < size; i++)
            {
                int isNull = ReadWord();
                if (isNull == 0)
                {
                    ret.Add(creator(this, i));
                }

            }
            return ret;
        }

        /// <summary>
        /// Reads a regular array (slightly different to a pointer array)
        /// </summary>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <param name="creator">A function that is in charge of creating
        /// the type that is pointed to by the pointer</param>
        /// <returns>A collection of created items at the given pointers
        /// </returns>
        internal IEnumerable<T> ReadArray<T>(Func<NetworkMethods, int, T> creator)
        {
            int size = ReadWord();

            // This is screaming out for a yield statement here and part of me
            // loved that I could with a Where (or whatever) break out early
            // here, but the problem is that if you do that SANE basically
            // freaks out - you need to get back the entire result set, hence
            // the list
            var ret = new List<T>();
            for (int i = 0; i < size; i++)
            {
                var val = creator(this, i);
                if (!EqualsDefaultValue(val))
                {
                    ret.Add(val);
                }
            }
            return ret;
        }

        public byte[] ReadBlock(int size)
        {
            byte[] array = new byte[size];
            int offset = 0;
                while (size > 0)
                {
                    int len = _stream.Read(array, offset, size);
                    offset += len;
                    size -= len;
                }
           
            return array;
        }

        /// <summary>
        /// Reads a single "record" of data
        /// </summary>
        /// <param name="atEnd"><c>true</c> if we are at the end of all the
        /// records</param>
        /// <returns>The data</returns>
        private byte[] ReadRecord(out bool atEnd)
        {
            int dataLength = ReadWord();
            //0xffffffff
            atEnd = (dataLength == -1);
            if (atEnd)
            {
                return null;
            }

            return ReadBlock(dataLength);
        }


        /// <summary>
        /// Simple little helper method to determing if the given generic
        /// parameters is it's "default" (e.g. null) value.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="value">The value to check</param>
        /// <returns><c>true</c> if it is the default value</returns>
        private static bool EqualsDefaultValue<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}
