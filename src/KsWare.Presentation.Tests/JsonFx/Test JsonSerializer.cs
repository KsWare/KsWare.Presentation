using System;
using System.Diagnostics;
using KsWare.Presentation.JsonFx;
using NUnit.Framework;

namespace KsWare.Test.JsonFx.Test {

	[TestFixture]
	public class Test_JsonSerializer_Deserialize {

		private JsonSerializer Json { get; set; }
//		private JavaScriptSerializer Json { get; set; }
		private JsonSerializer.Undefined undefined = JsonSerializer.Undefined.Value;

		[SetUp]
		public void SetUp() {
			Trace.WriteLine("SetUp");
			Json=new JsonSerializer();
//			Json=new JavaScriptSerializer();
		}

		[TearDown]
		public void TearDown() {
			Trace.WriteLine("TearDown");
			Json = null;
		}

		[Test] public void NumericInteger   () {Assert.AreEqual(5                      , Json.DeserializeObject("5"));}
		[Test] public void NumericIntegerExp() {Assert.AreEqual(5e10                   , Json.DeserializeObject("5e10"));}
		[Test] public void NumericDouble    () {Assert.AreEqual(5.1                    , Json.DeserializeObject("5.1"));}
		[Test] public void NumericDoubleExp () {Assert.AreEqual(5.1e10                 , Json.DeserializeObject("5.1e10"));}
		[Test] public void BoolTrue         () {Assert.AreEqual(true                   , Json.DeserializeObject("true"));}
		[Test] public void BoolFalse        () {Assert.AreEqual(false                  , Json.DeserializeObject("false"));}
		[Test] public void String           () {Assert.AreEqual("s"                    , Json.DeserializeObject("\"s\""));}
		[Test] public void Null             () {Assert.AreEqual(null                   , Json.DeserializeObject("null"));}
		[Test] public void NaN              () {Assert.AreEqual(Double.NaN             , Json.DeserializeObject("NaN"));}
		[Test] public void Infinity         () {Assert.AreEqual(Double.PositiveInfinity, Json.DeserializeObject("Infinity"));}
		[Test] public void Undefined        () {Assert.AreEqual(undefined              , Json.DeserializeObject("undefined"));} //??

		[Test] 
		public void Array () {
			var array=(object[])Json.DeserializeObject("[1,1.1,\"A\",true]");
			Assert.AreEqual(1 ,array[0]);
			Assert.AreEqual(1.1 ,array[1]);
			Assert.AreEqual("A" ,array[2]);
			Assert.AreEqual(true ,array[3]);
		}

		[Test] 
		public void NameValueCollection() {
			var dic=Json.Deserialize<NameValueCollection>("{\"int\":1,\"float\":1.1,\"string\":\"A\",\"bool\":true}");
			Assert.AreEqual(1 ,dic["int"]);
			Assert.AreEqual(1.1 ,dic["float"]);
			Assert.AreEqual("A" ,dic["string"]);
			Assert.AreEqual(true ,dic["bool"]);
		}
		[Test] 
		public void NameValueCollectionIndex() {
			var dic=Json.Deserialize<NameValueCollection>("{\"int\":1,\"float\":1.1,\"string\":\"A\",\"bool\":true}");
			Assert.AreEqual(1 ,dic[0]);
			Assert.AreEqual(1.1 ,dic[1]);
			Assert.AreEqual("A" ,dic[2]);
			Assert.AreEqual(true ,dic[3]);
		}
		[Test] 
		public void ArrayArray() {
			var o=Json.DeserializeObject("[[[1]]]");
			Assert.AreEqual(1 ,((object[])((object[])((object[])o)[0])[0])[0]);
		}

		[Test]
		public void ObjectA() {
			var o=Json.Deserialize<ClassA>("{\"l\":2,\"B\":true,\"D\":1.1,\"S\":\"s\"}");
//			Assert.AreEqual(2    ,o.l);
			Assert.AreEqual(true ,o.B);
//			Assert.AreEqual(1.1  ,o.D);
			Assert.AreEqual("s"  ,o.S);
			Assert.AreEqual(0  ,o.Ignore);
		}
	}

	[TestFixture]
	public class Test_JsonSerializer_Serialize {

		private JsonSerializer Json { get; set; }
//		private JavaScriptSerializer Json { get; set; }
		private JsonSerializer.Undefined undefined = JsonSerializer.Undefined.Value;

		[SetUp]
		public void SetUp() {
			Trace.WriteLine("SetUp");
			Json = new JsonSerializer();
//			Json=new JavaScriptSerializer();
		}

		[TearDown]
		public void TearDown() {
			Trace.WriteLine("TearDown");
			Json = null;
		}

		[Test] public void NumericInteger   () {Assert.AreEqual("5"        , Json.Serialize(5                      ));}
		[Test] public void NumericIntegerExp() {Assert.AreEqual("5E+123"   , Json.Serialize(5e123                  ));}
		[Test] public void NumericDouble    () {Assert.AreEqual("5.1"      , Json.Serialize(5.1                    ));}
		[Test] public void NumericDoubleExp () {Assert.AreEqual("5.1E+123" , Json.Serialize(5.1e123                ));}
		[Test] public void BoolTrue         () {Assert.AreEqual("true"     , Json.Serialize(true                   ));}
		[Test] public void BoolFalse        () {Assert.AreEqual("false"    , Json.Serialize(false                  ));}
		[Test] public void String           () {Assert.AreEqual("\"s\""    , Json.Serialize("s"                    ));}
		[Test] public void Null             () {Assert.AreEqual("null"     , Json.Serialize(null                   ));}
		[Test] public void NaN              () {Assert.AreEqual("NaN"      , Json.Serialize(Double.NaN             ));}
		[Test] public void Infinity         () {Assert.AreEqual("Infinity" , Json.Serialize(Double.PositiveInfinity));}
//		[Test] public void Undefined        () {Assert.AreEqual("undefined", Json.Serialize(undefined              ));} //??

		[Test] public void EnumString       () {Assert.AreEqual("\"0\""    , Json.Serialize(TestStringEnum.D0));}
		[Test] public void EnumNumeric      () {Assert.AreEqual("1"        , Json.Serialize(TestNumericEnum.D1));}

		
		[Test]
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
