using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace KsWare.JsonFx {

	public static class JsonConverter {

		private static readonly CultureInfo enus = CultureInfo.CreateSpecificCulture("en-US");
		
		public static T Convert<T>(Json json) where T:new() {
			var obj=new T();	
			     if(obj is IJsonImport) ((IJsonImport)obj).CopyFrom(json);
			else if(obj is IDictionary) ConvertDictionary(json,(IDictionary)(object)obj);
			else                        ConvertReflector(json, (object) obj);
			return obj;
		}

		internal static string ConvertToString(object obj) {
			if(obj is Json) obj = ((Json) obj).NativeValue;
			if(obj==null) return null;
			return string.Format(enus,"{0}",obj);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private static object Convert(Json json, Type type) {
			if(type==typeof(string)) {
				if(json.IsNull     ) return null;
				if(json.IsPrimitive) return ConvertToString(json.NativeValue);
				throw new InvalidCastException("Can not convert from "+json.NativeValue.GetType().Name+" to "+type.Name+"!");
			}else if(type.IsEnum ) {
				if(json.IsNull) throw new InvalidCastException("Can not convert from null to "+type.Name+"!");
				return Enum   .Parse(type, ConvertToString(json.NativeValue));
			}else if(type.IsPrimitive) {//Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.
				if(type==typeof(long    )) return long   .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(int     )) return int    .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(bool    )) return bool   .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(float   )) return float  .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(double  )) return double .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(decimal )) return decimal.Parse(ConvertToString(json.NativeValue));		
				if(type==typeof(long?   )) return json.IsNull?null:(object)long   .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(int?    )) return json.IsNull?null:(object)int    .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(bool?   )) return json.IsNull?null:(object)bool   .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(float?  )) return json.IsNull?null:(object)float  .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(double? )) return json.IsNull?null:(object)double .Parse(ConvertToString(json.NativeValue));
				if(type==typeof(decimal?)) return json.IsNull?null:(object)decimal.Parse(ConvertToString(json.NativeValue));	
				throw new InvalidCastException("Can not convert from "+json.NativeValue.GetType().Name+" to "+type.Name+"!");
			} else if(type.IsClass/* || type.IsValueType*/) {
				if(json.IsNull) return null;
				var obj=Activator.CreateInstance(type);	
					 if(obj is IJsonImport) ((IJsonImport)obj).CopyFrom(json);
				else if(obj is IDictionary) ConvertDictionary(json,(IDictionary)(object)obj);
				else                        ConvertReflector (json, (object) obj);
				return obj;
			} else if(type.IsInterface     ) {
				throw new NotImplementedException("{98DCD7EE-5B9E-4530-AB28-3E5366FB2FDC}");
			} else {
				throw new NotImplementedException("{19B9FEE6-AA18-4DF4-80CC-1F89D4879DDF}");
			}
		}

		private static void ConvertReflector(Json json, object obj) { 
			var props=obj.GetType().GetProperties();
			foreach (var prop in props) {
				var name = prop.Name;
				if(json.Contains(name)) {
					var v0 = json[name];
					var v1 = Convert(v0, prop.PropertyType);
					prop.SetValue(obj,v1,null);
				}
			}
		}

		private static void ConvertDictionary(Json json, IDictionary dic) { 
			foreach (var kv in (Dictionary<string,object>)json.NativeValue) {
				dic.Add(kv.Key,kv.Value);
			}
		}


	}

}
