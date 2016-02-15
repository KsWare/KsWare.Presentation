using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using System.Windows.Markup;

[assembly: AssemblyTitle("KsWare Presentation Framework")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("KsWare")]
[assembly: AssemblyProduct("Presentation Framework")]
[assembly: AssemblyCopyright("Copyright © 2002-2016 by KsWare. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: InternalsVisibleTo("KsWare.Presentation.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d3a0b493bda975526f908712b964179df870a583523eb8470b1aa09d97f61b665d1f96d4f495751c3f0d6ff08dc49bf4b22731be741e1c8861af28f7453b1de06bbfd4b5ebaf5f3bf928480ebe8a47877ee1c3e48ba4f906729ec9234934e5affb8e7e46b7451ffb0ff1298dc09bdea2dd0c1b1dcae7c409c1b8868c5af2a1ba")]

// use of KsWare.Presentation.ViewFramework.XmlDocMarkup without prefix required
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "KsWare.Presentation.ViewFramework.XmlDocMarkup")]

[assembly: XmlnsDefinition("http://schemas.ksware.de/netfx/2014/xaml/presentation", "KsWare.Presentation.ViewFramework.XmlDocMarkup")]
//[assembly: XmlnsPrefix("http://schemas.microsoft.com/netfx/2014/xaml/presentation", "")]