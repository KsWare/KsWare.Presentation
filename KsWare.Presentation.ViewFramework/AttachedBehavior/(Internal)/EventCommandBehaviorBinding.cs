using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior 
{
	/// <summary>
	/// Defines the command behavior binding
	/// </summary>
	internal class EventCommandBehaviorBinding : CommandBehaviorBinding 
	{
		bool _disposed;

		#region Properties

		/// <summary> The event name to hook up to
		/// </summary>
		public string EventName { get;internal set; }
		
		/// <summary> The event info of the event
		/// </summary>
		public EventInfo Event { get; private set; }
		
		/// <summary> Gets the EventHandler for the binding with the event
		/// </summary>
		public Delegate EventHandler { get; private set; }


		#endregion

		
		[SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId="System.String.Format(System.String,System.Object)")]
		public override void Bind(DependencyObject owner) { 
			Owner = owner;
			Event = Owner.GetType().GetEvent(EventName, BindingFlags.Public | BindingFlags.Instance);
			if (Event == null)
				throw new InvalidOperationException($"Could not resolve event name {EventName}");

			//Create an event handler for the event that will call the ExecuteCommand method
			EventHandler = EventHandlerGenerator.CreateDelegate(
				Event.EventHandlerType, 
				typeof(EventCommandBehaviorBinding).GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance), 
				this
			);
			//Register the handler to the Event
			Event.AddEventHandler(Owner, EventHandler);		
		}

		//Creates an EventHandler on runtime and registers that handler to the Event specified
		[Obsolete("Use Bind(DependencyObject)",true)]
		public void BindEvent(DependencyObject owner, string eventName) {
			if (eventName == null) throw new ArgumentNullException(nameof(eventName)); //ADDED 2010-02-08 ks
			if (eventName.Length == 0) throw new ArgumentOutOfRangeException(nameof(eventName),"EventName cannot be empty!");//ADDED 2010-02-08 ks
			EventName = eventName;
			Bind(owner);
		}

		#region IDisposable Members

		/// <summary>
		/// Unregisters the EventHandler from the Event
		/// </summary>
		public override void Dispose() {
//			base.Dispose();
			if (_disposed) return;
			Event.RemoveEventHandler(Owner, EventHandler);
			_disposed = true;
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}