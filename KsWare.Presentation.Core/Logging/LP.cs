using System;
using System.Globalization;
using KsWare.Presentation;

namespace KsWare.Presentation.Core.Logging
{
	/// <summary> Provides information about a parameter
	/// </summary>
	public class LogParameter
	{
		private readonly string name;
		private readonly object value;

		/// <summary> Initializes a new instance of the <see cref="LogParameter"/> class.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="value">The value of the parameter</param>
		public LogParameter(string name,object value) {
			this.name = name;
			this.value = value;
		}

		/// <summary> Gets the name of the parameter
		/// </summary>
		/// <value>The name.</value>
		public string Name {get {return this.name;}}

		/// <summary> Gets the value of the parameter
		/// </summary>
		/// <value>The value.</value>
		public object Value {get {return this.value;}}

	}

	/// <summary>
	/// Class to get the string of the exception message
	/// </summary>
	public class ExceptionParameter:LogParameter{
// ReSharper disable InconsistentNaming
		private static readonly IFormatProvider enUS = CultureInfo.CreateSpecificCulture("en-US");
// ReSharper restore InconsistentNaming

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionParameter"/> class.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="value">The value of the parameter</param>
		public ExceptionParameter(string name, object value): base(name, value) {}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString() {
			string valueString;
			if(Value==null) {
				valueString = "{NULL}";
			} else if(Value.ToString()==Value.GetType().Name) {
				valueString = Value.GetType().ToString("{G}"+"#"+Value.TypeInstanceId());
			} else {
				valueString = Value.ToString();
			}
			return string.Format(enUS, "\r\n\t{0}: {1}",Name, valueString);
		}
	}


}