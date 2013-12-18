using System;
using System.Reflection;

namespace NSane.Tests
{
	/// <summary>
	/// This little console application will just
	/// launch the unit tests
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// We'll just launch nunit.
		/// </summary>
		public static void Main ()
		{
	        int ret = NUnit.ConsoleRunner.Runner.Main(new []
			{ 
				Assembly.GetExecutingAssembly().Location 
			});

			// Just beep on a failure
	        if (ret != 0)
	            Console.Beep();
		}
	}
}

