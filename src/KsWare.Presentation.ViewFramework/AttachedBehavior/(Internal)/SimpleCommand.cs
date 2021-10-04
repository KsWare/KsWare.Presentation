using System;
using System.Windows.Input;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	// TODO move SimpleCommand => KsWare.Presentation.Input?
	// internally not used.

	/// <summary>
	/// Implements the ICommand and wraps up all the verbose stuff so that you can just pass 2 delegates 1 for the CanExecute and one for the Execute
	/// </summary>
	public class SimpleCommand : ICommand {

		/// <summary>
		/// Gets or sets the Predicate to execute when the CanExecute of the command gets called
		/// </summary>
		public Predicate<object> CanExecuteDelegate { get; set; }

		/// <summary>
		/// Gets or sets the action to be called when the Execute method of the command gets called
		/// </summary>
		public Action<object> ExecuteDelegate { get; set; }

		public void RaiseCanExecuteChanged() {
			CommandManager.InvalidateRequerySuggested();
		}

		#region ICommand Members

		/// <summary>
		/// Checks if the command Execute method can run
		/// </summary>
		/// <param name="parameter">THe command parameter to be passed</param>
		/// <returns>Returns true if the command can execute. By default true is returned so that if the user of SimpleCommand does not specify a CanExecuteCommand delegate the command still executes.</returns>
		public bool CanExecute(object parameter) {
			if (CanExecuteDelegate == null) return true; // if there is no ExecuteDelegate return true
			return CanExecuteDelegate(parameter);
		}

		/// <summary> Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler CanExecuteChanged {
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		/// <summary> Executes the command
		/// </summary>
		/// <param name="parameter">THe command parameter to be passed</param>
		public void Execute(object parameter) {
			if (ExecuteDelegate != null) ExecuteDelegate(parameter);
		}

		#endregion
	}
}