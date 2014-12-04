using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation {

	/// <summary> Provides event utilities
	/// </summary>
	public static partial class EventUtil {

		//TODO KUX Documentation

		// Register/RegisterSource		Registers an event handler / an event source
		// Raise						Raises an event
		// Release/ReleaseSource		Releases an event handler / an event source
		// Collect						Collect and remove event handlers / event sources and which are garbaged

		/// <summary> Manages weak events
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
		public static partial class WeakEventManager {

			private static readonly List<Container> s_WeakEvents =new List<Container>();
			private static DateTime s_LastCollect;
			private static TimeSpan s_MinCollectInterval=TimeSpan.FromMilliseconds(1000);

			internal static long StatisticsːRaiseːInvocationCount;

			/// <summary> Gets the number of registered weak event handlers.
			/// </summary>
			/// <value> The number of registered weak event handlers </value>
			public static int Count {get {lock(s_WeakEvents) return s_WeakEvents.Count;}}

			public static IWeakEventHandle Register(object destination, Delegate handler, string destinationUid, object source, string eventName) {
				CollectIfRequired();
				var handle = new RegisteredWeakEvent(destination, handler, destinationUid, source, eventName);
				lock(s_WeakEvents) s_WeakEvents.Add(new Container(handle,true));
				return handle;
			}

			public static IWeakEventHandle Register(Delegate handler, object source, string eventName) {
				CollectIfRequired();
				var destination = handler.Target;
				var destinationUid = handler.Method.ToString();// GetHashCode
				var handle = new RegisteredWeakEvent(destination, handler, destinationUid, source, eventName);
				lock(s_WeakEvents) s_WeakEvents.Add(new Container(handle,true));
				return handle;
			}

			public static IWeakEventHandle RegisterExpression(Expression<Action<object, EventArgs>> expression) {
//				var x=new Expression<Action<object, EventArgs>>();
//				x.Update()
				//expression.GetType().FullName	        "System.Linq.Expressions.Expression`1[[System.Action`2[[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.EventArgs, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"	string
				//expression.Body
				//+		GetType().FullName	"System.Linq.Expressions.InstanceMethodCallExpressionN"	string
				//+		Method				{Void UpdateComponent()}	System.Reflection.MethodInfo {System.Reflection.RuntimeMethodInfo}
				//+		Object				{value(ProDevis.Monitor.MessageList.CanMessageItem)}	System.Linq.Expressions.Expression {System.Linq.Expressions.ConstantExpression}
				//new System.Linq.Expressions.MethodCallExpression()

				var action = expression.Compile();
				action.Invoke(null,null);
				return null;
//				var handle = new RegisteredWeakEvent(destination, handler, destinationUid, source, eventName);
//				lock(s_WeakEvents) s_WeakEvents.Add(new Container(handle,true));
//				return handle;
			}

			/// <summary> [EXPERIMENTAL] 
			/// </summary>
			/// <param name="handler"></param>
			/// <param name="source"></param>
			/// <param name="eventName"></param>
			/// <returns></returns>
			public static IWeakEventHandle RegisterAsDisposable(Delegate handler, object source, string eventName) {
				CollectIfRequired();

				RegisteredWeakEvent handle;
				if(handler is EventHandler<TreeChangedEventArgs>)
					handle = new RegisteredWeakEvent1TreeChangedEvent(null, (EventHandler<TreeChangedEventArgs>)handler, null, source, eventName);
				else 
					handle = new RegisteredWeakEvent(null, handler, null, source, eventName);
				lock(s_WeakEvents) s_WeakEvents.Add(new Container(handle,false));
				return handle;
			}

			[Obsolete("Use overload!",true)]
			public static void Raise(IWeakEventSource source, EventArgs e) {
				var wes = (WeakEventSource) source;
				RaiseInternal(wes.Source,wes.EventName,new object[]{wes.Source,e});
			}

			public static void Raise<TEventHandler>(IWeakEventSource<TEventHandler> source, EventArgs e)  {
				var wes = (WeakEventSource<TEventHandler>) source;
				RaiseInternal(wes.Source,wes.EventName,new object[]{wes.Source,e});
			}

			// _ => MyEventProperty
			[Obsolete("Slow!",true)]
			public static void Raise<T>(Lazy<WeakEventPropertyStore> lazyWeakEventProperties, Expression<Func<object, IWeakEventSource<T>>> eventPropertyExpression, EventArgs e) /*where T:EventArgs*/ {
				if(!lazyWeakEventProperties.IsValueCreated) return;
				var weakEventPropertyStore = lazyWeakEventProperties.Value;
				if(weakEventPropertyStore.Count==0) return;
				var wes=(WeakEventSource<T>)weakEventPropertyStore.TryGet(eventPropertyExpression);
				if(wes==null/*No one has accessed the event propert*/) return;
				RaiseInternal(wes.Source,wes.EventName,new object[]{wes.Source,e});
			}

			// () => MyEventProperty
			[Obsolete("Slow!",true)]
			public static void Raise<T>(Lazy<WeakEventPropertyStore> lazyWeakEventProperties, Expression<Func<IWeakEventSource<T>>> eventPropertyExpression, EventArgs e) /*where T:EventArgs*/{
				if(!lazyWeakEventProperties.IsValueCreated) return;
				var weakEventPropertyStore = lazyWeakEventProperties.Value;
				if(weakEventPropertyStore.Count==0) return;
				var wes=(WeakEventSource<T>)weakEventPropertyStore.TryGet(eventPropertyExpression);
				if(wes==null/*No one has accessed the event propert*/) return;
				RaiseInternal(wes.Source,wes.EventName,new object[]{wes.Source,e});
			}

			public static void Raise<T>(Lazy<WeakEventPropertyStore> lazyWeakEventProperties, string eventName, EventArgs e) /*where T:EventArgs*/{
				if(!lazyWeakEventProperties.IsValueCreated) return;
				var weakEventPropertyStore = lazyWeakEventProperties.Value;
				if(weakEventPropertyStore.Count==0) return;
				var wes=(WeakEventSource<T>)weakEventPropertyStore.TryGet<T>(eventName);
				if(wes==null/*No one has accessed the event propert*/) return;
				RaiseInternal(wes.Source,wes.EventName,new object[]{wes.Source,e});
			}

			private static void RaiseInternal(object source, string eventName, object[] args) {
				Interlocked.Increment(ref StatisticsːRaiseːInvocationCount);
				var raise = new List<RegisteredWeakEvent>();

				lock (s_WeakEvents) {
					var remove = new List<Container>();
					foreach (var container in s_WeakEvents) {
						var handle = container.WeakEventHandle;
						if (handle == null) { remove.Add(container); continue; }
						if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}

						if(handle.EventName==eventName && handle.Source==source) raise.Add(handle);
					}
					foreach (var bEvent in remove) {bEvent.Dispose();}
				}

				foreach (var bEvent in raise) {
					bEvent.Raise(args);
				}
			}

			/// <summary> Releases all events for the specified destination and id
			/// </summary>
			/// <param name="destination">The event destination.</param>
			/// <param name="uid">A string (unique for the destination) identifying the event</param>
			public static void Release(object destination, string uid) {
				var release = new List<RegisteredWeakEvent>();
				lock (s_WeakEvents) {
					var remove = new List<Container>();
					foreach (var container in s_WeakEvents) {
						var handle = container.WeakEventHandle;
						if (handle == null) { remove.Add(container); continue; }
						if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}

						if (handle.DestinationUid == uid && handle.Destination == destination) release.Add(handle);
					}
					foreach (var container in remove) s_WeakEvents.Remove(container);
					foreach (var bEvent in release) bEvent.Release();
				}
			}

			/// <summary> Releases all events for the specified source
			/// </summary>
			/// <param name="source">The source</param>
			/// <remarks>Event source should call this on Dispose</remarks>
			public static void ReleaseSource(object source) {
				var release = new List<RegisteredWeakEvent>();
				lock (s_WeakEvents) {
					var remove = new List<Container>();
					foreach (var container in s_WeakEvents) {
						var handle = container.WeakEventHandle;
						if (handle == null) { remove.Add(container); continue; }
						if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}

						if(handle.Source==source) release.Add(handle);
					}
					foreach (var container in remove) s_WeakEvents.Remove(container);
					foreach (var bEvent in release) bEvent.Release();
				}
			}

			/// <summary> Releases all events for the specified destination
			/// </summary>
			/// <param name="destination">The destination</param>
			/// <remarks>Event listener should call this on Dispose</remarks>
			public static void ReleaseDestination(object destination) {
				var release = new List<RegisteredWeakEvent>();
				lock (s_WeakEvents) {
					var remove = new List<Container>();
					foreach (var container in s_WeakEvents) {
						var handle = container.WeakEventHandle;
						if (handle == null) { remove.Add(container); continue; }
						if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}

						if(handle.Destination==destination) release.Add(handle);
					}
					foreach (var container in remove) s_WeakEvents.Remove(container);
					foreach (var bEvent in release) {bEvent.Release();}
				}
			}


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

				if (gcCollect) {
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
				}

				var remove = new List<Container>();
				lock (s_WeakEvents) {
					foreach (var container in s_WeakEvents) {
						var handle = container.WeakEventHandle;
						if (handle == null) { remove.Add(container); continue; }
						if(!handle.IsAlive) { container.Dispose(); remove.Add(container);}
					}
					foreach (var container in remove) {
						container.Dispose();
						s_WeakEvents.Remove(container);
					}
				}
				s_LastCollect = DateTime.Now;
			}


			[Obsolete("Use overload!",true)]
			public static IWeakEventSource RegisterSource(object source, string eventName) {
				return new WeakEventSource(source, eventName);
			}

			[Obsolete("Use property initializer: public IWeakEventSource<EventHandler> MyPropertyChangedEvent { get { return WeakEventProperties.Get(() => MyPropertyChangedEvent); } }")]
			public static IWeakEventSource<TEvent> RegisterSource<TEvent>(object source, string eventName) /*where TEvent:EventHandler<EventArgs>*/ {
				return new WeakEventSource<TEvent>(source, eventName);
			}

			internal static IWeakEventSource<TEvent> RegisterSource_CalledByStore<TEvent>(object source, string eventName) /*where TEvent:EventHandler<EventArgs>*/ {
				return new WeakEventSource<TEvent>(source, eventName);
			}

			// ###


			/// <summary> Internal container to hold the event handler or a weak reference to the event handler
			/// </summary>
			private sealed class Container {

				private RegisteredWeakEvent m_WeakEventHandle;
				private WeakReference m_WeakEventHandleWeakRef;

				/// <summary> Initializes a new instance of the <see cref="Container"/> class.
				/// </summary>
				/// <param name="weakEventHandle">The weak event handle.</param>
				/// <param name="keepAlive">if set to <c>true</c> the <see cref="RegisteredWeakEvent"/> is kept alive.</param>
				public Container(RegisteredWeakEvent weakEventHandle, bool keepAlive) {
					if (keepAlive) {
						m_WeakEventHandle = weakEventHandle;
					} else {
						m_WeakEventHandleWeakRef = new WeakReference(weakEventHandle);
					}
					KeepAlive = keepAlive;
				}

				/// <summary> Gets the weak event handle.
				/// </summary>
				/// <value>a reference to <see cref="RegisteredWeakEvent"/> or <c>null</c> if the <see cref="RegisteredWeakEvent"/> has been garbage collected or container disposed.</value>
				public RegisteredWeakEvent WeakEventHandle { get { return  (m_WeakEventHandle ?? (m_WeakEventHandleWeakRef!=null ? (RegisteredWeakEvent) m_WeakEventHandleWeakRef.Target : null ) ); } }

				/// <summary> Gets a value indicating whether the <see cref="RegisteredWeakEvent"/> is kept alive.
				/// </summary>
				/// <value><c>true</c> if a reference to <see cref="RegisteredWeakEvent"/> is holded by this instance; otherwise, <c>false</c> if a <see cref="WeakReference"/> to <see cref="RegisteredWeakEvent"/> ist used.</value>
				public bool KeepAlive { get; private set; }

				/// <summary> Disposes this container.
				/// </summary>
				/// <remarks>Does not dispose the stored <see cref="RegisteredWeakEvent"/>. Only the reference to it is removed.</remarks>
				public void Dispose() {
					m_WeakEventHandle = null;
					m_WeakEventHandleWeakRef = null;
				}

			}

			/// <summary> Resets this instance. Used for unit tests.
			/// </summary>
			internal static void Reset() {
				s_WeakEvents.Clear();
				StatisticsːRaiseːInvocationCount = 0;
				s_LastCollect = DateTime.MinValue;
			}

		}


	}

	/// <summary> Interface for weak event handle
	/// </summary>
	public interface IWeakEventHandle:IDisposable {

		/// <summary> Releases this weak event.
		/// </summary>
		void Release();
	}

	/// <summary> Interface for weak event source 
	/// </summary>
	public interface IWeakEventSource {

		/// <summary> Gets the name of the event.
		/// </summary>
		/// <value>
		/// The name of the event.
		/// </value>
		string EventName { get; }
	}

	/// <summary> Interface for strong typed weak event source 
	/// </summary>
	/// <typeparam name="TEventHandler">The type of the event.</typeparam>
	/// <example> Definition of the event source property:
	/// <code> public IWeakEventSource&lt;EventHandler&lt;ViewModelPropertyChangedEventArgs>> PropertyChangedEvent { get { return WeakEventProperties.Get(()=>PropertyChangedEvent); } }</code>
	/// The <see cref="ObjectVM.WeakEventProperties"/> property is available in all <see cref="ObjectVM"/> and <see cref="ObjectBM"/> classes.</example>
	public interface IWeakEventSource<TEventHandler>:IWeakEventSource  {

		/// <summary> Registers an event handler.
		/// </summary>
		/// <param name="destination">The destination.</param>
		/// <param name="uniqueId">The unique id.</param>
		/// <param name="handler">The handler.</param>
		/// <remarks>The <paramref name="uniqueId"/> is used to release the event handler.</remarks>
		IWeakEventHandle Register(object destination, string uniqueId, TEventHandler handler);

		/// <summary> Registers an event handler.
		/// </summary>
		/// <param name="handler">The event handler to register.</param>
		/// <returns>IWeakEventHandle.</returns>
		IWeakEventHandle Register(TEventHandler handler);

		IWeakEventHandle RegisterExpression(Expression<Action<object, EventArgs>> expression);

		/// <summary> Registers a weak event handler.
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <returns>The weak event handle.</returns>
		IWeakEventHandle RegisterWeak(TEventHandler handler);

		/// <summary> Releases an event handler with the specified id.
		/// </summary>
		/// <param name="destination">The destination.</param>
		/// <param name="uniqueId">The unique id.</param>
		void Release(object destination, string uniqueId);

		/// <summary> Adds an event handler.
		/// </summary>
		/// <value>The event handler</value>
		/// <remarks>
		/// This is an alternate (event add like <c>+=</c> ) syntax to the <see cref="Register(TEventHandler)"/> method
		/// </remarks>
		/// <example>Adding a lambda:<code>MyModel.PropertyChangedEvent.add=(s,e) => {/* do anyting*/};</code></example>
		/// <example>Adding a method:<code>MyModel.PropertyChangedEvent.add=AtPropertyChanged;
		/// private void AtPropertyChanged(object sender, EventArgs e) {/*do anyting*/}
		/// </code></example>
		TEventHandler add { set; }

		/// <summary> Adds an event handler.
		/// </summary>
		/// <value>The event handler</value>
		/// <remarks>
		/// This is an alternate (event add like <c>+=</c> ) syntax to the <see cref="Register(TEventHandler)"/> method
		/// </remarks>
		/// <example>Adding a lambda:<code>MyModel.PropertyChangedEvent.ː=(s,e) => {/*do anyting*/};</code></example>
		/// <example>Adding a method:<code>MyModel.PropertyChangedEvent.ː=AtPropertyChanged;
		/// private void AtPropertyChanged(object sender, EventArgs e) {/*do anyting*/}
		/// </code></example>
		TEventHandler ː { set; }

//		TEvent remove { set; }
//
//		TEvent ｰ { set; }

	}

}
