using System.Linq;
using NUnit.Framework;

namespace NSane.Tests
{
    [TestFixture]
    public class ConnectionTest
    {
        [Test]
        public void Connection_Everthing_IsDisposed()
        {
            Assert.DoesNotThrow(() =>
                {
                    using (var connection = Connection.At(TestConstants.SaneDaemon))
                    {
                        Assert
                            .That(connection.Version,
                                  Is.GreaterThan(0),
                                  "Connection is not opened");


                        using (var d = connection.OpenDevice(TestConstants.UnAuthenticatedDevice))
                        {

                        }

                        var de = connection.AllDevices.First(d => d.Name.Equals(TestConstants.UnAuthenticatedDevice));

                        using (var d = de.Open())
                        {

                        }
                    }
                });
        }

        [Test]
        public void Connection_OpenWithFulleNetworkString_Succeeds()
        {
            using (var connection =  Connection.At(TestConstants.SaneDaemon))
            {
                Assert
                    .That(connection.Version,
                          Is.GreaterThan(0),
                          "Connection is not opened");
            }
        }
    }
}
