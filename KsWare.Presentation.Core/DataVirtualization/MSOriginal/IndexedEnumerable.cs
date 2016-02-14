// Type: MS.Internal.Data.IndexedEnumerable
// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0F086D49-4CAD-43AD-A3E2-A5268BD16302
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationFramework.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime;
using System.Windows;
using System.Windows.Data;
using MS.Internal;

namespace KsWare.Presentation.DataVirtualization.MSOriginal {

	internal class IndexedEnumerableOriginal : IEnumerable, IWeakEventListener {

		private int _cachedIndex = -1;
		private int _cachedVersion = -1;
		private int _cachedCount = -1;
		private IEnumerable _enumerable;
		private IEnumerator _enumerator;
		private IEnumerator _changeTracker;
		private ICollection _collection;
		private IList _list;
		private CollectionView _collectionView;
		private int _enumeratorVersion;
		private object _cachedItem;
		private bool? _cachedIsEmpty;
		private PropertyInfo _reflectedCount;
		private PropertyInfo _reflectedItemAt;
		private MethodInfo _reflectedIndexOf;
		private Predicate<object> _filterCallback;

		internal int Count {
			get {
				this.EnsureCacheCurrent();
				int num1 = 0;
				if (this.GetNativeCount(out num1)) return num1;
				if (this._cachedCount >= 0) return this._cachedCount;
				int num2 = 0;
				foreach (object obj in this) ++num2;
				this._cachedCount = num2;
				this._cachedIsEmpty = new bool?(this._cachedCount == 0);
				return num2;
			}
		}

		internal bool IsEmpty {
			get {
				bool isEmpty;
				if (this.GetNativeIsEmpty(out isEmpty)) return isEmpty;
				if (this._cachedIsEmpty.HasValue) return this._cachedIsEmpty.Value;
				IEnumerator enumerator = this.GetEnumerator();
				this._cachedIsEmpty = new bool?(!enumerator.MoveNext());
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null) disposable.Dispose();
				if (this._cachedIsEmpty.Value) this._cachedCount = 0;
				return this._cachedIsEmpty.Value;
			}
		}

		internal object this[int index] {
			get {
				object obj;
				if (this.GetNativeItemAt(index, out obj)) return obj;
				if (index < 0) throw new ArgumentOutOfRangeException("index");
				int num = index - this._cachedIndex;
				if (num < 0) {
					this.UseNewEnumerator();
					num = index + 1;
				}
				if (this.EnsureCacheCurrent()) { if (index == this._cachedIndex) return this._cachedItem; } else num = index + 1;
				while (num > 0 && this._enumerator.MoveNext()) --num;
				if (num != 0) throw new ArgumentOutOfRangeException("index");
				this.CacheCurrentItem(index, this._enumerator.Current);
				return this._cachedItem;
			}
		}

