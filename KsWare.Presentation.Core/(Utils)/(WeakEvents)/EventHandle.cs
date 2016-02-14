
#if(STATISTICS)
	#define IncludeRegisteredWeakEventStatistics
#endif
#define IncludeRegisteredWeakEventStatistics

using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	internal abstract class EventHandle : IEventHandle {

		#if(IncludeRegisteredWeakEventStatistics)
		internal static long StatisticsːInstancesˑCreated;
		internal static long StatisticsːInstancesˑCurrent;
		internal static long StatisticsːRaiseːInvocationCount;
		#endif

		private WeakReference m_WeakDestination;
		private Delegate m_Handler;
		private string m_DestinationUid;
		private WeakReference m_WeakSourceObject;
		private WeakReference m_WeakEventSource;
		private string m_EventName;
		private int m_IsDisposed;
		private string m_DebugString;
		private bool m_DoNotCallReleased;

		protected EventHandle() {
			#region Statistics (conditional)
#if(IncludeRegisteredWeakEventStatistics)
			Interlocked.Increment(ref StatisticsːInstancesˑCreated);
			Interlocked.Increment(ref StatisticsːInstancesˑCurrent);
#endif
			#endregion			
		}

		protected EventHandle([NotNull] IEventSource eventSource, object destination, Delegate handler, string destinationUid, object source, string eventName) 
			:this(){

			// if(handler==null) throw new ArgumentNullException("handler");
	

			m_WeakDestination  = destination==null?null:new WeakReference(destination);
			m_DestinationUid   = destinationUid;
			m_WeakSourceObject = new WeakReference(source);
			m_WeakEventSource  = new WeakReference(eventSource);
			m_EventName        = eventName;
			m_Handler          = handler;  
		}


		#if(IncludeRegisteredWeakEventStatistics)
		~EventHandle() {
			Interlocked.Decrement(ref StatisticsːInstancesˑCurrent);
		}
		#endif

		void IDisposable.Dispose() { Release(); }

		public void Release() {
			Dispose(true);
			#if(!IncludeRegisteredWeakEventStatistics) //only for statistics we require the finalizer
			GC.SuppressFinalize(this);
			#endif
		}

		internal void ReleaseBySource() {
			m_DoNotCallReleased = true;
			Release();
		}

		protected virtual bool Dispose(bool explicitDispose) {
			if (explicitDispose) {
				if (Interlocked.Exchange(ref m_IsDisposed, 1) > 0) return false;
				if (!m_DoNotCallReleased) {
					var eventsource = (IEventSourceInternal)(IEventSource)m_WeakEventSource.Target;
					if (eventsource != null) { eventsource.Released(this); }					
				}
				m_WeakDestination  = null;
				m_Handler          = null;
				m_DestinationUid   = null;
				m_WeakSourceObject = null;
				m_WeakEventSource  = null;
				m_EventName        = null;
			}
			return true;
		}

		/// <summary> Gets the destination object.
		/// </summary>
		/// <value>The destination object or <c>null</c> when destination is not alive</value>
		[CanBeNull]
		public object Destination {get {if (!m_WeakDestination.IsAlive) return null;try{return m_WeakDestination.Target;}catch(InvalidOperationException){return null;}}}

//		[CanBeNull]
//		public Delegate Handler{get {if (!m_Handler.IsAlive) return null;try{return (Delegate) m_Handler.Target;}catch(InvalidOperationException){return null;}}}

//		public Delegate Handler { get { return m_Handler; } }


		[CanBeNull]
		public string DestinationUid{get { return m_DestinationUid; }}

		/// <summary> Gets the source object.
		/// </summary>
		/// <value>The source object or <c>null</c> when source is not alive</value>
		[CanBeNull]
		public object Source{get {if (!m_WeakSourceObject.IsAlive) return null;try{return m_WeakSourceObject.Target;}catch(InvalidOperationException){return null;}}}
				
		/// <summary> Gets the name of the event.
		/// </summary>
		/// <value>The name of the event.</value>
		[CanBeNull]
		public string EventName{get { return m_EventName; }}

		/// <summary> Gets a value indicating whether this instance is alive.
		/// </summary>
		/// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
		/// <remarks>This instance is alive when both Destination and Source are alive</remarks>
		public bool IsAlive {
			get {
				if(!m_WeakSourceObject.IsAlive) return false;
				if (m_WeakDestination == null) {
					return Handler != null; // !use Handler and not m_Handler
				} else {
					return m_WeakDestination.IsAlive;
				}
			}
		}

		public virtual Delegate Handler { get { return m_Handler; } }

		/// <summary> Raises the event with the specified arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		internal virtual void Raise(object[] args) {

#if(false) // reflection stuff
			if (m_DelegateMethod != null) {
				var target=m_DelegateTarget.Target;
				if(target==null) {Release(); return;}
				var bf = m_DelegateMethod.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
				m_DelegateMethod.Invoke(target, bf | BindingFlags.NonPublic | BindingFlags.Public, null, args, null);
				return;
			}
#endif

			var handler = m_Handler;
			if (handler == null) { Release(); return; }
			
			EventUtil.Invoke(handler,args[0],(EventArgs)args[1],null);

		}

	}

	internal abstract class EventHandle<TEventArgs> : EventHandle {

		protected EventHandle([NotNull] IEventSource eventSource, object destination, Delegate handler, string destinationUid, object source, string eventName)
			:base(eventSource,destination,handler,destinationUid,source,eventName){
		}
	}

	internal class EventHandle4Universal:EventHandle<EventArgs> {

		public EventHandle4Universal([NotNull] IEventSource eventSource, object destination, Delegate handler, string destinationUid, object source, string eventName)
			:base(eventSource,destination,handler,destinationUid,source,eventName){

			if(handler==null) throw new ArgumentNullException("handler");
		}
	}

	#region specialized WeakEventHandler classes


	/// <summary> Handle for <see cref="System.EventHandler">System.EventHandler</see>
	/// </summary>
	internal class EventHandle4SystemEventHandler : EventHandle<EventArgs> {

		private System.EventHandler m_Handler;

		public EventHandle4SystemEventHandler(
			[NotNull] IEventSource eventSource, 
			          object       destination, 
			          EventHandler handler, 
			          string       destinationUid, 
			          object       source, 
			          string       eventName
		): base(
			eventSource   : eventSource, 
			destination   : destination,
			handler       : handler, 
			destinationUid: destinationUid, 
			source        : source,
			eventName     : eventName
		) {
			m_Handler = handler;
		}

		public override Delegate Handler { get { return m_Handler; } }

		internal override void Raise(object[] args) {
			m_Handler(args[0], (EventArgs) args[1]); 
		}

		protected override bool Dispose(bool explicitDispose) {
			if(!base.Dispose(explicitDispose)) return false;
			if (explicitDispose) {
				m_Handler = null;
			}
			return true;
		}
	}

	/// <summary> Handle for <see cref="System.ComponentModel.PropertyChangedEventHandler">System.ComponentModel.PropertyChangedEventHandler</see>
	/// </summary>
	internal class EventHandle4PropertyChangedEventHandler : EventHandle<System.Collections.Specialized.NotifyCollectionChangedEventArgs> {

		private System.ComponentModel.PropertyChangedEventHandler m_Handler;

		public EventHandle4PropertyChangedEventHandler(
			[NotNull] IEventSource eventSource, 
			          object       destination, 
			          System.ComponentModel.PropertyChangedEventHandler handler, 
			          string       destinationUid, 
			          object       source, 
			          string       eventName
		): base(
			eventSource   : eventSource, 
			destination   : destination,
			handler       : handler, 
			destinationUid: destinationUid, 
			source        : source,
			eventName     : eventName
		) {
			m_Handler = handler;
		}

		public override Delegate Handler { get { return m_Handler; } }

		internal override void Raise(object[] args) {
			m_Handler(args[0], (PropertyChangedEventArgs) args[1]); 
		}

		protected override bool Dispose(bool explicitDispose) {
			if(!base.Dispose(explicitDispose)) return false;
			if (explicitDispose) {
				m_Handler = null;
			}
			return true;
		}
	}

		/// <summary> Handle for <see cref="System.Collections.Specialized.NotifyCollectionChangedEventHandler">System.Collections.Specialized.NotifyCollectionChangedEventHandler</see>
	/// </summary>
	internal class EventHandle4NotifyCollectionChangedEventHandler : EventHandle<System.Collections.Specialized.NotifyCollectionChangedEventArgs> {

		private System.Collections.Specialized.NotifyCollectionChangedEventHandler m_Handler;

		public EventHandle4NotifyCollectionChangedEventHandler(
			[NotNull] IEventSource eventSource, 
			          object       destination, 
			          System.Collections.Specialized.NotifyCollectionChangedEventHandler handler, 
			          string       destinationUid, 
			          object       source, 
			          string       eventName
		): base(
			eventSource   : eventSource, 
			destination   : destination,
			handler       : handler, 
			destinationUid: destinationUid, 
			source        : source,
			eventName     : eventName
		) {
			m_Handler = handler;
		}

		public override Delegate Handler { get { return m_Handler; } }

		internal override void Raise(object[] args) {
			m_Handler(args[0], (System.Collections.Specialized.NotifyCollectionChangedEventArgs) args[1]); 
		}

		protected override bool Dispose(bool explicitDispose) {
			if(!base.Dispose(explicitDispose)) return false;
			if (explicitDispose) {
				m_Handler = null;
			}
			return true;
		}
	}

	/// <summary> Handle for <see cref="System.EventHandler{T}">System.EventHandler&lt;TEventArgs&gt;</see>
	/// </summary>
	internal class EventHandle4EventHandlerGeneric<TEventArgs> : EventHandle<TEventArgs> where TEventArgs:EventArgs{

		private System.EventHandler<TEventArgs> m_Handler;

		public EventHandle4EventHandlerGeneric([NotNull] IEventSource eventSource, object destination, System.EventHandler<TEventArgs> handler, string destinationUid, object source, string eventName)
			: base(eventSource, destination, handler, destinationUid, source, eventName) {
			m_Handler = handler;
		}

		public override Delegate Handler { get { return m_Handler; } }

		internal override void Raise(object[] args) {
			m_Handler(args[0], (TEventArgs)args[1]); 
		}

		protected override bool Dispose(bool explicitDispose) {
			if(!base.Dispose(explicitDispose)) return false;
			if (explicitDispose) {
				m_Handler = null;
			}
			return true;
		}
	}

	#endregion
}

