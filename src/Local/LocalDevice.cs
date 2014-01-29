using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace NSane.Local
{
    /// <summary>
    /// This is a device that is locally connected.
    /// </summary>
    internal class LocalDevice : Device
    {
        private readonly string _userName;
        private readonly string _password;

        private IntPtr _handle;
        private bool _open;
        private bool _started;

        /// <summary>
        /// Construct the device with the bits and bobs it needs
        /// </summary>
        /// <param name="name">The name of the device (unique I think)</param>
        /// <param name="vendor">The vendor</param>
        /// <param name="model">The model</param>
        /// <param name="type">The type</param>
        /// <param name="userName">The username to use for authenticated calls
        /// </param>
        /// <param name="password">The password to use for authenticated calls
        /// </param>
        internal LocalDevice(string name,
                             string vendor,
                             string model,
                             string type,
                             string userName,
                             string password)
            : base(name, vendor, model, type)
        {
            _userName = userName;
            _password = password;
            _open = false;
        }

        /// <summary>
        /// Construct a device in the open state
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        internal LocalDevice(string name,
                             IntPtr handle,
                             string userName,
                             string password)
            : base(name, null, null, null)
        {
            _userName = userName;
            _password = password;
            _handle = handle;
            _open = true;
        }

        public override IOpenedDevice Open()
        {
            IntPtr handle;
            var status = NativeMethods.SaneOpen(Name, out handle);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            _handle = handle;
            return this;
        }

        public override IScanResult Scan()
        {
            throw new NotImplementedException();
        }

        public override IScanResult Scan(Action<BitmapSource> onCompleteCallback, 
            Action<AggregateException> onFailureCallback)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IDeviceOption> AllOptions
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Create a user readble overview of the device (i.e. the
        /// name, vendor, etc)
        /// </summary>
        /// <returns>The aforementioned description</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 "{0} ({1}, {2} - {3})",
                                 Name,
                                 Vendor ?? string.Empty,
                                 Model ?? string.Empty,
                                 Type ?? string.Empty);
        }
        
        /// <summary>
        /// Dispose of the device object
        /// </summary>
        /// <param name="disposing"><c>true</c> if we are at disposing time
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (_open)
                Close();
            
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// Close the device
        /// </summary>
        private void Close()
        {
            if (_started)
                Cancel();

            NativeMethods.SaneClose(_handle);
            
            _handle = IntPtr.Zero;
            
            _open = false;
        }

        /// <summary>
        /// Cancels the current scan
        /// </summary>
        private void Cancel()
        {
            NativeMethods.SaneCancel(_handle);
            _started = false;
        }        
    }
}

