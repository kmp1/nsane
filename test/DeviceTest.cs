using System;
using System.Linq;

using NUnit.Framework;

namespace NSane.Tests
{
    [TestFixture]
    public class DeviceTest
    {
        [Test]
        public void Device_GetAll_Succeeds()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                Assert
                    .That(connection.AllDevices.Count(),
                          Is.GreaterThan(0),
                          "No devices discovered");
            }
        }

        [Test]
        public void Device_GetAllContainsTest_Succeeds()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                var testDevices =
                    connection
                        .AllDevices
                        .Where(d => d
                                        .Name
                                        .StartsWith("test",
                                                    StringComparison.OrdinalIgnoreCase));
                
                Assert
                    .That(testDevices.Count(),
                          Is.GreaterThan(0),
                          "No devices discovered");
            }
        }

        [Test]
        public void Device_Open_Succeeds()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                var testDevice =
                    connection
                        .AllDevices
                        .First(d => d
                                        .Name
                                        .Equals(TestConstants.UnAuthenticatedDevice,
                                                    StringComparison.OrdinalIgnoreCase));
                
                using (var dev = testDevice.Open())
                {
                    Assert
                        .That(dev,
                              Is.Not.Null,
                              "Device could not be opened");
                }
            }
        }
    }
}
