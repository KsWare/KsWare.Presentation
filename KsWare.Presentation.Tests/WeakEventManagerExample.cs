﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KsWare.Presentation;

namespace KsWare.Presentation.WeakEventManagerExamples {

	public class MyEventProvider {
		
		private Lazy<EventSourceStore> LazyWeakEventProperties; // store for lazy event sources (also the store is lazy)

		public MyEventProvider() {
			LazyWeakEventProperties = new Lazy<EventSourceStore>(()=>new EventSourceStore(this));
		}
		
		public IEventSource<EventHandler<EventArgs>> MyEvent { get { return LazyWeakEventProperties.Value.Get<EventHandler<EventArgs>>("MyEvent"); } }
		public IEventSource<EventHandler<EventArgs>> MyEvent2 { get { return LazyWeakEventProperties.Value.Get<EventHandler<EventArgs>>("MyEvent2"); } }
		
		private void OnMyEvent() {
			// if no one has registered an event handler the event source will not be creted.
			EventManager.Raise<EventHandler<EventArgs>,EventArgs>(LazyWeakEventProperties, "MyEvent", EventArgs.Empty);
		}
		
	}
		
	public class MyEventListener {
		private IEventHandle m_MyEventHolder;
		
		public MyEventListener(MyEventProvider provider) {
			m_MyEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
		}
		
		private void AtMyEvent(object sender, EventArgs eventArgs) { /*...*/ }
	}

	public class MyEventListener2 {
		private List<IEventHandle> m_MyWeakEventHolder=new List<IEventHandle>();
		
		public MyEventListener2(MyEventProvider provider,MyEventProvider provider2) {
			m_MyWeakEventHolder.Add(provider.MyEvent.RegisterWeak(AtMyEvent));
			m_MyWeakEventHolder.Add(provider.MyEvent2.RegisterWeak(AtMyEvent));
			m_MyWeakEventHolder.Add(provider2.MyEvent.RegisterWeak(AtMyEvent));
			m_MyWeakEventHolder.Add(provider2.MyEvent2.RegisterWeak(AtMyEvent));
		}
		
		private void AtMyEvent(object sender, EventArgs eventArgs) { /*...*/ }
	}
}
