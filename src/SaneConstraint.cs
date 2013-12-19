namespace NSane
{
    /// <summary>
    /// This is a constraint type from SANE
    /// </summary>
    internal enum SaneConstraint
    {
        /// <summary>
        /// No constraint
        /// </summary>
        None = 0,

        /// <summary>
        ///  Value has to be with in a certain range (Fixed or Int)
        /// </summary>
        Range = 1,

        /// <summary>
        /// Value can be one from a list of integers 
        /// </summary>
        IntegerList = 2,

        /// <summary>
        /// Value can be one from a list of strings
        /// </summary>
        StringList = 3
    }
}
