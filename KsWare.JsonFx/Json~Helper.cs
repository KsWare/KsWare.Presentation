using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if(DYNAMIC_SUPPORT)
using System.Web.Script.Serialization;
#endif

namespace KsWare.JsonFx {

	public partial class Json {

		#region Helper

		private static int ArrayCount(object o) { 
			if(o is ICollection) return ((ICollection) o).Count;
			if(o is ArrayList  ) return ((ArrayList  ) o).Count;
			if(o is object[]   ) return ((object[]   ) o).Length;
			if(o is Array      ) return ((Array      ) o).GetLength(0);
			throw new InvalidOperationException("{2055DDAD-493B-4918-BCFF-F96C0530F010}");
		}

		private static object ArrayGet(object o, int index) { 
			if(o is IList    ) return ((IList    ) o)[index];
			if(o is ArrayList) return ((ArrayList) o)[index];
			if(o is object[] ) return ((object[] ) o)[index];
			if(o is Array    ) return ((Array    ) o).GetValue(index);
			throw new InvalidOperationException("{0F1D1532-9A24-4262-9AB6-5060A6133354}");
		}

		private void ArraySet(object o, int index, object value) {
			     if(o is IList    ) ((IList    ) o)[index]=value;
			else if(o is ArrayList) ((ArrayList) o)[index]=value;
			else if(o is object[] ) ((object[] ) o)[index]=value;
			else if(o is Array    ) ((Array    ) o).SetValue(value, index);
			else throw new InvalidOperationException("{F30EAD67-DAF7-4A17-8B51-8CECEFE3AE7C}");
		}

		private static object DictionaryGet(object o, string key) { 
			if(o is NameValueCollection       ) return ((NameValueCollection      )o)[key];
			if(o is IDictionary<string,object>) return ((Dictionary<string,object>)o)[key];
			if(o is Hashtable                 ) return ((Hashtable                )o)[key];
			if(o is IDictionary               ) return ((IDictionary              )o)[key];
			var mi=o.GetType().GetMember(key, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if(mi.Length==0) throw new KeyNotFoundException();
			switch (mi[0].MemberType) {
				case MemberTypes.Field   : return ((FieldInfo)mi[0]).GetValue(o);
				case MemberTypes.Property: return ((PropertyInfo)mi[0]).GetValue(o,null);
			}
			throw new InvalidOperationException("{FA4C84D6-4493-4EBC-9F07-251C872E47D8}");
		}

		private static void DictionarySet(object o, string key, object value) { 
			     if(o is NameValueCollection       ) ((NameValueCollection      )o)[key]=value;
			else if(o is IDictionary<string,object>) ((Dictionary<string,object>)o)[key]=value;
			else if(o is Hashtable                 ) ((Hashtable                )o)[key]=value;
			else if(o is IDictionary               ) ((IDictionary              )o)[key]=value;
			else {
				var mi=o.GetType().GetMember(key, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if(mi.Length==0) throw new KeyNotFoundException();
				switch (mi[0].MemberType) {
					case MemberTypes.Field   : ((FieldInfo)mi[0]).SetValue(o,value);break;
					case MemberTypes.Property: ((PropertyInfo)mi[0]).SetValue(o,value,null);break;
					default: throw new InvalidOperationException("{77FAFFA9-C692-4123-9B6C-F8FA0F39D04D}");
				}
			}
		}

		private static bool DictionaryContainsKey(object o, string key) { 
			if(o is IDictionary               ) return ((IDictionary               ) o).Contains(key);
			if(o is NameValueCollection       ) return ((NameValueCollection       ) o).ContainsKey(key);
			if(o is IDictionary<string,object>) return ((IDictionary<string,object>) o).ContainsKey(key);
		//	if(o is Hashtable                 ) return ((Hashtable                 ) o).ContainsKey(key);
			return o.GetType().GetMember(key, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public).Length>0;
			throw new InvalidOperationException("{FBC4D401-EE8C-4B12-BDF3-0EA25EF283AA}");
		}

		private static int DictionaryCount(object o) { 
			if(o is IDictionary               ) return ((IDictionary               ) o).Count;
			if(o is NameValueCollection       ) return ((NameValueCollection       ) o).Count;
			if(o is IDictionary<string,object>) return ((IDictionary<string,object>) o).Count;
		//	if(o is Hashtable                 ) return ((Hashtable                 ) o).Count;
			return o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Length+o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Length;
			throw new InvalidOperationException("{FBC4D401-EE8C-4B12-BDF3-0EA25EF283AA}");
		}

		private static bool IsArrayInternal(object obj) {return (obj is object[]) || (obj is ArrayList);}

		private static bool IsDictionaryInternal(object obj) {return obj is Dictionary<string,object>;}

		private static bool IsPrimitiveInternal(object obj) { return !(obj is object[]) && !(obj is ArrayList) && !(obj is Dictionary<string,object>); }

		#endregion

		#region Converter

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private static object[] CopyToArray(Array array) {
			if(array==null) throw new ArgumentNullException("array");
			if(array.Rank>0) throw new ArgumentException("Multidimensional array is not supported!");
			
			var narr = new object[array.Length];
			for (int i = 0; i < array.Length; i++) narr[i] = array.GetValue(i);
			return narr;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private static object[] CopyToArray(ICollection collection) {
			var narr = new object[collection.Count];
			int i = 0;
			foreach (var o in collection) narr[i++] = o;
			return narr;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private static Dictionary<string,object> CopyToDictionary(Dictionary<string,object> dic) {
			var ndic = new Dictionary<string, object>();
			foreach (var kvp in dic) ndic.Add(kvp.Key,kvp.Value);
			return ndic;
		}

		#endregion
	}
}
