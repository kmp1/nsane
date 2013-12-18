using System;

namespace NSane
{
    /// <summary>
    /// Enumeration of SANE capabilities (this is a bit flag, so use
    /// appropriately)
    /// </summary>
    [Flags]
    public enum SaneCapabilities
    {
        /// <summary>
        /// No capabilities
        /// </summary>
        None = 0,

        /// <summary>
        /// The option value can be set by a call to sane_control_option().
        /// </summary>
        SoftSelect = 1,

        /// <summary>
        /// The option value can be set by user-intervention (e.g., by flipping
        ///  a switch). The user-interface should prompt the user to execute 
        /// the appropriate action to set such an option. This capability is 
        /// mutually exclusive with SANE_CAP_SOFT_SELECT (either one of them 
        /// can be set, but not both simultaneously).
        /// </summary>
        HardSelect = 2,

        /// <summary>
        /// The option value can be detected by software. If 
        /// <see cref="SaneCapabilities.SoftSelect"/> is set, this capability 
        /// must be set. If SANE_CAP_HARD_SELECT is set, this capability may or
        /// may not be set. If this capability is set but neither 
        /// <see cref="SaneCapabilities.SoftSelect"/> nor
        /// <see cref="SaneCapabilities.HardSelect"/> are, then there is no way
        ///  to control the option. That is, the option provides read-out of 
        /// the current value only.
        /// </summary>
        SoftDetect = 4,

        /// <summary>
        /// If set, this capability indicates that an option is not directly 
        /// supported by the device and is instead emulated in the backend. A 
        /// sophisticated frontend may elect to use its own (presumably better)
        /// emulation in lieu of an emulated option.
        /// </summary>
        Emulated = 8,

        /// <summary>
        /// If set, this capability indicates that the backend (or the device) 
        /// is capable to picking a reasonable option value automatically. For 
        /// such options, it is possible to select automatic operation by 
        /// calling <see cref="IDeviceOption.SetToAutomatic"/>.
        /// </summary>
        Automatic = 16,

        /// <summary>
        /// If set, this capability indicates that the option is not currently 
        /// active (e.g., because it's meaningful only if another option is set 
        /// to some other value).
        /// </summary>
        Inactive = 32,

        /// <summary>
        /// If set, this capability indicates that the option should be 
        /// considered an "advanced user option." A frontend typically 
        /// displays such options in a less conspicuous way than regular 
        /// options (e.g., a command line interface may list such options last 
        /// or a graphical interface may make them available in a seperate
        /// "advanced settings" dialog). 
        /// </summary>
        Advanced = 64
    }
}
