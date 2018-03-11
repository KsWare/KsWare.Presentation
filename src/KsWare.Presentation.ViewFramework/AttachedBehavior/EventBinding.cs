using System.Diagnostics.CodeAnalysis;
using System.Windows;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior 
{
	/// <summary> Defines a Command Binding
	/// </summary>
	/// <remarks>
	/// This class inherits from freezable so that it gets inheritance context for DataBinding to work
	/// </remarks>
	public sealed class EventBinding : BehaviorBinding 
	{

		/// <summary> Initializes a new instance of the <see cref="EventBinding"/> class.
		/// </summary>
		/// <remarks></remarks>
		public EventBinding(): base(typeof (EventCommandBehaviorBinding)) {}

		/// <summary> Stores the Command Behavior Binding
		/// </summary>
		internal new EventCommandBehaviorBinding Behavior {get {return (EventCommandBehaviorBinding) base.Behavior;}}

		#region Event

		/// <summary> Event Dependency Property
		/// </summary>
		public static readonly DependencyProperty EventProperty = DependencyProperty.Register(
			"Event", typeof(string), typeof(EventBinding),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnEventChanged))
		);

		/// <summary> Gets or sets the Event name.  
		/// </summary>
		[UsedImplicitly]
		public string Event {
			get { return (string)GetValue(EventProperty); }
			set { SetValue(EventProperty, value); }
		}

		/// <summary> Handles changes to the Event property.
		/// </summary>
		private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EventBinding)d).OnEventChanged(e);
		}

		/// <summary> Provides derived classes an opportunity to handle changes to the Event property.
		/// </summary>
		
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
		private void OnEventChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.EventName = Event;
			ResetBinding();
		}

		#endregion

		
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
		static void OwnerReset(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EventBinding)d).ResetBinding();
		}

		/// <summary> Resets the binding.
		/// </summary>
		/// <remarks></remarks>
		protected override void ResetBinding() {
			//only do this when the Owner is set
			if (Owner == null) return; 
			
			//check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
			if (Behavior.Event != null && Behavior.Owner != null) Behavior.Dispose();

			//bind the new event to the command
			Behavior.Bind(Owner);
		}

//		/// <summary>
//		/// This is not actually used. This is just a trick so that this object gets WPF Inheritance Context
//		/// </summary>
//		/// <returns></returns>
//		protected override Freezable CreateInstanceCore() {
//			throw new NotImplementedException();
//		}
	}
}