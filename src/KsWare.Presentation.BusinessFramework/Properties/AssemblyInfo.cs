﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: ComVisible(false)]
[assembly: Guid("816f4a41-a808-460e-8e5a-d048934b6a5d")]

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

[assembly: InternalsVisibleTo("KsWare.Presentation.ViewModelFramework, PublicKey=002400000480000094000000060200000024000052534131000400000100010041A176886DD69E39D1B9A10017A286FBF650E8F0CB84879B097856B3DFDD194CB06561F36780A9AD61BA8A69DEC80B4FAE69D8723BD67ED3052E82A10E221159DF072118B887BE867A299EB12A1256741E0230FDECF6BA9A806034E1C543EABD9E2CC21DCBE9B61463DB6635B0867EDA7E588409A155D97E17162257B61DECB4")]
[assembly: InternalsVisibleTo("KsWare.Presentation.Labs, PublicKey=002400000480000094000000060200000024000052534131000400000100010041A176886DD69E39D1B9A10017A286FBF650E8F0CB84879B097856B3DFDD194CB06561F36780A9AD61BA8A69DEC80B4FAE69D8723BD67ED3052E82A10E221159DF072118B887BE867A299EB12A1256741E0230FDECF6BA9A806034E1C543EABD9E2CC21DCBE9B61463DB6635B0867EDA7E588409A155D97E17162257B61DECB4")]

[assembly: XmlnsDefinition(KsWare.Presentation.BusinessFramework.AssemblyInfo.XmlNamespace, "KsWare.Presentation.BusinessFramework")]
[assembly: XmlnsPrefix("http://ksware.de/Presentation/BusinessFramework", "ksb")]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.BusinessFramework {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ksware.de/Presentation/BusinessFramework";

	}
}