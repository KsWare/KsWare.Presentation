using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	[TestFixture]
	public class ApplicationVMTests:ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public override void TestInitialize() {base.TestInitialize(); }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public override void TestCleanup() {base.TestCleanup(); }

		[Test]
		public void Current_IsNotNull() {
			Assert.IsNotNull(ApplicationVM.Current);
		}

		[Test]
		public void Current_Dispatcher_IsNotNull() {
			Assert.IsNotNull(ApplicationVM.Current.Dispatcher);
		}
	}
}
