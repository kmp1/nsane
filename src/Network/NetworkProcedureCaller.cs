using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;

using NSane.Constraint;

namespace NSane.Network
{
    /// <summary>
    /// Here we have centralized all the network RPC calls - basically it owns
    /// a <see cref="NetworkMethods"/> object, which is in charge of making the
    /// calls, but encapsulates the logic of making the them (when to 
    /// authenticate, etc, etc)
    /// </summary>
    internal class NetworkProcedureCaller : DisposableObject
    {
        /// <summary>
        /// This is a special tag that is used by the server to tell us we need
        /// to do some hashing of the password (not really secure, but better
        /// than plain text)
        /// </summary>
        private const string PasswordSecureTag = "$MD5$";

        private readonly NetworkMethods _wire;

        /// <summary>
        /// Construct with the minimum information it needs
        /// </summary>
        /// <param name="hostName">The host name to connect to</param>
        /// <param name="port">The port to connect to</param>
        internal NetworkProcedureCaller(string hostName, int port)
        {
            _wire = new NetworkMethods(hostName, port);
        }

        /// <summary>
        /// Calls the Init remote procedure
        /// </summary>
        /// <param name="userName">The username</param>
        /// <param name="versionCode">The version code</param>
        /// <returns>The version of the SANE daemon</returns>
        internal int Initialise(string userName, int versionCode)
        {
            _wire.SendCommand(NetworkCommand.Initialize);
            _wire.SendWord(versionCode);

            _wire.SendString(userName);
            int status = _wire.ReadWord();

            if (status != (int) SaneStatus.Success)
                throw NSaneException.CreateFromStatus(status);

            // TODO: Convert the version to something more human
            int version = _wire.ReadWord();
            return version;
        }

        /// <summary>
        /// Requests a list of devices
        /// </summary>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The device list</returns>
        internal IEnumerable<IOpenableDevice> RequestDeviceList(
            string userName,
            string password)
        {
            _wire.SendCommand(NetworkCommand.GetDevices);

            var status = _wire.ReadWord();
            if (status != (int) SaneStatus.Success)
                throw NSaneException.CreateFromStatus(status);

            var ret = _wire
                .ReadPointerArray((w, i) =>
                                  CreateDevice(w, userName, password));
            return ret;
        }

        /// <summary>
        /// Calls the open remote procedure to open the given device.
        /// </summary>
        /// <param name="deviceName">The name of the device to open</param>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <returns>A handle to the device</returns>
        internal int OpenDevice(string deviceName, 
                                string userName, 
                                string password)
        {
            return DoOpenDevice(deviceName, true, userName, password);
        }

        /// <summary>
        /// Calls the close remote procedure to close the given device
        /// </summary>
        /// <param name="handle">The handle of the device to close</param>
        internal void CloseDevice(int handle)
        {
            _wire.SendCommand(NetworkCommand.Close);
            _wire.SendWord(handle);
            _wire.ReadWord();
        }

        /// <summary>
        /// Calls the get option descriptors remote procedure call for the 
        /// given device
        /// </summary>
        /// <param name="handle">The handle of the device</param>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <param name="reloadFunction">The function to call if
        /// the device requires reloading options</param>
        /// <returns>The device options</returns>
        internal IEnumerable<IDeviceOption> RequestOptionList(
            int handle,
            string userName,
            string password,
            Action reloadFunction)
        {
            _wire.SendCommand(NetworkCommand.GetOptionDescriptors);
            _wire.SendWord(handle);
            var ret = _wire
                .ReadPointerArray((w, n) =>
                                  CreateDeviceOption(w,
                                                     n,
                                                     handle,
                                                     userName,
                                                     password,
                                                     reloadFunction));
            return ret;
        }

        /// <summary>
        /// Calls the control remote procedure call to adjust the value of the
        /// given device option
        /// </summary>
        /// <typeparam name="T">The type of option</typeparam>
        /// <param name="handle">The handle to the device</param>
        /// <param name="option">The option index</param>
        /// <param name="action">The action to perform</param>
        /// <param name="type">The type of the option</param>
        /// <param name="size">The size of the option</param>
        /// <param name="sender">An action to deal with serializing the
        /// sending data</param>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <param name="reloadFunction">The reload function</param>
        /// <returns>The result of the set/get operation</returns>
        internal T ControlOption<T>(int handle,
                                    int option,
                                    SaneOptionAction action,
                                    SaneType type,
                                    int size,
                                    Func<NetworkMethods, int, int> sender,
                                    string userName,
                                    string password,
                                    Action reloadFunction)
        {
            return DoControlOption<T>(handle,
                                      option,
                                      action,
                                      type,
                                      size,
                                      sender,
                                      true,
                                      userName,
                                      password,
                                      reloadFunction);
        }

