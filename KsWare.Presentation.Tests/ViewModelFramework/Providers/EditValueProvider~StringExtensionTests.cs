using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewModelFramework.Providers {

	[TestClass]
	public class EditValueProvider_StringExtensionTests {

		[TestMethod]
		public void Int32VM_EditValueProvider_String() {
			var vm = new Int32VM();
			vm.Value = 1;
			vm.EditValueProvider.String = "2";
			Assert.AreEqual((int) 2, vm.Value);
		}

	}

}