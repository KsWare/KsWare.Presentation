using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.ViewModelFramework {
	//// ReSharper disable InconsistentNaming

	/// <summary> Test Class
	/// </summary>
	[TestFixture]
	public class ListVMTests:ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public override void TestInitialize() {base.TestInitialize(); }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public override void TestCleanup() {base.TestCleanup(); }

		[Test]
		public void ListVM_Contains_Empty_BusinessList() {
			var dl = new List<TestItemDM>();
			var bl = new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};
			var vl = new ListVM<TestItemVM>{Metadata={DataProvider={Data =bl}}};
			var di = (TestItemDM) null;
			var vi = (TestItemVM) null;

			Assert.AreEqual(false,vl.IsReadOnly);
			Assert.AreEqual(false,((IList)vl).IsFixedSize);

			vl.Add(vi = new TestItemVM {Metadata = {DataProvider = {Data = new TestItemBM {Metadata = {DataProvider = {Data = di = new TestItemDM(0)}}}}}});
			Assert.AreEqual(1,          bl.Count          );
			Assert.AreEqual(1,          dl.Count          );
			Assert.AreEqual(0,          vl.IndexOf(vi)    );
			Assert.AreEqual(0, ((IList)vl).IndexOf(vi)    );
			Assert.AreEqual(true, ((IList)vl).Contains(vi)   );
			Assert.AreEqual(vi,         vl[0]              );
			Assert.AreEqual(vi, ((IList)vl)[0]             );
			vl.Add(new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}}}}});
			Assert.AreEqual(2,          dl.Count          );
			vl.Add(new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(2)}}}}}});
			Assert.AreEqual(3,          dl.Count          );
			vl.RemoveAt(1);
			Assert.AreEqual(2,          dl.Count          );
			vl.Remove(vi);
			Assert.AreEqual(1,dl.Count);
			vl.Clear();
			Assert.AreEqual(0,dl.Count);
			vl.Insert(0,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(0)}}}}}});
			vl.Insert(0,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}}}}});
			vl.Insert(2,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(2)}}}}}});
			Assert.AreEqual(3,dl.Count);

			foreach (var item in vl) {
				Assert.IsTrue(vl.Contains(item));
			}
			foreach (var item in (IEnumerable)vl) {
				Assert.IsTrue(vl.Contains(item));
			}

			vl.CopyTo(new TestItemVM[3],0);
			vl[0] = TestItemVM.New();
			((IList)vl)[0] = TestItemVM.New();
			vl.Move(0,2);
			vl.Dispose();
		}

		[Test]
		public void InitializedList() {
			var dl = new List<TestItemDM>{new TestItemDM(0),new TestItemDM(1),new TestItemDM(2)};
			var bl = new ListBM<TestItemBM> {Metadata = {DataProvider = {Data = dl}}};
			var vl = new ListVM<TestItemVM> {Metadata = {DataProvider = {Data = bl}}};
			Assert.AreEqual(3,vl.Count);

			bl.RemoveAt(1);
			Assert.AreEqual(2,vl.Count);
			bl.Remove(bl[0]);
			Assert.AreEqual(1,vl.Count);
			bl.Clear();
			Assert.AreEqual(0,vl.Count);

			bl.Add(new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(0)}}});
			Assert.AreEqual(1,vl.Count);
			bl.Insert(0,new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}});
			Assert.AreEqual(2,vl.Count);
			bl.Insert(2,new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}});
			Assert.AreEqual(3,vl.Count);
		}

		[Test]
		public void IList() {
			var dl = new List<TestItemDM>();
			var bl = new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};
			var vl = (IList) new ListVM<TestItemVM>{Metadata={DataProvider={Data =bl}}};
			var di = (TestItemDM) null;
			var vi = (TestItemVM) null;

			vl.Add(vi = new TestItemVM {Metadata = {DataProvider = {Data = new TestItemBM {Metadata = {DataProvider = {Data = di = new TestItemDM(0)}}}}}});
			Assert.AreEqual(1,bl.Count);
			Assert.AreEqual(1,dl.Count);
			Assert.AreEqual(0,vl.IndexOf(vi));
			vl.Add(new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}}}}});
			Assert.AreEqual(2,dl.Count);
			vl.Add(new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(2)}}}}}});
			Assert.AreEqual(3,dl.Count);
			vl.RemoveAt(1);
			Assert.AreEqual(2,dl.Count);
			vl.Remove(vi);
			Assert.AreEqual(1,dl.Count);
			vl.Clear();
			Assert.AreEqual(0,dl.Count);
			vl.Insert(0,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(0)}}}}}});
			vl.Insert(0,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}}}}});
			vl.Insert(2,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(2)}}}}}});
			Assert.AreEqual(3,dl.Count);
		}

		[Test]
		public void IListT() {
			var dl = new List<TestItemDM>();
			var bl = new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};
			var vl = (IList<TestItemVM>) new ListVM<TestItemVM>{Metadata={DataProvider={Data =bl}}};
			var di = (TestItemDM) null;
			var vi = (TestItemVM) null;

			vl.Add(vi = new TestItemVM {Metadata = {DataProvider = {Data = new TestItemBM {Metadata = {DataProvider = {Data = di = new TestItemDM(0)}}}}}});
			Assert.AreEqual(1,bl.Count);
			Assert.AreEqual(1,dl.Count);
			Assert.AreEqual(0,vl.IndexOf(vi));
			vl.Add(new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}}}}});
			Assert.AreEqual(2,dl.Count);
			vl.Add(new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(2)}}}}}});
			Assert.AreEqual(3,dl.Count);
			vl.RemoveAt(1);
			Assert.AreEqual(2,dl.Count);
			vl.Remove(vi);
			Assert.AreEqual(1,dl.Count);
			vl.Clear();
			Assert.AreEqual(0,dl.Count);
			vl.Insert(0,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(0)}}}}}});
			vl.Insert(0,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}}}}});
			vl.Insert(2,new TestItemVM{Metadata={DataProvider={Data = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(2)}}}}}});
			Assert.AreEqual(3,dl.Count);
		}

		[Test]
		public void RemoveNonContainedItem() {
			var dl = new List<TestItemDM>();
			var bl = new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};
			var vl = (IList<TestItemVM>) new ListVM<TestItemVM>{Metadata={DataProvider={Data =bl}}};

			Assert.AreEqual(false,vl.Remove(TestItemVM.New()));
		}

		[Test]
		public void ICollection() {
			var dl = new List<TestItemDM>(new []{new TestItemDM(0) });
			var bl = new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};
			var vl = (ICollection) new ListVM<TestItemVM>{Metadata={DataProvider={Data =bl}}};
			
			vl.CopyTo(new TestItemVM[1],0);
			Assert.AreEqual(1,vl.Count);
			Assert.AreEqual(false,vl.IsSynchronized);
			Assert.IsNotNull(vl.SyncRoot);
		}

		[Test]
		public void CheckReentrancyThrowsInvalidOperationException() {
			var dl = new List<TestItemDM>{new TestItemDM(0),new TestItemDM(1),new TestItemDM(2)};
			var bl = new ListBM<TestItemBM> {Metadata = {DataProvider = {Data = dl}}};
			var vl = new ListVM<TestItemVM> {Metadata = {DataProvider = {Data = bl}}};

			vl.CollectionChanged+=OnViewModelCollectionChangedCallInvalid;
			vl.Add(TestItemVM.New());
			vl.CollectionChanged-=OnViewModelCollectionChangedCallInvalid;
		}

		private void OnViewModelCollectionChangedCallInvalid(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) {
			Assert.Throws<InvalidOperationException>(
				() => ((IList) sender).Clear()
			);
		}

		[Test]
		public void Test1() {
			var dl = new List<TestItemDM>(new []{new TestItemDM(0) });
			var bl = new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};
			var vl = new ListVM<TestItemVM>{Metadata={DataProvider={Data =bl}}};
			
			dl = new List<TestItemDM>(new []{new TestItemDM(1) });
			bl = new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};

			TestItemVM oldFirst = vl.FirstOrDefault();
			TestItemVM newfirst = null;
			vl.CollectionChanged+=delegate {newfirst=vl.FirstOrDefault();};

			vl.Metadata.DataProvider.Data=bl;

			var oldFirstTypeInstanceId = oldFirst.TypeInstanceId();
			var newFirstTypeInstanceId=newfirst.TypeInstanceId();
			Assert.AreEqual(newFirstTypeInstanceId, oldFirstTypeInstanceId + 1);

		}

		[Test]
		public void Clear_NoUnderlyingObject() {
			var vm = new ListVM<TestItemVM>();
			vm.Clear();
		}


		public class TestItemDM{public TestItemDM(int v) { V = v; } public int V{get;set;}}
		public class TestItemBM:ObjectBM{}
		public class TestItemVM:ObjectVM{
			public static TestItemVM New() { return new TestItemVM {Metadata = {DataProvider = {Data = new TestItemBM {Metadata = {DataProvider = {Data = new TestItemDM(0)}}}}}}; }
		}
	}


	//// ReSharper restore InconsistentNaming
}
