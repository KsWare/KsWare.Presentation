using System;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides event arguments for the error event
	/// </summary>
	[UsedImplicitly]
	public class ErrorEventArgs:EventArgs {
		private readonly bool m_HasError;
		private readonly string m_ErrorMessage;
		private readonly Exception m_Exception;

		/// <summary> Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
		/// </summary>
		/// <param name="hasError">if set to <c>true</c> [has error].</param>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="exception">The exception.</param>
		public ErrorEventArgs(bool hasError, string errorMessage, Exception exception=null) {
			m_HasError = hasError;
			m_ErrorMessage = errorMessage;
			m_Exception = exception;
		}

		/// <summary> Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="exception">The exception.</param>
		public ErrorEventArgs(string errorMessage, Exception exception=null) {
			m_HasError = true;
			m_ErrorMessage = errorMessage;
			m_Exception = exception;
		}

		/// <summary> Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
		/// </summary>
		public ErrorEventArgs() {
//			this.hasError = false;
//			this.errorMessage = null;
//			this.exception = null;
		}

		/// <summary> Gets a value indicating whether this instance has error.
		/// </summary>
		/// <value><c>true</c> if this instance has error; otherwise, <c>false</c>.</value>
		public bool HasError{get {return m_HasError;}}
		
		/// <summary> Gets the error message.
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage{get {return m_ErrorMessage;}}

		/// <summary> Gets the exception.
		/// </summary>
		/// <value>The exception.</value>
		public Exception Exception{get {return m_Exception;}}
	}
}