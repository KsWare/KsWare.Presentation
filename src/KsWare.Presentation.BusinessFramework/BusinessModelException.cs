using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> BusinessModelException
	/// </summary>
	[Serializable] //Added for FxCop
	public class BusinessModelException: Exception {

		private BusinessModelError _errorNumber;
		private IObjectBM _businessObject;

		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessModelException"/> class.
		/// </summary>
		public BusinessModelException(): this(BusinessModelError.Undefined, null, null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessModelException"/> class.
		/// </summary>
		/// <param name="errorNumber">The error number.</param>
		/// <param name="businessObject">The business object.</param>
		public BusinessModelException(BusinessModelError errorNumber, IObjectBM businessObject): this(errorNumber, businessObject, null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessModelException"/> class.
		/// </summary>
		/// <param name="errorNumber">The error number.</param>
		/// <param name="businessObject">The business object.</param>
		/// <param name="innerException">The inner exception.</param>
		public BusinessModelException(BusinessModelError errorNumber, IObjectBM businessObject, Exception innerException): base(CreateMessage(errorNumber), innerException) {
			_errorNumber = errorNumber;
			_businessObject = businessObject;
		}

		/// <summary>[Obsolete] Initializes a new instance of the <see cref="BusinessModelException"/> class.
		/// </summary>
		/// <param name="obsolete">The obsolete.</param>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete")]
		[Obsolete("Obsolete. Use a more specific constructor",true)]
		public BusinessModelException(String obsolete){/*Added for FxCop*/}

		/// <summary> [Obsolete] Initializes a new instance of the <see cref="BusinessModelException"/> class.
		/// </summary>
		/// <param name="obsolete1">The obsolete1.</param>
		/// <param name="obsolete2">The obsolete2.</param>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete1")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obsolete2")]
		[Obsolete("Obsolete Use a more specific constructor",true)]
		public BusinessModelException(String obsolete1, Exception obsolete2){/*Added for FxCop*/}

		/// <summary> Initializes a new instance of the <see cref="BusinessModelException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected BusinessModelException(SerializationInfo info, StreamingContext context): base(info, context) {
			this._errorNumber = (BusinessModelError) info.GetValue("ErrorNumber",typeof (BusinessModelError));
		}

		/// <summary> When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
		/// </PermissionSet>
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("ErrorNumber",_errorNumber);
		}

		/// <summary> Gets the error number.
		/// </summary>
		/// <value>The error number.</value>
		public BusinessModelError ErrorNumber{get {return _errorNumber;}}

		/// <summary> Gets the business object.
		/// </summary>
		/// <value>The business object.</value>
		public IObjectBM BusinessObject{get {return _businessObject;}}

		private static string CreateMessage(BusinessModelError error) {
			//TODO implement CreateMessage
			return error.ToString();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public enum BusinessModelError {

		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// 
		/// </summary>
		Undefined,
		/// <summary>
		/// 
		/// </summary>
		ValidationError,
		/// <summary>
		/// 
		/// </summary>
		ValueValidationError,
	}
}