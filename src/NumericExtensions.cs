namespace NSane
{
    /// <summary>
    /// This class provides some extension methods for converting to and from
    /// fixed point numbers
    /// </summary>
    internal static class NumericExtensions
    {
        /// <summary>
        /// Convert the source to a floating point number from a fixed point
        /// </summary>
        /// <param name="source">The source to convert</param>
        /// <param name="fractionScale">The fraction scale</param>
        /// <returns>The value as a floating point number</returns>
        internal static double ToFloating(this int source, int fractionScale)
        {
            double value = source / ((double)(1 << fractionScale));
            return value;
        }

        /// <summary>
        /// Convert the source to a fixed point number - this may lose some
        /// precision
        /// </summary>
        /// <param name="source">The source to convert</param>
        /// <param name="fractionScale">The fraction scale</param>
        /// <returns>The number as fixed point</returns>
        public static int ToFixed(this double source, int fractionScale)
        {
            double value = source*(1 << fractionScale);
            return (int) value;
        }
    }
}
