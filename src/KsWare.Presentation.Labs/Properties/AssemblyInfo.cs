using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: ComVisible(false)]
[assembly: Guid("b2428431-ad89-4221-8b7c-4109afa93690")]

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

[assembly: XmlnsPrefix(KsWare.Presentation.Labs.AssemblyInfo.XmlNamespace, "kslab")]
[assembly: XmlnsDefinition(KsWare.Presentation.Labs.AssemblyInfo.XmlNamespace, "KsWare.Presentation.ViewFramework")]
[assembly: XmlnsDefinition(KsWare.Presentation.Labs.AssemblyInfo.XmlNamespace, "KsWare.Presentation.ViewModelFramework")]
[assembly: XmlnsDefinition(KsWare.Presentation.Labs.AssemblyInfo.XmlNamespace, "KsWare.Presentation.BusinessFramework")]
[assembly: XmlnsDefinition(KsWare.Presentation.Labs.AssemblyInfo.XmlNamespace, "KsWare.Presentation.Core")]
[assembly: XmlnsDefinition(KsWare.Presentation.Labs.AssemblyInfo.XmlNamespace, "KsWare.Presentation")]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.Labs {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ksware.de/Presentation/Labs";

		public const string RootNamespace = "KsWare.Presentation";
	}
}