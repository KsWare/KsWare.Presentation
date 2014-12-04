using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewFramework {

	[TestClass]
	public class ApplicationTests {

		private void Log(string s) {
			Debug.WriteLine(string.Format("{0:HH:mm:ss,fff} {1,-4} {2}",DateTime.Now, Thread.CurrentThread.ManagedThreadId,s));
		}
		private void Log(string format, params object[] args) {
			Log(string.Format(format,args));
		}

		public Dispatcher TestDispatcher { get; set; }

		private void Run(Action action,Dispatcher dispatcher) { dispatcher.Invoke(action, DispatcherPriority.Normal); }
		
		public Application TestSubject { get; set; }

		[TestMethod]
		public void TestMethod1() {
			TestDispatcher = Dispatcher.CurrentDispatcher;
			Log("Test Start");
			Log("new Application");
			TestSubject = new Application {
				ShutdownMode = ShutdownMode.OnExplicitShutdown,
			};
			TestSubject.Startup += (s, e) => Log("Startup");
			TestSubject.Exit += (s, e) => Log("Exit");
			TestSubject.SessionEnding += (s, e) => Log("SessionEnding");
			new Thread(new ThreadStart(delegate {Thread.Sleep(1000); Log("Shutdown"); Run(()=>TestSubject.Shutdown(),TestDispatcher); })).Start();
			TestSubject.Run();
			Log("Test End");
		}

		[TestMethod]
		public void TestMethod2() {
			// System.InvalidOperationException: Cannot create more than one System.Windows.Application instance in the same AppDomain.

			TestDispatcher = Dispatcher.CurrentDispatcher;
			Log("Test Start");
			Log("new Application");
			TestSubject = new Application {
				ShutdownMode = ShutdownMode.OnExplicitShutdown,
			};
			TestSubject.Startup += (s, e) => Log("Startup");
			TestSubject.Exit += (s, e) => Log("Exit");
			TestSubject.SessionEnding += (s, e) => Log("SessionEnding");
			new Thread(new ThreadStart(delegate {Thread.Sleep(1000); Log("Shutdown"); Run(()=>TestSubject.Shutdown(),TestDispatcher); })).Start();
			TestSubject.Run();
			Log("Test End");
		}

		
	}
}
