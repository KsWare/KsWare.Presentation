using System;
using KsWare.Presentation.JsonFx;
using KsWare.Test.Presentation.NUnitWpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Test.JsonFx.Test {

	[TestClass]
	public class Test_JsonPath_SplitPath2 {
		
		[TestMethod] public void Test01(){NUnit.Assert.Catch<ArgumentNullException>(() => JsonPath.SplitPath2(null));}
		[TestMethod] public void Test02(){CollectionAssert.AreEqual(new []{""         },JsonPath.SplitPath2(""      ));}
		[TestMethod] public void Test03(){CollectionAssert.AreEqual(new []{"a"        },JsonPath.SplitPath2("a"     ));}
		[TestMethod] public void Test04(){CollectionAssert.AreEqual(new []{"1"        },JsonPath.SplitPath2("1"     ));}
		[TestMethod] public void Test05(){CollectionAssert.AreEqual(new []{"a","1"    },JsonPath.SplitPath2("a.1"   ));}
		[TestMethod] public void Test06(){CollectionAssert.AreEqual(new []{"a","b"    },JsonPath.SplitPath2("a.b"   ));}
		[TestMethod] public void Test07(){CollectionAssert.AreEqual(new []{"a","b.c"  },JsonPath.SplitPath2("a.b.c" ));}
		[TestMethod] public void Test08(){CollectionAssert.AreEqual(new []{"a","b[1]" },JsonPath.SplitPath2("a.b[1]"));}
		[TestMethod] public void Test09(){CollectionAssert.AreEqual(new []{"a","[1].b"},JsonPath.SplitPath2("a[1].b"));}
		[TestMethod] public void Test10(){CollectionAssert.AreEqual(new []{"1","b"    },JsonPath.SplitPath2("[1].b" ));}
		[TestMethod] public void Test11(){CollectionAssert.AreEqual(new []{"1"        },JsonPath.SplitPath2("[1]"   ));}
	}

	[TestClass]
	public class Test_JsonPath_SplitPath2Rev {
		
		[TestMethod] public void Test01(){NUnit.Assert.Catch<ArgumentNullException>(() => JsonPath.SplitPath2Rev(null));}
		[TestMethod] public void Test02(){CollectionAssert.AreEqual(new []{""        },JsonPath.SplitPath2Rev(""      ));}
		[TestMethod] public void Test03(){CollectionAssert.AreEqual(new []{"a"       },JsonPath.SplitPath2Rev("a"     ));}
		[TestMethod] public void Test04(){CollectionAssert.AreEqual(new []{"1"       },JsonPath.SplitPath2Rev("1"     ));}
		[TestMethod] public void Test05(){CollectionAssert.AreEqual(new []{"a","1"   },JsonPath.SplitPath2Rev("a.1"   ));}
		[TestMethod] public void Test06(){CollectionAssert.AreEqual(new []{"a","b"   },JsonPath.SplitPath2Rev("a.b"   ));}
		[TestMethod] public void Test07(){CollectionAssert.AreEqual(new []{"a.b","c" },JsonPath.SplitPath2Rev("a.b.c" ));}
		[TestMethod] public void Test08(){CollectionAssert.AreEqual(new []{"a.b","1" },JsonPath.SplitPath2Rev("a.b[1]"));}
		[TestMethod] public void Test09(){CollectionAssert.AreEqual(new []{"a[1]","b"},JsonPath.SplitPath2Rev("a[1].b"));}
		[TestMethod] public void Test10(){CollectionAssert.AreEqual(new []{"[1]","b" },JsonPath.SplitPath2Rev("[1].b" ));}
		[TestMethod] public void Test11(){CollectionAssert.AreEqual(new []{"1"       },JsonPath.SplitPath2Rev("[1]"   ));}
	}
}
