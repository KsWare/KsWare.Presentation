using System;
using System.Diagnostics;


namespace KsWare.Presentation.ViewModelFramework {

	partial class ObjectVM /*:  */ {

		protected Lazy<EventSourceStore> LazyWeakEventStore;

		private void InitPartWeakEvents() {
			LazyWeakEventStore = new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected EventSourceStore EventSources { get { return LazyWeakEventStore.Value; }}
	}

}
