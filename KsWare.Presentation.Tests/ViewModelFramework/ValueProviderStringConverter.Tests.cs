using System;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using CollectionAssert = Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert;

namespace KsWare.Presentation.Tests.ViewFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="ValueProviderStringConverter"/>-class
	/// </summary>
	[TestClass]
	public class ValueProviderStringConverterTests {

		[TestInitialize]
		public void Setup() {
			TestSubject = new ValueProviderStringConverter();
		}


		[TestCleanup]
		public void Cleanup() {
			TestSubject = null;

			if(RestoreCurrentCulture!=null) Thread.CurrentThread.CurrentCulture = RestoreCurrentCulture;
			if(RestoreCurrentUICulture!=null) Thread.CurrentThread.CurrentUICulture = RestoreCurrentUICulture;
		}

		public CultureInfo RestoreCurrentUICulture { get; set; }

		public CultureInfo RestoreCurrentCulture { get; set; }

		public ValueProviderStringConverter TestSubject { get; set; }

		private void SetCulture(CultureInfo culture) {
			if(RestoreCurrentCulture  ==null) RestoreCurrentCulture   = Thread.CurrentThread.CurrentCulture;
			if(RestoreCurrentUICulture==null) RestoreCurrentUICulture = Thread.CurrentThread.CurrentUICulture;

			Thread.CurrentThread.CurrentCulture=culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void CanConvertFrom() {
//			Assert.IsTrue(TestSubject.CanConvertFrom(null,typeof(DateTime)));
			Assert.IsTrue(TestSubject.CanConvertFrom(null,typeof(Guid?)));
			Assert.IsTrue(TestSubject.CanConvertFrom(null,typeof(Boolean)));
//			Assert.IsTrue(TestSubject.CanConvertFrom(null,typeof(Boolean?)));
			Assert.IsTrue(TestSubject.CanConvertFrom(null,typeof(DayOfWeek)));//Enum
			Assert.IsTrue(TestSubject.CanConvertFrom(null,typeof(Int32)));

			Assert.IsFalse(TestSubject.CanConvertFrom(null,typeof(Thread)));
		}

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void CanConvertTo() {
//			Assert.IsTrue(TestSubject.CanConvertTo(null,typeof(DateTime)));
			Assert.IsTrue(TestSubject.CanConvertTo(null,typeof(Guid)));
			Assert.IsTrue(TestSubject.CanConvertTo(null,typeof(Guid?)));
			Assert.IsTrue(TestSubject.CanConvertTo(null,typeof(Boolean)));
//			Assert.IsTrue(TestSubject.CanConvertTo(null,typeof(Boolean?)));
			Assert.IsTrue(TestSubject.CanConvertTo(null,typeof(DayOfWeek)));//Enum
			Assert.IsTrue(TestSubject.CanConvertTo(null,typeof(Int32)));

			Assert.IsFalse(TestSubject.CanConvertTo(null,typeof(Thread)));
		}

		[TestMethod]
		public void ConvertFrom() {
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((Byte   ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((SByte  ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((Int16  ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((UInt16 ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((Int32  ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((UInt32 ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((Int64  ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((UInt64 ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((Single ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((Double ) 1));
			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((Decimal) 1));

			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new Byte   [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new SByte  [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new Int16  [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new UInt16 [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new Int32  [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new UInt32 [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new Int64  [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new UInt64 [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new Single [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new Double [] {1,2}));
			Assert.AreEqual("1 2",(string)TestSubject.ConvertFrom(new Decimal[] {1,2}));

			Assert.AreEqual("1",(string)TestSubject.ConvertFrom((string) "1"));

//			Assert.AreEqual("True",TestSubject.ConvertFrom((bool) true));
//			Assert.AreEqual("1",TestSubject.ConvertFrom(new DateTime(2000,1,1,12,0,0)));
		}

		[TestMethod]
		public void ConvertTo() {
			Assert.AreEqual((Byte   ) 1,(Byte   )TestSubject.ConvertTo("1",typeof(Byte   )));
			Assert.AreEqual((SByte  ) 1,(SByte  )TestSubject.ConvertTo("1",typeof(SByte  )));
			Assert.AreEqual((Int16  ) 1,(Int16  )TestSubject.ConvertTo("1",typeof(Int16  )));
			Assert.AreEqual((UInt16 ) 1,(UInt16 )TestSubject.ConvertTo("1",typeof(UInt16 )));
			Assert.AreEqual((Int32  ) 1,(Int32  )TestSubject.ConvertTo("1",typeof(Int32  )));
			Assert.AreEqual((UInt32 ) 1,(UInt32 )TestSubject.ConvertTo("1",typeof(UInt32 )));
			Assert.AreEqual((Int64  ) 1,(Int64  )TestSubject.ConvertTo("1",typeof(Int64  )));
			Assert.AreEqual((UInt64 ) 1,(UInt64 )TestSubject.ConvertTo("1",typeof(UInt64 )));
			Assert.AreEqual((Single ) 1,(Single )TestSubject.ConvertTo("1",typeof(Single )));
			Assert.AreEqual((Double ) 1,(Double )TestSubject.ConvertTo("1",typeof(Double )));
			Assert.AreEqual((Decimal) 1,(Decimal)TestSubject.ConvertTo("1",typeof(Decimal)));

			CollectionAssert.AreEqual(new Byte   [] {1,2},(Byte   [])TestSubject.ConvertTo("1 2",typeof(Byte   [])));
			CollectionAssert.AreEqual(new SByte  [] {1,2},(SByte  [])TestSubject.ConvertTo("1 2",typeof(SByte  [])));
			CollectionAssert.AreEqual(new Int16  [] {1,2},(Int16  [])TestSubject.ConvertTo("1 2",typeof(Int16  [])));
			CollectionAssert.AreEqual(new UInt16 [] {1,2},(UInt16 [])TestSubject.ConvertTo("1 2",typeof(UInt16 [])));
			CollectionAssert.AreEqual(new Int32  [] {1,2},(Int32  [])TestSubject.ConvertTo("1 2",typeof(Int32  [])));
			CollectionAssert.AreEqual(new UInt32 [] {1,2},(UInt32 [])TestSubject.ConvertTo("1 2",typeof(UInt32 [])));
			CollectionAssert.AreEqual(new Int64  [] {1,2},(Int64  [])TestSubject.ConvertTo("1 2",typeof(Int64  [])));
			CollectionAssert.AreEqual(new UInt64 [] {1,2},(UInt64 [])TestSubject.ConvertTo("1 2",typeof(UInt64 [])));
			CollectionAssert.AreEqual(new Single [] {1,2},(Single [])TestSubject.ConvertTo("1 2",typeof(Single [])));
			CollectionAssert.AreEqual(new Double [] {1,2},(Double [])TestSubject.ConvertTo("1 2",typeof(Double [])));
			CollectionAssert.AreEqual(new Decimal[] {1,2},(Decimal[])TestSubject.ConvertTo("1 2",typeof(Decimal[])));

			Assert.AreEqual((string) "1",(Decimal)TestSubject.ConvertTo("1",typeof(string)));
		}

		[TestMethod]
		public void ConvertTo_Double_Invariant() {
			TestSubject.Options.Culture=CultureInfo.InvariantCulture;
			Assert.AreEqual((Double ) 1.1, (Double )TestSubject.ConvertTo("1.1",typeof(Double )));
			Assert.AreEqual((Double ) 1.1, (Double )TestSubject.ConvertTo("1.10",typeof(Double )));
			Assert.AreEqual((Double ) 1.1, (Double )TestSubject.ConvertTo("01.1",typeof(Double )));
			Assert.AreEqual((Double ) 1.1, (Double )TestSubject.ConvertTo("+1.1",typeof(Double )));
			Assert.AreEqual((Double ) (-1.1), (Double )TestSubject.ConvertTo("-1.1",typeof(Double )));
			Assert.AreEqual((Double ) (-1.1), (Double )TestSubject.ConvertTo("-01.1",typeof(Double )));
		}

		[TestMethod]
		public void ConvertFrom_Double_Invariant() {
			TestSubject.Options.Culture = CultureInfo.InvariantCulture;

			Assert.AreEqual("1.1",(string)TestSubject.ConvertFrom((Double ) 1.1));
			Assert.AreEqual("NaN",(string)TestSubject.ConvertFrom((Double ) Double.NaN));
			Assert.AreEqual("Infinity",(string)TestSubject.ConvertFrom((Double ) Double.PositiveInfinity));
			Assert.AreEqual("-Infinity",(string)TestSubject.ConvertFrom((Double ) Double.NegativeInfinity));
		}

		[TestMethod]
		public void ConvertFrom_Double_DE() {
			TestSubject.Options.Culture = CultureInfo.CreateSpecificCulture("de-DE");

			Assert.AreEqual("1,1",(string)TestSubject.ConvertFrom((Double ) 1.1));
			Assert.AreEqual("NaN",(string)TestSubject.ConvertFrom( Double.NaN));
			Assert.AreEqual("+unendlich",(string)TestSubject.ConvertFrom( Double.PositiveInfinity));
			Assert.AreEqual("-unendlich",(string)TestSubject.ConvertFrom( Double.NegativeInfinity));
		}

		[TestMethod]
		public void ConvertFrom_Double_Current_EN() {
			SetCulture(CultureInfo.CreateSpecificCulture("en-US"));

			Assert.AreEqual("1.1",(string)TestSubject.ConvertFrom((Double ) 1.1));
			Assert.AreEqual("NaN",(string)TestSubject.ConvertFrom((Double ) Double.NaN));
			Assert.AreEqual("Infinity",(string)TestSubject.ConvertFrom((Double ) Double.PositiveInfinity));
			Assert.AreEqual("-Infinity",(string)TestSubject.ConvertFrom((Double ) Double.NegativeInfinity));
		}
		
		[TestMethod]
		public void ConvertFrom_Double_Current_DE() {
			SetCulture(CultureInfo.CreateSpecificCulture("de-DE"));

			Assert.AreEqual("1,1",(string)TestSubject.ConvertFrom((Double ) 1.1));
			Assert.AreEqual("NaN",(string)TestSubject.ConvertFrom( Double.NaN));
			Assert.AreEqual("+unendlich",(string)TestSubject.ConvertFrom( Double.PositiveInfinity));
			Assert.AreEqual("-unendlich",(string)TestSubject.ConvertFrom( Double.NegativeInfinity));
		}

	}

	// ReSharper restore InconsistentNaming
}