using System;
using System.Collections.Generic;

namespace NSane
{
	/// <summary>
	/// This makes a connection to the local SANE API.
	/// </summary>
	internal class LocalConnection : DisposableObject, IConnection
	{
		private readonly string _userName;
		private readonly string _password;
		private readonly LocalProcedureCaller _caller;

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="NSane.LocalConnection"/> class with
		/// credentials.
		/// </summary>
		/// <param name='userName'>
		/// User name.
		/// </param>
		/// <param name='password'>
		/// Password.
		/// </param>
		public LocalConnection(string userName, string password)
		{
			_userName = userName;
			_password = password;
			_caller = new LocalProcedureCaller();
		}

		public IOpenedDevice OpenDevice(string name)
		{
			throw new NotImplementedException ();
		}

		public int Version 
		{
			get{throw new NotImplementedException();}
		}

		public IEnumerable<IOpenableDevice> AllDevices 
		{
			get{throw new NotImplementedException();}
		}

		protected override void Dispose(bool disposing)
		{
			_caller.Dispose();

			base.Dispose (disposing);
		}
	}
}

