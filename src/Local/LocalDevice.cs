using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace NSane
{
	/// <summary>
	/// This is a device that is locally connected.
	/// </summary>
	internal class LocalDevice : Device
	{
		private readonly LocalProcedureCaller _caller;
		private readonly string _userName;
		private readonly string _password;

		private int _handle;
		private bool _open;
		private bool _started;

		/// <summary>
		/// Construct the device with the bits and bobs it needs
		/// </summary>
		/// <param name="name">The name of the device (unique I think)</param>
		/// <param name="vendor">The vendor</param>
		/// <param name="model">The model</param>
		/// <param name="type">The type</param>
		/// <param name="caller">The local calling implementation</param>
		/// <param name="userName">The username to use for authenticated calls
		/// </param>
		/// <param name="password">The password to use for authenticated calls
		/// </param>
		internal LocalDevice(string name,
		                     string vendor,
		                     string model,
		                     string type,
		                     LocalProcedureCaller caller,
		                     string userName,
		                     string password)
			: base(name, vendor, model, type)
		{
			_userName = userName;
			_password = password;
			_caller = caller;
		}

		public override IOpenedDevice Open()
		{
			throw new NotImplementedException();
		}

		public override IScanResult Scan()
		{
			throw new NotImplementedException();
		}

		public override IScanResult Scan(Action<BitmapSource> onCompleteCallback)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<IDeviceOption> AllOptions
		{
			get 
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Create a user readble overview of the device (i.e. the
		/// name, vendor, etc)
		/// </summary>
		/// <returns>The aforementioned description</returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture,
			                     "{0} ({1}, {2} - {3})",
			                     Name,
			                     Vendor ?? string.Empty,
			                     Model ?? string.Empty,
			                     Type ?? string.Empty);
		}
		
		/// <summary>
		/// Dispose of the device object
		/// </summary>
		/// <param name="disposing"><c>true</c> if we are at disposing time
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (_caller != null)
			{
				if (_open)
				{
					Close();
				}
			}
			
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// Close the device
		/// </summary>
		private void Close()
		{
			if (_started)
			{
				Cancel();
			}
			
			_caller.CloseDevice(_handle);
			_handle = 0;
			
			_open = false;
		}

		/// <summary>
		/// Cancels the current scan
		/// </summary>
		private void Cancel()
		{
			_caller.Cancel(_handle);
			_started = false;
		}
	}
}

