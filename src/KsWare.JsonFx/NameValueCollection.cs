using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KsWare.JsonFx {

	// 
	// System.Collections.Specialized.NameObjectCollectionBase : ICollection, IEnumerable, ISerializable, IDeserializationCallback
	//   System.Collections.Specialized.NameValueCollection
	//     System.Net.WebHeaderCollection
	//

	/// <summary> Represents a collection of associated String keys and object values that can be accessed either with the key or with the index.
	/// </summary>
	public class NameValueCollection:IDictionary<string,object>, IList<object>,IDictionary {

		readonly List<KeyValuePair<string,object>> _list=new List<KeyValuePair<string, object>>();
		readonly Dictionary<string,object> _dic=new Dictionary<string, object>();


		public IEnumerator<KeyValuePair<string, object>> GetEnumerator() { return _list.GetEnumerator(); }
		void IDictionary.Remove(object key) { Remove((string) key); }
		object IDictionary.this[object key] { get { return this[(string) key]; } set { this[(string) key]=value; } }

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public void Add(KeyValuePair<string, object> item) { _list.Add(item);_dic.Add(item.Key,item.Value); }
		bool IDictionary.Contains(object key) { return _dic.ContainsKey((string) key); }
		void IDictionary.Add(object key, object value) { Add((string)key,value); }
		public void Clear() { _list.Clear();_dic.Clear(); }
		IDictionaryEnumerator IDictionary.GetEnumerator() { return _dic.GetEnumerator(); }
		public bool Contains(KeyValuePair<string, object> item) { return _list.Contains(item); }
		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) { _list.CopyTo(array,arrayIndex); }
		public bool Remove(KeyValuePair<string, object> item) { _list.Remove(item); return _dic.Remove(item.Key); }
		void ICollection.CopyTo(Array array, int index) { ((ICollection)_list).CopyTo(array, index);}
		public int Count { get { return _list.Count; } }
		object ICollection.SyncRoot { get { return _list; } }

		bool ICollection.IsSynchronized { get { return ((ICollection) _list).IsSynchronized; } }

		ICollection IDictionary.Values { get { return ((IDictionary)_dic).Values; } }

		public bool IsReadOnly { get { return false; } }
		bool IDictionary.IsFixedSize { get { return false; } }

		public bool ContainsKey(string key) { return _dic.ContainsKey(key); }
		public void Add(string key, object value) { _list.Add(new KeyValuePair<string, object>(key,value)); _dic.Add(key,value); }
		public bool Remove(string key) { _dic.Remove(key); return _list.RemoveAll(pair => pair.Key==key)>0; }
		public bool TryGetValue(string key, out object value) { if(_dic.ContainsKey(key)){value=_dic[key];return true;} else {value = null;return false;}}
		public object this[string key] {
			get {
				if(_dic.ContainsKey(key)) return _dic[key];
				#region index fallback
				int i;
				if(int.TryParse(key, out i)) {
					if(i<0) throw new ArgumentOutOfRangeException("key");
					if(i>=_list.Count) throw new ArgumentOutOfRangeException("key");
					return _list[i].Value;
				}
				#endregion
				throw new KeyNotFoundException();
			} 
			set { 
				if(_dic.ContainsKey(key)) {
					_dic[key] = value;
					var indexOf = _list.FindIndex(pair => pair.Key==key);
					_list[indexOf]=new KeyValuePair<string, object>(key,value);
					return;
				}
				int i;
				if(int.TryParse(key, out i)) {
					if (i >= 0 && i < _list.Count) {
						var k = _list[i].Key;
						_dic[k] = value;
						_list[i]=new KeyValuePair<string, object>(k,value);
						return;
					}
				}
				_dic.Add(key,value);
				_list.Add(new KeyValuePair<string, object>(key,value));
			}
		}
		public ICollection<string> Keys {get {return _list.Select(pair => pair.Key).ToList();}}
		ICollection IDictionary.Keys { get { throw new NotImplementedException(); } }

		public ICollection<object> Values {get {return _list.Select(pair => pair.Value).ToList();}}


		IEnumerator<object> IEnumerable<object>.GetEnumerator() { throw new NotImplementedException("{3C3FA0FF-0B9C-48B7-A73B-DB6ABB8B919B}"); }
//		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		void ICollection<object>.Add(object item) { Add((KeyValuePair<string, object>) item); }
		bool ICollection<object>.Contains(object item) { 
			if(item is KeyValuePair<string,object>) return _list.Contains((KeyValuePair<string, object>) item);
			if(item is string) return _dic.ContainsKey((string) item);
			throw new ArgumentException("Invalid type of argument","item");
		}
		void ICollection<object>.CopyTo(object[] array, int arrayIndex) {
			int i = 0;
			foreach (var pair in _list) {
				if(i+arrayIndex>=array.Length) break;
				array[i+arrayIndex] = pair;
				i++;
			}
		}
		bool ICollection<object>.Remove(object item) { throw new NotImplementedException(); }
		int IList<object>.IndexOf(object item) { throw new NotImplementedException(); }
		void IList<object>.Insert(int index, object item) { throw new NotImplementedException(); }
		public void RemoveAt(int index) { 
			var key=_list[index].Key;
			_list.RemoveAt(index);
			_dic.Remove(key);
		}
		public object this[int index] {
			get { return _list[index].Value; } 
			set { 
				var key=_list[index].Key;
				_dic[key] = value;
				_list[index]=new KeyValuePair<string, object>(key,value);
			}
		}
	}
}