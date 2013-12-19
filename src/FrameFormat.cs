namespace NSane
{
    /// <summary>
    /// This is the type of frame that has been returned from the Start
    /// command.
    /// </summary>
    internal enum FrameFormat
    {
        /// <summary>
        /// Band covering human visual range.
        /// </summary>
        Gray = 0,

        /// <summary>
        /// Pixel-interleaved red/green/blue bands.
        /// </summary>
        Rgb = 1,

        /// <summary>
        /// Red band of a red/green/blue image.
        /// </summary>
        Red = 2,

        /// <summary>
        /// Green band of a red/green/blue image.
        /// </summary>
        Green = 3,

        /// <summary>
        /// Blue band of a red/green/blue image.
        /// </summary>
        Blue = 4 	 
    }
}
