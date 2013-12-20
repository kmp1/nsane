using System;
using System.Dynamic;
using System.Globalization;

namespace NSane
{
    /// <summary>
    /// This is a base class for device options (that could be shared between
    /// network and local implementations if desired).  Mostly not interesting
    /// except for the implementation of the <see cref="IDeviceOption.Value"/>
    /// property.  Here we are jumping through some hoops to get the dynamic
    /// sub-system to work how we like.  Basically the goal is that the user
    /// gets or sets the property with the appropriate type (they will know this
    /// either implicitly of by using the type property) and we will call an
    /// abstract method in the inheriting class to do the appropriate thing.
    /// </summary>
    internal abstract class DeviceOption : IDeviceOption
    {
        private readonly dynamic _dy;

        /// <summary>
        /// Construct the option with what it needs to work
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
        /// <param name="reloadFunction">A function to call if setting this
        /// option requires that all the options are reloaded</param>
        protected DeviceOption(string name,
                               string title,
                               string description,
                               int size,
                               int number,
                               SaneType type,
                               SaneUnit unit,
                               SaneCapabilities capabilities,
                               Action reloadFunction)
        {
            Number = number;
            Name = name;
            Title = title;
            Description = description;
            Size = size/4;
            Type = type;
            Unit = unit;
            Capabilities = capabilities;
            ReloadFunction = reloadFunction;

            _dy = new InnerValueDynamicObject(this);
        }

        /// <summary>
        /// The function to call to reload this and other options
        /// </summary>
        protected Action ReloadFunction { get; private set; }

        /// <summary>
        /// Gets or sets the value of the option
        /// </summary>
        public dynamic Value
        {
            get { return _dy.Value; }
            set { _dy.Value = value; }
        }

        /// <summary>
        /// The name of the option
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The title of the option
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// The description of the option
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The "unit" (i.e. pixels etc) for the option
        /// </summary>
        public SaneUnit Unit { get; internal set; }

        /// <summary>
        /// The full capabiilties of the device (this is a flags enumeration)
        /// </summary>
        public SaneCapabilities Capabilities { get; internal set; }

        /// <summary>
        /// Gets the constraint for the device (may be none)
        /// </summary>
        public IOptionConstraint Constraint { get; internal set; }

        /// <summary>
        /// The type of option
        /// </summary>
        public SaneType Type { get; internal set; }

        /// <summary>
        /// <c>true</c> if the device option is active
        /// </summary>
        public bool IsActive
        {
            get { return (Capabilities & SaneCapabilities.Inactive) == 0; }
        }

        /// <summary>
        /// <c>true</c> if the device option may be set
        /// </summary>
        public bool IsSettable
        {
            get { return (Capabilities & SaneCapabilities.SoftSelect) != 0; }
        }

        /// <summary>
        /// <c>true</c> if the device option is allowed to be set to "automatic"
        /// (where the scanner chooses a good value)
        /// </summary>
        public bool IsAutomaticAllowed
        {
            get { return (Capabilities & SaneCapabilities.Automatic) != 0; }
        }

        /// <summary>
        /// Gets the device option "number" (i.e. the index of the option since
        /// there is nothing else that uniquely identifies it)
        /// </summary>
        internal int Number { get; private set; }

        /// <summary>
        /// Gets the size of the option
        /// </summary>
        protected int Size { get; private set; }

        /// <summary>
        /// Set the option to automatic
        /// </summary>
        public abstract void SetToAutomatic();

        /// <summary>
        /// Set the option to the given value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <returns>The value as it was returned by SANE (sometimes you cannot
        /// set to exactly the value and it will return what it really set it
        /// to</returns>
        protected abstract int SetValue(int value);

        /// <summary>
        /// Set the option to the given value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <returns>The value as it was returned by SANE (sometimes you cannot
        /// set to exactly the value and it will return what it really set it
        /// to</returns>
        protected abstract string SetValue(string value);

        /// <summary>
        /// Set the option to the given value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <returns>The value as it was returned by SANE (sometimes you cannot
        /// set to exactly the value and it will return what it really set it
        /// to</returns>
        protected abstract bool SetValue(bool value);

        /// <summary>
        /// Set the option to the given value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <returns>The value as it was returned by SANE (sometimes you cannot
        /// set to exactly the value and it will return what it really set it
        /// to</returns>
        protected abstract double SetValue(double value);

        /// <summary>
        /// Gets the value of the option
        /// </summary>
        /// <returns>The value of the option</returns>
        protected abstract int GetInt32Value();

