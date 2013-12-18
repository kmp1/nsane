using System;
using System.Globalization;

namespace NSane.Constraint
{
    /// <summary>
    /// This is a constraint which says the value must be within a given range.
    /// </summary>
    internal class RangeConstraint : IOptionConstraint
    {
        private readonly int _min;
        private readonly int _max;
        private readonly int _quant;

        /// <summary>
        /// Construct with the bounds of the range
        /// </summary>
        /// <param name="min">The minimum allowable value</param>
        /// <param name="max">The maximum allowable value</param>
        /// <param name="quant">The quant</param>
        internal RangeConstraint(int min, int max, int quant)
        {
            _max = max;
            _min = min;
            _quant = quant;
        }

        /// <summary>
        /// Returns <c>true</c> if the given value is valid for the option
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns><c>true</c> if it is valid</returns>
        public bool IsValid(string value)
        {
            try
            {
                return IsValid(Convert.ToInt32(value,
                                               CultureInfo.CurrentCulture));
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the given value is valid for the option
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns><c>true</c> if it is valid</returns>
        public bool IsValid(int value)
        {
            return (value >= _min && value <= _max && value%_quant == 0);
        }
    }
}
