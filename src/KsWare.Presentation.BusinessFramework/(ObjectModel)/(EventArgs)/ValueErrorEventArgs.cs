using System;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides data for the ValueError-event
	/// </summary>
	public class ValueErrorEventArgs: EventArgs {

		private BusinessModelException _exception;

		/// <summary> Initializes a new instance of the <see cref="ValueErrorEventArgs"/> class.
		/// </summary>
		/// <param name="exception">The exception.</param>
		public ValueErrorEventArgs(BusinessModelException exception) {
			if (exception == null) throw new ArgumentNullException(nameof(exception));
			this._exception = exception;
		}

		/// <summary> Initializes a new instance of the <see cref="ValueErrorEventArgs"/> class.
		/// </summary>
		/// <param name="error">The error.</param>
		/// <param name="businessObject">The business object.</param>
		public ValueErrorEventArgs(BusinessModelError error, IObjectBM businessObject) { this._exception = new BusinessModelException(error, businessObject); }

		/// <summary> Gets the exception.
		/// </summary>
		/// <value>The exception.</value>
		public BusinessModelException Exception{get {return this._exception;}}
		
		/// <summary> Gets the business object.
		/// </summary>
		/// <value>The business object.</value>
		public IObjectBM BusinessObject{get {return this._exception.BusinessObject;}}
		
		/// <summary> Gets the error number.
		/// </summary>
		/// <value>The error number.</value>
		public BusinessModelError ErrorNumber{get {return this._exception.ErrorNumber;}}
	}
}