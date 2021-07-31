using System.ComponentModel;
using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.ViewModelFramework.Providers {

	[TestFixture]
	public class EditValueProviderˑHexNumberExtensionTests {

		[Test]
		public void Int32VM_EditValueProvider_HexNumber_10() {
			var vm = new Int32VM();
			vm.Value = 1;
			vm.EditValueProvider.HexNumber.Value = "10";
			Assert.AreEqual((int) 16, vm.Value);
		}

		[Test]
		public void Int32VM_EditValueProvider_HexNumber_FF() {
			var vm = new Int32VM();
			vm.Value = 1;
			vm.EditValueProvider.HexNumber.Value = "FF";
			Assert.AreEqual((int) 255, vm.Value);
		}

		[Test]
		public void Int32VM_EditValueProvider_HexNumber_0F() {
			var vm = new Int32VM();
			vm.Value = 1;
			vm.EditValueProvider.HexNumber.Value = "0F";
			Assert.AreEqual((int) 15, vm.Value);
		}

		[Test]
		public void ByteVM_EditValueProvider_HexNumber_100() {
			var vm = new ByteVM();
			vm.Value = 1;
			vm.EditValueProvider.HexNumber.Value = "100";
			Assert.AreEqual((int) 1, vm.Value);
			Assert.That(((IDataErrorInfo)vm.EditValueProvider.HexNumber)["Value"], Is.Not.Null.And.Not.Empty);
		}

	}

}