using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.Core.Providers;
using Assert=NUnit.Framework.Assert;
using Is=NUnit.Framework.Is;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	[TestClass]
	public class RefVMBusinessTests: ApplicationVMTestBase {

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

		[TestMethod][Ignore]//TODO Test RefVM with StringVM->StringBM
		public void Test_StringVM() {
			var bm=new RefBM<StringBM>();
			var vm=new RefVM<StringVM> {Metadata = new BusinessReferenceViewModelMetadata<StringVM,StringBM>()};
			vm.Metadata.DataProvider.Data = bm;

			Assert.AreEqual(null,vm.Target);

			#region change ref in business layer
			var newBM=new StringBM{Metadata = {DataProvider = {Data = "2"}}};
			bm.Target = newBM;

			Assert.AreEqual("2",vm.Target.Value);
			#endregion
		}

		[TestMethod,Ignore /*TODO document this test case and make it work*/]
		public void Test_ObjectVM() {
			TestDM dm = null;
			var bm=new RefBM<TestBM>{Metadata = new BusinessObjectMetadata {
					DataProvider = new CustomDataProvider<TestDM>(() => dm, value => dm = value)
				}};
			var vm = new RefVM<TestVM>();
			vm.Metadata.DataProvider.Data = bm;

			Assert.That(vm.Target,Is.Null,"vm.Target");

			#region change ref in business layer
			//var bmRef1=new TestBM{Metadata = {DataProvider = {Data = new TestDM{Test="1"}}}};
			var bmRef1 = new TestBM();
			var p=bmRef1.Metadata.DataProvider;
			var dm1=new TestDM {Test = "1"};
			p.Data = dm1;
			bm.Target = bmRef1;

			Assert.That(dm             ,Is.Not.Null     ,"dm");
			Assert.That(dm.Test        ,Is.EqualTo("1") ,"m.Test");
			Assert.That(vm.Target.Test ,Is.EqualTo("1") ,"vm.Target.Test");
			#endregion

			#region change ref in viewmodel layer
			var bmRef2=new TestBM{Metadata = {DataProvider = {Data = new TestDM{Test="2"}}}};
			var vmRef2 = new TestVM {Metadata = {DataProvider = {Data = bmRef2}}};
			vm.Target = vmRef2;

			Assert.AreEqual("2",dm.Test);
			Assert.AreEqual("2",bm.Target.Test);
			#endregion

//			#region change ref in viewmodel layer
//			var bmRef2=new TestBM{Test = "2"};
//			var vmRef2 = new TestVM {Metadata = {DataProvider = {Data = bmRef2}}};
//			vm.Ref = vmRef2;
//
//			Assert.AreEqual("2",bm.Ref.Test);
//			#endregion
		}

		[TestMethod]
		public void GetBusinessType() {
			Assert.AreEqual(typeof(TestBM), BusinessObjectVM.GetBusinessType(typeof(TestVM)));
		}
	}

	public class TestBM:ObjectBM {
		public string Test{get => ((TestDM) Metadata.DataProvider.Data).Test; set => ((TestDM) Metadata.DataProvider.Data).Test = value; }
	}

	public class TestVM:BusinessObjectVM<TestBM> {
		public string Test{get => ((TestBM) Metadata.DataProvider.Data).Test; set => ((TestBM) Metadata.DataProvider.Data).Test = value; }
	}

	public class TestDM {
		public string Test { get; set; }
	}
}
