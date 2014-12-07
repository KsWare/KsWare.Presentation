using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.BusinessFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test Class
	/// </summary>
	[TestClass]
	public class ListBMˑTests {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() { }

		[TestMethod]
		public void InvalidMetadataInitialization() {
			Assert.Throws<InvalidOperationException >(delegate {
				var bl = new ListBM<TestItemBM> {
					Metadata= {
						LogicProvider=new CustomListLogicProvider(OnCollectionChanged,OnCollectionChanged), // <-- invalid
						DataProvider={Data =new List<TestItemDM>()}
					}
				};				
			});
		}

		[TestMethod]
		public void BusinessList_Contains_Empty_DataList() {
			var dl = new List<TestItemDM>();
			var bl = new ListBM<TestItemBM> {
				Metadata= {
//TODO:				LogicProvider=new CustomListLogicProvider(OnCollectionChanged,OnCollectionChanged),
					DataProvider={Data =dl}
				}
			};
			var di = (TestItemDM) null;
			var bi = (TestItemBM) null;

			Assert.AreEqual(false,bl.IsReadOnly);
			Assert.AreEqual(false,((IList)bl).IsFixedSize);

			bl.CollectionChanged+=OnCollectionChanged;

			((IList) bl).Add(bi = new TestItemBM());
			bi.Metadata.DataProvider.Data = di = new TestItemDM(0);
			Assert.AreEqual(1, ((IList)bl).Count          );
			Assert.AreEqual(1,          dl.Count          );
			Assert.AreEqual(0,          bl.IndexOf(bi)   );
			Assert.AreEqual(0, ((IList)bl).IndexOf(bi)    );
			Assert.AreEqual(true, ((IList)bl).Contains(bi)   );
			Assert.AreEqual(bi,         bl[0]              );
			Assert.AreEqual(bi, ((IList)bl)[0]             );
			Assert.AreEqual(bl, bl[0].Parent              );
			bl.Add(new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}});
			Assert.AreEqual(2,          dl.Count            );
			bl.Add(new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(2)}}});
			Assert.AreEqual(3,          dl.Count            );
			((IList)bl).RemoveAt(1);
			Assert.AreEqual(2,          dl.Count          );
			((IList)bl).Remove(bi);
			Assert.IsNull(bi.Parent);
			Assert.AreEqual(1,dl.Count);
			((IList)bl).Clear();
			Assert.AreEqual(0,dl.Count);
			((IList)bl).Insert(0,new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(0)}}});
			bl.Insert(0,new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(1)}}});
			Assert.AreEqual(bl, bl[0].Parent               );
			bl.Insert(2,new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(2)}}});
			Assert.AreEqual(3,dl.Count);

			foreach (var item in bl) {
				Assert.IsTrue(bl.Contains(item));
			}
			foreach (var item in (IEnumerable)bl) {
				Assert.IsTrue(bl.Contains(item));
			}

			bl.CopyTo(new TestItemBM[3],0);
			bl[0] = new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(0)}}};
			((IList)bl)[0] = TestItemBM.New();
