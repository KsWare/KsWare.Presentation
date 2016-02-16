using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	[SerializableAttribute]
	public class PropertyNullException : InvalidOperationException /*ArgumentNullException*/ {
		private const string enM = "Property cannot be null.";
		private const string enP = "Property name";

		public PropertyNullException():base("Property cannot be null.") {}

		public PropertyNullException(string propertyName) : base(FormatMessage(propertyName)) {
			PropertyName = propertyName;
		}

		public PropertyNullException(string message, Exception innerException) : base(message, innerException) {}

		public PropertyNullException(string propertyName, string message) : base(FormatMessage(propertyName, message)) {
			PropertyName = propertyName;
		}

		protected PropertyNullException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) {}

		private static string FormatMessage(string propertyName) {
			return string.Format("{0}\r\n{1}: {2}", enM,enP, propertyName);
		}

		private static string FormatMessage(string propertyName, string message) {
			return string.Format("{0}\r\n{1}: {2}", message,enP, propertyName);
		}

		public string PropertyName { get; private set; }
	}

}