        /// <summary>
        /// Gets the value of the option
        /// </summary>
        /// <returns>The value of the option</returns>
        protected abstract string GetStringValue();

        /// <summary>
        /// Gets the value of the option
        /// </summary>
        /// <returns>The value of the option</returns>
        protected abstract bool GetBooleanValue();

        /// <summary>
        /// Gets the value of a fixed point option
        /// </summary>
        /// <returns>The fixed point option</returns>
        protected abstract double GetFixedValue();

        /// <summary>
        /// Set the value, checking the constraint first
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>The set value (may be different if the scanner has
        /// decided it should be so</returns>
        private int DoSetValue(int value)
        {
            if (!Constraint.IsValid(value))
                throw new InvalidOperationException(
                    string
                        .Format(CultureInfo.CurrentCulture,
                                "Value '{0}' is not permitted by the constraint",
                                value));

            return SetValue(value);
        }

        /// <summary>
        /// Set the value, checking the constraint first
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>The set value (may be different if the scanner has
        /// decided it should be so</returns>
        private string DoSetValue(string value)
        {
            if (!Constraint.IsValid(value))
                throw new InvalidOperationException(
                    string
                        .Format(CultureInfo.CurrentCulture,
                                "Value '{0}' is not permitted by the constraint",
                                value));

            return SetValue(value);
        }

        /// <summary>
        /// This class wraps up the calls to the <see cref="Value"/> property.
        /// We do it like this just to simplify the <see cref="TrySetMember"/>
        /// method and keep all the other properties non-dynamic
        /// </summary>
        private class InnerValueDynamicObject : DynamicObject
        {
            private readonly DeviceOption _deviceOption;

            private bool _beenGot;
            private object _value;

            /// <summary>
            /// Construct the inner dynamic object
            /// </summary>
            /// <param name="deviceOption">The option that owns it</param>
            public InnerValueDynamicObject(DeviceOption deviceOption)
            {
                _deviceOption = deviceOption;
            }

            /// <summary>
            /// Attempt to set the member.  This simply checks the _type member
            /// and based on that calls the appropriate method (if it is not
            /// an expected value we fail this call)
            /// </summary>
            /// <param name="binder">The binder</param>
            /// <param name="value">The value to set to</param>
            /// <returns><c>true</c> if it succeeds</returns>
            public override bool TrySetMember(SetMemberBinder binder,
                                              object value)
            {
                if (_deviceOption.Type == SaneType.Integer)
                {
                    _value = _deviceOption.DoSetValue((int) value);
                }
                else if (_deviceOption.Type == SaneType.String)
                {
                    _value = _deviceOption.DoSetValue((string) value);
                }
                else if (_deviceOption.Type == SaneType.Boolean)
                {
                    _value = _deviceOption.SetValue((bool) value);
                }
                else if (_deviceOption.Type == SaneType.Fixed)
                {
                    _value = _deviceOption.SetValue((double) value);
                }
                else if (_deviceOption.Type == SaneType.Button)
                {
                    _value = _deviceOption.SetValue((bool)value);
                }
                else
                {
                    return false;
                }

                _beenGot = true;
                return true;
            }

            /// <summary>
            /// Here we are attempting to get the member value.  Again, we use
            /// the _type member to call the appropriate method.
            /// </summary>
            /// <param name="binder">The binder</param>
            /// <param name="result">The resulting object</param>
            /// <returns><c>true</c> if it succeeds</returns>
            public override bool TryGetMember(GetMemberBinder binder,
                                              out object result)
            {
                if (!_beenGot)
                {
                    if (_deviceOption.Type == SaneType.Integer)
                    {
                        _value = _deviceOption.GetInt32Value();
                    }
                    else if (_deviceOption.Type == SaneType.String)
                    {
                        _value = _deviceOption.GetStringValue();
                    }
                    else if (_deviceOption.Type == SaneType.Boolean)
                    {
                        _value = _deviceOption.GetBooleanValue();
                    }
                    else if (_deviceOption.Type == SaneType.Fixed)
                    {
                        _value = _deviceOption.GetFixedValue();
                    }
                    else if (_deviceOption.Type == SaneType.Button)
                    {
                        // You cannot get the value of a button, you can only
                        // "push" it to make the scanner do something
                        _value = false;
                    }
                    else
                    {
                        result = null;
                        return false;
                    }
                }

                result = _value;
                _beenGot = true;

                return true;
            }
        }
    }
}
