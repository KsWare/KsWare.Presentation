using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

#if(DYNAMIC_SUPPORT)
using System.Web.Script.Serialization;
#endif
namespace KsWare.JsonFx {

	public partial class Json:IEnumerable {

		private static readonly CultureInfo enus = CultureInfo.CreateSpecificCulture("en-US");

		private string _json;
		private JsonType _jsObjType;
		private object _jsObj;
		private bool _isReadOnly;

		#region Contructors

		public Json(JsonType type) {
			_json = null;
			switch (type) {
				case JsonType.None      : _jsObj=null ; break; //TODO REVISE
				case JsonType.Null      : _jsObj=null ; break;
				case JsonType.Boolean   : _jsObj=false; break;
				case JsonType.Numeric   : _jsObj=0.0  ; break;
				case JsonType.String    : _jsObj=null ; break;
				case JsonType.Array     : _jsObj=new ArrayList();break;
				case JsonType.Object    : _jsObj=new NameValueCollection();break;
			}
			_jsObjType=type;
			_isReadOnly = false;
		}

		public Json(object obj):this(obj,false,true) {}

		public Json(object obj, bool parse, bool isReadonly) {
			BaseUpdate(obj, parse, isReadonly);
		}
		#endregion

		protected void BaseUpdate(object obj, bool parse, bool isReadonly) { 
			_isReadOnly = isReadonly;

			if(parse) {
				if (obj is string) {
					_json = (string) obj;
					obj = sJavaScriptSerializer.DeserializeObject(_json);
					parse = false;
				} else {
					throw new NotImplementedException("{D3AD5D00-9D94-41F5-BB19-2F4E0D8B6054}");
				}
			} else {
				_json = null;
			}

			_jsObjType = GetJsonType(obj);
			if(obj is Json) {_json = ((Json) obj)._json;_jsObj = ((Json) obj)._jsObj;} 
			else {_jsObj = obj;}		
		}

		public bool Contains(JsonPath path) {
			object obj;
			string reason;
			return TryGetValue(path, out obj, out reason);
		}

		public Json this[int index] {
			get { return this[index.ToString(enus)]; }
			set { this[index.ToString(enus)] = value; }
		}

		public Json this[int index, object defaultValue] {
			get { return this[index.ToString(enus),defaultValue]; }
		}

		public Json this[JsonPath path]{
			get { return new Json(GetValue(path,true)); }
			set {
				if(path==null) throw new ArgumentNullException("path");
				if(_isReadOnly) throw new InvalidOperationException("Object is read-only!");

				var pt=JsonPath.SplitPath2Rev(path);
				if(pt[0]=="") {
					_jsObj     = value._jsObj;
					_jsObjType = value._jsObjType;
					_json      = value._json;
					return;
				}

				object obj;string p1;
				if(pt.Length==2) {
					obj = GetValue(pt[0], false);
					p1 = pt[1];
				} else {
					obj = _jsObj;
					p1 = pt[0];
				}
				
				switch (GetJsonType(obj)) {
					case JsonType.Array: {
						var i = int.Parse(p1);
						ArraySet(obj, i, value._jsObj);
						return;
					}
					case JsonType.Object: {
						DictionarySet(obj, p1, value._jsObj);
						return;
					}
					default: throw new ArgumentException("Invalid path!","path");
				}
			}
		}

		public Json this[JsonPath path, object defaultValue] {
			get {
				object o;
				string reason;
				if(!TryGetValue(path, out o,out reason)) return new Json(defaultValue,false,true);
				return new Json(o,false,true);
			}
		}



		public bool ReParseString(JsonPath path) {
			object obj; string reason;
			if(!TryGetValue(path,out obj,out reason)) return false;
			if(!(obj is string)) return false;
			var newObj = new Json(obj, true, false);

			var ro = _isReadOnly;
			_isReadOnly = false;
			this[path] = newObj;
			_isReadOnly = ro;
			return true;
		}

