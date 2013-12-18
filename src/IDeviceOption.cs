namespace NSane
{
    /// <summary>
    /// An option for a device
    /// </summary>
    public interface IDeviceOption
    {
        /// <summary>
        /// The name of the option
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The title of the option
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The description of the option
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the constraint for the device (may be none)
        /// </summary>
        IOptionConstraint Constraint { get; }

        /// <summary>
        /// The "unit" (i.e. pixels etc) for the option
        /// </summary>
        SaneUnit Unit { get; }

        /// <summary>
        /// The full capabiilties of the device (this is a flags enumeration)
        /// </summary>
        SaneCapabilities Capabilities { get; }

        /// <summary>
        /// The type of option
        /// </summary>
        SaneType Type { get; }

        /// <summary>
        /// <c>true</c> if the device is active
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// <c>false</c> if the device is read only
        /// </summary>
        bool IsSettable { get; }

        /// <summary>
        /// <c>true</c> if the value can be set to "automatic"
        /// </summary>
        bool IsAutomaticAllowed { get; }

        /// <summary>
        /// Gets or sets the value of the option.
        /// </summary>
        /// <remarks>
        /// This is a <c>dynamic</c> type, which means that, depending on the
        /// option it will be a different value (i.e. some options are integers,
        /// some strings etc).  In order to use it, the best approach is to 
        /// directly assign to the correct type, for example, if you are working
        /// with an option that is an integer simply do:
        /// <code>
        /// int val = opt.Value;
        /// </code>
        /// </remarks>
        dynamic Value { get; set; }

        /// <summary>
        /// Set this option to automatic mode
        /// </summary>
        void SetToAutomatic();
    }
}
