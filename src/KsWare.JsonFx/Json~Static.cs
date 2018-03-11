using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

#if(DYNAMIC_SUPPORT)
using System.Web.Script.Serialization;
#endif

namespace KsWare.JsonFx {

	public partial class Json {
#if(DYNAMIC_SUPPORT)
		private static readonly JavaScriptSerializer sDynamicJsonSerializer;
#endif
//		private static readonly JavaScriptSerializer sJavaScriptSerializer;
		private static readonly JsonSerializer       sJavaScriptSerializer;

		static Json() {
			#if(DYNAMIC_SUPPORT)
			sDynamicJsonSerializer = new JavaScriptSerializer();
			sDynamicJsonSerializer.RegisterConverters(new[] {new DynamicJsonConverter()});
			#endif
//			sJavaScriptSerializer=new JavaScriptSerializer();
			sJavaScriptSerializer=new JsonSerializer();
		}
		
		public static string ToString(object obj) {
			var json=sJavaScriptSerializer.Serialize(obj);
			return json;
		}

		#if(DYNAMIC_SUPPORT)
		public static dynamic ToDynamic(string json) {
			dynamic obj = sDynamicJsonSerializer.Deserialize(json, typeof(object));	
			return obj;
		}
		#endif

		public static Json ToJsonObject(string json) { 
			return new Json(json,true,true);
		}

		public static T ToObject<T>(string json) {
			var obj=sJavaScriptSerializer.Deserialize<T>(json);
			return obj;
		}

		/*
		$pcre_regex = '
		  /
		  (?(DEFINE)
			 (?<number>   -? (?= [1-9]|0(?!\d) ) \d+ (\.\d+)? ([eE] [+-]? \d+)? )    
			 (?<boolean>   true | false | null )
			 (?<string>    " ([^"\\\\]* | \\\\ ["\\\\bfnrt\/] | \\\\ u [0-9a-f]{4} )* " )
			 (?<array>     \[  (?:  (?&json)  (?: , (?&json)  )*  )?  \s* \] )
			 (?<pair>      \s* (?&string) \s* : (?&json)  )
			 (?<object>    \{  (?:  (?&pair)  (?: , (?&pair)  )*  )?  \s* \} )
			 (?<json>   \s* (?: (?&number) | (?&boolean) | (?&string) | (?&array) | (?&object) ) \s* )
		  )
		  \A (?&json) \Z
		  /six   
		';
		*/
		public static bool IsValidNumber(string json) {
			return Regex.IsMatch(@"^-? (?= [1-9]|0(?!\d) ) \d+ (\.\d+)? ([eE] [+-]? \d+)?$",json,RegexOptions.Compiled|RegexOptions.IgnorePatternWhitespace);
		}
		public static bool IsValidBool(string json) {
			return Regex.IsMatch(@"^ true | false | null $",json,RegexOptions.Compiled|RegexOptions.IgnorePatternWhitespace);
		}
		public static bool IsValidString(string json) {//TODO validate
			return Regex.IsMatch(@"^ "" ([^""\\\\]* | \\\\ [""\\\\bfnrt\/] | \\\\ u [0-9a-f]{4} )* "" $",json,RegexOptions.Compiled|RegexOptions.IgnorePatternWhitespace);
		}

		//public static explicit operator object(Json json) {}
		public static implicit operator Json(string   primitive) {return new Json(primitive,false,true);}
		public static implicit operator Json(long     primitive) {return new Json(primitive,false,true);}
		public static implicit operator Json(int      primitive) {return new Json(primitive,false,true);}
		public static implicit operator Json(bool     primitive) {return new Json(primitive,false,true);}
		public static implicit operator Json(float    primitive) {return new Json(primitive,false,true);}
		public static implicit operator Json(double   primitive) {return new Json(primitive,false,true);}
		public static implicit operator Json(decimal  primitive) {return new Json(primitive,false,true);}
		public static implicit operator Json(object[] array    ) {return new Json(array    ,false,true);}
		public static implicit operator Json(Dictionary<string,object> dictionary) {return new Json(dictionary,false,true);}

