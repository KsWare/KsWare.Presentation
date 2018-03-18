using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KsWare Presentation - Labs")]
[assembly: AssemblyDescription("provides experimental functionality")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("KsWare")]
[assembly: AssemblyProduct("Presentation Framwork")]
[assembly: AssemblyCopyright("Copyright © 2002- 2018 by KsWare. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("b2428431-ad89-4221-8b7c-4109afa93690")]

[assembly: AssemblyVersion("0.18.3")]
[assembly: AssemblyFileVersion("0.18.3")]
[assembly: AssemblyInformationalVersion("0.18.3+20180318042901")]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.Labs {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

	}
}