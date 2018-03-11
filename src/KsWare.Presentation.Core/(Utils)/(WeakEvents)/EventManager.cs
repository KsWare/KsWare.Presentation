using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	/// <summary> Manages events
	/// </summary>
	/// <remarks>
	/// <list type="table">
	///		<listheader><term>Function</term><description>Description</description></listheader>
	///		<item><term>Register/RegisterSource</term><description>Registers an event handler / an event source</description></item>
	///		<item><term>Raise</term><description>Raises an event</description></item>
	///		<item><term>Release/ReleaseSource</term><description>Releases an event handler / an event source</description></item>
	///		<item><term>Collect</term><description>Collect and remove event handlers / event sources and which are garbaged</description></item>
	/// </list>
	/// </remarks>
	public static partial class EventManager {

//		private static readonly List<WeakEventContainer> s_WeakEvents =new List<WeakEventContainer>();
		private static DateTime s_LastCollect;
		private static TimeSpan s_MinCollectInterval=TimeSpan.FromMilliseconds(1000);

		internal static long StatisticsːRaiseːInvocationCount;

//		/// <summary> Gets the number of registered weak event handlers.
//		/// </summary>
//		/// <value> The number of registered weak event handlers </value>
//		public static int Count {
//			get { return 0; }
//			//get {lock(s_WeakEvents) return s_WeakEvents.Count;}
//		}

//		public static IWeakEventHandle Register(object destination, Delegate handler, string destinationUid, object source, string eventName) {
		internal static EventContainer Register(
			[NotNull] IEventSource     eventSource, 
			          object           destination, 
			          Delegate         handler, 
			          string           destinationUid, 
			          object           sourceObject, 
			          string           eventName
		) {
			return RegisterImpl(eventSource,destination, handler,destinationUid, sourceObject, eventName, weak:false);
		}

//		public static IWeakEventHandle Register(Delegate handler, object source, string eventName) {
		internal static EventContainer Register(
			[NotNull] IEventSource     eventSource, 
			          Delegate         handler, 
			          object           sourceObject, 
			          string           eventName
		) {
			return RegisterImpl(eventSource,null, handler, null, sourceObject, eventName, weak:false);
		}

//		public static IWeakEventHandle RegisterExpression(Expression<Action<object, EventArgs>> expression) {
////			var x=new Expression<Action<object, EventArgs>>();
////			x.Update()
//			//expression.GetType().FullName	        "System.Linq.Expressions.Expression`1[[System.Action`2[[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.EventArgs, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"	string
//			//expression.Body
//			//+		GetType().FullName	"System.Linq.Expressions.InstanceMethodCallExpressionN"	string
//			//+		Method				{Void UpdateComponent()}	System.Reflection.MethodInfo {System.Reflection.RuntimeMethodInfo}
//			//+		Object				{value(ProDevis.Monitor.MessageList.CanMessageItem)}	System.Linq.Expressions.Expression {System.Linq.Expressions.ConstantExpression}
//			//new System.Linq.Expressions.MethodCallExpression()
//
//			var action = expression.Compile();
//			action.Invoke(null,null);
//			return null;
////			var handle = new RegisteredWeakEvent(destination, handler, destinationUid, source, eventName);
////			lock(s_WeakEvents) s_WeakEvents.Add(new Container(handle,true));
////			return handle;
//		}

		/// <summary> [EXPERIMENTAL] 
		/// </summary>
		/// <param name="handler"></param>
		/// <param name="sourceObject"></param>
		/// <param name="eventName"></param>
		/// <returns></returns>
//		public static IWeakEventHandle RegisterWeak(Delegate handler, object source, string eventName) {

		internal static EventContainer RegisterWeak([NotNull] IEventSource eventSource, Delegate handler, object sourceObject, string eventName) {
			return RegisterImpl(
				eventSource: eventSource,
				destination    : null, 
				handler        : handler, 
				destinationUid : null, 
				sourceObject   : sourceObject, 
				eventName      : eventName,
				weak           : true
			);
		}

		internal static EventContainer RegisterWeak(
			[NotNull] IEventSource     eventSource, 
			          object           destination, 
			          Delegate         handler, 
			          string           destinationUid, 
			          object           sourceObject, 
			          string           eventName
			) {
			return RegisterImpl(
				eventSource: eventSource,
				destination    : destination, 
				handler        : handler, 
				destinationUid : destinationUid, 
				sourceObject   : sourceObject, 
				eventName      : eventName,
				weak           : true
			);
		}

		internal static EventContainer RegisterImpl(
			[NotNull] IEventSource eventSource, 
			          object           destination,
			          Delegate         handler, 
			          string           destinationUid,
			          object           sourceObject, 
			          string           eventName,
			          bool             weak
		) {
//			CollectIfRequired();
			var container=CreateContainer(
				eventSource: eventSource,
				destination    : destination, 
				handler        : handler, 
				destinationUid : destinationUid, 
				sourceObject   : sourceObject, 
				eventName      : eventName,
				keepAlive      : weak==false
			);
			return container;
		}

		internal static EventContainer CreateContainer(
			[NotNull] IEventSource eventSource, 
			          object       destination, 
			          Delegate     handler, 
			          string       destinationUid, 
			          object       sourceObject, 
			          string       eventName,
			          bool         keepAlive
		) {
			// to avoid unnecessary type checking and casting at runtime in the whole process of raising events
			// we do this only one time to create specialized classes which contains the well known delegate types at compile time
			// to speedup MakeGenericType we check first for some frequently used event handler types
			EventHandle handle;
//			switch (handler.GetType().FullName) {}
			if(handler is EventHandler)
				handle = new EventHandle4SystemEventHandler(eventSource, destination, (EventHandler)handler, destinationUid, sourceObject, eventName);
			else if(handler is EventHandler<TreeChangedEventArgs>)
				handle = new EventHandle4EventHandlerGeneric<TreeChangedEventArgs>(eventSource, destination, (EventHandler<TreeChangedEventArgs>)handler, destinationUid, sourceObject, eventName);
			else if (handler is EventHandler<ValueChangedEventArgs>) 
				handle = new EventHandle4EventHandlerGeneric<ValueChangedEventArgs>(eventSource, destination, (EventHandler<ValueChangedEventArgs>) handler, destinationUid, sourceObject, eventName);
			else if(handler is PropertyChangedEventHandler)
				handle = new EventHandle4PropertyChangedEventHandler(eventSource, destination, (PropertyChangedEventHandler)handler, destinationUid, sourceObject, eventName);
			else if(handler is NotifyCollectionChangedEventHandler)
				handle = new EventHandle4NotifyCollectionChangedEventHandler(eventSource, destination, (NotifyCollectionChangedEventHandler)handler, destinationUid, sourceObject, eventName);
			else {
				var handlerType = handler.GetType();
				if (handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof (EventHandler<>)) {
					//create a handler specific RegisteredWeakEvent1EventHandler1<...> object
					var gas  = handlerType.GetGenericArguments();
					var rwet = typeof (EventHandle4EventHandlerGeneric<>).MakeGenericType(gas[0]);
					var args = new object[] {eventSource, destination, handler, destinationUid, sourceObject, eventName};
					var bf   = BindingFlags.Instance | BindingFlags.Public;
					handle = (EventHandle) Activator.CreateInstance(rwet, bf, null, args, null);
				} else {
					//fallback to default handling
					handle = new EventHandle4Universal(eventSource, destination, handler, destinationUid, sourceObject, eventName);
				}
			}
			var container = new EventContainer(handle, keepAlive);
			return container;
		}

//		[Obsolete("Use overload!",true)]
//		public static void Raise(IWeakEventSource source, EventArgs e) {
//			var wes = (WeakEventSource) source;
//			RaiseInternal(wes,wes.EventName,new object[]{wes.Source,e});
//		}

//		// _ => MyEventProperty
//		[Obsolete("Slow!",true)]
//		public static void Raise<T>(Lazy<WeakEventPropertyStore> lazyWeakEventProperties, Expression<Func<object, IWeakEventSource<T>>> eventPropertyExpression, EventArgs e) /*where T:EventArgs*/ {
//			if(!lazyWeakEventProperties.IsValueCreated) return;
//			var weakEventPropertyStore = lazyWeakEventProperties.Value;
//			if(weakEventPropertyStore.Count==0) return;
//			var wes=(WeakEventSource<T>)weakEventPropertyStore.TryGet(eventPropertyExpression);
//			if(wes==null/*No one has accessed the event propert*/) return;
//			RaiseInternal(wes.Source,wes.EventName,new object[]{wes.Source,e});
//		}

//		// () => MyEventProperty
//		[Obsolete("Slow!",true)]
//		public static void Raise<T>(Lazy<WeakEventPropertyStore> lazyWeakEventProperties, Expression<Func<IWeakEventSource<T>>> eventPropertyExpression, EventArgs e) /*where T:EventArgs*/{
//			if(!lazyWeakEventProperties.IsValueCreated) return;
//			var weakEventPropertyStore = lazyWeakEventProperties.Value;
//			if(weakEventPropertyStore.Count==0) return;
//			var wes=(WeakEventSource<T>)weakEventPropertyStore.TryGet(eventPropertyExpression);
//			if(wes==null/*No one has accessed the event propert*/) return;
//			RaiseInternal(wes.Source,wes.EventName,new object[]{wes.Source,e});
//		}

//		public static void Raise<T>(Lazy<WeakEventPropertyStore> lazyWeakEventProperties, string eventName, EventArgs e) /*where T:EventArgs*/{
//			if(!lazyWeakEventProperties.IsValueCreated) return;
//			var weakEventPropertyStore = lazyWeakEventProperties.Value;
//			if(weakEventPropertyStore.Count==0) return;
//			var wes=(WeakEventSource<T>)weakEventPropertyStore.TryGet<T>(eventName);
//			if(wes==null/*No one has accessed the event propert*/) return;
//			wes.Raise(e);
//		}

		public static void Raise<TEventHandler,TEventArgs>(IEventSource<TEventHandler> source, TEventArgs e) where TEventArgs:EventArgs {
			var wes = (EventSource<TEventHandler>) source;
			wes.Raise<TEventArgs>(e);
		}

		/// <summary> Raises the specified lazy weak event.
		/// </summary>
		/// <typeparam name="TEventHandler">The type of the event handler (a delegate type)</typeparam>
		/// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
		/// <param name="lazyEventSourceStore">The store with the lazy weak event sources.</param>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="e">The <see cref="TEventArgs"/> instance containing the event data.</param>
		/// <example><code>WeakEventManager.Raise&gt;EventHandler,EventArgs>(LazyWeakEventStore,"MyPropertyChangedEvent", EventArgs.Empty);</code></example>
		public static void Raise<TEventHandler,TEventArgs>(Lazy<EventSourceStore> lazyEventSourceStore, string eventName, TEventArgs e) where TEventArgs:EventArgs {
			if(!lazyEventSourceStore.IsValueCreated) return;
			var weakEventPropertyStore = lazyEventSourceStore.Value;
			if(weakEventPropertyStore.Count==0) return;
			var wes=(EventSource<TEventHandler>)weakEventPropertyStore.TryGet<TEventHandler>(eventName);
			if(wes==null/*No one has accessed the event propert*/) return;
			wes.Raise<TEventArgs>(e);
		}

//		private static void RaiseInternal(object source, string eventName, object[] args) {
//			Interlocked.Increment(ref StatisticsːRaiseːInvocationCount);
//			var raise = new List<WeakEventHandle>();
//			lock (s_WeakEvents) {
//				var remove = new List<WeakEventContainer>();
//				var ec = s_WeakEvents.Count;
//				for (int i = 0; i < ec; i++) {
//					var container = s_WeakEvents[i];
//					var handle = container.WeakEventHandle;
//					if (handle == null) { remove.Add(container); continue; }
//					if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}
//
//					if(handle.EventName==eventName && handle.Source==source) raise.Add(handle);
//				}
//				var rc = remove.Count;
//				for (int i = 0; i < rc; i++) remove[i].Dispose();
//			}
//
//			var ic = raise.Count;
//			for (int i = 0; i < ic; i++) raise[i].Raise(args);
//		}

//		/// <summary> Releases all events for the specified destination and id
//		/// </summary>
//		/// <param name="destination">The event destination.</param>
//		/// <param name="uid">A string (unique for the destination) identifying the event</param>
//		public static void Release(object destination, string uid) {
//			var release = new List<RegisteredWeakEvent>();
//			lock (s_WeakEvents) {
//				var remove = new List<WeakEventContainer>();
//				foreach (var container in s_WeakEvents) {
//					var handle = container.WeakEventHandle;
//					if (handle == null) { remove.Add(container); continue; }
//					if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}
//
//					if (handle.DestinationUid == uid && handle.Destination == destination) release.Add(handle);
//				}
//				foreach (var container in remove) s_WeakEvents.Remove(container);
//				foreach (var bEvent in release) bEvent.Release();
//			}
//		}

//		/// <summary> Releases all events for the specified source
//		/// </summary>
//		/// <param name="source">The source</param>
//		/// <remarks>Event source should call this on Dispose</remarks>
//		public static void ReleaseSource(object source) {
//			var release = new List<WeakEventHandle>();
//			lock (s_WeakEvents) {
//				var remove = new List<WeakEventContainer>();
//				foreach (var container in s_WeakEvents) {
//					var handle = container.WeakEventHandle;
//					if (handle == null) { remove.Add(container); continue; }
//					if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}
//
//					if(handle.Source==source) release.Add(handle);
//				}
//				foreach (var container in remove) s_WeakEvents.Remove(container);
//				foreach (var bEvent in release) bEvent.Release();
//			}
//		}

//		/// <summary> Releases all events for the specified destination
//		/// </summary>
//		/// <param name="destination">The destination</param>
//		/// <remarks>Event listener should call this on Dispose</remarks>
//		public static void ReleaseDestination(object destination) {
//			var release = new List<WeakEventHandle>();
//			lock (s_WeakEvents) {
//				var remove = new List<WeakEventContainer>();
//				foreach (var container in s_WeakEvents) {
//					var handle = container.WeakEventHandle;
//					if (handle == null) { remove.Add(container); continue; }
//					if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}
//
//					if(handle.Destination==destination) release.Add(handle);
//				}
//				foreach (var container in remove) s_WeakEvents.Remove(container);
//				foreach (var bEvent in release) {bEvent.Release();}
//			}
//		}


		private static void CollectIfRequired() { 
			if(DateTime.Now<s_LastCollect.Add(s_MinCollectInterval))return;
			CollectInternal(false);
		}

		/// <summary> Forces an immediate garbage collection.
		/// </summary>
		public static void Collect() {
			CollectInternal(false);
		}
		/// <summary> Forces an immediate garbage collection und optional also GC.Collect.
		/// </summary>
		public static void Collect(bool gcCollect) {
			CollectInternal(gcCollect);
		}

		private static void CollectInternal(bool gcCollect) {

//			if (gcCollect) {
//				GC.Collect();
//				GC.WaitForPendingFinalizers();
//				GC.Collect();
//			}
//
//			var remove = new List<WeakEventContainer>();
//			lock (s_WeakEvents) {
//				foreach (var container in s_WeakEvents) {
//					var handle = container.WeakEventHandle;
//					if (handle == null) { remove.Add(container); continue; }
//					if(!handle.IsAlive) { container.Dispose(); remove.Add(container);}
//				}
//				foreach (var container in remove) {
//					container.Dispose();
//					s_WeakEvents.Remove(container);
//				}
//			}
//			s_LastCollect = DateTime.Now;
		}


//		[Obsolete("Use overload!",true)]
//		public static IWeakEventSource RegisterSource(object source, string eventName) {
//			return new WeakEventSource(source, eventName);
//		}

//		[Obsolete("Use property initializer: public IWeakEventSource<EventHandler> MyPropertyChangedEvent { get { return WeakEventProperties.Get(\"MyPropertyChangedEvent\"); } }")]
//		public static IWeakEventSource<TEvent> RegisterSource<TEvent>(object source, string eventName) /*where TEvent:EventHandler<EventArgs>*/ {
//			var eventsource= new WeakEventSource<TEvent>(source, eventName);
//			return eventsource;
//		}

		internal static IEventSource<TEvent> RegisterSource4Store<TEvent>(EventSourceStore store, object source, string eventName) /*where TEvent:EventHandler<EventArgs>*/ {
			return new EventSource<TEvent>(store, source, eventName);
		}

		// ###



		/// <summary> Resets this instance. Used for unit tests.
		/// </summary>
		internal static void Reset() {
//			s_WeakEvents.Clear();
			StatisticsːRaiseːInvocationCount = 0;
			s_LastCollect = DateTime.MinValue;
		}

	}

}
