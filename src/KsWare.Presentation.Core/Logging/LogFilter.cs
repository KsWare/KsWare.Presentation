using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace KsWare.Presentation.Core.Logging {
	/// <summary> Log filter class for the logging messages
	/// </summary>
	public class LogFilter {
		/// <summary>
		/// Gets or sets a value indicating whether this instance is none.
		/// </summary>
		/// <value><c>true</c> if this instance is none; otherwise, <c>false</c>.</value>
		public bool IsNone{get;set;}
		/// <summary>
		/// Gets or sets the assembly.
		/// </summary>
		/// <value>The assembly.</value>
		public Assembly Assembly{get;set;}
		/// <summary>
		/// Gets or sets the module.
		/// </summary>
		/// <value>The module.</value>
		public Module Module{get;set;}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		public Type Type{get;set;}

		/// <summary> Gets or sets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public object Instance{get;set;}

		/// <summary>
		/// Gets the none.
		/// </summary>
		/// <value>The none.</value>
		public static LogFilter None => new LogFilter{IsNone = true};
	}
}
