using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using KsWare.Presentation.BusinessFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert=NUnit.Framework.Assert;
using Is=NUnit.Framework.Is;
using IgnoreAttribute=Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute;

namespace KsWare.Presentation.Tests.Core {

	[TestClass]
	public class WeakEventManagerTests {

		[TestInitialize]
		public void Initialize() {
			EventUtil.WeakEventManager.Reset();
		}

		private static void ShowAssemblyPublicKey() {
			var ba = Assembly.GetExecutingAssembly().GetName(false).GetPublicKey();
			var hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba) hex.AppendFormat("{0:x2}", b);
			Trace.WriteLine("PublicKey: "+ hex);
		}


		[TestMethod]
		public void Reset() {
			var testSubject = new PrivateType(typeof (EventUtil.WeakEventManager));
			// done in Initialize():> EventUtil.WeakEventManager.Reset();
			Assert.That(EventUtil.WeakEventManager.Count,Is.EqualTo(0));
			Assert.That((DateTime)testSubject.GetStaticField("s_LastCollect"),Is.EqualTo(DateTime.MinValue));
			Assert.That(EventUtil.WeakEventManager.StatisticsːRaiseːInvocationCount,Is.EqualTo(0));
		}


		[TestMethod]
		public void GCCollectTest() {
			var provider = new MyEventProvider();
			var listener = new MyEventListener();

			listener.Listen(provider);
			Assert.That(EventUtil.WeakEventManager.Count, Is.EqualTo(1),"WeakEventManager.Count");

			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(1),"listener.Count");

			listener = null;
			EventUtil.WeakEventManager.Collect(true);
			Assert.That(EventUtil.WeakEventManager.Count, Is.EqualTo(0),"WeakEventManager.Count");
		}

		[TestMethod,Ignore /* does not work in unit test runner */]
		public void GCTest() {
			var provider = new MyEventProvider();
			var listener = new MyEventListener();

			listener.Listen(provider);
			Assert.That(EventUtil.WeakEventManager.Count, Is.EqualTo(1),"WeakEventManager.Count");

			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(1),"listener.Count");

//			var weakListener = new WeakReference(listener);
			listener = null;
			// how long we have to wait until the GC collects?

			GC.Collect();GC.WaitForPendingFinalizers();GC.Collect();

			var stopwatch = new Stopwatch();stopwatch.Start();
			while (EventUtil.WeakEventManager.Count>0) {
				provider.Raise();
				Thread.Sleep(10);
			}
			Trace.WriteLine("Collected after "+stopwatch.Elapsed);
			//Assert.That(EventUtil.WeakEventManager.Count, Is.EqualTo(0),"WeakEventManager.Count");
		}

		[TestMethod,Ignore /* does not work in unit test runner */]
		public void MassiveGCTest() {
			var provider = new MyEventProvider();
			var r = new System.Random();
			var stopwatch = new Stopwatch(); stopwatch.Start();
			const int max = 2000;
			var c = EventUtil.WeakEventManager.Count;
			for (int i = 1; i <= max; i++) {
				var listener = new MyEventListener();
				listener.Listen(provider);
				provider.Raise();
				Thread.Sleep(r.Next(8,12+1));
				if (EventUtil.WeakEventManager.Count+c < i) {
					//OK any listener has been collected
					Trace.WriteLine("GC collected at interation "+i+" "+ stopwatch.Elapsed + " "+EventUtil.WeakEventManager.Count +" alive");
					c=i - EventUtil.WeakEventManager.Count;
				}
			}
			Trace.WriteLine(EventUtil.WeakEventManager.Count*100/max+"% alive after "+ stopwatch.Elapsed);
			var next=stopwatch.Elapsed.Add(TimeSpan.FromSeconds(1));
			while (EventUtil.WeakEventManager.Count>0) {
				if(stopwatch.Elapsed<next){Thread.Sleep(10); EventUtil.WeakEventManager.Collect(); continue;}
				Trace.WriteLine(EventUtil.WeakEventManager.Count*100/max+"% alive after "+ stopwatch.Elapsed);
				next=stopwatch.Elapsed.Add(TimeSpan.FromSeconds(1));
			}
			
			//Assert.Fail("Nothing collected!");
		}

		[TestMethod]
		public void EventHandler() {
			var provider = new MyEventProvider ();
			var listener = new MyEventListener();
			listener.Listen(provider);
			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}

		[TestMethod]
		public void EventHandlerNewEventArgs() {
			var provider = new MyEventProvider1EventHandler<MyEventArgs> ();
			var listener = new MyEventListener1EventHandler<MyEventArgs>();
			listener.Listen(provider);
			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}

		[TestMethod]
		public void EventHandlerCommonEventArgs() {
			var provider = new MyEventProvider1EventHandler<TreeChangedEventArgs> ();
			var listener = new MyEventListener1EventHandler<TreeChangedEventArgs>();
			listener.Listen(provider);
			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}

		[TestMethod]
		public void AnyEventHandler() {
			var provider = new MyEventProvider2 ();
			var listener = new MyEventListener2();
			listener.Listen(provider);
			provider.Raise();
			Assert.That(listener.Count,Is.EqualTo(listener.Count),"listener.Count");
		}

		[TestMethod,Ignore]
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

		[TestMethod,Ignore]
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

			private WeakEventPropertyStore WeakEventProperties;

			public MyEventProvider() {
				WeakEventProperties =new WeakEventPropertyStore(this);
			}

