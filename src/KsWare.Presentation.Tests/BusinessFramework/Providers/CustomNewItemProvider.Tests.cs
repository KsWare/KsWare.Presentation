using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.BusinessFramework.Providers;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.BusinessFramework.Providers {
	// ReSharper disable InconsistentNaming

	/// <summary> Test Class
	/// </summary>
	[TestFixture]
	public class CustomNewItemProviderTests {

		[Test]
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
