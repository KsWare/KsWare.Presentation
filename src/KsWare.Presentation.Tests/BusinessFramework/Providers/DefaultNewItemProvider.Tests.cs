using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.BusinessFramework.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.BusinessFramework.Providers {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="DefaultNewItemProvider"/>
	/// </summary>
	[TestClass]
	public class DefaultNewItemProviderTests {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() { }

		[TestMethod][Ignore]
		public void Constructor() {
			var provider = new DefaultNewItemProvider();
			Assert.AreEqual(true,provider.IsSupported);
			Assert.IsInstanceOfType(provider.CreateItem<TestItemBM>(null),typeof(TestItemBM));
		}

		private class TestItemBM:ObjectBM{}

	}

	// ReSharper restore InconsistentNaming
}