        /// <summary>
        /// Calls the Start remote procedure (maybe many times) and gets hold
        /// of the scanned image
        /// </summary>
        /// <param name="handle">The handle to the device</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <param name="cancelToken">The cancellation token</param>
        /// <returns>The scanned image</returns>
        internal BitmapSource Scan(int handle, 
                                   string userName, 
                                   string password, 
                                   CancellationToken cancelToken)
        {
            cancelToken.Register(() => Cancel(handle));

            int pixelsPerLine;
            int lines;
            int depth;
            bool littleEndian;
            bool color;
            byte[] data;
            using (var ms = new MemoryStream())
            {
                int bytesPerLine;             
                using (var stream = DoScan(handle,
                                           userName,
                                           password,
                                           true,
                                           ms,
                                           cancelToken,
                                           out bytesPerLine,
                                           out pixelsPerLine,
                                           out lines,
                                           out depth,
                                           out littleEndian,
                                           out color))
                {
                    data = stream.ToArray();
                }              
            }

            var ret = ImageCreator.ToBitmap(data,
                                            pixelsPerLine,
                                            lines,
                                            depth,
                                            littleEndian,
                                            color);
            return ret;
        }

        /// <summary>
        /// Calls the cancel remote procedure call
        /// </summary>
        /// <param name="handle">The handle of the device whose scanning
        /// operaton you wish to cancel</param>
        internal void Cancel(int handle)
        {
            _wire.SendCommand(NetworkCommand.Cancel);
            _wire.SendWord(handle);
            _wire.ReadWord();
        }

        /// <summary>
        /// Calls the exit remote procedure call (this shuts down the session
        /// gracefully)
        /// </summary>
        internal void Exit()
        {
            _wire.SendCommand(NetworkCommand.Exit);
        }

        /// <summary>
        /// Dispose of this caller
        /// </summary>
        /// <param name="disposing"><c>true</c> at dispose time</param>
        protected override void Dispose(bool disposing)
        {
            _wire.Dispose();
            base.Dispose(disposing);
        }        

        /// <summary>
        /// Actually performs the Start remote procedure call
        /// </summary>
        /// <param name="handle">The handle to the device</param>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <param name="firstTime"><c>true</c> if this is the first attempt
        /// (in which case it will retry with the credentials)</param>
        /// <param name="stream">The stream to add onto (in the case of
        /// multiple calls for one image)</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="bytesPerLine">The returned bytes per line</param>
        /// <param name="pixelsPerLine">The returned pixels per line</param>
        /// <param name="lines">The returned lines</param>
        /// <param name="depth">The returned color depth</param>        
        /// <param name="littleEndian"><c>true</c> if the data is little
        /// endian</param>
        /// <param name="color"><c>true</c> if it is color</param>
        /// <returns>The final result of the scan</returns>
        private MemoryStream DoScan(int handle,
                                    string userName,
                                    string password,
                                    bool firstTime,
                                    MemoryStream stream,
                                    CancellationToken cancellationToken,
                                    out int bytesPerLine,
                                    out int pixelsPerLine,
                                    out int lines,
                                    out int depth,
                                    out bool littleEndian,
                                    out bool color)
        {
            FrameFormat format;
            bool lastFrame;

            GetParameters(handle,
                          out format,
                          out lastFrame,
                          out bytesPerLine,
                          out pixelsPerLine,
                          out lines,
                          out depth);

            color = format != FrameFormat.Gray;
            cancellationToken.ThrowIfCancellationRequested();

            _wire.SendCommand(NetworkCommand.Start);
            _wire.SendWord(handle);

            int status = _wire.ReadWord();
            int port = _wire.ReadWord();
            int byteOrder = _wire.ReadWord();
            string resource = _wire.ReadString();

            littleEndian = byteOrder == 0x1234;

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(resource) && firstTime)
            {
                Authorize(userName, password, resource);
                return DoScan(handle,
                              userName,
                              password,
                              false,
                              stream,
                              cancellationToken,
                              out bytesPerLine,
                              out pixelsPerLine,
                              out lines,
                              out depth,
                              out littleEndian,
                              out color);
            }

