using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NSane.Constraint
{
    /// <summary>
    /// This is a constraint that is a collection of strings that the property
    /// must be set to
    /// </summary>
    internal class StringListConstraint : IOptionConstraint
    {
        private readonly IEnumerable<string> _values;

        /// <summary>
        /// Construct with the values
        /// </summary>
        /// <param name="values">The allowable values for the constraint</param>
        internal StringListConstraint(IEnumerable<string> values)
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
            return _values.Contains(value);
        }

        /// <summary>
        /// Returns <c>true</c> if the given value is valid for the option
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns><c>true</c> if it is valid</returns>
        public bool IsValid(int value)
        {
            return IsValid(Convert.ToString(value,
                                            CultureInfo.CurrentCulture));
        }
    }
}
