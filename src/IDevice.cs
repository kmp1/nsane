namespace NSane
{
    /// <summary>
    /// A scanner device
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Gets the name of the device
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the vendor of the device
        /// </summary>
        string Vendor { get; }

        /// <summary>
        /// Gets the model of the device
        /// </summary>
        string Model { get; }

        /// <summary>
        /// Get the type of the device
        /// </summary>
        string Type { get; }
    }
}
