namespace NSane
{
    /// <summary>
    /// This is the unit for an option (e.g. mm, dpi, etc)
    /// </summary>
    public enum SaneUnit
    {
        /// <summary>
        /// Value is unit-less (e.g., page count).
        /// </summary>
        None = 0,

        /// <summary>
        /// Value is in number of pixels.
        /// </summary>
        Pixels = 1,

        /// <summary>
        /// Value is in number of bits.
        /// </summary>
        Bits = 2,

        /// <summary>
        /// Value is in millimeters.
        /// </summary>
        Millimeters = 3,

        /// <summary>
        /// Value is a resolution in dots/inch.
        /// </summary>
        DotsPerInches = 4,

        /// <summary>
        /// Value is a percentage.
        /// </summary>
        Percentage = 5,

        /// <summary>
        /// Value is time in µ-seconds.
        /// </summary>
        Microsecond = 6
    }
}
