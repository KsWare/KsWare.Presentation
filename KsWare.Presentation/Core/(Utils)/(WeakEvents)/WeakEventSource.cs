using System;
using System.Linq.Expressions;

namespace KsWare.Presentation {

	/// <summary> Provides event utilities
	/// </summary>
	public static partial class EventUtil {


		/// <summary> [OBSOLETE] Replaced by <see cref="WeakEventSource{TEvent}"/> </summary>
		[Obsolete("Replaced by WeakEventSource<TEvent>",true)]
		private class WeakEventSource:IWeakEventSource {

			private readonly WeakReference m_Source;
			private readonly string m_EventName;

			public WeakEventSource(object source, string eventName) {
				m_Source = new WeakReference(source);
				m_EventName = eventName;
			}

			public string EventName { get { return m_EventName; } }

			public object Source {get { return !m_Source.IsAlive?null:m_Source.Target; }}

			public IWeakEventHandle Register(object destination, string uniqueId, EventHandler handler) {
				return EventUtil.WeakEventManager.Register(destination,handler,uniqueId,m_Source.Target,m_EventName);
			}

			public void Release(object destination, string uniqueId) {
				EventUtil.WeakEventManager.Release(destination,uniqueId);
			}
		}

		private class WeakEventSource<TEvent>:IWeakEventSource<TEvent> /*where TEvent:EventHandler<EventArgs>*/ {

			private readonly WeakReference m_WeakSource;
			private readonly string m_EventName;

			public WeakEventSource(object source, string eventName) {
				m_WeakSource = new WeakReference(source);
				m_EventName = eventName;
			}

			public string EventName { get { return m_EventName; } }

			public object Source {get { return !m_WeakSource.IsAlive?null:m_WeakSource.Target; }}

			//maybe obsolete because destination could be come from handler.Target and uniqueId from handler.Method.ToString()
			public IWeakEventHandle Register(object destination, string uniqueId, TEvent handler) {
				return EventUtil.WeakEventManager.Register(destination,(Delegate) (object) handler,uniqueId,m_WeakSource.Target,m_EventName);
			}

			public IWeakEventHandle Register(TEvent handler) {
				return EventUtil.WeakEventManager.Register( (Delegate) (object) handler,m_WeakSource.Target,m_EventName);
			}

			public IWeakEventHandle RegisterExpression(Expression<Action<object, EventArgs>> expression) {
				return EventUtil.WeakEventManager.RegisterExpression(expression);
			}

			/// <summary> [EXPERIMENTAL] 
			/// </summary>
			/// <param name="handler"></param>
			/// <returns><see cref="IWeakEventHandle"/></returns>
			/// <remarks>You have to store the return value until you want to receive events.</remarks>
			public IWeakEventHandle RegisterWeak(TEvent handler) {
				var h = (Delegate) (object) handler;
				return EventUtil.WeakEventManager.RegisterAsDisposable(h,m_WeakSource.Target,m_EventName);
			}

			public IWeakEventHandle RegisterAsDisposable<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs:EventArgs {
				var h = (Delegate) (object) handler;
				return EventUtil.WeakEventManager.RegisterAsDisposable(h,m_WeakSource.Target,m_EventName);
			}

			public void Release(object destination, string uniqueId) {
				EventUtil.WeakEventManager.Release(destination,uniqueId);
			}

//			public event TEvent E;

			/// <summary> Adds an event handler.
			/// </summary>
			/// <value>The event handler</value>
			/// <remarks>
			/// This is an alternate (property like) syntax to the <see cref="Register(TEvent)"/> method
			/// </remarks>
			/// <example>Adding a lambda:<code>MyModel.PropertyChangedEvent.add=(s,e) => {/* do anyting*/};</code></example>
			/// <example>Adding a method:<code>MyModel.PropertyChangedEvent.add=AtPropertyChanged;
			/// private void AtPropertyChanged(object sender, EventArgs e) {/*do anyting*/}
			/// </code></example>
			public TEvent add { set { Register(value); } }
//			public TEvent remove { set { Release(value); } }

			/// <summary> Adds an event handler.
			/// </summary>
			/// <value>The event handler</value>
			/// <remarks>
			/// This is an alternate (property operator like) syntax to the <see cref="Register(TEvent)"/> method
			/// </remarks>
			/// <example>Adding a lambda:<code>MyModel.PropertyChangedEvent.ː=(s,e) => {/*do anyting*/};</code></example>
			/// <example>Adding a method:<code>MyModel.PropertyChangedEvent.ː=AtPropertyChanged;
			/// private void AtPropertyChanged(object sender, EventArgs e) {/*do anyting*/}
			/// </code></example>
			public TEvent ː { set { Register(value); } }
		}	

	}

}