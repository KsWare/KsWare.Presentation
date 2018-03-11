using System.Collections.ObjectModel;
using System.Linq;
using KsWare.Presentation.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using Assert=NUnit.Framework.Assert;
using Is=NUnit.Framework.Is;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="EnumVM{T}"/>-class
	/// </summary>
	[TestClass]
	public class EnumVMTests:ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public override void TestInitialize() { base.TestInitialize();}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public override void TestCleanup() {base.TestCleanup(); }

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void UseWidthDefaultMetadata() {
			var vm=new EnumVM<TestEnum>();
			vm.EditValueProvider.String="B";
			Assert.AreEqual(TestEnum.B,vm.Value);

			vm.EditValueProvider.String="D";
			Assert.IsTrue(vm.ErrorProvider.HasError);
		}

		[TestMethod]
		public void A() {
			var enumBM = new EnumBM<TestEnum>();
			Assert.IsTrue(enumBM.HasValue);
			Assert.AreEqual(default(TestEnum),enumBM.Value);
		}

		/// <summary> 
		/// </summary>
		[TestMethod,Ignore /*TODO document this test case and make it work*/]
		public void B() {
			var enumBM = new EnumBM<TestEnum>();
			enumBM.Metadata.DataProvider.Data = TestEnum.A;
			Assert.IsNotNull(enumBM.Settings);
			Assert.IsTrue(enumBM.Settings.IncludeValuesSpecified);
			Assert.IsNotNull(enumBM.Settings.IncludeValues);

			var enumVM = new EnumVM<TestEnum>{Metadata = new BusinessValueMetadata <TestEnum>()};
			((BusinessValueDataProvider<TestEnum>) enumVM .Metadata.DataProvider).BusinessValue = enumBM;

			Assert.That(enumVM.DisplayValueProvider,Is.TypeOf<EnumDisplayValueProvider>());

			Assert.AreEqual(TestEnum.A,enumVM.Value);
			Assert.AreEqual("A",enumVM.DisplayValueProvider.String);
			Assert.AreEqual("A",enumVM.EditValueProvider.String);

			Assert.IsNotNull(enumVM.ValueSourceProvider);
			Assert.IsNotNull(enumVM.ValueSourceProvider.SourceList);

			var validValues = enumVM.ValueSourceProvider.SourceList.Cast<EnumMemberVM<TestEnum>>().ToList();
			Assert.AreEqual(3,validValues.Count);

			enumVM.Value=TestEnum.B;
			Assert.AreEqual(TestEnum.B,enumBM.Value);

			enumVM.EditValueProvider.String="C";
			Assert.IsFalse(enumVM.ErrorProvider.HasError);
			Assert.AreEqual(TestEnum.C,enumBM.Value);
		}

		[TestMethod][Ignore]
		public void BusinessValueDataProvider_BusinessValue_Settings_IncludeValues_changed() {
			var enumBM = new EnumBM<TestEnum>();
			var enumVM = new EnumVM<TestEnum>{Metadata = new BusinessValueMetadata <TestEnum>()};
			((BusinessValueDataProvider<TestEnum>) enumVM .Metadata.DataProvider).BusinessValue = enumBM;
			((ObservableCollection<TestEnum>) enumBM.Metadata.Settings.IncludeValues).RemoveAt(1);// B removed
			Assert.AreEqual(2,enumVM.ValueSourceProvider.SourceList.Count());

			enumVM.EditValueProvider.String = "B";
			Assert.IsTrue(enumVM.ErrorProvider.HasError);
			Assert.IsNotNull(enumVM.ErrorProvider.ErrorMessage);
			Assert.AreNotEqual("",enumVM.ErrorProvider.ErrorMessage);
			Assert.AreEqual(TestEnum.A,enumVM.Value);

			enumVM.EditValueProvider.String = "C";
			Assert.IsFalse(enumVM.ErrorProvider.HasError);
			Assert.IsNull(enumVM.ErrorProvider.ErrorMessage);
			Assert.AreEqual(TestEnum.C,enumVM.Value);
		}

		[TestMethod][Ignore]
		public void BusinessValueDataProvider_BusinessValue_Settings_IncludeValues_changed_while_value_is_selected() {
			var enumBM = new EnumBM<TestEnum>();
			var enumVM = new EnumVM<TestEnum>{Metadata = new BusinessValueMetadata <TestEnum>()};
			((BusinessValueDataProvider<TestEnum>) enumVM .Metadata.DataProvider).BusinessValue = enumBM;

			//A selected and A removed from IncludeValues should result in ErrorProvider.HasError=true
			((ObservableCollection<TestEnum>) enumBM.Metadata.Settings.IncludeValues).RemoveAt(0); // C removed
			Assert.AreEqual(2,enumVM.ValueSourceProvider.SourceList.Count());
			Assert.IsTrue(enumVM.ErrorProvider.HasError);
		}

		[TestMethod][Ignore]//TODO
		public void BusinessValueDataProvider_BusinessValue_is_not_initialized() {
			var enumVM = new EnumVM<TestEnum>{Metadata = new BusinessValueMetadata <TestEnum>()};
			//Throws.TypeOf<InvalidOperationException>();

			//Assert.AreEqual(enumVM.Value,null);

		}



		private enum TestEnum{A,B,C}
	}

	// ReSharper restore InconsistentNaming
}