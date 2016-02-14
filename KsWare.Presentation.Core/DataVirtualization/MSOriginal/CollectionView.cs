// Type: System.Windows.Data.CollectionView
// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0F086D49-4CAD-43AD-A3E2-A5268BD16302
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationFramework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
using MS.Internal;
using MS.Internal.Data;
using MS.Internal.Hashing.PresentationFramework;

namespace KsWare.Presentation.DataVirtualization.MSOriginal {

	public class CollectionViewOriginal : DispatcherObject, ICollectionView, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged {

		private static object _newItemPlaceholder = (object) new NamedObject("NewItemPlaceholder");
		private static readonly ArrayList EmptyArrayList = new ArrayList();
		private static readonly string IEnumerableT = typeof (IEnumerable<>).Name;
		internal static readonly object NoNewItem = (object) new NamedObject("NoNewItem");
		private static readonly CurrentChangingEventArgs uncancelableCurrentChangingEventArgs = new CurrentChangingEventArgs(false);
		private ArrayList _changeLog = new ArrayList();
		private ArrayList _tempChangeLog = CollectionView.EmptyArrayList;
		private CollectionView.SimpleMonitor _currentChangedMonitor = new CollectionView.SimpleMonitor();
		private CollectionView.CollectionViewFlags _flags = CollectionView.CollectionViewFlags.ShouldProcessCollectionChanged | CollectionView.CollectionViewFlags.NeedsRefresh;
		private object _syncObject = new object();
		internal const string CountPropertyName = "Count";
		internal const string IsEmptyPropertyName = "IsEmpty";
		internal const string CulturePropertyName = "Culture";
		internal const string CurrentPositionPropertyName = "CurrentPosition";
		internal const string CurrentItemPropertyName = "CurrentItem";
		internal const string IsCurrentBeforeFirstPropertyName = "IsCurrentBeforeFirst";
		internal const string IsCurrentAfterLastPropertyName = "IsCurrentAfterLast";
		private DataBindOperation _databindOperation;
		private object _vmData;
		private IEnumerable _sourceCollection;
		private CultureInfo _culture;
		private int _deferLevel;
		private IndexedEnumerableOriginal _enumerableWrapper;
		private Predicate<object> _filter;
		private object _currentItem;
		private int _currentPosition;
		private bool _currentElementWasRemovedOrReplaced;
		private DataBindEngine _engine;
		private int _timestamp;

		[TypeConverter(typeof (CultureInfoIetfLanguageTagConverter))]
		public virtual CultureInfo Culture {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._culture; }
			set {
				if (value == null) throw new ArgumentNullException("value");
				if (this._culture == value) return;
				this._culture = value;
				this.OnPropertyChanged("Culture");
			}
		}

