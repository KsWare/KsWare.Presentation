
#if(STATISTICS)
	#define IncludeRegisteredWeakEventStatistics
#endif
#define IncludeRegisteredWeakEventStatistics

using System;
using System.ComponentModel;
using System.Threading;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	internal abstract class EventHandle : IEventHandle {

		#if(IncludeRegisteredWeakEventStatistics)
		internal static long StatisticsːInstancesˑCreated;
		internal static long StatisticsːInstancesˑCurrent;
		internal static long StatisticsːRaiseːInvocationCount;
		#endif

		private WeakReference _weakDestination;
		private Delegate _handler;
		private string _destinationUid;
		private WeakReference _weakSourceObject;
		private WeakReference _weakEventSource;
		private string _eventName;
		private int _isDisposed;
		private bool _doNotCallReleased;

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
	

			_weakDestination  = destination==null?null:new WeakReference(destination);
			_destinationUid   = destinationUid;
			_weakSourceObject = new WeakReference(source);
			_weakEventSource  = new WeakReference(eventSource);
			_eventName        = eventName;
			_handler          = handler;  
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
			_doNotCallReleased = true;
			Release();
		}

		protected virtual bool Dispose(bool explicitDispose) {
			if (explicitDispose) {
				if (Interlocked.Exchange(ref _isDisposed, 1) > 0) return false;
				if (!_doNotCallReleased) {
					var eventsource = (IEventSourceInternal)(IEventSource)_weakEventSource.Target;
					if (eventsource != null) { eventsource.Released(this); }					
				}
				_weakDestination  = null;
				_handler          = null;
				_destinationUid   = null;
				_weakSourceObject = null;
				_weakEventSource  = null;
				_eventName        = null;
			}
			return true;
		}

		/// <summary> Gets the destination object.
		/// </summary>
		/// <value>The destination object or <c>null</c> when destination is not alive</value>
		[CanBeNull]
		public object Destination {get {if (!_weakDestination.IsAlive) return null;try{return _weakDestination.Target;}catch(InvalidOperationException){return null;}}}

//		[CanBeNull]
//		public Delegate Handler{get {if (!_Handler.IsAlive) return null;try{return (Delegate) _Handler.Target;}catch(InvalidOperationException){return null;}}}

//		public Delegate Handler { get { return _Handler; } }


		[CanBeNull]
		public string DestinationUid => _destinationUid;

		/// <summary> Gets the source object.
		/// </summary>
		/// <value>The source object or <c>null</c> when source is not alive</value>
		[CanBeNull]
		public object Source{get {if (!_weakSourceObject.IsAlive) return null;try{return _weakSourceObject.Target;}catch(InvalidOperationException){return null;}}}
				
		/// <summary> Gets the name of the event.
		/// </summary>
		/// <value>The name of the event.</value>
		[CanBeNull]
		public string EventName => _eventName;

		/// <summary> Gets a value indicating whether this instance is alive.
		/// </summary>
		/// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
		/// <remarks>This instance is alive when both Destination and Source are alive</remarks>
		public bool IsAlive {
			get {
				if(!_weakSourceObject.IsAlive) return false;
				if (_weakDestination == null) {
					return Handler != null; // !use Handler and not _Handler
				} else {
					return _weakDestination.IsAlive;
				}
			}
		}

		public virtual Delegate Handler => _handler;

		/// <summary> Raises the event with the specified arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		internal virtual void Raise(object[] args) {

#if(false) // reflection stuff
			if (_DelegateMethod != null) {
				var target=_DelegateTarget.Target;
				if(target==null) {Release(); return;}
				var bf = _DelegateMethod.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
				_DelegateMethod.Invoke(target, bf | BindingFlags.NonPublic | BindingFlags.Public, null, args, null);
				return;
			}
#endif

			var handler = _handler;
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

			if(handler==null) throw new ArgumentNullException(nameof(handler));
		}
	}

	#region specialized WeakEventHandler classes


	/// <summary> Handle for <see cref="System.EventHandler">System.EventHandler</see>
	/// </summary>
	internal class EventHandle4SystemEventHandler : EventHandle<EventArgs> {

		private System.EventHandler _Handler;

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
			_Handler = handler;
		}

		public override Delegate Handler => _Handler;

		internal override void Raise(object[] args) {
			_Handler(args[0], (EventArgs) args[1]); 
		}

		protected override bool Dispose(bool explicitDispose) {
			if(!base.Dispose(explicitDispose)) return false;
			if (explicitDispose) {
				_Handler = null;
			}
			return true;
		}
	}

	/// <summary> Handle for <see cref="System.ComponentModel.PropertyChangedEventHandler">System.ComponentModel.PropertyChangedEventHandler</see>
	/// </summary>
	internal class EventHandle4PropertyChangedEventHandler : EventHandle<System.Collections.Specialized.NotifyCollectionChangedEventArgs> {

		private System.ComponentModel.PropertyChangedEventHandler _Handler;

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
			_Handler = handler;
		}

		public override Delegate Handler => _Handler;

		internal override void Raise(object[] args) {
			_Handler(args[0], (PropertyChangedEventArgs) args[1]); 
		}

		protected override bool Dispose(bool explicitDispose) {
			if(!base.Dispose(explicitDispose)) return false;
			if (explicitDispose) {
				_Handler = null;
			}
			return true;
		}
	}

		/// <summary> Handle for <see cref="System.Collections.Specialized.NotifyCollectionChangedEventHandler">System.Collections.Specialized.NotifyCollectionChangedEventHandler</see>
	/// </summary>
	internal class EventHandle4NotifyCollectionChangedEventHandler : EventHandle<System.Collections.Specialized.NotifyCollectionChangedEventArgs> {

		private System.Collections.Specialized.NotifyCollectionChangedEventHandler _Handler;

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
			_Handler = handler;
		}

		public override Delegate Handler => _Handler;

			internal override void Raise(object[] args) {
			_Handler(args[0], (System.Collections.Specialized.NotifyCollectionChangedEventArgs) args[1]); 
		}

		protected override bool Dispose(bool explicitDispose) {
			if(!base.Dispose(explicitDispose)) return false;
			if (explicitDispose) {
				_Handler = null;
			}
			return true;
		}
	}

	/// <summary> Handle for <see cref="System.EventHandler{T}">System.EventHandler&lt;TEventArgs&gt;</see>
	/// </summary>
	internal class EventHandle4EventHandlerGeneric<TEventArgs> : EventHandle<TEventArgs> where TEventArgs:EventArgs{

		private System.EventHandler<TEventArgs> _Handler;

		public EventHandle4EventHandlerGeneric([NotNull] IEventSource eventSource, object destination, System.EventHandler<TEventArgs> handler, string destinationUid, object source, string eventName)
			: base(eventSource, destination, handler, destinationUid, source, eventName) {
			_Handler = handler;
		}

		public override Delegate Handler => _Handler;

		internal override void Raise(object[] args) {
			_Handler(args[0], (TEventArgs)args[1]); 
		}

		protected override bool Dispose(bool explicitDispose) {
			if(!base.Dispose(explicitDispose)) return false;
			if (explicitDispose) {
				_Handler = null;
			}
			return true;
		}
	}

	#endregion
}