		public bool IsNull {get {return _jsObj==null;}}
		public bool IsArray {get {return IsArrayInternal(_jsObj);}}
		public bool IsDictionary {get { return IsDictionaryInternal(_jsObj); }}
		public bool IsPrimitive {get { return IsPrimitiveInternal(_jsObj); }}

		public string[] Keys {
			get {
				int count = -1;
				if(_jsObj is IList) {
					count=((IList) _jsObj).Count;
					var keys = new string[count];
					for (int i = 0; i < count; i++) keys[i] = i.ToString();
					return keys;
				} else if(_jsObj is NameValueCollection) {
					var keys=new string[((NameValueCollection) _jsObj).Count];
					((NameValueCollection) _jsObj).Keys.CopyTo(keys,0);
					return keys;
				} else if(_jsObj is IDictionary) {
					var keys=new string[((IDictionary) _jsObj).Count];
					((IDictionary) _jsObj).Keys.CopyTo(keys,0);
					return keys;
				} else {
					return new []{""};
				}
			}
		}

		public object[] Values { // implement this for all possible types of m_obj
			get {
				if(_jsObj is IDictionary) {
					var dic = (IDictionary) _jsObj;
					var count = dic.Count;
					var values = new object[count];
					dic.Values.CopyTo(values,0);
					return values;
				} else if(_jsObj is IList) {
					var list = (IList) _jsObj;
					var count = list.Count;
					var values = new object[count];
					list.CopyTo(values,0);
					return values;
				} else  { //TODO revise
					if(_jsObj==null) return new object[] {null};
					return new object[]{_jsObj};
				}
			}
		}

		public Dictionary<string,object> ToDictionary() {
			var dic = new Dictionary<string, object>();
			var keys = Keys;
			var vals = Values;
			for (int i = 0; i < keys.Length; i++) {
				dic.Add(keys[i],vals[i]);
			}
			return dic;
		} 

		public object NativeValue{get { return _jsObj; }}

		public object GetValue(JsonPath path, bool noException) {
			object obj;
			string reason;
			if(TryGetValue(path,out obj,out reason)) return obj;
			if(noException) return null;
			throw new KeyNotFoundException(reason);
		}
		public bool TryGetValue(JsonPath path, out object obj) {
			string reason;
			return TryGetValue(path, out obj, out reason);
		}

		private bool TryGetValue(JsonPath path, out object obj, out string reason) {
			var pathLeft = path;
			var pathCur = "";
			var o = _jsObj;
//			var oDic = o as NameValueCollection;
//			var oArr = o as ArrayList;
			if(path!="") {
				while (true) {
					var p = JsonPath.SplitPath2(pathLeft);
					var propName = p[0]; 
					pathLeft = p.Length == 2 ? p[1] : null;
					pathCur += (pathCur.Length>0?".":"")+propName;
					if(o is string) o=sJavaScriptSerializer.DeserializeObject((string) o);
					switch (GetJsonType(o)) {
						case JsonType.Null: case JsonType.None:{
							reason = "Path not found! Path=\""+pathCur+"\"";
							obj=null;
							return false;
						} 
						case JsonType.Object: {
							int i;
							if(DictionaryContainsKey(o, propName)) {
								o = DictionaryGet(o,propName);
							} else if(int.TryParse(propName, out i)) {
								if(o is NameValueCollection) {
									o = ((NameValueCollection) o)[i];
								} else {
									obj = null;
									reason = "Indexing of dictionary not supported! Path=\""+pathCur+"\"";
									Trace.WriteLine("WARNING "+reason +"..'"+pathLeft+"'");
									return false;									
								}
							} else {
								obj = null;
								reason = "Path not found! Path=\""+pathCur+"\"";
								if(pathLeft.StringValue!=null) Trace.WriteLine("WARNING JsonObject object null reference. path='"+pathCur+"'..'"+pathLeft+"'");
								return false;
							}
							break;
						}
						case JsonType.Array: {
							int index;
							if(propName=="length") {
								o = ArrayCount(o);
							} else if(int.TryParse(propName, out index)) {
								o = ArrayGet(o,index);							
							} else {
								var to=o.GetType().GetProperty(propName);
								if(to!=null){
									o=to.GetValue(o,null);
								} else {
									// throw new InvalidCastException("Object is not an array! Path=\""+pathCur+"\"");
									obj = null;
									reason="Object is not an array! Path=\""+pathCur+"\"";
									Trace.WriteLine("WARNING JsonObject Object is not an array! "+pathCur);
									return false;
								}
							}
							break;
						}
						default: {
							reason = "Path not found! Type=\""+o.GetType().Name+"\" Path=\""+pathCur+"\"";
							obj=null;
							return false;
						}
					}

					if(pathLeft==null) break;
				}				
			}

			obj=o;
			reason = null;
			return true;
		}

