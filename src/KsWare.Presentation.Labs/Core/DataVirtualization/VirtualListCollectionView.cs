using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace KsWare.Presentation.DataVirtualization {

	public class VirtualListCollectionView<T> : VirtualListBase<T>, ICollectionView where T : class {

// ReSharper disable StaticFieldInGenericType
		private static readonly PropertyChangedEventArgs CulturePropertyChanged      = new PropertyChangedEventArgs("Culture");
		private static readonly PropertyChangedEventArgs IsCurrentBeforeFirstChanged = new PropertyChangedEventArgs("IsCurrentBeforeFirst");
		private static readonly PropertyChangedEventArgs IsCurrentAfterLastChanged   = new PropertyChangedEventArgs("IsCurrentAfterLast");
		private static readonly PropertyChangedEventArgs CurrentPositionChanged      = new PropertyChangedEventArgs("CurrentPosition");
		private static readonly PropertyChangedEventArgs CurrentItemChanged          = new PropertyChangedEventArgs("CurrentItem");
// ReSharper restore StaticFieldInGenericType
		
		private readonly VirtualList<T> _sourceCollection;
		private readonly Func<int, SortDescriptionCollection, Predicate<object>, T[], int> _loadFunc;
		private int _deferRefreshCount;
		private bool _needsRefresh;
		private CultureInfo _cultureInfo;
		private int _currentPosition;
		private DataRefBase<T> _currentItem;
		private bool _isCurrentAfterLast;
		private bool _isCurrentBeforeFirst;
		private Predicate<object> _filter;
		private SortDescriptionCollection _sortDescriptionCollection;

		public VirtualListCollectionView(VirtualList<T> list)
			: base(list.Cache.NumCacheBlocks, list.Cache.NumItemsPerCacheBlock) 
		{
			_loadFunc = list.Load;
			_sourceCollection = list;

			
			_sourceCollection.CollectionChanged += AtSourceCollectionChanged; //EXPERIMENTAL "Add" Support

			// initialize current item and markers
			if (list.Count == 0) _isCurrentAfterLast = _isCurrentBeforeFirst = true;
			else {
				_currentPosition = 0;
				_currentItem = list[0];
			}
			_needsRefresh = true;
		}

		#region EXPERIMENTAL "Add" Support

		private void AtSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
//					var data = _SourceCollection[m_SourceCollection.Count - 1].Data;
					var data = ((CachedDataRef) e.NewItems[0]).Data;
					if (PassesFilter(data)) {
						var unfilteredIndex= _sourceCollection.Count-1; // Count is Count after add item in source collection
						var filteredIndex = Count;// Count is Count before add item in this view
						if (filteredIndex%Cache.NumItemsPerCacheBlock == 0) {
							_FilterMap.Add(filteredIndex, unfilteredIndex);
						}
						Cache.NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,data));
						RefreshForAdd();
					}
					break;
				case NotifyCollectionChangedAction.Remove:  // not optimized. same as Reset
				case NotifyCollectionChangedAction.Move:    // not optimized. same as Reset
				case NotifyCollectionChangedAction.Replace: // not optimized. same as Reset
				case NotifyCollectionChangedAction.Reset:
					Cache.NotifyCollectionChanged(e); 
					Refresh();
					break;
			}

		}

		#endregion

