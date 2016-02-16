using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	/// <summary> Defines a Command Binding
	/// This inherits from freezable so that it gets inheritance context for DataBinding to work
	/// </summary>
	public abstract class BehaviorBinding : Freezable {

		private readonly Type m_CommandBehaviorBindingType;
		DependencyObject m_Owner;
		private CommandBehaviorBinding m_Behavior;

		/// <summary> Initializes a new instance of the <see cref="BehaviorBinding"/> class.
		/// </summary>
		/// <param name="commandBehaviorBindingType">Type of the command behavior binding.</param>
		/// <remarks></remarks>
		protected BehaviorBinding(Type commandBehaviorBindingType) {
			if(commandBehaviorBindingType == null) throw new ArgumentNullException(nameof(commandBehaviorBindingType));
			if(!typeof(CommandBehaviorBinding).IsAssignableFrom(commandBehaviorBindingType)) throw new ArgumentOutOfRangeException(nameof(commandBehaviorBindingType));
			m_CommandBehaviorBindingType = commandBehaviorBindingType;
		}

		/// <summary> Stores the Command Behavior Binding
		/// </summary>
		internal CommandBehaviorBinding Behavior {
			get {
				if (m_Behavior == null) {
					m_Behavior = (CommandBehaviorBinding) Activator.CreateInstance(m_CommandBehaviorBindingType);
				}
				return m_Behavior;
			}
		}

		/// <summary> Gets or sets the Owner of the binding
		/// </summary>
		public DependencyObject Owner {
			get { return m_Owner; }
			set {
				m_Owner = value;
				ResetBinding();
			}
		}

		#region Command

		/// <summary>
		/// Command Dependency Property
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
			"Command", typeof(ICommand), typeof(BehaviorBinding),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged))
		);

		/// <summary>
		/// Gets or sets the Command property.  
		/// </summary>
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		/// <summary>
		/// Handles changes to the Command property.
		/// </summary>
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BehaviorBinding)d).OnCommandChanged(e);
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the Command property.
		/// </summary>
		protected virtual void OnCommandChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.Command = Command;
		}

		#endregion

		#region Action

		/// <summary>
		/// Action Dependency Property
		/// </summary>
		public static readonly DependencyProperty ActionProperty = DependencyProperty.Register(
			"Action", typeof(Action<object>), typeof(BehaviorBinding),
			new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnActionChanged)));

		/// <summary>
		/// Gets or sets the Action property. 
		/// </summary>
		public Action<object> Action {
			get { return (Action<object>)GetValue(ActionProperty); }
			set { SetValue(ActionProperty, value); }
		}

		/// <summary>
		/// Handles changes to the Action property.
		/// </summary>
		private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BehaviorBinding)d).OnActionChanged(e);
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the Action property.
		/// </summary>
		protected virtual void OnActionChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.Action = Action;
		}

		#endregion

		#region InvokeMethod

		/// <summary> InvokeMethod Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty InvokeMethodProperty = DependencyProperty.Register(
			"InvokeMethod", typeof(string), typeof(BehaviorBinding),
			new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnInvokeMethodChanged)));

		/// <summary>
		/// Gets or sets the InvokeMethod property. 
		/// </summary>
		public string InvokeMethod {
			get { return (string)GetValue(InvokeMethodProperty); }
			set { SetValue(InvokeMethodProperty, value); }
		}

		/// <summary> Handles changes to the InvokeMethod property.
		/// </summary>
		private static void OnInvokeMethodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BehaviorBinding)d).OnInvokeMethodChanged(e);
		}

		private void OnInvokeMethodChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.InvokeMethodName = (string) e.NewValue;
		}

		#endregion

		#region InvokeObject

		/// <summary> InvokeObject Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty InvokeObjectProperty = DependencyProperty.Register(
			"InvokeObject", typeof(object), typeof(BehaviorBinding),
			new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnInvokeObjectChanged)));

		/// <summary>
		/// Gets or sets the InvokeObject property. 
		/// </summary>
		public object InvokeObject {
			get { return (object)GetValue(InvokeMethodProperty); }
			set { SetValue(InvokeMethodProperty, value); }
		}

		/// <summary> Handles changes to the InvokeObject property.
		/// </summary>
		private static void OnInvokeObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BehaviorBinding)d).OnInvokeObjectChanged(e);
		}

		private void OnInvokeObjectChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.InvokeObject = e.NewValue;
		}

		#endregion

		#region CommandParameter

		/// <summary>
		/// CommandParameter Dependency Property
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
			"CommandParameter", typeof(object), typeof(BehaviorBinding),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandParameterChanged))
		);

		

		/// <summary>
		/// Gets or sets the CommandParameter property.  
		/// </summary>
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		/// <summary>
		/// Handles changes to the CommandParameter property.
		/// </summary>
		private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BehaviorBinding)d).OnCommandParameterChanged(e);
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the CommandParameter property.
		/// </summary>
		protected virtual void OnCommandParameterChanged(DependencyPropertyChangedEventArgs e) {
			Behavior.CommandParameter = CommandParameter;
		}

		#endregion


		/// <summary> Resets the binding.
		/// </summary>
		/// <param name="d">The DependencyObject</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks></remarks>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
		static void OwnerReset(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BehaviorBinding)d).ResetBinding();
		}

		/// <summary> Resets the binding.
		/// </summary>
		/// <remarks></remarks>
		protected virtual void ResetBinding() {
			//only do this when the Owner is set
			if (Owner == null) return; 
			
			//check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
			if (Behavior.Owner != null) Behavior.Dispose();

			//bind the new event to the command
			Behavior.Bind(Owner);
		}

		/// <summary> This is not actually used. 
		/// This is just a trick so that this object gets WPF Inheritance Context
		/// </summary>
		/// <returns></returns>
		protected override Freezable CreateInstanceCore() {
			throw new NotImplementedException();
		}
	}

}