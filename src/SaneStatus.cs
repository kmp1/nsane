namespace NSane
{
    /// <summary>
    /// SANE error codes enumeration
    /// </summary>
    public enum SaneStatus
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Not an error - success
        /// </summary>
        Success = 0,

        /// <summary>
        /// Unsupported call
        /// </summary>
        Unsupported = 1,

        /// <summary>
        /// Call canceled
        /// </summary>
        Canceled = 2,

        /// <summary>
        /// Device is busy
        /// </summary>
        DeviceBusy = 3,

        /// <summary>
        /// Invalid call
        /// </summary>
        Invalid = 4,

        /// <summary>
        /// End of file
        /// </summary>
        EndOfFile = 5,

        /// <summary>
        /// Scanner jammed
        /// </summary>
        Jammed = 6,

        /// <summary>
        /// No documents
        /// </summary>
        NoDocuments = 7,

        /// <summary>
        /// The cover is open
        /// </summary>
        CoverOpen = 8,

        /// <summary>
        /// I/O error
        /// </summary>
// ReSharper disable InconsistentNaming
        IOError = 9,
// ReSharper restore InconsistentNaming

        /// <summary>
        /// Out of memory
        /// </summary>
        OutOfMemory = 10,

        /// <summary>
        /// Access is denied
        /// </summary>
        AccessDenied = 11
    }
}