		public virtual IEnumerable SourceCollection {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._sourceCollection; }
		}

		public virtual Predicate<object> Filter {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._filter; }
			set {
				if (!this.CanFilter) throw new NotSupportedException();
				this._filter = value;
				this.RefreshOrDefer();
			}
		}

		public virtual bool CanFilter { get { return true; } }

		public virtual SortDescriptionCollection SortDescriptions {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return SortDescriptionCollection.Empty; }
		}

		public virtual bool CanSort { get { return false; } }

		public virtual bool CanGroup { get { return false; } }

		public virtual ObservableCollection<GroupDescription> GroupDescriptions { get { return (ObservableCollection<GroupDescription>) null; } }

		public virtual ReadOnlyObservableCollection<object> Groups { get { return (ReadOnlyObservableCollection<object>) null; } }

		public virtual object CurrentItem {
			get {
				this.VerifyRefreshNotDeferred();
				return this._currentItem;
			}
		}

		public virtual int CurrentPosition {
			get {
				this.VerifyRefreshNotDeferred();
				return this._currentPosition;
			}
		}

		public virtual bool IsCurrentAfterLast {
			get {
				this.VerifyRefreshNotDeferred();
				return this.CheckFlag(CollectionViewOriginal.CollectionViewFlags.IsCurrentAfterLast);
			}
		}

		public virtual bool IsCurrentBeforeFirst {
			get {
				this.VerifyRefreshNotDeferred();
				return this.CheckFlag(CollectionViewOriginal.CollectionViewFlags.IsCurrentBeforeFirst);
			}
		}

		public virtual int Count {
			get {
				this.VerifyRefreshNotDeferred();
				return this.EnumerableWrapper.Count;
			}
		}

		public virtual bool IsEmpty { get { return this.EnumerableWrapper.IsEmpty; } }

		public virtual IComparer Comparer { get { return this as IComparer; } }

		public virtual bool NeedsRefresh {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this.CheckFlag(CollectionView.CollectionViewFlags.NeedsRefresh); }
		}

		public virtual bool IsInUse {
			get {
				if (this.CollectionChanged == null && this.PropertyChanged == null && this.CurrentChanged == null) return this.CurrentChanging != null;
				else return true;
			}
		}

		public static object NewItemPlaceholder {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return CollectionView._newItemPlaceholder; }
		}

		protected bool IsDynamic {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this.CheckFlag(CollectionView.CollectionViewFlags.IsDynamic); }
		}

		protected bool AllowsCrossThreadChanges {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this.CheckFlag(CollectionView.CollectionViewFlags.AllowsCrossThreadChanges); }
		}

		protected bool UpdatedOutsideDispatcher {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this.AllowsCrossThreadChanges; }
		}

		protected bool IsRefreshDeferred { get { return this._deferLevel > 0; } }

		protected bool IsCurrentInSync {
			get {
				if (this.IsCurrentInView) return this.GetItemAt(this.CurrentPosition) == this.CurrentItem;
				else return this.CurrentItem == null;
			}
		}

		internal object SyncRoot {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._syncObject; }
		}

		internal int Timestamp {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._timestamp; }
		}

		private bool IsCurrentInView {
			get {
				this.VerifyRefreshNotDeferred();
				if (0 <= this.CurrentPosition) return this.CurrentPosition < this.Count;
				else return false;
			}
		}

		private IndexedEnumerableOriginal EnumerableWrapper {
			get {
				if (this._enumerableWrapper == null) Interlocked.CompareExchange<IndexedEnumerableOriginal>(ref this._enumerableWrapper, new IndexedEnumerableOriginal(this.SourceCollection, new Predicate<object>(this.PassesFilter)), (IndexedEnumerableOriginal) null);
				return this._enumerableWrapper;
			}
		}

		public virtual event CurrentChangingEventHandler CurrentChanging;

		public virtual event EventHandler CurrentChanged;

		protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] add { this.CollectionChanged += value; }
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] remove { this.CollectionChanged -= value; }
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] add { this.PropertyChanged += value; }
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] remove { this.PropertyChanged -= value; }
		}

		protected virtual event PropertyChangedEventHandler PropertyChanged;

		static CollectionViewOriginal() { }

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public CollectionViewOriginal(IEnumerable collection)
			: this(collection, 0) { }

		internal CollectionViewOriginal(IEnumerable collection, int moveToFirst) {
			if (collection == null) throw new ArgumentNullException("collection");
			if (this.GetType() == typeof (CollectionView) && TraceData.IsEnabled) TraceData.Trace(TraceEventType.Warning, TraceData.CollectionViewIsUnsupported);
			this._engine = DataBindEngine.CurrentDataBindEngine;
			if (!this._engine.IsShutDown) this.SetFlag(CollectionView.CollectionViewFlags.AllowsCrossThreadChanges, this._engine.ViewManager.GetSynchronizationInfo(collection).IsSynchronized);
			else moveToFirst = -1;
			this._sourceCollection = collection;
			INotifyCollectionChanged collectionChanged = collection as INotifyCollectionChanged;
			if (collectionChanged != null) {
				IBindingList bindingList;
				if (!(this is BindingListCollectionView) || (bindingList = collection as IBindingList) != null && !bindingList.SupportsChangeNotification) collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
				this.SetFlag(CollectionViewOriginal.CollectionViewFlags.IsDynamic, true);
			}
			object currentItem = (object) null;
			int currentPosition = -1;
			if (moveToFirst >= 0)
				BindingOperations.AccessCollection(collection, (Action) (() => {
					IEnumerator local_0 = collection.GetEnumerator();
					if (local_0.MoveNext()) {
						currentItem = local_0.Current;
						currentPosition = 0;
					}
					IDisposable local_1 = local_0 as IDisposable;
					if (local_1 == null) return;
					local_1.Dispose();
				}), false);
			this._currentItem = currentItem;
			this._currentPosition = currentPosition;
			this.SetFlag(CollectionViewOriginal.CollectionViewFlags.IsCurrentBeforeFirst, this._currentPosition < 0);
			this.SetFlag(CollectionViewOriginal.CollectionViewFlags.IsCurrentAfterLast, this._currentPosition < 0);
			this.SetFlag(CollectionViewOriginal.CollectionViewFlags.CachedIsEmpty, this._currentPosition < 0);
		}

		internal CollectionViewOriginal(IEnumerable collection, bool shouldProcessCollectionChanged)
			: this(collection) { this.SetFlag(CollectionViewOriginal.CollectionViewFlags.ShouldProcessCollectionChanged, shouldProcessCollectionChanged); }

		public virtual bool Contains(object item) {
			this.VerifyRefreshNotDeferred();
			return this.IndexOf(item) >= 0;
		}

		public virtual void Refresh() {
			IEditableCollectionView editableCollectionView = this as IEditableCollectionView;
			if (editableCollectionView != null && (editableCollectionView.IsAddingNew || editableCollectionView.IsEditingItem))
				throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
					(object) "Refresh"
				}));
			else this.RefreshInternal();
		}

		internal void RefreshInternal() {
			if (this.AllowsCrossThreadChanges) this.VerifyAccess();
			this.RefreshOverride();
			this.SetFlag(CollectionViewOriginal.CollectionViewFlags.NeedsRefresh, false);
		}

		public virtual IDisposable DeferRefresh() {
			if (this.AllowsCrossThreadChanges) this.VerifyAccess();
			IEditableCollectionView editableCollectionView = this as IEditableCollectionView;
			if (editableCollectionView != null && (editableCollectionView.IsAddingNew || editableCollectionView.IsEditingItem)) {
				throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedDuringAddOrEdit", new object[1] {
					(object) "DeferRefresh"
				}));
			} else {
				++this._deferLevel;
				return (IDisposable) new CollectionViewOriginal.DeferHelper(this);
			}
		}

		public virtual bool MoveCurrentToFirst() {
			this.VerifyRefreshNotDeferred();
			int position = 0;
			IEditableCollectionView editableCollectionView = this as IEditableCollectionView;
			if (editableCollectionView != null && editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) position = 1;
			return this.MoveCurrentToPosition(position);
		}

		public virtual bool MoveCurrentToLast() {
			this.VerifyRefreshNotDeferred();
			int position = this.Count - 1;
			IEditableCollectionView editableCollectionView = this as IEditableCollectionView;
			if (editableCollectionView != null && editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd) --position;
			return this.MoveCurrentToPosition(position);
		}

		public virtual bool MoveCurrentToNext() {
			this.VerifyRefreshNotDeferred();
			int position = this.CurrentPosition + 1;
			int count = this.Count;
			IEditableCollectionView editableCollectionView = this as IEditableCollectionView;
			if (editableCollectionView != null && position == 0 && editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) position = 1;
			if (editableCollectionView != null && position == count - 1 && editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd) position = count;
			if (position <= count) return this.MoveCurrentToPosition(position);
			else return false;
		}

		public virtual bool MoveCurrentToPrevious() {
			this.VerifyRefreshNotDeferred();
			int position = this.CurrentPosition - 1;
			int count = this.Count;
			IEditableCollectionView editableCollectionView = this as IEditableCollectionView;
			if (editableCollectionView != null && position == count - 1 && editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd) position = count - 2;
			if (editableCollectionView != null && position == 0 && editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) position = -1;
			if (position >= -1) return this.MoveCurrentToPosition(position);
			else return false;
		}

		public virtual bool MoveCurrentTo(object item) {
			this.VerifyRefreshNotDeferred();
			if ((object.Equals(this.CurrentItem, item) || object.Equals(CollectionView.NewItemPlaceholder, item)) && (item != null || this.IsCurrentInView)) return this.IsCurrentInView;
			int position = -1;
			IEditableCollectionView editableCollectionView = this as IEditableCollectionView;
			if (editableCollectionView != null && editableCollectionView.IsAddingNew && object.Equals(item, editableCollectionView.CurrentAddItem) || this.PassesFilter(item)) position = this.IndexOf(item);
			return this.MoveCurrentToPosition(position);
		}

		public virtual bool MoveCurrentToPosition(int position) {
			this.VerifyRefreshNotDeferred();
			if (position < -1 || position > this.Count) throw new ArgumentOutOfRangeException("position");
			IEditableCollectionView editableCollectionView = this as IEditableCollectionView;
			if (editableCollectionView != null && (position == 0 && editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning || position == this.Count - 1 && editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd) || (position == this.CurrentPosition && this.IsCurrentInSync || !this.OKToChangeCurrent())) return this.IsCurrentInView;
			bool currentAfterLast = this.IsCurrentAfterLast;
			bool currentBeforeFirst = this.IsCurrentBeforeFirst;
			this._MoveCurrentToPosition(position);
			this.OnCurrentChanged();
			if (this.IsCurrentAfterLast != currentAfterLast) this.OnPropertyChanged("IsCurrentAfterLast");
			if (this.IsCurrentBeforeFirst != currentBeforeFirst) this.OnPropertyChanged("IsCurrentBeforeFirst");
			this.OnPropertyChanged("CurrentPosition");
			this.OnPropertyChanged("CurrentItem");
			return this.IsCurrentInView;
		}

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

		public virtual bool PassesFilter(object item) {
			if (this.CanFilter && this.Filter != null) return this.Filter(item);
			else return true;
		}

		public virtual int IndexOf(object item) {
			this.VerifyRefreshNotDeferred();
			return this.EnumerableWrapper.IndexOf(item);
		}

		public virtual object GetItemAt(int index) {
			if (index < 0) throw new ArgumentOutOfRangeException("index");
			else return this.EnumerableWrapper[index];
		}

		public virtual void DetachFromSourceCollection() {
			INotifyCollectionChanged collectionChanged = this._sourceCollection as INotifyCollectionChanged;
			IBindingList bindingList;
			if (collectionChanged != null && (!(this is BindingListCollectionView) || (bindingList = this._sourceCollection as IBindingList) != null && !bindingList.SupportsChangeNotification)) collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
			this._sourceCollection = (IEnumerable) null;
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
			if (this.PropertyChanged == null) return;
			this.PropertyChanged((object) this, e);
		}

		protected virtual void RefreshOverride() {
			if (this.SortDescriptions.Count > 0) {
				throw new InvalidOperationException(System.Windows.SR.Get("ImplementOtherMembersWithSort", new object[1] {
					(object) "Refresh()"
				}));
			} else {
				object obj = this._currentItem;
				bool flag1 = this.CheckFlag(CollectionViewOriginal.CollectionViewFlags.IsCurrentAfterLast);
				bool flag2 = this.CheckFlag(CollectionViewOriginal.CollectionViewFlags.IsCurrentBeforeFirst);
				int num = this._currentPosition;
				this.OnCurrentChanging();
				this.InvalidateEnumerableWrapper();
				if (this.IsEmpty || flag2) this._MoveCurrentToPosition(-1);
				else if (flag1) this._MoveCurrentToPosition(this.Count);
				else if (obj != null) {
					int position = this.EnumerableWrapper.IndexOf(obj);
					if (position < 0) position = 0;
					this._MoveCurrentToPosition(position);
				}
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				this.OnCurrentChanged();
				if (this.IsCurrentAfterLast != flag1) this.OnPropertyChanged("IsCurrentAfterLast");
				if (this.IsCurrentBeforeFirst != flag2) this.OnPropertyChanged("IsCurrentBeforeFirst");
				if (num != this.CurrentPosition) this.OnPropertyChanged("CurrentPosition");
				if (obj == this.CurrentItem) return;
				this.OnPropertyChanged("CurrentItem");
			}
		}

		protected virtual IEnumerator GetEnumerator() {
			this.VerifyRefreshNotDeferred();
			if (this.SortDescriptions.Count <= 0) return this.EnumerableWrapper.GetEnumerator();
			throw new InvalidOperationException(System.Windows.SR.Get("ImplementOtherMembersWithSort", new object[1] {
				(object) "GetEnumerator()"
			}));
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
			if (args == null) throw new ArgumentNullException("args");
			++this._timestamp;
			if (this.CollectionChanged != null) this.CollectionChanged((object) this, args);
			if (args.Action != NotifyCollectionChangedAction.Replace && args.Action != NotifyCollectionChangedAction.Move) this.OnPropertyChanged("Count");
			bool isEmpty = this.IsEmpty;
			if (isEmpty == this.CheckFlag(CollectionView.CollectionViewFlags.CachedIsEmpty)) return;
			this.SetFlag(CollectionView.CollectionViewOriginal.CachedIsEmpty, isEmpty);
			this.OnPropertyChanged("IsEmpty");
		}

		protected void SetCurrent(object newItem, int newPosition) {
			int count = newItem != null ? 0 : (this.IsEmpty ? 0 : this.Count);
			this.SetCurrent(newItem, newPosition, count);
		}

		protected void SetCurrent(object newItem, int newPosition, int count) {
			if (newItem != null) {
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst, false);
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast, false);
			} else if (count == 0) {
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst, true);
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast, true);
				newPosition = -1;
			} else {
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst, newPosition < 0);
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast, newPosition >= count);
			}
			this._currentItem = newItem;
			this._currentPosition = newPosition;
		}

		protected bool OKToChangeCurrent() {
			CurrentChangingEventArgs args = new CurrentChangingEventArgs();
			this.OnCurrentChanging(args);
			return !args.Cancel;
		}

		protected void OnCurrentChanging() {
			this._currentPosition = -1;
			this.OnCurrentChanging(CollectionViewOriginal.uncancelableCurrentChangingEventArgs);
		}

		protected virtual void OnCurrentChanging(CurrentChangingEventArgs args) {
			if (args == null) throw new ArgumentNullException("args");
			if (this._currentChangedMonitor.Busy) {
				if (!args.IsCancelable) return;
				args.Cancel = true;
			} else {
				if (this.CurrentChanging == null) return;
				this.CurrentChanging((object) this, args);
			}
		}

		protected virtual void OnCurrentChanged() {
			if (this.CurrentChanged == null || !this._currentChangedMonitor.Enter()) return;
			using (this._currentChangedMonitor) this.CurrentChanged((object) this, EventArgs.Empty);
		}

		protected virtual void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args) {
			this.ValidateCollectionChangedEventArgs(args);
			object obj = this._currentItem;
			bool flag1 = this.CheckFlag(CollectionViewOriginal.CollectionViewFlags.IsCurrentAfterLast);
			bool flag2 = this.CheckFlag(CollectionViewOriginal.CollectionViewFlags.IsCurrentBeforeFirst);
			int num = this._currentPosition;
			bool flag3 = false;
			switch (args.Action) {
				case NotifyCollectionChangedAction.Add:
					if (this.PassesFilter(args.NewItems[0])) {
						flag3 = true;
						this.AdjustCurrencyForAdd(args.NewStartingIndex);
						break;
					} else break;
				case NotifyCollectionChangedAction.Remove:
					if (this.PassesFilter(args.OldItems[0])) {
						flag3 = true;
						this.AdjustCurrencyForRemove(args.OldStartingIndex);
						break;
					} else break;
				case NotifyCollectionChangedAction.Replace:
					if (this.PassesFilter(args.OldItems[0]) || this.PassesFilter(args.NewItems[0])) {
						flag3 = true;
						this.AdjustCurrencyForReplace(args.OldStartingIndex);
						break;
					} else break;
				case NotifyCollectionChangedAction.Move:
					if (this.PassesFilter(args.NewItems[0])) {
						flag3 = true;
						this.AdjustCurrencyForMove(args.OldStartingIndex, args.NewStartingIndex);
						break;
					} else break;
				case NotifyCollectionChangedAction.Reset:
					this.RefreshOrDefer();
					return;
			}
			if (flag3) this.OnCollectionChanged(args);
			if (this._currentElementWasRemovedOrReplaced) {
				this.MoveCurrencyOffDeletedElement();
				this._currentElementWasRemovedOrReplaced = false;
			}
			if (this.IsCurrentAfterLast != flag1) this.OnPropertyChanged("IsCurrentAfterLast");
			if (this.IsCurrentBeforeFirst != flag2) this.OnPropertyChanged("IsCurrentBeforeFirst");
			if (this._currentPosition != num) this.OnPropertyChanged("CurrentPosition");
			if (this._currentItem == obj) return;
			this.OnPropertyChanged("CurrentItem");
		}

		protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
			if (!this.CheckFlag(CollectionViewOriginal.CollectionViewFlags.ShouldProcessCollectionChanged)) return;
			if (!this.AllowsCrossThreadChanges) {
				if (!this.CheckAccess()) throw new NotSupportedException(System.Windows.SR.Get("MultiThreadedCollectionChangeNotSupported"));
				this.ProcessCollectionChanged(args);
			} else this.PostChange(args);
		}

		protected virtual void OnAllowsCrossThreadChangesChanged() { }

		protected void ClearPendingChanges() {
			lock (this._changeLog.SyncRoot) {
				this._changeLog.Clear();
				this._tempChangeLog.Clear();
			}
		}

		protected void ProcessPendingChanges() {
			lock (this._changeLog.SyncRoot) {
				this.ProcessChangeLog(this._changeLog, true);
				this._changeLog.Clear();
			}
		}

		[Obsolete("Replaced by OnAllowsCrossThreadChangesChanged")]
		protected virtual void OnBeginChangeLogging(NotifyCollectionChangedEventArgs args) { }

		[Obsolete("Replaced by ClearPendingChanges")]
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		protected void ClearChangeLog() { this.ClearPendingChanges(); }

		protected void RefreshOrDefer() {
			if (this.IsRefreshDeferred) this.SetFlag(CollectionView.CollectionViewFlags.NeedsRefresh, true);
			else this.RefreshInternal();
		}

		internal void SetAllowsCrossThreadChanges(bool value) {
			if (this.CheckFlag(CollectionView.CollectionViewFlags.AllowsCrossThreadChanges) == value) return;
			this.SetFlag(CollectionView.CollectionViewFlags.AllowsCrossThreadChanges, value);
			this.OnAllowsCrossThreadChangesChanged();
		}

		internal void SetViewManagerData(object value) {
			if (this._vmData == null) { this._vmData = value; } else {
				object[] objArray1;
				if ((objArray1 = this._vmData as object[]) == null) {
					this._vmData = (object) new object[2] {
						this._vmData,
						value
					};
				} else {
					object[] objArray2 = new object[objArray1.Length + 1];
					objArray1.CopyTo((Array) objArray2, 0);
					objArray2[objArray1.Length] = value;
					this._vmData = (object) objArray2;
				}
			}
		}

		internal virtual bool HasReliableHashCodes() {
			if (!this.IsEmpty) return HashHelper.HasReliableHashCode(this.GetItemAt(0));
			else return true;
		}

		internal void VerifyRefreshNotDeferred() {
			if (this.AllowsCrossThreadChanges) this.VerifyAccess();
			if (this.IsRefreshDeferred) throw new InvalidOperationException(System.Windows.SR.Get("NoCheckOrChangeWhenDeferred"));
		}

		internal void InvalidateEnumerableWrapper() {
			IndexedEnumerableOriginal indexedEnumerable = Interlocked.Exchange<IndexedEnumerableOriginal>(ref this._enumerableWrapper, (IndexedEnumerableOriginal) null);
			if (indexedEnumerable == null) return;
			indexedEnumerable.Invalidate();
		}

		internal ReadOnlyCollection<ItemPropertyInfo> GetItemProperties() {
			IEnumerable sourceCollection = this.SourceCollection;
			if (sourceCollection == null) return (ReadOnlyCollection<ItemPropertyInfo>) null;
			IEnumerable enumerable = (IEnumerable) null;
			ITypedList typedList = sourceCollection as ITypedList;
			if (typedList != null) { enumerable = (IEnumerable) typedList.GetItemProperties((PropertyDescriptor[]) null); } else {
				Type itemType;
				if ((itemType = this.GetItemType(false)) != (Type) null) { enumerable = (IEnumerable) TypeDescriptor.GetProperties(itemType); } else {
					object representativeItem;
					if ((representativeItem = this.GetRepresentativeItem()) != null) {
						ICustomTypeProvider customTypeProvider = representativeItem as ICustomTypeProvider;
						enumerable = customTypeProvider != null ? (IEnumerable) customTypeProvider.GetCustomType().GetProperties() : (IEnumerable) TypeDescriptor.GetProperties(representativeItem);
					}
				}
			}
			if (enumerable == null) return (ReadOnlyCollection<ItemPropertyInfo>) null;
			List<ItemPropertyInfo> list = new List<ItemPropertyInfo>();
			foreach (object obj in enumerable) {
				PropertyDescriptor propertyDescriptor;
				if ((propertyDescriptor = obj as PropertyDescriptor) != null) { list.Add(new ItemPropertyInfo(propertyDescriptor.Name, propertyDescriptor.PropertyType, (object) propertyDescriptor)); } else {
					PropertyInfo propertyInfo;
					if ((propertyInfo = obj as PropertyInfo) != (PropertyInfo) null) list.Add(new ItemPropertyInfo(propertyInfo.Name, propertyInfo.PropertyType, (object) propertyInfo));
				}
			}
			return new ReadOnlyCollection<ItemPropertyInfo>((IList<ItemPropertyInfo>) list);
		}

		internal Type GetItemType(bool useRepresentativeItem) {
			foreach (Type type in this.SourceCollection.GetType().GetInterfaces()) {
				if (type.Name == CollectionView.IEnumerableT) {
					Type[] genericArguments = type.GetGenericArguments();
					if (genericArguments.Length == 1) {
						Type c = genericArguments[0];
						if (!typeof (ICustomTypeProvider).IsAssignableFrom(c)) { if (!(c == typeof (object))) return c; } else break;
					}
				}
			}
			if (useRepresentativeItem) return ReflectionHelper.GetReflectionType(this.GetRepresentativeItem());
			else return (Type) null;
		}

		internal object GetRepresentativeItem() {
			if (this.IsEmpty) return (object) null;
			object obj = (object) null;
			IEnumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext()) {
				object current = enumerator.Current;
				if (current != null && current != CollectionView.NewItemPlaceholder) {
					obj = current;
					break;
				}
			}
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null) disposable.Dispose();
			return obj;
		}

		internal virtual void GetCollectionChangedSources(int level, Action<int, object, bool?, List<string>> format, List<string> sources) {
			format(level, (object) this, new bool?(), sources);
			if (this._sourceCollection == null) return;
			format(level + 1, (object) this._sourceCollection, new bool?(), sources);
		}

		private bool CheckFlag(CollectionView.CollectionViewFlags flags) { return (this._flags & flags) != (CollectionView.CollectionViewFlags) 0; }

		private void _MoveCurrentToPosition(int position) {
			if (position < 0) {
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst, true);
				this.SetCurrent((object) null, -1);
			} else if (position >= this.Count) {
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast, true);
				this.SetCurrent((object) null, this.Count);
			} else {
				this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst | CollectionView.CollectionViewFlags.IsCurrentAfterLast, false);
				this.SetCurrent(this.EnumerableWrapper[position], position);
			}
		}

		private void MoveCurrencyOffDeletedElement() {
			int num = this.Count - 1;
			int position = this._currentPosition < num ? this._currentPosition : num;
			this.OnCurrentChanging();
			this._MoveCurrentToPosition(position);
			this.OnCurrentChanged();
		}

		private void EndDefer() {
			--this._deferLevel;
			if (this._deferLevel != 0 || !this.CheckFlag(CollectionView.CollectionViewFlags.NeedsRefresh)) return;
			this.Refresh();
		}

		private void DeferProcessing(ICollection changeLog) {
			lock (this.SyncRoot) {
				lock (this._changeLog.SyncRoot) {
					if (this._changeLog == null) this._changeLog = new ArrayList(changeLog);
					else this._changeLog.InsertRange(0, changeLog);
					if (this._databindOperation != null) this._engine.ChangeCost(this._databindOperation, changeLog.Count);
					else this._databindOperation = this._engine.Marshal(new DispatcherOperationCallback(this.ProcessInvoke), (object) null, changeLog.Count);
				}
			}
		}

		private ICollection ProcessChangeLog(ArrayList changeLog, bool processAll = false) {
			int count1 = 0;
			bool flag = false;
			long ticks = DateTime.Now.Ticks;
			int count2 = changeLog.Count;
			for (; count1 < changeLog.Count && !flag; ++count1) {
				NotifyCollectionChangedEventArgs args = changeLog[count1] as NotifyCollectionChangedEventArgs;
				if (args != null) this.ProcessCollectionChanged(args);
				if (!processAll) flag = DateTime.Now.Ticks - ticks > 50000L;
			}
			if (!flag || count1 >= changeLog.Count) return (ICollection) null;
			changeLog.RemoveRange(0, count1);
			return (ICollection) changeLog;
		}

		private void SetFlag(CollectionView.CollectionViewFlags flags, bool value) {
			if (value) this._flags = this._flags | flags;
			else this._flags = this._flags & ~flags;
		}

		private void PostChange(NotifyCollectionChangedEventArgs args) {
			lock (this.SyncRoot) {
				lock (this._changeLog.SyncRoot) {
					if (args.Action == NotifyCollectionChangedAction.Reset) this._changeLog.Clear();
					if (this._changeLog.Count == 0 && this.CheckAccess()) { this.ProcessCollectionChanged(args); } else {
						this._changeLog.Add((object) args);
						if (this._databindOperation != null) return;
						this._databindOperation = this._engine.Marshal(new DispatcherOperationCallback(this.ProcessInvoke), (object) null, this._changeLog.Count);
					}
				}
			}
		}

		private object ProcessInvoke(object arg) {
			lock (this.SyncRoot) {
				lock (this._changeLog.SyncRoot) {
					this._databindOperation = (DataBindOperation) null;
					this._tempChangeLog = this._changeLog;
					this._changeLog = new ArrayList();
				}
			}
			ICollection changeLog = this.ProcessChangeLog(this._tempChangeLog, false);
			if (changeLog != null && changeLog.Count > 0) this.DeferProcessing(changeLog);
			this._tempChangeLog = CollectionView.EmptyArrayList;
			return (object) null;
		}

		private void ValidateCollectionChangedEventArgs(NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems.Count == 1) break;
					else throw new NotSupportedException(System.Windows.SR.Get("RangeActionsNotSupported"));
				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems.Count != 1) throw new NotSupportedException(System.Windows.SR.Get("RangeActionsNotSupported"));
					if (e.OldStartingIndex >= 0) break;
					else throw new InvalidOperationException(System.Windows.SR.Get("RemovedItemNotFound"));
				case NotifyCollectionChangedAction.Replace:
					if (e.NewItems.Count == 1 && e.OldItems.Count == 1) break;
					else throw new NotSupportedException(System.Windows.SR.Get("RangeActionsNotSupported"));
				case NotifyCollectionChangedAction.Move:
					if (e.NewItems.Count != 1) throw new NotSupportedException(System.Windows.SR.Get("RangeActionsNotSupported"));
					if (e.NewStartingIndex >= 0) break;
					else throw new InvalidOperationException(System.Windows.SR.Get("CannotMoveToUnknownPosition"));
				case NotifyCollectionChangedAction.Reset:
					break;
				default:
					throw new NotSupportedException(System.Windows.SR.Get("UnexpectedCollectionChangeAction", new object[1] {
						(object) e.Action
					}));
			}
		}

		private void AdjustCurrencyForAdd(int index) {
			if (this.Count == 1) { this._currentPosition = -1; } else {
				if (index > this._currentPosition) return;
				++this._currentPosition;
				if (this._currentPosition >= this.Count) return;
				this._currentItem = this.EnumerableWrapper[this._currentPosition];
			}
		}

		private void AdjustCurrencyForRemove(int index) {
			if (index < this._currentPosition) { --this._currentPosition; } else {
				if (index != this._currentPosition) return;
				this._currentElementWasRemovedOrReplaced = true;
			}
		}

		private void AdjustCurrencyForMove(int oldIndex, int newIndex) {
			if (oldIndex < this.CurrentPosition && newIndex < this.CurrentPosition || oldIndex > this.CurrentPosition && newIndex > this.CurrentPosition) return;
			if (oldIndex <= this.CurrentPosition) { this.AdjustCurrencyForRemove(oldIndex); } else {
				if (newIndex > this.CurrentPosition) return;
				this.AdjustCurrencyForAdd(newIndex);
			}
		}

		private void AdjustCurrencyForReplace(int index) {
			if (index != this._currentPosition) return;
			this._currentElementWasRemovedOrReplaced = true;
		}

		private void OnPropertyChanged(string propertyName) { this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName)); }

		internal class PlaceholderAwareEnumerator : IEnumerator {

			private CollectionView _collectionView;
			private IEnumerator _baseEnumerator;
			private NewItemPlaceholderPosition _placeholderPosition;
			private CollectionView.PlaceholderAwareEnumerator.Position _position;
			private object _newItem;
			private int _timestamp;

			public object Current {
				get {
					if (this._position == CollectionView.PlaceholderAwareEnumerator.Position.OnPlaceholder) return CollectionView.NewItemPlaceholder;
					if (this._position != CollectionView.PlaceholderAwareEnumerator.Position.OnNewItem) return this._baseEnumerator.Current;
					else return this._newItem;
				}
			}

			public PlaceholderAwareEnumerator(CollectionView collectionView, IEnumerator baseEnumerator, NewItemPlaceholderPosition placeholderPosition, object newItem) {
				this._collectionView = collectionView;
				this._timestamp = collectionView.Timestamp;
				this._baseEnumerator = baseEnumerator;
				this._placeholderPosition = placeholderPosition;
				this._newItem = newItem;
			}

			public bool MoveNext() {
				if (this._timestamp != this._collectionView.Timestamp) throw new InvalidOperationException(System.Windows.SR.Get("EnumeratorVersionChanged"));
				switch (this._position) {
					case CollectionView.PlaceholderAwareEnumerator.Position.BeforePlaceholder:
						if (this._placeholderPosition == NewItemPlaceholderPosition.AtBeginning) this._position = CollectionView.PlaceholderAwareEnumerator.Position.OnPlaceholder;
						else if (!this._baseEnumerator.MoveNext() || this._newItem != CollectionView.NoNewItem && this._baseEnumerator.Current == this._newItem && !this._baseEnumerator.MoveNext()) {
							if (this._newItem != CollectionView.NoNewItem) { this._position = CollectionView.PlaceholderAwareEnumerator.Position.OnNewItem; } else {
								if (this._placeholderPosition == NewItemPlaceholderPosition.None) return false;
								this._position = CollectionView.PlaceholderAwareEnumerator.Position.OnPlaceholder;
							}
						}
						return true;
					case CollectionView.PlaceholderAwareEnumerator.Position.OnPlaceholder:
						if (this._newItem != CollectionView.NoNewItem && this._placeholderPosition == NewItemPlaceholderPosition.AtBeginning) {
							this._position = CollectionView.PlaceholderAwareEnumerator.Position.OnNewItem;
							return true;
						} else break;
					case CollectionView.PlaceholderAwareEnumerator.Position.OnNewItem:
						if (this._placeholderPosition == NewItemPlaceholderPosition.AtEnd) {
							this._position = CollectionView.PlaceholderAwareEnumerator.Position.OnPlaceholder;
							return true;
						} else break;
				}
				this._position = CollectionView.PlaceholderAwareEnumerator.Position.AfterPlaceholder;
				if (!this._baseEnumerator.MoveNext()) return false;
				if (this._newItem != CollectionView.NoNewItem && this._baseEnumerator.Current == this._newItem) return this._baseEnumerator.MoveNext();
				else return true;
			}

			public void Reset() {
				this._position = CollectionView.PlaceholderAwareEnumerator.Position.BeforePlaceholder;
				this._baseEnumerator.Reset();
			}

			private enum Position {

				BeforePlaceholder,
				OnPlaceholder,
				OnNewItem,
				AfterPlaceholder,

			}

		}

		private class SimpleMonitor : IDisposable {

			private bool _entered;

			public bool Busy {
				[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._entered; }
			}

			public bool Enter() {
				if (this._entered) return false;
				this._entered = true;
				return true;
			}

			public void Dispose() {
				this._entered = false;
				GC.SuppressFinalize((object) this);
			}

		}

		[System.Flags]
		private enum CollectionViewFlags {

			UpdatedOutsideDispatcher = 2,
			ShouldProcessCollectionChanged = 4,
			IsCurrentBeforeFirst = 8,
			IsCurrentAfterLast = 16,
			IsDynamic = 32,
			IsDataInGroupOrder = 64,
			NeedsRefresh = 128,
			AllowsCrossThreadChanges = 256,
			CachedIsEmpty = 512,

		}

		private class DeferHelper : IDisposable {

			private CollectionView _collectionView;

			public DeferHelper(CollectionView collectionView) { this._collectionView = collectionView; }

			public void Dispose() {
				if (this._collectionView != null) {
					this._collectionView.EndDefer();
					this._collectionView = (CollectionView) null;
				}
				GC.SuppressFinalize((object) this);
			}

		}

	}

}