using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.Tests.Core {
	// ReSharper disable InconsistentNaming

	/// <summary> Test Class
	/// </summary>
	[TestClass]
	public class CustomDataProviderTests {

		object data;
		CustomDataProvider provider;

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() {
			this.provider = new CustomDataProvider(() => data, v => data = v);
		}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() { }

		[TestMethod]
		public void IsSupported() {
			Assert.AreEqual(true,provider.IsSupported);
		}

		[TestMethod]
		public void GetSet() {
			Assert.AreEqual(6,provider.Data=6);
			Assert.AreEqual(data,provider.Data  );

			provider.Data=5; Assert.AreEqual(data,5);
			Assert.AreEqual(5,provider.Data);
		}

		[TestMethod]
		public void NotifyDataChanged() {
			int cb = 0;
			provider.DataChanged+=delegate { cb++; };
			data = 7; Assert.AreEqual(7,provider.Data);
			provider.NotifyDataChanged();
			Assert.AreEqual(1,cb);
		}

		[TestMethod]
		public void DataValidatingCallback() {
			int cb = 0;
			provider.DataValidatingCallback=delegate(object sender, object value) { cb++; return new Exception("Test");};
			provider.Data=7;
			provider.NotifyDataChanged();
			Assert.AreEqual(1,cb);
		}
	}

	// ReSharper restore InconsistentNaming
}
