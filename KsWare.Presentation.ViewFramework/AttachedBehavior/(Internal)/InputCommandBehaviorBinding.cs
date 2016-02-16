using System;
using System.Windows.Input;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior 
{
	/// <summary>
	/// Defines the command behavior binding
	/// </summary>
	internal abstract class InputCommandBehaviorBinding : CommandBehaviorBinding 
	{
		bool _disposed;
		private InputGesture _gesture;

		#region Properties

		protected System.Windows.Input.InputBinding InputBinding { get;set; }

		public InputGesture Gesture {
			get {return _gesture;}
			set {
				_gesture = value;
				if (InputBinding != null) InputBinding.Gesture = value;
			}
		}

		#endregion

		protected override void OnCommandParameterChanged() {
			base.OnCommandParameterChanged();
			if (InputBinding != null) InputBinding.CommandParameter = CommandParameter;
		}

		#region IDisposable Members

		/// <summary>
		/// Unregisters the EventHandler from the Event
		/// </summary>
		public override void Dispose() {
//			base.Dispose();
			if (_disposed) return;
			((UIElement) Owner).InputBindings.Remove(InputBinding);
			_disposed = true;
			GC.SuppressFinalize(this);
		}

		#endregion
	}

	internal sealed class MouseBinding : InputCommandBehaviorBinding 
	{
		private MouseAction _mouseAction;

		public new System.Windows.Input.MouseBinding InputBinding { get { return (System.Windows.Input.MouseBinding)base.InputBinding; } private set {base.InputBinding = value; } }
		
		public MouseAction MouseAction {
			get {return _mouseAction;}
			set {
				_mouseAction = value;
				if (InputBinding != null) InputBinding.MouseAction = value;
			}
		}

		//Creates an EventHandler on runtime and registers that handler to the Event specified
		public override void Bind(DependencyObject owner) {
			if (owner == null) throw new ArgumentNullException("owner");
			Owner = owner;
			InputBinding = new System.Windows.Input.MouseBinding {
				MouseAction      = MouseAction, 
				Command          = Command, 
				CommandParameter = CommandParameter
			};
			if (Gesture != null) InputBinding.Gesture = Gesture;

			((UIElement) Owner).InputBindings.Add(InputBinding);
		}
	}

	internal sealed class KeyBinding : InputCommandBehaviorBinding 
	{
		private Key key;

		public new System.Windows.Input.KeyBinding InputBinding { get {return (System.Windows.Input.KeyBinding) base.InputBinding; } private set {base.InputBinding = value; } }

		public Key Key {
			get {return key;}
			set {
				key = value;
				if (InputBinding != null) InputBinding.Key = value;
			}
		}

		//Creates an EventHandler on runtime and registers that handler to the Event specified
		public override void Bind(DependencyObject owner) {
			if (owner == null) throw new ArgumentNullException(nameof(owner));
			Owner = owner;
			InputBinding = new System.Windows.Input.KeyBinding {
				Key              = Key, 
				Command          = Command, 
				CommandParameter = CommandParameter
			};
			if (Gesture != null) InputBinding.Gesture = Gesture;

			((UIElement) Owner).InputBindings.Add(InputBinding);
		}
	}
}