using System;

namespace NSane
{
    /// <summary>
    /// This is a base class to aid implementing the IDisposable pattern. 
    /// </summary>
    /// <remarks>
    /// It is based on the MSDN article here: 
    /// http://msdn.microsoft.com/en-us/library/system.idisposable.aspx
    /// </remarks>
    internal abstract class DisposableObject : IDisposable
    {
        /// <summary>
        /// Finalizer for the disposable object
        /// </summary>
        ~DisposableObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this method to handle disposing of the object.  
        /// </summary>
        /// <param name="disposing">
        /// If thIS argument is true, it is being explicitly disposed and if 
        /// false, it is being garbage collected:
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
