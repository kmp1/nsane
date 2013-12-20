namespace NSane
{
    /// <summary>
    /// This is a device that can be opened
    /// </summary>
    public interface IOpenableDevice : IDevice
    {
        /// <summary>
        /// Open the device
        /// </summary>
        /// <returns>The opened device</returns>
        IOpenedDevice Open();
    }
}
