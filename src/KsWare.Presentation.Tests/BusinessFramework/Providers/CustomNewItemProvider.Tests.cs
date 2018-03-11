using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.BusinessFramework.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.BusinessFramework.Providers {
	// ReSharper disable InconsistentNaming

	/// <summary> Test Class
	/// </summary>
	[TestClass]
	public class CustomNewItemProviderTests {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() { }

		[TestMethod]
		public void Constructor() {
			TestItemBM testItem=new TestItemBM();
			CreateNewItemCallbackHandler createNewItemCallbackHandler = delegate(object data) { return testItem; };

			var provider = new CustomNewItemProvider(createNewItemCallbackHandler);
			Assert.AreEqual(true,provider.IsSupported);
			Assert.AreEqual(testItem,provider.CreateItem<TestItemBM>(null));
		}

		private class TestItemBM:ObjectBM{}
	}

	// ReSharper restore InconsistentNaming
}
