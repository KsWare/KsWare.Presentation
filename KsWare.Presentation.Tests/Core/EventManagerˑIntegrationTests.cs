using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
			var listener = new MyEventListener();

			listener.RegisterˑMyEvent(provider);
			provider.RaiseˑMyEvent();
			Assert.That(listener.EventCount,Is.EqualTo(1));

			listener.ReleaseˑMyEvent();
			provider.RaiseˑMyEvent();
			Assert.That(listener.EventCount,Is.EqualTo(1));
		}

		[TestMethod]
		public void ListenerGarbageCollectionˑTest() {
			var provider = new MyEventProvider();
			var listener = new MyEventListener();

			listener.RegisterˑMyEvent(provider);
			provider.RaiseˑMyEvent();
			Assert.That(listener.EventCount,Is.EqualTo(1));

			var c = EventHandle.StatisticsːInstancesˑCurrent;
			listener = null;
			ForceGarbageCollection();
			provider.RaiseˑMyEvent();
			Assert.That(((IEventSourceInternal) provider.MyEvent).GetContainers().Count,Is.EqualTo(0));
		}

		[TestMethod,Ignore /* does not work in unit test runner */]
		public void MassiveListenerGCTest() {
			var provider = new MyEventProvider();
			var eventContainers = ((EventSource) provider.MyEvent).GetContainers();
			var r = new System.Random();
			var stopwatch = new Stopwatch(); stopwatch.Start();
			const int max = 2000;
			var c = eventContainers.Count;
			for (int i = 1; i <= max; i++) {
				var listener = new MyEventListener();
				listener.RegisterˑMyEvent(provider);
				provider.RaiseˑMyEvent();
				Thread.Sleep(r.Next(8,12+1));
				if (eventContainers.Count+c < i) {
					//OK any listener has been collected
					Trace.WriteLine("GC collected at interation "+i+" "+ stopwatch.Elapsed + " "+eventContainers.Count +" alive");
					c=i - eventContainers.Count;
				}
			}
			Trace.WriteLine(eventContainers.Count*100/max+"% alive after "+ stopwatch.Elapsed);
			var next=stopwatch.Elapsed.Add(TimeSpan.FromSeconds(1));
			while (eventContainers.Count>0) {
				if(stopwatch.Elapsed<next){Thread.Sleep(10); provider.RaiseˑMyEvent(); continue;}
				Trace.WriteLine(eventContainers.Count*100/max+"% alive after "+ stopwatch.Elapsed);
				next=stopwatch.Elapsed.Add(TimeSpan.FromSeconds(1));
			}
			
			//Assert.Fail("Nothing collected!");
		}

		[TestMethod]
		public void ProviderGarbageCollectionˑTest() {
			var provider = new MyEventProvider();
			var listener = new MyEventListener();

			listener.RegisterˑMyEvent(provider);
			provider.RaiseˑMyEvent();
			Assert.That(listener.EventCount,Is.EqualTo(1));

			var c = EventHandle.StatisticsːInstancesˑCurrent;
			provider = null;
			ForceGarbageCollection();
			Assert.That(((EventHandle) listener.m_MyEventHandle).IsAlive,Is.EqualTo(false));
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

		public class MyEventListener {

			internal List<IEventHandle> m_EventHandles=new List<IEventHandle>();
			internal IEventHandle m_MyEventHandle;
			
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
