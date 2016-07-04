using System;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides event arguments for the error event
	/// </summary>
	[UsedImplicitly]
	public class ErrorEventArgs:EventArgs {
		private readonly bool _HasError;
		private readonly string _ErrorMessage;
		private readonly Exception _Exception;

		/// <summary> Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
		/// </summary>
		/// <param name="hasError">if set to <c>true</c> [has error].</param>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="exception">The exception.</param>
		public ErrorEventArgs(bool hasError, string errorMessage, Exception exception=null) {
			_HasError = hasError;
			_ErrorMessage = errorMessage;
			_Exception = exception;
		}

		/// <summary> Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="exception">The exception.</param>
		public ErrorEventArgs(string errorMessage, Exception exception=null) {
			_HasError = true;
			_ErrorMessage = errorMessage;
			_Exception = exception;
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
		public bool HasError{get {return _HasError;}}
		
		/// <summary> Gets the error message.
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage{get {return _ErrorMessage;}}

		/// <summary> Gets the exception.
		/// </summary>
		/// <value>The exception.</value>
		public Exception Exception{get {return _Exception;}}
	}
}