            if (status != (int) SaneStatus.Success)
                throw NSaneException.CreateFromStatus(status);
            
            MemoryStream ret;
            if (!lastFrame)
            {
                ret = AddImageData(stream, format, port);
                return DoScan(handle,
                              userName,
                              password,
                              false,
                              ret,
                              cancellationToken,
                              out bytesPerLine,
                              out pixelsPerLine,
                              out lines,
                              out depth,
                              out littleEndian,
                              out color);
            }

            ret = AddImageData(stream, format, port);
            return ret;
        }

        /// <summary>
        /// Here we will take the given stream and add the current data to it
        /// and return a new stream
        /// </summary>
        /// <param name="stream">The stream to take</param>
        /// <param name="format">The frame format we are dealing with</param>
        /// <param name="port">The port to read on</param>
        /// <returns>The resulting data</returns>
        private MemoryStream AddImageData(MemoryStream stream,
                                          FrameFormat format,
                                          int port)
        {
            if (format == FrameFormat.Rgb || format == FrameFormat.Gray)
                return ReadImageData(port);

            if (stream == null)
                throw new ArgumentNullException("stream");

            // TODO Here we need to use the format to know what frame we are (
            // red, green or blue) and the original data - in the stream
            // argument to interleave the values
            throw new NotSupportedException();
        }

        /// <summary>
        /// Performs the get parameters remote procedure call
        /// </summary>
        /// <param name="handle">The handle to the device</param>
        /// <param name="format">The returned format</param>
        /// <param name="lastFrame">The returned last frame value</param>
        /// <param name="bytesPerLine">The returned bytes per line</param>
        /// <param name="pixelsPerLine">The returned pixels per line</param>
        /// <param name="lines">The returned lines</param>
        /// <param name="depth">The returned color depth</param>
        private void GetParameters(int handle,
                                   out FrameFormat format,
                                   out bool lastFrame,
                                   out int bytesPerLine,
                                   out int pixelsPerLine,
                                   out int lines,
                                   out int depth)
        {
            _wire.SendCommand(NetworkCommand.GetParameters);
            _wire.SendWord(handle);

            int status = _wire.ReadWord();
            format = (FrameFormat) _wire.ReadWord();
            lastFrame = _wire.ReadWord() == 1;
            bytesPerLine = _wire.ReadWord();
            pixelsPerLine = _wire.ReadWord();
            lines = _wire.ReadWord();
            depth = _wire.ReadWord();

            if (status != (int) SaneStatus.Success)
                throw NSaneException.CreateFromStatus(status);
        }

        /// <summary>
        /// Reads the whole image data from the given port
        /// </summary>
        /// <param name="port">The port to read from</param>
        /// <returns>The total data from the port</returns>
        private MemoryStream ReadImageData(int port)
        {
            MemoryStream ret;
            using (var wire = new NetworkMethods(_wire.HostName, port))
            {
                ret = wire.ReadImageRecords();
            }
            return ret;
        }