		internal IEnumerable Enumerable {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._enumerable; }
		}

		internal ICollection Collection {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._collection; }
		}

		internal IList List {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._list; }
		}

		internal CollectionView CollectionView {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._collectionView; }
		}

		private Predicate<object> FilterCallback { get { return this._filterCallback; } }

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		internal IndexedEnumerableOriginal(IEnumerable collection)
			: this(collection, (Predicate<object>) null) { }

		internal IndexedEnumerableOriginal(IEnumerable collection, Predicate<object> filterCallback) {
			this._filterCallback = filterCallback;
			this.SetCollection(collection);
			if (this.List != null) return;
			INotifyCollectionChanged source = collection as INotifyCollectionChanged;
			if (source == null) return;
			CollectionChangedEventManager.AddHandler(source, new EventHandler<NotifyCollectionChangedEventArgs>(this.OnCollectionChanged));
		}

		internal int IndexOf(object item) {
			int num1;
			if (this.GetNativeIndexOf(item, out num1)) return num1;
			if (this.EnsureCacheCurrent() && item == this._cachedItem) return this._cachedIndex;
			int index = -1;
			if (this._cachedIndex >= 0) this.UseNewEnumerator();
			int num2 = 0;
			while (this._enumerator.MoveNext()) {
				if (object.Equals(this._enumerator.Current, item)) {
					index = num2;
					break;
				} else ++num2;
			}
			if (index >= 0) { this.CacheCurrentItem(index, this._enumerator.Current); } else {
				this.ClearAllCaches();
				this.DisposeEnumerator(ref this._enumerator);
			}
			return index;
		}

		public IEnumerator GetEnumerator() { return (IEnumerator) new IndexedEnumerableOriginal.FilteredEnumerator(this, this.Enumerable, this.FilterCallback); }

		internal static void CopyTo(IEnumerable collection, Array array, int index) {
			Invariant.Assert(collection != null, "collection is null");
			Invariant.Assert(array != null, "target array is null");
			Invariant.Assert(array.Rank == 1, "expected array of rank=1");
			Invariant.Assert(index >= 0, "index must be positive");
			ICollection collection1 = collection as ICollection;
			if (collection1 != null) { collection1.CopyTo(array, index); } else {
				IList list = (IList) array;
				foreach (object obj in collection) {
					if (index >= array.Length) throw new ArgumentException(System.Windows.SR.Get("CopyToNotEnoughSpace"), "index");
					list[index] = obj;
					++index;
				}
			}
		}

		internal void Invalidate() {
			this.ClearAllCaches();
			if (this.List == null) {
				INotifyCollectionChanged source = this.Enumerable as INotifyCollectionChanged;
				if (source != null) CollectionChangedEventManager.RemoveHandler(source, new EventHandler<NotifyCollectionChangedEventArgs>(this.OnCollectionChanged));
			}
			this._enumerable = (IEnumerable) null;
			this.DisposeEnumerator(ref this._enumerator);
			this.DisposeEnumerator(ref this._changeTracker);
			this._collection = (ICollection) null;
			this._list = (IList) null;
			this._filterCallback = (Predicate<object>) null;
		}

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) { return this.ReceiveWeakEvent(managerType, sender, e); }

		protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) { return false; }

		private void CacheCurrentItem(int index, object item) {
			this._cachedIndex = index;
			this._cachedItem = item;
			this._cachedVersion = this._enumeratorVersion;
		}

		private bool EnsureCacheCurrent() {
			int num = this.EnsureEnumerator();
			if (num != this._cachedVersion) {
				this.ClearAllCaches();
				this._cachedVersion = num;
			}
			return num == this._cachedVersion && this._cachedIndex >= 0;
		}

		private int EnsureEnumerator() {
			if (this._enumerator == null) { this.UseNewEnumerator(); } else {
				try { this._changeTracker.MoveNext(); }
				catch (InvalidOperationException ex) {
					this.UseNewEnumerator();
				}
			}
			return this._enumeratorVersion;
		}

		private void UseNewEnumerator() {
			++this._enumeratorVersion;
			this.DisposeEnumerator(ref this._changeTracker);
			this._changeTracker = this._enumerable.GetEnumerator();
			this.DisposeEnumerator(ref this._enumerator);
			this._enumerator = this.GetEnumerator();
			this._cachedIndex = -1;
			this._cachedItem = (object) null;
		}

		private void InvalidateEnumerator() {
			++this._enumeratorVersion;
			this.DisposeEnumerator(ref this._enumerator);
			this.ClearAllCaches();
		}

		private void DisposeEnumerator(ref IEnumerator ie) {
			IDisposable disposable = ie as IDisposable;
			if (disposable != null) disposable.Dispose();
			ie = (IEnumerator) null;
		}

		private void ClearAllCaches() {
			this._cachedItem = (object) null;
			this._cachedIndex = -1;
			this._cachedCount = -1;
		}

		private void SetCollection(IEnumerable collection) {
			Invariant.Assert(collection != null);
			this._enumerable = collection;
			this._collection = collection as ICollection;
			this._list = collection as IList;
			this._collectionView = collection as CollectionView;
			if (this.List != null || this.CollectionView != null) return;
			Type type = collection.GetType();
			MethodInfo method = type.GetMethod("IndexOf", new Type[1] {
				typeof (object)
			});
			if (method != (MethodInfo) null && method.ReturnType == typeof (int)) this._reflectedIndexOf = method;
			MemberInfo[] defaultMembers = type.GetDefaultMembers();
			for (int index = 0; index <= defaultMembers.Length - 1; ++index) {
				PropertyInfo propertyInfo = defaultMembers[index] as PropertyInfo;
				if (propertyInfo != (PropertyInfo) null) {
					ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
					if (indexParameters.Length == 1 && indexParameters[0].ParameterType.IsAssignableFrom(typeof (int))) {
						this._reflectedItemAt = propertyInfo;
						break;
					}
				}
			}
			if (this.Collection != null) return;
			PropertyInfo property = type.GetProperty("Count", typeof (int));
			if (!(property != (PropertyInfo) null)) return;
			this._reflectedCount = property;
		}

		private bool GetNativeCount(out int value) {
			bool flag = false;
			value = -1;
			if (this.Collection != null) {
				value = this.Collection.Count;
				flag = true;
			} else if (this.CollectionView != null) {
				value = this.CollectionView.Count;
				flag = true;
			} else if (this._reflectedCount != (PropertyInfo) null) {
				try {
					value = (int) this._reflectedCount.GetValue((object) this.Enumerable, (object[]) null);
					flag = true;
				}
				catch (MethodAccessException ex) {
					this._reflectedCount = (PropertyInfo) null;
					flag = false;
				}
			}
			return flag;
		}

		private bool GetNativeIsEmpty(out bool isEmpty) {
			bool flag = false;
			isEmpty = true;
			if (this.Collection != null) {
				isEmpty = this.Collection.Count == 0;
				flag = true;
			} else if (this.CollectionView != null) {
				isEmpty = this.CollectionView.IsEmpty;
				flag = true;
			} else if (this._reflectedCount != (PropertyInfo) null) {
				try {
					isEmpty = (int) this._reflectedCount.GetValue((object) this.Enumerable, (object[]) null) == 0;
					flag = true;
				}
				catch (MethodAccessException ex) {
					this._reflectedCount = (PropertyInfo) null;
					flag = false;
				}
			}
			return flag;
		}

		private bool GetNativeIndexOf(object item, out int value) {
			bool flag = false;
			value = -1;
			if (this.List != null && this.FilterCallback == null) {
				value = this.List.IndexOf(item);
				flag = true;
			} else if (this.CollectionView != null) {
				value = this.CollectionView.IndexOf(item);
				flag = true;
			} else if (this._reflectedIndexOf != (MethodInfo) null) {
				try {
					value = (int) this._reflectedIndexOf.Invoke((object) this.Enumerable, new object[1] {
						item
					});
					flag = true;
				}
				catch (MethodAccessException ex) {
					this._reflectedIndexOf = (MethodInfo) null;
					flag = false;
				}
			}
			return flag;
		}

		private bool GetNativeItemAt(int index, out object value) {
			bool flag = false;
			value = (object) null;
			if (this.List != null) {
				value = this.List[index];
				flag = true;
			} else if (this.CollectionView != null) {
				value = this.CollectionView.GetItemAt(index);
				flag = true;
			} else if (this._reflectedItemAt != (PropertyInfo) null) {
				try {
					value = this._reflectedItemAt.GetValue((object) this.Enumerable, new object[1] {
						(object) index
					});
					flag = true;
				}
				catch (MethodAccessException ex) {
					this._reflectedItemAt = (PropertyInfo) null;
					flag = false;
				}
			}
			return flag;
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { this.InvalidateEnumerator(); }

		private class FilteredEnumerator : IEnumerator, IDisposable {

			private IEnumerable _enumerable;
			private IEnumerator _enumerator;
			private IndexedEnumerableOriginal _indexedEnumerable;
			private Predicate<object> _filterCallback;

			object IEnumerator.Current { get { return this._enumerator.Current; } }

			public FilteredEnumerator(IndexedEnumerableOriginal indexedEnumerable, IEnumerable enumerable, Predicate<object> filterCallback) {
				this._enumerable = enumerable;
				this._enumerator = this._enumerable.GetEnumerator();
				this._filterCallback = filterCallback;
				this._indexedEnumerable = indexedEnumerable;
			}

			void IEnumerator.Reset() {
				if (this._indexedEnumerable._enumerable == null) throw new InvalidOperationException(System.Windows.SR.Get("EnumeratorVersionChanged"));
				this.Dispose();
				this._enumerator = this._enumerable.GetEnumerator();
			}

			bool IEnumerator.MoveNext() {
				if (this._indexedEnumerable._enumerable == null) throw new InvalidOperationException(System.Windows.SR.Get("EnumeratorVersionChanged"));
				bool flag;
				if (this._filterCallback == null) { flag = this._enumerator.MoveNext(); } else { while ((flag = this._enumerator.MoveNext()) && !this._filterCallback(this._enumerator.Current)) ; }
				return flag;
			}

			public void Dispose() {
				IDisposable disposable = this._enumerator as IDisposable;
				if (disposable != null) disposable.Dispose();
				this._enumerator = (IEnumerator) null;
			}

		}

	}

}