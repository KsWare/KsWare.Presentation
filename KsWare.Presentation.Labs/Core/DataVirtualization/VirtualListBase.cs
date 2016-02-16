using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace KsWare.Presentation.DataVirtualization {

	public abstract class VirtualListBase<T> : IList<DataRefBase<T>>, IList, IItemProperties, INotifyPropertyChanged, INotifyCollectionChanged where T : class {

		#region static

		// ReSharper disable StaticFieldInGenericType
		protected static readonly PropertyChangedEventArgs IndexerChanged = new PropertyChangedEventArgs("Item[]");
		protected static readonly PropertyChangedEventArgs CountChanged = new PropertyChangedEventArgs("Count");
		private static readonly NotifyCollectionChangedEventArgs CollectionReset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
		protected const int UndefinedCount = -1;
		private const int UninitializedCount = -1;
		private static readonly ReadOnlyCollection<ItemPropertyInfo> s_ItemPropertyInfo;
		// ReSharper restore StaticFieldInGenericType

		static VirtualListBase() {
			IList<ItemPropertyInfo> itemPropertyInfo = new List<ItemPropertyInfo>(CachedDataRef.PropertyDescriptorCollection.Count);
			foreach (PropertyDescriptor propertyDescriptor in CachedDataRef.PropertyDescriptorCollection) itemPropertyInfo.Add(new ItemPropertyInfo(propertyDescriptor.Name, propertyDescriptor.PropertyType, propertyDescriptor));
			s_ItemPropertyInfo = new ReadOnlyCollection<ItemPropertyInfo>(itemPropertyInfo);
		}

		#endregion

//		protected int m_Count;

		// for IList //
		private object m_SyncRoot;

		public readonly CacheImpl Cache;


		public VirtualListBase(int numCacheBlocks, int cacheBlockLength) {
//			m_Count = UninitializedCount;
			Cache   = new CacheImpl(InternalLoad, numCacheBlocks, cacheBlockLength, this);
		}

		protected void OnPropertyChanged(PropertyChangedEventArgs e) { if (PropertyChanged != null) PropertyChanged(this, e); }

		protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e) { if (CollectionChanged != null) CollectionChanged(this, e); }

		protected void OnCollectionReset() { OnCollectionChanged(CollectionReset); }

		private void NotifyCollectionChanged() {
			OnCollectionReset();
			OnPropertyChanged(IndexerChanged);
			OnPropertyChanged(CountChanged);
		}

		protected abstract int InternalLoad(int startIndex, T[] data);

		private T LoadData(int index) { return Cache.LoadData(index); }

		protected virtual void ClearCache() {
			Cache.Clear();
//			m_Count = UndefinedCount;
			NotifyCollectionChanged();
		}

		protected void ClearCacheOptimizedForAdd() {
			//TODO we do not really need to clear all, only because a new item

		}


		#region IList<DataRefBase<T>> Members

		public int IndexOf(DataRefBase<T> item) {
			var dataRef = item as CachedDataRef;
			return dataRef != null && dataRef.List == this ? dataRef.Index : -1;
		}

		public void Insert(int index, DataRefBase<T> item) { throw new NotImplementedException(); }

		public void RemoveAt(int index) { throw new NotImplementedException(); }

		public DataRefBase<T> this[int index] { get { return new CachedDataRef(index, this); } set { throw new NotImplementedException(); } }

		#endregion

		#region ICollection<DataRefBase<T>> Members

		public void Add(DataRefBase<T> item) { throw new NotImplementedException(); }

		public void Clear() { throw new NotImplementedException(); }

		public bool Contains(DataRefBase<T> item) { return item != null && ((CachedDataRef) item).List == this; }

		public void CopyTo(DataRefBase<T>[] array, int arrayIndex) {
			if (array == null) throw new ArgumentNullException("array");
			if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex");
			if (arrayIndex >= array.Length) throw new ArgumentException("arrayIndex is greater or equal than the array length");
			if (arrayIndex + Count > array.Length) throw new ArgumentException("Number of elements in list is greater than available space");
			foreach (var item in this) array[arrayIndex++] = item;
		}

		public int Count {
			get {
//				// if the count hasn't been determind yet, we need to access at least one remote item to get it
//				if (Cache.Count == UndefinedCount || Cache.Count == UninitializedCount) LoadData(0);
				return Cache.Count;
			}
		}

		public bool IsReadOnly { get { return true; } }

		public bool Remove(DataRefBase<T> item) { throw new NotImplementedException(); }

		#endregion

		#region IEnumerable<DataRefBase<T>> Members

		public IEnumerator<DataRefBase<T>> GetEnumerator() {
//            for (int index = 0; index < Count; ++index)
//                yield return this[index];
			return new VirtualEnumerator(this);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

		#endregion

		#region IList Members

		public virtual int Add(object value) { throw new NotImplementedException(); }

		public bool Contains(object value) {
			var dataRef = value as CachedDataRef;
			return dataRef != null && Contains(dataRef);
		}

		public int IndexOf(object value) {
			var dataRef = value as CachedDataRef;
			return dataRef != null ? IndexOf(dataRef) : -1;
		}

		public void Insert(int index, object value) { throw new NotImplementedException(); }

		public bool IsFixedSize { get { return true; } }

		public void Remove(object value) { throw new NotImplementedException(); }

		object IList.this[int index] { get { return this[index]; } set { throw new NotImplementedException(); } }

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index) {
			if (array == null) throw new ArgumentNullException("array");
			if (array.Rank != 1) throw new ArgumentException("Array rank must be 1");
			if (index < array.GetLowerBound(0)) throw new ArgumentOutOfRangeException("index");
			if (index > array.GetUpperBound(0)) throw new ArgumentException("arrayIndex is greater or equal than the array upper bound");
			if (index + Count - 1 > array.GetUpperBound(0)) throw new ArgumentException("Number of elements in list is greater than available space");
			if (array is DataRefBase<T>[]) CopyTo((DataRefBase<T>[]) array, index);
			else
				try { foreach (var t in this) array.SetValue(t, index++); }
				catch (Exception e) {
					throw new InvalidCastException("Type of datalist cannot be converted to the type of the destination array", e);
				}
		}

		public bool IsSynchronized { get { return false; } }

		public object SyncRoot {
			get {
				if (m_SyncRoot == null) Interlocked.CompareExchange(ref m_SyncRoot, new object(), null);
				return m_SyncRoot;
			}
		}

		#endregion

		#region IItemProperties Members

		public ReadOnlyCollection<ItemPropertyInfo> ItemProperties { get { return s_ItemPropertyInfo; } }

		#endregion

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private class VirtualEnumerator : IEnumerator<DataRefBase<T>> {

			// this class is a (more debuggable) implementation of:
			//		for (int index = 0; index < Count; ++index) yield return this[index];
			// used in public IEnumerator<DataRefBase<T>> GetEnumerator() {...}
			// therewith is possible to use perfomance analysing for the enumerator

			private VirtualListBase<T> m_List;
			private int m_Current;

			public VirtualEnumerator(VirtualListBase<T> list) {
				m_List = list;
				m_Current = -1;
			}

			public void Dispose() { m_List = null; }

			public bool MoveNext() {
				m_Current++;
				return m_Current < m_List.Count;
			}

			public void Reset() { m_Current = -1; }

			public DataRefBase<T> Current { get { return m_List[m_Current]; } }

			object IEnumerator.Current { get { return Current; } }
		}

		protected class CachedDataRef : DataRefBase<T> {

			public readonly VirtualListBase<T> List;
			public readonly int Index;

			public CachedDataRef(int index, VirtualListBase<T> list) {
				Index = index;
				List = list;
			}

			public override T Data { get { return List.LoadData(Index); } }

			public override int GetHashCode() { return Index ^ List.GetHashCode(); }

			public override bool Equals(object obj) {
				var other = obj as CachedDataRef;
				return other != null && Index == other.Index && List == other.List;
			}

		}

		private class CacheBlock {

			//duplicate use of index
			//index = the index of record
			//index = the internal index / position within the cache block

			/// <summary> The start index (the first index stored in this cache block)
			/// </summary>
			public int StartIndex=-1;

			/// <summary> The end index (the last index stored in this cache block)
			/// </summary>
			public int EndIndex=-1;

			/// <summary> The data
			/// </summary>
			public readonly T[] Data;

			/// <summary> Initializes a new instance of the <see cref="CacheBlock"/> with the specified <paramref name="length"/>.
			/// </summary>
			/// <param name="length">The length of this cahe block.</param>
			public CacheBlock(int length) { Data = new T[length]; }

			/// <summary> Determines whether <paramref name="recordNo"/> is included in this cache block and returns also the <paramref name="index" /> within.
			/// </summary>
			/// <param name="recordNo">The record no.</param>
			/// <param name="index">[OUT] The index within this cacheblock or -1.</param>
			/// <returns><c>true</c> if the specified <paramref name="recordNo"/>; otherwise, <c>false</c>.</returns>
			public bool Contains(int recordNo, out int index) {
				if (StartIndex <= recordNo && recordNo <= EndIndex) {
					index = recordNo - StartIndex;
					return true;
				} else {
					index = -1;
					return false;
				}
			}

			/// <summary> Gets the index in this cache block
			/// </summary>
			/// <param name="recordNo">The record no.</param>
			/// <returns>The index in this cahe block or -1</returns>
			public int IndexOf(int recordNo) { return (StartIndex <= recordNo && recordNo <= EndIndex) ? (recordNo - StartIndex) : -1; }

			/// <summary> Clears this cache block.
			/// </summary>
			public void Clear() {
				StartIndex = EndIndex = -1;
				for (int i = 0; i < Data.Length; ++i) Data[i] = null;
			}

		}

		public sealed class CacheImpl {

			// statistics
			private int m_CacheRequest;
			private int m_CacheMisses;

			private readonly LinkedList<CacheBlock> m_CacheBlock;
			internal readonly int NumCacheBlocks;
			internal readonly int NumItemsPerCacheBlock;

			private int m_Count;
			private Func<int,T[], int> m_LoadCallback;
			private VirtualListBase<T> m_List; /*DEPRECATED*/

			public CacheImpl(Func<int,T[], int> loadCallback, int numCacheBlocks, int cacheBlockLength, VirtualListBase<T> list /*DEPRECATED*/) {
				m_LoadCallback = loadCallback;
				m_Count               = UninitializedCount;
				NumCacheBlocks        = numCacheBlocks;
				NumItemsPerCacheBlock = cacheBlockLength;
				m_CacheBlock          = new LinkedList<CacheBlock>();
				m_List                = list; /*DEPRECATED*/
			}

			public int Count {
				get {
					// if the count hasn't been determind yet, we need to access at least one remote item to get it
					if (m_Count == UndefinedCount || m_Count == UninitializedCount) LoadData(0);
					return m_Count;
				}
			}

			public void Clear() {
				m_Count=UninitializedCount;
				m_CacheBlock.Clear();
			}

			private int InternalLoad(int startIndex, T[] data) { return m_LoadCallback(startIndex, data); }

			public T LoadData(int index) {
				//Debug.WriteLine(string.Format("LoadData {0}",index));
				++m_CacheRequest;
				T returnValue;
 
				var cacheBlockNode = m_CacheBlock.First;
				int indexInCacheBlock = -1;
				while (cacheBlockNode != null) {
					if (cacheBlockNode.Value.Contains(index, out indexInCacheBlock)) break;
					cacheBlockNode = cacheBlockNode.Next;
				}

				if (cacheBlockNode == null) {
					++m_CacheMisses;
					CacheBlock cacheBlock;
					if (m_CacheBlock.Count < NumCacheBlocks) {
						cacheBlockNode = new LinkedListNode<CacheBlock>(cacheBlock = new CacheBlock(NumItemsPerCacheBlock));
					} else {
						cacheBlockNode = m_CacheBlock.Last;
						m_CacheBlock.RemoveLast();
						cacheBlock = cacheBlockNode.Value;
					}
					indexInCacheBlock = index%cacheBlock.Data.Length;

					//request to fill the cacheblock
					cacheBlock.StartIndex = index - indexInCacheBlock;
					int count = InternalLoad(cacheBlock.StartIndex, cacheBlock.Data);
					cacheBlock.EndIndex = Math.Min(count, cacheBlock.StartIndex + cacheBlock.Data.Length) -1;
					if (count != m_Count /*collection has changed in the meantime*/) { 
						bool firstTime = m_Count == UninitializedCount;
						// update the count [HÄÄ??->] unless it was undefined
						m_Count = count;

						// signal that our collection has changed, if this is not the first time aroud
						// failure to check for this will give a nullreferenceexception on collectionchanged
						if (!firstTime) m_List.NotifyCollectionChanged(); /*DEPRECATED*/ //TODO remove list access
						
						// clear the cache: the only block left is the one we're holding
						m_CacheBlock.Clear();
					}
					m_CacheBlock.AddFirst(cacheBlockNode);

					// if the index is outside the bounds of the new count, return nothing
					if (indexInCacheBlock > cacheBlock.EndIndex) returnValue=null;
					else returnValue=cacheBlock.Data[indexInCacheBlock];
					 if (/*DEBUG*/ returnValue == null) { Debug.WriteLine("returning null!");} 
				} else {
					// move the block to the front of the cache if it's not already there
					if (cacheBlockNode != m_CacheBlock.First) {
						m_CacheBlock.Remove(cacheBlockNode);
						m_CacheBlock.AddFirst(cacheBlockNode);
					}
					returnValue= cacheBlockNode.Value.Data[indexInCacheBlock];
					if (/*DEBUG*/ returnValue == null) { Debug.WriteLine("returning null!");} 
				}
				if (/*DEBUG*/ returnValue == null) { Debug.WriteLine("returning null!");} 
				return returnValue;
			}

			private void AddItem(int newIndex, T item) {
//				Debug.WriteLine(string.Format("Cache.Add {0}",index));
				if(item==null){ Debug.WriteLine("returning null!");} 

				var lastIndex = newIndex - 1;

				var cacheBlockNode = m_CacheBlock.First;
				int indexInCacheBlock = -1;
				while (indexInCacheBlock==-1 && cacheBlockNode != null) {
					indexInCacheBlock = cacheBlockNode.Value.IndexOf(lastIndex);
					if (indexInCacheBlock>=0) break;
					cacheBlockNode = cacheBlockNode.Next;
				}

				if (cacheBlockNode == null /* index not in cache*/) {
					m_Count++;
//					Debug.WriteLine(string.Format("AddItem {0,7} : no matching CacheBlockNode => do nothing", newIndex));
					return; // nothing to do
				} else /* block found */ {
					// 0-0 = 1
					// 1-0 = 2
					var numEntries=cacheBlockNode.Value.EndIndex - cacheBlockNode.Value.StartIndex +1;
					if (numEntries == NumItemsPerCacheBlock /*block is full*/) {
//						Debug.WriteLine(string.Format("AddItem {0,7} : CacheBlockNode is full, need new one => do nothing", newIndex));
						// TODO we could create a new cache block
						m_Count++;
					} else {
//						Debug.WriteLine(string.Format("AddItem {0,7} : CacheBlockNode expanded", newIndex));
						cacheBlockNode.Value.EndIndex++;
						cacheBlockNode.Value.Data[indexInCacheBlock+1]=item;
						m_Count++;
					}
				}
			}

			public void ReplaceItem(int index, T item) {
				//Debug.WriteLine(string.Format("Cache.Replace {0}",index));

				var cacheBlockNode = m_CacheBlock.First;
				int indexInCacheBlock = -1;
 
				while (cacheBlockNode != null) {
					if (cacheBlockNode.Value.Contains(index, out indexInCacheBlock)) break;
					cacheBlockNode = cacheBlockNode.Next;
				}

				if (cacheBlockNode == null /* index not in cache*/) {
					return; // nothing to do
				} else /* block found */ {
					cacheBlockNode.Value.Data[indexInCacheBlock]=item;
				}
			}

			public void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e) {
				if (m_Count == UndefinedCount) {Debugger.Break(); /*should not occur*/ return;}

				// NOTE m_Count: from here we assume m_Count has a valid value

				switch (e.Action) {
					case NotifyCollectionChangedAction.Add:
						if (e.NewStartingIndex != -1 /*Insert*/ && e.NewStartingIndex!=m_Count) {
							throw new NotImplementedException("{6A835BE6-2823-4B91-AB4E-5CB3086DC01E}");
						}
						var newIndex = m_Count;
						foreach (T newItem in e.NewItems) AddItem(newIndex++,newItem);
						//PERFORMANCE if more as 1 new item, we could implement AddRange
						break;
					case NotifyCollectionChangedAction.Replace: /*Not optimized, same as Reset*/ 
//						for (int i = 0; i < e.NewItems.Count; i++) {
//							ReplaceItem(...);
//						}
					case NotifyCollectionChangedAction.Move   : /*Not optimized, same as Reset*/ 
					case NotifyCollectionChangedAction.Remove : /*Not optimized, same as Reset*/ 
					case NotifyCollectionChangedAction.Reset  :
						Clear();
						break;
				}
			}

		}
	}
}