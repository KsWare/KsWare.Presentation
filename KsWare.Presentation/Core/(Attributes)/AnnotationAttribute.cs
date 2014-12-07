using System;
using System.Diagnostics;

namespace KsWare.Presentation.Annotation {

	[Conditional("__NEVER__")]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class AnnotationAttribute : Attribute {
		public AnnotationAttribute(string description, params object[] content) {  }
		public AnnotationAttribute(string description, params Type[] types) {  }
		public AnnotationAttribute(Inheritance inheritance, params Type[] types) {  }
		public AnnotationAttribute(Hierarchy hierarchy, params Type[] types) {  }
	}

	public enum Inheritance {
		None, 

		/// <summary> subclass, "derived class", heir class, or child class
		/// </summary>
		SubClass, 

		/// <summary> superclass, base class, or parent class
		/// </summary>
		SuperClass
	}

	public enum Hierarchy {
		None,
		Children,
		Parent,
	}
}