// Type: System.Windows.Data.ListCollectionView
// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0F086D49-4CAD-43AD-A3E2-A5268BD16302
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationFramework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Runtime;
using System.Windows.Data;
using System.Windows.Threading;
using MS.Internal;
using MS.Internal.Data;

namespace KsWare.Presentation.DataVirtualization.MSOriginal {

	public class ListCollectionViewOriginal : CollectionViewOriginal, IComparer, IEditableCollectionViewAddNewItem, IEditableCollectionView/*, ICollectionViewLiveShaping*/, IItemProperties {

		private object _newItem = CollectionViewOriginal.NoNewItem;
		private bool? _isLiveSorting = new bool?(false);
		private bool? _isLiveFiltering = new bool?(false);
		private bool? _isLiveGrouping = new bool?(false);
		private IList _internalList;
		private CollectionViewGroupRoot _group;
		private bool _isGrouping;
		private IComparer _activeComparer;
		private Predicate<object> _activeFilter;
		private SortDescriptionCollection _sort;
		private IComparer _customSort;
		private ArrayList _shadowCollection;
		private bool _currentElementWasRemoved;
		private object _editItem;
		private int _newItemIndex;
		private NewItemPlaceholderPosition _newItemPlaceholderPosition;
		private bool _isItemConstructorValid;
		private ConstructorInfo _itemConstructor;
		private List<Action> _deferredActions;
		private ObservableCollection<string> _liveSortingProperties;
		private ObservableCollection<string> _liveFilteringProperties;
		private ObservableCollection<string> _liveGroupingProperties;
		private bool _isLiveShapingDirty;
		private bool _isRemoving;
		private const double LiveSortingDensityThreshold = 0.8;
		private const int _unknownIndex = -1;

		public override bool CanGroup { get { return true; } }

		public override ObservableCollection<GroupDescription> GroupDescriptions { get { return this._group.GroupDescriptions; } }

		public override ReadOnlyObservableCollection<object> Groups {
			get {
				if (!this.IsGrouping) return (ReadOnlyObservableCollection<object>) null;
				else return this._group.Items;
			}
		}

		public override SortDescriptionCollection SortDescriptions {
			get {
				if (this._sort == null) this.SetSortDescriptions(new SortDescriptionCollection());
				return this._sort;
			}
		}

		public override bool CanSort { get { return true; } }

		public override bool CanFilter { get { return true; } }

