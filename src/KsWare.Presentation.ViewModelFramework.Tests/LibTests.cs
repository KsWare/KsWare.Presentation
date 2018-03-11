using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.ViewModelFramework.Tests {

	[TestClass]
	public class LibTests {

		[TestMethod]
		public void NamespaceMustMatchAssemblyName() {
			var t = typeof (KsWare.Presentation.ViewModelFramework.Lib);
			var assemblyName = KsWare.Presentation.ViewModelFramework.Lib.Assembly.GetName(false).Name;
			Assert.That(t.Namespace, Is.EqualTo(assemblyName));
		}
	}
}
