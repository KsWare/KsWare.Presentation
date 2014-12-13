using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert=NUnit.Framework.Assert;
using Is=NUnit.Framework.Is;
using IgnoreAttribute=Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute;

namespace KsWare.Presentation.Tests.Core {

	[TestClass]
	public class EventManagerˑIntegrationTests {

		[TestMethod]
		public void BasicˑTest() {
			var provider = new MyEventProvider();
			var listener = new MyEvenListener();

			listener.RegisterˑMyEvent(provider);
			provider.RaiseˑMyEvent();
			Assert.That(listener.EventCount,Is.EqualTo(1));

			listener.ReleaseˑMyEvent();
			provider.RaiseˑMyEvent();
			Assert.That(listener.EventCount,Is.EqualTo(1));
		}

		[TestMethod]
		public void GarbageCollectionˑTest() {
			var provider = new MyEventProvider();
			var listener = new MyEvenListener();

			listener.RegisterˑMyEvent(provider);
			provider.RaiseˑMyEvent();
			Assert.That(listener.EventCount,Is.EqualTo(1));

			var c = EventHandle.StatisticsːInstancesˑCurrent;
			listener = null;
			ForceGarbageCollection();
			provider.RaiseˑMyEvent();
			Assert.That(EventHandle.StatisticsːInstancesˑCurrent,Is.EqualTo(c-1));
		}

		private void ForceGarbageCollection() {
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		public class MyEventProvider {

			protected Lazy<EventSourceStore> LazyEventSources;

			public MyEventProvider() {
				LazyEventSources = new Lazy<EventSourceStore>(() => new EventSourceStore(this));
			}

			protected EventSourceStore EventSources { get { return LazyEventSources.Value; }}


			public IEventSource<EventHandler> MyEvent { get { return EventSources.Get<EventHandler>("MyEvent"); } }

			public void RaiseˑMyEvent() {
				EventManager.Raise<EventHandler,EventArgs>(LazyEventSources,"MyEvent",EventArgs.Empty);
			}
			
		}

		public class MyEvenListener {

			private List<IEventHandle> m_EventHandles=new List<IEventHandle>();
			private IEventHandle m_MyEventHandle;
			
				public int EventCount;

			public void RegisterˑMyEvent(MyEventProvider listener) {
				m_MyEventHandle=listener.MyEvent.RegisterWeak(AtMyEvent);
			}

			private void AtMyEvent(object sender, EventArgs eventArgs) { EventCount++; }

			public void ReleaseˑMyEvent() {
				m_MyEventHandle.Release();
			}

		}


	}

}
