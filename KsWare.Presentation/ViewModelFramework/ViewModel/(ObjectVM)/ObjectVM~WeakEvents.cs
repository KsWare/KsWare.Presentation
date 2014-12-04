using System;

namespace KsWare.Presentation.ViewModelFramework {

	partial class ObjectVM /*:  */ {

		protected Lazy<WeakEventPropertyStore> LazyWeakEventProperties;

		private void InitPartWeakEvents() {
			LazyWeakEventProperties = new Lazy<WeakEventPropertyStore>(() => new WeakEventPropertyStore(this));
		}

		protected WeakEventPropertyStore WeakEventProperties { get { return LazyWeakEventProperties.Value; }}
	}

}
