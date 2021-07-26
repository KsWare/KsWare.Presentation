using System.Collections;
using System.Diagnostics;
using KsWare.Presentation.JsonFx;
using NUnit.Framework;

namespace KsWare.Test.JsonFx.Test {

	[TestFixture]
	public class Test_Json {

		[Test] public void Constructor3StringParse() {var json=new Json("{\"S\":\"s\",\"L\":2,\"B\":true,\"D\":2.1}",true,true);}
		[Test] public void Constructor3Null() {var json=new Json(null,false,true);}
		[Test] public void Constructor3String() {var json=new Json("s",false,true);}
		[Test] public void Constructor3Long() {var json=new Json(2,false,true);}
		[Test] public void Constructor3Double() {var json=new Json(2.1,false,true);}
		[Test] public void Constructor3Bool() {var json=new Json(true,false,true);}
		[Test] public void Constructor3ArrayList() {var json=new Json(new ArrayList{"s",2,2.1,true},false,true);}
//		[Test] public void Constructor3ObjectArray() {var json=new Json(new object[]{"s",2,2.1,true},false,true);}
		[Test] public void Constructor3NameValueCollection() {var json=new Json(new NameValueCollection{{"S","s"},{"L",2},{"B",true},{"D",2.1}},false,true);}
//		[Test] public void Constructor3DictionaryStringObject() {var json=new Json(new Dictionary<string,object> {{"S","s"},{"L",2},{"B",true},{"D",2.1}},false,true);}
//		[Test] public void Constructor3DictionaryStringString() {var json=new Json(new Dictionary<string,string> {{"A","1"},{"B","2"}},false,true);}

		[Test]
		public void Array1() {
			var json=Json.ToJsonObject("[1,2,\"s\"]");
			Assert.AreEqual(2, json.GetLong("1", 0));
			Assert.AreEqual(2, json.GetLong("[1]", 0));
			Assert.AreEqual("s", json.GetString("2"));
		}

		[Test]
		public void ArrayPath1() {
			var json=Json.ToJsonObject("{\"Array\":[1,2,3,\"d\"]}");
			Assert.AreEqual(2, json.GetLong("Array.1", 0));
		}

		[Test]
		public void ArrayPath2() {
			var json=Json.ToJsonObject("{\"Array\":[1,2,3,\"d\"]}");
			Assert.AreEqual(2, json.GetLong("Array[1]", 0));
		}

		[Test]
		public void ClassDefinition() {
			var s = "{\"s0Kid\":\"135764\",\"s1Kid\":\"91655\",\"s1KLv\":\"1\",\"s0KCombatLv\":\"50\",\"s1KCombatLv\":\"255\",\"fght\":{\"s1\":{\"u7\":[\"200000\",200000,0]}},\"rnds\":2,\"winner\":1,\"bonus\":{\"mod\":{\"s0\":{\"b19\":{\"hp\":[45000000,45000000],\"atk\":[0,0],\"def\":[0,0],\"spd\":[0,0],\"rng\":[0,0]}},\"s1\":{\"u7\":{\"hp\":[1000,1550],\"atk\":[500,2537.5],\"def\":[45,129.375],\"spd\":[1000,1750],\"rng\":[100,100],\"ld\":[100,210]}}},\"tch\":{\"s0\":{\"hp\":0.45,\"atk\":0.45,\"def\":0.45,\"spd\":0.45,\"rng\":0.45},\"s1\":{\"hp\":0.55,\"atk\":0.55,\"def\":0.55,\"spd\":0.55,\"rng\":0.55,\"ld\":1.1}}},\"wall\":100,\"s0atkBoost\":0,\"s0defBoost\":0,\"s0lifeBoost\":0,\"s1atkBoost\":0,\"s1defBoost\":0,\"s1lifeBoost\":0,\"s1guardianAtkBoost\":0.4,\"s0guardianDefBoost\":0.1,\"s1ThroneRoomBoosts\":{\"2\":5,\"1\":80,\"78\":5,\"4\":20,\"93\":10,\"85\":10,\"18\":-122,\"20\":-42,\"21\":-4,\"41\":-8,\"36\":17,\"34\":39,\"90\":12},\"loot\":[111,14419,8800,6747,7504,[],0],\"glory\":0,\"displayGlory\":true,\"errorMsg\":{\"tracker\":true,\"errorCode\":\"default\",\"msg\":\"Something has gone wrong.\"}}";
			var json=new Json(s,true,true);
			Trace.WriteLine(json.ToClassDefinition("GenFetchReportClass",1));
		}

