using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.KsWare {

	[TestClass]
	public class StringTests {

		[TestMethod] public void TrimLeftTest001() { Assert.AreEqual("0123456789",StringUtil.TrimLeft("0123456789",0));}
		[TestMethod] public void TrimLeftTest002() { Assert.AreEqual("123456789",StringUtil.TrimLeft("0123456789",1));}
		[TestMethod] public void TrimLeftTest003() { Assert.AreEqual("",StringUtil.TrimLeft("0123456789",10));}
		[TestMethod] public void TrimLeftTest004() { Assert.AreEqual("",StringUtil.TrimLeft("0123456789",11));}
		[TestMethod] public void TrimLeftTest005() { Assert.Throws<ArgumentOutOfRangeException>(()=>StringUtil.TrimLeft("0123456789",-1));}
		[TestMethod] public void TrimLeftTest006() { Assert.AreEqual(null,StringUtil.TrimLeft(null,1));}

		[TestMethod] public void TrimRightTest001() { Assert.AreEqual("0123456789",StringUtil.TrimRight("0123456789",0));}
		[TestMethod] public void TrimRightTest002() { Assert.AreEqual("012345678",StringUtil.TrimRight("0123456789",1));}
		[TestMethod] public void TrimRightTest003() { Assert.AreEqual("",StringUtil.TrimRight("0123456789",10));}
		[TestMethod] public void TrimRightTest004() { Assert.AreEqual("",StringUtil.TrimRight("0123456789",11));}
		[TestMethod] public void TrimRightTest005() { Assert.Throws<ArgumentOutOfRangeException>(()=>StringUtil.TrimRight("0123456789",-1));}
		[TestMethod] public void TrimRightTest006() { Assert.AreEqual(null,StringUtil.TrimRight(null,1));}

	}

}