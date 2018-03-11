using System;

#if(DYNAMIC_SUPPORT)
using System.Web.Script.Serialization;
#endif

namespace KsWare.JsonFx {

	public partial class Json {

//		public T Get<T>(JsonPath path) { return (T) this[path].To(default(T), exact: true, parseString: false); }

		public string GetString(JsonPath path) { return this.GetString(path, null); }

		public string GetString(JsonPath path,string defaultValue) {
			if(!Contains(path)) return defaultValue;
			var s=GetValue(path,true);
			if(s==null) return null;
			return s.ToString();
		}

		public decimal? GetNumber(JsonPath path) {
			var s=GetValue(path,true);
			if(s==null) return null;
			var l=System.Decimal.Parse(s.ToString());
			return l;
		}

		public decimal GetNumber(JsonPath path, decimal defaultValue) {
			var s=GetValue(path,true);
			if(s==null) return defaultValue;
			var l=System.Decimal.Parse(s.ToString());
			return l;
		}

		public bool? GetBool(JsonPath path) {
			var s=GetValue(path,true);
			if(s==null) return null;
			var l=bool.Parse(s.ToString());
			return l;
		}

		public bool GetBool(JsonPath path, bool defaultValue) {
			var s=GetValue(path,true);
			if(s==null) return defaultValue;
			var l=bool.Parse(s.ToString());
			return l;
		}

		public object[] GetArray(JsonPath path) {
			var s=GetValue(path,true);
			if(s==null) return new object[0];
			return (object[]) s;
		}

		public long? LongZ(JsonPath path) {
			var s=GetValue(path,true);
			if(s==null) return null;
			var l=long.Parse(s.ToString());
			return l;
		}

		public long GetLong(JsonPath path, long defaultValue) {
			var s=GetValue(path,true);
			if(s==null) return defaultValue;
			var l=long.Parse(s.ToString());
			return l;
		}

		public Int32 GetInt32(JsonPath path, Int32 defaultValue) {
			var s=GetValue(path,true);
			if(s==null) return defaultValue;
			var l=System.Int32.Parse(s.ToString());
			return l;
		}

		public Json GetPath(JsonPath path) {
			var obj=GetValue(path,true);
			return new Json(obj);
		}

		public JsonType GetJsonType(JsonPath path) {
			object obj; string reason;
			if(!TryGetValue(path,out obj,out reason)) return JsonType.None;
			return GetJsonType(obj);
		}

	}
}
