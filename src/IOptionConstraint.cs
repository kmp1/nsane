namespace NSane
{
    /// <summary>
    /// This is the common interface for all constraints on device options
    /// </summary>
    public interface IOptionConstraint
    {
        /// <summary>
        /// Returns <c>true</c> if the given value is valid for the option
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns><c>true</c> if it is valid</returns>
        bool IsValid(string value);

        /// <summary>
        /// Returns <c>true</c> if the given value is valid for the option
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns><c>true</c> if it is valid</returns>
        bool IsValid(int value);
    }
}
