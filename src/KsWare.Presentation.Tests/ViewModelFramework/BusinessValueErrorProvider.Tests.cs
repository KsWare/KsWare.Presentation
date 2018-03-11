

using System;
using KsWare.Presentation.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.Tests.ViewModelFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="BusinessValueErrorProvider"/>-class
	/// </summary>
	[TestClass]
	public class BusinessValueErrorProviderTests: TestBase {

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
			base.TestInitialize();
		}

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void Common() {
			var bm = new Int32BM{Metadata = new BusinessValueMetadata{DataProvider = new LocalDataProvider(),Settings = new ValueSettings<Int32>()}};
			Assert.IsNotNull(bm.Settings);
			var vm = new Int32VM {MemberName="Int32", Metadata = new BusinessValueMetadata<Int32>()};
			Assert.IsInstanceOfType(vm.Metadata.ErrorProvider,typeof(BusinessValueErrorProvider));
			((IBusinessValueDataProvider) vm.Metadata.DataProvider).BusinessValue = bm;
			Assert.IsInstanceOfType(vm.Metadata.ErrorProvider,typeof(BusinessValueErrorProvider));
			bm.Metadata.Settings.Maximum = 100;
		}

	}

	// ReSharper restore InconsistentNaming
}