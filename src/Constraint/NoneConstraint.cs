namespace NSane.Constraint
{
    /// <summary>
    /// This is a constraint with no requirements (so always valid)
    /// </summary>
    internal class NoneConstraint : IOptionConstraint
    {
        /// <summary>
        /// Returns <c>true</c> if the given value is valid for the option
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns><c>true</c> if it is valid</returns>
        public bool IsValid(string value)
        {
            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if the given value is valid for the option
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns><c>true</c> if it is valid</returns>
        public bool IsValid(int value)
        {
            return true;
        }
    }
}
