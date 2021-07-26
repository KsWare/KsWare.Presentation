using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using KsWare.Presentation.UnitTesting;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.Testing {


	/// <summary> Base class for tests which need an initialized <see cref="ApplicationVM"/>
	/// </summary>
	public class ApplicationVMTestBase {

/*  Die Methoden werden in folgender Reihenfolge ausgeführt:

    Mit [AssemblyInitialize] markierte Methoden.
		Methode, die Code enthält, der vor dem Ausführen aller Tests in der Assembly verwendet wird und dazu dient, 
		durch die Assembly abgerufene Ressourcen zuzuordnen. 
    Mit [ClassInitialize] markierte Methoden.     
		Die mit diesem Attribut markierte Methode wird in einem Auslastungstest einmal ausgeführt. 
		Alle von der Methode ausgeführten Initialisierungsvorgänge gelten für den gesamten Test.
    Mit [SetUp] markierte Methoden.
    Mit [Test] markierte Methoden.
 */

// ONLY ONE TIME in assembly
//		[AssemblyInitialize]
//		public static void AssemblyInitialize(TestContext context) {
//			WriteLine("=>Test: AssemblyInitialize");
//			var waitForApplicationRun = new ManualResetEventSlim();
//			Task.Factory.StartNew(() => {
//				var application = new Application();
//				application.Startup += (s, e) => { waitForApplicationRun.Set(); };
//				application.Run();
//			});
//			waitForApplicationRun.Wait();
//		}

		
		public virtual void TestInitialize() {
			WriteLine("=>Test: TestInitialize");
			var appvm = new UnitTestAppVM();
			if(ApplicationVM.Current==null) throw new NullReferenceException("ApplicationVM.Current is null!");
		}

		//[TearDown]
		public virtual void TestCleanup() {
			WriteLine("=>Test: TestCleanup");
			ViewModelFramework.ApplicationVM.TestCleanup(); // clears the singleton
		}

		public IDispatcher Dispatcher => ApplicationVM.Current.Dispatcher;


		public void Run(Action action) {
			Dispatcher.Invoke(action);
		}

		// protected void ExecuteInSeparateAppDomain(string methodName) {
		// 	//does not more work in net5, deactivated because not used.
		// 	AppDomainSetup appDomainSetup = new AppDomainSetup();
		// 	appDomainSetup.ApplicationBase = Environment.CurrentDirectory;
		// 	AppDomain appDomain = AppDomain.CreateDomain(methodName, null, appDomainSetup);
		//
		// 	try {
		// 		appDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) {
		// 			throw (Exception)e.ExceptionObject;
		// 		};
		//
		// 		var unitTest = (ApplicationVMTestBase)appDomain.CreateInstanceAndUnwrap(GetType().Assembly.GetName().Name, GetType().FullName);
		//
		// 		unitTest.TestInitialize();
		//
		// 		MethodInfo methodInfo = unitTest.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		//
		// 		if (methodInfo == null) { throw new InvalidOperationException(string.Format("Method '{0}' not found on type '{1}'.", methodName, unitTest.GetType().FullName)); }
		//
		// 		try { methodInfo.Invoke(unitTest, null); }
		// 		catch (System.Reflection.TargetInvocationException e) {
		// 			throw e.InnerException;
		// 		}
		//
		// 		unitTest.TestCleanup();
		// 	}
		// 	finally { AppDomain.Unload(appDomain); }
		// }


		protected static void WriteLine() {
			Debug.Write("\n");
		}

		protected static void WriteLine(object o) {WriteLine("{0}",o);}

		protected static void WriteLine(string s, params object[] args) {
			s = s.Replace("\r\n", "\n").Replace("\r", "\n");
			s = string.Format(CultureInfo.InvariantCulture, s, args);
			s = string.Format("{0,2}⋮ ", Thread.CurrentThread.ManagedThreadId) + s;
			var sp = s.Split('\n');
			for (int i = 1; i < sp.Length; i++) sp[i] = "  ⋮ " + sp[i];
			s = string.Join("\n", sp) + "\n";
			Debug.Write(s);
		}
	}
}
