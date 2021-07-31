using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Windows.Data;
using System.Windows.Threading;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {


	/// <summary> Provides a list for view model with functionality of a <see cref="List{TItem}"/>
	/// </summary>
	/// <typeparam name="TItem">Type of item view model</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public partial class ListVM<TItem>:ObjectVM, IListVM<TItem>, IListVM /*, IBindingList*/ /*where TItem:class, IObjectVM*/ {

		//// ReSharper disable VirtualMemberNeverOverriden.Global

//		static readonly CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");

		private string _objectKey; //for managing instance 
		private bool _isDisposed;

		private readonly List<TItem> _innerList;
		private IList _cachedDataList;
		private MethodInfo _cachedDataListMove;
		private ListVM<TItem>.ItemMap _tmpMapIns;
		private ItemMap _tmpMapRem;
		private readonly SimpleMonitor _monitor;
		[NonSerialized] private NotifyCollectionChangedEventHandler _collectionChanged;
		protected bool IsReferenceList;
		protected bool IsViewModelList;
		
		private TItem _itemTemplate;

		private ICollectionView _collectionView;
		private bool _isInBackgroundMode;

		private const string CountString = "Count";
		private const string IndexerName = "Item[]";

		/// <summary> Initializes a new instance of the <see cref="ListVM{TItem}"/> class.
		/// </summary>
		public ListVM() {
			base.DebuggerFlags=new ClassDebuggerFlags();
			_innerList     = new List<TItem>();
			_monitor       = new SimpleMonitor();

			IsViewModelList = typeof (IObjectVM).IsAssignableFrom(typeof (TItem));

//			try {_itemTemplate = (TItem)Activator.CreateInstance(typeof(TItem));}catch(Exception ex) {
//				Debug.WriteLine("WARNING: Can not create instance for ItemTemplate!"
//					+"\n\t"+ ex.Message
//					+"\n\tModel: "+"ListVM<"+ typeof(TItem).Name+">"
//					+"\n\tPath: "+this.MemberPath
//					+"\r\tItem Type: "+typeof(TItem).Name
//					+"\r\tErrorID: {08B0C478-4AE0-41B6-A588-B306067DB09C}"
//				);
//			}
		}

		#region IList<TItem>,IList,ICollection<TItem>,ICollection,IEnumerable<TItem>,IEnumerable

		/// <summary> Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns> A <see cref="System.Collections.Generic.IEnumerator{T}"/> that can be used to iterate through the collection. </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<TItem> GetEnumerator() { return InnerList.GetEnumerator(); }

		/// <summary> Adds an item to the <see cref="ICollection{TItem}"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="ICollection{TItem}"/>.</param>
		/// <exception cref="System.NotSupportedException">The <see cref="ICollection{TItem}"/> is read-only.</exception>
		public void Add(TItem item)  {InsertItem(InnerList.Count,item);}

		/// <summary> Adds an item to the <see cref="IList"/>.
		/// </summary>
		/// <returns> The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection. </returns>
		/// <param name="value">The object to add to the <see cref="IList"/>. </param>
		/// <exception cref="System.NotSupportedException">The <see cref="IList"/> is read-only.-or- The <see cref="IList"/> has a fixed size. </exception>
		/// <filterpriority>2</filterpriority>
		int IList.Add(object value) { return InsertItem(InnerList.Count,(TItem) value);}

		/// <summary> Determines whether the <see cref="IList"/> contains a specific value.
		/// </summary>
		/// <returns> true if the <see cref="System.Object"/> is found in the <see cref="IList"/>; otherwise, false. </returns>
		/// <param name="value">The object to locate in the <see cref="IList"/>. </param>
		/// <filterpriority>2</filterpriority>
		bool IList.Contains(object value) { return Contains((TItem) value);}

		/// <summary> Removes all items from the <see cref="IList"/>.
		/// </summary>
		/// <exception cref="System.NotSupportedException">The <see cref="IList"/> is read-only. </exception>
		/// <filterpriority>2</filterpriority>
		public void Clear() {ClearItems();}

		/// <summary> Determines the index of a specific item in the <see cref="IList"/>.
		/// </summary>
		/// <returns> The index of <paramref name="value"/> if found in the list; otherwise, -1. </returns>
		/// <param name="value">The object to locate in the <see cref="IList"/>. </param>
		/// <filterpriority>2</filterpriority>
		int IList.IndexOf(object value) { return IndexOf((TItem) value);}

		/// <summary> Inserts an item to the <see cref="IList"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted. </param>
		/// <param name="value">The object to insert into the <see cref="IList"/>. </param>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="IList"/>. </exception><exception cref="System.NotSupportedException">The <see cref="IList"/> is read-only.-or- The <see cref="IList"/> has a fixed size. </exception>
		/// <exception cref="System.NullReferenceException"><paramref name="value"/> is null reference in the <see cref="IList"/>.</exception>
		/// <filterpriority>2</filterpriority>
		void IList.Insert(int index, object value) { Insert(index, (TItem) value); }

		/// <summary> Removes the first occurrence of a specific object from the <see cref="IList"/>.
		/// </summary>
		/// <param name="value">The object to remove from the <see cref="IList"/>. </param>
		/// <exception cref="System.NotSupportedException">The <see cref="IList"/> is read-only.-or- The <see cref="IList"/> has a fixed size. </exception>
		/// <filterpriority>2</filterpriority>
		void IList.Remove(object value) { Remove((TItem) value); }

		/// <summary> Determines whether the <see cref="ICollection{TItem}"/> contains a specific value.
		/// </summary>
		/// <returns> true if <paramref name="item"/> is found in the <see cref="ICollection{TItem}"/>; otherwise, false. </returns>
		/// <param name="item">The object to locate in the <see cref="ICollection{TItem}"/>.</param>
		public bool Contains(TItem item) { return InnerList.Contains(item);}

		/// <summary> Copies the elements of the <see cref="ICollection{TItem}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection{TItem}"/>. 
		/// The <see cref="Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
		/// <exception cref="System.ArgumentException">
		///     <paramref name="array"/> is multidimensional. <br/> -or- <br/>
		///     The number of elements in the source <see cref="ICollection{TItem}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.<br/> -or- <br/>
		///     Type TItem cannot be cast automatically to the type of the destination <paramref name="array"/>.
		/// </exception>
		public void CopyTo(TItem[] array, int arrayIndex) { InnerList.CopyTo(array,arrayIndex); }

		/// <summary> Removes the first occurrence of a specific object from the <see cref="ICollection{TItem}"/>.
		/// </summary>
		/// <returns> true if <paramref name="item"/> was successfully removed from the <see cref="ICollection{TItem}"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="ICollection{TItem}"/>. </returns>
		/// <param name="item">The object to remove from the <see cref="ICollection{TItem}"/>.</param><exception cref="System.NotSupportedException">The <see cref="ICollection{TItem}"/> is read-only.</exception>
		public bool Remove(TItem item) { 
			int idx = IndexOf(item);
			if(idx<0) return false;
			RemoveAt(idx);
			return true;
		}

		/// <summary> Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection"/>. The <see cref="Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. </param><exception cref="System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. </exception><exception cref="System.ArgumentException"><paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception><exception cref="System.ArgumentException">The type of the source <see cref="ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception><filterpriority>2</filterpriority>
		void ICollection.CopyTo(Array array, int index) { ((ICollection)InnerList).CopyTo(array,index); }

		/// <summary> Gets the number of elements contained in the <see cref="ICollection"/>.
		/// </summary>
		/// <returns> The number of elements contained in the <see cref="ICollection"/>. </returns>
		/// <filterpriority>2</filterpriority>
		public int Count{get {if(DebuggerFlags.Breakpoints.Count) DebuggerːBreak("Count"); return InnerList.Count;}}

		/// <summary> Gets an object that can be used to synchronize access to the <see cref="ICollection"/>.
		/// </summary>
		/// <returns> An object that can be used to synchronize access to the <see cref="ICollection"/>. </returns>
		/// <filterpriority>2</filterpriority>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object ICollection.SyncRoot => ((ICollection)InnerList).SyncRoot;

		/// <summary> Gets a value indicating whether access to the <see cref="ICollection"/> is synchronized (thread safe).
		/// </summary>
		/// <returns> true if access to the <see cref="ICollection"/> is synchronized (thread safe); otherwise, false. </returns>
		/// <filterpriority>2</filterpriority>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool ICollection.IsSynchronized => ((ICollection)InnerList).IsSynchronized;

//		/// <summary> Gets a value indicating whether the <see cref="IList"/> is read-only.
//		/// </summary>
//		/// <returns> true if the <see cref="IList"/> is read-only; otherwise, false. </returns>
//		/// <filterpriority>2</filterpriority>
//		//TOOD: IsReadOnly List|ObjectVM ??
		bool IList.IsReadOnly => ((IList)InnerList).IsReadOnly;
//		public bool new IsReadOnly{get {return ((IList)InnerList).IsReadOnly;}}

		/// <summary> Gets a value indicating whether the <see cref="IList"/> has a fixed size.
		/// </summary>
		/// <returns> true if the <see cref="IList"/> has a fixed size; otherwise, false. </returns>
		/// <filterpriority>2</filterpriority>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool IList.IsFixedSize => ((IList)InnerList).IsFixedSize;

		/// <summary> Determines the index of a specific item in the <see cref="System.Collections.Generic.IList{TItem}"/>.
		/// </summary>
		/// <returns> The index of <paramref name="item"/> if found in the list; otherwise, -1. </returns>
		/// <param name="item">The object to locate in the <see cref="System.Collections.Generic.IList{TItem}"/>.</param>
		public int IndexOf(TItem item) { return InnerList.IndexOf(item); }

		/// <summary> Inserts an item to the <see cref="System.Collections.Generic.IList{TItem}"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
		/// <param name="item">The object to insert into the <see cref="System.Collections.Generic.IList{TItem}"/>.</param>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="System.Collections.Generic.IList{TItem}"/>.</exception>
		/// <exception cref="System.NotSupportedException">The <see cref="System.Collections.Generic.IList{TItem}"/> is read-only.</exception>
		public void Insert(int index, TItem item) {InsertItem(index, item);}

		/// <summary> Removes the <see cref="IList"/> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove. </param><exception cref="System.ArgumentOutOfRangeException"> <paramref name="index"/> is not a valid index in the <see cref="IList"/>. </exception>
		/// <exception cref="System.NotSupportedException">The <see cref="System.Collections.IList"/> is read-only.-or- The <see cref="System.Collections.IList"/> has a fixed size. </exception>
		/// <filterpriority>2</filterpriority>
		public void RemoveAt(int index) {RemoveItem(index, null);}

		/// <summary> Gets or sets the element at the specified index.
		/// </summary>
		/// <returns> The element at the specified index. </returns>
		/// <param name="index">The zero-based index of the element to get or set. </param>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="IList"/>. </exception>
		/// <exception cref="System.NotSupportedException">The property is set and the <see cref="IList"/> is read-only. </exception>
		/// <filterpriority>2</filterpriority>
		object IList.this[int index]{get => GetItem(index); set => ReplaceItem(index, (TItem) value); }

		/// <summary> Gets or sets the element at the specified index.
		/// </summary>
		/// <returns> The element at the specified index. </returns>
		/// <param name="index">The zero-based index of the element to get or set.</param><exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="System.Collections.Generic.IList{TItem}"/>.</exception>
		/// <exception cref="System.NotSupportedException">The property is set and the <see cref="System.Collections.Generic.IList{TItem}"/> is read-only.</exception>
		public TItem this[int index] {get => GetItem(index); set => ReplaceItem(index, value); }

		/// <summary> Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns> An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection. </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		#endregion


		/// <summary> Moves the item at the specified index to a new location in the collection.
		/// </summary>
		/// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
		/// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public void Move(int oldIndex, int newIndex) {MoveItem(oldIndex, newIndex);}

		/// <summary> Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="explicitDisposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
		/// <remarks></remarks>
		protected override void Dispose(bool explicitDisposing) {
			base.Dispose(explicitDisposing);
			if(explicitDisposing) {
				_objectKey = this.GetObjectKey(); // gets a unique key which ist used after Dispose to identify this instance
				_isDisposed = true;
				this.DebugObjectTraceˑDispose(explicitDisposing);
			}
		}

		protected virtual void DemandValidObject() {
			if(_isDisposed) throw new ObjectDisposedException(_objectKey);
		}

		#region Metadata

		/// <summary> Creates the default metadata.
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected override ViewModelMetadata CreateDefaultMetadata() {
			return new ListViewModelMetadata();
		}

		/// <summary> Gets or sets the metadata.
		/// </summary>
		/// <value>The metadata.</value>
		/// <remarks></remarks>
		public new ListViewModelMetadata Metadata{get => (ListViewModelMetadata) base.Metadata; set => base.Metadata = value; }

		#endregion

		private List<TItem> InnerList {
			get {
				// TODO because the data has been changed silently (no event) we have to check on each access!
				// BUT an update while a get operation will have side effects (trigger events)

				// DRAFT Solution A: check only if we not have data, suppress any events.
				// so we get an initialized list on first access.
				if (_cachedDataList == null) {
					Exception exception;
					var data=Metadata.DataProvider.TryGetData(out exception);
					if (data != null) {
						Debug.WriteLine("WARNING: Unexpected data changed! Continue w/o raising any events! " +
								        "\n\t"+"Type: " + DebugUtil.FormatTypeName(this) +
								        "\n\t"+"UniqueID: {3BEA2E12-8CC6-4952-8714-4C57FC370764}");
						SuppressAnyEvents++;
						UpdateInnerList(new DataChangedEventArgs(_cachedDataList,data));
						SuppressAnyEvents--;
					}
				}
				return _innerList;
			}
		}

		#region Data

		protected override void OnDataChanged(DataChangedEventArgs e) {
			if(DebuggerFlags.Breakpoints.OnDataChanged) DebuggerːBreak("OnDataChanged");
//			if (_IsInBackgroundMode) UpdateInnerList(e);
//			ApplicationDispatcher.InvokeIfRequired(UpdateInnerList, e);
			UpdateInnerList(e);
		}

		private void UpdateInnerList(DataChangedEventArgs e) {

			#region clean-up cached data
			if(_cachedDataList!=null) {
				if (_cachedDataList is INotifyCollectionChanged) {
					((INotifyCollectionChanged) _cachedDataList).CollectionChanged -= AtDataCollectionChanged;
				}
				if (_cachedDataList is IModel) {
					((IModel) _cachedDataList).Disposed -= AtDataDisposed;
				}
				_cachedDataListMove = null;
			}
			#endregion

			//var tmpDataList = (IList) newDataList;
			IList tmpDataList = null;
			if (e.NewData == null || e.NewData == DBNull.Value) {
				if(_cachedDataList==null) /*no changes*/ return;
			} else if(e.NewData is IList ){
				tmpDataList = (IList) e.NewData;
			} else if(e.NewData is IEnumerable ){
				// not supported
				Debug.WriteLine("WARNING: Type of data not supported!"+
					"\n\tType: "+DebugUtil.FormatTypeName(e.NewData)+
					"\n\tUniqueID: {78776B46-69D9-4D01-A7A9-C107D605FDB7}"
				);
			}
			SetEnabled("ListVM:AtDataChanged:NoData",tmpDataList!=null);
			if (tmpDataList == null) {
				ClearInnerList();
			} else {
				ClearInnerList();
				if (tmpDataList is IListBM) {
					var bl = (IListBM) tmpDataList;
					try {bl.GetEnumerator();/*this will force the ListBM to initialize*/} catch(Exception ex) {
						this.DoNothing(ex); 
						tmpDataList = null;
						if (bl.IsApplicable) throw;
					}
					SetEnabled("ListVM:AtDataChanged:IListBM.NotApplicable",bl.IsApplicable);
				}
			}
			if (tmpDataList != null) {
				foreach (object data in tmpDataList) {
					var item = Metadata.NewItemProvider.CreateItem<TItem>(data);
					_innerList.Add(item);
					if(!IsReferenceList) {
						var hirarchicalItem = item as IHierarchical<IObjectVM>;
						if(hirarchicalItem!=null) {
							hirarchicalItem.Parent = this;
							hirarchicalItem.MemberName = string.Format(EnUs,"[{0}]", (_innerList.Count - 1));
							//hirarchicalItem.MemberName = "[?]";
						}						
					}
				}
			}

			_cachedDataList = tmpDataList;

			if(_cachedDataList!=null) {
				if(_cachedDataList is INotifyCollectionChanged) {
					((INotifyCollectionChanged)_cachedDataList).CollectionChanged+=AtDataCollectionChanged;
				}
				if(_cachedDataList is IModel) {
					((IModel)_cachedDataList).Disposed+=AtDataDisposed;
				}

				_cachedDataListMove = _cachedDataList.GetType().GetMethod("Move", new[] {typeof (int), typeof (int)});				
			}
			
			OnCollectionReset();
		}

		private void ClearInnerList() {
			foreach (TItem item in _innerList) CleanupRemovedItem(item);
			_innerList.Clear();
		}

		private void CleanupRemovedItem(TItem item) {
			if(!IsViewModelList) return; // EXPERIMENTAL if item is no IObjectVM we do nothing
			if(Equals(item,null)) return; // for whatever reason?! REVISE sometime  foreach _InnerList returns a null item

			var hierarchicalItem = item as IHierarchical<IObjectVM>;
			if (hierarchicalItem!=null) {
				hierarchicalItem.Parent = null;
				hierarchicalItem.MemberName = null;
			}
			var viewModelItem = item as IObjectVM;
			if (viewModelItem != null) {
				viewModelItem.Metadata.DataProvider.Data = null;
			}
		}

		private void AtDataDisposed(object sender, EventArgs e) { 
			this.DoNothing();
		}

		/// <summary> Occurs if the underlying data collection changes.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		/// <seealso cref="m_CachedDataList"/>
		/// <seealso cref="INotifyCollectionChanged"/>
		/// <seealso cref="INotifyCollectionChanged.CollectionChanged"/>
		private void AtDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add: {
					if(e.NewItems.Count!=1) throw new NotImplementedException("{04009FB6-1A37-4A8A-9B44-EAEE29B073CE}");
					TItem newItem;
					int index=e.NewStartingIndex;
					if(_tmpMapIns!=null) {
						//triggered by this list
						newItem = _tmpMapIns.Item;
					} else /*triggered by underlying list*/ {
						if (IsReferenceList && Metadata.ValueSourceProvider.SourceList != null) {
							//EXPERIMENTAL 2014-0-15
							newItem=(TItem) Metadata.ValueSourceProvider.SourceList.Cast<IBusinessObjectVM>().FirstOrDefault(x => ReferenceEquals(x.BusinessObject, e.NewItems[0]));
						} else {
							newItem = Metadata.NewItemProvider.CreateItem<TItem>(e.NewItems[0]);
						}
					}
					_innerList.Insert(index,newItem);
					if(!IsReferenceList) {
						var hierarchicalItem = newItem as IHierarchical<IObjectVM>;
						if(hierarchicalItem!=null) {
							hierarchicalItem.Parent = this;
							//hierarchicalItem.MemberName = string.Format("[{0}]", (InnerList.Count - 1));
							hierarchicalItem.MemberName = "[?]";
						}						
					}
					OnItemInserted(index, newItem);

//TODO (optional): implement case {e.NewItems.Count > 1}
//					int idx = e.NewStartingIndex;
//					foreach (object data in e.NewItems) {
//						var item = Metadata.NewItemProvider.CreateItem<TItem>(data);
//						InnerList.Insert(idx,item);
//						idx++;
//					}
					break;
				}
				case NotifyCollectionChangedAction.Remove:{
					int idx = e.OldStartingIndex;
					foreach (object data in e.OldItems) {
						TItem oldItem = _innerList[idx];
						_innerList.RemoveAt(idx);
						OnItemRemoved(idx,oldItem);
						CleanupRemovedItem(oldItem);
					}
					break;
				}
				case NotifyCollectionChangedAction.Replace:{
					if(e.NewItems.Count!=1) throw new NotImplementedException("{15C55634-0EC1-40DB-96D6-ADEE7C2ACA6E}");
					TItem newItem,oldItem;
					int index=e.NewStartingIndex;
					if(_tmpMapIns!=null && _tmpMapRem==null) throw new NotImplementedException("{0002AE66-4DB4-4505-BD89-2007FAFEFA9E}");
					if(_tmpMapIns==null && _tmpMapRem!=null) throw new NotImplementedException("{18F96D1A-9190-4DA8-8395-CF2D262B54DB}");
					if(_tmpMapIns!=null && _tmpMapRem!=null) {
						//triggered by this list
						newItem = _tmpMapIns.Item;
						oldItem = _tmpMapRem.Item;
					} else {
						//triggered by underlying list
						newItem = Metadata.NewItemProvider.CreateItem<TItem>(e.NewItems[0]);
						oldItem = _innerList[index];
					}
					CleanupRemovedItem(oldItem);
					_innerList[index]=newItem;
					var hierarchicalItem = newItem as IHierarchical<IObjectVM>;
					if(hierarchicalItem!=null) {
						hierarchicalItem.Parent = this;
						//hirarchicalItem.MemberName = string.Format("[{0}]", (InnerList.Count - 1));
						hierarchicalItem.MemberName = "[?]";
					}
					OnItemReplaced(index,oldItem, newItem);

//TODO (optional): implement case {e.NewItems.Count > 1}
//					int idx = e.NewStartingIndex;
//					foreach (object data in e.NewItems) {
//						var item = Metadata.NewItemProvider.CreateItem<TItem>(data);
//						InnerList[idx]=item;
//						idx++;
//					}
					break;
				}
				case NotifyCollectionChangedAction.Move:{
					if(e.NewItems.Count>1)throw new NotImplementedException("{3B5993CB-D286-4A3D-9FE5-4DC5EDBA1052}"); 

					var item = _innerList[e.OldStartingIndex];
					_innerList.RemoveAt(e.OldStartingIndex);
					_innerList.Insert(e.NewStartingIndex,item);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,item,e.NewStartingIndex,e.OldStartingIndex));
					break;
				}
				case NotifyCollectionChangedAction.Reset:{
					ClearInnerList();
					foreach (object data in _cachedDataList) {
						var item = Metadata.NewItemProvider.CreateItem<TItem>(data);
						_innerList.Add(item);
						if(!IsReferenceList) {
							var hirarchicalItem = item as IHierarchical<IObjectVM>;
							if (hirarchicalItem != null) {
								hirarchicalItem.Parent = this;
								hirarchicalItem.MemberName = string.Format(EnUs,"[{0}]", (_innerList.Count - 1));
								hirarchicalItem.MemberName = "[?]";
							}							
						}
					}
					OnCollectionReset();
					break;
				}
			}

		}


		/// <summary> Gets the item data (short for item.Metadata.DataProvider.Data)
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		/// <remarks>If <typeparamref name="TItem"/> is no view model then <c>null</c> is returned.</remarks>
		protected virtual object GetItemData(TItem item) {
			var vm = item as IObjectVM; if(vm==null) return null;
			return vm.Metadata.DataProvider.Data;
		}

		#endregion Data

		/// <summary> Blocks reentrancy.
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected IDisposable BlockReentrancy() {
			_monitor.Enter();
			return _monitor;
		}

		/// <summary> Checks reentrancy.
		/// </summary>
		/// <remarks></remarks>
		protected void CheckReentrancy() {
			if ((_monitor.IsBusy && (_collectionChanged != null)) && (_collectionChanged.GetInvocationList().Length > 0)) {
				throw new InvalidOperationException("Cannot change ObservableCollection during a CollectionChanged event.");
			}
		}

		#region Item action logic

		/// <summary> Inserts the item.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual int InsertItem(int index, TItem item) { 
			if(DebuggerFlags.Breakpoints.Add) DebuggerːBreak("Add");
			if(DebuggerFlags.Breakpoints.Insert) DebuggerːBreak("Insert");
			CheckReentrancy();
			OnItemInserting(index, item);
			
			_tmpMapIns=new ItemMap(item, GetItemData(item));
			this.DoNothing(InnerList);//forces InnerList init
			if(_cachedDataList!=null) {
				if(_tmpMapIns.Data==null) {
					if(item is IListItemVM) {
						_tmpMapIns.Data = ((IListItemVM) item).DataRequired();
					}
					if(_tmpMapIns.Data==null) throw new InvalidOperationException("Item.Data must not be null! \r\n\tErrorID:{0DE8AEA7-04D6-49D2-9F17-D59BF527613F}");//REVISE: create new Data?
				}
				_cachedDataList.Insert(index, _tmpMapIns.Data);
			}
			if(_cachedDataList==null || !(_cachedDataList is INotifyCollectionChanged)) {
				InnerList.Insert(index, item);
				if(IsViewModelList && !IsReferenceList) {
					((IObjectVM)item).Parent = this;
				}
				OnItemInserted(index, item);
			}
			_tmpMapIns = null;
			return index;
		}

		/// <summary> Removes the item.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual bool RemoveItem(int index, object item) { 
			if(DebuggerFlags.Breakpoints.Remove) DebuggerːBreak("Remove");
			if(DebuggerFlags.Breakpoints.RemoveAt) DebuggerːBreak("RemoveAt");
			this.CheckReentrancy();
			_tmpMapRem = new ItemMap(InnerList[index], GetItemData(InnerList[index]));
			OnItemRemoving(index,_tmpMapRem.Item);
			if(_cachedDataList!=null) {
				_cachedDataList.RemoveAt(index);
			}
			if(_cachedDataList==null || !(_cachedDataList is INotifyCollectionChanged)) {
				InnerList.RemoveAt(index);
				OnItemRemoved(index, _tmpMapRem.Item);
			}
			_tmpMapRem = null;
			return true;
		}

		protected virtual TItem GetItem(int index) {
			if(DebuggerFlags.Breakpoints.ThisGet) DebuggerːBreak("ThisGet");
			return InnerList[index];
		}

		/// <summary> Replaces the item.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual TItem ReplaceItem(int index, TItem value) { //alias SetItem
			if(DebuggerFlags.Breakpoints.ThisSet) DebuggerːBreak("ThisSet");
			CheckReentrancy();
			TItem replaced;
			_tmpMapRem=new ItemMap(replaced=InnerList[index],GetItemData(InnerList[index]));
			_tmpMapIns=new ItemMap(value,GetItemData(value));
			OnItemReplacing(index, _tmpMapRem.Item, value);
			if(_cachedDataList!=null) {
				_cachedDataList[index] = _tmpMapIns.Data;
			}
			if (_cachedDataList != null && (_cachedDataList is INotifyCollectionChanged)) {
				//done, continue handling event -> AtDataCollectionChanged
			} else {
				InnerList[index] = value;
				OnItemReplaced(index, _tmpMapRem.Item, value);
			}
			_tmpMapRem = _tmpMapIns = null;
			return replaced;
		}

		/// <summary> Moves the item.
		/// </summary>
		/// <param name="oldIndex">The old index.</param>
		/// <param name="newIndex">The new index.</param>
		/// <remarks></remarks>
		protected virtual void MoveItem(int oldIndex, int newIndex){
			CheckReentrancy();
			TItem movedItem;
			_tmpMapIns=new ItemMap(movedItem=InnerList[oldIndex],GetItemData(InnerList[oldIndex]));

			if((_cachedDataList is INotifyCollectionChanged) && _cachedDataListMove!=null) {
				// (_cachedDataList is ObservableCollection)
				_cachedDataListMove.Invoke(_cachedDataList,new object[]{oldIndex,newIndex});
				//done, continue handling event -> AtDataCollectionChanged
			} else {
				if(_cachedDataList!=null) {
					_cachedDataList.RemoveAt(oldIndex);
					_cachedDataList.Insert(newIndex, _tmpMapIns.Data);
				}
				if (_cachedDataList is INotifyCollectionChanged) {
					//done, continue handling event -> AtDataCollectionChanged
				} else {
					InnerList.RemoveAt(oldIndex);
					InnerList.Insert(newIndex, _tmpMapIns.Item);
					OnItemMoved(movedItem, oldIndex, newIndex);
				}
			}
			_tmpMapIns = null;
		}

		/// <summary> Removes all items in a single operation.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void ClearItems() {
			if(DebuggerFlags.Breakpoints.Clear) DebuggerːBreak("Clear");
			CheckReentrancy();
			this.DoNothing(InnerList);//forces InnerList init
			OnClearing();
			if(_cachedDataList!=null) {
				_cachedDataList.Clear();
			}
			if(_cachedDataList==null || !(_cachedDataList is INotifyCollectionChanged)) {
				InnerList.Clear();
				OnCollectionReset();
			}
		}

		#endregion

		/// <summary>Called before a item will be inserted.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void OnItemInserting(int index, TItem insertedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, insertedItem, index);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangingCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangingCallback(this, args);
			}
		}

		/// <summary>Called when a item has been inserted.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void OnItemInserted(int index, TItem insertedItem) {

			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, insertedItem, index);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangedCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangedCallback(this, args);
			}

			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			if(index==0                  ) OnPropertyChanged("First");
			if(index==InnerList.Count-1) OnPropertyChanged("Last");

			OnCollectionChanged(args);
		}

		/// <summary>Called before a item will be replaced.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void OnItemReplacing(int index, TItem removedItem, TItem insertedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,insertedItem, removedItem,index);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangingCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangingCallback(this,args);
			}
		}

		/// <summary>Called when a item has been replaced.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void OnItemReplaced(int index, TItem removedItem, TItem insertedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,insertedItem, removedItem,index);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangedCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangedCallback(this,args);
			}
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			if(index==0                  ) OnPropertyChanged("First");
			if(index==InnerList.Count-1) OnPropertyChanged("Last");
			OnCollectionChanged(args);
		}

		/// <summary>Called before a item will be removed.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void OnItemRemoving(int index, TItem removedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removedItem,index);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangingCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangingCallback(this,args);
			}
		}

		/// <summary>Called when a item has been removed.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void OnItemRemoved(int index, TItem removedItem) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removedItem,index);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangedCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangedCallback(this,args);
			}
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			if(index==0                ) OnPropertyChanged("First");
			if(index==InnerList.Count) OnPropertyChanged("Last");
			OnCollectionChanged(args);
		}

		/// <summary> Called when a item has been moved.
		/// </summary>
		/// <remarks></remarks>
		private void OnItemMoved(TItem movedItem, int oldIndex, int newIndex) {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,movedItem, newIndex, oldIndex);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangedCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangedCallback(this,args);
			}
			OnPropertyChanged("Item[]");
			if(newIndex==0                  ||oldIndex==0) OnPropertyChanged("First");
			if(newIndex==InnerList.Count-1||oldIndex==InnerList.Count-1) OnPropertyChanged("Last");
			OnCollectionChanged(args);
		}

		/// <summary> Called when collection has been cleared.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void OnClearing() {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangingCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangingCallback(this,args);
			}
		}

		/// <summary> Called when collection has been reseted.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void OnCollectionReset() {
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			if(Metadata.ExternalControlProvider!=null &&  Metadata.ExternalControlProvider.CollectionChangedCallback!=null) {
				Metadata.ExternalControlProvider.CollectionChangedCallback(this,args);
			}
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(args);
		}

		protected override void OnPropertyChanged(string propertyName) {
			var raiseEvent = new Action(() => {
					base.OnPropertyChanged(propertyName);
				});

			if (_isInBackgroundMode) ApplicationDispatcher.Invoke(DispatcherPriority.Background, raiseEvent);
			else ApplicationDispatcher.InvokeIfRequired(raiseEvent);
		}

		/// <summary> Raises the <see cref="CollectionChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks></remarks>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if(SuppressAnyEvents!=0) return;

			var raiseEvent = new Action(() => {
					using (BlockReentrancy()) {
						EventUtil.Raise(_collectionChanged, this, e, "{FDAFEEF3-5008-40B1-83A6-BBA5D45E9AD5}");
						EventManager.Raise<NotifyCollectionChangedEventHandler,NotifyCollectionChangedEventArgs>(LazyWeakEventStore, "CollectionChangedEvent", e);
					}
				});

			if (_isInBackgroundMode) ApplicationDispatcher.BeginInvoke(DispatcherPriority.Background, raiseEvent);
			else ApplicationDispatcher.InvokeIfRequired(raiseEvent);