		[Test] public void To_double_double___1(){ Assert.AreEqual((double  )2.0  ,new Json(2.0,false,true).To<double  >(0.0  ,true,false)); }
		[Test] public void To_double_float____1(){ Assert.AreEqual((float   )2.0F ,new Json(2.0,false,true).To<float   >(0.0F ,true,false)); }
		[Test] public void To_double_long_____1(){ Assert.AreEqual((long    )2L   ,new Json(2.0,false,true).To<long    >(0L   ,true,false)); }
		[Test] public void To_double_int______1(){ Assert.AreEqual((int     )2    ,new Json(2.0,false,true).To<int     >(0    ,true,false)); }
		[Test] public void To_double_bool_____1(){ Assert.AreEqual( false         ,new Json(2.0,false,true).To<bool    >(false,true,false)); }
		[Test] public void To_double_decimal__1(){ Assert.AreEqual((decimal )2.0M ,new Json(2.0,false,true).To<decimal >(0M   ,true,false)); }
		[Test] public void To_double_string___1(){ Assert.AreEqual( null          ,new Json(2.0,false,true).To<string  >(null ,true,false)); }
//		[Test] public void To_double_object___1(){ Assert.AreEqual((object  )"2.0",new Json(2.0,false,true).To<object  >(null ,true,false)); }
		[Test] public void To_double_doubleN__1(){ Assert.AreEqual((double? )2.0  ,new Json(2.0,false,true).To<double? >(null ,true,false)); }
		[Test] public void To_double_floatN___1(){ Assert.AreEqual((float?  )2.0F ,new Json(2.0,false,true).To<float?  >(null ,true,false)); }
		[Test] public void To_double_longN____1(){ Assert.AreEqual((long?   )2L   ,new Json(2.0,false,true).To<long?   >(null ,true,false)); }
		[Test] public void To_double_intN_____1(){ Assert.AreEqual((int?    )2    ,new Json(2.0,false,true).To<int?    >(null ,true,false)); }
		[Test] public void To_double_boolN____1(){ Assert.AreEqual( null          ,new Json(2.0,false,true).To<bool?   >(null ,true,false)); }
		[Test] public void To_double_decimalM_1(){ Assert.AreEqual((decimal?)2.0M ,new Json(2.0,false,true).To<decimal?>(null ,true,false)); }

		[Test] public void To_string_double___2(){ Assert.AreEqual((double  )2.0  ,new Json("2.0",false,true).To<double  >(0.0  ,false,true)); }
		[Test] public void To_string_float____2(){ Assert.AreEqual((float   )2.0F ,new Json("2.0",false,true).To<float   >(0.0F ,false,true)); }
		[Test] public void To_string_long_____2(){ Assert.AreEqual((long    )2L   ,new Json("2.0",false,true).To<long    >(0L   ,false,true)); }
		[Test] public void To_string_int______2(){ Assert.AreEqual((int     )2    ,new Json("2.0",false,true).To<int     >(0    ,false,true)); }
		[Test] public void To_string_bool_____2(){ Assert.AreEqual( false         ,new Json("2.0",false,true).To<bool    >(false,false,true)); }
		[Test] public void To_string_decimal__2(){ Assert.AreEqual((decimal )2.0M ,new Json("2.0",false,true).To<decimal >(0M   ,false,true)); }
		[Test] public void To_string_string___2(){ Assert.AreEqual((string  )"2.0",new Json("2.0",false,true).To<string  >(null ,false,true)); }
		[Test] public void To_string_object___2(){ Assert.AreEqual((object  )"2.0",new Json("2.0",false,true).To<object  >(null ,false,true)); }
		[Test] public void To_string_doubleN__2(){ Assert.AreEqual((double? )2.0  ,new Json("2.0",false,true).To<double? >(null ,false,true)); }
		[Test] public void To_string_floatN___2(){ Assert.AreEqual((float?  )2.0F ,new Json("2.0",false,true).To<float?  >(null ,false,true)); }
		[Test] public void To_string_longN____2(){ Assert.AreEqual((long?   )2L   ,new Json("2.0",false,true).To<long?   >(null ,false,true)); }
		[Test] public void To_string_intN_____2(){ Assert.AreEqual((int?    )2    ,new Json("2.0",false,true).To<int?    >(null ,false,true)); }
		[Test] public void To_string_boolN____2(){ Assert.AreEqual( null          ,new Json("2.0",false,true).To<bool?   >(null ,false,true)); }
		[Test] public void To_string_decimalM_2(){ Assert.AreEqual((decimal?)2.0M ,new Json("2.0",false,true).To<decimal?>(null ,false,true)); }
	}
}
