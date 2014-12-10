
#if(STATISTICS)
	#define IncludeRegisteredWeakEventStatistics
#endif
//#define IncludeRegisteredWeakEventStatistics

using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation {

	/// <summary> Provides event utilities
	/// </summary>
	public static partial class EventUtil {

		partial class WeakEventManager {

			private class RegisteredWeakEvent:IWeakEventHandle {
				
				#if(IncludeRegisteredWeakEventStatistics)
				internal static long StatisticsːNumberOfCreatedInstances;//StatisticsːMethodInvocationːConstructorːCount
				internal static long StatisticsːNumberOfInstances;
				internal static long StatisticsːMethodInvocationːDestructorːCount;
				internal static long StatisticsːRaiseːInvocationCount;
				#endif

				private WeakReference m_WeakDestination;
				private Delegate m_Handler;
				private string m_DestinationUid;
				private WeakReference m_WeakSource;
				private string m_EventName;
				private int m_IsDisposed;
				private string m_DebugString;
				private HandlerType m_HandlerType;

//				public RegisteredWeakEvent(Expression<Action<object, EventArgs>> expression) {} 
//				public RegisteredWeakEvent(EventHandler expression) {} 

				public RegisteredWeakEvent(object destination, Delegate handler, string destinationUid, object source, string eventName)
					: this(false, destination, handler, destinationUid, source, eventName) {
				}

				public RegisteredWeakEvent(bool overidden, object destination, Delegate handler, string destinationUid, object source, string eventName) {
					
					#if(IncludeRegisteredWeakEventStatistics)
					Interlocked.Increment(ref StatisticsːNumberOfCreatedInstances);
					Interlocked.Increment(ref StatisticsːNumberOfInstances);
					#endif

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
					m_WeakDestination= destination==null?null:new WeakReference(destination);
					m_DestinationUid = destinationUid;
					m_WeakSource     = new WeakReference(source);
					m_EventName      = eventName;

					if (!overidden) {
						m_Handler        = handler;  
						m_HandlerType    = GetHandlerType();
					}

				}

				private HandlerType GetHandlerType() {
					if(m_Handler is System.EventHandler                                  ) return HandlerType.SystemEventHandler;
					if(m_Handler is System.EventHandler<EventArgs                       >) return HandlerType.SystemEventHandler1EventArgs;
					if(m_Handler is System.EventHandler<ExecutedEventArgs               >) return HandlerType.SystemEventHandler1ExecutedEventArgs;
					if(m_Handler is System.EventHandler<ValueSettingsChangedEventArgs   >) return HandlerType.SystemEventHandler1ValueSettingsChangedEventArgs;
					if(m_Handler is System.EventHandler<UserFeedbackEventArgs           >) return HandlerType.SystemEventHandler1UserFeedbackEventArgs;
					if(m_Handler is System.EventHandler<BusinessPropertyChangedEventArgs>) return HandlerType.SystemEventHandler1BusinessPropertyChangedEventArgs;
//	class			if(m_Handler is System.EventHandler<TreeChangedEventArgs            >) return HandlerType.SystemEventHandler1TreeChangedEventArgs;
//	class			if(m_Handler is System.EventHandler<ValueChangedEventArgs           >) return HandlerType.SystemEventHandler1ValueChangedEventArgs;
					if(m_Handler is System.EventHandler<PropertyChangedEventArgs        >) return HandlerType.SystemEventHandler1PropertyChangedEventArgs;
				
					var t = m_Handler.GetType();
					var name = t.Name;
					if (t.IsGenericType && t.GetGenericTypeDefinition()==typeof(System.EventHandler<>)) {//TODO speedup
						var p = t.GetGenericArguments();
						if (!p[0].IsGenericType) {
							// sample: EventHandler<OtherEventArgs>
							
						} 
// TODO SystemEventHandler1ValueChangedEventArgs1
//						else if (p[0].GetGenericTypeDefinition() == typeof (ValueChangedEventArgs<>)) { //TODO speedup
//							// sample: EventHandler<ValueChangedEventArgs<Int32>>
//							var pP0 = t.GetGenericArguments();
//							m_HandlerTypeGenericArgumentType = pP0[0];
//							return HandlerType.SystemEventHandler1ValueChangedEventArgs1;
//						}
					}

					return HandlerType.Unknown;
				}

				#if(IncludeRegisteredWeakEventStatistics)
				~RegisteredWeakEvent() {
					Interlocked.Increment(ref StatisticsːMethodInvocationːDestructorːCount);
					Interlocked.Decrement(ref StatisticsːNumberOfInstances);
				}
				#endif

				void IDisposable.Dispose() { Release(); }

				public void Release() {
					Dispose(true);
					#if(!IncludeRegisteredWeakEventStatistics) //only for statistics we requiere the finalizer
					GC.SuppressFinalize(this);
					#endif
				}

				private void Dispose(bool explicitDispose) {
					if (explicitDispose) {
						if (Interlocked.Exchange(ref m_IsDisposed, 1) > 0) return;
						lock(s_WeakEvents) s_WeakEvents.RemoveAll(x=>x.WeakEventHandle==this);
						m_WeakDestination = null;
						m_Handler         = null;
						m_DestinationUid  = null;
						m_WeakSource      = null;
						m_EventName       = null;
					}
				}

				/// <summary> Gets the destination object.
				/// </summary>
				/// <value>The destination object or <c>null</c> when destination is not alive</value>
				[CanBeNull]
				public object Destination {get {if (!m_WeakDestination.IsAlive) return null;try{return m_WeakDestination.Target;}catch(InvalidOperationException){return null;}}}

//				[CanBeNull]
//				public Delegate Handler{get {if (!m_Handler.IsAlive) return null;try{return (Delegate) m_Handler.Target;}catch(InvalidOperationException){return null;}}}

//				public Delegate Handler { get { return m_Handler; } }


				[CanBeNull]
				public string DestinationUid{get { return m_DestinationUid; }}

				/// <summary> Gets the source object.
				/// </summary>
				/// <value>The source object or <c>null</c> when destination is not alive</value>
				[CanBeNull]
				public object Source{get {if (!m_WeakSource.IsAlive) return null;try{return m_WeakSource.Target;}catch(InvalidOperationException){return null;}}}

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
						if(!m_WeakSource.IsAlive) return false;
						if (m_WeakDestination == null) {
							return m_Handler != null;
						} else {
							return m_WeakDestination.IsAlive;
						}
					}
				}

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
//					Debug.WriteLine(string.Format("=>Raise event: #{0} {1}",++InvokationCount, DebugUtil.FormatTypeName(handler)));

					// handler.DynamicInvoke(args); 
					// ^- is very slow!

					// handler.Method.Invoke(handler.Target, args);
					// ^- is also slow

					switch (m_HandlerType) {
						case HandlerType.SystemEventHandler: 
							((System.EventHandler) handler)(args[0], (EventArgs) args[1]); 
							break;
						case HandlerType.SystemEventHandler1EventArgs: 
							((System.EventHandler<EventArgs>) handler)(args[0], (EventArgs) args[1]); 
							break;
						case HandlerType.SystemEventHandler1PropertyChangedEventArgs: 
							((System.EventHandler<PropertyChangedEventArgs>) handler)(args[0], (PropertyChangedEventArgs) args[1]); 
							break;
						case HandlerType.SystemEventHandler1ExecutedEventArgs: 
							((System.EventHandler<ExecutedEventArgs>) handler)(args[0], (ExecutedEventArgs) args[1]); 
							break;
						case HandlerType.SystemEventHandler1ValueSettingsChangedEventArgs: 
							((System.EventHandler<ValueSettingsChangedEventArgs>) handler)(args[0], (ValueSettingsChangedEventArgs) args[1]); 
							break;
						case HandlerType.SystemEventHandler1UserFeedbackEventArgs: 
							((System.EventHandler<UserFeedbackEventArgs>) handler)(args[0], (UserFeedbackEventArgs) args[1]); 
							break;
						case HandlerType.SystemEventHandler1BusinessPropertyChangedEventArgs: 
							((System.EventHandler<BusinessPropertyChangedEventArgs>) handler)(args[0], (BusinessPropertyChangedEventArgs) args[1]); 
							break;
// class available:		case HandlerType.SystemEventHandler1TreeChangedEventArgs: 
//							((System.EventHandler<TreeChangedEventArgs>) handler)(args[0], (TreeChangedEventArgs) args[1]); 
//							break;
//	class available:	case HandlerType.SystemEventHandler1ValueChangedEventArgs: 
//							((System.EventHandler<ValueChangedEventArgs>) handler)(args[0], (ValueChangedEventArgs) args[1]); 
//							break;
						default:
							handler.Method.Invoke(handler.Target, args);
							break;
					}

				}

			}

			#region specialized RegisteredWeakEvent classes
			
			private class RegisteredWeakEvent1TreeChangedEvent : RegisteredWeakEvent {

				private System.EventHandler<TreeChangedEventArgs> m_Handler;

				public RegisteredWeakEvent1TreeChangedEvent(object destination, System.EventHandler<TreeChangedEventArgs> handler, string destinationUid, object source, string eventName)
					: base(true, destination, null, destinationUid, source, eventName) {
					m_Handler = handler;
				}

				public Delegate Handler { get { return m_Handler; } }

				public override void Raise(object[] args) {
					m_Handler(args[0], (TreeChangedEventArgs) args[1]); 
				}

			}

			private class RegisteredWeakEvent1ValueChangedEvent : RegisteredWeakEvent {

				private System.EventHandler<ValueChangedEventArgs> m_Handler;

				public RegisteredWeakEvent1ValueChangedEvent(object destination, System.EventHandler<ValueChangedEventArgs> handler, string destinationUid, object source, string eventName)
					: base(true, destination, null, destinationUid, source, eventName) {
					m_Handler = handler;
				}

				public Delegate Handler { get { return m_Handler; } }

				public override void Raise(object[] args) {
					m_Handler(args[0], (ValueChangedEventArgs) args[1]); 
				}

			}

			#endregion

			public enum HandlerType {
				None,
				Unknown=None,
				SystemEventHandler,
				SystemEventHandler1,
				SystemEventHandler1EventArgs,
				SystemEventHandler1PropertyChangedEventArgs,
				SystemEventHandler1ExecutedEventArgs,
				SystemEventHandler1ValueSettingsChangedEventArgs,
				SystemEventHandler1UserFeedbackEventArgs,
				SystemEventHandler1BusinessPropertyChangedEventArgs,
//				SystemEventHandler1TreeChangedEventArgs,				// implemented as class
//				SystemEventHandler1ValueChangedEventArgs,				// implemented as class
				SystemEventHandler1ValueChangedEventArgs1
			}
		}

	}

}