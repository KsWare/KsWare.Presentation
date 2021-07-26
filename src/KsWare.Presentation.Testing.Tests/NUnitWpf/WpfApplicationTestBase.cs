using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NUnit.Framework;
using KsWare.Test.Presentation.NUnitWpf.Mock;

namespace KsWare.Test.Presentation.NUnitWpf {

	[Obsolete("Use ApplicationVMTestBase",true)]
	public class WpfApplicationTestBase {

		//[SetUp]
		public virtual void TestInitialize() {
			WriteLine("=>Test: TestInitialize");
			if(Application.Current==null) {
				WriteLine("=>Test: Create new Application");
				new Thread(delegate() {
					var app = new ApplicationMock {
						ShutdownMode = ShutdownMode.OnExplicitShutdown,
						//Resources = ???,
						//MainWindow = new MainWindowMock()
					};
					Dispatcher.Run();
					app.Run();
					// now Application.Current is valid;
				}){IsBackground = true}.Start();

			} else {
				WriteLine("=>Test: Using existing Application");
			}

			//Wait until Application.Current is valid
        	while (Application.Current==null) {Thread.Sleep(25);}
		}

		//[TearDown]
		public virtual void TestCleanup() {
			WriteLine("=>Test: TestCleanup");
		}

		public void Run(Action action) {
			Application.Current.Dispatcher.Invoke(action);
		}


		protected void WriteLine() {
			Debug.Write("\n");
		}
		protected void WriteLine(string s) {
			Debug.Write(s+"\n");
		}
		protected void WriteLine(string s, params object[] args) {
			Debug.Write(string.Format(s,args)+"\n");
		}
	}
}
