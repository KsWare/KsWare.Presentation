#if false
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using KsWare.Presentation.BusinessFramework;
using NUnit.Framework;
using Assert=NUnit.Framework.Assert;
using Is=NUnit.Framework.Is;

namespace KsWare.Presentation.Tests.Core {

	[TestFixture,NUnit.Framework.Ignore("TODO")]
	public class WeakEventManagerTests {

		[SetUp]
		public void Initialize() {
			EventManager.Reset();
		}

		private static void ShowAssemblyPublicKey() {
			var ba = Assembly.GetExecutingAssembly().GetName(false).GetPublicKey();
			var hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba) hex.AppendFormat("{0:x2}", b);
			Trace.WriteLine("PublicKey: "+ hex);
		}


		[Test]
		public void Reset() {
			var testSubject = new PrivateType(typeof (EventManager));
			// done in Initialize():> WeakEventManager.Reset();
			Assert.That(EventManager.Count,Is.EqualTo(0));
			Assert.That((DateTime)testSubject.GetStaticField("s_LastCollect"),Is.EqualTo(DateTime.MinValue));
			Assert.That(EventManager.StatisticsːRaiseːInvocationCount,Is.EqualTo(0));
		}


		[Test]
		public void GCCollectTest() {
			var provider = new MyEventProvider();
			var listener = new MyEventListener();

			listener.Listen(provider);
			Assert.That(EventManager.Count, Is.EqualTo(1),"WeakEventManager.Count");

			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(1),"listener.Count");

			listener = null;
			EventManager.Collect(true);
			Assert.That(EventManager.Count, Is.EqualTo(0),"WeakEventManager.Count");
		}

		[Test,Ignore("TODO") /* does not work in unit test runner */]
		public void GCTest() {
			var provider = new MyEventProvider();
			var listener = new MyEventListener();

			listener.Listen(provider);
			Assert.That(EventManager.Count, Is.EqualTo(1),"WeakEventManager.Count");

			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(1),"listener.Count");

