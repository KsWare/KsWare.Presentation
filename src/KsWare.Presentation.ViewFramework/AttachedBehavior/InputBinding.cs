using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	/// <summary> Defines a Command Binding
	/// This inherits from freezable so that it gets inheritance context for DataBinding to work
	/// </summary>
	public abstract class InputBinding : BehaviorBinding {

		/// <summary> Initializes a new instance of the <see cref="InputBinding"/> class.
		/// </summary>
		/// <param name="commandBehaviorBinding">The command behavior binding.</param>
		/// <remarks></remarks>
		protected InputBinding(Type commandBehaviorBinding):base(commandBehaviorBinding) {}

		/// <summary> Stores the Command Behavior Binding
		/// </summary>
		internal new InputCommandBehaviorBinding Behavior => (InputCommandBehaviorBinding)base.Behavior;

		#region Gesture

		/// <summary> Gesture Dependency Property
		/// </summary>
		public static readonly DependencyProperty GestureProperty = DependencyProperty.Register(
			"Gesture", typeof(InputGesture), typeof(InputBinding),
			new FrameworkPropertyMetadata(null, (d, e) => ((MouseInputBehaviorBinding)d).OnGestureChanged(e))
		);

		/// <summary> Gets or sets the Gesture property.  
		/// </summary>
		public InputGesture Gesture {
			get => (InputGesture)GetValue(GestureProperty);
			set => SetValue(GestureProperty, value);
		}

		/// <summary> Handles changes to the Gesture property.
		/// Provides derived classes an opportunity to handle changes to the Gesture property.
		/// </summary>
		protected virtual void OnGestureChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.Gesture = Gesture;
		}

		#endregion


		/// <summary> Resets the binding.
		/// </summary>
		/// <param name="d">The DependencyObject</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks></remarks>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
		static void OwnerReset(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((InputBinding)d).ResetBinding();
		}

		/// <summary> Resets the binding.
		/// </summary>
		/// <remarks></remarks>
		protected override void ResetBinding() {
			//only do this when the Owner is set
			if (Owner == null) return; 
			
			//check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
			if (Behavior.Owner != null) Behavior.Dispose();

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

	/// <summary> Binder for mouse input 
	/// </summary>
	public sealed class MouseInputBehaviorBinding:InputBinding {

		/// <summary> Initializes a new instance of the <see cref="MouseInputBehaviorBinding"/> class.
		/// </summary>
		/// <remarks></remarks>
		public MouseInputBehaviorBinding(): base(typeof (MouseBinding)) {}

		/// <summary> Stores the Command Behavior Binding
		/// </summary>
		internal new MouseBinding Behavior => (MouseBinding)base.Behavior;

		#region MouseAction

		/// <summary> MouseAction Dependency Property
		/// </summary>
		public static readonly DependencyProperty MouseActionProperty = DependencyProperty.Register(
			"MouseAction", typeof(MouseAction), typeof(MouseInputBehaviorBinding),
			new FrameworkPropertyMetadata(MouseAction.None, (d, e) => ((MouseInputBehaviorBinding)d).OnMouseActionChanged(e))
		);

		/// <summary> Gets or sets the MouseAction property.  
		/// </summary>
		public MouseAction MouseAction {
			get => (MouseAction)GetValue(MouseActionProperty);
			set => SetValue(MouseActionProperty, value);
		}


		/// <summary> Handles changes to the MouseAction property.
		/// <s>Provides derived classes an opportunity to handle changes to the MouseAction property.</s>
		/// </summary>
		private void OnMouseActionChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.MouseAction = MouseAction;
		}

		#endregion
	}
	
	/// <summary>  Binder for keyboard input 
	/// </summary>
	[UsedImplicitly]
	public sealed class KeyInputBehaviorBinding:InputBinding {

		/// <summary> Initializes a new instance of the <see cref="KeyInputBehaviorBinding"/> class.
		/// </summary>
		/// <remarks></remarks>
		public KeyInputBehaviorBinding(): base(typeof (KeyBinding)) {}

		/// <summary> Stores the Command Behavior Binding
		/// </summary>
		internal new KeyBinding Behavior => (KeyBinding) base.Behavior;

		#region Key

		/// <summary> Key Dependency Property
		/// </summary>
		public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
			"Key", typeof(Key), typeof(KeyInputBehaviorBinding),
			new FrameworkPropertyMetadata(Key.None, (d, e) => ((KeyInputBehaviorBinding)d).AtKeyChanged(e))
		);

		/// <summary> Gets or sets the Key property.  
		/// </summary>
		[UsedImplicitly]
		public Key Key {
			get => (Key)GetValue(KeyProperty);
			set => SetValue(KeyProperty, value);
		}

		/// <summary> Provides derived classes an opportunity to handle changes to the MouseAction property.
		/// </summary>
		private void AtKeyChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.Key = Key;
		}

		#endregion
	}
}