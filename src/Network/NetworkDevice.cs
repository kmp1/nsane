using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NSane.Network
{
    /// <summary>
    /// This is a device which connected to a SANE daemon over the network
    /// </summary>
    internal class NetworkDevice : Device
    {
        private readonly NetworkProcedureCaller _caller;
        private readonly string _userName;
        private readonly string _password;

        private bool _open;
        private int _handle;
        private bool _started;
        private IEnumerable<IDeviceOption> _options;

        /// <summary>
        /// Construct the device with the bits and bobs it needs
        /// </summary>
        /// <param name="name">The name of the device (unique I think)</param>
        /// <param name="vendor">The vendor</param>
        /// <param name="model">The model</param>
        /// <param name="type">The type</param>
        /// <param name="caller">The network RPC calling implementation</param>
        /// <param name="userName">The username to use for authenticated calls
        /// </param>
        /// <param name="password">The password to use for authenticated calls
        /// </param>
        internal NetworkDevice(string name,
                               string vendor,
                               string model,
                               string type,
                               NetworkProcedureCaller caller,
                               string userName,
                               string password)
            : base(name, vendor, model, type)
        {
            _caller = caller;
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// Construct a network device in the open state
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="handle">The handle</param>
        /// <param name="caller">The network RPC calling implementation</param>
        /// <param name="userName">The username to use for authenticated calls
        /// </param>
        /// <param name="password">The password to use for authenticated calls
        /// </param>
        internal NetworkDevice(string name,
                               int handle,
                               NetworkProcedureCaller caller,
                               string userName,
                               string password)
            : base(name, null, null, null)
        {
            _caller = caller;
            _userName = userName;
            _password = password;
            _handle = handle;
            _open = true;
        }

        /// <summary>
        /// Gets all the options for the device
        /// </summary>
        public override IEnumerable<IDeviceOption> AllOptions
        {
            get
            {
                return _options ??
                       (_options = _caller.RequestOptionList(_handle,
                                                             _userName,
                                                             _password,
                                                             ReloadOptions));
            }
        }

        /// <summary>
        /// Open the device
        /// </summary>
        /// <returns>The opened device</returns>
        public override IOpenedDevice Open()
        {
            _caller.OpenDevice(Name, _userName, _password);
            _open = true;
            return this;
        }

        /// <summary>
        /// Performs a sacn
        /// </summary>
        /// <returns>The scanned image result</returns>
        public override IScanResult Scan()
        {
            return Scan(null);
        }

        /// <summary>
        /// Performs a scan and calls a callback when complete
        /// </summary>
        /// <param name="onCompleteCallback">The callback to cal on
        /// completion</param>
        /// <returns>The scanned image result</returns>
        public override IScanResult Scan(Action<Bitmap> onCompleteCallback)
        {
            var cts = new CancellationTokenSource();
            var task = Task<Bitmap>.Factory.StartNew(
                () => _caller.Scan(_handle, _userName, _password, cts.Token),
                cts.Token);
            return new TaskBasedScanResult(cts, task, onCompleteCallback);
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
            if (_caller != null)
            {
                if (_open)
                {
                    Close();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Close the device
        /// </summary>
        private void Close()
        {
            if (_started)
            {
                Cancel();
            }

            _caller.CloseDevice(_handle);
            _handle = 0;

            _open = false;
        }

        /// <summary>
        /// Cancels the current scan
        /// </summary>
        private void Cancel()
        {
            _caller.Cancel(_handle);
            _started = false;
        }

        /// <summary>
        /// This function is called when the device needs to have it's options
        /// refreshed - we just call the get option list and then map the
        /// properties across
        /// </summary>
        private void ReloadOptions()
        {
            var options = _caller.RequestOptionList(_handle,
                                                    _userName,
                                                    _password,
                                                    ReloadOptions);

            foreach (var deviceOption in options)
            {
                var opt = _options
                    .Cast<DeviceOption>()
                    .Single(o => (o).Number ==
                                 ((DeviceOption) deviceOption).Number);

                opt.Name = deviceOption.Name;
                opt.Capabilities = deviceOption.Capabilities;
                opt.Constraint = deviceOption.Constraint;
                opt.Type = deviceOption.Type;
                opt.Unit = deviceOption.Unit;
                opt.Description = deviceOption.Description;
                opt.Title = deviceOption.Title;
            }
        }
    }
}
