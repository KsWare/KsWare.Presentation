using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.ViewModelFramework.Providers {

	[TestFixture]
	public class EditValueProviderˑStringExtensionTests {

		[Test]
		public void Int32VM_EditValueProvider_String() {
			var vm = new Int32VM();
			vm.Value = 1;
			vm.EditValueProvider.String = "2";
			Assert.AreEqual((int) 2, vm.Value);
		}

	}

}