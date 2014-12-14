﻿
#if(STATISTICS)
	#define IncludeRegisteredWeakEventStatistics
#endif
#define IncludeRegisteredWeakEventStatistics

using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation {

	internal class EventHandle:IEventHandle {
				
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

//		public RegisteredWeakEvent(Expression<Action<object, EventArgs>> expression) {} 
//		public RegisteredWeakEvent(EventHandler expression) {} 

		public EventHandle([NotNull] IEventSource eventSource, object destination, Delegate handler, string destinationUid, object source, string eventName)
			: this(false, eventSource, destination, handler, destinationUid, source, eventName) {
		}

		public EventHandle(bool overidden, [NotNull] IEventSource eventSource, object destination, Delegate handler, string destinationUid, object source, string eventName) {

			#region Statistics (conditional)
#if(IncludeRegisteredWeakEventStatistics)
			Interlocked.Increment(ref StatisticsːInstancesˑCreated);
			Interlocked.Increment(ref StatisticsːInstancesˑCurrent);
#endif
			#endregion

			if (!overidden) {
				if(handler==null) throw new ArgumentNullException("handler");
			}
					
#if(false) // reflection stuff
			if(handler.Target==null) throw new NotImplementedException("handler.Target is null");
			// handler.Target is null for lambdas, at present we do not support this

			m_DelegateTarget=new WeakReference(handler.Target);
			var m = handler.Method;
			var t = handler.Target.GetType();
			var b = m.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
			var p = m.GetParameters().Select(x => x.ParameterType).ToArray();
			m_DelegateMethod=t.GetMethod(m.Name, b | BindingFlags.NonPublic | BindingFlags.Public,null,p,null);
					

			if (destination == null) {
				// += syntax used 
//						var stackFrame = new StackFrame(2);
//						destination = stackFrame.GetMethod().ReflectedObject??;
				destination = handler.Target;
			}
#endif
			m_WeakDestination  = destination==null?null:new WeakReference(destination);
			m_DestinationUid   = destinationUid;
			m_WeakSourceObject = new WeakReference(source);
			m_WeakEventSource  = new WeakReference(eventSource);
			m_EventName        = eventName;

			if (!overidden) {
				m_Handler        = handler;  
			}

		}

		#if(IncludeRegisteredWeakEventStatistics)
		~EventHandle() {
			Interlocked.Decrement(ref StatisticsːInstancesˑCurrent);
		}
		#endif

		void IDisposable.Dispose() { Release(); }

		public void Release() {
			Dispose(true);
			#if(!IncludeRegisteredWeakEventStatistics) //only for statistics we requiere the finalizer
			GC.SuppressFinalize(this);
			#endif
		}

		internal void ReleaseBySource() {
			m_DoNotCallReleased = true;
			Release();
		}

		private void Dispose(bool explicitDispose) {
			if (explicitDispose) {
				if (Interlocked.Exchange(ref m_IsDisposed, 1) > 0) return;
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
					return Handler != null; // !use Handle and not m_Handle
				} else {
					return m_WeakDestination.IsAlive;
				}
			}
		}

		public virtual Delegate Handler { get { return m_Handler; } }

		/// <summary> Raises the event with the specified arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public virtual void Raise(object[] args) {

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
//			Debug.WriteLine(string.Format("=>Raise event: #{0} {1}",++InvokationCount, DebugUtil.FormatTypeName(handler)));
			
			EventUtil.Invoke(handler,args[0],(EventArgs)args[1],null);

		}

	}

	#region specialized WeakEventHandler classes
			
			
	internal class EventHandler4SystemEventHandler : EventHandle {

		private System.EventHandler m_Handler;

		public EventHandler4SystemEventHandler(
			[NotNull] IEventSource eventSource, 
			          object       destination, 
			          EventHandler handler, 
			          string       destinationUid, 
			          object       source, 
			          string       eventName
		): base(
			overidden     : true, 
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

		public override void Raise(object[] args) {
			m_Handler(args[0], (EventArgs) args[1]); 
		}
	}

	internal class EventHandler4EventHandlerGeneric<TEventArgs> : EventHandle where TEventArgs:EventArgs{

		private System.EventHandler<TEventArgs> m_Handler;

		public EventHandler4EventHandlerGeneric([NotNull] IEventSource eventSource, object destination, System.EventHandler<TEventArgs> handler, string destinationUid, object source, string eventName)
			: base(true, eventSource, destination, handler, destinationUid, source, eventName) {
			m_Handler = handler;
		}

		public override Delegate Handler { get { return m_Handler; } }

		public override void Raise(object[] args) {
			m_Handler(args[0], (TEventArgs)args[1]); 
		}
	}

	#endregion
}

