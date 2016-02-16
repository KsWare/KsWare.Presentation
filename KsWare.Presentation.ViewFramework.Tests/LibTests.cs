using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.ViewFramework.Tests {

	[TestClass]
	public class LibTests {

		[TestMethod]
		public void NamespaceMustMatchAssemblyName() {
			var t = typeof (KsWare.Presentation.ViewFramework.Lib);
			var assemblyName = KsWare.Presentation.ViewFramework.Lib.Assembly.GetName(false).Name;
			Assert.That(t.Namespace, Is.EqualTo(assemblyName));
		}
	}
}
