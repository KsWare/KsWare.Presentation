using System;

namespace KsWare.Presentation {


	partial class BackingFieldsStore {
		public interface IValueFactory {
			object CreateInstance(Type type, BackingFieldsStore store);
		}
	}
}
