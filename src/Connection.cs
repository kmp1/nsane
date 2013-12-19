using System;
using System.Globalization;

using NSane.Network;

namespace NSane
{
    /// <summary>
    /// This is the entry point int the API - use this to get hold of a
    /// connection and from there you can start interacting with the scanner
    /// </summary>
    /// <remarks>
    /// The idea behind this is that if, at any time, we need to add in a
    /// non-network version of the connection (i.e. if I ever get around to
    /// getting Mono working and if it can pinvoke into the native SANE dll)
    /// I could simply add a method (or two) here to construct the appropriate
    /// <see cref="IConnection"/> implementation.
    /// </remarks>
    public static class Connection
    {
        /// <summary>
        /// The SANE version code
        /// </summary>
        private static readonly int SaneVersion = CreateVersionCode(1, 0, 3);

#if Mono
        /// <summary>
        /// Makes a connection to a local SANE instance without any
        /// credentials.
        /// </summary>
        public static IConnection Local()
        {
            return Local (null, null);
        }

        /// <summary>
        /// Makes a connection to a local SANE instance with some
        /// credentials.
        /// </summary>
        /// <param name='userName'>
        /// User name.
        /// </param>
        /// <param name='password'>
        /// Password.
        /// </param>
        public static IConnection Local(string userName, string password)
        {
            return new LocalConnection(userName, password);
        }
#endif
        /// <summary>
        /// Create a connection at the given address (host:port)
        /// </summary>
        /// <param name="address">The address (host:port)</param>
        /// <returns>A connection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
                "If I did that, this whole thing would break!")]
        public static IConnection At(string address)
        {
            return At(address, Environment.UserName, null);
        }

        /// <summary>
        /// Create a connection at the given address (host:port) with the
        /// credentials.
        /// </summary>
        /// <param name="address">The address (host:port)</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>A connection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
                "If I did that, this whole thing would break!")]
        public static IConnection At(string address,
                                     string userName,
                                     string password)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("address");
            }

            string[] ep = address.Split(':');
            if (ep.Length < 2)
            {
                throw new ArgumentException("Invalid address format");
            }

            string host = ep.Length > 2
                              ? string.Join(":", ep, 0, ep.Length - 1)
                              : ep[0];

            int port;
            if (!int.TryParse(ep[ep.Length - 1],
                              NumberStyles.None,
                              NumberFormatInfo.CurrentInfo,
                              out port))
            {
                throw new ArgumentException("Invalid port");
            }

            return At(host, port, userName, password);
        }

        /// <summary>
        /// Create a connection at the given host and port
        /// </summary>
        /// <param name="host">The host</param>
        /// <param name="port">The port</param>
        /// <returns>A connection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
                "If I did that, this whole thing would break!")]
        public static IConnection At(string host, int port)
        {
            return At(host, port, Environment.UserName, null);
        }

        /// <summary>
        /// Create a connection at the given host and port with the credentials
        /// </summary>
        /// <param name="host">The host</param>
        /// <param name="port">The port</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>A connection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
                "If I did that, this whole thing would break!")]
        public static IConnection At(string host,
                                     int port,
                                     string userName,
                                     string password)
        {
            return new NetworkConnection(host,
                                         port,
                                         SaneVersion,
                                         userName,
                                         password);
        }

        /// <summary>
        /// Creates the SANE version code to send when initialising
        /// </summary>
        /// <param name="major">The major version</param>
        /// <param name="minor">The minor version</param>
        /// <param name="build">The build number</param>
        /// <returns>The combined code</returns>
        private static int CreateVersionCode(int major, int minor, int build)
        {
            int value = (major & 0xff) << 24;
            value += (minor & 0xff) << 16;
            value += (build & 0xffff) << 0;
            return value;
        }
    }
}
