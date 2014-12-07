using System;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	[TestClass]
	public class ApplicationVMTests:ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public override void TestInitialize() {base.TestInitialize(); }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public override void TestCleanup() {base.TestCleanup(); }

		[TestMethod]
		public void Current_IsNotNull() {
			Assert.IsNotNull(ApplicationVM.Current);
		}

		[TestMethod]
		public void Current_Dispatcher_IsNotNull() {
			Assert.IsNotNull(ApplicationVM.Current.Dispatcher);
		}
	}
}
