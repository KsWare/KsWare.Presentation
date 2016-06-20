using System;
using System.Windows.Input;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior 
{
	/// <summary>
	/// Defines the command behavior binding
	/// </summary>
	internal abstract class CommandBehaviorBinding : IDisposable 
	{
		private ICommand _command;
		private Action<object> _action;
		private string _invokeMethodName;
		private object _invokeObject;
		private object _commandParameter;
		private IExecutionStrategy _strategy;//stores the strategy of how to execute the event handler

		/// <summary>  Get the owner of the CommandBinding ex: a Button
		/// This property can only be set from the <see cref="Bind"/> Method
		/// </summary>
		public DependencyObject Owner { get; protected set; }

		/// <summary> Gets or sets a CommandParameter
		/// </summary>
		public object CommandParameter {
			get {return _commandParameter;}
			set {
				if(_commandParameter==value) return;
				_commandParameter = value;
				OnCommandParameterChanged();
			}
		}


		/// <summary> The command to execute when the specified event is raised
		/// </summary>
		public ICommand Command {
			get { return _command; }
			set {
				_command = value;
				_strategy = new CommandExecutionStrategy { Behavior = this };
			}
		}

		/// <summary> Gets or sets the Action
		/// </summary>
		public Action<object> Action {
			get { return _action; }
			set {
				_action = value;
				// set the execution strategy to execute the action
				_strategy = new ActionExecutionStrategy { Behavior = this };
			}
		}

//		/// <summary> Gets or sets the Method
//		/// </summary>
//		public MethodInfo Method {
//			get { return m_Method; }
//			set {
//				m_Method = value;
//				// set the execution strategy to execute the action
//				m_Strategy = new MethodInvokeExecutionStrategy { Behavior = this };
//			}
//		}

		/// <summary> Gets or sets the name of the method to invoke.
		/// </summary>
		public string InvokeMethodName {
			get { return _invokeMethodName; }
			set {
				_invokeMethodName = value;
				// set the execution strategy to execute the action
				if(!(_strategy is MethodInvokeExecutionStrategy)) _strategy = new MethodInvokeExecutionStrategy { Behavior = this };
			}
		}

		/// <summary> Gets or sets the object or <see cref="Type"/> for method invoke
		/// </summary>
		public object InvokeObject {
			get { return _invokeObject; }
			set {
				_invokeObject = value;
				// set the execution strategy to execute the action
				if(!(_strategy is MethodInvokeExecutionStrategy)) _strategy = new MethodInvokeExecutionStrategy { Behavior = this };
			}
		}

		protected virtual void OnCommandParameterChanged() { }

		//Creates an EventHandler on runtime and registers that handler to the Event specified
		public abstract void Bind(DependencyObject owner); 

		/// <summary>
		/// Executes the strategy
		/// </summary>
		public void Execute() {
			_strategy.Execute(CommandParameter);
		}

		#region IDisposable Members

//		/// <summary>
//		/// Unregisters the EventHandler from the Event
//		/// </summary>
//		public void Dispose() {
//			if (!m_Disposed) {
////				Event.RemoveEventHandler(Owner, EventHandler);
//				((UIElement) Owner).InputBindings.Remove(InputBinding);
//				m_Disposed = true;
//			}
//		}
		public abstract void Dispose();

		#endregion
	}

}