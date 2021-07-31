using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("35cde7f2-ecbb-469e-8a4b-9027e626554e")]

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("Windows7.0")]
#endif
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "SupportedOSPlatform seems not to work", Scope = "member", Target = "~M:KsWare.Presentation.ViewFramework.Behaviors.WindowChromeBehavior.OnAttached")]