		private JsonType GetJsonType(object obj) { 
			if(obj ==null) return JsonType.Null;
			if(obj is Json) return GetJsonType(((Json) obj)._jsObj);
			if((obj is string)) return JsonType.String;
			if((obj is bool)) return JsonType.Boolean;
			if((obj is long) || (obj is int) || (obj is double) || (obj is float) || (obj is Decimal)) return JsonType.Numeric;
			if((obj is NameValueCollection) || (obj is Dictionary<string,object>) || (obj is Hashtable) || (obj is IDictionary)) return JsonType.Object;
			if((obj is ArrayList) || (obj is object[]) || (obj is IList) || (obj.GetType().IsArray)) return JsonType.Array;
			return JsonType.Object;
		}

		public IEnumerator GetEnumerator() { 
			if(_jsObj is IDictionary) return ((IDictionary) _jsObj).GetEnumerator();
			if(_jsObj is Dictionary<string,object>) return ((Dictionary<string,object>) _jsObj).GetEnumerator();
			if(_jsObj is IList) return ((IList) _jsObj).GetEnumerator();
			if(_jsObj is object[]) return ((object[]) _jsObj).GetEnumerator();
			if(_jsObj is ArrayList) return ((ArrayList) _jsObj).GetEnumerator();
			return new object[0].GetEnumerator();
		}

		public override string ToString() {
			if(_json!=null) return _json;
			//return "{Json not serialized}";
			return sJavaScriptSerializer.Serialize(_jsObj);
		}

		public int Count {
			get {
				if(_jsObj is string) ReParseString("");
				if(IsArray) return ArrayCount(_jsObj);
				if(IsDictionary) return DictionaryCount(_jsObj);
				else return 0;
			}
		}

		#region Generator

		#endregion




		public string   S {get { return To<string  >(null ); }}
		public long     L {get { return To<long    >(0L   ); }}
		public long?    LZ{get { return To<long?   >(null ); }}
		public ulong    UL{get { return To<ulong   >(0UL  ); }}
		public int      I {get { return To<int     >(0    ); }}
		public int?     IZ{get { return To<int?    >(null ); }}
		public uint     U {get { return To<uint    >(0U   ); }}
		public double   D {get { return To<double  >(0.0D ); }}
		public double?  DZ{get { return To<double? >(null ); }}
		public float    F {get { return To<float   >(0.0F ); }}
		public float?   FZ{get { return To<float?  >(null ); }}
		public bool     B {get { return To<bool    >(false); }}
		public bool?    BZ{get { return To<bool?   >(null ); }}
		public decimal  M {get { return To<decimal >(0.0M ); }}
		public decimal? MZ{get { return To<decimal?>(null ); }}
		public object   O {get { return _jsObj; }}

		public T To<T>(T defaultValue) { return To(defaultValue, false, true); }

