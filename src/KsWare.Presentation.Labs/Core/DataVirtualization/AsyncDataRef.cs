using System;
using System.Threading;

namespace KsWare.Presentation.DataVirtualization {

	public class AsyncDataRef<TId, T> : DataRefBase<T> where T : class {

		private readonly TId _id;
		private int _loading;
		private readonly Func<TId, T> _loadFunc;
		private volatile T _data;

		public AsyncDataRef(TId id, Func<TId, T> loadFunc) {
			_id = id;
			_loadFunc = loadFunc;
		}

		public override T Data {
			get {
				if (_data != null) return _data;
				if (Interlocked.Increment(ref _loading) == 1)
					if (_data == null) _loadFunc.BeginInvoke(_id, AsyncLoadCallback, null);
					else Interlocked.Decrement(ref _loading);
				else Interlocked.Decrement(ref _loading);
				return _data;
			}
		}


		private void AsyncLoadCallback(IAsyncResult ar) {
			_data = _loadFunc.EndInvoke(ar);
			Interlocked.Decrement(ref _loading);
			// when the object is loaded, signal that all the properties have changed
			NotifyAllPropertiesChanged();
		}

	}

}