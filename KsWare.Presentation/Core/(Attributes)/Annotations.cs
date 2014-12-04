using System;
using System.Diagnostics;

namespace KsWare.Presentation {

	/// <summary> Provides references to navigate between related documents.
	/// </summary>
	/// <example>
	/// <code>	
	/// [Docu("Parent",typeof(MyParentViewModel))]
	/// [Docu("View",typeof(MyView))]</code>
	/// </example>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true),Conditional("NEVER")]
	[Obsolete("Use AnnotationAttribute",true)]
	public class DocuAttribute:Attribute {

		public DocuAttribute(string key, Type symbol, string comment=null) {}

		public DocuAttribute(string key, object symbol, string comment=null) {}

		public DocuAttribute(string key, string symbol, string comment=null) {}
	}
}
