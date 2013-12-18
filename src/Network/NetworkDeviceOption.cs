using System;
using System.Globalization;
using System.Text;

namespace NSane.Network
{
    /// <summary>
    /// This is the networked version of an <see cref="IDeviceOption"/>.  It
    /// calls the RPC on the SANE daemon to set and get its value
    /// </summary>
    internal class NetworkDeviceOption : DeviceOption
    {
        private readonly int _handle;
        private readonly NetworkProcedureCaller _caller;
        private readonly string _userName;
        private readonly string _password;

        /// <summary>
        /// The precision of a SANE_Fixed type - used for converting
        /// </summary>
        private const int SaneFixedPrecision = 16;

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
        /// <param name="caller">The RPC caller</param>
        /// <param name="userName">The username for authenticated calls</param>
        /// <param name="password">The password for authenticated calls</param>
        /// <param name="reloadFunction">A function to call if setting this
        /// option requires that all the options are reloaded</param>
        internal NetworkDeviceOption(string name,
                                     string title,
                                     string description,
                                     int size,
                                     int number,
                                     SaneType type,
                                     SaneUnit unit,
                                     SaneCapabilities capabilities,
                                     int handle,
                                     NetworkProcedureCaller caller,
                                     string userName,
                                     string password,
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
            _userName = userName;
            _password = password;
            _caller = caller;
        }

        /// <summary>
        /// Set the option to automatic
        /// </summary>
        public override void SetToAutomatic()
        {
            if (Type == SaneType.String)
            {
                Control<string>((w, s) => SendString(w, string.Empty),
                                SaneOptionAction.Automatic);
            }
            else if (Type == SaneType.Boolean)
            {
                Control<bool>((w, s) => SendWord(w, s, 1),
                              SaneOptionAction.Automatic);
            }
            else if (Type == SaneType.Integer)
            {
                Control<int>((w, s) => SendWord(w, s, 1),
                             SaneOptionAction.Automatic);
            }
            else if (Type == SaneType.Fixed)
            {
                Control<int>((w, s) => SendWord(w,
                                                s,
                                                1.0.ToFixed(SaneFixedPrecision)),
                             SaneOptionAction.Automatic);
            }
            else
            {
                throw new NotSupportedException(
                    "Option type not supported");
            }
        }

        /// <summary>
        /// Set the option to the given value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <returns>The value as it was returned by SANE (sometimes you cannot
        /// set to exactly the value and it will return what it really set it
        /// to</returns>
        protected override double SetValue(double value)
        {
            var val = value.ToFixed(SaneFixedPrecision);

            if (!Constraint.IsValid(val))
                throw new InvalidOperationException(
                    string
                        .Format(CultureInfo.CurrentCulture,
                                "Value '{0}' is not permitted by the constraint",
                                value));

            var set = Control<int>((w, s) => SendWord(w, s, val),
                                   SaneOptionAction.Set);
            return set.ToFloating(SaneFixedPrecision);
        }

        /// <summary>
        /// Set the option to the given value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <returns>The value as it was returned by SANE (sometimes you cannot
        /// set to exactly the value and it will return what it really set it
        /// to</returns>
        protected override int SetValue(int value)
        {
            var set = Control<int>((w, s) => SendWord(w, s, value),
                                   SaneOptionAction.Set);
            return set;
        }

        /// <summary>
        /// Set the option to the given value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <returns>The value as it was returned by SANE (sometimes you cannot
        /// set to exactly the value and it will return what it really set it
        /// to</returns>
        protected override string SetValue(string value)
        {
            var set = Control<string>((w, s) => SendString(w, value),
                                      SaneOptionAction.Set);
            return set;
        }

        /// <summary>
        /// Set the option to the given value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <returns>The value as it was returned by SANE (sometimes you cannot
        /// set to exactly the value and it will return what it really set it
        /// to</returns>
        protected override bool SetValue(bool value)
        {
            var set = Control<bool>((w, s) => SendWord(w, s, value ? 1 : 0),
                                    SaneOptionAction.Set);
            return set;
        }

        /// <summary>
        /// Gets the value of the option
        /// </summary>
        /// <returns>The value of the option</returns>
        protected override bool GetBooleanValue()
        {
            return Control<bool>((w, s) => SendWord(w, s, 1),
                                 SaneOptionAction.Get);
        }

        /// <summary>
        /// Gets the value of the option
        /// </summary>
        /// <returns>The value of the option</returns>
        protected override int GetInt32Value()
        {
            return Control<int>((w, s) => SendWord(w, s, 1),
                                SaneOptionAction.Get);
        }

        /// <summary>
        /// Gets the value of the option
        /// </summary>
        /// <returns>The value of the option</returns>
        protected override string GetStringValue()
        {
            return Control<string>((w, s) => SendString(w, string.Empty),
                                   SaneOptionAction.Get);
        }

        /// <summary>
        /// Gets the value of a fixed point option
        /// </summary>
        /// <returns>The fixed point option</returns>
        protected override double GetFixedValue()
        {
            var ret = Control<int>((w, s) => SendWord(w, s, 1),
                                   SaneOptionAction.Get);
            return ret.ToFloating(SaneFixedPrecision);
        }

        /// <summary>
        /// Serialize a word to send down the wire
        /// </summary>
        /// <param name="wire">The wire</param>
        /// <param name="size">The size of the word (usually 1)</param>
        /// <param name="value">The value to send</param>
        /// <returns>The size sent</returns>
        private static int SendWord(NetworkMethods wire, int size, int value)
        {
            int sze = size*4;
            wire.SendWord(sze);
            wire.SendWord(1);
            wire.SendWord(value);
            return sze;
        }

        /// <summary>
        /// Serialize a string to send down the wire
        /// </summary>
        /// <param name="wire">The wire</param>
        /// <param name="value">The value to send</param>
        /// <returns>The size sent</returns>
        private static int SendString(NetworkMethods wire, string value)
        {
            int sze = Encoding.ASCII.GetBytes(value).Length;
            wire.SendWord(sze);
            wire.SendString(value);
            return sze;
        }

        /// <summary>
        /// Just a little helper method to save the number of arguments we are
        /// passing around
        /// </summary>
        /// <typeparam name="T">The otpion type</typeparam>
        /// <param name="sender">The sender action (i.e. the thing that
        ///converts the object to its network specific binary data)</param>
        /// <param name="action">The action to perform</param>
        /// <returns>The result of the control call</returns>
        private T Control<T>(Func<NetworkMethods, int, int> sender,
                             SaneOptionAction action)
        {
            if (action == SaneOptionAction.Automatic && !IsAutomaticAllowed)
                throw new InvalidOperationException(
                    "You cannot set this option to automatic");

            if (action == SaneOptionAction.Set)
            {
                if (!IsActive)
                    throw new InvalidOperationException(
                        "You cannot set this option as it is inactive");

                if (!IsSettable)
                    throw new InvalidOperationException(
                        "You cannot set this option as it is read-only");
            }

            var ret = _caller.ControlOption<T>(_handle,
                                               Number,
                                               action,
                                               Type,
                                               Size,
                                               sender,
                                               _userName,
                                               _password,
                                               ReloadFunction);
            return ret;
        }
    }
}
