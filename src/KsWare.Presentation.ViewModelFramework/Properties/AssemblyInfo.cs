using System.Reflection;
using System.Runtime.CompilerServices;
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

[assembly: AssemblyVersion("0.18.38")]
[assembly: AssemblyFileVersion("0.18.38")]
[assembly: AssemblyInformationalVersion("0.18.38")]

[assembly: InternalsVisibleTo("KsWare.Presentation.ViewFramework, PublicKey=002400000480000094000000060200000024000052534131000400000100010041A176886DD69E39D1B9A10017A286FBF650E8F0CB84879B097856B3DFDD194CB06561F36780A9AD61BA8A69DEC80B4FAE69D8723BD67ED3052E82A10E221159DF072118B887BE867A299EB12A1256741E0230FDECF6BA9A806034E1C543EABD9E2CC21DCBE9B61463DB6635B0867EDA7E588409A155D97E17162257B61DECB4")]

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