using System.Runtime.InteropServices;

namespace NSane.Local
{
    /// <summary>
    /// A SANE device
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SaneDevice
    {
        /// <summary>
        /// The name
        /// </summary>
        public string Name;

        /// <summary>
        /// The vendor
        /// </summary>
        public string Vendor;

        /// <summary>
        /// The model
        /// </summary>
        public string Model;

        /// <summary>
        /// The type
        /// </summary>
        public string Type;
    }
}
