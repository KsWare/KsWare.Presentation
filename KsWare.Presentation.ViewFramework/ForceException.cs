using System;
using System.Diagnostics;

namespace KsWare.Presentation.ViewFramework {

	[DebuggerStepThrough][DebuggerNonUserCode]
	public class ForceException {

		public ForceException() { throw new Exception("ForceException"); }

		public static Uri Uri { get { throw new Exception("ForceException"); } }

	}


}