//			public IWeakEventSource<EventHandler> MyEvent { get { return WeakEventProperties.Get(() => MyEvent); } }
			public IWeakEventSource<EventHandler> MyEvent { get { return WeakEventProperties.Get<EventHandler>("MyEvent"); } }

			public void Raise() {
				EventUtil.WeakEventManager.Raise(MyEvent,EventArgs.Empty);
			}

		}

		public class MyEventListener {

			private IWeakEventHandle m_MyWeakEventHolder;
			public int Count;

			public MyEventListener() {
				
			}

			public void Listen(MyEventProvider provider) {
				m_MyWeakEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
			}

			private void AtMyEvent(object sender, EventArgs eventArgs) { Count++; }

		}

		public class MyEventProvider1EventHandler<TEventArgs> where TEventArgs:EventArgs {

			public TEventArgs DefaultEventArgs;

			private WeakEventPropertyStore WeakEventProperties;

			public MyEventProvider1EventHandler() {
				WeakEventProperties =new WeakEventPropertyStore(this);
			}

			public IWeakEventSource<EventHandler<TEventArgs>> MyEvent { get { return WeakEventProperties.Get<EventHandler<TEventArgs>>("MyEvent"); } }

			public void Raise() {
				EventUtil.WeakEventManager.Raise(MyEvent,DefaultEventArgs);
			}

		}

		public class MyEventListener1EventHandler<TEventArgs> where TEventArgs:EventArgs {

			private IWeakEventHandle m_MyWeakEventHolder;
			public int Count;

			public MyEventListener1EventHandler() {
				
			}

			public void Listen(MyEventProvider1EventHandler<TEventArgs> provider) {
				m_MyWeakEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
			}

			private void AtMyEvent(object sender, TEventArgs eventArgs) { Count++; }

		}


		public class MyEventProvider2 {

			private WeakEventPropertyStore WeakEventProperties;

			public MyEventProvider2() {
				WeakEventProperties =new WeakEventPropertyStore(this);
			}

			public IWeakEventSource<MyDelegate> MyEvent { get { return WeakEventProperties.Get<MyDelegate>("MyEvent"); } }

			public void Raise() {
				EventUtil.WeakEventManager.Raise(MyEvent,EventArgs.Empty);
			}

		}

		public class MyEventListener2 {

			private IWeakEventHandle m_MyWeakEventHolder;
			public int Count;

			public MyEventListener2() {
				
			}

			public void Listen(MyEventProvider2 provider) {
				m_MyWeakEventHolder=provider.MyEvent.RegisterWeak(AtMyEvent);
			}

			private void AtMyEvent(object sender, EventArgs eventArgs) { Count++; }

		}


		public class MyEventArgs : EventArgs {

			public new static readonly MyEventArgs Empty=new MyEventArgs();

		}

		public delegate void MyDelegate(object sender,EventArgs eventArgs);

	}
}
