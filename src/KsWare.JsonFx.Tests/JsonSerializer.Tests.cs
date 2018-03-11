using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.JsonFx.Test {

	[TestClass]
	public class JsonSerializer_Deserialize_Tests {

		private JsonSerializer Json { get; set; }
//		private JavaScriptSerializer Json { get; set; }
		private JsonSerializer.Undefined undefined = JsonSerializer.Undefined.Value;

		[TestInitialize]
		public void SetUp() {
			Trace.WriteLine("SetUp");
			Json=new JsonSerializer();
//			Json=new JavaScriptSerializer();
		}

		[TestCleanup]
		public void TearDown() {
			Trace.WriteLine("TearDown");
			Json = null;
		}

		[TestMethod] public void NumericInteger   () {Assert.AreEqual(5.0                    , Json.DeserializeObject("5"));}
		[TestMethod] public void NumericIntegerExp() {Assert.AreEqual(5e10                   , Json.DeserializeObject("5e10"));}
		[TestMethod] public void NumericDouble    () {Assert.AreEqual(5.1                    , Json.DeserializeObject("5.1"));}
		[TestMethod] public void NumericDoubleExp () {Assert.AreEqual(5.1e10                 , Json.DeserializeObject("5.1e10"));}
		[TestMethod] public void BoolTrue         () {Assert.AreEqual(true                   , Json.DeserializeObject("true"));}
		[TestMethod] public void BoolFalse        () {Assert.AreEqual(false                  , Json.DeserializeObject("false"));}
		[TestMethod] public void String           () {Assert.AreEqual("s"                    , Json.DeserializeObject("\"s\""));}
		[TestMethod] public void Null             () {Assert.AreEqual(null                   , Json.DeserializeObject("null"));}
		[TestMethod] public void NaN              () {Assert.AreEqual(Double.NaN             , Json.DeserializeObject("NaN"));}
		[TestMethod] public void Infinity         () {Assert.AreEqual(Double.PositiveInfinity, Json.DeserializeObject("Infinity"));}
		[TestMethod] public void Undefined        () {Assert.AreEqual(undefined              , Json.DeserializeObject("undefined"));} //??

		[TestMethod] 
		public void Array () {
			var array=Json.Deserialize<JsonValue>("[1,1.1,\"A\",true]");
			Assert.AreEqual(new JsonNumber(1)    ,array[0]);
			Assert.AreEqual(new JsonNumber(1.1)  ,array[1]);
			Assert.AreEqual(new JsonString("A")  ,array[2]);
			Assert.AreEqual(new JsonBool(true)   ,array[3]);
		}

		[TestMethod] 
		public void NameValueCollection() {
			var dic=Json.Deserialize<NameValueCollection>("{\"int\":1,\"float\":1.1,\"string\":\"A\",\"bool\":true}");
			Assert.AreEqual(1.0  ,dic["int"   ]);
			Assert.AreEqual(1.1  ,dic["float" ]);
			Assert.AreEqual("A"  ,dic["string"]);
			Assert.AreEqual(true ,dic["bool"  ]);
		}

		[TestMethod] 
		public void NameValueCollectionIndex() {
			var dic=Json.Deserialize<NameValueCollection>("{\"int\":1,\"float\":1.1,\"string\":\"A\",\"bool\":true}");
			Assert.AreEqual(1.0  ,dic[0]);
			Assert.AreEqual(1.1  ,dic[1]);
			Assert.AreEqual("A"  ,dic[2]);
			Assert.AreEqual(true ,dic[3]);
		}

		[TestMethod] 
		public void ArrayArray() {
			var o=Json.DeserializeObject("[[[1]]]");
			Assert.AreEqual((double)1 ,((object[])((object[])((object[])o)[0])[0])[0]);
		}
		[TestMethod] 
		public void ArrayArray3() {
			var o=Json.Deserialize<JsonValue>("[[[1]]]");
			Assert.AreEqual((JsonNumber)1 ,o[0][0][0]);
		}

		[TestMethod]
		public void ObjectA() {
			var o=Json.Deserialize<ClassA>("{\"l\":2,\"B\":true,\"D\":1.1,\"S\":\"s\"}");
//			Assert.AreEqual(2    ,o.l);
			Assert.AreEqual(true ,o.B);
//			Assert.AreEqual(1.1  ,o.D);
			Assert.AreEqual("s"  ,o.S);
			Assert.AreEqual(0  ,o.Ignore);
		}
	}

	[TestClass]
	public class JsonSerializer_Serialize_Tests {

		private JsonSerializer Json { get; set; }
//		private JavaScriptSerializer Json { get; set; }
		private JsonSerializer.Undefined undefined = JsonSerializer.Undefined.Value;

		[TestInitialize]
		public void SetUp() {
			Trace.WriteLine("SetUp");
			Json = new JsonSerializer();
//			Json=new JavaScriptSerializer();
		}

		[TestCleanup]
		public void TearDown() {
			Trace.WriteLine("TearDown");
			Json = null;
		}

		[TestMethod] public void NumericInteger   () {Assert.AreEqual("5"        , Json.Serialize(5                      ));}
		[TestMethod] public void NumericIntegerExp() {Assert.AreEqual("5E+123"   , Json.Serialize(5e123                  ));}
		[TestMethod] public void NumericDouble    () {Assert.AreEqual("5.1"      , Json.Serialize(5.1                    ));}
		[TestMethod] public void NumericDoubleExp () {Assert.AreEqual("5.1E+123" , Json.Serialize(5.1e123                ));}
		[TestMethod] public void BoolTrue         () {Assert.AreEqual("true"     , Json.Serialize(true                   ));}
		[TestMethod] public void BoolFalse        () {Assert.AreEqual("false"    , Json.Serialize(false                  ));}
		[TestMethod] public void String           () {Assert.AreEqual("\"s\""    , Json.Serialize("s"                    ));}
		[TestMethod] public void Null             () {Assert.AreEqual("null"     , Json.Serialize(null                   ));}
		[TestMethod] public void NaN              () {Assert.AreEqual("NaN"      , Json.Serialize(Double.NaN             ));}
		[TestMethod] public void Infinity         () {Assert.AreEqual("Infinity" , Json.Serialize(Double.PositiveInfinity));}
//		[TestMethod] public void Undefined        () {Assert.AreEqual("undefined", Json.Serialize(undefined              ));} //??

		[TestMethod] public void EnumString       () {Assert.AreEqual("\"0\""    , Json.Serialize(TestStringEnum.D0));}
		[TestMethod] public void EnumNumeric      () {Assert.AreEqual("1"        , Json.Serialize(TestNumericEnum.D1));}

		
		[TestMethod]
		public void ObjectA() {
			var s=Json.Serialize(new ClassA{B=true,S="s",Ignore = 1});
			Assert.AreEqual("{\"B\":true, \"S\":\"s\"}",s);
		}
	}


	// ############################################################################################################


	[JsonType(JsonType.String)]
	internal enum TestStringEnum { A, [JsonName("0")]D0}

	[JsonType(JsonType.Numeric)]
	internal enum TestNumericEnum { D0, D1}

	internal class ClassA {
		private long l;
		public bool B;
		private double D { get; set; }
		public string S { get; set; }

		[JsonIgnore]
		public int Ignore { get; set; }
	}
}
