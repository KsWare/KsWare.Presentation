using System;
using System.Collections;
using System.Windows.Input;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> 
	/// </summary>
	public class CommandActionProvider: ActionProvider {

		private readonly ArrayList _canExecuteObjections=new ArrayList();
		private ICommand _command;

		/// <summary> Initializes a new instance of the <see cref="DisplayValueProvider"/> class.
		/// </summary>
		public CommandActionProvider() {}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported{get {return true;}}

		/// <summary> Gets a value whether the action can be executed or not
		/// </summary>
		/// <value>The value whether the action can be executed or not</value>
		public override bool CanExecute {
			get {
				//return _CanExecuteObjections.Count==0;
//				return _Command!=null && _Command.CanExecute(null);
				return true;
			}
		}

		/// <summary> Sets a value whether the action can be executed or not
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <remarks></remarks>
		public override void SetCanExecute(object token, bool value) {
			var canExecute = this.CanExecute;
			if(value) _canExecuteObjections.Remove(token);
			else if(!_canExecuteObjections.Contains(token)) _canExecuteObjections.Add(token);
			if(CanExecute != canExecute) OnCanExecuteChanged();
		}

		public ICommand Command {
			get { return _command; }
			set {
				if(_command==value)return;

				if(_command!=null) {
					_command.CanExecuteChanged-=AtCommandOnCanExecuteChanged;
					ExecutedCallback = null;
				}
				_command = value;
				if(_command!=null) {
					_command.CanExecuteChanged+=AtCommandOnCanExecuteChanged;
					ExecutedCallback= (s,e) => _command.Execute(e.Parameter);
				}
			}
		}

		private void AtCommandOnCanExecuteChanged(object sender, EventArgs eventArgs) {
			OnPropertyChanged("CanExecute");
		}

		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks></remarks>
		public override object BusinessObject {get {throw new NotSupportedException();}set {throw new NotSupportedException();}}
	}

}