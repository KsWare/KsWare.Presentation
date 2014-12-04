using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using KsWare.Presentation.Testing;
using KsWare.Presentation.Tests.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewModelFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="DispatcherExtension"/>-class
	/// </summary>
	[TestClass][Ignore]//TODO Test does not work without an Application instance
	public class DispatcherExtensionTests:TestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public override void TestInitialize() {
			base.TestInitialize();
			//...do anything here...
		}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public override void TestCleanup() {
			//...do anything here...
			base.TestCleanup();
		}

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void Common() {

			//TODO 

		}

		[TestMethod]
		public void IsInvokeRequired() {
//			Run(delegate { Assert.IsFalse(Application.Current.Dispatcher.IsInvokeRequired()); });
//			Assert.IsTrue(Application.Current.Dispatcher.IsInvokeRequired());
		}

		[TestMethod]
		public void InvokeIfRequiered() {
//			int c=0;
//			Run(delegate { Application.Current.Dispatcher.InvokeIfRequired(new Action(() => c++)); });
//			Application.Current.Dispatcher.InvokeIfRequired(new Action(() => c++));
//			Assert.AreEqual(2,c);
		}

		[TestMethod]
		public void DoEvents() { 
//			Application.Current.Dispatcher.DoEvents();
		} 
 
	}

	// ReSharper restore InconsistentNaming
}