		public static implicit operator string  (Json json) {return json.S;}
		public static implicit operator long    (Json json) {return json.L;}
		public static implicit operator int     (Json json) {return json.I;}
		public static implicit operator bool    (Json json) {return json.B;}
		public static implicit operator float   (Json json) {return json.F;}
		public static implicit operator double  (Json json) {return json.D;}
		public static implicit operator decimal (Json json) {return json.M;}
		public static implicit operator object[](Json json) {return json.To<object[]>(new object[0]);}
		public static implicit operator Dictionary<string,object>(Json json) {return json.To<Dictionary<string,object>>(new  Dictionary<string,object>());}


//		public static Json operator == (Json a, Json b) {...}
//		public static Json operator != (Json a, Json b) {...}

//		public static Json operator + (Json a, Json b) {
//			if(a._jsObjType==JsonType.Numeric) {
//				var a0=Convert.ToDouble(a._jsObj);
//				var b0=Convert.ToDouble(b._jsObj);
//				return new Json(a0+b0,false,a._isReadOnly);
//			}else if(a._jsObjType==JsonType.String) {
//				var a0=Convert.ToString(a._jsObj);
//				var b0=Convert.ToString(b._jsObj);
//				return new Json(a0+b0,false,a._isReadOnly);
//			} else {
//				var ex=new InvalidOperationException();
//				ex.Data.Add("ErrorID","{85A3E02C-402A-4E77-87CF-09213ECF6576}");
//				throw ex;
//			}
//		}
		public static Json operator + (Json a, Json b) { return new Json(OpPlus(a._jsObj, b._jsObj),false,a._isReadOnly); }
		private static object OpPlus(object a, object b) { 
			if(a is Int32) {
				var bc=(Int32)Convert.ChangeType(b,typeof(Int32),CultureInfo.InvariantCulture);
				return (Int32) a + bc;
			} else if(a is Int64) {
				var bc=(Int64)Convert.ChangeType(b,typeof(Int64),CultureInfo.InvariantCulture);
				return (Int64) a + bc;
			} else if(a is Double) {
				var bc=(Double)Convert.ChangeType(b,typeof(Double),CultureInfo.InvariantCulture);
				return (Double) a + bc;
			} else if(a is Single) {
				var bc=(Single)Convert.ChangeType(b,typeof(Single),CultureInfo.InvariantCulture);
				return (Single) a + bc;
			} else if(a is Decimal) {
				var bc=(Decimal)Convert.ChangeType(b,typeof(Decimal),CultureInfo.InvariantCulture);
				return (Decimal) a + bc;
			} else if(a is String) {
				var bc=(String)Convert.ChangeType(b,typeof(String),CultureInfo.InvariantCulture);
				return (String) a + bc;
			} else {
				var ex=new InvalidOperationException();
				ex.Data.Add("ErrorID","{66B03E9E-D6E8-4DC2-AFBF-B77D953EEF36}");
				throw ex;
			}
		}

		public static Json operator - (Json a, Json b) {
			if(a._jsObjType==JsonType.Numeric) {
				var a0=(Double)Convert.ChangeType(a._jsObj,typeof(Double),CultureInfo.InvariantCulture);
				var b0=(Double)Convert.ChangeType(b._jsObj,typeof(Double),CultureInfo.InvariantCulture);
				return new Json(a0-b0,false,a._isReadOnly);
			} else {
				var ex=new InvalidOperationException();
				ex.Data.Add("ErrorID","{2CC27FE8-581E-455C-A51E-999AC8E78C00}");
				throw ex;
			}
		}
		public static Json operator * (Json a, Json b) {
			if(a._jsObjType==JsonType.Numeric) {
				var a0=(Double)Convert.ChangeType(a._jsObj,typeof(Double),CultureInfo.InvariantCulture);
				var b0=(Double)Convert.ChangeType(b._jsObj,typeof(Double),CultureInfo.InvariantCulture);
				return new Json(a0*b0,false,a._isReadOnly);
			} else {
				var ex=new InvalidOperationException();
				ex.Data.Add("ErrorID","{30B9A239-E15B-4367-8C20-F56D89C6AC8C}");
				throw ex;
			}
		}
		public static Json operator / (Json a, Json b) {
			if(a._jsObjType==JsonType.Numeric) {
				var a0=(Double)Convert.ChangeType(a._jsObj,typeof(Double),CultureInfo.InvariantCulture);
				var b0=(Double)Convert.ChangeType(b._jsObj,typeof(Double),CultureInfo.InvariantCulture);
				return new Json(a0/b0,false,a._isReadOnly);
			} else {
				var ex=new InvalidOperationException();
				ex.Data.Add("ErrorID","{5E0232CD-59E2-47B0-9F84-E248B1B1A98B}");
				throw ex;
			}
		}

		public static Json operator ++ (Json a) {
			if(a._jsObjType==JsonType.Numeric) {
				var a0=Convert.ToDouble(a._jsObj);
				a0 += 1.0;
				var a1 = Convert.ChangeType(a0, a._jsObj.GetType(), CultureInfo.InvariantCulture);
				a.BaseUpdate(a1,false,a._isReadOnly);
				return a;
			} else {
				var ex=new InvalidOperationException();
				ex.Data.Add("ErrorID","{DB699BB0-60DB-467B-ADF1-C139B64A38A2}");
				throw ex;
			}
		}
		public static Json operator -- (Json a) {
			if(a._jsObjType==JsonType.Numeric) {
				var a0=Convert.ToDouble(a._jsObj);
				a0 -= 1.0;
				var a1 = Convert.ChangeType(a0, a._jsObj.GetType(), CultureInfo.InvariantCulture);
				a.BaseUpdate(a1,false,a._isReadOnly);
				return a;
			} else {
				var ex=new InvalidOperationException();
				ex.Data.Add("ErrorID","{DB699BB0-60DB-467B-ADF1-C139B64A38A2}");
				throw ex;
			}
		}
	}
}
