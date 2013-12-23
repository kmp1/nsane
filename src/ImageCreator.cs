using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NSane
{
    /// <summary>
    /// The job of this class is to data the raw binary data for an "image"
    /// that has been returned by the SANE API and convert it to something that
    /// is useful to .NET developers (i.e. a <see cref="BitmapSource"/> object)
    /// </summary>
    internal static class ImageCreator
    {
        private static readonly Color[] GrayPalette = CreateGrayPalette();
        
        private static readonly Color[] BlackAndWhitePalette
            = CreateBlackAndWhitePalette();

        /// <summary>
        /// Convert the given stream into a bitmap
        /// </summary>
        /// <param name="data">The data to convert</param>
        /// <param name="pixelsPerLine">The pixels per line</param>
        /// <param name="lines">The number of lines</param>
        /// <param name="depth">THe depth</param>
        /// <param name="littleEndian"><c>true</c> if the values are little
        /// endian</param>
        /// <param name="color"><c>true</c> if it is a color bitmap</param>
        /// <returns>A <see cref="BitmapSource"/> containing the resulting
        /// image</returns>
        internal static BitmapSource ToBitmap(byte[] data,
                                              int pixelsPerLine,
                                              int lines,
                                              int depth,
                                              bool littleEndian,
                                              bool color)
        {
            BitmapFrame ret;
            if (depth == 1)
            {
                ret = ToBitmap1(lines, pixelsPerLine, data);
            }
            else if (depth == 8)
            {
                ret = color
                    ? ToRgbBitmap8(lines, pixelsPerLine, data)
                    : ToGrayBitmap8(lines, pixelsPerLine, data);
            }
            else
            {
                if (!littleEndian)
                    data = SwapPairs(data).ToArray();
                    
                ret = color 
                    ? ToRgbBitmap16(pixelsPerLine, lines, data) 
                    : ToGrayBitmap16(pixelsPerLine, lines, data);
            }
            
            ret.Freeze();

            return ret;
        }

        /// <summary>
        /// Swaps pairs of bytes around
        /// </summary>
        /// <param name="data">The data to swap</param>
        /// <returns></returns>
        private static IEnumerable<byte> SwapPairs(byte[] data)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                var b1 = data[i];
                if (i + 1 < data.Length)
                {
                    var b2 = data[i + 1];
                    yield return b2;
                }
                yield return b1;
            }
        }

        /// <summary>
        /// Convert the data into a bitmap using a depth of 1
        /// </summary>
        /// <remarks>
        /// This should be called for Gray 1 and RGB 1 which is "rarely used
        /// and may not be supported by every frontend" so I think it's
        /// reasonable just to handle it the same
        /// </remarks>
        /// <param name="lines">The number of lines</param>
        /// <param name="pixelsPerLine">The count of pixels per line</param>
        /// <param name="data">The data</param>
        /// <returns>A newly created <see cref="BitmapSource"/></returns>       
        private static BitmapFrame ToBitmap1(int lines,
                                             int pixelsPerLine,
                                             byte[] data)
        {
            var wb = new WriteableBitmap(pixelsPerLine, 
                                         lines,
                                         50.0, 
                                         50.0, 
                                         PixelFormats.Indexed1,
                                         new BitmapPalette(BlackAndWhitePalette));
            
            CopyIntoBitmap(wb, data);

            return BitmapFrame.Create(wb);
        }

        /// <summary>
        /// Convert the data into a bitmap using a depth of 8 (grayscale)
        /// </summary>
        /// <param name="lines">The number of lines</param>
        /// <param name="pixelsPerLine">The count of pixels per line</param>
        /// <param name="data">The data</param>
        /// <returns>A newly created <see cref="BitmapSource"/></returns>
        private static BitmapFrame ToGrayBitmap8(int lines,
                                                 int pixelsPerLine,
                                                 byte[] data)
        {
            var wb = new WriteableBitmap(pixelsPerLine,
                                         lines,
                                         50.0,
                                         50.0,
                                         PixelFormats.Indexed8,
                                         new BitmapPalette(GrayPalette));

            CopyIntoBitmap(wb, data);

            return BitmapFrame.Create(wb);
        }

        /// <summary>
        /// Convert the data into a bitmap using a depth of 8 (color)
        /// </summary>
        /// <param name="lines">The number of lines</param>
        /// <param name="pixelsPerLine">The count of pixels per line</param>
        /// <param name="data">The data</param>
        /// <returns>A newly created <see cref="BitmapSource"/></returns>
        private static BitmapFrame ToRgbBitmap8(int lines,
                                                int pixelsPerLine,
                                                byte[] data)
        {
            var wb = new WriteableBitmap(pixelsPerLine,
                                         lines,
                                         50.0,
                                         50.0,
                                         PixelFormats.Rgb24,
                                         null);

            CopyIntoBitmap(wb, data);

            return BitmapFrame.Create(wb);
        }
        
        /// <summary>
        /// Convert to an RGB 16 bitmap
        /// </summary>
        /// <param name="pixelsPerLine"></param>
        /// <param name="lines"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static BitmapFrame ToRgbBitmap16(int pixelsPerLine, 
                                                 int lines, 
                                                 byte[] data)
        {
            var format = PixelFormats.Rgb48;
            int stride = (pixelsPerLine*format.BitsPerPixel + 7)/8;
            const double dpi = 50.0;

            var wb = new WriteableBitmap(pixelsPerLine, lines, 
                dpi, dpi, format, null);

            wb.WritePixels(new Int32Rect(0, 0, pixelsPerLine, lines),
                           data,
                           stride,
                           0);
            
            return BitmapFrame.Create(wb);
        }

        /// <summary>
        /// Convert to an RGB 16 bitmap
        /// </summary>
        /// <param name="pixelsPerLine"></param>
        /// <param name="lines"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static BitmapFrame ToGrayBitmap16(int pixelsPerLine,
                                                  int lines,
                                                  byte[] data)
        {
            var format = PixelFormats.Gray16;
            int stride = (pixelsPerLine * format.BitsPerPixel + 7) / 8;
            const double dpi = 50.0;

            var wb = new WriteableBitmap(pixelsPerLine, lines,
                dpi, dpi, format, null);

            wb.WritePixels(new Int32Rect(0, 0, pixelsPerLine, lines),
                           data,
                           stride,
                           0);

            return BitmapFrame.Create(wb);
        }

        /// <summary>
        /// Copy the given data into a bitmap object
        /// </summary>
        /// <param name="bmp">The bitmap to copy into</param>
        /// <param name="data">The data to copy</param>
        private static void CopyIntoBitmap(WriteableBitmap bmp,
                                           byte[] data)
        {
            int stride = (bmp.PixelWidth * bmp.Format.BitsPerPixel + 7) / 8;
            bmp.WritePixels(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight),
                data,
                stride,
                0);
        }
        
        /// <summary>
        /// Creates a black and white color palette
        /// </summary>
        private static Color[] CreateBlackAndWhitePalette()
        {
            var palette = new []
            {
                Colors.White,
                Colors.Black
            };
            return palette;
        }
        
        /// <summary>
        /// Creates a grayscale color palette
        /// </summary>
        private static Color[] CreateGrayPalette()
        {
            var entries = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                var b = (byte)i;
                entries[i] = Color.FromArgb(255, b, b, b);
            }      
            return entries;
        }
    }
}
