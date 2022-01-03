using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: ComVisible(false)]
[assembly: Guid("480179c9-c018-4515-8b83-44e21112887d")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("Windows7.0")]
#endif
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "SupportedOSPlatform seems not to work", Scope = "member", Target = "~M:KsWare.Presentation.ViewFramework.Behaviors.WindowChromeBehavior.OnAttached")]

[assembly: XmlnsDefinition(KsWare.Presentation.ViewFramework.AssemblyInfo.XmlNamespace,"KsWare.Presentation.ViewFramework")]
[assembly: XmlnsPrefix(KsWare.Presentation.ViewFramework.AssemblyInfo.XmlNamespace,"ksv")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "KsWare.Presentation.ViewFramework.GlobalNamespace")]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.ViewFramework {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ksware.de/Presentation/ViewFramework";

		public const string RootNamespace = "KsWare.Presentation.ViewFramework";

	}
}