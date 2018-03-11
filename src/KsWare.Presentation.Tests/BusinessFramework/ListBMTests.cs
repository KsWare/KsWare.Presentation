using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KsWare.Presentation.BusinessFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.BusinessFramework {

	[TestClass]
	public class ListBMTests {

		// DataProvider: DefaultDataProvider
		// Data:         NoData, List, Array, IEnumerable, ICollection, ICollection<?>, IList<?>, IList
		// Methods:      Add, Remove, RemoteAt, Indexer/Get, Indexer/Set, Clear
		//               AddRange


		[TestMethod]
		public void Add_NoUnderlyingObject() {
			var bm = new ListBM<TestItemBM>();
			bm.Add(new TestItemBM());
			Assert.AreEqual(1,bm.Count,"Count");
		}

		[TestMethod]
		public void RemoveAt_NoUnderlyingObject() {
			var bm = new ListBM<TestItemBM>();
			bm.Add(new TestItemBM());
			bm.RemoveAt(0);
			Assert.AreEqual(0,bm.Count,"Count");
		}

		[TestMethod]
		public void Remove_NoUnderlyingObject() {
			var bm = new ListBM<TestItemBM>();
			TestItemBM item=new TestItemBM();
			bm.Add(item);
			bm.Remove(item);
			Assert.AreEqual(0,bm.Count,"Count");
		}

		[TestMethod]
		public void Clear_NoUnderlyingObject() {
			var bm = new ListBM<TestItemBM>();
			bm.Add(new TestItemBM());
			bm.Clear();
			Assert.AreEqual(0,bm.Count,"Count");
		}

		// ### 


		[TestMethod]
		public void Add_DefaultDataProvider_List() {
			var dm = new List<TestItemDM>();
			var bm = new ListBM<TestItemBM>{MːData = dm};

			bm.Add(new TestItemBM{MːData = new TestItemDM(100)});
			
			Assert.AreEqual(1,bm.Count,"Count");
			Assert.AreEqual(1,dm.Count,"Data.Count");
		}

		[TestMethod]
		public void RemoveAt_DefaultDataProvider_List() {
			var dm = new List<TestItemDM>(new []{new TestItemDM(100)});
			var bm = new ListBM<TestItemBM>{MːData = dm};

			bm.RemoveAt(0);

			Assert.AreEqual(0,bm.Count,"Count");
			Assert.AreEqual(0,dm.Count,"Data.Count");
		}

		[TestMethod]
		public void Remove_DefaultDataProvider_List() {
			var dm = new List<TestItemDM>(new []{new TestItemDM(100)});
			var bm = new ListBM<TestItemBM>{MːData = dm};

			bm.Remove(bm[0]);

			Assert.AreEqual(0,bm.Count,"Count");
			Assert.AreEqual(0,dm.Count,"Data.Count");
		}

		[TestMethod]
		public void Clear_DefaultDataProvider_List() {
			var dm = new List<TestItemDM>(new []{new TestItemDM(100)});
			var bm = new ListBM<TestItemBM>{MːData = dm};

			bm.Clear();

			Assert.AreEqual(0,bm.Count,"Count");
			Assert.AreEqual(0,dm.Count,"Data.Count");
		}

		// ###

		[TestMethod]
		public void ReadOnly_OriginalBehavior() {
			List<string>               a = new List<string>();
			ReadOnlyCollection<string> b = a.AsReadOnly();
			ICollection<string> c = b;
			IList d=b;
			IList<string> e=b;
			
//			c.Add("C");		// System.NotSupportedException: Collection is read-only.
//			d.Add("D");		// System.NotSupportedException: Collection is read-only.
//			e.Insert(0,"E");// System.NotSupportedException: Collection is read-only.

			d = new string[] {"A", "B"};
//			d.Add("F");		// System.NotSupportedException: Collection was of a fixed size.
		}


		[TestMethod]
		[ExpectedException(typeof(NotSupportedException), AllowDerivedTypes = true)]
		public void Add_DefaultDataProvider_Array() {
			var dm = new []{new TestItemDM(100),new TestItemDM(101)};
			var bm = new ListBM<TestItemBM>{MːData = dm};

			bm.Add(new TestItemBM{MːData = new TestItemDM(100)});
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException), AllowDerivedTypes = true)]
		public void RemoveAt_DefaultDataProvider_Array() {
			var dm = new []{new TestItemDM(100),new TestItemDM(101)};
			var bm = new ListBM<TestItemBM>{MːData = dm};

			bm.RemoveAt(0);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException), AllowDerivedTypes = true)]
		public void Remove_DefaultDataProvider_Array() {
			var dm = new []{new TestItemDM(100),new TestItemDM(101)};
			var bm = new ListBM<TestItemBM>{MːData = dm};

			bm.Remove(bm[0]);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException), AllowDerivedTypes = true)]
		public void Clear_DefaultDataProvider_Array() {
			var dm = new []{new TestItemDM(100),new TestItemDM(101)};
			var bm = new ListBM<TestItemBM>{MːData = dm};
			
			bm.Clear();
		}

		[TestMethod]
		public void SetData_DefaultDataProvider_List_to_Array() {
			var dm1 = new List<TestItemDM>(new []{new TestItemDM(100)});
			var dm2 = new []{new TestItemDM(102),new TestItemDM(103)};
			var bm = new ListBM<TestItemBM>{MːData = dm1};
			
			Assert.AreEqual(dm1.Count,bm.Count,"Count");
			bm.MːData = dm2;
			Assert.AreEqual(dm2.Length,bm.Count,"Count");
		}


		public class TestItemDM{public TestItemDM(int v) { V = v; } public int V{get;set;}}
		public class TestItemBM:ObjectBM{}

	}

}