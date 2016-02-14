using System;

namespace KsWare.Presentation.ViewModelFramework {

	partial class ObjectVM /*:  */ {

		protected Lazy<EventSourceStore> LazyWeakEventStore;

		private void InitPartWeakEvents() {
			LazyWeakEventStore = new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		protected EventSourceStore EventSources { get { return LazyWeakEventStore.Value; }}
	}

}
