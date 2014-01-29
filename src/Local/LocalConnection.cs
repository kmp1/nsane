using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NSane.Local
{
    /// <summary>
    /// This makes a connection to the local SANE API.
    /// </summary>
    internal class LocalConnection : DisposableObject, IConnection
    {
        private IEnumerable<IOpenableDevice> _devices;
        private bool _open;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="LocalConnection"/> class with
        /// credentials.
        /// </summary>
        public LocalConnection()
        {
            int ver;
            var status = NativeMethods.SaneInitialize(out ver, IntPtr.Zero);
            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            Version = ver;

            _open = true;
        }

        /// <summary>
        /// Directly open a device
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IOpenedDevice OpenDevice(string name)
        {
            IntPtr handle;
            var status = NativeMethods.SaneOpen(name, out handle);
            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            var device = new LocalDevice(name, handle);
            return device;
        }

        /// <summary>
        /// Gets the SANE version
        /// </summary>
        public int Version
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all the available devices
        /// </summary>
        public IEnumerable<IOpenableDevice> AllDevices 
        {
            get { return _devices ?? (_devices = LoadDevices()); }
        }        

        /// <summary>
        /// Dispose the connection
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Close();
            base.Dispose(disposing);
        }

        private void Close()
        {
            if (_open)
            {
                NativeMethods.SaneExit();
                _open = false;
            }
        }

        private IEnumerable<IOpenableDevice> LoadDevices()
        {
            IntPtr devicesPointer = IntPtr.Zero;
            var status = NativeMethods.SaneGetDevices(ref devicesPointer, true);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            var devs = PointerToDevices(devicesPointer);

            return devs.ToList();
        }

        private IEnumerable<IOpenableDevice> PointerToDevices(IntPtr array)
        {
            if (array == IntPtr.Zero)
                return new IOpenableDevice[] { };
            int cnt = GetDeviceCount(array);
            return PointerToDevices(array, cnt);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", 
            "CA2000:Dispose objects before losing scope",
            Justification = "If I did this, the whole thing would break!")]
        private IEnumerable<IOpenableDevice> PointerToDevices(IntPtr array, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (array == IntPtr.Zero)
                yield break;
                        
            for (int i = 0; i < count; ++i)
            {
                IntPtr s = Marshal.ReadIntPtr(array, i * IntPtr.Size);

                if (s == IntPtr.Zero)
                    continue;

                var device = (SaneDevice)Marshal.PtrToStructure(s, typeof(SaneDevice));
                
                // TODO: Maybe we need to free here??
                // Marshal.FreeCoTaskMem(s);

                yield return new LocalDevice(device.Name,
                                             device.Vendor,
                                             device.Model,
                                             device.Type);
            }            
        }

        private static int GetDeviceCount(IntPtr array)
        {
            int count = 0;
            while (Marshal.ReadIntPtr(array, count * IntPtr.Size) != IntPtr.Zero)
            {
                ++count;
            }
            return count;
        }
    }
}

