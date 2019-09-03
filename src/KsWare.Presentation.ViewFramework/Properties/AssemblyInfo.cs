using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Ksware Presentation - View Framework")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("KsWare")]
[assembly: AssemblyProduct("Presentation Framework")]
[assembly: AssemblyCopyright("Copyright © 2002-2018 by KsWare. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("480179c9-c018-4515-8b83-44e21112887d")]

[assembly: AssemblyVersion("0.18.40")]
[assembly: AssemblyFileVersion("0.18.40")]
[assembly: AssemblyInformationalVersion("0.18.40")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsDefinition(KsWare.Presentation.ViewFramework.AssemblyInfo.XmlNamespace,"KsWare.Presentation.ViewFramework")]
[assembly: XmlnsPrefix(KsWare.Presentation.ViewFramework.AssemblyInfo.XmlNamespace,"ksv")]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.ViewFramework {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ksware.de/Presentation/ViewFramework";

		public const string RootNamespace = "KsWare.Presentation.ViewFramework";

	}
}