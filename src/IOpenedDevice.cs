using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace NSane
{
    /// <summary>
    /// This is a scanning device that is opened
    /// </summary>
    public interface IOpenedDevice : IDevice, IDisposable
    {
        /// <summary>
        /// Gets all the options for the device
        /// </summary>
        IEnumerable<IDeviceOption> AllOptions { get; }

        /// <summary>
        /// Perform a scan
        /// </summary>
        /// <returns>The <see cref="IScanResult"/> object that has been
        /// scanned</returns>
        IScanResult Scan();

        /// <summary>
        /// Performs a scan and calls a callback when complete
        /// </summary>
        /// <param name="onCompleteCallback">The callback to call on
        /// completion</param>
        /// <param name="onFailureCallback">The callback to call on
        /// error - have a look at the exception that was sent to get
        /// further information - it is an <see cref="AggregateException"/>.
        /// </param>
        /// <returns>The scanned image result</returns>
        IScanResult Scan(Action<BitmapSource> onCompleteCallback,
                         Action<AggregateException> onFailureCallback);
    }
}
