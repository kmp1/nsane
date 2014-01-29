using System;
using System.Runtime.InteropServices;

namespace NSane.Local
{
    /// <summary>
    /// A SANE option descriptor
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SaneOptionDescriptor
    {
        /// <summary>
        /// The name
        /// </summary>
        public string Name;

        /// <summary>
        /// The title
        /// </summary>
        public string Title;

        /// <summary>
        /// The description
        /// </summary>
        public string Description;

        /// <summary>
        /// The type
        /// </summary>
        public SaneType Type;

        /// <summary>
        /// The unit
        /// </summary>
        public SaneUnit Unit;

        /// <summary>
        /// The size
        /// </summary>
        public int Size;

        /// <summary>
        /// The capabilities (this is a flags enumeration)
        /// </summary>
        public SaneCapabilities Capabilities;

        /// <summary>
        /// The constraint, if there is any
        /// </summary>
        public SaneConstraint Constraint;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr StringList;
    };
}
