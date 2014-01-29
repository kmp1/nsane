using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NSane.Local
{
    /// <summary>
    /// This contains the p/invoke signatures for the native method calls
    /// required to interface with the SANE C API.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Initialize SANE
        /// </summary>
        /// <param name="version">The version of sane</param>
        /// <param name="callback">The authentication callback handle</param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_init")]
        public extern static SaneStatus SaneInitialize(out int version, IntPtr callback);

        /// <summary>
        /// Open SANE
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_open", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public extern static SaneStatus SaneOpen(string name, out IntPtr handle);

        /// <summary>
        /// Close SANE
        /// </summary>
        /// <param name="handle"></param>
        [DllImport("libsane", EntryPoint = "sane_close")]
        public extern static void SaneClose(IntPtr handle);
        
        /// <summary>
        /// Exit SANE
        /// </summary>
        [DllImport("libsane", EntryPoint = "sane_exit")]
        public extern static void SaneExit();

        /// <summary>
        /// Get the status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_strstatus")]
        public extern static IntPtr SaneStatus(SaneStatus status);
        
        /// <summary>
        /// Gets the available devices
        /// </summary>
        /// <param name="deviceList"></param>
        /// <param name="localOnly"></param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_get_devices")]
        public extern static SaneStatus SaneGetDevices(ref IntPtr deviceList, [MarshalAs(UnmanagedType.Bool)] bool localOnly);

       
        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_control_option")]
        public extern static SaneStatus SaneControlOption(IntPtr handle, int n, SaneOptionAction a, out IntPtr v, ref int i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_control_option")]
        public extern static SaneStatus SaneControlOption2(IntPtr handle, int n, SaneOptionAction a, ref IntPtr v, ref int i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_control_option")]
        public extern static SaneStatus SaneControlOptionInteger(IntPtr handle, int n, SaneOptionAction a, ref int v, ref int i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_control_option")]
        public extern static SaneStatus SaneControlOptionString(IntPtr handle, int n, SaneOptionAction a, ref string v, ref int i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_control_option", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public extern static SaneStatus SaneControlOptionStringBuilder(IntPtr handle, int n, SaneOptionAction a, StringBuilder v, ref int i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_control_option")]
        public extern static SaneStatus SaneControlOptionFixed(IntPtr handle, int n, SaneOptionAction a, ref int v, ref int i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_control_option")]
        public extern static SaneStatus SaneControlOptionBoolean(IntPtr handle, int n, SaneOptionAction a, [MarshalAs(UnmanagedType.Bool)] ref bool v, ref int i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_control_option")]
        public extern static SaneStatus SaneControlOption(IntPtr handle, int n, SaneOptionAction a, ref IntPtr v, ref IntPtr i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", 
            "CA1811:AvoidUncalledPrivateCode", Justification = 
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_control_option")]
        public extern static SaneStatus SaneControlOption(IntPtr handle, int n, SaneOptionAction a, ref float v, ref int i);

        /// <summary>
        /// Set a control option
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_control_option", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public extern static SaneStatus SaneControlOption(IntPtr handle, int n, SaneOptionAction a, StringBuilder v, ref int i);

        /// <summary>
        /// Get option descriptor
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        [DllImport("libsane", EntryPoint = "sane_get_option_descriptor")]
        public extern static IntPtr SaneGetOptionDescriptor(IntPtr handle, int n);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="buf"></param>
        /// <param name="maxlen"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_read")]
        public extern static SaneStatus SaneRead(IntPtr handle, out IntPtr buf, int maxlen, ref int len);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="buf"></param>
        /// <param name="maxlen"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_read")]
        public extern static SaneStatus SaneRead(IntPtr handle, [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 32768)] byte[] buf, int maxlen, ref int len);

        /// <summary>
        /// Get parameters
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_get_parameters")]
        public extern static SaneStatus SaneGetParameters(IntPtr handle, ref IntPtr p);

        /// <summary>
        /// Get parameters
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_get_parameters")]
        public extern static SaneStatus SaneGetParameters(IntPtr handle, [In, Out, MarshalAs(UnmanagedType.LPStruct)] SaneParameters p);

        /// <summary>
        /// Get parameters
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_get_parameters")]
        public extern static SaneStatus SaneGetParameters(IntPtr handle, IntPtr p);

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_start")]
        public extern static SaneStatus SaneStart(IntPtr handle);
       
        /// <summary>
        /// Cancel
        /// </summary>
        /// <param name="handle"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_cancel")]
        public extern static void SaneCancel(IntPtr handle);

        /// <summary>
        /// Set I/O mode
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_set_io_mode")]
        public extern static SaneStatus SaneSetIoMode(IntPtr handle, [MarshalAs(UnmanagedType.Bool)] bool async);

        /// <summary>
        /// Select feature descriptor
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="fd"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification =
            "This is here so we have the complete API wrapped")]
        [DllImport("libsane", EntryPoint = "sane_get_select_fd")]
        public extern static SaneStatus SaneGetSelectFd(IntPtr handle, out IntPtr fd);
    }
}