//			if (SuppressAnyEvents == 0) {
//				if (_collectionChanged != null) {
//					using (BlockReentrancy()) {
//						_collectionChanged(this, e);
//					}
//				}
//				using (BlockReentrancy()) {
//					WeakEventManager.Raise(CollectionChangedEvent,e);
//				}				
//			}
		}

		/// <summary> Occurs when the collection changes.
		/// </summary>
		/// <remarks></remarks>
		public event NotifyCollectionChangedEventHandler CollectionChanged {
			// TODO [xgksc 2012-11-27] revise this complicated stuff!
			add {
				NotifyCollectionChangedEventHandler handler2;
				NotifyCollectionChangedEventHandler collectionChanged = _collectionChanged;
				do {
					handler2 = collectionChanged;
					var handler3 = (NotifyCollectionChangedEventHandler) Delegate.Combine(handler2, value);
					collectionChanged = Interlocked.CompareExchange<NotifyCollectionChangedEventHandler>(ref _collectionChanged, handler3, handler2);
				} while (collectionChanged != handler2);
			}
			remove {
				NotifyCollectionChangedEventHandler handler2;
				NotifyCollectionChangedEventHandler collectionChanged = _collectionChanged;
				do {
					handler2 = collectionChanged;
					var handler3 = (NotifyCollectionChangedEventHandler) Delegate.Remove(handler2, value);
					collectionChanged = Interlocked.CompareExchange<NotifyCollectionChangedEventHandler>(ref _collectionChanged, handler3, handler2);
				} while (collectionChanged != handler2);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<NotifyCollectionChangedEventHandler> CollectionChangedEvent => EventSources.Get<NotifyCollectionChangedEventHandler>("CollectionChangedEvent");

		/// <summary> Returns the first element of a sequence, or a default value if the sequence contains no elements.
		/// </summary>
		public TItem First => this.FirstOrDefault();

		/// <summary> Returns the last element of a sequence, or a default value if the sequence contains no elements.
		/// </summary>
		public TItem Last => this.LastOrDefault();

		[Obsolete("DRAFT")]
		public TItem ItemTemplate {
			get {
				if (_itemTemplate == null) {
					Debug.WriteLine("WARNING: Possible NullReferenceException!"
						+"\n\t"+"Property: "+ "ItemTemplate"
						+"\r\t"+"Property Type: "+typeof(TItem).Name
						+"\n\t"+"Model: "+DebugUtil.FormatTypeName(this)
						+"\n\t"+"Path: "+this.MemberPath
						+"\r\t"+"ErrorID: {08B0C478-4AE0-41B6-A588-B306067DB09C}"
					);					
				}
				return _itemTemplate;
			}
			set {
				MemberAccessUtil.DemandNotNull(value,null,this,"{AD627602-2FF2-4C71-9827-863FDDE8D2B5}");
				_itemTemplate=value;
				OnPropertyChanged("ItemTemplate");
			}
		}

		public ICollectionView CollectionView {
			get => _collectionView ??CollectionViewSource.GetDefaultView(this);
			set { _collectionView = value; OnPropertyChanged("CollectionView");}
		}

		public TItem Next(TItem item) {
			var idx = IndexOf(item);
			if(idx<0) throw new ArgumentException("Item is not in collection!",nameof(item));
			if(idx==Count-1) throw new ArgumentOutOfRangeException(nameof(item),"Item is the last item!");
			return this[idx + 1];
		}
		public TItem Previous(TItem item) {
			var idx = IndexOf(item);
			if(idx<0) throw new ArgumentException("Item is not in collection!",nameof(item));
			if(idx==0) throw new ArgumentOutOfRangeException(nameof(item),"Item is the first item!");
			return this[idx - 1];
		}

		public override string ToString() {
			return string.Format("{{Count: {0} {{{1}<{2}>}} Path:{3}}}", Count, GetType().Name, typeof(TItem).Name,MemberPath);
		}


		
		// ReSharper restore VirtualMemberNeverOverriden.Global

		/// <summary> [EXPERIMENTAL]
		/// </summary>
		public void BeginBackgroundUpdate() {
			_isInBackgroundMode = true;
		}

		/// <summary> [EXPERIMENTAL]
		/// </summary>
		public void EndBackgroundUpdate() {
			_isInBackgroundMode = false;
		}

	}






}
