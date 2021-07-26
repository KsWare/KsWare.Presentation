using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;


namespace KsWare.Presentation.Tests.KsWare {

	[TestFixture]
	public class StringTests {

		[Test] public void TrimLeftTest001() { Assert.AreEqual("0123456789",StringUtil.TrimLeft("0123456789",0));}
		[Test] public void TrimLeftTest002() { Assert.AreEqual("123456789",StringUtil.TrimLeft("0123456789",1));}
		[Test] public void TrimLeftTest003() { Assert.AreEqual("",StringUtil.TrimLeft("0123456789",10));}
		[Test] public void TrimLeftTest004() { Assert.AreEqual("",StringUtil.TrimLeft("0123456789",11));}
		[Test] public void TrimLeftTest005() { Assert.Throws<ArgumentOutOfRangeException>(()=>StringUtil.TrimLeft("0123456789",-1));}
		[Test] public void TrimLeftTest006() { Assert.AreEqual(null,StringUtil.TrimLeft(null,1));}

		[Test] public void TrimRightTest001() { Assert.AreEqual("0123456789",StringUtil.TrimRight("0123456789",0));}
		[Test] public void TrimRightTest002() { Assert.AreEqual("012345678",StringUtil.TrimRight("0123456789",1));}
		[Test] public void TrimRightTest003() { Assert.AreEqual("",StringUtil.TrimRight("0123456789",10));}
		[Test] public void TrimRightTest004() { Assert.AreEqual("",StringUtil.TrimRight("0123456789",11));}
		[Test] public void TrimRightTest005() { Assert.Throws<ArgumentOutOfRangeException>(()=>StringUtil.TrimRight("0123456789",-1));}
		[Test] public void TrimRightTest006() { Assert.AreEqual(null,StringUtil.TrimRight(null,1));}

	}

}