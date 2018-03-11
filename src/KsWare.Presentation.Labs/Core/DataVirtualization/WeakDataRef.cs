using System;

namespace KsWare.Presentation.DataVirtualization {

	public class WeakDataRef<TId, T> : DataRefBase<T> where T : class {

		private readonly TId _Id;
		private readonly WeakReference _data = new WeakReference(null);
		private readonly Func<TId, T> Load;

		public WeakDataRef(TId id, Func<TId, T> load) {
			_Id = id;
			Load = load;
		}


		public override T Data {
			get {
				var data = (T) _data.Target;
				if (data == null) _data.Target = data = Load(_Id);
				return data;
			}
		}

	}

}
