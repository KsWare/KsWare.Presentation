using System;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> <see cref="ActionProvider"/> for actions in business layer
	/// </summary>
	/// <remarks></remarks>
	public class BusinessActionProvider: ActionProvider {

		private ActionBM _actionBM;

		/// <summary> Initializes a new instance of the <see cref="BusinessActionProvider"/> class.
		/// </summary>
		public BusinessActionProvider() {}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported => _actionBM !=null;

		/// <summary> Gets a value whether the action can be executed or not
		/// </summary>
		/// <value>The value whether the action can be executed or not</value>
		public override bool CanExecute {
			get {
				if(_actionBM==null) return false;
				return _actionBM.CanExecute;
			}
		}

		/// <summary> Sets a value whether the action can be executed or not
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <remarks></remarks>
		public override void SetCanExecute(object token, bool value) {
			if(_actionBM==null) throw new NotSupportedException("Underlying object not initialized!");
			//throw new NotSupportedException("Action not supported by underlying object! {8FFF5FFA-8E8E-471A-A302-E9DC0F9EAC70}");
			((BusinessFramework.BusinessActionMetadata) _actionBM.Metadata).SetCanExecute(token,value);
		}

		/// <summary> Executes the action.
		/// </summary>
		/// <param name="parameter">The action parameter.</param>
		/// <remarks></remarks>
		public override void Execute(object parameter) {
			if(_actionBM==null) throw new NotSupportedException("Underlying object not initialized!");
			_actionBM.Execute(parameter);
			//### base.Execute(parameter);
		}


		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks>
		/// <para>NOTE: This property is automatically set by <see cref="ActionVM"/> using DataProvider.Data</para>
		/// </remarks>
		public override object BusinessObject { //TODO revise type of BusinessObject, maybe sould be be IObjectBM/IActionBM
			get => _actionBM;
			set {

				if(_actionBM!=null) {
					_actionBM.CanExecuteChanged-=AtActionCanExecuteChanged;
				}

				_actionBM=(ActionBM) value;

				if(_actionBM!=null) {
					_actionBM.CanExecuteChanged+=AtActionCanExecuteChanged;
				}

				OnCanExecuteChanged();//TODO: call only when value of CanExecute has been changed
			}
		}

		/// <summary> Called when this.action.CanExecute-property has been changed
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void AtActionCanExecuteChanged(object sender, EventArgs eventArgs) { 
			OnCanExecuteChanged();
		}
		
	}

}