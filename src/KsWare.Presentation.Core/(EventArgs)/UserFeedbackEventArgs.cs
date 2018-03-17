using System;

namespace KsWare.Presentation {

	/// <summary> Provides arguments for the user feedback event
	/// </summary>
	/// <remarks>
	/// <blockquote>
	/// <b>Asynchronous handling of user feedback:</b><br/>
	/// The eventhandler <br/>
	/// - sets <see cref="UserFeedbackEventArgs.AsyncHandled"/> to <see langword="true"/><br/>
	/// - starts an asynchronous operation<br/>
	/// - and returns<br/>
	/// If the asynchronous operation ends: <br/>
	/// - sets <see cref="UserFeedbackEventArgs.Handled"/> to <see langword="true"/><br/>
	/// - sets the other return parameters to the desired values <br/>
	/// - calls the <see cref="UserFeedbackEventArgs.AsyncCallback"/> function. <br/>
	/// - and returns
	/// </blockquote>
	/// </remarks>
	public abstract class UserFeedbackEventArgs: EventArgs {
		
		private bool _handled;
		private object _returnValue;
		private bool _asyncHandled;
		private Action<UserFeedbackEventArgs> _asyncCallback;
		private object _asyncCallbackParameters;

		/// <summary> Initializes a new instance of the <see cref="UserFeedbackEventArgs"/> class.
		/// </summary>
		public UserFeedbackEventArgs() {}

		/// <summary> Gets or sets a value indicating whether event will be handled asynchronous.
		/// </summary>
		/// <value><see langword="true"/> if handled asynchronous; otherwise, <see langword="false"/>.
		/// </value>
		public bool AsyncHandled {
			get => _asyncHandled;
			set {
				if (_asyncHandled == true && value == false)
					throw new InvalidOperationException("UniqueID: {499187B0-B162-4DCC-AAEE-DA2DFAEB8E7F}");//REVIEW
				_asyncHandled = value;
			}
		}

		/// <summary> Gets or sets a value indicating whether event is handled.
		/// </summary>
		/// <value><see langword="true"/> if handled; otherwise, <see langword="false"/>.
		/// </value>
		public bool Handled {
			get => _handled;
			set {
				if (_handled == true && value == false)
					throw new InvalidOperationException("UniqueID: {83D4D228-D7B7-4A9E-AC4B-BB47533E6246}");//REVIEW
				_handled = value;
			}
		}

		/// <summary> Gets or sets the return value.
		/// </summary>
		/// <value>The return value.</value>
		public object ReturnValue {
			get => _returnValue;
			set {
				MemberAccessUtil.DemandWriteOnce(this._returnValue==null,null,this,nameof(ReturnValue),"{E10EB54D-F471-4C8E-8682-8C678A374E9B}");
				this._returnValue = value;
			}
		}

		public Action<UserFeedbackEventArgs> AsyncCallback {
			get => _asyncCallback;
			set => _asyncCallback = value;
		}

		public object AsyncCallbackParameters {
			get => _asyncCallbackParameters;
			set => _asyncCallbackParameters = value;
		}

		/// <summary> Gets the requesting source
		/// </summary>
		public object Source { get; set; }

		public abstract FeedbackType FeedbackType { get; }
	}

	public enum FeedbackType {
		None,
		Input,
		OpenFile,
		SaveFile,
		Message, 
		DetailMessage, 
		Exception,
		Busy,
		StringResource,
		Command

	}
}