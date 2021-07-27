using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.BusinessFramework.Tests {

	[TestFixture]
	public class LibTests {

		[Test]
		public void NamespaceMustMatchAssemblyName() {
			var t = typeof (KsWare.Presentation.BusinessFramework.AssemblyInfo);
			var assemblyName = KsWare.Presentation.BusinessFramework.AssemblyInfo.Assembly.GetName(false).Name;
			Assert.That(t.Namespace, Is.EqualTo(assemblyName));
		}
	}
}
