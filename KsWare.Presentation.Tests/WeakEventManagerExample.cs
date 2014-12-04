using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KsWare.Presentation;

namespace KsWare.Presentation.WeakEventManagerExamples {

	public class MyEventProvider {
		
		private Lazy<WeakEventPropertyStore> LazyWeakEventProperties; // store for lazy event sources (also the store is lazy)

		public MyEventProvider() {
			LazyWeakEventProperties = new Lazy<WeakEventPropertyStore>(()=>new WeakEventPropertyStore(this));
		}
		
		public IWeakEventSource<EventHandler<EventArgs>> MyEvent { get { return LazyWeakEventProperties.Value.Get<EventHandler<EventArgs>>("MyEvent"); } }
		public IWeakEventSource<EventHandler<EventArgs>> MyEvent2 { get { return LazyWeakEventProperties.Value.Get<EventHandler<EventArgs>>("MyEvent2"); } }
		
		private void OnMyEvent() {
			// if no one has registered an event handler the event source will not be creted.
			EventUtil.WeakEventManager.Raise<EventHandler<EventArgs>>(LazyWeakEventProperties, "MyEvent", EventArgs.Empty);
		}
		
	}
		
	public class MyEventListener {
		private IWeakEventHandle m_MyWeakEventHolder;
		
		public MyEventListener(MyEventProvider provider) {
			m_MyWeakEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
		}
		
		private void AtMyEvent(object sender, EventArgs eventArgs) { /*...*/ }
	}

	public class MyEventListener2 {
		private List<IWeakEventHandle> m_MyWeakEventHolder=new List<IWeakEventHandle>();
		
		public MyEventListener2(MyEventProvider provider,MyEventProvider provider2) {
			m_MyWeakEventHolder.Add(provider.MyEvent.RegisterWeak(AtMyEvent));
			m_MyWeakEventHolder.Add(provider.MyEvent2.RegisterWeak(AtMyEvent));
			m_MyWeakEventHolder.Add(provider2.MyEvent.RegisterWeak(AtMyEvent));
			m_MyWeakEventHolder.Add(provider2.MyEvent2.RegisterWeak(AtMyEvent));
		}
		
		private void AtMyEvent(object sender, EventArgs eventArgs) { /*...*/ }
	}
}
