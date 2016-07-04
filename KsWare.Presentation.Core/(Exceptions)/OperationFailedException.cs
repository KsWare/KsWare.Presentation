using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	/// <summary> The exception that is thrown when a method call is failed for any reason.
	/// </summary>
	public class OperationFailedException : InvalidOperationException {

		private string _stackTrace;

		public OperationFailedException() {}

		public OperationFailedException(string message, Exception innerException) 
			: base(message, innerException) {}

		public OperationFailedException(string message, StackTrace stackTrace) 
			: base(message) {
			_stackTrace = stackTrace.ToString();
		}

		public OperationFailedException(string message, StackTrace stackTrace, Exception innerException) 
			: base(message, innerException) {}

		public OperationFailedException(string message) 
			: base(message) {}

		public OperationFailedException(Exception innerException) : base("Operation failed.",innerException) {}

		public OperationFailedException(Exception innerException, StackTrace stackTrace) 
			: base("Operation failed.", innerException) {
			_stackTrace = stackTrace.ToString();
		}

		protected OperationFailedException([NotNull] SerializationInfo info, StreamingContext context) 
			: base(info, context) {}

		public OperationFailedException(string message, Exception innerException, StackTrace stackTrace) : base(message, innerException) {
			_stackTrace = stackTrace.ToString();
		}

		public override string StackTrace { get { return _stackTrace?? base.StackTrace; } }

	}
}
