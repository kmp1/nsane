using System;

namespace NSane
{
    /// <summary>
    /// This is used to tell how well the control request has been satisfied
    /// </summary>
    [Flags]
    internal enum SaneOptionInformation
    {
        /// <summary>
        /// An exact set operation, nothing to do
        /// </summary>
        None = 0,

        /// <summary>
        /// This value is returned when setting an option value resulted in a 
        /// value being selected that does not exactly match the requested 
        /// value. For example, if a scanner can adjust the resolution in 
        /// increments of 30dpi only, setting the resolution to 307dpi may 
        /// result in an actual setting of 300dpi. When this happens, the 
        /// bitset returned in *i has this member set. In addition, the option 
        /// value is modified to reflect the actual (rounded) value that was 
        /// used by the backend. Note that inexact values are admissible for 
        /// strings as well. A backend may choose to ``round'' a string to the 
        /// closest matching legal string for a constrained string value.
        /// </summary>
        Inexact = 1,

        /// <summary>
        /// The setting of an option may affect the value or availability of 
        /// one or more other options. When this happens, the SANE backend sets
        /// this member in *i to indicate that the application should reload 
        /// all options. This member may be set if and only if at least one 
        /// option changed.
        /// </summary>
        ReloadOptions = 2,

        /// <summary>
        /// The setting of an option may affect the parameter values (see 
        /// sane_get_parameters()). If setting an option affects the parameter 
        /// values, this member will be set in *i. Note that this member may be 
        /// set even if the parameters did not actually change. However, it is 
        /// guaranteed that the parameters never change without this member 
        /// being set.
        /// </summary>
        ReloadParameters = 4 
    }
}
