using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

using NSane.L10n;

namespace NSane
{
    /// <summary>
    /// This exception is thrown from many of the operations invoking the SANE
    /// system. 
    /// </summary>
    [Serializable]
    public class NSaneException : Exception
    {
        /// <summary>
        /// Default constructor - no message
        /// </summary>
        public NSaneException()
        {
        }

        /// <summary>
        /// Construct with a message
        /// </summary>
        /// <param name="message"></param>
        public NSaneException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construct with a message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NSaneException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        /// Construct for serilization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NSaneException(SerializationInfo info,
                                 StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the status that caused this exception to be raised
        /// </summary>
        public SaneStatus Status { get; private set; }

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand,
            SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info,
                                           StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("Status", Status);

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Create from a status value
        /// </summary>
        /// <param name="status">The status value returned from SANE</param>
        /// <returns></returns>
        internal static NSaneException CreateFromStatus(int status)
        {
            // If status is 0 - it is a bug in this API since it is not an 
            // exception
            // ReSharper disable LocalizableElement
            if (status == 0)
                throw new ArgumentOutOfRangeException(
                    "status",
                    "The status is 0 which is not an error so why are "
                    + "you calling this method?");
            // ReSharper restore LocalizableElement

            // TODO: Get all the status messages from the resx
            switch (status)
            {
                case 1:
                    return new NSaneException(Strings.EXCEPTION_UNSUPPORTED)
                        {
                            Status = SaneStatus.Unsupported
                        };
                case 2:
                    return new NSaneException(Strings.EXCEPTION_CANCELED)
                        {
                            Status = SaneStatus.Canceled
                        };
                case 3:
                    return new NSaneException(Strings.EXCEPTION_DEVICE_BUSY)
                        {
                            Status = SaneStatus.DeviceBusy
                        };
                case 4:
                    return new NSaneException(Strings
                        .EXCEPTION_INVALID_OPERATION)
                        {
                            Status = SaneStatus.Invalid
                        };
                case 5:
                    return new NSaneException(Strings.EXCEPTION_END_OF_FILE)
                        {
                            Status = SaneStatus.EndOfFile
                        };
                case 6:
                    return new NSaneException(Strings.EXCEPTION_PAPER_JAMMED)
                        {
                            Status = SaneStatus.Jammed
                        };
                case 7:
                    return new NSaneException(Strings.EXCEPTION_NO_DOCUMENT)
                        {
                            Status = SaneStatus.NoDocuments
                        };
                case 8:
                    return new NSaneException(Strings.EXCEPTION_COVER_OPEN)
                        {
                            Status = SaneStatus.CoverOpen
                        };
                case 9:
                    return new NSaneException(Strings.EXCEPTION_IO_ERROR)
                        {
                            Status = SaneStatus.IOError
                        };
                case 10:
                    return new NSaneException(Strings.EXCEPTION_OUT_OF_MEMORY)
                        {
                            Status = SaneStatus.OutOfMemory
                        };
                case 11:
                    return new NSaneException(Strings.EXCEPTION_ACCESS_DENIED)
                        {
                            Status = SaneStatus.AccessDenied
                        };
                default:
                    return new NSaneException(Strings.EXCEPTION_UNKNOWN)
                        {
                            Status = SaneStatus.Unknown
                        };
            }
        }
    }
}
