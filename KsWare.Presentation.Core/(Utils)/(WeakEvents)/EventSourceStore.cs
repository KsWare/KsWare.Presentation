using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace KsWare.Presentation {

	/// <summary> Provides a store for weak event properties
	/// </summary>
	/// <example>Usage: <code>
	/// public class MyClass {
	///		private readonly Lazy&lt;WeakEventSourceStore&gt; _LazyWeakEventStore;
	///		private object _myProperty;
	/// 
	///		public MyClass() {
	///			_LazyWeakEventStore = new Lazy&lt;WeakEventSourceStore&gt;(() => new WeakEventSourceStore(this));
	///		}
	/// 
	///		public object MyProperty {
	///			get { return _myProperty; }
	///			set {
	///				if(Equals(_myProperty,value)) return;
	///				_MyProperty = value;
	///				OnMyPropertyChanged();
	///			}
	///		}
	/// 
	///		protected virtual void OnMyPropertyChanged() { 
	///			WeakEventManager.Raise&lt;EventHandler,EventArgs>(_lazyWeakEventProperties, "MyPropertyChangedEvent", EventArgs.Empty);
	///		}
	/// 
	///		public IWeakEventSource&lt;EventHandler&gt; MyPropertyChangedEvent { get { return WeakEventProperties.Get&lt;EventHandler&gt;("MyPropertyChangedEvent"); } }
	/// }
	/// </code></example>
	public class EventSourceStore {

		private readonly WeakReference _WeakSourceObject;
		private readonly Dictionary<string,IEventSource> _EventSources=new Dictionary<string, IEventSource>();

		/// <summary> Initializes a new instance of the <see cref="EventSourceStore"/> for the specifiecified source object.
		/// </summary>
		/// <param name="sourceObject">The source object.</param>
		public EventSourceStore(object sourceObject) {
			_WeakSourceObject = new WeakReference(sourceObject);
		}

		/// <summary> Gets the number of initialized weak event sources
		/// </summary>
		public int Count { get { return _EventSources.Count; } }

        //		/// <summary> Gets a weak event source (IWeakEventSource)
        //		/// </summary>
        //		/// <typeparam name="TEventHandler">The type of the event handler</typeparam>
        //		/// <param name="eventPropertyExpression">(e.g. <c>_=>MyPropertyChangedEvent</c>)</param>
        //		/// <returns>The weak event source</returns>
        //		[Obsolete("Slow!",true)]
        //		public IWeakEventSource<TEventHandler> Get<TEventHandler>(Expression<Func<object, IWeakEventSource<TEventHandler>>> eventPropertyExpression) {
        //			var name =GetName(eventPropertyExpression);
        //			return Get<TEventHandler>(name);
        //		}

        //		[Obsolete("Slow!",true)]
        //		public IWeakEventSource<TEventHandler> Get<TEventHandler>(Expression<Func<IWeakEventSource<TEventHandler>>> eventPropertyExpression) {
        //			var name =GetName(eventPropertyExpression);
        //			return Get<TEventHandler>(name);
        //		}

        /// <summary> Gets a weak event source (IWeakEventSource)
        /// </summary>
        /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
        /// <param name="eventName">The event name.</param>
        /// <returns>The weak event source</returns>
		 public IEventSource<TEventHandler> Get<TEventHandler>(string eventName) {
			IEventSource eventSource;
			if (!_EventSources.TryGetValue(eventName, out eventSource)) {
				var sourceObject = _WeakSourceObject.Target;
				eventSource=EventManager.RegisterSource4Store<TEventHandler>(this, sourceObject, eventName);
				_EventSources.Add(eventName,eventSource);
			}
			return (IEventSource<TEventHandler>) eventSource;
		}

//		/// <summary> Determines whether the store contains the property
//		/// </summary>
//		/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
//		/// <param name="eventPropertyExpression">The event property expression.</param>
//		/// <returns><c>true</c> if the store contains the property; otherwise, <c>false</c>.</returns>
//		[Obsolete("Slow!",true)]
//		public bool Has<TEventHandler>(Expression<Func<object, IWeakEventSource<TEventHandler>>> eventPropertyExpression) {
//			var propertyName = GetName(eventPropertyExpression);
//			var b = _EventProperties.ContainsKey(propertyName);
//			return b;
//		}

//		/// <summary> Determines whether the store contains the property
//		/// </summary>
//		/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
//		/// <param name="eventPropertyExpression">The event property expression.</param>
//		/// <returns><c>true</c> if the store contains the property; otherwise, <c>false</c>.</returns>
//		[Obsolete("Slow!",true)]
//		public bool Has<TEventHandler>(Expression<Func<IWeakEventSource<TEventHandler>>> eventPropertyExpression) {
//			var propertyName = GetName(eventPropertyExpression);
//			var b = _EventProperties.ContainsKey(propertyName);
//			return b;
//		}

		/// <summary> Determines whether the store contains the property
		/// </summary>
		/// <param name="property">The event property name.</param>
		/// <returns><c>true</c> if the store contains the property; otherwise, <c>false</c>.</returns>
		public bool Has(string property) {
			var b = _EventSources.ContainsKey(property);
			return b;
		}

		public IEventSource<TEventHandler> TryGet<TEventHandler>(string eventName) {
			IEventSource eventSource;
			if (!_EventSources.TryGetValue(eventName, out eventSource)) return null;
			return (IEventSource<TEventHandler>) eventSource;
		}

//		// _ => MyeventPropertyExpression
//		[Obsolete("Slow!",true)]
//		public IWeakEventSource<TEventHandler> TryGet<TEventHandler>(Expression<Func<object, IWeakEventSource<TEventHandler>>> eventPropertyExpression) {
//			var name = GetName(eventPropertyExpression);
//			IWeakEventSource eventSource;
//			if (!_EventProperties.TryGetValue(name, out eventSource)) return null;
//			return (IWeakEventSource<TEventHandler>) eventSource;
//		}

//		// () => MyeventPropertyExpression
//		[Obsolete("Slow!",true)]
//		public IWeakEventSource<TEventHandler> TryGet<TEventHandler>(Expression<Func<IWeakEventSource<TEventHandler>>> eventPropertyExpression) {
//			var name = GetName(eventPropertyExpression);
//			IWeakEventSource eventSource;
//			if (!_EventProperties.TryGetValue(name, out eventSource)) return null;
//			return (IWeakEventSource<TEventHandler>) eventSource;
//		}

//		// _ => MyeventPropertyExpression
//		[Obsolete("Slow!",true)]
//		private string GetName<T>(Expression<Func<object, IWeakEventSource<T>>> eventPropertyExpression) {
//			var eventPropertyName = MemberNameUtil.GetPropertyName(eventPropertyExpression);
//			var propertyName = eventPropertyName.EndsWith("Event") ? eventPropertyName.Substring(0, eventPropertyName.Length - "Event".Length) : eventPropertyName;
//			return propertyName;
//		}

//		// () => MyeventPropertyExpression
//		[Obsolete("Slow!",true)]
//		private string GetName<T>(Expression<Func<IWeakEventSource<T>>> eventPropertyExpression) {
//			var eventPropertyName = MemberNameUtil.GetPropertyName(eventPropertyExpression);
//			var propertyName = eventPropertyName.EndsWith("Event") ? eventPropertyName.Substring(0, eventPropertyName.Length - "Event".Length) : eventPropertyName;
//			return propertyName;
//		}

		public void Dispose() {
			Dispose(true);
		}

		private void Dispose(bool explicitDispose) {
			if (explicitDispose) {
				foreach (var p in _EventSources) p.Value.Dispose();
				_EventSources.Clear();
//				_EventProperties = null;
//				_WeakSourceObject=null;
							
			}
		}


	}
}