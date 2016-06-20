using System;
using System.Collections;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> 
	/// </summary>
	public class LocalActionProvider: ActionProvider  {

		private readonly ArrayList m_CanExecuteObjections=new ArrayList();

		/// <summary> Initializes a new instance of the <see cref="DisplayValueProvider"/> class.
		/// </summary>
		public LocalActionProvider() {}

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
				if(CanExecuteCallback!=null) {
					var e=new CanExecuteEventArgs(null);
					CanExecuteCallback(this, e);
					if(!e.CanExecute) return false;
				}
				return m_CanExecuteObjections.Count==0;
			}
		}

		/// <summary> Sets a value whether the action can be executed or not
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <remarks></remarks>
		public override void SetCanExecute(object token, bool value) {
			var canExecute = this.CanExecute;
			if(value) m_CanExecuteObjections.Remove(token);
			else if(!m_CanExecuteObjections.Contains(token)) m_CanExecuteObjections.Add(token);
			if(CanExecute != canExecute) OnCanExecuteChanged();
		}

		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks></remarks>
		public override object BusinessObject {get {throw new NotSupportedException();}set {throw new NotSupportedException();}}
	}

}