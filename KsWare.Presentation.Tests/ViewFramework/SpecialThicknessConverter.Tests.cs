using System;
using System.Windows;
using KsWare.Presentation.ViewFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.ViewFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="SpecialThicknessConverter"/>-class
	/// </summary>
	[TestClass]
	public class SpecialThicknessConverterTests {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() {
			//...do anything here...
		}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() {
			//...do anything here...
		}

		/// <summary> Common
		/// </summary>
		[TestMethod]
		public void Convert_variant_Formats() {
			var conv = new SpecialThicknessConverter();
			var v = 23;
			Assert.AreEqual(new Thickness(0, -23,0,0) ,conv.Convert("23", typeof (Thickness), "0 -{0} 0 0", null));
			Assert.AreEqual(new Thickness(v, 0, 0, 0) ,conv.Convert("23", typeof (Thickness), "Left",       null));
			Assert.AreEqual(new Thickness(0, v, 0, 0) ,conv.Convert("23", typeof (Thickness), "Top",        null));
			Assert.AreEqual(new Thickness(0, 0, v, 0) ,conv.Convert("23", typeof (Thickness), "Right",      null));
			Assert.AreEqual(new Thickness(0, 0, 0, v) ,conv.Convert("23", typeof (Thickness), "Bottom",     null));
			Assert.AreEqual(new Thickness(-v, 0, 0, 0),conv.Convert("23", typeof (Thickness), "-Left",      null));
			Assert.AreEqual(new Thickness(0, -v, 0, 0),conv.Convert("23", typeof (Thickness), "-Top",       null));
			Assert.AreEqual(new Thickness(0, 0, -v, 0),conv.Convert("23", typeof (Thickness), "-Right",     null));
			Assert.AreEqual(new Thickness(0, 0, 0, -v),conv.Convert("23", typeof (Thickness), "-Bottom",    null));
			
		}

		/// <summary> TestDescription
		/// </summary>
		[TestMethod]
		public void Convert_variant_Types() {
			var conv = new SpecialThicknessConverter();
			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((String)"1", typeof (Thickness), "Left", null));
			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((Int32 ) 1 , typeof (Thickness), "Left", null));
			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((Double) 1 , typeof (Thickness), "Left", null));
			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((Single) 1 , typeof (Thickness), "Left", null));
			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((Int64 ) 1 , typeof (Thickness), "Left", null));
			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((Int16 ) 1 , typeof (Thickness), "Left", null));
//			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((Byte  ) 1 , typeof (Thickness), "Left", null));
//			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((SByte ) 1 , typeof (Thickness), "Left", null));
			Assert.AreEqual(new Thickness(1, 0, 0, 0),conv.Convert((Int32 ) 1 , typeof (Thickness), "Left", null));
		}

		/// <summary> TestDescription
		/// </summary>
		[TestMethod][Ignore]//TODO aktivate Test
		public void Convert_unsupported_Types() {
			var conv = new SpecialThicknessConverter();
			Assert.AreEqual(new Thickness(0, 0, 0, 0), conv.Convert((Decimal)1, typeof (Thickness), "Left", null));
		}

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void ConvertBack_NotImplemented() {
			var conv = new SpecialThicknessConverter();
			Assert.Throws<NotImplementedException>(delegate { conv.ConvertBack(new Thickness(1, 2, 3, 4), typeof(double), "Left", null); });
		}
	}

	// ReSharper restore InconsistentNaming
}