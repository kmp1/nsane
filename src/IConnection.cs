using System;
using System.Collections.Generic;

namespace NSane
{
    /// <summary>
    /// The interface for the connection
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Gets the version number of the SANE API
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets  all the devices
        /// </summary>
        IEnumerable<IOpenableDevice> AllDevices { get; }

        /// <summary>
        /// Opens the device with the name
        /// </summary>
        /// <param name="name">The name of the device to open</param>
        /// <returns></returns>
        IOpenedDevice OpenDevice(string name);
    }
}
