using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace NSane
{
    /// <summary>
    /// The job of this class is to data the raw binary data for an "image"
    /// that has been returned by the SANE API and convert it to something that
    /// is useful to .NET developers (i.e. a <see cref="Bitmap"/> object)
    /// </summary>
    internal static class ImageCreator
    {
        /// <summary>
        /// Convert the given stream into a bitmap
        /// </summary>
        /// <param name="stream">The stream to convert</param>
        /// <param name="bytesPerLine">The bytes per line</param>
        /// <param name="pixelsPerLine">The pixels per line</param>
        /// <param name="lines">The number of lines</param>
        /// <param name="depth">THe depth</param>
        /// <param name="littleEndian"><c>true</c> if the values are little
        /// endian</param>
        /// <param name="color"><c>true</c> if it is a color bitmap</param>
        /// <returns>A <see cref="Bitmap"/> containing the resulting
        /// image</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
                "If I did that, this whole thing would break!")]
        internal static Bitmap ToBitmap(MemoryStream stream,
                                        int bytesPerLine,
                                        int pixelsPerLine,
                                        int lines,
                                        int depth,
                                        bool littleEndian,
                                        bool color)
        {

            var data = stream.ToArray();

            Bitmap ret;
            if (depth == 1)
            {
                ret = ToBitmap1(lines, bytesPerLine, pixelsPerLine, data);
            }
            else if (depth == 8)
            {
                if (color)
                {
                    ret = ToRgbBitmap8(lines,
                                       bytesPerLine,
                                       pixelsPerLine,
                                       data);
                }
                else
                {
                    ret = ToGrayBitmap8(lines,
                                        bytesPerLine,
                                        pixelsPerLine,
                                        data);
                }
            }
            else
            {
                using (var f = File.Create("c:\\data16.bin"))
                {
                    f.Write(data, 0, data.Length);
                }

                throw new NotImplementedException("LE: " + littleEndian);
            }

            return ret;
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
        /// <param name="bytesPerLine">The count of bytes per line</param>
        /// <param name="pixelsPerLine">The count of pixels per line</param>
        /// <param name="data">The data</param>
        /// <returns>A newly created <see cref="Bitmap"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability", 
            "CA2000:Dispose objects before losing scope", Justification = 
            "If I disposed of it, it just wouldn't work now would it!?")]
        private static Bitmap ToBitmap1(int lines,
                                        int bytesPerLine,
                                        int pixelsPerLine,
                                        byte[] data)
        {
            var bmp = new Bitmap(pixelsPerLine,
                                 lines,
                                 PixelFormat.Format1bppIndexed);

            CreateBlackAndWhitePalette(bmp);

            CopyIntoBitmap(bmp, data, bytesPerLine);

            return bmp;
        }

        /// <summary>
        /// Convert the data into a bitmap using a depth of 8 (grayscale)
        /// </summary>
        /// <param name="lines">The number of lines</param>
        /// <param name="bytesPerLine">The count of bytes per line</param>
        /// <param name="pixelsPerLine">The count of pixels per line</param>
        /// <param name="data">The data</param>
        /// <returns>A newly created <see cref="Bitmap"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
            "If I disposed of it, it just wouldn't work now would it!?")]
        private static Bitmap ToGrayBitmap8(int lines,
                                            int bytesPerLine,
                                            int pixelsPerLine,
                                            byte[] data)
        {
            var bmp = new Bitmap(pixelsPerLine,
                                 lines,
                                 PixelFormat.Format8bppIndexed);

            CreateGrayPalette(bmp);

            CopyIntoBitmap(bmp, data, bytesPerLine);

            return bmp;
        }

        /// <summary>
        /// Convert the data into a bitmap using a depth of 8 (color)
        /// </summary>
        /// <param name="lines">The number of lines</param>
        /// <param name="bytesPerLine">The count of bytes per line</param>
        /// <param name="pixelsPerLine">The count of pixels per line</param>
        /// <param name="data">The data</param>
        /// <returns>A newly created <see cref="Bitmap"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification =
            "If I disposed of it, it just wouldn't work now would it!?")]
        private static Bitmap ToRgbBitmap8(int lines,
                                           int bytesPerLine,
                                           int pixelsPerLine,
                                           byte[] data)
        {
            SwapRedAndBlue8(data);

            var bmp = new Bitmap(pixelsPerLine,
                                 lines,
                                 PixelFormat.Format24bppRgb);

            CopyIntoBitmap(bmp, data, bytesPerLine);

            return bmp;
        }

        /// <summary>
        /// The order is not red,green,blue but blue,green,red despite the
        /// name of the class!
        /// </summary>
        /// <param name="data"></param>
        private static void SwapRedAndBlue8(byte[] data)
        {
            for(int i = 2; i < data.Length; i+=3)
            {
                var r = data[i - 2];
                var b = data[i];
                data[i] = r;
                data[i - 2] = b;
            }
        }

        /// <summary>
        /// Copy the given data into a bitmap object
        /// </summary>
        /// <param name="bmp">The bitmap to copy into</param>
        /// <param name="data">The data to copy</param>
        /// <param name="bytesPerLine">The bytes per line</param>
        private static void CopyIntoBitmap(Bitmap bmp,
                                           byte[] data, 
                                           int bytesPerLine)
        {
            BitmapData bmpData = bmp
                .LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                          ImageLockMode.ReadWrite,
                          bmp.PixelFormat);

            MarshalBitmapData(bmpData, bmp.Height, bytesPerLine, data);

            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// Copies each row of data into the bitmap (the rows may require
        /// padding due to the bitmap format)
        /// </summary>
        /// <param name="bmpData">The data object</param>
        /// <param name="lines">How many lines there are</param>
        /// <param name="bytesPerLine">Number of bytes per line</param>
        /// <param name="data">The data</param>
        private static void MarshalBitmapData(BitmapData bmpData,
                                              int lines,
                                              int bytesPerLine,
                                              byte[] data)
        {
            
            for (int i = 0; i < lines; i++)
            {
                IntPtr offset = bmpData.Scan0 + i*bmpData.Stride;
                Marshal.Copy(data, i*bytesPerLine, offset, bytesPerLine);

                int padding = bmpData.Stride - bytesPerLine;
                if (padding > 0)
                {
                    var pad = new byte[padding];
                    Marshal.Copy(pad, 0, offset + bytesPerLine, padding);
                }
            }
        }

        /// <summary>
        /// Creates a black and white color palette
        /// </summary>
        /// <param name="bmp">The bitmap to set the palette of</param>
        private static void CreateBlackAndWhitePalette(Bitmap bmp)
        {
            ColorPalette palette = bmp.Palette;
            palette.Entries[0] = Color.White;
            palette.Entries[1] = Color.Black;
            bmp.Palette = palette;
        }

        /// <summary>
        /// Creates a grayscale color palette
        /// </summary>
        /// <param name="bmp">The bitmap to create the palette of</param>
        private static void CreateGrayPalette(Bitmap bmp)
        {
            var palette = bmp.Palette;
            var entries = palette.Entries;
            for (int i = 0; i < 256; i++)
            {
                entries[i] = Color.FromArgb(255, i, i, i);
            }
            bmp.Palette = palette;
        }
    }
}
