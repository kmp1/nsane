using System.IO;
using System.Windows.Media.Imaging;

namespace NSane
{
    /// <summary>
    /// This is the result of a scan call.  A call to scan returns immediately
    /// and you can use this object to wait for the result (just get the
    /// property and it will block <see cref="Image"/> or you can check the
    /// <see cref="IsFinished"/> property and also call 
    /// <see cref="StopScanning"/> if you wish to cancel the process.
    /// </summary>
    public interface IScanResult
    {
        /// <summary>
        /// Gets the result image.  As soon as you call the getter for this 
        /// property, the call will block until the operation has completed
        /// so use the <see cref="IsFinished"/> property to decide whether to
        /// call this or not.
        /// </summary>
        BitmapSource Image { get; }

        /// <summary>
        /// Returns <c>true</c> if the scanning has completed
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Cancel the scanning operation
        /// </summary>
        void StopScanning();
    }
}
