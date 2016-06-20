using System.Collections.Generic;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.Tests.Core.Provides {

	// ReSharper disable InconsistentNaming

	/// <summary> Test Class
	/// </summary>
	[TestClass]
	public class MappedDataProviderTests:TestBase {

		private RootVM m_RootVM;
		private TestVM m_TestVM;
		private MappedDataProvider<TestData, int> provider;

		/// <summary> Initialize the test.
		/// </summary>
		[TestInitialize]
		public override void TestInitialize() {
			base.TestInitialize();
//			m_RootVM=new RootVM();
//			provider = (MappedDataProvider<TestData, int>) m_RootVM.TestObject.Number.Metadata.DataProvider;
			m_TestVM=new TestVM();
			provider = (MappedDataProvider<TestData, int>) m_TestVM.Number.Metadata.DataProvider;
		}

		/// <summary> Cleanup this test.
		/// </summary>
		[TestCleanup]
		public override void TestCleanup() {
			base.TestCleanup();
		}

		[TestMethod]
		public void T001_IsSupported() {
			Assert.AreEqual(true,provider.IsSupported);
		}

		[TestMethod]
		public void T002_InitData() {
			m_TestVM.Data=new TestData{Number = 1};
			Assert.AreEqual(1,m_TestVM.Number.Value);
		}
		[TestMethod]
		public void T003_GetSet() {
			var data = new TestData();
			m_TestVM.Data=data;

			m_TestVM.Number.Value = 2;
			Assert.AreEqual(2,data.Number);

			data.Number = 3;
			Assert.AreEqual(3,m_TestVM.Number.Value);
		}

		[TestMethod]
		public void T004_InitRootData() {
			m_RootVM=new RootVM();
			var data = new RootData {TestObject = new TestData {Number = 1},TestList = new List<TestData>{new TestData{Number = 2}}};
			m_RootVM.Data=data;
			m_RootVM.TestList.Metadata.DataProvider.NotifyDataChanged();//HACK

			Assert.AreEqual(1,data.TestObject.Number);
			Assert.AreEqual(1,data.TestList.Count);
			Assert.AreEqual(2,data.TestList[0].Number);

			Assert.IsNotNull(m_RootVM.TestObject.Data);
			Assert.IsNotNull(m_RootVM.TestList.MːData);

			Assert.AreEqual(1,m_RootVM.TestObject.Number.Value);
			Assert.AreEqual(1,m_RootVM.TestList.Count);
			Assert.AreEqual(2,m_RootVM.TestList[0].Number.Value);
		}

//		[TestMethod]
//		public void GetSet() {
//			m_RootVM.Data=new RootData{TestObject=new TestData{Number = 1}};
//			Assert.AreEqual(1,provider.Data=1);
//			Assert.AreEqual(data,provider.Data  );
//
//			provider.Data=5; Assert.AreEqual(data,5);
//			Assert.AreEqual(5,provider.Data);
//		}

//		[TestMethod]
//		public void NotifyDataChanged() {
//			int cb = 0;
//			provider.DataChanged+=delegate { cb++; };
//			data = 7; Assert.AreEqual(7,provider.Data);
//			provider.NotifyDataChanged();
//			Assert.AreEqual(1,cb);
//		}

//		[TestMethod]
//		public void DataValidatingCallback() {
//			int cb = 0;
//			provider.DataValidatingCallback=delegate(object sender, object value) { cb++; return new Exception("Test");};
//			provider.Data=7;
//			provider.NotifyDataChanged();
//			Assert.AreEqual(1,cb);
//		}


		private class RootVM:DataVM<RootData> {
			public RootVM() {
				TestObject = RegisterChild("TestObject", new TestVM{Metadata = new ViewModelMetadata {DataProvider = new MappedDataProvider<RootData, TestData>("TestObject")}});
				TestList = RegisterChild("TestList", new ListVM<TestVM> {Metadata = new ListViewModelMetadata {DataProvider = new MappedDataProvider<RootData, List<TestData>>("TestList")}});
			}

			public TestVM TestObject { get; private set; }
			public ListVM<TestVM> TestList { get; private set; }
		}

		private class RootData {
			public TestData TestObject { get; set; }
			public List<TestData> TestList { get; set; }
		}

		private class TestVM:DataVM<TestData> {
			public TestVM() {
				Number = RegisterChild("Number", new Int32VM {Metadata = new ViewModelMetadata {DataProvider = new MappedDataProvider<TestData, int>("Number")}});
			}
			public Int32VM  Number{ get; private set; }
		}

		private class TestData{
			 public int Number { get; set; }
		}
	}

	// ReSharper restore InconsistentNaming
}
