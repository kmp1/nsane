using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace NSane
{
    /// <summary>
    /// Extension methods for <see cref="BitmapSource"/> objects
    /// </summary>
    public static class BitmapSourceExtensions
    {
        /// <summary>
        /// Convert a <see cref="BitmapSource"/> to a <see cref="BitmapImage"/>
        /// </summary>
        /// <param name="source">The original source</param>
        /// <returns>A <see cref="BitmapImage"/></returns>
        public static BitmapImage ToBitmapImage(this BitmapSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var encoder = new TiffBitmapEncoder
                {
                    Compression = TiffCompressOption.None
                };

            var ret = new BitmapImage();
            using (var memoryStream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(memoryStream);

                ret.BeginInit();
                ret.StreamSource = new MemoryStream(memoryStream.ToArray());
                ret.EndInit();
                ret.Freeze();
            }
            return ret;
        }
    }
}
