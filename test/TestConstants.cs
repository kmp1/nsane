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
        internal const string SaneDaemonHost = "10.0.0.3";

        /// <summary>
        /// The port of the SANE daemon - change to yours
        /// </summary>
        internal const string SaneDaemonPort = "6566";

        /// <summary>
        /// This is a directory that can be used to save the images in case of
        /// test failures
        /// </summary>
        internal const string FailedTestOutputFolder =
            @"C:\nsane_test_output\failures";

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
