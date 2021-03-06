﻿using System.Collections.Generic;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	[TestClass]
	public class ListVMˑCustomDataProviderˑNotifyDataChangedTests:ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public override void TestInitialize() { base.TestInitialize();}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public override void TestCleanup() {base.TestCleanup(); }

		private List<TestData> _dataList;
		
		[TestMethod]
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