        /// <summary>
        /// Creates a new device object
        /// </summary>
        /// <param name="wire">The wire to read from</param>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <returns>A new device object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
                "If I did that, this whole thing would break!")]
        private IOpenableDevice CreateDevice(NetworkMethods wire,
                                             string userName,
                                             string password)
        {
            var name = wire.ReadString();
            var vendor = wire.ReadString();
            var model = wire.ReadString();
            var type = wire.ReadString();
            return new NetworkDevice(name,
                                     vendor,
                                     model,
                                     type,
                                     this,
                                     userName,
                                     password);
        }

        /// <summary>
        /// Actually performs the control option remote procedure call
        /// </summary>
        /// <typeparam name="T">The type of option</typeparam>
        /// <param name="handle">The handle to the device</param>
        /// <param name="option">The option index</param>
        /// <param name="action">The action to perform</param>
        /// <param name="type">The type of the option</param>
        /// <param name="size">The size of the option</param>
        /// <param name="sender">An action to deal with serializing the
        /// sending data</param>
        /// <param name="firstTime"><c>true</c> if this is the first attempt
        /// (in which case it will retry with the credentials)</param>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <param name="reloadFunction">This function is called if
        /// the options require reloading</param>
        /// <returns>The result of the operation</returns>
        private T DoControlOption<T>(int handle,
                                     int option,
                                     SaneOptionAction action,
                                     SaneType type,
                                     int size,
                                     Func<NetworkMethods, int, int> sender,
                                     bool firstTime,
                                     string userName,
                                     string password,
                                     Action reloadFunction)
        {         
            _wire.SendCommand(NetworkCommand.ControlOption);
            _wire.SendWord(handle);
            _wire.SendWord(option);
            _wire.SendWord((int) action);
            _wire.SendWord((int) type);

            int sze = sender(_wire, size);

            int status = _wire.ReadWord();            
            
            var info = (SaneOptionInformation) _wire.ReadWord();
            var valueType = (SaneType) _wire.ReadWord();
            int valueSize = _wire.ReadWord();

            object ret;
            switch (type)
            {
                case SaneType.Boolean:
                    _wire.ReadWord();
                    ret = _wire.ReadWord() == 1;
                    break;
                case SaneType.Integer:
                    _wire.ReadWord();
                    ret = _wire.ReadWord();
                    break;
                case SaneType.String:
                    ret = _wire.ReadString();
                    break;
                case SaneType.Fixed:
                    _wire.ReadWord();
                    ret = _wire.ReadWord();
                    break;
                case SaneType.Button:
                    ret = _wire.ReadWord() == 1;
                    break;
                default:
                    throw new NotSupportedException(
                        "Option type not supported");
            }

            string resource = _wire.ReadString();

            if (valueType != type)
                throw new InvalidOperationException(
                    "Something has gone horribly wrong here - the returned "
                    + "type is different to the passed type!");

            if (valueSize != sze)
                throw new InvalidOperationException(
                    "Something has gone horribly wrong here - the returned "
                    + "size is different to the passed type!");

            if (!string.IsNullOrEmpty(resource) && firstTime)
            {
                Authorize(userName, password, resource);
                return DoControlOption<T>(handle,
                                          option,
                                          action,
                                          type,
                                          size,
                                          sender,
                                          false,
                                          userName,
                                          password,
                                          reloadFunction);
            }

            if (status != (int) SaneStatus.Success)
                throw NSaneException.CreateFromStatus(status);

            // OK, we need to reload the options now...
            if ((info & SaneOptionInformation.ReloadOptions) ==
                SaneOptionInformation.ReloadOptions)
                reloadFunction();

            return (T) ret;
        }

        /// <summary>
        /// Creates a device option
        /// </summary>
        /// <param name="wire">The wire to read on</param>
        /// <param name="number">The option number</param>
        /// <param name="handle">The device handle</param>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <param name="reloadFunction">The function to call if
        /// the device requires reloading options</param>
        /// <returns>The device option</returns>
        private IDeviceOption CreateDeviceOption(NetworkMethods wire,
                                                 int number,
                                                 int handle,
                                                 string userName,
                                                 string password,
                                                 Action reloadFunction)
        {
            var name = wire.ReadString();
            var title = wire.ReadString();
            var description = wire.ReadString();

            int type = wire.ReadWord();
            int unit = wire.ReadWord();
            int size = wire.ReadWord();
            int cap = wire.ReadWord();
            int ct = wire.ReadWord();

            var opt = new NetworkDeviceOption(name,
                                              title,
                                              description,
                                              size,
                                              number,
                                              (SaneType) type,
                                              (SaneUnit) unit,
                                              (SaneCapabilities) cap,
                                              handle,
                                              this,
                                              userName,
                                              password,
                                              reloadFunction);

            switch (ct)
            {
                case (int) SaneConstraint.None:
                    opt.Constraint = new NoneConstraint();
                    break;
                case (int) SaneConstraint.IntegerList:
                    opt.Constraint = CreateIntegerListConstraint(wire);
                    break;
                case (int) SaneConstraint.StringList:
                    opt.Constraint = CreateStringListConstraint(wire);
                    break;
                case (int) SaneConstraint.Range:
                    opt.Constraint = CreateRangeConstraint(wire);
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format
                            (CultureInfo.CurrentCulture,
                             "The constraint type '{0}' is not supported",
                             ct));
            }

            return opt;
        }

        /// <summary>
        /// Creates a range constraint
        /// </summary>
        /// <param name="wire">The wire to read on</param>
        /// <returns>The constraint</returns>
        private static IOptionConstraint CreateRangeConstraint(
            NetworkMethods wire)
        {
            wire.ReadWord();
            int min = wire.ReadWord();
            int max = wire.ReadWord();
            int quant = wire.ReadWord();
            return new RangeConstraint(min, max, quant);
        }

        /// <summary>
        /// Creates an integer list constraint
        /// </summary>
        /// <param name="wire">The wire to read on</param>
        /// <returns>The constraint</returns>
        private static IOptionConstraint CreateIntegerListConstraint(
            NetworkMethods wire)
        {
            wire.ReadWord();
            var values = wire.ReadArray((w, i) => w.ReadWord());
            return new Int32ListConstraint(values);
        }

        /// <summary>
        /// Creates the string list constraint
        /// </summary>
        /// <param name="wire">The wire to read on</param>
        /// <returns>The constraint</returns>
        private static IOptionConstraint CreateStringListConstraint(
            NetworkMethods wire)
        {
            var values = wire.ReadArray((w, i) =>
                                            {
                                                var s = w.ReadString();
                                                if (string.IsNullOrEmpty(s))
                                                    return null;
                                                return s;
                                            });
            return new StringListConstraint(values);
        }

        /// <summary>
        /// Actually performs the open procedure call
        /// </summary>
        /// <param name="deviceName">The device to open</param>
        /// <param name="firstTime"><c>true</c> if this is the first attempt
        /// (in which case it will retry with the credentials)</param>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The handle to the open device</returns>
        private int DoOpenDevice(string deviceName,
                                 bool firstTime,
                                 string userName,
                                 string password)
        {
            _wire.SendCommand(NetworkCommand.Open);
            _wire.SendString(deviceName);

            int status = _wire.ReadWord();
            int handle = _wire.ReadWord();
            string resource = _wire.ReadString();

            if (!string.IsNullOrEmpty(resource) && firstTime)
            {
                Authorize(userName, password, resource);
                return DoOpenDevice(deviceName, false, null, null);
            }

            if (status != (int) SaneStatus.Success)
                throw NSaneException.CreateFromStatus(status);

            return handle;
        }

        /// <summary>
        /// Calls the authorize remote procedure
        /// </summary>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        /// <param name="resource">The resource to access</param>
        private void Authorize(string userName,
                               string password,
                               string resource)
        {
            if (userName == null)
                userName = string.Empty;
            
            if (password == null)
                password = string.Empty; 

            string pwd = resource.Contains(PasswordSecureTag)
                             ? SecurePassword(password, ref resource)
                             : password;

            _wire.SendCommand(NetworkCommand.Authorize);
            _wire.SendString(resource);
            _wire.SendString(userName);
            _wire.SendString(pwd);

            _wire.ReadWord();
        }

        /// <summary>
        /// Secures a password as per the SANE API documentation (basically
        /// just hashes it using the MD5 algorithm)
        /// </summary>
        /// <param name="password">The password to secure</param>
        /// <param name="resource">The resource we are securing</param>
        /// <returns>The secured password</returns>
        private static string SecurePassword(string password, 
                                             ref string resource)
        {
            int tagIndex = resource
                .LastIndexOf(PasswordSecureTag,
                             StringComparison.OrdinalIgnoreCase);
            
            string random = resource
                .Substring(
                    tagIndex
                    + PasswordSecureTag.Length);

            password = PasswordSecureTag
                         + CalculateMd5Hash(random + password);
            
            resource = resource.Substring(0, tagIndex);
            return password;
        }
        
        /// <summary>
        /// Calucaltes an MD5 hash of the given input
        /// </summary>
        /// <param name="input">The input to calculate the has of</param>
        /// <returns>The MD5 hash</returns>
        private static string CalculateMd5Hash(string input)
        {
            byte[] buf = Encoding.ASCII.GetBytes(input);

            var sb = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(buf);

                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2",
                                               CultureInfo.InvariantCulture));
                }
            }
            return sb.ToString();
        }         
    }
}
