using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NSane
{
    /// <summary>
    /// This is the implementation of <see cref="IScanResult"/> that uses
    /// the <see cref="Task"/> mechanism of the .net framework
    /// </summary>
    internal class TaskBasedScanResult : IScanResult
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task<BitmapSource> _task;
        private readonly Action<BitmapSource> _onCompleteCallback;
        private readonly Action<AggregateException> _onFailureCallback;

        /// <summary>
        /// Construct with the details it needs to allow for completion
        /// checking and stopping
        /// </summary>
        /// <param name="cancellationTokenSource">The cancellation source
        /// </param>
        /// <param name="task">The task</param>
        /// <param name="onCompleteCallback">A callback to call when the
        /// task has finished (can be null if you don't care)</param>
        /// <param name="onFailureCallback">A callback to call when some
        /// sort of error occurs</param>
        /// <param name="scheduler">The scheduler</param>
        internal TaskBasedScanResult(
            CancellationTokenSource cancellationTokenSource,
            Task<BitmapSource> task,
            Action<BitmapSource> onCompleteCallback,
            Action<AggregateException> onFailureCallback,
            TaskScheduler scheduler)
        {
            _onCompleteCallback = onCompleteCallback;
            _onFailureCallback = onFailureCallback;
            _task = task;
            _cancellationTokenSource = cancellationTokenSource;
            _task.ContinueWith(t => FinishedCallback(), scheduler);
        }

        /// <summary>
        /// Gets the result image.  As soon as you call the getter for this 
        /// property, the call will block until the operation has completed
        /// so use the <see cref="IScanResult.IsFinished"/> property to decide whether to
        /// call this or not.
        /// </summary>
        public BitmapSource Image
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
        /// Returns <c>true</c> if there was an error in scanning
        /// </summary>
        public bool IsError
        {
            get { return _task.IsFaulted; }
        }

        /// <summary>
        /// Returns the <see cref="AggregateException"/> for anything that went
        /// wrong making the call 
        /// </summary>
        public AggregateException Exception
        {
            get { return _task.Exception; }
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
            if (IsError)
            {
                if (_onFailureCallback != null)
                    _onFailureCallback(Exception);
            }
            else
            {
                if (_onCompleteCallback != null)
                    _onCompleteCallback(Image);
            }        
        }
    }
}
