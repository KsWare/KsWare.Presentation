using System;
using System.Collections.Generic;

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
	/// <para> XAML
	/// <code>
	/// &lt;UserControl ...
	///    xmlns:behaviors="clr-namespace:KsWare.Presentation.ViewFramework.Behaviors;assembly=KsWare.Presentation"
	///    &lt;Grid behaviors:BusyAdornerBehavior.BindToBusyUserRequest="True" >
	///       ...
	///    &lt;/Grid>
	/// &lt;/UserControl>
	/// </code>
	/// NOTE: BusyAdornerBehavior doesn't work at Window. Use the root element instead.
	/// </para> 
	/// <para>
	/// Optional override the default stlye:
	/// <code>
	/// &lt;ResourceDictionary
	///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	///     xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
	///     xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
	///     xmlns:behaviors="clr-namespace:KsWare.Presentation.ViewFramework.Behaviors"
	///     xmlns:viewFramework="clr-namespace:KsWare.Presentation.ViewFramework"
	///     mc:Ignorable="d" 
	/// >
	///	    &lt;Style TargetType="{x:Type behaviors:BusyAdornerVisual}">
	///	        ...
	///	    &lt;/Style>
	/// &lt;/ResourceDictionary>
	/// </code>
	/// default style: /KsWare.Presentation.ViewFramework;component/Themes/Styles/BusyAdornerVisual.xaml
	/// </para> 
	/// </example>	
	public class BusyManager:Singleton<BusyManager> {

		Dictionary<IObjectVM,BusyToken> _tokens=new Dictionary<IObjectVM, BusyToken>();

		/// <summary> Creates a busy context
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <returns>BusyToken</returns>
		/// <example><code>using (BusyManager.Instance.Context(this)) { ... } </code></example>
		public BusyToken Context(IObjectVM sender) {
			BusyToken token;
			_tokens.TryGetValue(sender, out token);
			if (token == null) {
				token = new BusyToken(sender);
				_tokens.Add(sender,token);
				token.Finished += (s, e) => _tokens.Remove(((BusyToken)s).Sender);
				token.Start();
			} else { token.ContinuedAsync(); }
			return token;
		}

		public void ContinueAsync(IObjectVM sender) {
			BusyToken token;
			_tokens.TryGetValue(sender, out token);
			token.ContinueAsync();
		}

		public void EndAsync(IObjectVM sender) {
			BusyToken token;
			_tokens.TryGetValue(sender, out token);
			if(token!=null) token.Done();
		}

		/// <summary> Represent a busy token
		/// </summary>
		public sealed class BusyToken : IDisposable {

			private volatile bool _isDisposed;
			private IObjectVM _sender;
			private volatile bool _suppressDispose;

			internal BusyToken(IObjectVM sender) {
				_sender = sender;
			}

			internal void Start() {
				_sender.RequestUserFeedback(new BusyUserFeedbackEventArgs(true));
			}

			internal IObjectVM Sender => _sender;

			void IDisposable.Dispose() {
				if (_suppressDispose) { _suppressDispose = false; return;}
				if(_isDisposed) return; _isDisposed = true;
				_sender.RequestUserFeedback(new BusyUserFeedbackEventArgs(false));
				if (Finished != null) Finished(this, EventArgs.Empty);
				_sender = null;
			}

			public void ContinueAsync() {
				_suppressDispose = true;
			}

			public void ContinuedAsync() {
				_suppressDispose = false;
			}

			public void Done() {
				_suppressDispose = false;
				((IDisposable)this).Dispose();
			}

			public event EventHandler Finished;
		}


	}
}
