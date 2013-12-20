using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace NSane
{
    /// <summary>
    /// A useful base class for things that are common
    /// </summary>
    internal abstract class Device : DisposableObject,
                                     IOpenedDevice,
                                     IOpenableDevice
    {
        /// <summary>
        /// Construct the device with what we know about it
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="vendor">The vendor</param>
        /// <param name="model">The model</param>
        /// <param name="type">The type</param>
        protected Device(string name,
                         string vendor,
                         string model,
                         string type)
        {
            Name = name;
            Vendor = vendor;
            Model = model;
            Type = type;
        }

        /// <summary>
        /// Gets the name of the device
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the vendor of the device
        /// </summary>
        public string Vendor { get; private set; }

        /// <summary>
        /// Gets the model of the device
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Get the type of the device
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Gets all the options for the device
        /// </summary>
        public abstract IEnumerable<IDeviceOption> AllOptions { get; }

        /// <summary>
        /// Opens the device
        /// </summary>
        /// <returns>An open device</returns>
        public abstract IOpenedDevice Open();

        /// <summary>
        /// Performs a scan
        /// </summary>
        /// <returns>The <see cref="IScanResult"/> that is produced as a 
        /// result of the scan</returns>
        public abstract IScanResult Scan();

        /// <summary>
        /// Performs a scan and calls a callback when complete
        /// </summary>
        /// <param name="onCompleteCallback">The callback to cal on
        /// completion</param>
        /// <param name="onFailureCallback">The callback to call on
        /// error - have a look at the exception that was sent to get
        /// further information - it is an <see cref="AggregateException"/>.
        /// </param>
        /// <returns>The scanned image result</returns>
        public abstract IScanResult Scan(
            Action<BitmapSource> onCompleteCallback,
            Action<AggregateException> onFailureCallback);
    }
}
