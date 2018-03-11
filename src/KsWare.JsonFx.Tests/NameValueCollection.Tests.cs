using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.JsonFx.Test {

	[TestClass]
	public class NameValueCollectionTests {

		NameValueCollection col1=new NameValueCollection{{"A",1},{"B",true},{"S","s"},{"A1",1.1},{"a1",5},{"4",4}};

		[TestMethod] public void Count(){Assert.AreEqual(6,col1.Count);}
		[TestMethod] public void IndexerGet01(){Assert.AreEqual(1,col1["A"]);Assert.AreEqual(true,col1["B"]);Assert.AreEqual(5,col1["a1"]);}
		[TestMethod] public void IndexerGet02(){Assert.AreEqual(1,col1[0]);Assert.AreEqual(true,col1[1]);Assert.AreEqual(5,col1[4]);}
		[TestMethod] public void IndexerGet03(){Assert.AreEqual(1,col1["0"]);}
		[TestMethod] public void IndexerGet04(){Assert.AreEqual(4,col1["4"]);}
		[TestMethod] public void Keys(){CollectionAssert.AreEqual(new []{"A","B","S","A1","a1","4"},col1.Keys.ToArray());}
		[TestMethod] public void Values(){CollectionAssert.AreEqual(new object[]{1,true,"s",1.1,5,4},col1.Values.ToArray());}
	}
}
