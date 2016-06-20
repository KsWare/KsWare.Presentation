using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	/// <summary>
	/// 
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
	[Serializable]
	public sealed class MultiTargetInvocationException : Exception {

		private static readonly TargetInvocationException SampleTargetInvocationException=new TargetInvocationException(new Exception("dummy")); 
		
		private readonly List<Exception> m_Exceptions=new List<Exception>();

		/// <summary> Initializes a new instance of the <see cref="MultiTargetInvocationException"/> class.
		/// </summary>
		public MultiTargetInvocationException(IEnumerable<Exception> exceptions) : base(SampleTargetInvocationException.Message) {
			m_Exceptions.AddRange(exceptions);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiTargetInvocationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exceptions">The exceptions.</param>
		public MultiTargetInvocationException(string message,[NotNull] IEnumerable<TargetInvocationException> exceptions): base(message) {
			if (exceptions == null) 
				throw new ArgumentNullException("exceptions");
			
			m_Exceptions.AddRange(exceptions);
		}

//		/// <summary> Initializes a new instance of the <see cref="MultiTargetInvocationException"/> class.
//		/// </summary>
//		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
//		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
//		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
//		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
//		protected MultiTargetInvocationException([NotNull] SerializationInfo info, StreamingContext context): base(info, context) {
//			exceptions=(List<TargetInvocationException>) info.GetValue("Exceptions",typeof(List<TargetInvocationException>));
//		}

		/// <summary> When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
		///   
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
		///   </PermissionSet>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2236:CallBaseClassMethodsOnISerializableTypes")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			throw new NotImplementedException("{B745DBDF-CE6C-40D0-B67E-0A6BEB387AD7}");
			//REVISE not used / not tested
//			base.GetObjectData(info, context);
//			info.AddValue("Exceptions",exceptions,typeof(List<TargetInvocationException>));
		}

		/// <summary> Gets the exceptions.
		/// </summary>
		/// <value>The exceptions.</value>
		public ReadOnlyCollection<Exception> Exceptions {
			get {
				return m_Exceptions.AsReadOnly();
			}
		}

	}
}
