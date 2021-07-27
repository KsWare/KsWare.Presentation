using System.Reflection;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.Core {

	[TestFixture]
	public class BasicTests {

		[Test][Ignore("no longer always true")]
		public void EntryAssemblyMustBeNull() {
			var entryAssembly=Assembly.GetEntryAssembly();
			Assert.IsNull(entryAssembly);
			// but in net5, core31:
			// <ReSharperTestRunner64, Version=1.3.1.57, Culture=neutral, PublicKeyToken=5c492ec4f3eccde3>
			// <testhost.x86, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a>
		}

	}
}
