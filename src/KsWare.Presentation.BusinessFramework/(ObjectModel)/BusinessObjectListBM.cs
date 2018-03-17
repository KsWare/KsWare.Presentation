using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {



	/// <summary> [OBSOLETE] Provides a list of business objects
	/// </summary>
	/// <typeparam name="TItem">Type of business object</typeparam>
	/// <remarks>This was the ListBM until 2014-08-07</remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class BusinessObjectListBM<TItem>:ObjectBM,IListBM<TItem>,IListBM where TItem:class,IObjectBM{

		/// <summary> Specifies the max number of delayed events. If the number of events exceeds this limit, only a single events is raised (Reset). 
		/// </summary>
		private const int MaxDelayedCollectionChangedEvents=10; //

		/// <summary> Don't use this field directly! Use <see cref="InnerList"/> instead. Only <see cref="InnerList"/> and <see cref="InitializeInnerList"/> should call this field. </summary>
		private List<TItem> _innerList;
		private List<TItem> _innerListOld;

		private bool _updating;
		private readonly List<NotifyCollectionChangedEventArgs> _delayedCollectionChangedEventArgs=new List<NotifyCollectionChangedEventArgs>();
		private readonly SimpleMonitor _collectionChangedMonitor= new SimpleMonitor();
		private SimpleMonitor _innerListInitializeMonitor= new SimpleMonitor();
		protected bool IsReferenceList;


		/// <summary> Initializes a new instance of the <see cref="BusinessObjectListBM{T}"/> class.
		/// </summary>
		public BusinessObjectListBM() {
			if(typeof(TItem).GetInterface(typeof(IValueBM).FullName)!=null) {
				Debug.WriteLine("=>WARNING: BusinessObjectListBM<" + typeof(TItem).FullName + "> is not fully supported!");
			}
			_innerList = new List<TItem>();
		}

		#region Metadata

		/// <summary> Creates the default metadata for the current type of business list .
		/// </summary>
		/// <returns>business list metadata</returns>
		protected override BusinessMetadata CreateDefaultMetadata() {
			return new BusinessListMetadata{DataProvider = new LocalDataProvider<IList>()};
		}
		
		
		/// <summary> Gets or sets the business object metadata.
		/// </summary>
		/// <value>The business object metadata.</value>
		public new BusinessListMetadata Metadata{get => (BusinessListMetadata) base.Metadata; set => base.Metadata = value; }// implemented for type cast

		#endregion

		#region Data

		/// <summary> Provides access to the underlying data list using the data provider specified in metadata.
		/// </summary>
		private IList DataList => (IList) Metadata.DataProvider.Data;

		/// <summary> Invoked after the underlying data (<see cref="DataList"/> / Metadata.DataProvider.Data) has been changed.
		/// </summary>
		/// <param name="e"></param>
		override protected void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);

			_innerListOld=_innerList;
			_innerList = null;
			_innerListInitializeMonitor=new SimpleMonitor(); // this indicates InnerList is not initialized

			InitializeInnerList();
		}

		private static object GetItemData(TItem item) { return item.Metadata.DataProvider.Data; }

		#endregion

		#region InnerList

		/// <summary> Provides access to the <see cref="m_InnerList"/> and forces that the list is initalized.
		/// </summary>
		private IList<TItem> InnerList {
			get {
				if(_innerListInitializeMonitor!=null) InitializeInnerList();
				return _innerList;
			}
		}

		/// <summary> Initializes the complete inner list of <see cref="ObjectBM"/>s.
		/// </summary>
		/// <remarks>
		/// This method is is invoked after the underlying data has been changed or before any list member is executed the first time.
		/// </remarks>
		private void InitializeInnerList() {
			const string noDataToken = "InitializeInnerList:NoData";
			if(_innerListInitializeMonitor==null/*this indicates InnerList is initialized*/) return;

			//check reentrancy
			if (_innerListInitializeMonitor.IsBusy) 
				throw new InvalidOperationException("Cannot initialize the list during an event! (Possibly recursive call)");

			var delayedevents = new List<InvokeArgs>();
			//block reentrancy
			using(_innerListInitializeMonitor.Enter()){

				IList dataList;
				//dataList = this.DataList;
				try { dataList = DataList;
					SetApplicable(noDataToken, true);
				} catch (Exception ex) {
					#region find the first object in hierarch which throws an exception on getting data:
					var hierarchyPath = new List<ObjectBM>();
					var p = (ObjectBM)Parent;
					while (p!=null) {hierarchyPath.Add(p);p = (ObjectBM)p.Parent;}
					hierarchyPath.Reverse();
					foreach (var p1 in hierarchyPath) {
						Exception ex2;
						p1.Metadata.DataProvider.TryGetData(out ex2);
						if (ex2 != null) {
							Debug.WriteLine("WARNING: "+p1.DebugːGetTypeːName+" MemberPath: " +p1.MemberPath+" get data throws "+ex2.GetType().Name+ " "+ex2.Message);
							break;
						}
					}
					#endregion

					Debug.WriteLine("WARNING: " + ex.GetType().Name + " caught! Set BusinessObjectListBM.IsApplicable set to false. " +
						"\n\tMemberPath: " + MemberPath+
						"\n\tUniqueID: {1336A7FC-E685-410F-AA03-93310C7742BD}");
					SetApplicable(noDataToken, false);
					dataList=new ArrayList();
				}

				if(_innerListOld!=null && _innerListOld.Count>0) {
					//REVISE: OnItemRemoved(..) -or- OnCleared()
					/*A:*/ while (_innerListOld.Count>0) {
						var item = _innerListOld[0]; _innerListOld.RemoveAt(0);
						delayedevents.Add(new InvokeArgs(new Action<int, TItem>(OnItemRemoved), new object[] {0, item}));//OnItemRemoved(0,item);
					}
					/*B:*/ // OnCleared();
				}

				if (dataList == null) {
				
				} else {
					_innerList=new List<TItem>();
					int i = -1;
					
					foreach (object data in dataList) {
						i++;
						var item = Metadata.NewItemProvider.CreateItem<TItem>(data);
						_innerList.Add(item);
						if(!IsReferenceList) {
							item.Parent = this;
						}
						/*A:*/ 
						delayedevents.Add(new InvokeArgs(new Action<int, TItem>(OnItemInserted), new object[] {0, item}));//OnItemInserted(i,item); 
						//REVISE: mayby use /*B:*/ OnCleared() after all inserts
					}
				}

				/*B:*/ // innerListBackingFieldOld.Clear(); OnCleared();

				_innerListOld = null;
			}
			_innerListInitializeMonitor = null;// this indicates InnerList is initialized

			//raise delayed events
//			foreach (var delayedevent in delayedevents) {
//				delayedevent.Function.DynamicInvoke(delayedevent.Args);
//			}

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		class InvokeArgs {

			public Delegate Function{get;private set;}
			public object[] Args{get;private set;}

			public InvokeArgs(Delegate function, object[] args) {
				Function = function;
				Args = args;
			}
		}

		#endregion

		#region IList<T>,IList,...,IEnumerable

		/// <summary> Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns> A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection. </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<TItem> GetEnumerator() { return InnerList.GetEnumerator(); }

		/// <summary> Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public void Add(TItem item) { InsertItem(InnerList.Count, item); }

		/// <summary> Adds an item to the <see cref="T:System.Collections.IList"/>.
		/// </summary>
		/// <returns> The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection. </returns>
		/// <param name="value">The object to add to the <see cref="T:System.Collections.IList"/>. </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
		/// <filterpriority>2</filterpriority>
		int IList.Add(object value) { Add((TItem) value); return InnerList.Count-1;}

		/// <summary> Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
		/// </summary>
		/// <returns> true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false. </returns>
		/// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param>
		/// <filterpriority>2</filterpriority>
		bool IList.Contains(object value) { return Contains((TItem) value);}

		/// <summary> Removes all items from the <see cref="T:System.Collections.IList"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception>
		/// <filterpriority>2</filterpriority>
		public void Clear() { ClearItems(); }

		/// <summary> Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
		/// </summary>
		/// <returns> The index of <paramref name="value"/> if found in the list; otherwise, -1. </returns>
		/// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param>
		/// <filterpriority>2</filterpriority>
		int IList.IndexOf(object value) { return IndexOf((TItem) value);}

		/// <summary> Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted. </param>
		/// <param name="value">The object to insert into the <see cref="T:System.Collections.IList"/>. </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
		/// <exception cref="T:System.NullReferenceException"><paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception>
		/// <filterpriority>2</filterpriority>
		void IList.Insert(int index, object value) { Insert(index, (TItem) value); }

		/// <summary> Removes the first occurrence of a specific object from the <see cref="System.Collections.IList"/>.
		/// </summary>
		/// <param name="value">The object to remove from the <see cref="System.Collections.IList"/>. </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
		/// <filterpriority>2</filterpriority>
		void IList.Remove(object value) { Remove((TItem) value); }

		/// <summary> Determines whether the <see cref="System.Collections.Generic.ICollection{T}"/> contains a specific value.
		/// </summary>
		/// <returns> true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		public bool Contains(TItem item) { return InnerList.Contains(item);}

		/// <summary> Copies the elements of the <see cref="System.Collections.Generic.ICollection{T}"/> to an <see cref="System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
		public void CopyTo(TItem[] array, int arrayIndex) { InnerList.CopyTo(array,arrayIndex); }

		/// <summary> Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns> true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>. </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public bool Remove(TItem item) { return RemoveItemHelper(item); }

		/// <summary> Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception><exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception><filterpriority>2</filterpriority>
		void ICollection.CopyTo(Array array, int index) { ((ICollection)InnerList).CopyTo(array,index); }

		/// <summary> Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <returns> The number of elements contained in the <see cref="T:System.Collections.ICollection"/>. </returns>
		/// <filterpriority>2</filterpriority>
		public int Count {
			get {
				if(_innerListInitializeMonitor!=null) { // use DataList if innerList not yet initialized
					var dataList = DataList;
					return dataList!=null ? dataList.Count : 0;
				}
				return InnerList.Count;
			}
		}

		/// <summary> Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <returns> An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>. </returns>
		/// <filterpriority>2</filterpriority>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object ICollection.SyncRoot => ((ICollection)InnerList).SyncRoot;

		/// <summary> Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
		/// </summary>
		/// <returns> true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false. </returns>
		/// <filterpriority>2</filterpriority>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool ICollection.IsSynchronized => ((ICollection)InnerList).IsSynchronized;

		/// <summary> Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
		/// </summary>
		/// <returns> true if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, false. </returns>
		/// <filterpriority>2</filterpriority>
		public bool IsReadOnly => ((IList)InnerList).IsReadOnly;

		/// <summary> Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
		/// </summary>
		/// <returns> true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false. </returns>
		/// <filterpriority>2</filterpriority>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool IList.IsFixedSize => ((IList)InnerList).IsFixedSize;

		/// <summary> Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </summary>
		/// <returns> The index of <paramref name="item"/> if found in the list; otherwise, -1. </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		public int IndexOf(TItem item) { return InnerList.IndexOf(item); }

		/// <summary> Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
		/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
		public void Insert(int index, TItem item) { InsertItem(index, item); }

		/// <summary> Removes the <see cref="T:System.Collections.IList"/> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove. </param><exception cref="T:System.ArgumentOutOfRangeException"> <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
		/// <filterpriority>2</filterpriority>
		public void RemoveAt(int index) { RemoveItem(index); }

		/// <summary> Gets or sets the element at the specified index.
		/// </summary>
		/// <returns> The element at the specified index. </returns>
		/// <param name="index">The zero-based index of the element to get or set. </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
		/// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"/> is read-only. </exception>
		/// <filterpriority>2</filterpriority>
		object IList.this[int index]{get => this[index]; set => this[index]=(TItem) value; }

		/// <summary> Gets or sets the element at the specified index.
		/// </summary>
		/// <returns> The element at the specified index. </returns>
		/// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
		/// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
		public TItem this[int index] {get => InnerList[index]; set => ReplaceItem(index, value); }

		/// <summary> Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns> An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection. </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		#endregion

		/// <summary> Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Blocks the reentrancy.
		/// </summary>
		/// <returns>
		/// A monitor which is used in <see langword="using"/>(){...}
		/// </returns>
		protected IDisposable BlockReentrancy() {
			return _collectionChangedMonitor.Enter();
		}

		/// <summary> Checks the reentrancy.
		/// </summary>
		protected void CheckReentrancy() {
			if ((_collectionChangedMonitor.IsBusy && (CollectionChanged != null)) && (CollectionChanged.GetInvocationList().Length > 0)) {
				throw new InvalidOperationException("Cannot change ObservableCollection during a CollectionChanged event.");
			}
		}

		/// <summary> Delays any event raising until <see cref="EndUpdate"/> is invoked.
		/// </summary>
		public void BeginUpdate() {
			if(_updating) throw new InvalidOperationException("Update operation already started!");
			_updating = true;
		}

		/// <summary> Disables event raising delay and raises all events since <see cref="BeginUpdate"/>-call.
		/// </summary>
		public void EndUpdate() {
			if(!_updating) throw new InvalidOperationException("Update operation not started!");
			_updating = false;
			if(CollectionChanged!=null) {

				if(_delayedCollectionChangedEventArgs.Count>MaxDelayedCollectionChangedEvents) {
					_delayedCollectionChangedEventArgs.Clear();
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				} else {
					while (_delayedCollectionChangedEventArgs.Count != 0) {
						OnCollectionChanged(_delayedCollectionChangedEventArgs[0]);
						_delayedCollectionChangedEventArgs.RemoveAt(0);
					}					
				}
			}
		}

		#region List actions
		// the following methods implements the list logic for all IList,Collection,... interface members 

		private void InsertItem(int index, TItem item) {
			CheckReentrancy();
			if(!IsReferenceList) {
				if (item.Parent != null) throw new InvalidOperationException("Item is already in a collection!");
			}
			if (index == -1) index = InnerList.Count;
			OnItemInserting(index, item);
			InnerList.Insert(index, item);
			if(!IsReferenceList) {
				item.Parent = this;
			}
			if(DataList!=null) DataList.Insert(index, GetItemData(item));
			OnItemInserted(index, item);		

		}

		private void RemoveItem(int index) { 
			CheckReentrancy();
			var item = InnerList[index];
			OnItemRemoving(index, item);
			InnerList.RemoveAt(index);
			if(!IsReferenceList) {
				item.Parent = null;
			}
			if(DataList!=null) DataList.RemoveAt(index);
			OnItemRemoved(index, item);
		}

		private bool RemoveItemHelper(TItem item) { 
			int idx = InnerList.IndexOf(item);
			if(idx<0) return false;
			RemoveItem(idx);
			return true;
		}

		private void ReplaceItem(int index, TItem newItem) { 
			CheckReentrancy();
			if(!IsReferenceList) {
				if (newItem.Parent != null) throw new InvalidOperationException("Item is already in a collection!");
			}
			var oldItem = InnerList[index];
			OnItemReplacing(index, oldItem, newItem);
			InnerList[index] = newItem;
			if(DataList!=null) DataList[index] = GetItemData(oldItem);
			if(!IsReferenceList) {
				newItem.Parent = this;
				oldItem.Parent = null;
			}
			OnItemReplaced(index, oldItem, newItem);
		}

		private void ClearItems() { 
			CheckReentrancy();
			OnClearing();
			if(!IsReferenceList) {
				foreach (var item in InnerList) {item.Parent = null;}
			}
			InnerList.Clear();
			if(DataList!=null) DataList.Clear();
			OnCleared();
		}

		#endregion

		#region raising events/callbacks

		/// <summary> Called before a item will be inserted.
		/// </summary>
		protected virtual void OnItemInserting(int index, TItem insertedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,insertedItem,index);

			if(Metadata.LogicProvider!=null && Metadata.LogicProvider.IsSupported) {
				Metadata.LogicProvider.CollectionChanging(args);
			}
		}

		/// <summary> Called when a item has been inserted.
		/// </summary>
		protected virtual void OnItemInserted(int index, TItem insertedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, insertedItem, index);

			if (Metadata.LogicProvider != null && Metadata.LogicProvider.IsSupported) {
				Metadata.LogicProvider.CollectionChanged(args);
			}

			if(ItemInserted!=null) ItemInserted(index,insertedItem);

			if (!_updating) OnCollectionChanged(args);
			else if (CollectionChanged != null) _delayedCollectionChangedEventArgs.Add(args);

			OnTreeChanged();
		}

		/// <summary> Gets or sets the callback method which is called for each inserted item.
		/// </summary>
		/// <remarks>This callback should be used only from the business object which contains the list.</remarks>
		public Action<int, TItem> ItemInserted { get;set;}

		/// <summary> Called before a item will be replaced.
		/// </summary>
		protected virtual void OnItemReplacing(int index, TItem removedItem, TItem insertedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,insertedItem, removedItem,index);

			if(Metadata.LogicProvider!=null && Metadata.LogicProvider.IsSupported) {
				Metadata.LogicProvider.CollectionChanging(args);
			}
		}

		/// <summary> Called when a item has been replaced.
		/// </summary>
		protected virtual void OnItemReplaced(int index, TItem removedItem, TItem insertedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,insertedItem, removedItem,index);

			if(Metadata.LogicProvider!=null && Metadata.LogicProvider.IsSupported) {
				Metadata.LogicProvider.CollectionChanged(args);
			}

			if (ItemRemoved  != null) ItemRemoved(index, removedItem);
			if (ItemInserted != null) ItemInserted(index, insertedItem);

			if(!_updating) OnCollectionChanged(args);
			else if(CollectionChanged!=null) _delayedCollectionChangedEventArgs.Add(args);

			OnTreeChanged();
		}

		/// <summary> Called before a item will be removed (<see cref="RemoveAt"/>, <see cref="Remove"/>).
		/// </summary>
		protected virtual void OnItemRemoving(int index, TItem removedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removedItem,index);
			
			if(Metadata.LogicProvider!=null && Metadata.LogicProvider.IsSupported) {
				Metadata.LogicProvider.CollectionChanging(args);
			}
		}

		/// <summary> Called when a item has been removed (<see cref="RemoveAt"/>, <see cref="Remove"/>).
		/// </summary>
		protected virtual void OnItemRemoved(int index, TItem removedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removedItem,index);

			if(Metadata.LogicProvider!=null && Metadata.LogicProvider.IsSupported) {
				Metadata.LogicProvider.CollectionChanged(args);
			}

			if (ItemRemoved!=null) ItemRemoved(index, removedItem);

			if(!_updating) OnCollectionChanged(args);
			else if(CollectionChanged!=null) _delayedCollectionChangedEventArgs.Add(args);

			OnTreeChanged();
		}

		/// <summary> Gets or sets the callback method which is called for each removed item.
		/// </summary>
		/// <remarks>This callback should be used only from the business object which contains the list.</remarks>
		public Action<int, TItem> ItemRemoved { get;set;}

		/// <summary> Called before all items will be removed (<see cref="Clear"/>).
		/// </summary>
		protected virtual void OnClearing() {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

			if(Metadata.LogicProvider!=null && Metadata.LogicProvider.IsSupported) {
				Metadata.LogicProvider.CollectionChanging(args);
			}
		}

		/// <summary>  Called when all items has been removed (<see cref="Clear"/>).
		/// </summary>
		protected virtual void OnCleared() {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

			if(Metadata.LogicProvider!=null && Metadata.LogicProvider.IsSupported) {
				Metadata.LogicProvider.CollectionChanged(args);
			}

			if(!_updating) OnCollectionChanged(args);
			else if(CollectionChanged!=null) _delayedCollectionChangedEventArgs.Add(args);

			OnTreeChanged();
		}

		/// <summary> Raises the System.Collections.ObjectModel.ObservableCollection{T}.CollectionChanged event with the provided arguments.
		/// </summary>
		/// <param name="e">Arguments of the event being raised.</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (CollectionChanged != null) {
				using (BlockReentrancy()) {
					if(TraceLevel>=3) Debug.WriteLine("BusinessObjectListBM.OnCollectionChanged {0} (Count:{1})",e.Action, Count);
					EventUtil.Raise(CollectionChanged,this,e,"{80369EB5-B7BE-4DA7-BE83-F711D2227D5A}");
				}
			}
			OnPropertyChanged(nameof(Count));
		}

		#endregion

		/// <summary> Adds the elements of the specified collection to the end of the <see cref="BusinessObjectListBM{TItem}"/>.
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the <see cref="BusinessObjectListBM{TItem}"/>. The collection itself cannot be null, but it can contain elements that are null, if type TItem is a reference type.</param>
		/// <seealso cref="List{TItem}.AddRange"/>
		public void AddRange(IEnumerable<TItem> collection) {
			foreach (var item in collection) {
				Add(item);
			}
		}

		public int TraceLevel { get; set; }

		/// <summary> [EXPERIMENTAL] Do things for each inserted or removed item
		/// </summary>
		/// <param name="insertedAction">The action which is executed when an item is inserted</param>
		/// <param name="removedAction">The action which is executed when an item is removed</param>
		/// <remarks> The behavior on <see cref="NotifyCollectionChangedAction.Reset"/> is undefined! </remarks>
		public void ForEachItem(Action<int, TItem> insertedAction, Action<int, TItem> removedAction) {
			ItemInserted = insertedAction;
			ItemRemoved  = removedAction;
		}

	}

	/// <summary> A <see cref="BusinessObjectListBM{T}"/> w/o setting the <see cref="IObjectBM.Parent"/> of items.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RefBusinessObjectListBM<T> :BusinessObjectListBM<T> where T : class, IObjectBM {
		public RefBusinessObjectListBM() {
			IsReferenceList = true;
		}
	}
}
