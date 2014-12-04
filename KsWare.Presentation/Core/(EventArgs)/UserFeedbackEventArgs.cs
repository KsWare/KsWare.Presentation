using System;
using KsWare.Presentation.ViewModelFramework;

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
		
		private bool m_Handled;
		private object m_ReturnValue;
		private bool m_AsyncHandled;
		private Action<UserFeedbackEventArgs> m_AsyncCallback;
		private object m_AsyncCallbackParameters;

		/// <summary> Initializes a new instance of the <see cref="UserFeedbackEventArgs"/> class.
		/// </summary>
		public UserFeedbackEventArgs() {}

		/// <summary> Gets or sets a value indicating whether event will be handled asynchronous.
		/// </summary>
		/// <value><see langword="true"/> if handled asynchronous; otherwise, <see langword="false"/>.
		/// </value>
		public bool AsyncHandled {
			get {return m_AsyncHandled;}
			set {
				if (m_AsyncHandled == true && value == false)
					throw new InvalidOperationException("UniqueID: {499187B0-B162-4DCC-AAEE-DA2DFAEB8E7F}");//REVIEW
				m_AsyncHandled = value;
			}
		}

		/// <summary> Gets or sets a value indicating whether event is handled.
		/// </summary>
		/// <value><see langword="true"/> if handled; otherwise, <see langword="false"/>.
		/// </value>
		public bool Handled {
			get {return m_Handled;}
			set {
				if (m_Handled == true && value == false)
					throw new InvalidOperationException("UniqueID: {83D4D228-D7B7-4A9E-AC4B-BB47533E6246}");//REVIEW
				m_Handled = value;
			}
		}

		/// <summary> Gets or sets the return value.
		/// </summary>
		/// <value>The return value.</value>
		public object ReturnValue {
			get {return m_ReturnValue;}
			set {
				MemberAccessUtil.DemandWriteOnce(this.m_ReturnValue==null,null,this,"ReturnValue","{E10EB54D-F471-4C8E-8682-8C678A374E9B}");
				this.m_ReturnValue = value;
			}
		}

		public Action<UserFeedbackEventArgs> AsyncCallback {
			get { return m_AsyncCallback; }
			set { 
//				MemberAccessUtil.DemandWriteOnce(m_AsyncCallback==null && m_AsyncCallbackParameters==null,null,this,"AsyncCallback","{CD39C0F6-59AF-4CFD-9C19-95E650766618}");
				m_AsyncCallback = value;
			}
		}

		public object AsyncCallbackParameters {
			get { return m_AsyncCallbackParameters; }
			set {
//				MemberAccessUtil.DemandWriteOnce(m_AsyncCallback==null && m_AsyncCallbackParameters==null,null,this,"AsyncCallbackParameters","{CD39C0F6-59AF-4CFD-9C19-95E650766618}");
				m_AsyncCallbackParameters = value;
			}
		}

		/// <summary> Gets the requesting source
		/// </summary>
		public object Source { get; internal set; }

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