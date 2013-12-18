using System.Collections.Generic;

namespace NSane.Network
{
    /// <summary>
    /// This is an implementation of the Connection interfaces that uses RPC
    /// calls to a SANE network daemon
    /// </summary>
    internal class NetworkConnection : DisposableObject, IConnection
    {
        private readonly NetworkProcedureCaller _caller;
        private readonly string _password;
        private readonly string _userName;

        private bool _open;
        private IEnumerable<IOpenableDevice> _devices;

        /// <summary>
        /// Construct with the host and port of the SANE daemon
        /// </summary>
        /// <param name="host">The host to connect to</param>
        /// <param name="port">The port to connect on</param>
        /// <param name="versionCode">The SANE version code</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        internal NetworkConnection(string host, 
                                   int port, 
                                   int versionCode, 
                                   string userName, 
                                   string password)
        {
            _userName = userName;
            _password = password;
            _caller = new NetworkProcedureCaller(host, port);

            Version = _caller.Initialise(_userName, versionCode);
            _open = true;
        }      

        /// <summary>
        /// Close the connection
        /// </summary>
        /// <returns>The closed connection</returns>
        private void Close()
        {
            if (_open)
            {
                _caller.Exit();
                _open = false;
            }
        }

        /// <summary>
        /// Gets the version number of the SANE API
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Gets  all the devices
        /// </summary>
        public IEnumerable<IOpenableDevice> AllDevices
        {
            get
            {
                return _devices ??
                       (_devices = _caller.RequestDeviceList(_userName,
                                                             _password));
            }
        }

        /// <summary>
        /// Opens the device with the name
        /// </summary>
        /// <param name="name">The name of the device to open</param>
        /// <returns></returns>
        public IOpenedDevice OpenDevice(string name)
        {
            int h = _caller.OpenDevice(name, _userName, _password);
            return new NetworkDevice(name, h, _caller, _userName, _password);
        }

        /// <summary>
        /// Handle disposing of this 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Close();
            _caller.Dispose();
            base.Dispose(disposing);
        }
    }
}