//			bl.Move(0,2);

			bl.CollectionChanged+=OnCollectionChanged;
			bl.Dispose();
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			
		}

		[TestMethod]
		public void InitializedList() {
			var dl = new List<TestItemDM>{new TestItemDM(0),new TestItemDM(1),new TestItemDM(2)};
			var bl = new ListBM<TestItemBM> {Metadata = {DataProvider = {Data = dl}}};
			Assert.AreEqual(3,bl.Count);
		}


		[TestMethod]
		public void RemoveNonContainedItem() {
			var dl = new List<TestItemDM>();
			var bl = new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};

			Assert.IsFalse(bl.Remove(new TestItemBM{Metadata={DataProvider={Data = new TestItemDM(99)}}}));
		}

		[TestMethod]
		public void ICollection() {
			var dl = new List<TestItemDM>(new []{new TestItemDM(0) });
			var bl = (ICollection)new ListBM<TestItemBM>{Metadata={DataProvider={Data =dl}}};
			
			bl.CopyTo(new TestItemBM[1],0);
			Assert.AreEqual(1,bl.Count);
			Assert.IsFalse(bl.IsSynchronized);
			Assert.IsNotNull(bl.SyncRoot);
		}

		[TestMethod]
		public void CheckReentrancyThrowsInvalidOperationException() {
			var dl = new List<TestItemDM>{new TestItemDM(0),new TestItemDM(1),new TestItemDM(2)};
			var bl = new ListBM<TestItemBM> {Metadata = {DataProvider = {Data = dl}}};

			bl.CollectionChanged+=OnViewModelCollectionChangedCallInvalid;
			bl.Add(TestItemBM.New());
			bl.CollectionChanged-=OnViewModelCollectionChangedCallInvalid;
		}

		private void OnViewModelCollectionChangedCallInvalid(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) {
			Assert.Throws<InvalidOperationException>(
				() => ((IList) sender).Clear()
			);
		}

		[TestMethod]
		public void BeginUpdateEndUpdate() {
			var dl = new List<TestItemDM>{new TestItemDM(0),new TestItemDM(1),new TestItemDM(2)};
			var bl = new ListBM<TestItemBM> {Metadata = {DataProvider = {Data = dl}}};

			bl.CollectionChanged+=OnViewModelCollectionChangedCallInvalid;
			bl.BeginUpdate();
			Assert.Throws<InvalidOperationException>(()=>bl.BeginUpdate());
			bl.Add(TestItemBM.New());
			bl.EndUpdate();
			Assert.Throws<InvalidOperationException>(()=>bl.EndUpdate());
			bl.CollectionChanged-=OnViewModelCollectionChangedCallInvalid;
		}

		[TestMethod]
		public void DataChange() {
			var dl = new List<TestItemDM>{new TestItemDM(0),new TestItemDM(1),new TestItemDM(2)};
			var bl = new ListBM<TestItemBM> {Metadata = {DataProvider = {Data = dl}}};

			dl = new List<TestItemDM>{new TestItemDM(3),new TestItemDM(4),new TestItemDM(5)};
			bl.Metadata.DataProvider.Data = dl;
			for (int i = 0; i < bl.Count; i++) {
				Assert.AreEqual(dl[i], bl[i].Metadata.DataProvider.Data);
			}
		}

		[TestMethod]
		public void DataListItemsChange() {
			var dl = new List<TestItemDM>{new TestItemDM(0),new TestItemDM(1),new TestItemDM(2)};
			var bl = new ListBM<TestItemBM> {Metadata = {DataProvider = {Data = dl}}};

			dl.RemoveAt(2);dl.Add(new TestItemDM(3));
			bl.Metadata.DataProvider.NotifyDataChanged();
			for (int i = 0; i < bl.Count; i++) {
				Assert.AreEqual(dl[i], bl[i].Metadata.DataProvider.Data);
			}
		}

		[TestMethod]
		public void SetParent() {
			var dl = new List<TestItemDM>();
			var bl = new DerivedList {Metadata = {DataProvider = {Data = dl}}};
			bl.Add(new TestItemBM());
			bl.Add(new TestItemBM());
		}

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void SetParent_DerivedList() {
			var bl = new DerivedList {Metadata = {DataProvider = {Data = new List<TestItemDM>()}}};
			var bi = new TestItemBM();
			bl.Add(new TestItemBM());
		}

        [TestMethod][Ignore] //TODO: Later activate this. Error in test of release version
		public void SetParent_InvalidParent() {
			var bl = new DerivedList {Metadata = {DataProvider = {Data = new List<TestItemDM>()}}};
			var bi = new TestItemBM();
			Assert.Throws<InvalidOperationException>(delegate { bi.Parent = bl; });
		}

		[TestMethod]
		public void AddSameItemToMultipleLists() {
			var bl = new DerivedList {Metadata = {DataProvider = {Data = new List<TestItemDM>()}}};
			var bi = new TestItemBM();
			bl.Add(bi);
			var bl2 = new DerivedList {Metadata = {DataProvider = {Data = new List<TestItemDM>()}}};
			Assert.Throws<InvalidOperationException>(delegate { bl2.Add(bi); });
		}

		private class DerivedList:ListBM<TestItemBM>{}
	}

	public class TestItemDM{public TestItemDM(int v) { V = v; } public int V{get;set;}}
		
	public class TestItemBM:ObjectBM{
		public static TestItemBM New() { return new TestItemBM {Metadata = {DataProvider = {Data = new TestItemDM(0)}}}; }
	}


	// ReSharper restore InconsistentNaming
}