		public T To<T>(T defaultValue, bool exact, bool parseString) {
			var conversionType = typeof (T);
			var jsonType = GetJsonType(_jsObj);

			if(false) {
			} else if(conversionType==typeof(double) || conversionType==typeof(float) || conversionType==typeof(decimal)) {
				if(_jsObj==null) return defaultValue;
				if(exact && jsonType!=JsonType.Numeric) return defaultValue;
				try {object result=Convert.ChangeType(_jsObj, conversionType, enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(double?) || conversionType==typeof(float?) || conversionType==typeof(decimal?)) {
				if(_jsObj==null) return (T)(object)null;
				conversionType = Nullable.GetUnderlyingType(conversionType);
				if(exact && (jsonType!=JsonType.Numeric && jsonType!=JsonType.Null)) return defaultValue;
				try {object result=Convert.ChangeType(_jsObj, conversionType, enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(long)) {
				if(_jsObj==null) return defaultValue;
				if(exact && jsonType!=JsonType.Numeric) return defaultValue;
				if(_jsObj is string && parseString){
					long result;
					if(!long.TryParse((string)_jsObj,NumberStyles.Any,enus, out result)) return defaultValue;
					return (T)(object)result;
				}
				try {object result=Convert.ChangeType(_jsObj, conversionType, enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(long?)) {
				if(_jsObj==null) return defaultValue;
				if(exact && (jsonType!=JsonType.Numeric && jsonType!=JsonType.Null)) return defaultValue;
				if(_jsObj is string && parseString){
					long result;
					if(!long.TryParse((string)_jsObj,NumberStyles.Any,enus, out result)) return defaultValue;
					return (T)(object)result;
				}
				try {object result=Convert.ChangeType(_jsObj, typeof(long), enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(int)) {
				if(_jsObj==null) return defaultValue;
				if(exact && jsonType!=JsonType.Numeric) return defaultValue;
				if(_jsObj is string && parseString){
					int result;
					if(!int.TryParse((string)_jsObj,NumberStyles.Any,enus, out result)) return defaultValue;
					return (T)(object)result;
				}
				try {object result=Convert.ChangeType(_jsObj, conversionType, enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(int?)) {
				if(_jsObj==null) return defaultValue;
				if(exact && (jsonType!=JsonType.Numeric && jsonType!=JsonType.Null)) return defaultValue;
				if(_jsObj is string && parseString){
					int result;
					if(!int.TryParse((string)_jsObj,NumberStyles.Any,enus, out result)) return defaultValue;
					return (T)(object)result;
				}
				try {object result=Convert.ChangeType(_jsObj, typeof(int), enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(bool)) {
				if(_jsObj==null) return defaultValue;
				if(exact && jsonType!=JsonType.Boolean) return defaultValue;
				try {object result=Convert.ChangeType(_jsObj, conversionType, enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(bool?)) {
				if(_jsObj==null) return defaultValue;
				if(exact && (jsonType!=JsonType.Boolean && jsonType!=JsonType.Null)) return defaultValue;
				try {object result=Convert.ChangeType(_jsObj, typeof(bool?), enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(string)) {
				if(_jsObj==null) return defaultValue;
				if(exact && jsonType!=JsonType.String) return defaultValue;
				try {object result=Convert.ChangeType(_jsObj, conversionType, enus); return (T)result;	} catch (Exception ex) {this.DoNothing(ex); return defaultValue;}
			} else if(conversionType==typeof(object)) {
				object result=_jsObj;
				return (T)result;
			} else if(conversionType.IsPrimitive) {
				try {
					object result=Convert.ChangeType(_jsObj, conversionType, enus);
					return (T)result;	
				} catch (Exception ex) {
					this.DoNothing(ex); 
					return defaultValue;
				}			
			} else if(!conversionType.IsInterface) {
				var s = ToString();
				object result=ToObject<T>(s); //TODO revise
				return (T)result;
			} else {
				try {
					object result=Convert.ChangeType(_jsObj, conversionType, enus);
					return (T)result;	
				} catch (Exception ex) {
					this.DoNothing(ex); 
					return defaultValue;
				}
			}
			
		}
	}

}
