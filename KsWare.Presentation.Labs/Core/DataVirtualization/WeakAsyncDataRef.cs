using System;
using System.Threading;

namespace KsWare.Presentation.DataVirtualization {

	public class WeakAsyncDataRef<TId, TData> : DataRefBase<TData> where TData : class {

		private readonly TId _id;
		private int _loading;
		private readonly Func<TId, TData> Load;
		private readonly WeakReference _data = new WeakReference(null);

		public WeakAsyncDataRef(TId id, Func<TId, TData> load) {
			_id = id;
			Load = load;
		}

		public override TData Data {
			get {
				TData data = (TData) _data.Target;
				if (data != null) return data;
				if (Interlocked.Increment(ref _loading) == 1) {
					data = (TData) _data.Target;
					if (data != null) {
						Interlocked.Decrement(ref _loading);
						return data;
					}
					Load.BeginInvoke(_id, AsyncLoadCallback, null);
				} else Interlocked.Decrement(ref _loading);
				return data;
			}
		}


		private void AsyncLoadCallback(IAsyncResult ar) {
			_data.Target = Load.EndInvoke(ar);
			Interlocked.Decrement(ref _loading);
			NotifyAllPropertiesChanged();
		}

	}

}