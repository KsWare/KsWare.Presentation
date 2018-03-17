using System;
using KsWare.Presentation.BusinessFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Core.CLR452.Tests {

	[TestClass]
	public class CallerNameTests {

		[TestMethod]
		public void TestMethod1() {
			var bm=new TestBM();
			bm.Int32 = 1;
			Assert.AreEqual(1,bm.Int32);
		}
	}

	public class TestBM:ObjectBM{

		public TestBM() {
			RegisterChildren(()=>this);
		}

		public int Int32 { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }

		public string GetCallerName([System.Runtime.CompilerServices.CallerMemberName] string name =null) { return name; }
	}
}
