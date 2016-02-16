using System;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	/// <summary> Provides the InvokeMethod <see cref="Attribute"/>
	/// </summary>
	/// <remarks></remarks>
	[Obsolete]
	[AttributeUsage(AttributeTargets.Method,AllowMultiple = false,Inherited = true)]
	public sealed class InvokeMethodAttribute:Attribute  {

		/// <summary> Initializes a new instance of the <see cref="InvokeMethodAttribute"/> class.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <remarks></remarks>
		public InvokeMethodAttribute(string methodName) { MethodName = methodName; }

		/// <summary> Gets the name of the method.
		/// </summary>
		/// <remarks></remarks>
		public string MethodName{get;private set;}
	}
}
