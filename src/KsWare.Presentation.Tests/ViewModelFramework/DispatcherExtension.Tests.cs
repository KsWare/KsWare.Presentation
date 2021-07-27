using KsWare.Presentation.Testing;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.ViewModelFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="DispatcherExtension"/>-class
	/// </summary>
	[TestFixture][Ignore("TODO")]//TODO Test does not work without an Application instance
	public class DispatcherExtensionTests:TestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public override void TestInitialize() {
			base.TestInitialize();
			//...do anything here...
		}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public override void TestCleanup() {
			//...do anything here...
			base.TestCleanup();
		}

		/// <summary> 
		/// </summary>
		[Test]
		public void Common() {

			//TODO 

		}

		[Test]
		public void IsInvokeRequired() {
//			Run(delegate { Assert.IsFalse(Application.Current.Dispatcher.IsInvokeRequired()); });
//			Assert.IsTrue(Application.Current.Dispatcher.IsInvokeRequired());
		}

		[Test]
		public void InvokeIfRequiered() {
//			int c=0;
//			Run(delegate { Application.Current.Dispatcher.InvokeIfRequired(new Action(() => c++)); });
//			Application.Current.Dispatcher.InvokeIfRequired(new Action(() => c++));
//			Assert.AreEqual(2,c);
		}

		[Test]
		public void DoEvents() { 
//			Application.Current.Dispatcher.DoEvents();
		} 
 
	}

	// ReSharper restore InconsistentNaming
}