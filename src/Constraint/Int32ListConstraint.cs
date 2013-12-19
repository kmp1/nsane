using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NSane.Constraint
{
    /// <summary>
    /// This is a constraint for a device option that is a collection of
    /// allowable integers
    /// </summary>
    internal class Int32ListConstraint : IOptionConstraint
    {
        private readonly IEnumerable<int> _values;

        /// <summary>
        /// Construct with the values
        /// </summary>
        /// <param name="values">The allowable values for this option</param>
        internal Int32ListConstraint(IEnumerable<int> values)
        {
            _values = values;
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
            return _values.Contains(value);
        }
    }
}
