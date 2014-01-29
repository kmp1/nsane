using System;
using System.Text;

namespace NSane.Local
{
    /// <summary>
    /// This is an option for a locally connected device.
    /// </summary>
    internal class LocalDeviceOption : DeviceOption
    {
        private const int SaneFixedScaleShift = 16;

        private readonly IntPtr _handle;

        /// <summary>
        /// Construct the option with what it needs
        /// </summary>
        /// <param name="name">The name of the option</param>
        /// <param name="title">The title of the option</param>
        /// <param name="description">A long description for the option</param>
        /// <param name="size">The size of the option</param>
        /// <param name="number">The number (i.e. index) of the option</param>
        /// <param name="type">The type of the option (string, bool, etc)
        /// </param>
        /// <param name="unit">The unit for the option (mm, etc, etc)</param>
        /// <param name="capabilities">A bit flag of the capabilities of
        /// the option</param>
        /// <param name="handle">The handle to the device</param>
        /// <param name="reloadFunction">A function to call if setting this
        /// option requires that all the options are reloaded</param>
        internal LocalDeviceOption(string name,
                                   string title,
                                   string description,
                                   int size,
                                   int number,
                                   SaneType type,
                                   SaneUnit unit,
                                   SaneCapabilities capabilities,
                                   IntPtr handle,
                                   Action reloadFunction)
            : base(name,
                   title,
                   description,
                   size,
                   number,
                   type,
                   unit,
                   capabilities,
                   reloadFunction)
        {
            _handle = handle;
        }

        public override void SetToAutomatic()
        {
            int i = 0;
            SaneStatus status;
            if (Type == SaneType.String)
            {
                status = NativeMethods.SaneControlOptionStringBuilder(
                    _handle, 
                    Number, 
                    SaneOptionAction.Automatic, 
                    new StringBuilder(string.Empty), 
                    ref i);
            }
            else if (Type == SaneType.Boolean)
            {
                bool v = true;
                status = NativeMethods.SaneControlOptionBoolean(
                    _handle, Number, SaneOptionAction.Automatic, ref v, ref i);
            }
            else if (Type == SaneType.Integer)
            {
                int v = 0;
                status = NativeMethods.SaneControlOptionInteger(
                    _handle, 
                    Number,
                    SaneOptionAction.Automatic, 
                    ref v, 
                    ref i);
            }
            else if (Type == SaneType.Fixed)
            {
                var val = 1.0.ToFixed(SaneFixedScaleShift);
                
                status = NativeMethods.SaneControlOptionInteger(
                   _handle, Number, SaneOptionAction.Automatic, ref val, ref i);                
            }
            else
            {
                throw new NotSupportedException(
                    "Option type not supported");
            }

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);
        }

        protected override int SetValue (int value)
        {
            int i = 0;
            var status = NativeMethods.SaneControlOptionInteger(
                _handle, Number, SaneOptionAction.Set, ref value, ref i);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            var ret = GetInt32Value();
            return ret;
        }

        protected override string SetValue(string value)
        {
            int i = 0;
            var status = NativeMethods.SaneControlOptionStringBuilder(
                _handle, Number, SaneOptionAction.Set, new StringBuilder(value), ref i);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            var ret = GetStringValue();
            return ret;
        }

        protected override bool SetValue(bool value)
        {
            int i = 0;
            var status = NativeMethods.SaneControlOptionBoolean(
                _handle, Number, SaneOptionAction.Set, ref value, ref i);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            var ret = GetBooleanValue();
            return ret;
        }

        protected override double SetValue(double value)
        {
            var val = value.ToFixed(SaneFixedScaleShift);
            int i = 0;

            var status = NativeMethods.SaneControlOptionInteger(
               _handle, Number, SaneOptionAction.Set, ref val, ref i);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            var ret = GetFixedValue();
            return ret;
        }

        protected override int GetInt32Value()
        {
            int i = 0;
            int val = 0;
            var status = NativeMethods.SaneControlOptionInteger(
                _handle, Number, SaneOptionAction.Get, ref val, ref i);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            return val;
        }

        protected override string GetStringValue()
        {
            int i = 0;
            string val = string.Empty;
            var status = NativeMethods.SaneControlOptionString(
                _handle, Number, SaneOptionAction.Get, ref val, ref i);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            return val;
        }

        protected override bool GetBooleanValue()
        {
            int i = 0;
            bool val = false;
            var status = NativeMethods.SaneControlOptionBoolean(
                _handle, Number, SaneOptionAction.Get, ref val, ref i);

            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            return val;
        }

        protected override double GetFixedValue()
        {
            IntPtr pointer;
            int i = 0;
            var status = NativeMethods.SaneControlOption(
                _handle, Number, 0, out pointer, ref i);
            if (status != (int)SaneStatus.Success)
                throw NSaneException.CreateFromStatus((int)status);

            var val = pointer.ToInt32().ToFloating(SaneFixedScaleShift);
            return val;
        }
    }
}

