using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace KsWare.Presentation.DataVirtualization {

	public class VirtualList<T> : VirtualListBase<T>, ICollectionViewFactory where T : class {

		internal readonly Func<int, SortDescriptionCollection, Predicate<object>, T[], int> Load;

		public VirtualList(Func<int, SortDescriptionCollection, Predicate<object>, T[], int> load, int numCacheBlocks, int cacheBlockLength)
			: base(numCacheBlocks, cacheBlockLength) { Load = load; }

		public VirtualList(Func<int, SortDescriptionCollection, Predicate<object>, T[], int> load)
			: base(5, 200) { Load = load; }

		protected override int InternalLoad(int startIndex, T[] data) { return Load(startIndex, null, null, data); }

		#region ICollectionViewFactory Members

		public ICollectionView CreateView() { return new VirtualListCollectionView<T>(this); }

		#endregion

		#region EXPERIMENTAL "Add" support

		public int Add(T item) {
			var index = Count;
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item));
			return index;
		}

		public override int Add(object value) { return Add((T)value); }

		public void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e) {
			var oldCount = Count;

			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					if (e.NewStartingIndex != -1 /*Insert*/&& e.NewStartingIndex != oldCount) {
						throw new InvalidOperationException("Action not supported." +
						"\n\t"+"Action: "+"Add (Insert)" +
						"\n\t"+"UniqueID: {347B25FD-8F9F-463A-AD33-430EC4D815B2}");
					}
					Cache.NotifyCollectionChanged(e);
					if (e.NewItems.Count > 1) {
						var firstNewIndex = oldCount;
						var newIndex = firstNewIndex;
						var newItems = e.NewItems.Cast<object>().Select(x => new CachedDataRef(newIndex++, this)).ToArray();
						OnPropertyChanged(CountChanged);
						OnPropertyChanged(IndexerChanged);
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,newItems));
					} else {
						var newIndex = oldCount;
						OnPropertyChanged(CountChanged);
						OnPropertyChanged(IndexerChanged);
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,new CachedDataRef(newIndex, this)));
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					Cache.NotifyCollectionChanged(e);
					OnPropertyChanged(CountChanged);
					OnPropertyChanged(IndexerChanged);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					break;
//				case NotifyCollectionChangedAction.Remove : break; //TODO
//				case NotifyCollectionChangedAction.Replace: break; //TODO
//				case NotifyCollectionChangedAction.Move   : break; //TODO
				default:
					throw new InvalidOperationException("Action not supported." +
						"\n\t"+"Action: "+e.Action+
						"\n\t"+"UniqueID: {347B25FD-8F9F-463A-AD33-430EC4D815B2}");
			}
		}

		#endregion
	}

}