//			var weakListener = new WeakReference(listener);
			listener = null;
			// how long we have to wait until the GC collects?

			GC.Collect();GC.WaitForPendingFinalizers();GC.Collect();

			var stopwatch = new Stopwatch();stopwatch.Start();
			while (EventManager.Count>0) {
				provider.Raise();
				Thread.Sleep(10);
			}
			Trace.WriteLine("Collected after "+stopwatch.Elapsed);
			//Assert.That(WeakEventManager.Count, Is.EqualTo(0),"WeakEventManager.Count");
		}

		[Test,Ignore("TODO") /* does not work in unit test runner */]
		public void MassiveGCTest() {
			var provider = new MyEventProvider();
			var r = new System.Random();
			var stopwatch = new Stopwatch(); stopwatch.Start();
			const int max = 2000;
			var c = EventManager.Count;
			for (int i = 1; i <= max; i++) {
				var listener = new MyEventListener();
				listener.Listen(provider);
				provider.Raise();
				Thread.Sleep(r.Next(8,12+1));
				if (EventManager.Count+c < i) {
					//OK any listener has been collected
					Trace.WriteLine("GC collected at interation "+i+" "+ stopwatch.Elapsed + " "+EventManager.Count +" alive");
					c=i - EventManager.Count;
				}
			}
			Trace.WriteLine(EventManager.Count*100/max+"% alive after "+ stopwatch.Elapsed);
			var next=stopwatch.Elapsed.Add(TimeSpan.FromSeconds(1));
			while (EventManager.Count>0) {
				if(stopwatch.Elapsed<next){Thread.Sleep(10); EventManager.Collect(); continue;}
				Trace.WriteLine(EventManager.Count*100/max+"% alive after "+ stopwatch.Elapsed);
				next=stopwatch.Elapsed.Add(TimeSpan.FromSeconds(1));
			}
			
			//Assert.Fail("Nothing collected!");
		}

		[Test]
		public void EventHandler() {
			var provider = new MyEventProvider ();
			var listener = new MyEventListener();
			listener.Listen(provider);
			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}

		[Test]
		public void EventHandlerNewEventArgs() {
			var provider = new MyEventProvider1EventHandler<MyEventArgs> ();
			var listener = new MyEventListener1EventHandler<MyEventArgs>();
			listener.Listen(provider);
			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}
		[Test]
		public void EventHandlerNewEventArgsˑPerformanceTest() {
			var provider = new MyEventProvider1EventHandler<MyEventArgs> ();
			var listener = new MyEventListener1EventHandler<MyEventArgs>();
			listener.Listen(provider);
			provider.Raise4PerformanceTest();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}

		[Test]
		public void EventHandlerCommonEventArgs() {
			var provider = new MyEventProvider1EventHandler<TreeChangedEventArgs> ();
			var listener = new MyEventListener1EventHandler<TreeChangedEventArgs>();
			listener.Listen(provider);
			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}

		[Test]
		public void AnyEventHandler() {
			var provider = new MyEventProvider2 ();
			var listener = new MyEventListener2();
			listener.Listen(provider);
			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}

		[Test,Ignore("TODO")]
		public void PerformanceTest1() {
			var provider = new MyEventProvider();
			var listener = new MyEventListener();
			listener.Listen(provider);
			var count = 1000000;

			var stopwatch = new Stopwatch();stopwatch.Start();
			for (int i = 0; i < count; i++) {
				provider.Raise();
			}
			stopwatch.Stop();
			Assert.That(listener.Count,Is.EqualTo(count),"listener.Count");
			Trace.WriteLine(string.Format("Time: {0}",new TimeSpan(stopwatch.ElapsedTicks*1)));
		}

		[Test,Ignore("TODO")]
		public void PerformanceTest2() {
			var provider = new MyEventProvider1EventHandler<TreeChangedEventArgs>();
			var listener = new MyEventListener1EventHandler<TreeChangedEventArgs>();
			listener.Listen(provider);
			var count = 1000000;

			var stopwatch = new Stopwatch();stopwatch.Start();
			for (int i = 0; i < count; i++) {
				provider.Raise();
			}
			stopwatch.Stop();
			Assert.That(listener.Count,Is.EqualTo(count),"listener.Count");
			Trace.WriteLine(string.Format("Time: {0}",new TimeSpan(stopwatch.ElapsedTicks*1)));
		}

		public class MyEventProvider {

			private EventSourceStore EventSources;

			public MyEventProvider() {
				EventSources =new EventSourceStore(this);
			}

			public IEventSource<EventHandler> MyEvent { get { return EventSources.Get<EventHandler>("MyEvent"); } }

			public void Raise() {
				EventManager.Raise<EventHandler,EventArgs>(MyEvent,EventArgs.Empty);
			}

		}

		public class MyEventListener {

			private IEventHandle _MyEventHolder;
			public int Count;

			public MyEventListener() {
				
			}

			public void Listen(MyEventProvider provider) {
				_MyEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
			}

			private void AtMyEvent(object sender, EventArgs eventArgs) { Count++; }

		}

		public class MyEventProvider1EventHandler<TEventArgs> where TEventArgs:EventArgs {

			public TEventArgs DefaultEventArgs;

			private EventSourceStore EventSources;

			public MyEventProvider1EventHandler() {
				EventSources =new EventSourceStore(this);
			}

			public IEventSource<EventHandler<TEventArgs>> MyEvent { get { return EventSources.Get<EventHandler<TEventArgs>>("MyEvent"); } }

			public void Raise() {
				EventManager.Raise(MyEvent,DefaultEventArgs);
			}
			public void Raise4PerformanceTest() {
				for (int i = 1000000; i>=0; i--) EventManager.Raise(MyEvent,DefaultEventArgs);
			}
		}

		public class MyEventListener1EventHandler<TEventArgs> where TEventArgs:EventArgs {

			private IEventHandle _MyEventHolder;
			public int Count;

			public MyEventListener1EventHandler() {
				
			}

			public void Listen(MyEventProvider1EventHandler<TEventArgs> provider) {
				_MyEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
			}

			private void AtMyEvent(object sender, TEventArgs eventArgs) { Count++; }

		}


		public class MyEventProvider2 {

			private EventSourceStore EventSources;

			public MyEventProvider2() {
				EventSources =new EventSourceStore(this);
			}

			public IEventSource<MyDelegate> MyEvent { get { return EventSources.Get<MyDelegate>("MyEvent"); } }

			public void Raise() {
				EventManager.Raise(MyEvent,EventArgs.Empty);
			}

		}

		public class MyEventListener2 {

			private IEventHandle _MyEventHolder;
			public int Count;

			public MyEventListener2() {
				
			}

			public void Listen(MyEventProvider2 provider) {
				_MyEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
			}

			private void AtMyEvent(object sender, EventArgs eventArgs) { Count++; }

		}


		public class MyEventArgs : EventArgs {

			public new static readonly MyEventArgs Empty=new MyEventArgs();

		}

		public delegate void MyDelegate(object sender,EventArgs eventArgs);

	}
}
#endif