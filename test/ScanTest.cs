using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace NSane.Tests
{

    [TestFixture]
    public class ScanTest
    {
        
        [Test, TestCaseSource(typeof (ScanTestCaseFactory), "ScanTestCases")]
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

                    bool match;
                    using (var bmp = res.Image)
                    {
                        match = ImagesMatch(bmp, outFile);

                        if (!match)
                        {
                            bmp.Save(@"c:\\nsane_test_output\\failures\\" + outFile + ".bmp", ImageFormat.Bmp);
                        }                     
                    }

                    Assert.That(match,
                                    Is.True,
                                    "Image does not match reference");
                }
            }
        }

        private bool ImagesMatch(Bitmap ours, string fileName)
        {
            using (var theirs = LoadReference(fileName))
            {
                if (ours.Height != theirs.Height ||
                    ours.Width != theirs.Width)
                    return false;

                for (int y = 0; y < theirs.Height; y++)
                {
                    for (int x = 0; x < theirs.Width; x++)
                    {
                        var theirPixel = theirs.GetPixel(x, y);
                        var myPixel = ours.GetPixel(x, y);

                        if (theirPixel != myPixel)
                            return false;
                    }
                }
            }

            return true;
        }

        private Bitmap LoadReference(string fileName)
        {
            return new Bitmap(Assembly
                                  .GetExecutingAssembly()
                                  .GetManifestResourceStream(
                                      "NSane.Tests.images." + fileName + ".tiff"));
        }
    }
}
