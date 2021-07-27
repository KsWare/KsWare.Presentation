using System;
using KsWare.Presentation.Core.Providers;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.Core {
	// ReSharper disable InconsistentNaming

	/// <summary> Test Class
	/// </summary>
	[TestFixture]
	public class CustomDataProviderTests {

		object data;
		CustomDataProvider provider;

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public void Setup() {
			this.provider = new CustomDataProvider(() => data, v => data = v);
		}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public void Teardown() { }

		[Test]
		public void IsSupported() {
			Assert.AreEqual(true,provider.IsSupported);
		}

		[Test]
		public void GetSet() {
			Assert.AreEqual(6,provider.Data=6);
			Assert.AreEqual(data,provider.Data  );

			provider.Data=5; Assert.AreEqual(data,5);
			Assert.AreEqual(5,provider.Data);
		}

		[Test]
		public void NotifyDataChanged() {
			int cb = 0;
			provider.DataChanged+=delegate { cb++; };
			data = 7; Assert.AreEqual(7,provider.Data);
			provider.NotifyDataChanged();
			Assert.AreEqual(1,cb);
		}

		[Test]
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
