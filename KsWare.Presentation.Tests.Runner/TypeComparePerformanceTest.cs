using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KsWare.Presentation.Tests.Runner {

	public class TypeComparePerformanceTest {

		public void Run() {
			A(new NotifyCollectionChangedEventHandler((sender, eventArgs) => { }));
			B(new NotifyCollectionChangedEventHandler((sender, eventArgs) => { }));
			C(new NotifyCollectionChangedEventHandler((sender, eventArgs) => { }));
			D(new NotifyCollectionChangedEventHandler((sender, eventArgs) => { }));
			A1(new NotifyCollectionChangedEventHandler((sender, eventArgs) => { }));
			D1(new NotifyCollectionChangedEventHandler((sender, eventArgs) => { }));
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		private static void A(Delegate handler) {
			object handle=null;
			for (int i = 0; i < 1000000; i++) {
				switch (handler.GetType().FullName) {
					case "System.EventHandler`1[[System.ComponentModel.PropertyChangedEventArgs, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]": handle = handler;break;
					case "System.EventHandler`1[[System.Collections.Specialized.NotifyCollectionChangedEventArgs, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler": handle = handler;break;
					case "System.Collections.Specialized.NotifyCollectionChangedEventHandler": handle = handler;break;
					default: handle = handler;break;
				}
			}
			DoNothing(handle);
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		private static void A1(Delegate handler) {
			object handle=null;
			for (int i = 0; i < 1000000; i++) {
				switch (handler.GetType().FullName) {
					case "System.ComponentModel.PropertyChangedEventHandler0": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler1": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler2": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler3": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler4": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler5": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler6": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler7": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler8": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler9": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler10": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler11": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler12": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler13": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler14": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler15": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler16": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler17": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler18": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler19": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler20": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler21": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler22": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler23": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler24": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler25": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler26": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler27": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler28": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler29": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler30": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler31": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler32": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler33": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler34": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler35": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler36": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler37": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler38": handle = handler;break;
					case "System.ComponentModel.PropertyChangedEventHandler39": handle = handler;break;
					default: handle = handler;break;
				}
			}
			DoNothing(handle);
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		private static void B(Delegate handler) {
			object handle=null;
			for (int i = 0; i < 1000000; i++) {
				if(handler is EventHandler)
					handle = handler;
				else if(handler is EventHandler<PropertyChangedEventArgs>)
					handle = handler;
				else if (handler is EventHandler<NotifyCollectionChangedEventArgs>) 
					handle = handler;
				else if(handler is PropertyChangedEventHandler)
					handle = handler;
				else if(handler is NotifyCollectionChangedEventHandler)
					handle = handler;
				else 
					handle = handler;
			}
			DoNothing(handle);
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		private static void C(Delegate handler) {
			object handle=null;
			for (int i = 0; i < 1000000; i++) {
				var t = handler.GetType();
				if(t== typeof(EventHandler))
					handle = handler;
				else if(t == typeof(EventHandler<PropertyChangedEventArgs>))
					handle = handler;
				else if (t == typeof(EventHandler<NotifyCollectionChangedEventArgs>))
					handle = handler;
				else if(t == typeof(PropertyChangedEventHandler))
					handle = handler;
				else if(t == typeof(NotifyCollectionChangedEventHandler))
					handle = handler;
				else 
					handle = handler;
			}
			DoNothing(handle);
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		private static void D(Delegate handler) {
			object handle=null;
			var a = typeof(EventHandler<PropertyChangedEventArgs>);
			var b = typeof(EventHandler<NotifyCollectionChangedEventArgs>);
			var c = typeof(PropertyChangedEventHandler);
			var d = typeof(NotifyCollectionChangedEventHandler);
			for (int i = 0; i < 1000000; i++) {
				var t = handler.GetType();
				if(t== typeof(EventHandler))
					handle = handler;
				else if(t == a)
					handle = handler;
				else if (t == b)
					handle = handler;
				else if(t == c)
					handle = handler;
				else if(t == d)
					handle = handler;
				else 
					handle = handler;
			}
			DoNothing(handle);
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		private static void D1(Delegate handler) {
			object handle=null;
			var t1 = typeof(EventHandler01);
			var t2 = typeof(EventHandler02);
			var t3 = typeof(EventHandler03);
			var t4 = typeof(EventHandler04);
			var t5 = typeof(EventHandler05);
			var t6 = typeof(EventHandler06);
			var t7 = typeof(EventHandler07);
			var t8 = typeof(EventHandler08);
			var t9 = typeof(EventHandler09);
			var t10 = typeof(EventHandler10);
			var t11 = typeof(EventHandler11);
			var t12 = typeof(EventHandler12);
			var t13 = typeof(EventHandler13);
			var t14 = typeof(EventHandler14);
			var t15 = typeof(EventHandler15);
			var t16 = typeof(EventHandler16);
			var t17 = typeof(EventHandler17);
			var t18 = typeof(EventHandler18);
			var t19 = typeof(EventHandler19);
			var t20 = typeof(EventHandler20);
			var t21 = typeof(EventHandler21);
			var t22 = typeof(EventHandler22);
			var t23 = typeof(EventHandler23);
			var t24 = typeof(EventHandler24);
			var t25 = typeof(EventHandler25);
			var t26 = typeof(EventHandler26);
			var t27 = typeof(EventHandler27);
			var t28 = typeof(EventHandler28);
			var t29 = typeof(EventHandler29);
			var t30 = typeof(EventHandler30);
			var t31 = typeof(EventHandler31);
			var t32 = typeof(EventHandler32);
			var t33 = typeof(EventHandler33);
			var t34 = typeof(EventHandler34);
			var t35 = typeof(EventHandler35);
			var t36 = typeof(EventHandler36);
			var t37 = typeof(EventHandler37);
			var t38 = typeof(EventHandler38);
			var t39 = typeof(EventHandler39);
			for (int i = 0; i < 1000000; i++) {
				var t = handler.GetType();
				if(t== typeof(EventHandler)) handle = handler;
				else if(t == t1) handle = handler;
				else if(t == t2) handle = handler;
				else if(t == t3) handle = handler;
				else if(t == t4) handle = handler;
				else if(t == t5) handle = handler;
				else if(t == t6) handle = handler;
				else if(t == t7) handle = handler;
				else if(t == t8) handle = handler;
				else if(t == t9) handle = handler;
				else if(t == t10) handle = handler;
				else if(t == t11) handle = handler;
				else if(t == t12) handle = handler;
				else if(t == t13) handle = handler;
				else if(t == t14) handle = handler;
				else if(t == t15) handle = handler;
				else if(t == t16) handle = handler;
				else if(t == t17) handle = handler;
				else if(t == t18) handle = handler;
				else if(t == t19) handle = handler;
				else if(t == t20) handle = handler;
				else if(t == t21) handle = handler;
				else if(t == t22) handle = handler;
				else if(t == t23) handle = handler;
				else if(t == t24) handle = handler;
				else if(t == t25) handle = handler;
				else if(t == t26) handle = handler;
				else if(t == t27) handle = handler;
				else if(t == t28) handle = handler;
				else if(t == t29) handle = handler;
				else if(t == t30) handle = handler;
				else if(t == t31) handle = handler;
				else if(t == t32) handle = handler;
				else if(t == t33) handle = handler;
				else if(t == t34) handle = handler;
				else if(t == t35) handle = handler;
				else if(t == t36) handle = handler;
				else if(t == t37) handle = handler;
				else if(t == t38) handle = handler;
				else if(t == t39) handle = handler;
				else handle = handler;
			}
			DoNothing(handle);
		}

		private static void DoNothing(object handle) {  }

		private delegate void EventHandler00(object sender, EventArgs args);
		private delegate void EventHandler01(object sender, EventArgs args);
		private delegate void EventHandler02(object sender, EventArgs args);
		private delegate void EventHandler03(object sender, EventArgs args);
		private delegate void EventHandler04(object sender, EventArgs args);
		private delegate void EventHandler05(object sender, EventArgs args);
		private delegate void EventHandler06(object sender, EventArgs args);
		private delegate void EventHandler07(object sender, EventArgs args);
		private delegate void EventHandler08(object sender, EventArgs args);
		private delegate void EventHandler09(object sender, EventArgs args);
		private delegate void EventHandler10(object sender, EventArgs args);
		private delegate void EventHandler11(object sender, EventArgs args);
		private delegate void EventHandler12(object sender, EventArgs args);
		private delegate void EventHandler13(object sender, EventArgs args);
		private delegate void EventHandler14(object sender, EventArgs args);
		private delegate void EventHandler15(object sender, EventArgs args);
		private delegate void EventHandler16(object sender, EventArgs args);
		private delegate void EventHandler17(object sender, EventArgs args);
		private delegate void EventHandler18(object sender, EventArgs args);
		private delegate void EventHandler19(object sender, EventArgs args);
		private delegate void EventHandler20(object sender, EventArgs args);
		private delegate void EventHandler21(object sender, EventArgs args);
		private delegate void EventHandler22(object sender, EventArgs args);
		private delegate void EventHandler23(object sender, EventArgs args);
		private delegate void EventHandler24(object sender, EventArgs args);
		private delegate void EventHandler25(object sender, EventArgs args);
		private delegate void EventHandler26(object sender, EventArgs args);
		private delegate void EventHandler27(object sender, EventArgs args);
		private delegate void EventHandler28(object sender, EventArgs args);
		private delegate void EventHandler29(object sender, EventArgs args);
		private delegate void EventHandler30(object sender, EventArgs args);
		private delegate void EventHandler31(object sender, EventArgs args);
		private delegate void EventHandler32(object sender, EventArgs args);
		private delegate void EventHandler33(object sender, EventArgs args);
		private delegate void EventHandler34(object sender, EventArgs args);
		private delegate void EventHandler35(object sender, EventArgs args);
		private delegate void EventHandler36(object sender, EventArgs args);
		private delegate void EventHandler37(object sender, EventArgs args);
		private delegate void EventHandler38(object sender, EventArgs args);
		private delegate void EventHandler39(object sender, EventArgs args);
	}
}
