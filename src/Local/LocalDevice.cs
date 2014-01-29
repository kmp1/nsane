using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace NSane.Local
{
    /// <summary>
    /// This is a device that is locally connected.
    /// </summary>
    internal class LocalDevice : Device
    {
        private IntPtr _handle;
        private bool _open;
        private bool _started;
        private IEnumerable<IDeviceOption> _options;

        /// <summary>
        /// Construct the device with the bits and bobs it needs
        /// </summary>
        /// <param name="name">The name of the device (unique I think)</param>
        /// <param name="vendor">The vendor</param>
        /// <param name="model">The model</param>
        /// <param name="type">The type</param>
        internal LocalDevice(string name,
                             string vendor,
                             string model,
                             string type)
            : base(name, vendor, model, type)
        {
            _open = false;
        }

        /// <summary>
        /// Construct a device in the open state
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        internal LocalDevice(string name, IntPtr handle)
            : base(name, null, null, null)
        {
            _handle = handle;
            _open = true;
        }

        /// <summary>
        /// Opens the device
        /// </summary>
        /// <returns></returns>
        public override IOpenedDevice Open()
        {
            IntPtr handle;
            var status = NativeMethods.SaneOpen(Name, out handle);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            _handle = handle;
            return this;
        }

        /// <summary>
        /// Implements the synchronous scanning method
        /// </summary>
        /// <returns></returns>
        public override IScanResult Scan()
        {
            return Scan(null, null);
        }

        /// <summary>
        /// Implements an asynchronous scan
        /// </summary>
        /// <param name="onCompleteCallback">The on complete callback</param>
        /// <param name="onFailureCallback">The on failure callback</param>
        /// <returns></returns>
        public override IScanResult Scan(Action<BitmapSource> onCompleteCallback, 
            Action<AggregateException> onFailureCallback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all options
        /// </summary>
        public override IEnumerable<IDeviceOption> AllOptions
        {
            get
            {
                return _options ??
                       (_options = RequestOptionList(ReloadOptions).ToList());
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

        private IEnumerable<IDeviceOption> RequestOptionList(Action reloadFunction)
        {
            var p = NativeMethods.SaneGetOptionDescriptor(_handle, 0);
            int count = p == IntPtr.Zero
                ? 0
                : p.ToInt32();
                
            for (int i = 1; i < count; i++)
            {
                var opt = ToOptionDescriptor(NativeMethods.SaneGetOptionDescriptor(_handle, i));

                var localOption = new LocalDeviceOption(opt.Name,
                                                        opt.Title,
                                                        opt.Description,
                                                        opt.Size,
                                                        i,
                                                        opt.Type,
                                                        opt.Unit,
                                                        opt.Capabilities,
                                                        _handle,
                                                        reloadFunction);
                yield return localOption;
            }
        }

        private static SaneOptionDescriptor ToOptionDescriptor(IntPtr pointer)
        {
            return (SaneOptionDescriptor)Marshal.PtrToStructure(
                pointer, typeof(SaneOptionDescriptor));
        }

        /// <summary>
        /// This function is called when the device needs to have it's options
        /// refreshed - we just call the get option list and then map the
        /// properties across
        /// </summary>
        private void ReloadOptions()
        {
            var options = RequestOptionList(ReloadOptions);

            foreach (var deviceOption in options)
            {                
                var opt = _options
                    .Cast<DeviceOption>()
                    .Single(o => (o).Number ==
                                 ((DeviceOption)deviceOption).Number);

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

