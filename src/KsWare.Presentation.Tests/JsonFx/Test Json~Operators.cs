using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KsWare.Presentation.JsonFx;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Test.JsonFx.Test {
	
	[TestClass]
	public class Test_Json_Operators {

		[TestMethod] public void OpJsonDoubleAddJsonDouble(){Assert.AreEqual(5.0, (new Json(2.0,false,true) + new Json(3.0,false,true)).NativeValue);}
		[TestMethod] public void OpJsonDoubleSubJsonDouble(){Assert.AreEqual(2.0, (new Json(5.0,false,true) - new Json(3.0,false,true)).NativeValue);}
		[TestMethod] public void OpJsonDoubleMulJsonDouble(){Assert.AreEqual(6.0, (new Json(2.0,false,true) * new Json(3.0,false,true)).NativeValue);}
		[TestMethod] public void OpJsonDoubleDivJsonDouble(){Assert.AreEqual(2.0, (new Json(6.0,false,true) / new Json(3.0,false,true)).NativeValue);}

		[TestMethod] public void OpJsonDoubleAddDouble(){Assert.AreEqual(5.0, (new Json(2.0,false,true) + 3).NativeValue);}

		[TestMethod] public void OpJsonDoubleAddInt32(){Assert.AreEqual(5.0, (new Json(2.0,false,true) + 3).NativeValue);}
		[TestMethod] public void OpJsonDoubleAddInt64(){Assert.AreEqual(5.0, (new Json(2.0,false,true) + 3L).NativeValue);}
		[TestMethod] public void OpJsonDoubleAddStringInt(){Assert.AreEqual(5.0, (new Json(2.0,false,true) + "3").NativeValue);}
		[TestMethod] public void OpJsonDoubleAddStringDouble(){Assert.AreEqual(5.0, (new Json(2.0,false,true) + "3.0").NativeValue);}

		[TestMethod] public void OpJsonDoubleMulInt32(){Assert.AreEqual(6.0, (new Json(2.0,false,true) * 3).NativeValue);}
		[TestMethod] public void OpJsonDoubleMulInt64(){Assert.AreEqual(6.0, (new Json(2.0,false,true) * 3L).NativeValue);}
		[TestMethod] public void OpJsonDoubleMulStringInt(){Assert.AreEqual(6.0, (new Json(2.0,false,true) * "3").NativeValue);}
		[TestMethod] public void OpJsonDoubleMulStringDouble(){Assert.AreEqual(6.0, (new Json(2.0,false,true) * "3.0").NativeValue);}

		[TestMethod] public void OpJsonStringAddJsonStringA(){Assert.AreEqual("Ab", (new Json("A",false,true) + new Json("b",false,true)).NativeValue);}
		[TestMethod] public void OpJsonStringAddJsonStringB(){Assert.AreEqual("50", (new Json("5",false,true) + new Json("0",false,true)).NativeValue);}
		[TestMethod] public void OpJsonStringAddInt32A(){Assert.AreEqual("A1", (new Json("A",false,true) + 1).NativeValue);}
		[TestMethod] public void OpJsonStringAddDoubleA(){Assert.AreEqual("A1", (new Json("A",false,true) + 1.0).NativeValue);}
		[TestMethod] public void OpJsonStringAddInt32B(){Assert.AreEqual("11", (new Json("1",false,true) + 1).NativeValue);}
		[TestMethod] public void OpJsonStringAddDoubleB(){Assert.AreEqual("11", (new Json("1",false,true) + 1.0).NativeValue);}

		[TestMethod] public void OpJsonDoubleAddAddA(){var j=new Json(1.0,false,true);j++; Assert.AreEqual(2.0, j.D);}
		[TestMethod] public void OpJsonDoubleAddAddB(){var j=new Json(1.0,false,true);++j; Assert.AreEqual(2.0, j.D);}
		[TestMethod] public void OpJsonDoubleAddAddC(){var j=new Json(1.0,false,true);var k=j++; Assert.AreEqual(1.0, k);}
		[TestMethod] public void OpJsonDoubleAddAddD(){var j=new Json(1.0,false,true);var k=++j; Assert.AreEqual(2.0, k);}

		[TestMethod] public void OpJsonDoubleSubSubA(){var j=new Json(2.0,false,true);j--; Assert.AreEqual(1.0, j.D);}

	}
}
