using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.ViewModelFramework.Providers {

	[TestClass]
	public class EditValueProvider_NullableBoolExtensionTests {

		[TestMethod]
		public void BoolVM_InitialValue_MustBe_False() {
			var vm = new BoolVM();
			Assert.AreEqual(false, vm.Value);
			Assert.AreEqual(false, vm.EditValueProvider.BoolNullable);
		}	
	 
		[TestMethod]
		public void BoolVM_True() {
			var vm = new BoolVM();
			vm.Value = false;
			vm.EditValueProvider.BoolNullable = true;
			Assert.AreEqual(true, vm.Value);
		}

		[TestMethod]
		public void BoolVM_False() {
			var vm = new BoolVM();
			vm.Value = true;
			vm.EditValueProvider.BoolNullable = false;
			Assert.AreEqual(false, vm.Value);
		}

		[TestMethod]
		public void BoolVM_Null_ShouldFail() {
			var vm = new BoolVM();
			vm.Value = true;
			vm.EditValueProvider.BoolNullable = null;
			Assert.AreEqual(false, vm.Value);
		}


	}

}