using System.Collections.Generic;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	[TestFixture]
	public class ListVMˑCustomDataProviderˑNotifyDataChangedTests:ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public override void TestInitialize() { base.TestInitialize();}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public override void TestCleanup() {base.TestCleanup(); }

		private List<TestData> _dataList;
		
		[Test]
		public void NotifyDataChangedTest() {
			_dataList=new List<TestData> {new TestData()};
			var listVM = new ListVM<TestVM> {Metadata = new ListViewModelMetadata {DataProvider = new CustomDataProvider<IEnumerable<TestData>>(GetData, null)}};
			Assert.AreEqual(1,listVM.Count);
			_dataList.Add(new TestData());
			Assert.AreEqual(1,listVM.Count);
			//listVM.DebuggerFlags.Breakpoints.OnDataChanged = true;
			listVM.Metadata.DataProvider.NotifyDataChanged();
			Assert.AreEqual(2,listVM.Count);
		}

		private IEnumerable<TestData> GetData() {return _dataList;}

		private class TestData{}
		private class TestVM:DataVM<TestData>{} 
	}
}
