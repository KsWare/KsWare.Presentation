/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : ValidationException.cs
 * OriginalNamespace: KsWare.Presentation.BusinessFramework
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Exception for validation
	/// </summary>
	[Serializable] //Added for FxCop
	public class ValidationException: BusinessModelException {

		/// <summary> Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		public ValidationException(): base(BusinessModelError.ValueValidationError, null, null) { }

		/// <summary> Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="innerException">The inner exception.</param>
		public ValidationException(Exception innerException): base(BusinessModelError.ValidationError, null, innerException) { }

		/// <summary> Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="errorValue">The error value.</param>
		protected ValidationException(BusinessModelError errorValue): base(errorValue, null, null) { }

		/// <summary> Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="errorValue">The error value.</param>
		/// <param name="businessObject">The business object.</param>
		protected ValidationException(BusinessModelError errorValue,IObjectBM businessObject): base(errorValue, businessObject, null) { }

		/// <summary> Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="errorValue">The error value.</param>
		/// <param name="innerException">The inner exception.</param>
		protected ValidationException(BusinessModelError errorValue, Exception innerException): base(errorValue, null, innerException) { }

		/// <summary> Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="errorValue">The error value.</param>
		/// <param name="businessObject">The business object.</param>
		/// <param name="innerException">The inner exception.</param>
		protected ValidationException(BusinessModelError errorValue,IObjectBM businessObject, Exception innerException): base(errorValue, businessObject, innerException) { }

		/// <summary> Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		[Obsolete("Not implemented")]
		protected ValidationException(SerializationInfo info, StreamingContext context): base(info, context) {
			throw new NotImplementedException("{C9D6036F-41B3-4482-B1B1-C845307BE6BA}");
		}

		/// <summary> [Obsolete] Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="obsolete">The obsolete.</param>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete")]
		[Obsolete("Obsolete. Use a more specific constructor",true)]
		public ValidationException(String obsolete){/*Added for FxCop*/}

		/// <summary> [Obsolete] Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="obsolete1">The obsolete1.</param>
		/// <param name="obsolete2">The obsolete2.</param>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete1")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete2")]
		[Obsolete("Obsolete Use a more specific constructor",true)]
		public ValidationException(String obsolete1, Exception obsolete2){/*Added for FxCop*/}

		/// <summary> Gets the object data.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
		}

		//TODO implement ValidationException
	}

	/// <summary> Exception for value validation
	/// </summary>
	[Serializable] //Added for FxCop
	public class ValueValidationException: ValidationException {

		private readonly object _Value;


		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		/// <param name="obsolete">The obsolete.</param>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete")]
		[Obsolete("Obsolete. Use a more specific constructor",true)]
		public ValueValidationException(String obsolete){/*Added for FxCop*/}

		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		/// <param name="obsolete1">The obsolete1.</param>
		/// <param name="obsolete2">The obsolete2.</param>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete1")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete2")]
		[Obsolete("Obsolete Use a more specific constructor",true)]
		public ValueValidationException(String obsolete1, Exception obsolete2){/*Added for FxCop*/}

		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		public ValueValidationException(): base(BusinessModelError.ValueValidationError) { }

		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		/// <param name="innerException">The inner exception.</param>
		public ValueValidationException(Exception innerException): base(BusinessModelError.ValueValidationError, innerException) { }

		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		public ValueValidationException(object value): base(BusinessModelError.ValueValidationError) {
			_Value = value;
		}

		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="innerException">The inner exception.</param>
		public ValueValidationException(object value, Exception innerException): base(BusinessModelError.ValueValidationError, innerException) {
			_Value = value;
		}

		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="businessObject">The business object.</param>
		public ValueValidationException(object value, IObjectBM businessObject): base(BusinessModelError.ValueValidationError, businessObject) {
			_Value = value;
		}

		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="businessObject">The business object.</param>
		/// <param name="innerException">The inner exception.</param>
		public ValueValidationException(object value, IObjectBM businessObject, Exception innerException): base(BusinessModelError.ValueValidationError, businessObject,innerException) {
			_Value = value;
		}

		/// <summary> Initializes a new instance of the <see cref="ValueValidationException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		[Obsolete("Not implemented")]
		protected ValueValidationException(SerializationInfo info, StreamingContext context): base(info, context) {
			_Value = info.GetValue("Value",typeof(object));
		}

		/// <summary> Gets the object data.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("Value",_Value);
		}
		
		/// <summary> Gets the erroneous value.
		/// </summary>
		/// <value>The erroneous value.</value>
		public object Value => _Value;

		//TODO implement ValueValidationException
	}
}