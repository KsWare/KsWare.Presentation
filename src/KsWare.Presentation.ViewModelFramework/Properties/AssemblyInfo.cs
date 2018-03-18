using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KsWare Presentation - ViewModel Framework")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("KsWare")]
[assembly: AssemblyProduct("Presentation Framework")]
[assembly: AssemblyCopyright("Copyright © 2002-2018 by KsWare. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("e45e9672-0236-47fc-9ade-5fef5cdfc7ae")]

[assembly: AssemblyVersion("0.18.4")]
[assembly: AssemblyFileVersion("0.18.4")]
[assembly: AssemblyInformationalVersion("0.18.4+20180318052432")]

[assembly: XmlnsDefinition(KsWare.Presentation.ViewModelFramework.AssemblyInfo.XmlNamespace, "KsWare.Presentation.ViewModelFramework")]
[assembly: XmlnsPrefix("http://ksware.de/Presentation/ViewModelFramework", "ksvm")]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.ViewModelFramework {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ksware.de/Presentation/ViewModelFramework";

	}
}