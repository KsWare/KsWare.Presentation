using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.Core {

	[TestClass]
	public class BasicTests {

		[TestMethod]
		public void EntryAssemblyMustBeNull() {
			var entryAssembly=Assembly.GetEntryAssembly();
			Assert.IsNull(entryAssembly);
		}

	}
}
