using System;
using System.Collections.Generic;
using System.Threading;

namespace KsWare.Presentation {

	internal abstract class EventSource {

		internal static int StatisticsːInstancesˑCreated;
		internal static int StatisticsːInstancesˑCurrent;

		protected readonly WeakReference m_WeakSource;
		protected readonly string m_EventName;
		private readonly WeakReference m_WeakStore;
		protected readonly List<EventContainer> m_EventHandleContainers =new List<EventContainer>();

		protected EventSource(EventSourceStore store, object sourceObject, string eventName) {
			Interlocked.Increment(ref StatisticsːInstancesˑCreated);
			Interlocked.Increment(ref StatisticsːInstancesˑCurrent);

			m_WeakStore  = store!=null ? new WeakReference(store) : null;
			m_WeakSource = new WeakReference(sourceObject);
			m_EventName = eventName;
		}

		~EventSource() {
			Interlocked.Decrement(ref StatisticsːInstancesˑCurrent);
		}

		/// <summary> Gets the name of the event.
		/// </summary>
		/// <value>The name of the event.</value>
		public string EventName { get { return m_EventName; } }

		/// <summary> Gets the source object.
		/// </summary>
		/// <value>The source.</value>
		public object Source {get { return m_WeakSource.Target; }}

		internal List<EventContainer> GetContainers() { return m_EventHandleContainers; }
	}

	internal class EventSource<TEvent>:EventSource,IEventSource<TEvent>,IEventSourceInternal /*where TEvent:EventHandler<EventArgs>*/ {

		public EventSource(object source, string eventName) 
			:this(null,source,eventName){}

		/// <summary> Initializes a new instance of the <see cref="EventSource{TEvent}"/> class.
		/// </summary>
		/// <param name="store">The store which holds all event sources from source object.</param>
		/// <param name="sourceObject">The source object which provides this event.</param>
		/// <param name="eventName">The name of the event.</param>
		public EventSource(EventSourceStore store, object sourceObject, string eventName)
			:base(store,sourceObject, eventName) {
		}

		//[NotNull] IWeakEventSource weakEventSource, object destination, Delegate handler, string destinationUid, object source, string eventName
		public IEventHandle Register(object destination, string uniqueId, TEvent handler) {
			var handle=EventManager.Register(
				eventSource    : this, 
				destination    : destination,
				handler        : (Delegate) (object) handler,
				destinationUid : uniqueId,
				sourceObject   : m_WeakSource.Target,
				eventName      : m_EventName
			);
			lock (m_EventHandleContainers) m_EventHandleContainers.Add(handle);
			return handle.EventHandle;
		}

		public IEventHandle Register(TEvent handler) {
			var handle= EventManager.Register(
				eventSource    : this, 
				handler        : (Delegate) (object) handler,
				sourceObject   : m_WeakSource.Target,
				eventName      : m_EventName
			);
			lock (m_EventHandleContainers) m_EventHandleContainers.Add(handle);
			return handle.EventHandle;
		}

		/// <summary> [EXPERIMENTAL] 
		/// </summary>
		/// <param name="handler"></param>
		/// <returns><see cref="IEventHandle"/></returns>
		/// <remarks>You have to store the return value while you want to receive events.</remarks>
		public IEventHandle RegisterWeak(TEvent handler) {
			var h = (Delegate) (object) handler;
			var handle= EventManager.RegisterWeak(this, h,m_WeakSource.Target,m_EventName);
			lock (m_EventHandleContainers) m_EventHandleContainers.Add(handle);
			return handle.EventHandle;
		}

		public IEventHandle RegisterWeak(object destination, string uniqueId, TEvent handler) {
			var h = (Delegate) (object) handler;
			var handle= EventManager.RegisterWeak(this,destination, h,uniqueId, m_WeakSource.Target,m_EventName);
			lock (m_EventHandleContainers) m_EventHandleContainers.Add(handle);
			return handle.EventHandle;
		}

		public IEventHandle RegisterWeak<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs:EventArgs {
			var h = (Delegate) (object) handler;
			var handle= EventManager.RegisterWeak(this,h,m_WeakSource.Target,m_EventName);
			lock (m_EventHandleContainers) m_EventHandleContainers.Add(handle);
			return handle.EventHandle;
		}


		public void Release(object destination, string eventHandlerId) {
			ReleaseImpl(destination, eventHandlerId);
		}

		private void ReleaseImpl(object destination, string eventHandlerId) {
			var release = new List<EventHandle>();
			lock (m_EventHandleContainers) {
				var remove = new List<EventContainer>();
				foreach (var container in m_EventHandleContainers) {
					var handle = container.EventHandle;
					if (handle == null) { remove.Add(container); continue; }
					if(!handle.IsAlive) { container.Dispose(); remove.Add(container); continue;}

					if (handle.DestinationUid == eventHandlerId && handle.Destination == destination) release.Add(handle);
				}
				foreach (var container in remove) m_EventHandleContainers.Remove(container);
				foreach (var bEvent in release) bEvent.Release(); // --> Released(..)
			}
		}

		private void ReleaseAll() {
			lock (m_EventHandleContainers) {
				foreach (var container in m_EventHandleContainers) {
					var handle = container.EventHandle;
					if (handle != null) handle.ReleaseBySource(); // does not call Released(..)
					container.Dispose();
				}
				m_EventHandleContainers.Clear();
			}
		}

		// internally called by WeakEventHandle
		void IEventSourceInternal.Released(EventHandle eventHandle) {
			lock (m_EventHandleContainers) m_EventHandleContainers.RemoveAll(x => ReferenceEquals(x.EventHandle, eventHandle)); 
		}

		List<EventContainer> IEventSourceInternal.GetContainers() { return m_EventHandleContainers; }


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

		/// <summary> Raises the event with specified arguments.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
		internal void Raise<T>(EventArgs args) {
			//TODO revise T and EventArgs
			Interlocked.Increment(ref EventManager.StatisticsːRaiseːInvocationCount);
			var raise = new List<EventHandle>();
			lock (m_EventHandleContainers) {
				var remove = new List<EventContainer>();
				var ec = m_EventHandleContainers.Count;
				for (int i = 0; i < ec; i++) {
					var container = m_EventHandleContainers[i];
					var handle = container.EventHandle;
					if (handle == null || !handle.IsAlive) { remove.Add(container); continue; }
					raise.Add(handle);
				}

				var rc = remove.Count;
				for (int i = 0; i < rc; i++) {
					var container = remove[i];
					container.Dispose();
					m_EventHandleContainers.Remove(container);
				}

				var ic = raise.Count;
				for (int i = 0; i < ic; i++) raise[i].Raise(new object[]{m_WeakSource.Target,args});
			}
		}


		/// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			Dispose(true);
		}

		/// <summary> Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="explicitDispose"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool explicitDispose) {
			if (explicitDispose) {
				ReleaseAll();
//				m_WeakSource = null;
//				m_EventName = null;
//				m_EventHandles = null;
			}
		}

	}
}