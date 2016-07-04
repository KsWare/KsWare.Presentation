using System;

namespace KsWare.Presentation.DataVirtualization {

	public class DataRef<TId, T> : DataRefBase<T> where T : class {

		private readonly TId _id;
		private T _data;

		private readonly Func<TId, T> Load;

		public DataRef(TId id, Func<TId, T> load) {
			_id = id;
			Load = load;
		}

		public override T Data {
			get {
				if (_data == null) _data = Load(_id);
				return _data;
			}
		}

	}

}
