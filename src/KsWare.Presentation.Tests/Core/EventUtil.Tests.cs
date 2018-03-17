using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.Core {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="EventUtil"/>-class
	/// </summary>
	[TestClass]//[TestSubject(typeof(EventUtil))]
	public class EventUtilTests {

		// PerformanceTest results (1mio calls)
		// delegate direct					  7ms
		// Raise with optimized delegate	 70ms
		// Raise with unknwon delegate		400ms
		// RaiseDynamic						700ms

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Initialize() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Cleanup() { }

		/// <summary> Common
		/// </summary>
		[TestMethod] [Ignore]
		public void Common() {
			TestEvent += FailTestEvent;
			Assert.Throws<MultiTargetInvocationException>(()=> EventUtil.Raise(TestEvent,this,EventArgs.Empty,"{F2D351D0-FE36-46AC-8D9D-C33969840478}"));
		}

		private void FailTestEvent(object sender, EventArgs e) { throw new NotImplementedException(); }

		public event EventHandler TestEvent;

//		public delegate void EventHandler<in TEventArgs>(object sender, TEventArgs e);
		private void DoNothingEventHandlerEventArgs(object sender, EventArgs e) {  }
		private void DoNothingEventHandlerDispatcherEventArgs(object sender, DispatcherEventArgs e) {  }


		public void MeasureSomeInvokationMethods() {
			var max = 10000000;
			var sender=new object();
			var eargs = EventArgs.Empty;
			TestEvent += DoNothingEventHandlerEventArgs;

//			Meassure("pure",() => {
//				var eventHandler = TestEvent;
//				for (int i = 0; i < max; i++) { ; }
//			});
//			Meassure("EventHandler direct",() => {
//				var eventHandler = TestEvent;
//				for (int i = 0; i < max; i++) { eventHandler(sender, eargs); }
//			});
//			Meassure("EventHandler Invoke",() => {
//				var eventHandler = TestEvent;
//				for (int i = 0; i < max; i++) { eventHandler.Invoke(sender, eargs); }
//			});
//			Meassure("EventHandler DynamicInvoke",() => {
//				var eventHandler = TestEvent;
//				for (int i = 0; i < max; i++) { eventHandler.DynamicInvoke(sender, eargs); }
//			});
//			Meassure("Delegate Method.Invoke",() => {
//				Delegate d = TestEvent.GetInvocationList()[0];
//				for (int i = 0; i < max; i++) { d.Method.Invoke(d.Target, new [] {sender, eargs}); }
//			});
//			Meassure("Delegate DynamicInvoke",() => {
//				Delegate d = TestEvent.GetInvocationList()[0];
//				for (int i = 0; i < max; i++) { d.DynamicInvoke(sender, eargs); }
//			});
//			Meassure("EventHandler direct try cast 2x",() => {
//				Delegate d = TestEvent.GetInvocationList()[0];
//				for (int i = 0; i < max; i++) {
//					{var eh = d as EventHandler<DispatcherEventArgs>; if(eh!=null) {eh(sender, (DispatcherEventArgs)eargs); continue;}}
//					{var eh = d as EventHandler; if(eh!=null) {eh(sender, eargs); continue;}}
//				}
//			});
//			Meassure("EventHandler direct try cast 10x",() => {
//				Delegate d = TestEvent.GetInvocationList()[0];
//				for (int i = 0; i < max; i++) {
//					for (int j = 0; j < 9; j++) {
//						{var eh = d as EventHandler<DispatcherEventArgs>; if(eh!=null) {eh(sender, (DispatcherEventArgs)eargs); continue;}}
//					}
//					{var eh = d as EventHandler; if(eh!=null) {eh(sender, eargs); continue;}}
//				}
//			});
//			Meassure("EventHandler direct try cast 100x",() => {
//				Delegate d = TestEvent.GetInvocationList()[0];
//				for (int i = 0; i < max; i++) {
//					for (int j = 0; j < 99; j++) {
//						{var eh = d as EventHandler<DispatcherEventArgs>; if(eh!=null) {eh(sender, (DispatcherEventArgs)eargs); continue;}}
//					}
//					{var eh = d as EventHandler; if(eh!=null) {eh(sender, eargs); continue;}}
//				}
//			});
			

		}

		private double Meassure(string description, Action action) {
			Stopwatch sw = Stopwatch.StartNew();
			action();
			var duration= sw.ElapsedMilliseconds;
			Console.WriteLine(string.Format("{0} {1}", description.Substring(0,Math.Min(description.Length,40)).PadRight(40), duration));
			return duration;
		}

		[TestMethod]
		public void RaiseˑTest() {
			var c=new EventUtilˑRaiseˑTestClass();
			c.RaiseEventHandlerEvent();	
			c.RaiseEventHandler1EventArgsEvent();
			c.RaiseEventHandler1ValueChangedEventArgsEvent();
			c.RaiseCustomEventHandler();
		}

		[TestMethod]
		public void RaiseˑPerformanceTest() {
			var c=new EventUtilˑRaiseˑTestClass();
			for (int i = 0; i < 1000000; i++) {
				c.RaiseEventHandlerEventDirect();
				c.RaiseEventHandlerEvent();
				c.RaiseEventHandler1EventArgsEvent();
				c.RaiseEventHandler1ValueChangedEventArgsEvent();
//				c.RaiseCustomEventHandler();
			}
		}

		[TestMethod]
		public void RaiseːEventHandlerEventDirectˑPerformanceTest() {
			var c=new EventUtilˑRaiseˑTestClass();
			for (int i = 0; i < 1000000; i++)c.RaiseEventHandlerEventDirect();	
		}

		[TestMethod]
		public void RaiseːEventHandlerEventˑPerformanceTest() {
			var c=new EventUtilˑRaiseˑTestClass();
			for (int i = 0; i < 1000000; i++) c.RaiseEventHandlerEvent();	
		}

		[TestMethod]
		public void RaiseːEventHandler1EventArgsEventˑPerformanceTest() {
			var c=new EventUtilˑRaiseˑTestClass();
			for (int i = 0; i < 1000000; i++) c.RaiseEventHandler1EventArgsEvent();
		}

		[TestMethod]
		public void RaiseːEventHandler1ValueChangedEventArgsEventˑPerformanceTest() {
			var c=new EventUtilˑRaiseˑTestClass();
			for (int i = 0; i < 1000000; i++) c.RaiseEventHandler1ValueChangedEventArgsEvent();
		}

		[TestMethod]
		public void RaiseːCustomEventHandlerˑPerformanceTest() {
			// test for a not optimized delegate
			var c=new EventUtilˑRaiseˑTestClass();
			for (int i = 0; i < 1000000; i++) c.RaiseCustomEventHandler();
		}

		[TestMethod]
		public void RaiseˑTargetInvocationExceptionˑTest() {
			var c=new EventUtilˑRaiseˑTestClass();
			Assert.Throws<TargetInvocationException>(() => c.RaiseːTargetInvocationExceptionEvent());
		}


		[TestMethod]
		public void RaiseDynamicːDoubleNestedEventHandlerˑTest() {
			var c=new EventUtilˑRaiseDynamicˑTestClass();
			c.RaiseːDoubleNestedEventHandlerEvent();
		}

		[TestMethod]
		public void RaiseDynamicˑTest() {
			var c=new EventUtilˑRaiseDynamicˑTestClass();
			c.RaiseːEventHandlerEvent();	
			c.RaiseːEventHandler1EventArgsEvent();
			c.RaiseːEventHandler1ValueChangedEventArgsEvent();
			c.RaiseːCustomEventHandlerEvent();				
		}

		[TestMethod]
		public void RaiseDynamicˑCustomEventHandlerˑPerformanceTest() {
			var c=new EventUtilˑRaiseDynamicˑTestClass();
			for (int i = 0; i < 1000000; i++) c.RaiseːCustomEventHandlerEvent();				
		}

		[TestMethod]
		public void RaiseDynamicˑPerformanceTest() {
			var c=new EventUtilˑRaiseDynamicˑTestClass();
			for (int i = 0; i < 100000; i++) {
				c.RaiseːEventHandlerEvent();	
				c.RaiseːEventHandler1EventArgsEvent();
				c.RaiseːEventHandler1ValueChangedEventArgsEvent();
//				c.RaiseCustomEventHandler();					
			}
		}

		[TestMethod]
		public void RaiseDynamicˑTargetInvocationExceptionˑTest() {
			var c=new EventUtilˑRaiseDynamicˑTestClass();
			Assert.Throws<Exception>(() => c.RaiseːExceptionEvent(),"TestCase");
		}

		private class EventUtilˑRaiseˑTestClass {
			public ValueChangedEventArgs DefaultValueChangedEventArgs=new ValueChangedEventArgs(1,2);
			public int EventCount;

			public EventUtilˑRaiseˑTestClass() {
				EventHandlerEvent+=OnEventHandlerEvent;
				EventHandler1EventArgsEvent+=OnEventHandler1EventArgsEvent;
				EventHandler1ValueChangedEventArgsEvent+=OnEventHandler1ValueChangedEventArgsEvent;
				CustomEventHandlerEvent+=OnCustomEventHandlerEvent;
				TargetInvocationExceptionEvent+=OnTargetInvocationExceptionEvent;

				//force ApplicationDispatcher is initialized (for perfoamce test, we do not need measure initialization time)
				var dummy = ApplicationDispatcher.IsInvokeRequired;
			}

			private void OnTargetInvocationExceptionEvent(object sender, EventArgs eventArgs) { throw new Exception("TestCase"); }
			private void OnCustomEventHandlerEvent(object sender, EventArgs args) { EventCount++; }
			private void OnEventHandler1ValueChangedEventArgsEvent(object sender, ValueChangedEventArgs valueChangedEventArgs) { EventCount++; }
			private void OnEventHandler1EventArgsEvent(object sender, EventArgs eventArgs) { EventCount++; }
			private void OnEventHandlerEvent(object sender, EventArgs eventArgs) { EventCount++; }

			public event EventHandler EventHandlerEvent;
			public event EventHandler<EventArgs> EventHandler1EventArgsEvent;
			public event EventHandler<ValueChangedEventArgs> EventHandler1ValueChangedEventArgsEvent;
			public event CustomEventHandler CustomEventHandlerEvent;
			public event CustomEventHandler TargetInvocationExceptionEvent;

			public void RaiseEventHandlerEventDirect() {
				EventHandlerEvent(this, EventArgs.Empty);
			}

			public void RaiseEventHandlerEvent() {
				EventUtil.Raise(EventHandlerEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}

			public void RaiseEventHandler1EventArgsEvent() {
				EventUtil.Raise(EventHandler1EventArgsEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}
			public void RaiseEventHandler1ValueChangedEventArgsEvent() {
				EventUtil.Raise(EventHandler1ValueChangedEventArgsEvent,this,DefaultValueChangedEventArgs,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}

			public void RaiseCustomEventHandler() {
				EventUtil.Raise(CustomEventHandlerEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}
			public void RaiseːTargetInvocationExceptionEvent() {
				EventUtil.Raise(TargetInvocationExceptionEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}
		}
		
		private class EventUtilˑRaiseDynamicˑTestClass {
			public ValueChangedEventArgs DefaultValueChangedEventArgs=new ValueChangedEventArgs(1,2);
			public int EventCount;

			public EventUtilˑRaiseDynamicˑTestClass() {
				EventHandlerEvent+=OnEventHandlerEvent;
				EventHandler1EventArgsEvent+=OnEventHandler1EventArgsEvent;
				EventHandler1ValueChangedEventArgsEvent+=OnEventHandler1ValueChangedEventArgsEvent;
				CustomEventHandlerEvent+=OnCustomEventHandlerEvent;
				DoubleNestedEventHandlerEvent+=OnDoubleNestedEventHandlerEvent;
				ExceptionEvent+=OnExceptionEvent;
				//force ApplicationDispatcher is initialized (for perfoamce test, we do not need measure initialization time)
				var dummy = ApplicationDispatcher.IsInvokeRequired;
			}

			private void OnExceptionEvent(object sender, EventArgs eventArgs) { throw new Exception("TestCase"); }
			private void OnDoubleNestedEventHandlerEvent(object sender, EventArgs args) { EventCount++; }
			private void OnCustomEventHandlerEvent(object sender, EventArgs args) { EventCount++; }
			private void OnEventHandler1ValueChangedEventArgsEvent(object sender, ValueChangedEventArgs valueChangedEventArgs) { EventCount++; }
			private void OnEventHandler1EventArgsEvent(object sender, EventArgs eventArgs) { EventCount++; }
			private void OnEventHandlerEvent(object sender, EventArgs eventArgs) { EventCount++; }

			public event EventHandler EventHandlerEvent;
			public event EventHandler<EventArgs> EventHandler1EventArgsEvent;
			public event EventHandler<ValueChangedEventArgs> EventHandler1ValueChangedEventArgsEvent;
			public event CustomEventHandler CustomEventHandlerEvent;
			public event DoubleNestedEventHandler DoubleNestedEventHandlerEvent;
			public event EventHandler ExceptionEvent;

			public void RaiseːEventHandlerEventｰDirect() {
				EventHandlerEvent(this, EventArgs.Empty);
			}

			public void RaiseːEventHandlerEvent() {
				EventUtil.RaiseDynamic(EventHandlerEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}

			public void RaiseːEventHandler1EventArgsEvent() {
				EventUtil.RaiseDynamic(EventHandler1EventArgsEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}
			public void RaiseːEventHandler1ValueChangedEventArgsEvent() {
				EventUtil.RaiseDynamic(EventHandler1ValueChangedEventArgsEvent,this,DefaultValueChangedEventArgs,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}

			public void RaiseːCustomEventHandlerEvent() {
				EventUtil.RaiseDynamic(CustomEventHandlerEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}
			public void RaiseːDoubleNestedEventHandlerEvent() {
				EventUtil.RaiseDynamic(DoubleNestedEventHandlerEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}

			public void RaiseːExceptionEvent() {
				EventUtil.RaiseDynamic(ExceptionEvent,this,EventArgs.Empty,"{9EDDC463-BD20-4436-9168-B317A6D25A84}");
			}

			//this is a event handler which is double nested (=> namespace.class+class+delegate)
			public delegate void DoubleNestedEventHandler(object sender, EventArgs args);
		}

		//this is a event handler which is not known in Presentation framework
		public delegate void CustomEventHandler(object sender, EventArgs args);

		//this is a event handler which has unsupported method signature
		public delegate void UnsupportedEventHandler(object sender, EventArgs args, string unsupportedParameter);

	}
	// ReSharper restore InconsistentNaming
}
