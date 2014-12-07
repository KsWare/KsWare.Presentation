#if(!NET_45 && !NET_451)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Runtime.CompilerServices {

	[AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
	public sealed class CallerMemberNameAttribute:Attribute {

	}

	[AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
	public sealed class CallerFilePathAttribute:Attribute {

	}

	[AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
	public sealed class CallerLineNumberAttribute:Attribute {

	}
}

#endif
