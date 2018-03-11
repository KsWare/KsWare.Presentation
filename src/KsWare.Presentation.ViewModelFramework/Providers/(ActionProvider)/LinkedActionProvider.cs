//using System;
//
//namespace KsWare.Presentation.ViewModelFramework.Providers {
//
//	/// <summary>  <see cref="ActionProvider"/> for linked actions
//	/// </summary>
//	/// <remarks></remarks>
//	public class LinkedActionProvider: ActionProvider {
//
//		private ActionVM _LinkedAction;
//
//		/// <summary> Initializes a new instance of the <see cref="DisplayValueProvider"/> class.
//		/// </summary>
//		public LinkedActionProvider() {}
//
//		/// <summary> Gets a value indicating whether the provider is supported.
//		/// </summary>
//		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
//		/// <remarks></remarks>
//		public override bool IsSupported{get {return _LinkedAction!=null;}}
//
//		/// <summary> Gets a value whether the action can be executed or not
//		/// </summary>
//		/// <value>The value whether the action can be executed or not</value>
//		public override bool CanExecute {
//			get {
//				if(_LinkedAction==null) return false;
//				return _LinkedAction.CanExecute;
//			}
//		}
//
//		/// <summary> Sets a value whether the action can be executed or not
//		/// </summary>
//		/// <param name="token">The token.</param>
//		/// <param name="value">if set to <c>true</c> [value].</param>
//		/// <remarks></remarks>
//		public override void SetCanExecute(object token, bool value) {
//			if(_LinkedAction==null) throw new NotSupportedException("Underlying object not initalized!");
//			_LinkedAction.Metadata.ActionProvider.SetCanExecute(token,value);
//		}
//
//		/// <summary> Executes the action.
//		/// </summary>
//		/// <param name="parameter">The action parameter.</param>
//		/// <remarks></remarks>
//		public override void Execute(object parameter) {
//			if(_LinkedAction==null) throw new NotSupportedException("Underlying object not initalized!");
//			_LinkedAction.Execute(parameter);
//		}
//
//
//		/// <summary> Gets or sets the business object.
//		/// </summary>
//		/// <value>The business object.</value>
//		/// <remarks></remarks>
//		public override object BusinessObject {
//			get {return _LinkedAction;}
//			set {
//				if(_LinkedAction!=null) {
//					_LinkedAction.CanExecuteChanged-=AtActionCanExecuteChanged;
//				}
//
//				_LinkedAction=(ActionVM) value;
//
//				if(_LinkedAction!=null) {
//					_LinkedAction.CanExecuteChanged+=AtActionCanExecuteChanged;
//				}
//
//				OnCanExecuteChanged();//TODO: call only when value of CanExecute has been changed
//			}
//		}
//
//		/// <summary> Called when this.action.CanExecute-property has been changed
//		/// </summary>
//		/// <param name="sender">The sender.</param>
//		/// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
//		private void AtActionCanExecuteChanged(object sender, EventArgs eventArgs) { 
//			OnCanExecuteChanged();
//		}
//	}
//
//}