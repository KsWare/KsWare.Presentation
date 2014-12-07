using System;
using System.Collections.Generic;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	[TestClass]
	public class ValueVMWeakEventTests:ApplicationVMTestBase {

		[TestInitialize] public override void TestInitialize() {base.TestInitialize();}
		[TestCleanup   ] public override void TestCleanup() {base.TestCleanup();}

		[TestMethod]
		public void A() {
			var count = 0;
			var vm = new Int32VM();
			vm.ValueChangedEvent.Register(this,"{D9FCCC5F-79B6-4DC2-B4EC-EC734EA2EC11}",delegate(object sender, ValueChangedEventArgs args) { count++; });
			vm.Value = 1;
			Assert.AreEqual(1,count);

			EventUtil.WeakEventManager.Release(this,"{D9FCCC5F-79B6-4DC2-B4EC-EC734EA2EC11}");
			count = 0;
			vm.Value = 2;
			Assert.AreEqual(0,count);
		}


		[TestMethod,Ignore /*TODO*/]
		public void Collect() {
			var count = 0;
			var vm = new Int32VM();

			var c = EventUtil.WeakEventManager.Count;
			var i = TestObject.LivingInstances;

			for (int j = 0; j < 1000; j++) {
				if (j%50==0) {
					GC.Collect();GC.GetTotalMemory(true);
					EventUtil.WeakEventManager.Collect();
				}
				var c0 = EventUtil.WeakEventManager.Count;
				var i0 = TestObject.LivingInstances;
				Assert.AreEqual(c0,i0);
				var destination = new TestObject();
				vm.ValueChangedEvent.Register(destination,"{D9FCCC5F-79B6-4DC2-B4EC-EC734EA2EC11}",delegate { count++; });
				count = 0;
				vm.Value++;
				Assert.AreEqual(TestObject.LivingInstances,count);
			}
		}

		public class TestObject {

			public static int LivingInstances;
			public static int NextID;
			public static List<int> list=new List<int>(); 

			public TestObject() { LivingInstances++; ID=NextID++; list.Add(ID);}
			~TestObject() { LivingInstances--; list.Remove(ID);}

			public void ValueChangedCallback(object sender, EventArgs eventArgs) {}

			public int ID { get; set; }
			public int Count { get; set; }
			
		}
	}
}
