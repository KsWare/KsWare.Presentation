using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.ViewModelFramework.Tests {

	[TestFixture]
	public class LibTests {

		[Test]
		public void NamespaceMustMatchAssemblyName() {
			var t = typeof (KsWare.Presentation.ViewModelFramework.AssemblyInfo);
			var assemblyName = KsWare.Presentation.ViewModelFramework.AssemblyInfo.Assembly.GetName(false).Name;
			Assert.That(t.Namespace, Is.EqualTo(assemblyName));
		}
	}
}
