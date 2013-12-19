using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NSane.Tests
{
    /// <summary>
    /// Useful little extension method to allow us to compare images
    /// </summary>
    public static class BitmapImageExtensions
    {

        /// <summary>
        /// Compare the two images
        /// </summary>
        /// <param name="image1"></param>
        /// <param name="image2"></param>
        /// <returns><C>true</C> if they match</returns>
        public static bool IsEqual(this BitmapImage image1, BitmapImage image2)
        {
            if (image1 == null && image2 == null)
                return true;

            if (image1 == null || image2 == null)
                return false;
            
            var image1Pixels = GetPixels(image1);
            var image2Pixels = GetPixels(image2);

            if (image1Pixels.Length != image2Pixels.Length)
                return false;

            var iDimensionLength = image1Pixels.GetLength(0);
            if (iDimensionLength != image1Pixels.GetLength(0))
                return false;

            var jDimensionLength = image1Pixels.GetLength(1);
            if (jDimensionLength != image2Pixels.GetLength(1))
                return false;

            for (int i = 0; i < iDimensionLength; i++)
            {
                for (int j = 0; j < jDimensionLength; j++)
                {
                    var image1Pixel = image1Pixels[i, j];
                    var image2Pixel = image2Pixels[i, j];

                    if (image1Pixel.Alpha != image2Pixel.Alpha ||
                        image1Pixel.Red != image2Pixel.Red ||
                        image1Pixel.Green != image2Pixel.Green ||
                        image1Pixel.Blue != image2Pixel.Blue)
                    {
                        return false;
                    }
                }
            }

            return true;
            
        }
        /*
         Maybe this'll come in handy - a function to convert to bytes
        public static byte[] ToBytes(this BitmapImage image)
        {
            var data = new byte[0];
            if (image != null)
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                using (var ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
                return data;
            }
            return data;
        }*/

        [StructLayout(LayoutKind.Sequential)]
        private struct PixelColor
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        private static PixelColor[,] GetPixels(BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
            {
                source = new FormatConvertedBitmap(source,
                                                   PixelFormats.Bgra32,
                                                   null,
                                                   0);
            }

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            var result = new PixelColor[width, height];

            CopyPixels(source, result, width * 4, 0);
            return result;
        }

        private unsafe static void CopyPixels(BitmapSource source, 
                                              PixelColor[,] pixels, 
                                              int stride, 
                                              int offset)
        {
            fixed (PixelColor* buffer = &pixels[0, 0])
                source.CopyPixels(
                  new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight),
                  (IntPtr)(buffer + offset),
                  pixels.GetLength(0) * pixels.GetLength(1) * sizeof(PixelColor),
                  stride);
        }
    }
}
