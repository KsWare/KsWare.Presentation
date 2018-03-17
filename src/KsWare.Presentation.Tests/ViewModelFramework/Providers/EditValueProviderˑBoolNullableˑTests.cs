using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;

namespace KsWare.Presentation.Tests.ViewModelFramework.Providers {

	/// <summary> Tests for EditValueProvider.BoolNullable
	/// </summary>
	[TestClass]
	public class EditValueProviderˑBoolNullableˑTests {

		[TestMethod]
		public void BoolVM_InitialValue_MustBe_False() {
			var vm = new BoolVM();
			Assert.That(vm.Value,Is.EqualTo(false));
			Assert.That(vm.EditValueProvider.BoolNullable,Is.EqualTo(false));
		}	
	 
		[TestMethod] // true => true
		public void BoolVM_SetTrue() {
			var vm = new BoolVM();
			vm.Value = false;								// set the initial value
			vm.EditValueProvider.BoolNullable = true;		// set the edit value
			Assert.That(vm.Value,Is.EqualTo(true));			// compare
		}

		[TestMethod] // false => false
		public void BoolVM_SetFalse() {
			var vm = new BoolVM();
			vm.Value = true;//set the initial value			// set the initial value
			vm.EditValueProvider.BoolNullable = false;		// set the edit value
			Assert.That(vm.Value,Is.EqualTo(false));		// compare
		}

		[TestMethod,Ignore /*behavior is to be defined*/] // null => false | null => {Error} keep value unchanged
		public void BoolVM_SetNull_ShouldFail() {
			var vm = new BoolVM();
			vm.Value = true;								// set the initial value
			vm.EditValueProvider.BoolNullable = null;		// set the edit value
			Assert.That(vm.Value,Is.EqualTo(false));		// ??
		}


	}

}