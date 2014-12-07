using System.Collections.Generic;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewModelFramework {
	[TestClass]
	public class ListVM_CustomDataProvider_NotifyDataChangedTests:ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public override void TestInitialize() { base.TestInitialize();}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public override void TestCleanup() {base.TestCleanup(); }

		private List<TestData> m_DataList;
		
		[TestMethod]
		public void NotifyDataChangedTest() {
			m_DataList=new List<TestData> {new TestData()};
			var listVM = new ListVM<TestVM> {Metadata = new ListViewModelMetadata {DataProvider = new CustomDataProvider<IEnumerable<TestData>>(GetData, null)}};
			Assert.AreEqual(1,listVM.Count);
			m_DataList.Add(new TestData());
			Assert.AreEqual(1,listVM.Count);
			//listVM.DebuggerFlags.Breakpoints.OnDataChanged = true;
			listVM.Metadata.DataProvider.NotifyDataChanged();
			Assert.AreEqual(2,listVM.Count);
		}

		private IEnumerable<TestData> GetData() {return m_DataList;}

		private class TestData{}
		private class TestVM:DataVM<TestData>{} 
	}
}
