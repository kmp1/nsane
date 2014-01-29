using System.Runtime.InteropServices;

namespace NSane.Local
{
    /// <summary>
    /// The parameters returned by the SANE native API
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct SaneParameters
    {
        /// <summary>
        /// The format
        /// </summary>
        [MarshalAs(UnmanagedType.SysInt), FieldOffset(0)]
        public FrameFormat Format;

        /// <summary>
        /// The last frame
        /// </summary>
        [FieldOffset(4)]
        public int LastFrame;
        
        /// <summary>
        /// The number of bytes per line
        /// </summary>
        [FieldOffset(8)]
        public int BytesPerLine;

        /// <summary>
        /// The number of pixels per line
        /// </summary>
        [FieldOffset(12)]
        public int PixelsPerLine;
        
        /// <summary>
        /// The number of lines
        /// </summary>
        [FieldOffset(16)]
        public int Lines;
        
        /// <summary>
        /// The color depth (1,8 or 16)
        /// </summary>
        [FieldOffset(20)]
        public int Depth;
    }
}
