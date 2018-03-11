

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// namespace must be "System.Runtime.CompilerServices"
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices {

	/*
	Universal Windows Platform		Available since 4.5
	.NET Framework					Available since 4.5
	Portable Class Library			Supported in: portable .NET platforms
	Windows Phone Silverlight		Available since 8.0
	Windows Phone					Available since 8.1
	*/

	/// <summary>
	/// Allows you to obtain the method or property name of the caller to the method.
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
	public sealed class CallerMemberNameAttribute : Attribute {

	}

	/// <summary>
	/// Allows you to obtain the full path of the source file that contains the caller. This is the file path at the time of compile.
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
	public sealed class CallerFilePathAttribute : Attribute {

	}

	/// <summary>
	/// Allows you to obtain the line number in the source file at which the method is called.
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
	public sealed class CallerLineNumberAttribute : Attribute {

	}
}


