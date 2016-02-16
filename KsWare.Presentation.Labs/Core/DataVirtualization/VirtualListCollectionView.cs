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
		
		private readonly VirtualList<T> m_SourceCollection;
		private readonly Func<int, SortDescriptionCollection, Predicate<object>, T[], int> Load;
		private int m_DeferRefreshCount;
		private bool m_NeedsRefresh;
		private CultureInfo m_CultureInfo;
		private int m_CurrentPosition;
		private DataRefBase<T> m_CurrentItem;
		private bool m_IsCurrentAfterLast;
		private bool m_IsCurrentBeforeFirst;
		private Predicate<object> m_Filter;
		private SortDescriptionCollection m_SortDescriptionCollection;

		public VirtualListCollectionView(VirtualList<T> list)
			: base(list.Cache.NumCacheBlocks, list.Cache.NumItemsPerCacheBlock) 
		{
			Load = list.Load;
			m_SourceCollection = list;

			
			m_SourceCollection.CollectionChanged += AtSourceCollectionChanged; //EXPERIMENTAL "Add" Support

			// initialize current item and markers
			if (list.Count == 0) m_IsCurrentAfterLast = m_IsCurrentBeforeFirst = true;
			else {
				m_CurrentPosition = 0;
				m_CurrentItem = list[0];
			}
			m_NeedsRefresh = true;
		}

		#region EXPERIMENTAL "Add" Support

		private void AtSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
