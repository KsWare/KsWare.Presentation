using System;
using KsWare.Presentation.BusinessFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Core.CLR452.Tests {
	[TestClass]
	public class UnitTest1 {

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

		public int Int32 { get { return Fields.Get2<int>(); } set { Fields.Set2(value); } }

		public string GetCallerName([System.Runtime.CompilerServices.CallerMemberName] string name =null) { return name; }
	}
}
