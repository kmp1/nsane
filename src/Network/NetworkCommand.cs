namespace NSane.Network
{
    /// <summary>
    /// This enumeration stores all the commands that can be performed via the
    /// network API of SANE.
    /// </summary>
    internal enum NetworkCommand
    {
        /// <summary>
        /// Initialize the connection to the saned
        /// </summary>
        Initialize = 0,

        /// <summary>
        /// Get the available devices 
        /// </summary>
        GetDevices = 1,

        /// <summary>
        /// Open the connection to a device
        /// </summary>
        Open = 2,

        /// <summary>
        /// Closes the connection to a device
        /// </summary>
        Close = 3,

        /// <summary>
        /// Gets the option descriptors
        /// </summary>
        GetOptionDescriptors = 4,

        /// <summary>
        /// Gets or sets control options
        /// </summary>
        ControlOption = 5,

        /// <summary>
        /// Get the scanning parameters
        /// </summary>
        GetParameters = 6,

        /// <summary>
        /// Starts the scanning process
        /// </summary>
        Start = 7,

        /// <summary>
        /// Cancels the current operation (it may or may not cancel straight
        /// away)
        /// </summary>
        Cancel = 8,
        
        /// <summary>
        /// The authorize command
        /// </summary>
        Authorize = 9,

        /// <summary>
        /// Exit the network connection
        /// </summary>
        Exit = 10
    }
}