//					var data = m_SourceCollection[m_SourceCollection.Count - 1].Data;
					var data = ((CachedDataRef) e.NewItems[0]).Data;
					if (PassesFilter(data)) {
						var unfilteredIndex= m_SourceCollection.Count-1; // Count is Count after add item in source collection
						var filteredIndex = Count;// Count is Count before add item in this view
						if (filteredIndex%Cache.NumItemsPerCacheBlock == 0) {
							m_FilterMap.Add(filteredIndex, unfilteredIndex);
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

//		protected override int InternalLoad(T[] data, int startIndex) {return Load(m_SortDescriptionCollection, m_Filter, data, startIndex);}

		private Dictionary<int,int> m_FilterMap;

		protected override int InternalLoad(int startIndex, T[] data) {

			int count;

			#region create filter index

			if (m_FilterMap == null /*reset requiered*/) {
				if (m_Filter != null) {
					count = 0; // will be increased for each filter match
					m_FilterMap = new Dictionary<int, int>();
					var unfilteredCount = m_SourceCollection.Count;
					int iFiltered = -1;

					for (int i = 0; i < unfilteredCount; i++) {
						var p = m_SourceCollection[i].Data;
						if (m_Filter.Invoke(p)) {
							iFiltered++;
							count++;
							if (iFiltered%Cache.NumItemsPerCacheBlock == 0) { m_FilterMap.Add(iFiltered, i); }
						}
					}
				} else /*no filter*/ {
					count = Load(startIndex, m_SortDescriptionCollection, null, data);
					m_FilterMap = new Dictionary<int, int>(); // let the map empty, we don't need this
				}
			} else {
				count = Cache.Count; 
			}

			#endregion

			if (m_Filter != null) {
				int unfilteredStartIndex;
				if (!m_FilterMap.TryGetValue(startIndex, out unfilteredStartIndex)) {
					return count; // TODO check this
				}
				var unfilteredCount = m_SourceCollection.Count;

				for (int i = 0, iUnfiltered = unfilteredStartIndex; iUnfiltered < unfilteredCount && i < Cache.NumItemsPerCacheBlock; ++iUnfiltered) {
					if (iUnfiltered == unfilteredStartIndex) {
						var item = m_SourceCollection[iUnfiltered].Data; /*DEBUG*/ if (item == null) { Debug.WriteLine("returning null!");} 
						data[i] = item;
						i++;
					} else {
						var item = m_SourceCollection[iUnfiltered].Data; /*DEBUG*/ if (item == null) { Debug.WriteLine("returning null!");} 
						if (m_Filter.Invoke(item)) {
							data[i] = item;
							i++;
						}
					}
				}
				return count;
			} else /*no filter*/ {
				return Load(startIndex, m_SortDescriptionCollection, null, data);
			}
		}

		private bool IsRefreshDeferred { get { return m_DeferRefreshCount > 0; } }

		private void ThrowIfDeferred() { if (IsRefreshDeferred) throw new Exception("Can't do this while I'm deferred"); }

		private void RefreshOrDefer() {
			if (IsRefreshDeferred) m_NeedsRefresh = true;
			else Refresh();
		}

		private void EndDeferRefresh() { if (0 == --m_DeferRefreshCount && m_NeedsRefresh) Refresh(); }

		private bool IsCurrentInView {
			get {
				ThrowIfDeferred();
				return CurrentPosition >= 0 && CurrentPosition < Count;
			}
		}

		private void OnCurrentChanged() { if (CurrentChanged != null) CurrentChanged(this, EventArgs.Empty); }

		private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e) { RefreshOrDefer(); }

		private void SetCurrent(DataRefBase<T> newItem, int newPosition, int count) {
			if (newItem != null) m_IsCurrentBeforeFirst = m_IsCurrentAfterLast = false;
			else if (count == 0) {
				m_IsCurrentBeforeFirst = m_IsCurrentAfterLast = true;
				newPosition = -1;
			} else {
				m_IsCurrentBeforeFirst = newPosition < 0;
				m_IsCurrentAfterLast = newPosition >= count;
			}
			m_CurrentItem = newItem;
			m_CurrentPosition = newPosition;
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

		public bool CanFilter { get { return true; } }

		public bool CanGroup { get { return false; } }

		public bool CanSort { get { return true; } }

		public CultureInfo Culture {
			get { return m_CultureInfo; }
			set {
				if (value == null) throw new ArgumentNullException("value");
				if (m_CultureInfo != value) {
					m_CultureInfo = value;
					OnPropertyChanged(CulturePropertyChanged);
				}
			}
		}

		public event EventHandler CurrentChanged;

		public event CurrentChangingEventHandler CurrentChanging;

		public object CurrentItem {
			get {
				ThrowIfDeferred();
				return m_CurrentItem;
			}
		}

		public int CurrentPosition {
			get {
				ThrowIfDeferred();
				return m_CurrentPosition;
			}
		}

		public IDisposable DeferRefresh() {
			++m_DeferRefreshCount;
			return new RefreshDeferrer(this);
		}

		public Predicate<object> Filter {
			get { return m_Filter; }
			set {
				if (!CanFilter) throw new NotSupportedException("Filter not supported");
				m_Filter = value;
				RefreshOrDefer();
			}
		}

		public ObservableCollection<GroupDescription> GroupDescriptions { get { return null; } }

		public ReadOnlyObservableCollection<object> Groups { get { return null; } }

		public bool IsCurrentAfterLast {
			get {
				ThrowIfDeferred();
				return m_IsCurrentAfterLast;
			}
		}

		public bool IsCurrentBeforeFirst {
			get {
				ThrowIfDeferred();
				return m_IsCurrentBeforeFirst;
			}
		}

		public bool IsEmpty { get { return Count == 0; } }

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
			if (position < -1 || position > Count) throw new ArgumentOutOfRangeException("position");
			if (position != CurrentPosition && OnCurrentChanging()) {
				bool isCurrentBeforeFirst = m_IsCurrentBeforeFirst;
				bool isCurrentAfterLast = m_IsCurrentAfterLast;
				if (position < 0) {
					m_IsCurrentBeforeFirst = true;
					SetCurrent(null, -1);
				} else if (position >= Count) {
					m_IsCurrentAfterLast = true;
					SetCurrent(null, Count);
				} else {
					m_IsCurrentBeforeFirst = m_IsCurrentAfterLast = false;
					SetCurrent(this[position], position);
				}
				OnCurrentChanged();
				if (isCurrentBeforeFirst != m_IsCurrentBeforeFirst) OnPropertyChanged(IsCurrentBeforeFirstChanged);
				if (isCurrentAfterLast != m_IsCurrentAfterLast) OnPropertyChanged(IsCurrentAfterLastChanged);
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
			m_FilterMap = null;
			base.ClearCache();
		}

		public void Refresh() {
			var currentItem           = (DataRefBase<T>) CurrentItem;
			int currentPosition       = IsEmpty ? -1 : CurrentPosition;
			bool isCurrentBeforeFirst = m_IsCurrentBeforeFirst;
			bool isCurrentAfterLast   = m_IsCurrentAfterLast;
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
			m_NeedsRefresh = false;
			OnCollectionReset();
			OnCurrentChanged();
			if (isCurrentBeforeFirst != m_IsCurrentBeforeFirst) OnPropertyChanged(IsCurrentBeforeFirstChanged);
			if (isCurrentAfterLast   != m_IsCurrentAfterLast  ) OnPropertyChanged(IsCurrentAfterLastChanged);
			if (currentPosition      != CurrentPosition       ) OnPropertyChanged(CurrentPositionChanged);
			if (currentItem          != CurrentItem           ) OnPropertyChanged(CurrentItemChanged);
		}

		public void RefreshForAdd() {//EXPERIMENTAL for "Add" support
			var currentItem           = (DataRefBase<T>) CurrentItem;
			int currentPosition       = IsEmpty ? -1 : CurrentPosition;
			bool isCurrentBeforeFirst = m_IsCurrentBeforeFirst;
			bool isCurrentAfterLast   = m_IsCurrentAfterLast;
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
			m_NeedsRefresh = false;
			OnCollectionReset();
			OnCurrentChanged();
			if (isCurrentBeforeFirst != m_IsCurrentBeforeFirst) OnPropertyChanged(IsCurrentBeforeFirstChanged);
			if (isCurrentAfterLast   != m_IsCurrentAfterLast  ) OnPropertyChanged(IsCurrentAfterLastChanged);
			if (currentPosition      != CurrentPosition       ) OnPropertyChanged(CurrentPositionChanged);
			if (currentItem          != CurrentItem           ) OnPropertyChanged(CurrentItemChanged);
		}

		public SortDescriptionCollection SortDescriptions {
			get {
				if (m_SortDescriptionCollection == null) {
					m_SortDescriptionCollection = new SortDescriptionCollection();
					((INotifyCollectionChanged) m_SortDescriptionCollection).CollectionChanged += SortDescriptionsChanged;
				}
				return m_SortDescriptionCollection;
			}
		}


		public IEnumerable SourceCollection { get { return m_SourceCollection; } }

		#endregion

		private class RefreshDeferrer : IDisposable {

			private VirtualListCollectionView<T> m_List;

			public RefreshDeferrer(VirtualListCollectionView<T> list) { m_List = list; }

			#region IDisposable Members

			public void Dispose() {
				if (m_List != null) {
					m_List.EndDeferRefresh();
					m_List = null;
				}
			}

			#endregion
		}

	}

}