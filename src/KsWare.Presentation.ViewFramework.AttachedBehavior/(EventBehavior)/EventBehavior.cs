using System;
using System.Windows;
using System.Windows.Input;

//REQUIRES: CommandBehaviorBinding (in KsWare.Presentation.ViewFramework.AttachedBehavior)
//REQUIRES: EventCommandBehaviorBinding (in KsWare.Presentation.ViewFramework.AttachedBehavior)


namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	/// <summary> Defines the attached properties to create a CommandBehaviorBinding
	/// </summary>
	public static class EventBehavior {

		//Adapted from AttachedCommandBehavior v2.0 - CommandBehavior.cs

		#region private Behavior

		/// <summary> Behavior Attached Dependency Property
		/// </summary>
		private static readonly DependencyProperty BehaviorProperty =DependencyProperty.RegisterAttached(
			"Behavior", typeof(EventCommandBehaviorBinding), typeof(EventBehavior),
			new FrameworkPropertyMetadata((CommandBehaviorBinding)null)
		);

		/// <summary> Gets the Behavior property. 
		/// </summary>
		private static EventCommandBehaviorBinding GetBehavior(DependencyObject d) {
			return (EventCommandBehaviorBinding)d.GetValue(BehaviorProperty);
		}

		/// <summary> Sets the Behavior property.  
		/// </summary>
		private static void SetBehavior(DependencyObject d, EventCommandBehaviorBinding value) {
			d.SetValue(BehaviorProperty, value);
		}

		#endregion

		#region Command

		/// <summary>
		/// Command Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty CommandProperty =DependencyProperty.RegisterAttached(
			"Command", typeof(ICommand), typeof(EventBehavior),
			new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnCommandChanged))
		);

		/// <summary> Gets the Command property.  
		/// </summary>
		public static ICommand GetCommand(DependencyObject d) {
			return (ICommand)d.GetValue(CommandProperty);
		}

		/// <summary> Sets the Command property. 
		/// </summary>
		public static void SetCommand(DependencyObject d, ICommand value) {
			d.SetValue(CommandProperty, value);
		}

		/// <summary> Handles changes to the Command property.
		/// </summary>
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CommandBehaviorBinding binding = FetchOrCreateBinding(d);
			binding.Command = (ICommand)e.NewValue;
		}

		#endregion

		#region Action

		/// <summary> Action Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty ActionProperty = DependencyProperty.RegisterAttached(
			"Action", typeof(Action<object>), typeof(EventBehavior),
			new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnActionChanged)));

		/// <summary> Gets the Action property.  
		/// </summary>
		public static Action<object> GetAction(DependencyObject d) {
			return (Action<object>)d.GetValue(ActionProperty);
		}

		/// <summary> Sets the Action property. 
		/// </summary>
		public static void SetAction(DependencyObject d, Action<object> value) {
			d.SetValue(ActionProperty, value);
		}

		/// <summary> Handles changes to the Action property.
		/// </summary>
		private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CommandBehaviorBinding binding = FetchOrCreateBinding(d);
			binding.Action = (Action<object>)e.NewValue;
		}

		#endregion

		#region InvokeMethod

		/// <summary> InvokeMethod Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty InvokeMethodProperty = DependencyProperty.RegisterAttached(
			"InvokeMethod", typeof(string), typeof(EventBehavior),
			new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnInvokeMethodChanged)));

		/// <summary> Gets the InvokeMethod property.  
		/// </summary>
		public static string GetInvokeMethod(DependencyObject d) {
			return (string)d.GetValue(InvokeMethodProperty);
		}

		/// <summary> Sets the InvokeMethod property. 
		/// </summary>
		public static void SetInvokeMethod(DependencyObject d, string value) {
			d.SetValue(InvokeMethodProperty, value);
		}

		/// <summary> Handles changes to the InvokeMethod property.
		/// </summary>
		private static void OnInvokeMethodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CommandBehaviorBinding binding = FetchOrCreateBinding(d);
			binding.InvokeMethodName =(string)e.NewValue;
		}

		#endregion

		#region InvokeObject

		/// <summary> InvokeObject Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty InvokeObjectProperty = DependencyProperty.RegisterAttached(
			"InvokeObject", typeof(object), typeof(EventBehavior),
			new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnInvokeObjectChanged)));

		/// <summary> Gets the InvokeObject property.  
		/// </summary>
		public static string GetInvokeObject(DependencyObject d) {return (string)d.GetValue(InvokeObjectProperty);}

		/// <summary> Sets the InvokeObject property. 
		/// </summary>
		public static void SetInvokeObject(DependencyObject d, string value) {d.SetValue(InvokeObjectProperty, value);}

		/// <summary> Handles changes to the InvokeObject property.
		/// </summary>
		private static void OnInvokeObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CommandBehaviorBinding binding = FetchOrCreateBinding(d);
			binding.InvokeObject = e.NewValue;
		}

		#endregion

		#region CommandParameter

		/// <summary> CommandParameter Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty =DependencyProperty.RegisterAttached(
			"CommandParameter", typeof(object), typeof(EventBehavior),
			new FrameworkPropertyMetadata((object)null,new PropertyChangedCallback(OnCommandParameterChanged))
		);

		/// <summary> Gets the CommandParameter property.  
		/// </summary>
		public static object GetCommandParameter(DependencyObject d) {
			return (object)d.GetValue(CommandParameterProperty);
		}

		/// <summary> Sets the CommandParameter property. 
		/// </summary>
		public static void SetCommandParameter(DependencyObject d, object value) {
			d.SetValue(CommandParameterProperty, value);
		}

		/// <summary> Handles changes to the CommandParameter property.
		/// </summary>
		private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CommandBehaviorBinding binding = FetchOrCreateBinding(d);
			binding.CommandParameter = e.NewValue;
		}

		#endregion

		#region Event

		/// <summary> Event Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty EventProperty = DependencyProperty.RegisterAttached(
			"Event", typeof(string), typeof(EventBehavior),
			new FrameworkPropertyMetadata(String.Empty,new PropertyChangedCallback(OnEventChanged))
		);

		/// <summary> Gets the Event property.
		/// </summary>
		public static string GetEvent(DependencyObject d) {
			return (string)d.GetValue(EventProperty);
		}

		/// <summary>  Sets the Event property.
		/// </summary>
		public static void SetEvent(DependencyObject d, string value) {
			d.SetValue(EventProperty, value);
		}

		/// <summary> Handles changes to the Event property.
		/// </summary>
		private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var binding = FetchOrCreateBinding(d);
			//check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
			if (binding.Event != null && binding.Owner != null) binding.Dispose();

			var eventName = e.NewValue == null ? null : e.NewValue.ToString().Trim();
			if(string.IsNullOrEmpty(eventName)) {
				binding.EventName = null;
				//do not bind because no event specified
			} else {
				//bind the new event to the command
				binding.EventName = eventName;
				binding.Bind(d);
			}
		}

		#endregion

		#region Helpers

		//tries to get a CommandBehaviorBinding from the element. Creates a new instance if there is not one attached
		private static EventCommandBehaviorBinding FetchOrCreateBinding(DependencyObject d) {
			var binding = GetBehavior(d);
			if (binding == null) {
				binding = new EventCommandBehaviorBinding();
				SetBehavior(d, binding);
			}
			return binding;
		}

		#endregion

	}

}
