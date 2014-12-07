using System;
using System.Globalization;
using System.Threading;
using KsWare.Presentation.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="BasicTypeVM"/>-class
	/// </summary>
	[TestClass]
	public class BasicTypeVMTests: ApplicationVMTestBase {

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
			base.TestCleanup();
		}

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void Common() {
			var types = new Type[] {typeof (ByteVM), typeof (Int16VM), typeof (Int32VM), typeof (Int64VM), typeof (SingleVM), typeof (DoubleVM), typeof (BoolVM), typeof (DateTimeVM), typeof (TimeSpanVM), typeof (GuidVM)};
			foreach (var type in types) {
				var vm=(IValueVM)Activator.CreateInstance(type);
				vm.ValueChanged+=OnValueChanged;
				vm.EditValueProvider.String = "blubber";
				vm.ValueChanged-=OnValueChanged;
			}
		}

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void ByteVM() {
			var vm=new ByteVM();
			vm.EditValueProvider.String = "-1";
			Assert.AreEqual(true,vm.ErrorProvider.HasError);
			Assert.AreEqual(0,vm.Value);
			vm.EditValueProvider.String = "1";
			Assert.AreEqual(false, vm.ErrorProvider.HasError);
			Assert.AreEqual(1,vm.Value);
			vm.EditValueProvider.String = "256";
			Assert.AreEqual(true,vm.ErrorProvider.HasError);
			Assert.AreEqual(1,vm.Value);
			vm.EditValueProvider.String = "255";
			Assert.AreEqual(false,vm.ErrorProvider.HasError);
			Assert.AreEqual(255,vm.Value);
		}

		[TestMethod]
		public void Int16VM() {
			var vm = new Int16VM();
		}

		[TestMethod]
		public void Int32VM() {
			var vm = new Int32VM();
			Assert.AreEqual(0,vm.Value);

			vm.Value = 1;
			Assert.AreEqual(1,vm.Value);

			vm.EditValueProvider.String = "2";
			Assert.AreEqual(2,vm.Value);
		}

		[TestMethod]
		public void Int32VM_InitValueMustBe_0() {
			var vm = new Int32VM();
			Assert.AreEqual((int)0,vm.Value);
		}

		[TestMethod]
		public void Int64VM() {
			var vm = new Int64VM();
		}

		[TestMethod]
		public void SingleVM() {
			var vm = new SingleVM();
			vm.EditValueProvider.String = "2";
			Assert.AreEqual(2.0,vm.Value);

			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
			vm.EditValueProvider.String = "2,4";
			Assert.AreEqual((float)2.4,vm.Value);

			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
			vm.EditValueProvider.String = "2.4";
			Assert.AreEqual((float)2.4,vm.Value);
		}

		[TestMethod]
		public void DoubleVM() {
			var vm = new DoubleVM();
			vm.EditValueProvider.String = "2";
			Assert.AreEqual(2.0,vm.Value);

			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
			vm.EditValueProvider.String = "2,4";
			Assert.AreEqual(2.4,vm.Value);

			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
			vm.EditValueProvider.String = "2.4";
			Assert.AreEqual(2.4,vm.Value);
		}

		[TestMethod]
		public void BoolVM() {
			var vm = new BoolVM();
			vm.EditValueProvider.String = "True";
			Assert.AreEqual(true,vm.Value);
			vm.EditValueProvider.String = "False";
			Assert.AreEqual(false,vm.Value);

			//Yes/No support
			vm.EditValueProvider.String = "Yes";
			Assert.AreEqual(true,vm.Value);
			vm.EditValueProvider.String = "No";
			Assert.AreEqual(false,vm.Value);

			//Ja/Nein support
			vm.EditValueProvider.String = "Ja";
			Assert.AreEqual(true,vm.Value);
			vm.EditValueProvider.String = "Nein";
			Assert.AreEqual(false,vm.Value);

			//Wahr/Falsch support
			vm.EditValueProvider.String = "Wahr";
			Assert.AreEqual(true, vm.Value);
			vm.EditValueProvider.String = "Falsch";
			Assert.AreEqual(false, vm.Value);
		}

		[TestMethod]
		public void BoolVM_InitValueMustBe_False() {
			var vm = new BoolVM();
			Assert.AreEqual(false,vm.Value);
		}

		[TestMethod][Ignore] //TODO rewrite the test
		public void DateTimeVM() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
			var vm = new DateTimeVM();
			vm.EditValueProvider.String = "01.02.2010 08:22";
			Assert.AreEqual(new DateTime(2010,02,01,08,22,00),vm.Value);
			Assert.AreEqual("01.02.2010 08:22:00",vm.DisplayValueProvider.String);

			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
			vm = new DateTimeVM();
			vm.EditValueProvider.String = "02/28/2010 08:22";
			Assert.AreEqual(new DateTime(2010,02,28,08,22,00),vm.Value);
			Assert.AreEqual("2/28/2010 8:22:00 AM",vm.DisplayValueProvider.String);
		}


		[TestMethod]
		public void GuidVM() {
			var vm = new GuidVM();
			vm.EditValueProvider.String = "{C24A250D-54A6-4FBC-BF63-2798DDAC7930}";
			Assert.AreEqual(new Guid("{C24A250D-54A6-4FBC-BF63-2798DDAC7930}"),vm.Value);
			Assert.AreEqual("c24a250d-54a6-4fbc-bf63-2798ddac7930",vm.DisplayValueProvider.String);
		}

		private void OnValueChanged(object sender, EventArgs args) { }
	}

	// ReSharper restore InconsistentNaming
}