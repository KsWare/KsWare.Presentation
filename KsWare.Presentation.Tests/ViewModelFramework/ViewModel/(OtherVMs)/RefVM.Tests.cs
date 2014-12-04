using System;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewFramework {

	[TestClass]
	public class RefVMTests {

		[TestMethod]
		public void T001_Target_Set() {
			var refVM = new RefVM<StringVM>();
			refVM.Target=new StringVM();
		}

		[TestMethod]
		public void T002_TargetChanged_MustRaised() {
			var refVM = new RefVM<StringVM>();
			int targetChangedCounter=0;
			refVM.TargetChanged += (s, e) => targetChangedCounter++;
			refVM.Target=new StringVM();
			Assert.AreEqual(1,targetChangedCounter);
		}

		[TestMethod]
		public void T003_TargetChangedEvent_MustRaised() {
			var refVM = new RefVM<StringVM>();
			int targetChangedCounter=0;
			refVM.TargetChangedEvent.Register(this,"TargetChanged", (s, e) => targetChangedCounter++);
			refVM.Target=new StringVM();
			Assert.AreEqual(1,targetChangedCounter);
		}
	}
}
