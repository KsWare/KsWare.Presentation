﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using Assert=NUnit.Framework.Assert;
using Is=NUnit.Framework.Is;

namespace KsWare.Presentation.Tests.Core {

	[TestFixture]
	public class EventManagerˑIntegrationTests {

		[Test]
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

		[Test][Ignore("ForceGarbageCollection does not have a (direct) effect.")]
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

		[Test,Ignore("does not work in unit test runner")]
		public void MassiveListenerGCTest() {
			var provider = new MyEventProvider();
			var eventContainers = ((EventSource) provider.MyEvent).GetContainers();
			var r = new System.Random();
			var stopwatch = new Stopwatch(); stopwatch.Start();
			const int max = 5000;
			var c = eventContainers.Count;
			for (int i = 1; i <= max; i++) {
				var listener = new MyEventListener();
				listener.RegisterˑMyEvent(provider);
				provider.RaiseˑMyEvent();
				Thread.Sleep(r.Next(8,12+1));
				if (eventContainers.Count+c < i) {
					//OK any listener has been collected
					Trace.WriteLine("GC collected at iteration "+i+" "+ stopwatch.Elapsed + " "+eventContainers.Count +" alive");
					c=i - eventContainers.Count;
				}
				listener.Dispose();
				listener = null;
			}
			Trace.WriteLine(eventContainers.Count*100/max+"% alive after "+ stopwatch.Elapsed);
			var next=stopwatch.Elapsed.Add(TimeSpan.FromSeconds(1));
			while (eventContainers.Count>0) {
				if (stopwatch.Elapsed < next) {
					Thread.Sleep(100); 
					//ForceGarbageCollection();
					provider.RaiseˑMyEvent(); continue;
				}
				Trace.WriteLine(eventContainers.Count*100/max+"% alive after "+ stopwatch.Elapsed);
				next=stopwatch.Elapsed.Add(TimeSpan.FromSeconds(1));
			}
			
			//Assert.Fail("Nothing collected!");
		}

		[Test,Ignore("TODO")]
		public void ProviderGarbageCollectionˑTest() {
			var provider = new MyEventProvider();
			var listener = new MyEventListener();

			listener.RegisterˑMyEvent(provider);
			provider.RaiseˑMyEvent();
			Assert.That(listener.EventCount,Is.EqualTo(1));

			var c = EventHandle.StatisticsːInstancesˑCurrent;
			provider = null;
			ForceGarbageCollection();
			Assert.That(((EventHandle) listener._MyEventHandle).IsAlive,Is.EqualTo(false));
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

			protected EventSourceStore EventSources => LazyEventSources.Value;


			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public IEventSource<EventHandler> MyEvent => EventSources.Get<EventHandler>("MyEvent");

			public void RaiseˑMyEvent() {
				EventManager.Raise<EventHandler,EventArgs>(LazyEventSources,"MyEvent",EventArgs.Empty);
			}
			
		}

		public class MyEventListener {

			internal List<IEventHandle> _EventHandles=new List<IEventHandle>();
			internal IEventHandle _MyEventHandle;
			
				public int EventCount;

			public void RegisterˑMyEvent(MyEventProvider listener) {
				_MyEventHandle=listener.MyEvent.RegisterWeak(this,"AtMyEvent",AtMyEvent);
			}

			private void AtMyEvent(object sender, EventArgs eventArgs) { EventCount++; }

			public void ReleaseˑMyEvent() {
				_MyEventHandle.Release();
			}

			public void Dispose() {
				if(_MyEventHandle!=null) _MyEventHandle.Dispose();
				_MyEventHandle = null;
				if (_EventHandles != null) {
					foreach (var eventHandle in _EventHandles) eventHandle.Dispose();
					_EventHandles.Clear();
				}
				_EventHandles = null;
			}

		}


	}

}
