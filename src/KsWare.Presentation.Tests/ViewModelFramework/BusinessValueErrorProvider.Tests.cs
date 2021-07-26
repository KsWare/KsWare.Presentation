

using System;
using KsWare.Presentation.Testing;
using NUnit.Framework;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.Tests.ViewModelFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="BusinessValueErrorProvider"/>-class
	/// </summary>
	[TestFixture]
	public class BusinessValueErrorProviderTests: TestBase {

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
			base.TestInitialize();
		}

		/// <summary> 
		/// </summary>
		[Test]
		public void Common() {
			var bm = new Int32BM{Metadata = new BusinessValueMetadata{DataProvider = new LocalDataProvider(),Settings = new ValueSettings<Int32>()}};
			Assert.IsNotNull(bm.Settings);
			var vm = new Int32VM {MemberName="Int32", Metadata = new BusinessValueMetadata<Int32>()};
			Assert.IsInstanceOf<BusinessValueErrorProvider>(vm.Metadata.ErrorProvider);
			((IBusinessValueDataProvider) vm.Metadata.DataProvider).BusinessValue = bm;
			Assert.IsInstanceOf<BusinessValueErrorProvider>(vm.Metadata.ErrorProvider);
			bm.Metadata.Settings.Maximum = 100;
		}

	}

	// ReSharper restore InconsistentNaming
}