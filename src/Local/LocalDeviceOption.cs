using System;

namespace NSane.Local
{
	/// <summary>
	/// This is an option for a locally connected device.
	/// </summary>
	internal class LocalDeviceOption : DeviceOption
	{
		private readonly int _handle;
		private readonly string _userName;
		private readonly string _password;

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
		/// <param name="userName">The username for authenticated calls</param>
		/// <param name="password">The password for authenticated calls</param>
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
		                          int handle,
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
		}

		public override void SetToAutomatic()
		{
			throw new NotImplementedException();
		}

		protected override int SetValue (int value)
		{
			throw new NotImplementedException();
		}

		protected override string SetValue(string value)
		{
			throw new NotImplementedException ();
		}

		protected override bool SetValue(bool value)
		{
			throw new NotImplementedException ();
		}

		protected override double SetValue(double value)
		{
			throw new NotImplementedException();
		}

		protected override int GetInt32Value()
		{
			throw new NotImplementedException();
		}

		protected override string GetStringValue()
		{
			throw new NotImplementedException();
		}

		protected override bool GetBooleanValue()
		{
			throw new NotImplementedException();
		}

		protected override double GetFixedValue()
		{
			throw new NotImplementedException();
		}
	}
}

