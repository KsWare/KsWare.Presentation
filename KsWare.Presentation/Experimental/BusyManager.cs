using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KsWare.Presentation.Core;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> [EXPERIMENTAL]
	/// </summary>
	/// <example><code>
	/// void DoOperation() {
	///     using(BusyManager.Instance.Context(this)) { 
	///         /*do anything long operation*/
	/// 
	///      // optional continue the long operation asynchronous.
	///         new Thread(DoOperationProc).Start();
	///         BusyManager.Instance.ContinueAsync(this);
	///     }
	/// }
	/// void DoOperationProc() {
	///     try {
	///	        // continue the long operation asynchronous.
	///         ...
	///     } finally { BusyManager.Instance.EndAsync(this);}    
	/// }
	/// </code>
	/// <code>
	/// &lt;UserControl ...
	///    xmlns:behaviors="clr-namespace:KsWare.Presentation.ViewFramework.Behaviors;assembly=KsWare.Presentation"
	///    &lt;Grid behaviors:BusyAdornerBehavior.BindToBusyUserRequest="True" >
	///       ...
	///    &lt;/Grid>
	/// &lt;/UserControl>
	/// </code>
	/// </example>	
	public class BusyManager:Singleton<BusyManager> {

		private BusyToken m_Busy;

		/// <summary>
		/// [EXPERIMENTAL] Creates a busy context
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <returns>BusyToken</returns>
		/// <example><code>using (BusyManager.Instance.Context(this)) { ... } </code></example>
		public BusyToken Context(IObjectVM sender) {
			if (m_Busy == null) {
				m_Busy = new BusyToken(sender);
				m_Busy.Finished += (s, e) => m_Busy = null;
			} else { m_Busy.ContinuedAsync(); }
			return m_Busy;
		}

		public void ContinueAsync(ObjectVM sender) {
			m_Busy.ContinueAsync();
		}

		/// <summary> [EXPERIMENTAL]
		/// </summary>
		public class BusyToken : IDisposable {

			private volatile bool m_IsDisposed;
			private IObjectVM m_Sender;
			private volatile bool m_SuppressDispose;

			public BusyToken(IObjectVM sender) {
				m_Sender = sender;
				m_Sender.RequestUserFeedback(new BusyUserFeedbackEventArgs(true));
			}

			void IDisposable.Dispose() {
				if (m_SuppressDispose) { m_SuppressDispose = false; return;}
				if(m_IsDisposed) return; m_IsDisposed = true;
				m_Sender.RequestUserFeedback(new BusyUserFeedbackEventArgs(false));
				if (Finished != null) Finished(this, EventArgs.Empty);
				m_Sender = null;
			}

			public void ContinueAsync() {
				m_SuppressDispose = true;
			}
			public void ContinuedAsync() {
				m_SuppressDispose = false;
			}

			public void Done() {
				m_SuppressDispose = false;
				((IDisposable)this).Dispose();
			}

			public event EventHandler Finished;
		}

		public void EndAsync(ObjectVM sender) {
			m_Busy.Done();
		}
	}
}
