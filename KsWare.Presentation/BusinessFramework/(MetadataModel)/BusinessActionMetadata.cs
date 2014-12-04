using System;
using System.Collections.Generic;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides metadata for <see cref="ActionBM"/>
	/// </summary>
	public class BusinessActionMetadata:BusinessMetadata {

		private readonly List<object> m_CanExecuteObjections=new List<object>();


		/// <summary> Occurs when <see cref="ActionBM.CanExecute"/> property has been changed.
		/// </summary>
		private EventHandler<EventArgs> m_CanExecuteChangedCallback;

		/// <summary> Sets and removes objections for <see cref="ActionBM.CanExecute"/>.
		/// </summary>
		/// <param name="token">The objection token.</param>
		/// <param name="value">if set to <see langword="true"/> removes a objection; else adds a objection.</param>
		public void SetCanExecute(object token, bool value) {
			bool oldCanExecute = m_CanExecuteObjections.Count == 0;
			if(value) m_CanExecuteObjections.Remove(token); else if(!m_CanExecuteObjections.Contains(token))m_CanExecuteObjections.Add(token);
			bool newCanExecute = m_CanExecuteObjections.Count == 0;
			if(oldCanExecute!=newCanExecute) m_CanExecuteChangedCallback(this, EventArgs.Empty);
		}

		/// <summary> Gets or sets the execute callback.
		/// </summary>
		/// <value>The execute callback.</value>
		public EventHandler<ExecutedEventArgs> ExecuteCallback{get;set;}

		/// <summary> Occurs when <see cref="ActionBM.CanExecute"/> property has been changed.
		/// </summary>
		public EventHandler<EventArgs> CanExecuteChangedCallback {
			get { return m_CanExecuteChangedCallback; }
			set { m_CanExecuteChangedCallback = value; }
		}


		/// <summary> Gets a value indicating whether this instance can execute.
		/// </summary>
		/// <remarks></remarks>
		public bool CanExecute { get{return m_CanExecuteObjections.Count == 0; }}
	}
}