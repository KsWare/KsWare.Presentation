using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.ViewFramework {

	[TestFixture]
	public class RefVMTests {

		[Test]
		public void T001_Target_Set() {
			var refVM = new RefVM<StringVM>();
			refVM.Target=new StringVM();
		}

		[Test]
		public void T002_TargetChanged_MustRaised() {
			var refVM = new RefVM<StringVM>();
			int targetChangedCounter=0;
#pragma warning disable 618
			refVM.TargetChanged += (s, e) => targetChangedCounter++;
#pragma warning restore 618
			refVM.Target=new StringVM();
			Assert.AreEqual(1,targetChangedCounter);
		}

		[Test]
		public void T003_TargetChangedEvent_MustRaised() {
			var refVM = new RefVM<StringVM>();
			int targetChangedCounter=0;
			refVM.TargetChangedEvent.Register(this,"TargetChanged", (s, e) => targetChangedCounter++);
			refVM.Target=new StringVM();
			Assert.AreEqual(1,targetChangedCounter);
		}
	}
}
