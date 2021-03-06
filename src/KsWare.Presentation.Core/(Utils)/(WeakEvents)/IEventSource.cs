﻿using System;
using System.Collections.Generic;

namespace KsWare.Presentation {

	/// <summary> Interface for weak event source 
	/// </summary>
	public interface IEventSource:IDisposable {

		/// <summary> Gets the name of the event.
		/// </summary>
		/// <value>
		/// The name of the event.
		/// </value>
		string EventName { get; }
	}

	internal interface IEventSourceInternal {
		void Released(EventHandle registeredEvent);
		List<EventContainer>GetContainers();
	}

	/// <summary> Public interface for strong typed weak event source 
	/// </summary>
	/// <typeparam name="TEventHandler">The type of the event.</typeparam>
	/// <example> Definition of the event source property:
	/// <code> public IWeakEventSource&lt;EventHandler&lt;ViewModelPropertyChangedEventArgs>> PropertyChangedEvent { get { return WeakEventProperties.Get(()=>PropertyChangedEvent); } }</code>
	/// The ObjectVM.EventSourcesproperty is available in all ObjectVM and ObjectBM classes.</example>
	public interface IEventSource<TEventHandler>:IEventSource  {

		/// <summary> Registers an event handler.
		/// </summary>
		/// <param name="destination">The destination object.</param>
		/// <param name="uniqueId">The unique id.</param>
		/// <param name="handler">The handler.</param>
		/// <remarks>The <paramref name="uniqueId"/> is used to release the event handler.</remarks>
		IEventHandle Register(object destination, string uniqueId, TEventHandler handler);

		/// <summary> Registers an event handler.
		/// </summary>
		/// <param name="handler">The event handler to register.</param>
		/// <returns>IWeakEventHandle.</returns>
		IEventHandle Register(TEventHandler handler);

//		IWeakEventHandle RegisterExpression(Expression<Action<object, EventArgs>> expression);

		/// <summary> Registers a weak event handler.
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <returns>The weak event handle.</returns>
		IEventHandle RegisterWeak(TEventHandler handler);

		/// <summary> Registers a weak event handler.
		/// </summary>
		/// <param name="destination">The destination object</param>
		/// <param name="uniqueId">The unique id.</param>
		/// <param name="handler">The handler.</param>
		/// <returns>The weak event handle.</returns>
		IEventHandle RegisterWeak(object destination, string uniqueId, TEventHandler handler);

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

//		/// <summary> Releases an event handler.
//		/// </summary>
//		/// <value>The event handler</value>
//		/// <remarks>
//		/// This is an alternate (event remove like <c>-=</c> ) syntax to the <see cref="Release(object,string)"/> method
//		/// </remarks>
//		/// <example>removing a lambda:<code>MyModel.PropertyChangedEvent.remove=(s,e) => {/*do anyting*/};</code></example>
//		/// <example>removing a method:<code>MyModel.PropertyChangedEvent.remove=AtPropertyChanged;
//		/// private void AtPropertyChanged(object sender, EventArgs e) {/*do anyting*/}
//		/// </code></example>
//		TEventHandler remove { set; }

//		/// <summary> Releases an event handler.
//		/// </summary>
//		/// <value>The event handler</value>
//		/// <remarks>
//		/// This is an alternate (event remove like <c>-=</c> ) syntax to the <see cref="Release(object,string)"/> method
//		/// </remarks>
//		/// <example>removing a lambda:<code>MyModel.PropertyChangedEvent.ｰ=(s,e) => {/*do anyting*/};</code></example>
//		/// <example>removing a method:<code>MyModel.PropertyChangedEvent.ｰ=AtPropertyChanged;
//		/// private void AtPropertyChanged(object sender, EventArgs e) {/*do anyting*/}
//		/// </code></example>
//		TEventHandler ｰ { set; }

	}


}