		public override Predicate<object> Filter {
			get { return base.Filter; }
			set {
				if (this.AllowsCrossThreadChanges) this.VerifyAccess();
				if (this.IsAddingNew || this.IsEditingItem)
					throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
						(object) "Filter"
					}));
				else base.Filter = value;
			}
		}


		public IComparer CustomSort {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._customSort; }
			set {
				if (this.AllowsCrossThreadChanges) this.VerifyAccess();
				if (this.IsAddingNew || this.IsEditingItem) {
					throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
						(object) "CustomSort"
					}));
				} else {
					this._customSort = value;
					this.SetSortDescriptions((SortDescriptionCollection) null);
					this.RefreshOrDefer();
				}
			}
		}

		[DefaultValue(null)]
		public virtual GroupDescriptionSelectorCallback GroupBySelector {
			get { return this._group.GroupBySelector; }
			set {
				if (!this.CanGroup) throw new NotSupportedException();
				if (this.IsAddingNew || this.IsEditingItem) {
					throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
						(object) "Grouping"
					}));
				} else {
					this._group.GroupBySelector = value;
					this.RefreshOrDefer();
				}
			}
		}

		public override int Count {
			get {
				this.VerifyRefreshNotDeferred();
				return this.InternalCount;
			}
		}

		public override bool IsEmpty { get { return this.InternalCount == 0; } }

		public bool IsDataInGroupOrder { get { return this._group.IsDataInGroupOrder; } set { this._group.IsDataInGroupOrder = value; } }

		public NewItemPlaceholderPosition NewItemPlaceholderPosition {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._newItemPlaceholderPosition; }
			set {
				this.VerifyRefreshNotDeferred();
				if (value != this._newItemPlaceholderPosition && this.IsAddingNew) throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringTransaction", (object) "NewItemPlaceholderPosition", (object) "AddNew"));
				else if (value != this._newItemPlaceholderPosition && this._isRemoving) { this.DeferAction((Action) (() => this.NewItemPlaceholderPosition = value)); } else {
					NotifyCollectionChangedEventArgs args = (NotifyCollectionChangedEventArgs) null;
					int num1 = -1;
					int num2 = -1;
					switch (value) {
						case NewItemPlaceholderPosition.None:
							switch (this._newItemPlaceholderPosition) {
								case NewItemPlaceholderPosition.AtBeginning:
									num1 = 0;
									args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, CollectionViewOriginal.NewItemPlaceholder, num1);
									break;
								case NewItemPlaceholderPosition.AtEnd:
									num1 = this.InternalCount - 1;
									args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, CollectionViewOriginal.NewItemPlaceholder, num1);
									break;
							}
							break;
						case NewItemPlaceholderPosition.AtBeginning:
							switch (this._newItemPlaceholderPosition) {
								case NewItemPlaceholderPosition.None:
									num2 = 0;
									args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, CollectionViewOriginal.NewItemPlaceholder, num2);
									break;
								case NewItemPlaceholderPosition.AtEnd:
									num1 = this.InternalCount - 1;
									num2 = 0;
									args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, CollectionViewOriginal.NewItemPlaceholder, num2, num1);
									break;
							}
							break;
						case NewItemPlaceholderPosition.AtEnd:
							switch (this._newItemPlaceholderPosition) {
								case NewItemPlaceholderPosition.None:
									num2 = this.InternalCount;
									args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, CollectionViewOriginal.NewItemPlaceholder, num2);
									break;
								case NewItemPlaceholderPosition.AtBeginning:
									num1 = 0;
									num2 = this.InternalCount - 1;
									args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, CollectionViewOriginal.NewItemPlaceholder, num2, num1);
									break;
							}
							break;
					}
					if (args == null) return;
					this._newItemPlaceholderPosition = value;
					if (!this.IsGrouping) { this.ProcessCollectionChangedWithAdjustedIndex(args, num1, num2); } else {
						if (num1 >= 0) this._group.RemoveSpecialItem(num1 == 0 ? 0 : this._group.Items.Count - 1, CollectionViewOriginal.NewItemPlaceholder, false);
						if (num2 >= 0) this._group.InsertSpecialItem(num2 == 0 ? 0 : this._group.Items.Count, CollectionViewOriginal.NewItemPlaceholder, false);
					}
					this.OnPropertyChanged("NewItemPlaceholderPosition");
				}
			}
		}


		public bool CanAddNew {
			get {
				if (!this.IsEditingItem && !this.SourceList.IsFixedSize) return this.CanConstructItem;
				else return false;
			}
		}

		public bool CanAddNewItem {
			get {
				if (!this.IsEditingItem) return !this.SourceList.IsFixedSize;
				else return false;
			}
		}

		private bool CanConstructItem {
			get {
				if (!this._isItemConstructorValid) this.EnsureItemConstructor();
				return this._itemConstructor != (ConstructorInfo) null;
			}
		}

		public bool IsAddingNew { get { return this._newItem != CollectionViewOriginal.NoNewItem; } }

		public object CurrentAddItem {
			get {
				if (!this.IsAddingNew) return (object) null;
				else return this._newItem;
			}
		}

		public bool CanRemove {
			get {
				if (!this.IsEditingItem && !this.IsAddingNew) return !this.SourceList.IsFixedSize;
				else return false;
			}
		}

		public bool CanCancelEdit { get { return this._editItem is IEditableObject; } }

		public bool IsEditingItem { get { return this._editItem != null; } }

		public object CurrentEditItem {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._editItem; }
		}

		public bool CanChangeLiveSorting { get { return true; } }

		public bool CanChangeLiveFiltering { get { return true; } }

		public bool CanChangeLiveGrouping { get { return true; } }

		public bool? IsLiveSorting {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._isLiveSorting; }
			set {
				if (!value.HasValue) throw new ArgumentNullException("value");
				bool? nullable1 = value;
				bool? nullable2 = this._isLiveSorting;
				if ((nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : (nullable1.HasValue != nullable2.HasValue ? 1 : 0)) == 0) return;
				this._isLiveSorting = value;
				this.RebuildLocalArray();
				this.OnPropertyChanged("IsLiveSorting");
			}
		}

		public bool? IsLiveFiltering {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._isLiveFiltering; }
			set {
				if (!value.HasValue) throw new ArgumentNullException("value");
				bool? nullable1 = value;
				bool? nullable2 = this._isLiveFiltering;
				if ((nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : (nullable1.HasValue != nullable2.HasValue ? 1 : 0)) == 0) return;
				this._isLiveFiltering = value;
				this.RebuildLocalArray();
				this.OnPropertyChanged("IsLiveFiltering");
			}
		}

		public bool? IsLiveGrouping {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._isLiveGrouping; }
			set {
				if (!value.HasValue) throw new ArgumentNullException("value");
				bool? nullable1 = value;
				bool? nullable2 = this._isLiveGrouping;
				if ((nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : (nullable1.HasValue != nullable2.HasValue ? 1 : 0)) == 0) return;
				this._isLiveGrouping = value;
				this.RebuildLocalArray();
				this.OnPropertyChanged("IsLiveGrouping");
			}
		}

		private bool IsLiveShaping {
			get {
				bool? isLiveSorting = this.IsLiveSorting;
				if ((!isLiveSorting.GetValueOrDefault() ? 0 : (isLiveSorting.HasValue ? 1 : 0)) == 0) {
					bool? isLiveFiltering = this.IsLiveFiltering;
					if ((!isLiveFiltering.GetValueOrDefault() ? 0 : (isLiveFiltering.HasValue ? 1 : 0)) == 0) {
						bool? isLiveGrouping = this.IsLiveGrouping;
						if (isLiveGrouping.GetValueOrDefault()) return isLiveGrouping.HasValue;
						else return false;
					}
				}
				return true;
			}
		}

		public ObservableCollection<string> LiveSortingProperties {
			get {
				if (this._liveSortingProperties == null) {
					this._liveSortingProperties = new ObservableCollection<string>();
					this._liveSortingProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnLivePropertyListChanged);
				}
				return this._liveSortingProperties;
			}
		}

		public ObservableCollection<string> LiveFilteringProperties {
			get {
				if (this._liveFilteringProperties == null) {
					this._liveFilteringProperties = new ObservableCollection<string>();
					this._liveFilteringProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnLivePropertyListChanged);
				}
				return this._liveFilteringProperties;
			}
		}

		public ObservableCollection<string> LiveGroupingProperties {
			get {
				if (this._liveGroupingProperties == null) {
					this._liveGroupingProperties = new ObservableCollection<string>();
					this._liveGroupingProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnLivePropertyListChanged);
				}
				return this._liveGroupingProperties;
			}
		}

		public ReadOnlyCollection<ItemPropertyInfo> ItemProperties { get { return this.GetItemProperties(); } }

		protected bool UsesLocalArray {
			get {
				if (this.ActiveComparer != null || this.ActiveFilter != null) return true;
				if (!this.IsGrouping) return false;
				bool? isLiveGrouping = this.IsLiveGrouping;
				if (isLiveGrouping.GetValueOrDefault()) return isLiveGrouping.HasValue;
				else return false;
			}
		}

		protected IList InternalList {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._internalList; }
		}

		protected IComparer ActiveComparer {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._activeComparer; }
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set { this._activeComparer = value; }
		}

		protected Predicate<object> ActiveFilter {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._activeFilter; }
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set { this._activeFilter = value; }
		}

		protected bool IsGrouping {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._isGrouping; }
		}

		protected int InternalCount {
			get {
				if (this.IsGrouping) return this._group.ItemCount;
				int num = this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.None ? 0 : 1;
				if (this.UsesLocalArray && this.IsAddingNew) ++num;
				return num + this.InternalList.Count;
			}
		}

		internal ArrayList ShadowCollection {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._shadowCollection; }
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set { this._shadowCollection = value; }
		}

		internal bool HasSortDescriptions {
			get {
				if (this._sort != null) return this._sort.Count > 0;
				else return false;
			}
		}

		private bool IsCurrentInView {
			get {
				if (0 <= this.CurrentPosition) return this.CurrentPosition < this.InternalCount;
				else return false;
			}
		}

		private bool CanGroupNamesChange { get { return true; } }

		private IList SourceList { get { return this.SourceCollection as IList; } }

		internal bool IsLiveShapingDirty {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._isLiveShapingDirty; }
			set {
				if (value == this._isLiveShapingDirty) return;
				this._isLiveShapingDirty = value;
				if (!value) return;
				this.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Delegate) new Action(this.RestoreLiveShaping));
			}
		}

		public ListCollectionViewOriginal(IList list)
			: base((IEnumerable) list) {
			if (this.AllowsCrossThreadChanges)
				BindingOperations.AccessCollection((IEnumerable) list, (Action) (() => {
					this.ClearPendingChanges();
					this.ShadowCollection = new ArrayList((ICollection) this.SourceCollection);
					this._internalList = (IList) this.ShadowCollection;
				}), false);
			else this._internalList = list;
			if (this.InternalList.Count == 0) this.SetCurrent((object) null, -1, 0);
			else this.SetCurrent(this.InternalList[0], 0, 1);
			this._group = new CollectionViewGroupRoot((CollectionViewOriginal) this);
			this._group.GroupDescriptionChanged += new EventHandler(this.OnGroupDescriptionChanged);
			this._group.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupChanged);
			this._group.GroupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupByChanged);
		}

		protected override void RefreshOverride() {
			if (this.AllowsCrossThreadChanges)
				BindingOperations.AccessCollection(this.SourceCollection, (Action) (() => {
					lock (this.SyncRoot) {
						this.ClearPendingChanges();
						this.ShadowCollection = new ArrayList((ICollection) this.SourceCollection);
					}
				}), false);
			object currentItem = this.CurrentItem;
			int num1 = this.IsEmpty ? -1 : this.CurrentPosition;
			bool currentAfterLast = this.IsCurrentAfterLast;
			bool currentBeforeFirst = this.IsCurrentBeforeFirst;
			this.OnCurrentChanging();
			this.PrepareLocalArray();
			if (currentBeforeFirst || this.IsEmpty) this.SetCurrent((object) null, -1);
			else if (currentAfterLast) { this.SetCurrent((object) null, this.InternalCount); } else {
				int newPosition = this.InternalIndexOf(currentItem);
				if (newPosition < 0) {
					int num2 = this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning ? 1 : 0;
					object newItem;
					if (num2 < this.InternalCount && (newItem = this.InternalItemAt(num2)) != CollectionViewOriginal.NewItemPlaceholder) this.SetCurrent(newItem, num2);
					else this.SetCurrent((object) null, -1);
				} else this.SetCurrent(currentItem, newPosition);
			}
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			this.OnCurrentChanged();
			if (this.IsCurrentAfterLast != currentAfterLast) this.OnPropertyChanged("IsCurrentAfterLast");
			if (this.IsCurrentBeforeFirst != currentBeforeFirst) this.OnPropertyChanged("IsCurrentBeforeFirst");
			if (num1 != this.CurrentPosition) this.OnPropertyChanged("CurrentPosition");
			if (currentItem == this.CurrentItem) return;
			this.OnPropertyChanged("CurrentItem");
		}

		public override bool Contains(object item) {
			this.VerifyRefreshNotDeferred();
			return this.InternalContains(item);
		}

		public override bool MoveCurrentToPosition(int position) {
			this.VerifyRefreshNotDeferred();
			if (position < -1 || position > this.InternalCount) throw new ArgumentOutOfRangeException("position");
			if (position != this.CurrentPosition || !this.IsCurrentInSync) {
				object newItem = 0 > position || position >= this.InternalCount ? (object) null : this.InternalItemAt(position);
				if (newItem != CollectionViewOriginal.NewItemPlaceholder && this.OKToChangeCurrent()) {
					bool currentAfterLast = this.IsCurrentAfterLast;
					bool currentBeforeFirst = this.IsCurrentBeforeFirst;
					this.SetCurrent(newItem, position);
					this.OnCurrentChanged();
					if (this.IsCurrentAfterLast != currentAfterLast) this.OnPropertyChanged("IsCurrentAfterLast");
					if (this.IsCurrentBeforeFirst != currentBeforeFirst) this.OnPropertyChanged("IsCurrentBeforeFirst");
					this.OnPropertyChanged("CurrentPosition");
					this.OnPropertyChanged("CurrentItem");
				}
			}
			return this.IsCurrentInView;
		}

		public override bool PassesFilter(object item) {
			if (this.ActiveFilter != null) return this.ActiveFilter(item);
			else return true;
		}

		public override int IndexOf(object item) {
			this.VerifyRefreshNotDeferred();
			return this.InternalIndexOf(item);
		}

		public override object GetItemAt(int index) {
			this.VerifyRefreshNotDeferred();
			return this.InternalItemAt(index);
		}

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		int IComparer.Compare(object o1, object o2) { return this.Compare(o1, o2); }

		protected virtual int Compare(object o1, object o2) {
			if (this.IsGrouping) return this.InternalIndexOf(o1) - this.InternalIndexOf(o2);
			if (this.ActiveComparer != null) return this.ActiveComparer.Compare(o1, o2);
			else return this.InternalList.IndexOf(o1) - this.InternalList.IndexOf(o2);
		}

		protected override IEnumerator GetEnumerator() {
			this.VerifyRefreshNotDeferred();
			return this.InternalGetEnumerator();
		}

		public object AddNew() {
			this.VerifyRefreshNotDeferred();
			if (this.IsEditingItem) this.CommitEdit();
			this.CommitNew();
			if (this.CanAddNew) return this.AddNewCommon(this._itemConstructor.Invoke((object[]) null));
			throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedForView", new object[1] {
				(object) "AddNew"
			}));
		}

		public object AddNewItem(object newItem) {
			this.VerifyRefreshNotDeferred();
			if (this.IsEditingItem) this.CommitEdit();
			this.CommitNew();
			if (this.CanAddNewItem) return this.AddNewCommon(newItem);
			throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedForView", new object[1] {
				(object) "AddNewItem"
			}));
		}

		public void CommitNew() {
			if (this.IsEditingItem) { throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringTransaction", (object) "CommitNew", (object) "EditItem")); } else {
				this.VerifyRefreshNotDeferred();
				if (this._newItem == CollectionViewOriginal.NoNewItem) return;
				if (this.IsGrouping) { this.CommitNewForGrouping(); } else {
					int num1 = 0;
					switch (this.NewItemPlaceholderPosition) {
						case NewItemPlaceholderPosition.None:
							num1 = this.UsesLocalArray ? this.InternalCount - 1 : this._newItemIndex;
							break;
						case NewItemPlaceholderPosition.AtBeginning:
							num1 = 1;
							break;
						case NewItemPlaceholderPosition.AtEnd:
							num1 = this.InternalCount - 2;
							break;
					}
					object changedItem = this.EndAddNew(false);
					int num2 = this.AdjustBefore(NotifyCollectionChangedAction.Add, changedItem, this._newItemIndex);
					if (num2 < 0) this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItem, num1), num1, -1);
					else if (num1 == num2) {
						if (!this.UsesLocalArray) return;
						if (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) --num2;
						this.InternalList.Insert(num2, changedItem);
					} else this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, changedItem, num2, num1), num1, num2);
				}
			}
		}

		public void CancelNew() {
			if (this.IsEditingItem) { throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringTransaction", (object) "CancelNew", (object) "EditItem")); } else {
				this.VerifyRefreshNotDeferred();
				if (this._newItem == CollectionViewOriginal.NoNewItem) return;
				BindingOperations.AccessCollection((IEnumerable) this.SourceList, (Action) (() => {
					this.ProcessPendingChanges();
					this.SourceList.RemoveAt(this._newItemIndex);
					if (this._newItem == CollectionViewOriginal.NoNewItem) return;
					int local_0 = this.AdjustBefore(NotifyCollectionChangedAction.Remove, this._newItem, this._newItemIndex);
					this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this.EndAddNew(true), local_0), local_0, -1);
				}), true);
			}
		}

		public void RemoveAt(int index) {
			if (this.IsEditingItem || this.IsAddingNew) {
				throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
					(object) "RemoveAt"
				}));
			} else {
				this.VerifyRefreshNotDeferred();
				this.RemoveImpl(this.GetItemAt(index), index);
			}
		}

		public void Remove(object item) {
			if (this.IsEditingItem || this.IsAddingNew) {
				throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
					(object) "Remove"
				}));
			} else {
				this.VerifyRefreshNotDeferred();
				int index = this.InternalIndexOf(item);
				if (index < 0) return;
				this.RemoveImpl(item, index);
			}
		}

		public void EditItem(object item) {
			this.VerifyRefreshNotDeferred();
			if (item == CollectionViewOriginal.NewItemPlaceholder) throw new ArgumentException(System_Windows_SR_Get("CannotEditPlaceholder"), "item");
			if (this.IsAddingNew) {
				if (object.Equals(item, this._newItem)) return;
				this.CommitNew();
			}
			this.CommitEdit();
			this.SetEditItem(item);
			IEditableObject editableObject = item as IEditableObject;
			if (editableObject == null) return;
			editableObject.BeginEdit();
		}

		public void CommitEdit() {
			if (this.IsAddingNew) { throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringTransaction", (object) "CommitEdit", (object) "AddNew")); } else {
				this.VerifyRefreshNotDeferred();
				if (this._editItem == null) return;
				object obj = this._editItem;
				IEditableObject editableObject = this._editItem as IEditableObject;
				this.SetEditItem((object) null);
				if (editableObject != null) editableObject.EndEdit();
				int num1 = this.InternalIndexOf(obj);
				bool flag1 = num1 >= 0;
				bool flag2 = flag1 ? this.PassesFilter(obj) : this.SourceList.Contains(obj) && this.PassesFilter(obj);
				if (this.IsGrouping) {
					if (flag1) this.RemoveItemFromGroups(obj);
					if (!flag2) return;
					LiveShapingList liveShapingList = this.InternalList as LiveShapingList;
					LiveShapingItem lsi = liveShapingList == null ? (LiveShapingItem) null : liveShapingList.ItemAt(liveShapingList.IndexOf(obj));
					this.AddItemToGroups(obj, lsi);
				} else {
					if (!this.UsesLocalArray) return;
					IList internalList = this.InternalList;
					int num2 = this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning ? 1 : 0;
					int num3 = -1;
					if (flag1) {
						if (!flag2) { this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, num1), num1, -1); } else {
							if (this.ActiveComparer == null) return;
							int count = num1 - num2;
							if (count > 0 && this.ActiveComparer.Compare(internalList[count - 1], obj) > 0) {
								num3 = DataExtensionMethods.Search(internalList, 0, count, obj, this.ActiveComparer);
								if (num3 < 0) num3 = ~num3;
							} else if (count < internalList.Count - 1 && this.ActiveComparer.Compare(obj, internalList[count + 1]) > 0) {
								int num4 = DataExtensionMethods.Search(internalList, count + 1, internalList.Count - count - 1, obj, this.ActiveComparer);
								if (num4 < 0) num4 = ~num4;
								num3 = num4 - 1;
							}
							if (num3 < 0) return;
							this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, obj, num3 + num2, num1), num1, num3 + num2);
						}
					} else {
						if (!flag2) return;
						int num4 = this.AdjustBefore(NotifyCollectionChangedAction.Add, obj, this.SourceList.IndexOf(obj));
						this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, num4 + num2), -1, num4 + num2);
					}
				}
			}
		}

		public void CancelEdit() {
			if (this.IsAddingNew) { throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringTransaction", (object) "CancelEdit", (object) "AddNew")); } else {
				this.VerifyRefreshNotDeferred();
				if (this._editItem == null) return;
				IEditableObject editableObject = this._editItem as IEditableObject;
				this.SetEditItem((object) null);
				if (editableObject == null) throw new InvalidOperationException(System_Windows_SR_Get("CancelEditNotSupported"));
				editableObject.CancelEdit();
			}
		}

		protected override void OnAllowsCrossThreadChangesChanged() {
			if (this.AllowsCrossThreadChanges) {
				BindingOperations.AccessCollection(this.SourceCollection, (Action) (() => {
					lock (this.SyncRoot) {
						this.ClearPendingChanges();
						this.ShadowCollection = new ArrayList((ICollection) this.SourceCollection);
						if (this.UsesLocalArray) return;
						this._internalList = (IList) this.ShadowCollection;
					}
				}), false);
			} else {
				this.ShadowCollection = (ArrayList) null;
				if (this.UsesLocalArray) return;
				this._internalList = this.SourceList;
			}
		}

		[Obsolete("Replaced by OnAllowsCrossThreadChangesChanged")]
		protected override void OnBeginChangeLogging(NotifyCollectionChangedEventArgs args) { }

		protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args) {
			if (args == null) throw new ArgumentNullException("args");
			this.ValidateCollectionChangedEventArgs(args);
			if (!this._isItemConstructorValid) {
				switch (args.Action) {
					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Replace:
					case NotifyCollectionChangedAction.Reset:
						this.OnPropertyChanged("CanAddNew");
						break;
				}
			}
			int adjustedOldIndex = -1;
			int adjustedNewIndex = -1;
			if (this.AllowsCrossThreadChanges && args.Action != NotifyCollectionChangedAction.Reset) {
				if (args.Action != NotifyCollectionChangedAction.Remove && args.NewStartingIndex < 0 || args.Action != NotifyCollectionChangedAction.Add && args.OldStartingIndex < 0) return;
				this.AdjustShadowCopy(args);
			}
			if (args.Action == NotifyCollectionChangedAction.Reset) {
				if (this.IsEditingItem) this.ImplicitlyCancelEdit();
				if (this.IsAddingNew) {
					this._newItemIndex = this.SourceList.IndexOf(this._newItem);
					if (this._newItemIndex < 0) this.EndAddNew(true);
				}
				this.RefreshOrDefer();
			} else if (args.Action == NotifyCollectionChangedAction.Add && this._newItemIndex == -2) { this.BeginAddNew(args.NewItems[0], args.NewStartingIndex); } else {
				if (args.Action != NotifyCollectionChangedAction.Remove) adjustedNewIndex = this.AdjustBefore(NotifyCollectionChangedAction.Add, args.NewItems[0], args.NewStartingIndex);
				if (args.Action != NotifyCollectionChangedAction.Add) {
					adjustedOldIndex = this.AdjustBefore(NotifyCollectionChangedAction.Remove, args.OldItems[0], args.OldStartingIndex);
					if (this.UsesLocalArray && adjustedOldIndex >= 0 && adjustedOldIndex < adjustedNewIndex) --adjustedNewIndex;
				}
				switch (args.Action) {
					case NotifyCollectionChangedAction.Add:
						if (this.IsAddingNew && args.NewStartingIndex <= this._newItemIndex) {
							++this._newItemIndex;
							break;
						} else break;
					case NotifyCollectionChangedAction.Remove:
						if (this.IsAddingNew && args.OldStartingIndex < this._newItemIndex) --this._newItemIndex;
						object obj = args.OldItems[0];
						if (obj == this.CurrentEditItem) {
							this.ImplicitlyCancelEdit();
							break;
						} else if (obj == this.CurrentAddItem) {
							this.EndAddNew(true);
							break;
						} else break;
					case NotifyCollectionChangedAction.Move:
						if (this.IsAddingNew) {
							if (args.OldStartingIndex == this._newItemIndex) this._newItemIndex = args.NewStartingIndex;
							else if (args.OldStartingIndex < this._newItemIndex && this._newItemIndex <= args.NewStartingIndex) --this._newItemIndex;
							else if (args.NewStartingIndex <= this._newItemIndex && this._newItemIndex < args.OldStartingIndex) ++this._newItemIndex;
						}
						if (this.ActiveComparer != null && adjustedOldIndex == adjustedNewIndex) return;
						else break;
				}
				this.ProcessCollectionChangedWithAdjustedIndex(args, adjustedOldIndex, adjustedNewIndex);
			}
		}

		protected int InternalIndexOf(object item) {
			if (this.IsGrouping) return this._group.LeafIndexOf(item);
			if (item == CollectionViewOriginal.NewItemPlaceholder) {
				switch (this.NewItemPlaceholderPosition) {
					case NewItemPlaceholderPosition.None:
						return -1;
					case NewItemPlaceholderPosition.AtBeginning:
						return 0;
					case NewItemPlaceholderPosition.AtEnd:
						return this.InternalCount - 1;
				}
			} else if (this.IsAddingNew && object.Equals(item, this._newItem)) {
				switch (this.NewItemPlaceholderPosition) {
					case NewItemPlaceholderPosition.None:
						if (this.UsesLocalArray) return this.InternalCount - 1;
						else break;
					case NewItemPlaceholderPosition.AtBeginning:
						return 1;
					case NewItemPlaceholderPosition.AtEnd:
						return this.InternalCount - 2;
				}
			}
			int num = this.InternalList.IndexOf(item);
			if (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && num >= 0) num += this.IsAddingNew ? 2 : 1;
			return num;
		}

		protected object InternalItemAt(int index) {
			if (this.IsGrouping) return this._group.LeafAt(index);
			switch (this.NewItemPlaceholderPosition) {
				case NewItemPlaceholderPosition.None:
					if (this.IsAddingNew && this.UsesLocalArray && index == this.InternalCount - 1) return this._newItem;
					else break;
				case NewItemPlaceholderPosition.AtBeginning:
					if (index == 0) return CollectionViewOriginal.NewItemPlaceholder;
					--index;
					if (this.IsAddingNew) {
						if (index == 0) return this._newItem;
						if (this.UsesLocalArray || index <= this._newItemIndex) {
							--index;
							break;
						} else break;
					} else break;
				case NewItemPlaceholderPosition.AtEnd:
					if (index == this.InternalCount - 1) return CollectionViewOriginal.NewItemPlaceholder;
					if (this.IsAddingNew) {
						if (index == this.InternalCount - 2) return this._newItem;
						if (!this.UsesLocalArray && index >= this._newItemIndex) {
							++index;
							break;
						} else break;
					} else break;
			}
			return this.InternalList[index];
		}

		protected bool InternalContains(object item) {
			if (item == CollectionViewOriginal.NewItemPlaceholder) return this.NewItemPlaceholderPosition != NewItemPlaceholderPosition.None;
			if (this.IsGrouping) return this._group.LeafIndexOf(item) >= 0;
			else return this.InternalList.Contains(item);
		}

		protected IEnumerator InternalGetEnumerator() {
			if (!this.IsGrouping) return (IEnumerator) new CollectionViewOriginal.PlaceholderAwareEnumerator((KsWare.Presentation.DataVirtualization.CollectionView) this, this.InternalList.GetEnumerator(), this.NewItemPlaceholderPosition, this._newItem);
			else return this._group.GetLeafEnumerator();
		}

		internal void AdjustShadowCopy(NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					if (e.NewStartingIndex > -1) {
						this.ShadowCollection.Insert(e.NewStartingIndex, e.NewItems[0]);
						break;
					} else {
						this.ShadowCollection.Add(e.NewItems[0]);
						break;
					}
				case NotifyCollectionChangedAction.Remove:
					if (e.OldStartingIndex > -1) {
						this.ShadowCollection.RemoveAt(e.OldStartingIndex);
						break;
					} else {
						this.ShadowCollection.Remove(e.OldItems[0]);
						break;
					}
				case NotifyCollectionChangedAction.Replace:
					if (e.OldStartingIndex > -1) {
						this.ShadowCollection[e.OldStartingIndex] = e.NewItems[0];
						break;
					} else {
						this.ShadowCollection[this.ShadowCollection.IndexOf(e.OldItems[0])] = e.NewItems[0];
						break;
					}
				case NotifyCollectionChangedAction.Move:
					int index = e.OldStartingIndex;
					if (index < 0) index = this.ShadowCollection.IndexOf(e.NewItems[0]);
					this.ShadowCollection.RemoveAt(index);
					this.ShadowCollection.Insert(e.NewStartingIndex, e.NewItems[0]);
					break;
				default:
					throw new NotSupportedException(System_Windows_SR_Get("UnexpectedCollectionChangeAction", new object[1] {
						(object) e.Action
					}));
			}
		}

		internal void RestoreLiveShaping() {
			LiveShapingList liveShapingList = this.InternalList as LiveShapingList;
			if (liveShapingList == null) return;
			if (this.ActiveComparer != null) {
				if ((double) liveShapingList.SortDirtyItems.Count/(double) (liveShapingList.Count + 1) < 0.8) {
					foreach (LiveShapingItem lsi in liveShapingList.SortDirtyItems) {
						if (lsi.IsSortDirty && !lsi.IsDeleted && lsi.ForwardChanges) {
							lsi.IsSortDirty = false;
							lsi.IsSortPendingClean = false;
							int oldIndex;
							int newIndex;
							liveShapingList.FindPosition(lsi, out oldIndex, out newIndex);
							if (oldIndex != newIndex) {
								if (oldIndex < newIndex) --newIndex;
								this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, lsi.Item, oldIndex, newIndex), oldIndex, newIndex);
							}
						}
					}
				} else liveShapingList.RestoreLiveSortingByInsertionSort(new Action<NotifyCollectionChangedEventArgs, int, int>(this.ProcessCollectionChangedWithAdjustedIndex));
			}
			liveShapingList.SortDirtyItems.Clear();
			if (this.ActiveFilter != null) {
				foreach (LiveShapingItem lsi in liveShapingList.FilterDirtyItems) {
					if (lsi.IsFilterDirty && lsi.ForwardChanges) {
						object obj = lsi.Item;
						bool failsFilter = lsi.FailsFilter;
						bool flag = !this.PassesFilter(obj);
						if (failsFilter != flag) {
							if (flag) {
								int num = liveShapingList.IndexOf(lsi);
								this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, num), num, -1);
								liveShapingList.AddFilteredItem(lsi);
							} else {
								liveShapingList.RemoveFilteredItem(lsi);
								int num;
								if (this.ActiveComparer != null) {
									num = liveShapingList.Search(0, liveShapingList.Count, obj);
									if (num < 0) num = ~num;
								} else {
									IList ilFull = (this.AllowsCrossThreadChanges ? (IEnumerable) this.ShadowCollection : this.SourceCollection) as IList;
									int clearStartingIndex = lsi.GetAndClearStartingIndex();
									while (clearStartingIndex < ilFull.Count && !object.Equals(obj, ilFull[clearStartingIndex])) ++clearStartingIndex;
									liveShapingList.SetStartingIndexForFilteredItem(obj, clearStartingIndex + 1);
									num = this.MatchingSearch(obj, clearStartingIndex, ilFull, (IList) liveShapingList);
								}
								this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, num), -1, num);
							}
						}
						lsi.IsFilterDirty = false;
					}
				}
			}
			liveShapingList.FilterDirtyItems.Clear();
			if (this.IsGrouping) {
				List<AbandonedGroupItem> deleteList = new List<AbandonedGroupItem>();
				foreach (LiveShapingItem lsi in liveShapingList.GroupDirtyItems) {
					if (lsi.IsGroupDirty && !lsi.IsDeleted && lsi.ForwardChanges) {
						this._group.RestoreGrouping(lsi, deleteList);
						lsi.IsGroupDirty = false;
					}
				}
				this._group.DeleteAbandonedGroupItems(deleteList);
			}
			liveShapingList.GroupDirtyItems.Clear();
			this.IsLiveShapingDirty = false;
		}

		private void EnsureItemConstructor() {
			if (this._isItemConstructorValid) return;
			Type itemType = this.GetItemType(true);
			if (!(itemType != (Type) null)) return;
			this._itemConstructor = itemType.GetConstructor(Type.EmptyTypes);
			this._isItemConstructorValid = true;
		}

		private object AddNewCommon(object newItem) {
			BindingOperations.AccessCollection((IEnumerable) this.SourceList, (Action) (() => {
				this.ProcessPendingChanges();
				this._newItemIndex = -2;
				int local_0 = this.SourceList.Add(newItem);
				if (this.SourceList is INotifyCollectionChanged) return;
				if (!object.Equals(newItem, this.SourceList[local_0])) local_0 = this.SourceList.IndexOf(newItem);
				this.BeginAddNew(newItem, local_0);
			}), true);
			this.MoveCurrentTo(newItem);
			ISupportInitialize supportInitialize = newItem as ISupportInitialize;
			if (supportInitialize != null) supportInitialize.BeginInit();
			IEditableObject editableObject = newItem as IEditableObject;
			if (editableObject != null) editableObject.BeginEdit();
			return newItem;
		}

		private void BeginAddNew(object newItem, int index) {
			this.SetNewItem(newItem);
			this._newItemIndex = index;
			int num = -1;
			switch (this.NewItemPlaceholderPosition) {
				case NewItemPlaceholderPosition.None:
					num = this.UsesLocalArray ? this.InternalCount - 1 : this._newItemIndex;
					break;
				case NewItemPlaceholderPosition.AtBeginning:
					num = 1;
					break;
				case NewItemPlaceholderPosition.AtEnd:
					num = this.InternalCount - 2;
					break;
			}
			this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, num), -1, num);
		}

		private void CommitNewForGrouping() {
			int index1;
			switch (this.NewItemPlaceholderPosition) {
				case NewItemPlaceholderPosition.AtBeginning:
					index1 = 1;
					break;
				case NewItemPlaceholderPosition.AtEnd:
					index1 = this._group.Items.Count - 2;
					break;
				default:
					index1 = this._group.Items.Count - 1;
					break;
			}
			int index2 = this._newItemIndex;
			object changedItem = this.EndAddNew(false);
			this._group.RemoveSpecialItem(index1, changedItem, false);
			this.ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItem, index2));
		}

		private object EndAddNew(bool cancel) {
			object obj = this._newItem;
			this.SetNewItem(CollectionViewOriginal.NoNewItem);
			IEditableObject editableObject = obj as IEditableObject;
			if (editableObject != null) {
				if (cancel) editableObject.CancelEdit();
				else editableObject.EndEdit();
			}
			ISupportInitialize supportInitialize = obj as ISupportInitialize;
			if (supportInitialize != null) supportInitialize.EndInit();
			return obj;
		}

		private void SetNewItem(object item) {
			if (object.Equals(item, this._newItem)) return;
			this._newItem = item;
			this.OnPropertyChanged("CurrentAddItem");
			this.OnPropertyChanged("IsAddingNew");
			this.OnPropertyChanged("CanRemove");
		}

		private void RemoveImpl(object item, int index) {
			if (item == CollectionViewOriginal.NewItemPlaceholder) throw new InvalidOperationException(System_Windows_SR_Get("RemovingPlaceholder"));
			BindingOperations.AccessCollection((IEnumerable) this.SourceList, (Action) (() => {
				this.ProcessPendingChanges();
				if (index >= this.InternalList.Count || !object.Equals(item, this.GetItemAt(index))) {
					index = this.InternalList.IndexOf(item);
					if (index < 0) return;
				}
				int local_1 = index - (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning ? 1 : 0);
				bool local_2 = !(this.SourceList is INotifyCollectionChanged);
				try {
					this._isRemoving = true;
					if (this.UsesLocalArray || this.IsGrouping) {
						if (local_2) {
							local_1 = this.SourceList.IndexOf(item);
							this.SourceList.RemoveAt(local_1);
						} else this.SourceList.Remove(item);
					} else this.SourceList.RemoveAt(local_1);
					if (!local_2) return;
					this.ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, local_1));
				}
				finally {
					this._isRemoving = false;
					this.DoDeferredActions();
				}
			}), true);
		}

		private void ImplicitlyCancelEdit() {
			IEditableObject editableObject = this._editItem as IEditableObject;
			this.SetEditItem((object) null);
			if (editableObject == null) return;
			editableObject.CancelEdit();
		}

		private void SetEditItem(object item) {
			if (object.Equals(item, this._editItem)) return;
			this._editItem = item;
			this.OnPropertyChanged("CurrentEditItem");
			this.OnPropertyChanged("IsEditingItem");
			this.OnPropertyChanged("CanCancelEdit");
			this.OnPropertyChanged("CanAddNew");
			this.OnPropertyChanged("CanAddNewItem");
			this.OnPropertyChanged("CanRemove");
		}

		private void OnLivePropertyListChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (!this.IsLiveShaping) return;
			this.RebuildLocalArray();
		}

		private void ProcessCollectionChangedWithAdjustedIndex(NotifyCollectionChangedEventArgs args, int adjustedOldIndex, int adjustedNewIndex) {
			NotifyCollectionChangedAction action = args.Action;
			if (adjustedOldIndex == adjustedNewIndex && adjustedOldIndex >= 0) action = NotifyCollectionChangedAction.Replace;
			else if (adjustedOldIndex == -1) { if (adjustedNewIndex < 0 && args.Action != NotifyCollectionChangedAction.Add) action = NotifyCollectionChangedAction.Remove; } else if (adjustedOldIndex < -1) {
				if (adjustedNewIndex >= 0) action = NotifyCollectionChangedAction.Add;
				else if (action == NotifyCollectionChangedAction.Move) return;
			} else action = adjustedNewIndex >= 0 ? NotifyCollectionChangedAction.Move : NotifyCollectionChangedAction.Remove;
			int num = this.IsGrouping ? 0 : (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning ? (this.IsAddingNew ? 2 : 1) : 0);
			int currentPosition1 = this.CurrentPosition;
			int currentPosition2 = this.CurrentPosition;
			object currentItem1 = this.CurrentItem;
			bool currentAfterLast1 = this.IsCurrentAfterLast;
			bool currentBeforeFirst1 = this.IsCurrentBeforeFirst;
			object obj1 = args.OldItems == null || args.OldItems.Count <= 0 ? (object) null : args.OldItems[0];
			object obj2 = args.NewItems == null || args.NewItems.Count <= 0 ? (object) null : args.NewItems[0];
			LiveShapingList liveShapingList = this.InternalList as LiveShapingList;
			NotifyCollectionChangedEventArgs args1 = (NotifyCollectionChangedEventArgs) null;
			switch (action) {
				case NotifyCollectionChangedAction.Add:
					if (adjustedNewIndex == -2) {
						if (liveShapingList == null) return;
						bool? isLiveFiltering = this.IsLiveFiltering;
						if ((!isLiveFiltering.GetValueOrDefault() ? 0 : (isLiveFiltering.HasValue ? 1 : 0)) == 0) return;
						liveShapingList.AddFilteredItem(obj2);
						return;
					} else {
						bool flag = obj2 == CollectionViewOriginal.NewItemPlaceholder || this.IsAddingNew && object.Equals(this._newItem, obj2);
						if (this.UsesLocalArray && !flag) this.InternalList.Insert(adjustedNewIndex - num, obj2);
						if (!this.IsGrouping) {
							this.AdjustCurrencyForAdd(adjustedNewIndex);
							args = new NotifyCollectionChangedEventArgs(action, obj2, adjustedNewIndex);
							break;
						} else {
							LiveShapingItem lsi = liveShapingList == null || flag ? (LiveShapingItem) null : liveShapingList.ItemAt(adjustedNewIndex - num);
							this.AddItemToGroups(obj2, lsi);
							break;
						}
					}
				case NotifyCollectionChangedAction.Remove:
					if (adjustedOldIndex == -2) {
						if (liveShapingList == null) return;
						bool? isLiveFiltering = this.IsLiveFiltering;
						if ((!isLiveFiltering.GetValueOrDefault() ? 0 : (isLiveFiltering.HasValue ? 1 : 0)) == 0) return;
						liveShapingList.RemoveFilteredItem(obj1);
						return;
					} else {
						if (this.UsesLocalArray) {
							int index = adjustedOldIndex - num;
							if (index < this.InternalList.Count && object.Equals(this.ItemFrom(this.InternalList[index]), obj1)) this.InternalList.RemoveAt(index);
						}
						if (!this.IsGrouping) {
							this.AdjustCurrencyForRemove(adjustedOldIndex);
							args = new NotifyCollectionChangedEventArgs(action, args.OldItems[0], adjustedOldIndex);
							break;
						} else {
							this.RemoveItemFromGroups(obj1);
							break;
						}
					}
				case NotifyCollectionChangedAction.Replace:
					if (adjustedOldIndex == -2) {
						if (liveShapingList == null) return;
						bool? isLiveFiltering = this.IsLiveFiltering;
						if ((!isLiveFiltering.GetValueOrDefault() ? 0 : (isLiveFiltering.HasValue ? 1 : 0)) == 0) return;
						liveShapingList.ReplaceFilteredItem(obj1, obj2);
						return;
					} else {
						if (this.UsesLocalArray) this.InternalList[adjustedOldIndex - num] = obj2;
						if (!this.IsGrouping) {
							this.AdjustCurrencyForReplace(adjustedOldIndex);
							args = new NotifyCollectionChangedEventArgs(action, args.NewItems[0], args.OldItems[0], adjustedOldIndex);
							break;
						} else {
							LiveShapingItem lsi = liveShapingList == null ? (LiveShapingItem) null : liveShapingList.ItemAt(adjustedNewIndex - num);
							this.RemoveItemFromGroups(obj1);
							this.AddItemToGroups(obj2, lsi);
							break;
						}
					}
				case NotifyCollectionChangedAction.Move:
					bool flag1 = object.Equals(obj1, obj2);
					if (this.UsesLocalArray && (liveShapingList == null || !liveShapingList.IsRestoringLiveSorting)) {
						int index1 = adjustedOldIndex - num;
						int index2 = adjustedNewIndex - num;
						if (index1 < this.InternalList.Count && object.Equals(this.InternalList[index1], obj1)) {
							if (CollectionViewOriginal.NewItemPlaceholder != obj2) {
								DataExtensionMethods.Move(this.InternalList, index1, index2);
								if (!flag1) this.InternalList[index2] = obj2;
							} else this.InternalList.RemoveAt(index1);
						} else if (CollectionViewOriginal.NewItemPlaceholder != obj2) this.InternalList.Insert(index2, obj2);
					}
					if (!this.IsGrouping) {
						this.AdjustCurrencyForMove(adjustedOldIndex, adjustedNewIndex);
						if (flag1) {
							args = new NotifyCollectionChangedEventArgs(action, args.OldItems[0], adjustedNewIndex, adjustedOldIndex);
							break;
						} else {
							args1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, args.NewItems, adjustedNewIndex);
							args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, args.OldItems, adjustedOldIndex);
							break;
						}
					} else {
						LiveShapingItem lsi = liveShapingList == null ? (LiveShapingItem) null : liveShapingList.ItemAt(adjustedNewIndex);
						if (flag1) {
							this.MoveItemWithinGroups(obj1, lsi, adjustedOldIndex, adjustedNewIndex);
							break;
						} else {
							this.RemoveItemFromGroups(obj1);
							this.AddItemToGroups(obj2, lsi);
							break;
						}
					}
				default:
					Invariant.Assert(0 != 0, System_Windows_SR_Get("UnexpectedCollectionChangeAction", new object[1] {
						(object) action
					}));
					break;
			}
			bool flag2 = this.IsCurrentAfterLast != currentAfterLast1;
			bool flag3 = this.IsCurrentBeforeFirst != currentBeforeFirst1;
			bool flag4 = this.CurrentPosition != currentPosition2;
			bool flag5 = this.CurrentItem != currentItem1;
			bool currentAfterLast2 = this.IsCurrentAfterLast;
			bool currentBeforeFirst2 = this.IsCurrentBeforeFirst;
			int currentPosition3 = this.CurrentPosition;
			object currentItem2 = this.CurrentItem;
			if (!this.IsGrouping) {
				this.OnCollectionChanged(args);
				if (args1 != null) this.OnCollectionChanged(args1);
				if (this.IsCurrentAfterLast != currentAfterLast2) {
					flag2 = false;
					currentAfterLast2 = this.IsCurrentAfterLast;
				}
				if (this.IsCurrentBeforeFirst != currentBeforeFirst2) {
					flag3 = false;
					currentBeforeFirst2 = this.IsCurrentBeforeFirst;
				}
				if (this.CurrentPosition != currentPosition3) {
					flag4 = false;
					currentPosition3 = this.CurrentPosition;
				}
				if (this.CurrentItem != currentItem2) {
					flag5 = false;
					currentItem2 = this.CurrentItem;
				}
			}
			if (this._currentElementWasRemoved) {
				this.MoveCurrencyOffDeletedElement(currentPosition1);
				flag2 = flag2 || this.IsCurrentAfterLast != currentAfterLast2;
				flag3 = flag3 || this.IsCurrentBeforeFirst != currentBeforeFirst2;
				flag4 = flag4 || this.CurrentPosition != currentPosition3;
				flag5 = flag5 || this.CurrentItem != currentItem2;
			}
			if (flag2) this.OnPropertyChanged("IsCurrentAfterLast");
			if (flag3) this.OnPropertyChanged("IsCurrentBeforeFirst");
			if (flag4) this.OnPropertyChanged("CurrentPosition");
			if (!flag5) return;
			this.OnPropertyChanged("CurrentItem");
		}

		private void ValidateCollectionChangedEventArgs(NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems.Count == 1) break;
					else throw new NotSupportedException(System_Windows_SR_Get("RangeActionsNotSupported"));
				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems.Count == 1) break;
					else throw new NotSupportedException(System_Windows_SR_Get("RangeActionsNotSupported"));
				case NotifyCollectionChangedAction.Replace:
					if (e.NewItems.Count == 1 && e.OldItems.Count == 1) break;
					else throw new NotSupportedException(System_Windows_SR_Get("RangeActionsNotSupported"));
				case NotifyCollectionChangedAction.Move:
					if (e.NewItems.Count != 1) throw new NotSupportedException(System_Windows_SR_Get("RangeActionsNotSupported"));
					if (e.NewStartingIndex >= 0) break;
					else throw new InvalidOperationException(System_Windows_SR_Get("CannotMoveToUnknownPosition"));
				case NotifyCollectionChangedAction.Reset:
					break;
				default:
					throw new NotSupportedException(System_Windows_SR_Get("UnexpectedCollectionChangeAction", new object[1] {
						(object) e.Action
					}));
			}
		}

		private void PrepareLocalArray() {
			this.PrepareShaping();
			LiveShapingList liveShapingList1 = this._internalList as LiveShapingList;
			if (liveShapingList1 != null) {
				liveShapingList1.LiveShapingDirty -= new EventHandler(this.OnLiveShapingDirty);
				liveShapingList1.Clear();
			}
			IList list1 = this.AllowsCrossThreadChanges ? (IList) this.ShadowCollection : this.SourceCollection as IList;
			if (!this.UsesLocalArray) { this._internalList = list1; } else {
				int count = list1.Count;
				IList list2 = this.IsLiveShaping ? (IList) new LiveShapingList((ICollectionViewLiveShaping) this, this.GetLiveShapingFlags(), this.ActiveComparer) : (IList) new ArrayList(count);
				LiveShapingList liveShapingList2 = list2 as LiveShapingList;
				for (int index = 0; index < count; ++index) {
					if (!this.IsAddingNew || index != this._newItemIndex) {
						object obj = list1[index];
						if (this.ActiveFilter == null || this.ActiveFilter(obj)) { list2.Add(obj); } else {
							bool? isLiveFiltering = this.IsLiveFiltering;
							if ((!isLiveFiltering.GetValueOrDefault() ? 0 : (isLiveFiltering.HasValue ? 1 : 0)) != 0) liveShapingList2.AddFilteredItem(obj);
						}
					}
				}
				if (this.ActiveComparer != null) DataExtensionMethods.Sort(list2, this.ActiveComparer);
				if (liveShapingList2 != null) liveShapingList2.LiveShapingDirty += new EventHandler(this.OnLiveShapingDirty);
				this._internalList = list2;
			}
			this.PrepareGroups();
		}

		private void OnLiveShapingDirty(object sender, EventArgs e) { this.IsLiveShapingDirty = true; }

		private void RebuildLocalArray() {
			if (this.IsRefreshDeferred) this.RefreshOrDefer();
			else this.PrepareLocalArray();
		}

		private void MoveCurrencyOffDeletedElement(int oldCurrentPosition) {
			int num1 = this.InternalCount - 1;
			int num2 = oldCurrentPosition < num1 ? oldCurrentPosition : num1;
			this._currentElementWasRemoved = false;
			this.OnCurrentChanging();
			if (num2 < 0) this.SetCurrent((object) null, num2);
			else this.SetCurrent(this.InternalItemAt(num2), num2);
			this.OnCurrentChanged();
		}

		private int AdjustBefore(NotifyCollectionChangedAction action, object item, int index) {
			if (action == NotifyCollectionChangedAction.Reset) return -1;
			if (item == CollectionViewOriginal.NewItemPlaceholder) {
				if (this.NewItemPlaceholderPosition != NewItemPlaceholderPosition.AtBeginning) return this.InternalCount - 1;
				else return 0;
			} else if (this.IsAddingNew && this.NewItemPlaceholderPosition != NewItemPlaceholderPosition.None && object.Equals(item, this._newItem)) {
				if (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) return 1;
				if (!this.UsesLocalArray) return index;
				else return this.InternalCount - 2;
			} else {
				int num = this.IsGrouping ? 0 : (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning ? (this.IsAddingNew ? 2 : 1) : 0);
				IList ilFull = (this.AllowsCrossThreadChanges ? (IEnumerable) this.ShadowCollection : this.SourceCollection) as IList;
				if (index < -1 || index > ilFull.Count) { throw new InvalidOperationException(System_Windows_SR_Get("CollectionChangeIndexOutOfRange", (object) index, (object) ilFull.Count)); } else {
					if (action == NotifyCollectionChangedAction.Add) {
						if (index >= 0) {
							if (!object.Equals(item, ilFull[index]))
								throw new InvalidOperationException(System_Windows_SR_Get("AddedItemNotAtIndex", new object[1] {
									(object) index
								}));
						} else {
							index = ilFull.IndexOf(item);
							if (index < 0) throw new InvalidOperationException(System_Windows_SR_Get("AddedItemNotInCollection"));
						}
					}
					if (!this.UsesLocalArray) {
						if (this.IsAddingNew && this.NewItemPlaceholderPosition != NewItemPlaceholderPosition.None && index > this._newItemIndex) --index;
						if (index >= 0) return index + num;
						else return index;
					} else {
						if (action == NotifyCollectionChangedAction.Add) {
							if (!this.PassesFilter(item)) return -2;
							if (!this.UsesLocalArray) index = -1;
							else if (this.ActiveComparer != null) {
								index = DataExtensionMethods.Search(this.InternalList, item, this.ActiveComparer);
								if (index < 0) index = ~index;
							} else index = this.MatchingSearch(item, index, ilFull, this.InternalList);
						} else if (action == NotifyCollectionChangedAction.Remove) {
							if (!this.IsAddingNew || item != this._newItem) {
								index = this.InternalList.IndexOf(item);
								if (index < 0) return -2;
							} else {
								switch (this.NewItemPlaceholderPosition) {
									case NewItemPlaceholderPosition.None:
										return this.InternalCount - 1;
									case NewItemPlaceholderPosition.AtBeginning:
										return 1;
									case NewItemPlaceholderPosition.AtEnd:
										return this.InternalCount - 2;
								}
							}
						} else index = -1;
						if (index >= 0) return index + num;
						else return index;
					}
				}
			}
		}

		private int MatchingSearch(object item, int index, IList ilFull, IList ilPartial) {
			int index1 = 0;
			int index2 = 0;
			while (index1 < index && index2 < this.InternalList.Count) {
				if (object.Equals(ilFull[index1], ilPartial[index2])) {
					++index1;
					++index2;
				} else if (object.Equals(item, ilPartial[index2])) ++index2;
				else ++index1;
			}
			return index2;
		}

		private void AdjustCurrencyForAdd(int index) {
			if (this.InternalCount == 1) { this.SetCurrent((object) null, -1); } else {
				if (index > this.CurrentPosition) return;
				int num = this.CurrentPosition + 1;
				if (num < this.InternalCount) this.SetCurrent(this.GetItemAt(num), num);
				else this.SetCurrent((object) null, this.InternalCount);
			}
		}

		private void AdjustCurrencyForRemove(int index) {
			if (index < this.CurrentPosition) { this.SetCurrent(this.CurrentItem, this.CurrentPosition - 1); } else {
				if (index != this.CurrentPosition) return;
				this._currentElementWasRemoved = true;
			}
		}

		private void AdjustCurrencyForMove(int oldIndex, int newIndex) {
			if (oldIndex == this.CurrentPosition) this.SetCurrent(this.GetItemAt(newIndex), newIndex);
			else if (oldIndex < this.CurrentPosition && this.CurrentPosition <= newIndex) { this.SetCurrent(this.CurrentItem, this.CurrentPosition - 1); } else {
				if (newIndex > this.CurrentPosition || this.CurrentPosition >= oldIndex) return;
				this.SetCurrent(this.CurrentItem, this.CurrentPosition + 1);
			}
		}

		private void AdjustCurrencyForReplace(int index) {
			if (index != this.CurrentPosition) return;
			this._currentElementWasRemoved = true;
		}

		private void PrepareShaping() {
			if (this._customSort != null) this.ActiveComparer = this._customSort;
			else if (this._sort != null && this._sort.Count > 0) {
				IComparer comparer = SystemXmlHelper.PrepareXmlComparer(this.SourceCollection, this._sort, this.Culture);
				this.ActiveComparer = comparer == null ? (IComparer) new SortFieldComparer(this._sort, this.Culture) : comparer;
			} else this.ActiveComparer = (IComparer) null;
			this.ActiveFilter = this.Filter;
			this._group.Clear();
			this._group.Initialize();
			this._isGrouping = this._group.GroupBy != null;
		}

		private void SetSortDescriptions(SortDescriptionCollection descriptions) {
			if (this._sort != null) this._sort.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.SortDescriptionsChanged);
			this._sort = descriptions;
			if (this._sort == null) return;
			Invariant.Assert(this._sort.Count == 0, "must be empty SortDescription collection");
			this._sort.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SortDescriptionsChanged);
		}

		private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (this.IsAddingNew || this.IsEditingItem) {
				throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
					(object) "Sorting"
				}));
			} else {
				if (this._sort.Count > 0) this._customSort = (IComparer) null;
				this.RefreshOrDefer();
			}
		}

		private void PrepareGroups() {
			if (!this._isGrouping) return;
			IComparer activeComparer = this.ActiveComparer;
			if (activeComparer != null) { this._group.ActiveComparer = activeComparer; } else {
				CollectionViewGroupInternal.IListComparer ilistComparer = this._group.ActiveComparer as CollectionViewGroupInternal.IListComparer;
				if (ilistComparer != null) ilistComparer.ResetList(this.InternalList);
				else this._group.ActiveComparer = (IComparer) new CollectionViewGroupInternal.IListComparer(this.InternalList);
			}
			if (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) {
				this._group.InsertSpecialItem(0, CollectionViewOriginal.NewItemPlaceholder, true);
				if (this.IsAddingNew) this._group.InsertSpecialItem(1, this._newItem, true);
			}
			bool? isLiveGrouping = this.IsLiveGrouping;
			if (isLiveGrouping.GetValueOrDefault()) { int num = isLiveGrouping.HasValue ? 1 : 0; }
			LiveShapingList liveShapingList = this.InternalList as LiveShapingList;
			int index = 0;
			for (int count = this.InternalList.Count; index < count; ++index) {
				object objB = this.InternalList[index];
				LiveShapingItem lsi = liveShapingList != null ? liveShapingList.ItemAt(index) : (LiveShapingItem) null;
				if (!this.IsAddingNew || !object.Equals(this._newItem, objB)) this._group.AddToSubgroups(objB, lsi, true);
			}
			if (this.IsAddingNew && this.NewItemPlaceholderPosition != NewItemPlaceholderPosition.AtBeginning) this._group.InsertSpecialItem(this._group.Items.Count, this._newItem, true);
			if (this.NewItemPlaceholderPosition != NewItemPlaceholderPosition.AtEnd) return;
			this._group.InsertSpecialItem(this._group.Items.Count, CollectionViewOriginal.NewItemPlaceholder, true);
		}

		private void OnGroupChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Add) this.AdjustCurrencyForAdd(e.NewStartingIndex);
			else if (e.Action == NotifyCollectionChangedAction.Remove) this.AdjustCurrencyForRemove(e.OldStartingIndex);
			this.OnCollectionChanged(e);
		}

		private void OnGroupByChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (this.IsAddingNew || this.IsEditingItem)
				throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
					(object) "Grouping"
				}));
			else this.RefreshOrDefer();
		}

		private void OnGroupDescriptionChanged(object sender, EventArgs e) {
			if (this.IsAddingNew || this.IsEditingItem)
				throw new InvalidOperationException(System_Windows_SR_Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
					(object) "Grouping"
				}));
			else this.RefreshOrDefer();
		}

		private void AddItemToGroups(object item, LiveShapingItem lsi) {
			if (this.IsAddingNew && item == this._newItem) {
				int index;
				switch (this.NewItemPlaceholderPosition) {
					case NewItemPlaceholderPosition.AtBeginning:
						index = 1;
						break;
					case NewItemPlaceholderPosition.AtEnd:
						index = this._group.Items.Count - 1;
						break;
					default:
						index = this._group.Items.Count;
						break;
				}
				this._group.InsertSpecialItem(index, item, false);
			} else this._group.AddToSubgroups(item, lsi, false);
		}

		private void RemoveItemFromGroups(object item) {
			if (!this.CanGroupNamesChange && !this._group.RemoveFromSubgroups(item)) return;
			this._group.RemoveItemFromSubgroupsByExhaustiveSearch(item);
		}

		private void MoveItemWithinGroups(object item, LiveShapingItem lsi, int oldIndex, int newIndex) { this._group.MoveWithinSubgroups(item, lsi, this.InternalList, oldIndex, newIndex); }

		private LiveShapingFlags GetLiveShapingFlags() {
			LiveShapingFlags liveShapingFlags = (LiveShapingFlags) 0;
			bool? isLiveSorting = this.IsLiveSorting;
			if ((!isLiveSorting.GetValueOrDefault() ? 0 : (isLiveSorting.HasValue ? 1 : 0)) != 0) liveShapingFlags |= LiveShapingFlags.Sorting;
			bool? isLiveFiltering = this.IsLiveFiltering;
			if ((!isLiveFiltering.GetValueOrDefault() ? 0 : (isLiveFiltering.HasValue ? 1 : 0)) != 0) liveShapingFlags |= LiveShapingFlags.Filtering;
			bool? isLiveGrouping = this.IsLiveGrouping;
			if ((!isLiveGrouping.GetValueOrDefault() ? 0 : (isLiveGrouping.HasValue ? 1 : 0)) != 0) liveShapingFlags |= LiveShapingFlags.Grouping;
			return liveShapingFlags;
		}

		private object ItemFrom(object o) {
			LiveShapingItem liveShapingItem = o as LiveShapingItem;
			if (liveShapingItem != null) return liveShapingItem.Item;
			else return o;
		}

		private void OnPropertyChanged(string propertyName) { base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName)); }

		private void DeferAction(Action action) {
			if (this._deferredActions == null) this._deferredActions = new List<Action>();
			this._deferredActions.Add(action);
		}

		private void DoDeferredActions() {
			if (this._deferredActions == null) return;
			List<Action> list = this._deferredActions;
			this._deferredActions = (List<Action>) null;
			foreach (Action action in list) action();
		}

		private string System_Windows_SR_Get(string s0) { throw new NotImplementedException(); }
		private string System_Windows_SR_Get(string s0, object[] o1) { throw new NotImplementedException(); }
		private string System_Windows_SR_Get(string s0, object o1, object o2) { throw new NotImplementedException(); }

	}
}