using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.BusinessFramework.Providers;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.BusinessFramework.Providers {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="DefaultNewItemProvider"/>
	/// </summary>
	[TestFixture]
	public class DefaultNewItemProviderTests {

		[Test][Ignore("TODO")]
		public void Constructor() {
			var provider = new DefaultNewItemProvider();
			Assert.AreEqual(true,provider.IsSupported);
			Assert.IsInstanceOf<TestItemBM>(provider.CreateItem<TestItemBM>(null));
		}

		private class TestItemBM:ObjectBM{}

	}

	// ReSharper restore InconsistentNaming
}
