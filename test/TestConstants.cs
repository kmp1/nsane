namespace NSane.Tests
{
    /// <summary>
    /// Constants for the tests - some of these will need to be configured
    /// for the tests to work
    /// </summary>
    internal static class TestConstants
    {
        /// <summary>
        /// The host IP address of the SANE daemon - change to yours
        /// </summary>
        internal const string SaneDaemonHost = "127.0.0.1";

        /// <summary>
        /// The port of the SANE daemon - change to yours
        /// </summary>
        internal const string SaneDaemonPort = "6566";

		/// <summary>
		/// The un authenticated device.
		/// </summary>The unauthenticate device name
        internal const string UnAuthenticatedDevice = "test:0";

		/// <summary>
		/// The sane daemon path.
		/// </summary>
        internal const string SaneDaemon =
           SaneDaemonHost + ":" + SaneDaemonPort;
    }
}
