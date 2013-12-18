using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace NSane
{
    /// <summary>
    /// This is the implementation of <see cref="IScanResult"/> that uses
    /// the <see cref="Task"/> mechanism of the .net framework
    /// </summary>
    internal class TaskBasedScanResult : IScanResult
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task<Bitmap> _task;
        private readonly Action<Bitmap> _onCompleteCallback;

        /// <summary>
        /// Construct with the details it needs to allow for completion
        /// checking and stopping
        /// </summary>
        /// <param name="cancellationTokenSource">The cancellation source
        /// </param>
        /// <param name="task">The task</param>
        /// <param name="onCompleteCallback">A callback to call when the
        /// task has finished (can be null if you don't care)</param>
        internal TaskBasedScanResult(
            CancellationTokenSource cancellationTokenSource,
            Task<Bitmap> task,
            Action<Bitmap> onCompleteCallback)
        {
            _onCompleteCallback = onCompleteCallback;
            _task = task;
            _cancellationTokenSource = cancellationTokenSource;
            _task.ContinueWith(t => FinishedCallback());
        }

        /// <summary>
        /// Gets the result image.  As soon as you call the getter for this 
        /// property, the call will block until the operation has completed
        /// so use the <see cref="IScanResult.IsFinished"/> property to decide whether to
        /// call this or not.
        /// </summary>
        public Bitmap Image
        {
            get { return _task.Result; }
        }

        /// <summary>
        /// Returns <c>true</c> if the scanning has completed
        /// </summary>
        public bool IsFinished
        {
            get { return _task.IsCompleted; }
        }

        /// <summary>
        /// Cancel the scanning operation
        /// </summary>
        public void StopScanning()
        {       
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// This is called when the scan has completed
        /// </summary>
        private void FinishedCallback()
        {
            if (_onCompleteCallback != null)
            {
                _onCompleteCallback(Image);
            }
        }
    }
}
