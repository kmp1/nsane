NSane
=====

This little assembly wraps up the SANE API, details of which can be found at the [SANE Website](http://www.sane-project.org/), so that it can be used from .Net.  Yes, this is a slightly weird idea since SANE is all about scanning on Linux and .Net is, well, not very Linux (yup, mono, I will come to that) but if, for example, you have a box on your network that runs the saned daemon, you can use this API to make it scan.

The story was this: I had a Raspberry Pi and a scanner and two PCs and I wanted to write an application that ran on the PCs and allowed scanning.  I looked at the SANE to TWAIN [bridge](http://sanetwain.ozuzo.net) and then decided that I didn't really want to use TWAIN (because of the whole dependency on window handles and whatnot - I mean, maybe I want to use it in a console application).  So then I had a look at the API documentation for the network mode of the API and figured it looked pretty simple and I could knock something up in .Net that just targeted that.

And this is what this project is.  Nothing more, nothing less.

Getting Started
---------------

So, start by adding a reference to the assembly and stick this using statement in your code:

    using NSane;

There is only one public class in that namespace that is not an Exception or extension methods and it is a static so you can't really go wrong.  The idea is that you start with a "Connection" to SANE then you get hold of the available devices (or a specific one if you know it's name).  Once you have your device you open it and then you can fiddle with the options it has or just scan.

This is probably the simplest way to use the API if you just want to do a scan:

    BitmapImage image;
    using(var c = Connection.At("mymachine", 65535))
    {
        using(var dev = c.OpenDevice("test:0"))
        {
            var result = dev.Scan();
            image = result.Image.ToBitmapImage();
        }
    }

But Scanning is Delightfully Asynchronous!?
-------------------------------------------

Yup, it is and that is why the Scan method returns an `IScanResult` object.  Internally it is using the [Task](http://msdn.microsoft.com/en-us/library/system.threading.tasks.task%28v=vs.110%29.aspx) mechanism and hence there are `IsFinished` and `IsError` properties on the return object.  By accessing the `Image` property in the last example I am effectively blocking until the task completes.

Most likely you will want to continue on and have a callback called when scanning is finished, so to do that you can code up something like this (feel free to use Lambda expressions or whatever as the callbacks are simply [Action<T>](http://msdn.microsoft.com/en-us/library/018hxwa8%28v=vs.110%29.aspx)s):

	private static void ScanningCompleted(BitmapSource image)
	{
	}
	
	private static void ScanningError(AggregateException exception)
	{
	}
	
	...
	
	using(var c = Connection.At("mymachine", 65535))
    {
        using(var dev = c.OpenDevice("test:0"))
        {
            var result = dev.Scan(ScanningCompleted, ScanningError);
			
			// check the IsFinished property on the result to see what's what
			while(!result.IsFinished) {} // <-- Of course, no reason to do this
										 // as if you want to block, just use the
										 // Scan() method
        }
    }
	
Setting Scan Options
--------------------

There is one bit of the API that is not really discoverable and that is setting option values.  I am using a dynamic property in this case since the type of value you can set varies according to the option itself.  You should, therefore, make sure that you use the correct type when setting the property, for example here we will set the "depth" option of the test backend - notice that we are passing it an integer:

    // Find the property
    var depth = dev.AllOptions.First(n =>
        n.Name.Equals("depth", StringComparison.OrdinalIgnoreCase));

    // Get the original value
    int orig = depth.Value;

    // Set it to something else
    depth.Value = 16;
    
Tests
-----

There are a bunch of unit tests in the NSaneTests project, so have a look in there if you get stuck (also, if you find a bug, please get the tests to fail in your scenario when you report it - that would be a massive help).

In order to run the tests you will need to have configured the SANE test backend and you will need to adjust the constants in the TestConstants.cs file.  See the troubleshooting part below for information about what may go wrong.

Next Steps
----------

I see no reason why a "local" SANE scanner could not be implemented, at least
for [mono](http://www.mono-project.com) that would work on Mac and Linux.  The project has mono csproj files that but I have not been able to get them to compile as I have no [PresentationCore](http://msdn.microsoft.com/en-us/library/system.windows.media.imagesource%28v=vs.110%29.aspx) assembly available to me (I'm running Debian 7).  I believe that newer mono versions have this library.

License
-------

I am giving this to the community under the Apache license - enjoy and please don't blame me if somethnig bad happens when you use it.  See the LICENSE file in the repository.

Test Troubleshooting
--------------------

- If you get a TCP error about the connection being actively refused check the `TestConstants.SaneDaemonHost` and `TestConstants.SaneDaemonPort` values are correct and that the `saned` process is running on the remote machine.

- If `Device_GetAll_Succeeds` fails with the message `No devices discovered` edit vi `/etc/sane.d/dll.conf` on the sane server and make sure the test backend is not commented out.

Oh, One Other Thing
-------------------

If you are using this from a WPF application or winforms application you should probably be fine.  If you are not using Windows Server 2003, you should also probably be fine.  

Sadly, in Windows 2003 there's an issue with WriteableBitmap that means it throws invalid cast exception if the thread is not an STA thread.  [Here](http://msdn.microsoft.com/en-us/library/system.invalidcastexception%28v=vs.110%29.aspx) is some information about that.  

So, if this is you, you need to make sure that the threads that are launched all launch with STA and the way to do that is with a SynchronizationContext.  Have a look at the following atticles and the SingleThreadSynchronizationContext in the ScanTest for further information:

- [Await, SynchronizationContext, and Console Apps](http://blogs.msdn.com/b/pfxteam/archive/2012/01/20/10259049.aspx)
- [How to unit test code involving SynchronizationContext?](http://stackoverflow.com/q/8353950/1039947)
- [Why SynchronizationContext does not work properly?](http://stackoverflow.com/a/14144101/1039947)