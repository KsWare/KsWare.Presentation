using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Core.Tests {

	[TestClass]
	public class LibTests {

		[TestMethod]
		public void NamespaceMustMatchAssemblyName() {
			var t = typeof (KsWare.Presentation.Core.AssemblyInfo);
			var assemblyName = KsWare.Presentation.Core.AssemblyInfo.Assembly.GetName(false).Name;
			Assert.That(t.Namespace, Is.EqualTo(assemblyName));
		}
	}
}
