using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
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

	[SerializableAttribute]
	public class PropertyOutOfRangeException : InvalidOperationException /*ArgumentOutOfRangeException*/ {

		private const string enM = "The property was out of the range of valid values.";
		private const string enP = "Property name";
		private const string enV = "Actual value was";

		public PropertyOutOfRangeException(string propertyName, object actualValue, string message) : base(FormatMessage(propertyName, actualValue, message)) {
			PropertyName = propertyName;
			ActualValue = actualValue;
		}

		public PropertyOutOfRangeException():base(enM) { }

		public PropertyOutOfRangeException(string propertyName) : base(FormatMessage(propertyName)) {
			PropertyName = propertyName;
		}

		public PropertyOutOfRangeException(string message, Exception innerException) : base(message, innerException) {}

		public PropertyOutOfRangeException(string propertyName, string message) : base(FormatMessage(propertyName,message)) {
			PropertyName = propertyName;
		}

		protected PropertyOutOfRangeException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) {}

		private static string FormatMessage(string propertyName) {
			return string.Format("{0}\r\n{1}: {2}", enM,enP, propertyName);
		}

		private static string FormatMessage(string propertyName, string message) {
			return string.Format("{0}\r\n{1}: {2}", message,enP, propertyName);
		}

		private static string FormatMessage(string propertyName, object actualValue, string message) {
			return string.Format("{0}\r\n{1}: {2}\r\n{3} {4}", message,enP, propertyName,enV, actualValue);
		}

		public object ActualValue { get; private set; }

		public string PropertyName { get; private set; }

	}
}
