using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;

using NUnit.Framework;

namespace NSane.Tests
{

    [TestFixture, RequiresSTA]
    public class ScanTest
    {
        [SetUp]
        public void TestSetUp()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [Test, TestCaseSource(typeof (ScanTestCaseFactory), "ScanTestCases"), RequiresSTA]
        public void Scan_NormalPicture_Succeeds(int depth,
                                                string mode,
                                                string pattern,
                                                bool inverted,
                                                string outFile)
        {
            using (var connection = Connection.At(TestConstants.SaneDaemon))
            {
                using (var device = connection.OpenDevice(
                    TestConstants.UnAuthenticatedDevice))
                {
                    var opts = device.AllOptions.ToList();

                    var pict = opts.First(o => o.Name.Equals("test-picture"));
                    pict.Value = pattern;

                    var cdepth = opts.First(o => o.Name.Equals("depth"));
                    cdepth.Value = depth;

                    var cmode = opts.First(o => o.Name.Equals("mode"));
                    cmode.Value = mode;

                    var inv = opts.First(o => o.Name.Equals("invert-endianess"));
                    if (inv.IsActive)
                    {
                        inv.Value = inverted;
                    }
                    var res = device.Scan();

                    var ours = res.Image.ToBitmapImage();

                    var theirs = LoadReference(outFile).ToBitmapImage();
              
                    bool match = theirs.IsEqual(ours);
                    if (!match)
                    {
                        var failureFile = Path.Combine(
                            TestConstants.FailedTestOutputFolder,
                            outFile) + ".tiff";

                        var encoder = new TiffBitmapEncoder
                            {
                                Compression = TiffCompressOption.None
                            };

                        using (var f = File.Create(failureFile))
                        {
                            encoder.Frames.Add(BitmapFrame.Create(ours));
                            encoder.Save(f);
                        }
                    }                                   

                    Assert.That(match,
                                    Is.True,
                                    "Image does not match reference");
                }
            }
        }

        private BitmapSource LoadReference(string fileName)
        {
            var imageStreamSource = Assembly
                                  .GetExecutingAssembly()
                                  .GetManifestResourceStream(
                                      "NSane.Tests.images." + fileName + ".tiff");
            
            var decoder = new TiffBitmapDecoder(imageStreamSource, 
                BitmapCreateOptions.PreservePixelFormat, 
                BitmapCacheOption.Default);
            BitmapSource ret = decoder.Frames[0];
            return ret;
        }
    }
}
