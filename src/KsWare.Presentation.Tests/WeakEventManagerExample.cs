using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace KsWare.Presentation.WeakEventManagerExamples {

	public class MyEventProvider {
		
		private Lazy<EventSourceStore> LazyWeakEventProperties; // store for lazy event sources (also the store is lazy)

		public MyEventProvider() {
			LazyWeakEventProperties = new Lazy<EventSourceStore>(()=>new EventSourceStore(this));
		}
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler<EventArgs>> MyEvent { get { return LazyWeakEventProperties.Value.Get<EventHandler<EventArgs>>("MyEvent"); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler<EventArgs>> MyEvent2 { get { return LazyWeakEventProperties.Value.Get<EventHandler<EventArgs>>("MyEvent2"); } }
		
		private void OnMyEvent() {
			// if no one has registered an event handler the event source will not be creted.
			EventManager.Raise<EventHandler<EventArgs>,EventArgs>(LazyWeakEventProperties, "MyEvent", EventArgs.Empty);
		}
		
	}
		
	public class MyEventListener {
		private IEventHandle _myEventHolder;
		
		public MyEventListener(MyEventProvider provider) {
			_myEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
		}
		
		private void AtMyEvent(object sender, EventArgs eventArgs) { /*...*/ }
	}

	public class MyEventListener2 {
		private List<IEventHandle> _myWeakEventHolder=new List<IEventHandle>();
		
		public MyEventListener2(MyEventProvider provider,MyEventProvider provider2) {
			_myWeakEventHolder.Add(provider.MyEvent.RegisterWeak(AtMyEvent));
			_myWeakEventHolder.Add(provider.MyEvent2.RegisterWeak(AtMyEvent));
			_myWeakEventHolder.Add(provider2.MyEvent.RegisterWeak(AtMyEvent));
			_myWeakEventHolder.Add(provider2.MyEvent2.RegisterWeak(AtMyEvent));
		}
		
		private void AtMyEvent(object sender, EventArgs eventArgs) { /*...*/ }
	}
}
