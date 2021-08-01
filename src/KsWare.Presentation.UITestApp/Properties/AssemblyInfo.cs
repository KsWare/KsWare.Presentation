using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("Windows7.0")]
#endif
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "SupportedOSPlatform seems not to work", Scope = "member", Target = "~M:KsWare.Presentation.ViewFramework.Behaviors.WindowChromeBehavior.OnAttached")]

[assembly: ComVisible(false)]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
