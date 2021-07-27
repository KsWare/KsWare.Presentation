using NUnit.Framework;

namespace EmptyNunitTestProjectMT {
	public class Tests {
		[SetUp]
		public void Setup() {
		}

		[Test]
		public void Test1() {
			Assert.Pass();
		}
	}
}