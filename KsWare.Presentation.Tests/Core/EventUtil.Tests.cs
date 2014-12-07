using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Threading;
using KsWare.Presentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.Core {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="EventUtil"/>-class
	/// </summary>
	[TestClass]
	public class EventUtilTests {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() { }

		/// <summary> Common
		/// </summary>
		[TestMethod] [Ignore]
		public void Common() {
			TestEvent += FailTestEvent;
			Assert.Throws<MultiTargetInvocationException>(()=> EventUtil.Raise(TestEvent,this,EventArgs.Empty,"{F2D351D0-FE36-46AC-8D9D-C33969840478}"));
		}

		private void FailTestEvent(object sender, EventArgs e) { throw new NotImplementedException(); }

		public event EventHandler TestEvent;

		public event EventHandler<DispatcherEventArgs> TestEvent2;
//		public delegate void EventHandler<in TEventArgs>(object sender, TEventArgs e);
		private void DoNothingEventHandlerEventArgs(object sender, EventArgs e) {  }
		private void DoNothingEventHandlerDispatcherEventArgs(object sender, DispatcherEventArgs e) {  }


		[TestMethod]
		public void Invoke1() {
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

		[TestMethod,Ignore,Obsolete("Only for debug.")]
		public void DelegateCasting() {
			TestEvent2+=DoNothingEventHandlerDispatcherEventArgs;
			var d = TestEvent2.GetInvocationList()[0];
//			var e = (EventHandler<DispatcherEventArgs>) d;
			var e = (EventHandler) d;
			e(null, null);
		}

		private double Meassure(string description, Action action) {
			Stopwatch sw = Stopwatch.StartNew();
			action();
			var duration= sw.ElapsedMilliseconds;
			Console.WriteLine(string.Format("{0} {1}", description.Substring(0,Math.Min(description.Length,40)).PadRight(40), duration));
			return duration;
		}

	}
	// ReSharper restore InconsistentNaming
}
