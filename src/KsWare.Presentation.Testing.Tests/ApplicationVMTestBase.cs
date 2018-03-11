using System;
using System.Diagnostics;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.Testing.Tests {

	public class ApplicationVMTestBase {

		public virtual void TestInitialize() {
			new ApplicationVM();
		}

		public virtual void TestCleanup() {
			ApplicationVM.TestCleanup();
		}

		public void Run(Action action) {
			ApplicationVM.Current.Dispatcher.Invoke(action);
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