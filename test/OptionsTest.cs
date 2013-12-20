using System;
using System.Linq;

using NUnit.Framework;

namespace NSane.Tests
{
    [TestFixture]
    public class OptionsTest
    {
        [Test]
        public void DeviceOption_ButtonOption_CanBeReadAndSet()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                using (var d = connection
                    .OpenDevice(TestConstants.UnAuthenticatedDevice))
                {
                   
                    Assert.DoesNotThrow(() =>
                        {
                            var setbut = d
                                .AllOptions
                                .First(o => o.IsSettable &&
                                            o.Type == SaneType.Button);

                            var before = setbut.Value;
                            Assert.That(before, Is.False);

                            setbut.Value = true;
                            setbut.Value = before;

                        });
                }
            }
        }

        [Test]
        public void DeviceOption_FixedOption_CanBeReadAndSet()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon, "test", "testpwd"))
            {
                using (var d = connection
                    .OpenDevice(TestConstants.UnAuthenticatedDevice))
                {
                    var resolution = d.AllOptions
                        .First(o => o.Name.Equals("resolution", 
                            StringComparison.OrdinalIgnoreCase));

                    double valu = resolution.Value;

                    Assert.That(valu, Is.Not.EqualTo(0));

                    resolution.Value = 1200.0;
                }
            }
        }

        [Test]
        public void Device_HasOptions_Succeeds()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                var testDevice =
                    connection
                        .AllDevices
                        .First(d => d
                                        .Name
                                        .Equals(TestConstants.UnAuthenticatedDevice,
                                                StringComparison
                                                    .OrdinalIgnoreCase));
                
                using (var dev = testDevice.Open())
                {
                    var opts = dev.AllOptions;

                    Assert.That(opts.Any(), Is.True, "No device options");
                }
            }
        }

        [Test]
        public void DeviceOption_First_IsTheCorrectTotalCount()
        {
            // The first "option" tells us how many options (including itself
            // there are, so it is very handy for a test)...
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                var testDevice =
                    connection
                        .AllDevices
                        .First(d => d
                                        .Name
                                        .Equals(TestConstants.UnAuthenticatedDevice,
                                                StringComparison
                                                    .OrdinalIgnoreCase));

                using (var dev = testDevice.Open())
                {
                    var opts = dev.AllOptions.ToList();
                    var first = opts.First();
                    int totalOptionCount = first.Value;

                    Assert
                        .That(totalOptionCount,
                              Is.EqualTo(opts.Count()),
                              "The first option's value is either not correct "
                              + "or the collection of options is wrong - they "
                              + "should match");
                }
            }
        }

        [Test]
        public void DeviceOption_GetString_Succeeds()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                var testDevice =
                    connection
                        .AllDevices
                        .First(d => d
                                        .Name
                                        .Equals(TestConstants.UnAuthenticatedDevice,
                                                StringComparison
                                                    .OrdinalIgnoreCase));

                using (var dev = testDevice.Open())
                {
                    var modes = dev.AllOptions.Where(n =>
                        n.Name.Equals("mode",
                                      StringComparison.OrdinalIgnoreCase));

                    var mode = modes.First();

                    string modeValue = mode.Value;

                    Assert.That(modeValue,
                                Is.EqualTo(""),
                                "Mode not empty");

                    mode.Value = "Gray";

                    modeValue = mode.Value;
                    Assert.That(modeValue,
                                Is.EqualTo("Gray"),
                                "Mode not empty");

                }
            }
        }

        [Test]
        public void DeviceOption_GetBoolean_Succeeds()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                var testDevice =
                    connection
                        .AllDevices
                        .First(d => d
                                        .Name
                                        .Equals(TestConstants.UnAuthenticatedDevice,
                                                StringComparison
                                                    .OrdinalIgnoreCase));

                using (var dev = testDevice.Open())
                {
                    var hand = dev.AllOptions.First(n =>
                        n.Name.Equals("hand-scanner"));

                    bool isThreePass = hand.Value;

                    Assert.That(isThreePass,
                                Is.False,
                                "Default 3 pass is invalid");
                }
            }
        }

        [Test]
        public void DeviceOption_GetIntegerMultipleTimes_Succeeds()
        {
            using (var connection =  Connection.At(TestConstants.SaneDaemon))
            {
                var testDevice =
                    connection
                        .AllDevices
                        .First(d => d
                                        .Name
                                        .Equals(TestConstants.UnAuthenticatedDevice,
                                                StringComparison
                                                    .OrdinalIgnoreCase));

                using (var dev = testDevice.Open())
                {
                    var depth = dev.AllOptions.First(n =>
                        n.Name.Equals("depth",
                                      StringComparison.OrdinalIgnoreCase));

                    int orig = depth.Value;
              
                    int newValue = depth.Value;

                    Assert.That(newValue, Is.EqualTo(orig));
                }
            }   
        }

        [Test]
        public void DeviceOption_SetInteger_Succeeds()
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                var testDevice =
                    connection
                        .AllDevices
                        .First(d => d
                                        .Name
                                        .Equals(TestConstants.UnAuthenticatedDevice,
                                                StringComparison
                                                    .OrdinalIgnoreCase));

                using (var dev = testDevice.Open())
                {
                    var depth = dev.AllOptions.First(n =>
                        n.Name.Equals("depth", 
                                      StringComparison.OrdinalIgnoreCase));

                    int orig = depth.Value;
                    int target = orig == 16 ? 8 : 16;

                    depth.Value = target;

                    int newValue = depth.Value;

                    Assert.That(newValue, Is.EqualTo(target));
                }
            }
        }
    }
}
