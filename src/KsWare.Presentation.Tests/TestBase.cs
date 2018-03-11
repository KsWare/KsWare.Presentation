namespace KsWare.Presentation.Testing {

	public class TestBase {
		public virtual void TestInitialize() { }
		public virtual void TestCleanup() { }
	}

	public class TestBase<TSubject>:TestBase {

	}
}
