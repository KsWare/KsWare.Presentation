
using KsWare.Presentation.JsonFx;
using NUnit.Framework;

namespace KsWare.Test.JsonFx.Test {

	[TestFixture]
	public class Test_NameValueCollection {

		NameValueCollection col1=new NameValueCollection{{"A",1},{"B",true},{"S","s"},{"A1",1.1},{"a1",5},{"4",4}};

		[Test] public void Count(){Assert.AreEqual(6,col1.Count);}
		[Test] public void IndexerGet01(){Assert.AreEqual(1,col1["A"]);Assert.AreEqual(true,col1["B"]);Assert.AreEqual(5,col1["a1"]);}
		[Test] public void IndexerGet02(){Assert.AreEqual(1,col1[0]);Assert.AreEqual(true,col1[1]);Assert.AreEqual(5,col1[4]);}
		[Test] public void IndexerGet03(){Assert.AreEqual(1,col1["0"]);}
		[Test] public void IndexerGet04(){Assert.AreEqual(4,col1["4"]);}
		[Test] public void Keys(){Assert.AreEqual(new []{"A","B","S","A1","a1","4"},col1.Keys);}
		[Test] public void Values(){Assert.AreEqual(new object[]{1,true,"s",1.1,5,4},col1.Values);}
	}
}
