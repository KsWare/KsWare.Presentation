using System;
using KsWare.Presentation.JsonFx;
using KsWare.Test.Presentation.NUnitWpf;
using NUnit.Framework;

namespace KsWare.Test.JsonFx.Test {

	[TestFixture]
	public class Test_JsonPath_SplitPath2 {
		
		[Test] public void Test01(){NUnit.Assert.Catch<ArgumentNullException>(() => JsonPath.SplitPath2(null));}
		[Test] public void Test02(){CollectionAssert.AreEqual(new []{""         },JsonPath.SplitPath2(""      ));}
		[Test] public void Test03(){CollectionAssert.AreEqual(new []{"a"        },JsonPath.SplitPath2("a"     ));}
		[Test] public void Test04(){CollectionAssert.AreEqual(new []{"1"        },JsonPath.SplitPath2("1"     ));}
		[Test] public void Test05(){CollectionAssert.AreEqual(new []{"a","1"    },JsonPath.SplitPath2("a.1"   ));}
		[Test] public void Test06(){CollectionAssert.AreEqual(new []{"a","b"    },JsonPath.SplitPath2("a.b"   ));}
		[Test] public void Test07(){CollectionAssert.AreEqual(new []{"a","b.c"  },JsonPath.SplitPath2("a.b.c" ));}
		[Test] public void Test08(){CollectionAssert.AreEqual(new []{"a","b[1]" },JsonPath.SplitPath2("a.b[1]"));}
		[Test] public void Test09(){CollectionAssert.AreEqual(new []{"a","[1].b"},JsonPath.SplitPath2("a[1].b"));}
		[Test] public void Test10(){CollectionAssert.AreEqual(new []{"1","b"    },JsonPath.SplitPath2("[1].b" ));}
		[Test] public void Test11(){CollectionAssert.AreEqual(new []{"1"        },JsonPath.SplitPath2("[1]"   ));}
	}

	[TestFixture]
	public class Test_JsonPath_SplitPath2Rev {
		
		[Test] public void Test01(){NUnit.Assert.Catch<ArgumentNullException>(() => JsonPath.SplitPath2Rev(null));}
		[Test] public void Test02(){CollectionAssert.AreEqual(new []{""        },JsonPath.SplitPath2Rev(""      ));}
		[Test] public void Test03(){CollectionAssert.AreEqual(new []{"a"       },JsonPath.SplitPath2Rev("a"     ));}
		[Test] public void Test04(){CollectionAssert.AreEqual(new []{"1"       },JsonPath.SplitPath2Rev("1"     ));}
		[Test] public void Test05(){CollectionAssert.AreEqual(new []{"a","1"   },JsonPath.SplitPath2Rev("a.1"   ));}
		[Test] public void Test06(){CollectionAssert.AreEqual(new []{"a","b"   },JsonPath.SplitPath2Rev("a.b"   ));}
		[Test] public void Test07(){CollectionAssert.AreEqual(new []{"a.b","c" },JsonPath.SplitPath2Rev("a.b.c" ));}
		[Test] public void Test08(){CollectionAssert.AreEqual(new []{"a.b","1" },JsonPath.SplitPath2Rev("a.b[1]"));}
		[Test] public void Test09(){CollectionAssert.AreEqual(new []{"a[1]","b"},JsonPath.SplitPath2Rev("a[1].b"));}
		[Test] public void Test10(){CollectionAssert.AreEqual(new []{"[1]","b" },JsonPath.SplitPath2Rev("[1].b" ));}
		[Test] public void Test11(){CollectionAssert.AreEqual(new []{"1"       },JsonPath.SplitPath2Rev("[1]"   ));}
	}
}
