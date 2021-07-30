using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: ComVisible(false)]
[assembly: Guid("480179c9-c018-4515-8b83-44e21112887d")]

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