//		protected override int InternalLoad(T[] data, int startIndex) {return Load(_SortDescriptionCollection, _Filter, data, startIndex);}

		private Dictionary<int,int> _FilterMap;

		protected override int InternalLoad(int startIndex, T[] data) {

			int count;

			#region create filter index

			if (_FilterMap == null /*reset requiered*/) {
				if (_filter != null) {
					count = 0; // will be increased for each filter match
					_FilterMap = new Dictionary<int, int>();
					var unfilteredCount = _sourceCollection.Count;
					int iFiltered = -1;

					for (int i = 0; i < unfilteredCount; i++) {
						var p = _sourceCollection[i].Data;
						if (_filter.Invoke(p)) {
							iFiltered++;
							count++;
							if (iFiltered%Cache.NumItemsPerCacheBlock == 0) { _FilterMap.Add(iFiltered, i); }
						}
					}
				} else /*no filter*/ {
					count = _loadFunc(startIndex, _sortDescriptionCollection, null, data);
					_FilterMap = new Dictionary<int, int>(); // let the map empty, we don't need this
				}
			} else {
				count = Cache.Count; 
			}

			#endregion

			if (_filter != null) {
				int unfilteredStartIndex;
				if (!_FilterMap.TryGetValue(startIndex, out unfilteredStartIndex)) {
					return count; // TODO check this
				}
				var unfilteredCount = _sourceCollection.Count;

				for (int i = 0, iUnfiltered = unfilteredStartIndex; iUnfiltered < unfilteredCount && i < Cache.NumItemsPerCacheBlock; ++iUnfiltered) {
					if (iUnfiltered == unfilteredStartIndex) {
						var item = _sourceCollection[iUnfiltered].Data; /*DEBUG*/ if (item == null) { Debug.WriteLine("returning null!");} 
						data[i] = item;
						i++;
					} else {
						var item = _sourceCollection[iUnfiltered].Data; /*DEBUG*/ if (item == null) { Debug.WriteLine("returning null!");} 
						if (_filter.Invoke(item)) {
							data[i] = item;
							i++;
						}
					}
				}
				return count;
			} else /*no filter*/ {
				return _loadFunc(startIndex, _sortDescriptionCollection, null, data);
			}
		}

		private bool IsRefreshDeferred => _deferRefreshCount > 0;

		private void ThrowIfDeferred() { if (IsRefreshDeferred) throw new Exception("Can't do this while I'm deferred"); }

		private void RefreshOrDefer() {
			if (IsRefreshDeferred) _needsRefresh = true;
			else Refresh();
		}

		private void EndDeferRefresh() { if (0 == --_deferRefreshCount && _needsRefresh) Refresh(); }

		private bool IsCurrentInView {
			get {
				ThrowIfDeferred();
				return CurrentPosition >= 0 && CurrentPosition < Count;
			}
		}

		private void OnCurrentChanged() { if (CurrentChanged != null) CurrentChanged(this, EventArgs.Empty); }

		private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e) { RefreshOrDefer(); }

		private void SetCurrent(DataRefBase<T> newItem, int newPosition, int count) {
			if (newItem != null) _isCurrentBeforeFirst = _isCurrentAfterLast = false;
			else if (count == 0) {
				_isCurrentBeforeFirst = _isCurrentAfterLast = true;
				newPosition = -1;
			} else {
				_isCurrentBeforeFirst = newPosition < 0;
				_isCurrentAfterLast = newPosition >= count;
			}
			_currentItem = newItem;
			_currentPosition = newPosition;
		}

		private void SetCurrent(DataRefBase<T> newItem, int newPosition) {
			int count = newItem != null ? 0 : Count;
			SetCurrent(newItem, newPosition, count);
		}

		private bool PassesFilter(object item) { return (!CanFilter || Filter == null) || Filter(item); }

		private bool OnCurrentChanging() {
			if (CurrentChanging == null) return true;
			else {
				CurrentChangingEventArgs e = new CurrentChangingEventArgs();
				CurrentChanging(this, e);
				return !e.Cancel;
			}
		}

		#region ICollectionView Members

		public bool CanFilter => true;

		public bool CanGroup => false;

		public bool CanSort => true;

		public CultureInfo Culture {
			get => _cultureInfo;
			set {
				if (value == null) throw new ArgumentNullException(nameof(value));
				if (_cultureInfo != value) {
					_cultureInfo = value;
					OnPropertyChanged(CulturePropertyChanged);
				}
			}
		}

		public event EventHandler CurrentChanged;

		public event CurrentChangingEventHandler CurrentChanging;

		public object CurrentItem {
			get {
				ThrowIfDeferred();
				return _currentItem;
			}
		}

		public int CurrentPosition {
			get {
				ThrowIfDeferred();
				return _currentPosition;
			}
		}

		public IDisposable DeferRefresh() {
			++_deferRefreshCount;
			return new RefreshDeferrer(this);
		}

		public Predicate<object> Filter {
			get => _filter;
			set {
				if (!CanFilter) throw new NotSupportedException("Filter not supported");
				_filter = value;
				RefreshOrDefer();
			}
		}

		public ObservableCollection<GroupDescription> GroupDescriptions => null;

		public ReadOnlyObservableCollection<object> Groups => null;

		public bool IsCurrentAfterLast {
			get {
				ThrowIfDeferred();
				return _isCurrentAfterLast;
			}
		}

		public bool IsCurrentBeforeFirst {
			get {
				ThrowIfDeferred();
				return _isCurrentBeforeFirst;
			}
		}

		public bool IsEmpty => Count == 0;

		public bool MoveCurrentTo(object item) {
			ThrowIfDeferred();
			if (Object.Equals(CurrentItem, item) && (item != null || IsCurrentInView)) return IsCurrentInView;
			int position = -1;
			if (PassesFilter(item)) position = IndexOf(item);
			return MoveCurrentToPosition(position);
		}

		public bool MoveCurrentToFirst() {
			ThrowIfDeferred();
			return MoveCurrentToPosition(0);
		}

		public bool MoveCurrentToLast() {
			ThrowIfDeferred();
			return MoveCurrentToPosition(Count - 1);
		}

		public bool MoveCurrentToNext() {
			ThrowIfDeferred();
			int position = CurrentPosition + 1;
			return position <= Count && MoveCurrentToPosition(position);
		}

		public bool MoveCurrentToPosition(int position) {
			ThrowIfDeferred();
			if (position < -1 || position > Count) throw new ArgumentOutOfRangeException(nameof(position));
			if (position != CurrentPosition && OnCurrentChanging()) {
				bool isCurrentBeforeFirst = _isCurrentBeforeFirst;
				bool isCurrentAfterLast = _isCurrentAfterLast;
				if (position < 0) {
					_isCurrentBeforeFirst = true;
					SetCurrent(null, -1);
				} else if (position >= Count) {
					_isCurrentAfterLast = true;
					SetCurrent(null, Count);
				} else {
					_isCurrentBeforeFirst = _isCurrentAfterLast = false;
					SetCurrent(this[position], position);
				}
				OnCurrentChanged();
				if (isCurrentBeforeFirst != _isCurrentBeforeFirst) OnPropertyChanged(IsCurrentBeforeFirstChanged);
				if (isCurrentAfterLast != _isCurrentAfterLast) OnPropertyChanged(IsCurrentAfterLastChanged);
				OnPropertyChanged(CurrentPositionChanged);
				OnPropertyChanged(CurrentItemChanged);
			}
			return IsCurrentInView;
		}

		public bool MoveCurrentToPrevious() {
			ThrowIfDeferred();
			int position = CurrentPosition - 1;
			return position >= -1 && MoveCurrentToPosition(position);
		}

		protected override void ClearCache() {
			_FilterMap = null;
			base.ClearCache();
		}

		public void Refresh() {
			var currentItem           = (DataRefBase<T>) CurrentItem;
			int currentPosition       = IsEmpty ? -1 : CurrentPosition;
			bool isCurrentBeforeFirst = _isCurrentBeforeFirst;
			bool isCurrentAfterLast   = _isCurrentAfterLast;
			OnCurrentChanging();
			ClearCache(); // raises a CollectionChanged! 
			if (isCurrentBeforeFirst || IsEmpty) {
				SetCurrent(null, 0);
			} else if (isCurrentAfterLast) {
				SetCurrent(null, Count);
			} else {
				int index = IndexOf(currentItem);
				if (index < 0) SetCurrent(null, -1);
				else SetCurrent(currentItem, index);
			}
			_needsRefresh = false;
			OnCollectionReset();
			OnCurrentChanged();
			if (isCurrentBeforeFirst != _isCurrentBeforeFirst) OnPropertyChanged(IsCurrentBeforeFirstChanged);
			if (isCurrentAfterLast   != _isCurrentAfterLast  ) OnPropertyChanged(IsCurrentAfterLastChanged);
			if (currentPosition      != CurrentPosition       ) OnPropertyChanged(CurrentPositionChanged);
			if (currentItem          != CurrentItem           ) OnPropertyChanged(CurrentItemChanged);
		}

		public void RefreshForAdd() {//EXPERIMENTAL for "Add" support
			var currentItem           = (DataRefBase<T>) CurrentItem;
			int currentPosition       = IsEmpty ? -1 : CurrentPosition;
			bool isCurrentBeforeFirst = _isCurrentBeforeFirst;
			bool isCurrentAfterLast   = _isCurrentAfterLast;
			OnCurrentChanging();

			// clears the cache, set count to undefined and raises a CollectionChanged! 
//			base.ClearCache();
			base.ClearCacheOptimizedForAdd();
			
			if (isCurrentBeforeFirst || IsEmpty) {
				SetCurrent(null, 0);
			} else if (isCurrentAfterLast) {
				SetCurrent(null, Count);
			} else {
				int index = IndexOf(currentItem);
				if (index < 0) SetCurrent(null, -1);
				else SetCurrent(currentItem, index);
			}
			_needsRefresh = false;
			OnCollectionReset();
			OnCurrentChanged();
			if (isCurrentBeforeFirst != _isCurrentBeforeFirst) OnPropertyChanged(IsCurrentBeforeFirstChanged);
			if (isCurrentAfterLast   != _isCurrentAfterLast  ) OnPropertyChanged(IsCurrentAfterLastChanged);
			if (currentPosition      != CurrentPosition       ) OnPropertyChanged(CurrentPositionChanged);
			if (currentItem          != CurrentItem           ) OnPropertyChanged(CurrentItemChanged);
		}

		public SortDescriptionCollection SortDescriptions {
			get {
				if (_sortDescriptionCollection == null) {
					_sortDescriptionCollection = new SortDescriptionCollection();
					((INotifyCollectionChanged) _sortDescriptionCollection).CollectionChanged += SortDescriptionsChanged;
				}
				return _sortDescriptionCollection;
			}
		}


		public IEnumerable SourceCollection => _sourceCollection;

		#endregion

		private class RefreshDeferrer : IDisposable {

			private VirtualListCollectionView<T> _list;

			public RefreshDeferrer(VirtualListCollectionView<T> list) { _list = list; }

			#region IDisposable Members

			public void Dispose() {
				if (_list != null) {
					_list.EndDeferRefresh();
					_list = null;
				}
			}

			#endregion
		}

	}

}