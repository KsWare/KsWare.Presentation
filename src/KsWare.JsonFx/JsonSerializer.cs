// This class encodes and decodes JSON strings.
// Spec. details, see http://www.json.org/
//
// JSON uses Arrays and Objects. These correspond here to the datatypes ArrayList and NameValueCollection.
// All numbers are parsed to doubles.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KsWare.JsonFx {

	public class JsonSerializer {

		protected enum JsonToken {
			None,
			CurlyOpen,
			CurlyClose,
			SquaredOpen,
			SquaredClose,
			Colon,
			Comma,
			String,
			Number,
			True,
			False,
			Null,
			NaN,
			Infinity,
			Undefined
		}
		private const int builderCapacity = 2000;

		private Type _hashtableType = typeof (NameValueCollection);
		private Type _arrayType = typeof (object[]);
		private Type _numberType = typeof(double);
		private bool _throwOnParserError=true;


		public object DeserializeObject(string json) {
			bool success = true;
			return JsonDecode(json, typeof(object), ref success);
		}

		public object Deserialize(string json,Type type) {
			bool success = true;
			return JsonDecode(json, type, ref success);
		}

		public T Deserialize<T>(string json) {
			bool success = true;
			return (T)JsonDecode(json, typeof(T), ref success);
		}

		public void Serialize(object obj, StringBuilder output) {
			bool success = SerializeValue(obj, null, output);
			this.DoNothing(success);
		}

		public string Serialize(object obj) {
			var builder = new StringBuilder(builderCapacity);
			bool success = SerializeValue(obj, null, builder);
			return (success ? builder.ToString() : null);		
		}

		/// <summary> Parses the string json into a value; and fills 'success' with the successfullness of the parse.
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <param name="type"> </param>
		/// <param name="success">Successful parse?</param>
		/// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
		private object JsonDecode(string json, Type type, ref bool success) {
			if (json == null) return null;
			var charArray = json.ToCharArray();
			int index = 0;
			var value = ParseValue(charArray, ref index, type, ref success);
			return value;
		}

		private object ParseObject(char[] json, ref int index, Type type, ref bool success) {
			if(typeof (JsonValue).IsAssignableFrom(type)) return ParseObject3(json, ref index, ref success);
			if(type==null || type==typeof(object) ||type==typeof(NameValueCollection)) return ParseObject2(json, ref index, type, ref success);

			var members = new Dictionary<string,MemberInfo>();
			foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public /*| BindingFlags.NonPublic*/)) {
				var ignore=(JsonIgnoreAttribute)prop.GetCustomAttributes(typeof (JsonIgnoreAttribute),false).FirstOrDefault();
				if(ignore!=null)continue;
				var a=(JsonNameAttribute)prop.GetCustomAttributes(typeof (JsonNameAttribute),false).FirstOrDefault();
				if(a!=null) members.Add(a.Name,prop);
				else members.Add(prop.Name,prop);
			}
			foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public /*| BindingFlags.NonPublic*/)) {
				var ignore=(JsonIgnoreAttribute)field.GetCustomAttributes(typeof (JsonIgnoreAttribute),false).FirstOrDefault();
				if(ignore!=null)continue;
				var a=(JsonNameAttribute)field.GetCustomAttributes(typeof (JsonNameAttribute),false).FirstOrDefault();
				if(a!=null) members.Add(a.Name,field);
				else members.Add(field.Name,field);
			}

			var obj = Activator.CreateInstance(type);
			JsonToken token;

			// {
			NextToken(json, ref index);

			bool done = false;
			while (!done) {
				token = LookAhead(json, index);
				if (token == JsonToken.None) {
					success = false; 
					if(_throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
					return null;
				} else if (token == JsonToken.Comma) {
					NextToken(json, ref index);
				} else if (token == JsonToken.CurlyClose) {
					NextToken(json, ref index);
					return obj;
				} else {
					// name
					string name = (string)ParseString(json, ref index, typeof(string), ref success);
					if (!success) {
						success = false;
						if(_throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
						return null;
					}

					// :
					token = NextToken(json, ref index);
					if (token != JsonToken.Colon) {
						success = false;
						if(_throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
						return null;
					}

					

					Type memberType=null;
					MemberInfo memberInfo=null;
					if(members.ContainsKey(name)) {
						memberInfo = members[name];
						switch (memberInfo.MemberType) {
							case MemberTypes.Field   : memberType = ((FieldInfo   ) memberInfo).FieldType;break;
							case MemberTypes.Property: memberType = ((PropertyInfo) memberInfo).PropertyType;break;
							default:throw new NotImplementedException("{569C8499-17DB-46D8-8B93-DCC5BB9681CC}");
						}
					}
					
					// value
					object value = ParseValue(json, ref index, memberType, ref success);
					if (!success) {
						success = false;
						if(_throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
						return null;
					}

					if (IsNumeric(memberType) && IsNumeric(value)) value=ChangeNumericType(value, memberType);

					if(memberInfo!=null){
						switch (memberInfo.MemberType) {
							case MemberTypes.Field   : ((FieldInfo   ) memberInfo).SetValue(obj,value,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,CultureInfo.InvariantCulture);break;
							case MemberTypes.Property: ((PropertyInfo) memberInfo).SetValue(obj,value,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,null,CultureInfo.InvariantCulture);break;
							default: throw new NotImplementedException("{13B261D3-D24B-4331-B933-3DBA45612043}");
						}
					}
				}
			}

			return obj;
		}

		private static bool IsNumericOrNull(object typeOrValue) {return typeOrValue == null || IsNumeric(typeOrValue);}

		private static bool IsNumeric(object typeOrValue) {
			if(typeOrValue==null) return false;
			var type = (typeOrValue is Type ? (Type) typeOrValue : typeOrValue.GetType());
			return (type == typeof (SByte  ) || type == typeof (Int16 ) || type == typeof (Int32 ) || type == typeof (Int64 ) ||
			        type == typeof (Byte   ) || type == typeof (UInt16) || type == typeof (UInt32) || type == typeof (UInt64) ||
			        type == typeof (Single ) || type == typeof (Double) ||
			        type == typeof (Decimal)
				);
		}

		private static object ChangeNumericType(object value, Type type) {
			var v=System.Convert.ChangeType(value, type);
			var v1=System.Convert.ChangeType(v, value.GetType());
			if(((IComparable)value).CompareTo(v1)!=0) throw new InvalidCastException(new InvalidCastException().Message+ " {8B83C505-14BD-4373-A935-1F1A54FAB458}");
			return v;
		}

		private static void ConvertNumeric_Test() {
			Debug.WriteLine(ChangeNumericType("12", typeof (int)));
			Debug.WriteLine(ChangeNumericType(12M, typeof (int)));
			Debug.WriteLine(ChangeNumericType(12D, typeof (int)));
			try { Debug.WriteLine(ChangeNumericType("12.1", typeof (int)));}catch (Exception ex) {Debug.WriteLine(ex.Message);}
			try { Debug.WriteLine(ChangeNumericType(12.1M, typeof (int)));}catch (Exception ex) {Debug.WriteLine(ex.Message);}
			try { Debug.WriteLine(ChangeNumericType(12.1D, typeof (int)));}catch (Exception ex) {Debug.WriteLine(ex.Message);}
		}

		private object ParseObject2(char[] json, ref int index, Type type, ref bool success) {
			if (typeof (JsonValue).IsAssignableFrom(type)) return ParseObject3(json, ref index, ref success);

			var table = new NameValueCollection();
			JsonToken token;

			// {
			NextToken(json, ref index);

			while (true) {
				token = LookAhead(json, index);
				if (token == JsonSerializer.JsonToken.None) {
					success = false;
					return null;
				} else if (token == JsonSerializer.JsonToken.Comma) {
					NextToken(json, ref index);
				} else if (token == JsonSerializer.JsonToken.CurlyClose) {
					NextToken(json, ref index);
					return table;
				} else {

					// name
					string name = (string)ParseString(json, ref index, typeof(string),ref success);
					if (!success) {
						success = false;
						return null;
					}

					// :
					token = NextToken(json, ref index);
					if (token != JsonSerializer.JsonToken.Colon) {
						success = false;
						return null;
					}

					// value
					object value = ParseValue(json, ref index, null, ref success);
					if (!success) {
						success = false;
						return null;
					}

					table[name] = value;
				}
			}
		}

		private JsonObject ParseObject3(char[] json, ref int index, ref bool success) {
			var table = new JsonObject(new List<KeyValuePair<string, JsonValue>>());

			// {
			NextToken(json, ref index);

			while (true) {
				var token = LookAhead(json, index);
				switch (token) {
					case JsonToken.None      : success = false;return null;
					case JsonToken.Comma     : NextToken(json, ref index);break;
					case JsonToken.CurlyClose: NextToken(json, ref index); return table;
					default: {
						// name
						string name = (string)ParseString(json, ref index,typeof(string), ref success);
						if (!success) {success = false;return null;}

						// :
						token = NextToken(json, ref index);
						if (token != JsonToken.Colon) {success = false;return null;}

						// value
						var value = ParseValue3(json, ref index, ref success);
						if (!success) {success = false;return null;}

						table[name] = value;
					}
					break;
				}
			}
		}

		protected object ParseArray(char[] json, ref int index, Type type, ref bool success) {
			if(type==null || type==typeof(object) || typeof (JsonValue).IsAssignableFrom(type)) return ParseArray2(json, ref index,type, ref success);

			var obj = Activator.CreateInstance(type);
			var list = (IList) obj;
			Type memberType = null; //TODO

			// [
			NextToken(json, ref index);

			bool done = false;
			while (!done) {
				var token = LookAhead(json, index);
				if (token == JsonSerializer.JsonToken.None) {
					if(_throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
					success = false;
					return null;
				} else if (token == JsonToken.Comma) {
					NextToken(json, ref index);
				} else if (token == JsonToken.SquaredClose) {
					NextToken(json, ref index);
					break;
				} else {
					object value = ParseValue(json, ref index, memberType, ref success);
					if (!success) {
						return null;
					}

					list.Add(value);
				}
			}

			return obj;
		}

		protected object ParseArray2(char[] json, ref int index, Type type, ref bool success) {
			if(typeof (JsonValue).IsAssignableFrom(type)) return ParseArray3(json, ref index, ref success);

			ArrayList array = new ArrayList();

			// [
			NextToken(json, ref index);

			while (true) {
				var token = LookAhead(json, index);
				if (token == JsonToken.None) {
					if(_throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
					success = false;
					return null;
				} else if (token == JsonToken.Comma) {
					NextToken(json, ref index);
				} else if (token == JsonToken.SquaredClose) {
					NextToken(json, ref index);
					break;
				} else {
					object value = ParseValue(json, ref index, null, ref success);
					if (!success) return null;

					array.Add(value);
				}
			}

			if(_arrayType==typeof(ArrayList)) return array;
			if(_arrayType==typeof(object[])) return array.ToArray();
			if(_arrayType==typeof(List<object>)) return new List<object>(array.ToArray());
			throw new InvalidOperationException("Type of Array not supported! "+_arrayType.Name);
		}

		protected JsonArray ParseArray3(char[] json, ref int index, ref bool success) {
			var array = new JsonArray(new List<JsonValue>());

			// [
			NextToken(json, ref index);

			while (true) {
				switch(LookAhead(json, index)){
					case JsonToken.None        : return (JsonArray)Error(index,ref success);
					case JsonToken.Comma       : NextToken(json, ref index); break;
					case JsonToken.SquaredClose: NextToken(json, ref index); return array;
					default:{
						var value = ParseValue3(json, ref index, ref success);
						if (!success) return null;
						array.Add(value);
					}
					break;
				}
			}
		}
		
		protected JsonValue ParseValue3(char[] json, ref int index,ref bool success) {
			switch (LookAhead(json, index)) {
				case JsonToken.String     : return ParseString3(json, ref index, ref success);
				case JsonToken.Number     : return ParseNumber3(json, ref index, ref success);
				case JsonToken.CurlyOpen  : return ParseObject3(json, ref index, ref success);
				case JsonToken.SquaredOpen: return ParseArray3 (json, ref index, ref success);
				case JsonToken.True       : NextToken(json, ref index);return new JsonBool     (true                   );
				case JsonToken.False      : NextToken(json, ref index);return new JsonBool     (false                  );
				case JsonToken.Null       : NextToken(json, ref index);return     JsonNull     .Value                   ;
				case JsonToken.NaN        : NextToken(json, ref index);return new JsonNumber   (double.NaN             );
				case JsonToken.Infinity   : NextToken(json, ref index);return new JsonNumber   (double.PositiveInfinity);
				case JsonToken.Undefined  : NextToken(json, ref index);return     JsonUndefined.Value                   ;
				case JsonToken.None       : break;
			}
			return (JsonValue)Error(index, ref success);
		}

		protected object ParseValue(char[] json, ref int index, Type type, ref bool success) {
			if (typeof (JsonValue).IsAssignableFrom(type)) return ParseValue3(json, ref index, ref success);
			switch (LookAhead(json, index)) {
				case JsonToken.String     : return ParseString(json, ref index, type, ref success);
				case JsonToken.Number     : return ParseNumber(json, ref index, type, ref success);
				case JsonToken.CurlyOpen  : return ParseObject(json, ref index, type, ref success);
				case JsonToken.SquaredOpen: return ParseArray (json, ref index, type, ref success);
				case JsonToken.True       : NextToken(json, ref index);return true;
				case JsonToken.False      : NextToken(json, ref index);return false;
				case JsonToken.Null       : NextToken(json, ref index);return null;
				case JsonToken.NaN        : NextToken(json, ref index);return GetNaN();
				case JsonToken.Infinity   : NextToken(json, ref index);return GetInfinity();
				case JsonToken.Undefined  : NextToken(json, ref index);return GetUndefined();
				case JsonToken.None       : break;
			}
			return Error(index, ref success);
		}

		private object Error(int index, ref bool success) {
			if(_throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
			success = false;
			return null;
		}

		private object GetUndefined() { return Undefined.Value; }

		private object GetNaN() { 
			if(_numberType==typeof(double)) {
				return Double.NaN;
			} else if(_numberType==typeof(float)) {
				return Single.NaN;
			} else if(_numberType==typeof(decimal)) {
				return new Decimal(Double.NaN);//???
			} else {
				throw new InvalidOperationException("Type of number not supported! "+ _numberType.Name);
			}			
		}

		private object GetInfinity() { 
			if(_numberType==typeof(double)) {
				return Double.PositiveInfinity;
			} else if(_numberType==typeof(float)) {
				return Single.PositiveInfinity;
			} else if(_numberType==typeof(decimal)) {
				return new Decimal(Double.PositiveInfinity); //???
			} else {
				throw new InvalidOperationException("Type of number not supported! "+ _numberType.Name);
			}			
		}

		protected JsonString ParseString3(char[] json, ref int index, ref bool success) {
			StringBuilder s = new StringBuilder(builderCapacity);
			char c;

			EatWhitespace(json, ref index);

			// "
			c = json[index++];

			bool complete = false;
			while (!complete) {
				if (index == json.Length) break;

				c = json[index++];
				if (c == '"') {complete = true;break;} 
				if (c == '\\') {
					if (index == json.Length) break;
					c = json[index++];
					switch (c) {
						case '"' :s.Append('"');break;
						case '\\':s.Append('\\');break;
						case '/' :s.Append('/');break;
						case 'b' :s.Append('\b');break;
						case 'f' :s.Append('\f');break;
						case 'n' :s.Append('\n');break;
						case 'r' :s.Append('\r');break;
						case 't' :s.Append('\t');break;
						case 'u' :{
							int remainingLength = json.Length - index;
							if (remainingLength >= 4) {
								// parse the 32 bit hex into an integer codepoint
								uint codePoint;
								if (!UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)) {
									return (JsonString) Error(index, ref success);
								}
								// convert the integer codepoint to a unicode char and add to string
								s.Append(Char.ConvertFromUtf32((int) codePoint));
								// skip 4 chars
								index += 4;
							} else {
								return (JsonString) Error(index, ref success);
							}
						}
						break;
					}

				} else {
					s.Append(c);
				}

			}

			if (!complete) return (JsonString) Error(index, ref success);
			return new JsonString(s.ToString());
		}

		protected object ParseString(char[] json, ref int index, Type type, ref bool success) {
			var isJsonType = typeof (JsonValue).IsAssignableFrom(type);
			StringBuilder s = new StringBuilder(builderCapacity);
			char c;

			EatWhitespace(json, ref index);

			// "
			c = json[index++];

			bool complete = false;
			while (!complete) {

				if (index == json.Length) {
					break;
				}

				c = json[index++];
				if (c == '"') {
					complete = true;
					break;
				} else if (c == '\\') {

					if (index == json.Length) {
						break;
					}
					c = json[index++];
					if (c == '"') {
						s.Append('"');
					} else if (c == '\\') {
						s.Append('\\');
					} else if (c == '/') {
						s.Append('/');
					} else if (c == 'b') {
						s.Append('\b');
					} else if (c == 'f') {
						s.Append('\f');
					} else if (c == 'n') {
						s.Append('\n');
					} else if (c == 'r') {
						s.Append('\r');
					} else if (c == 't') {
						s.Append('\t');
					} else if (c == 'u') {
						int remainingLength = json.Length - index;
						if (remainingLength >= 4) {
							// parse the 32 bit hex into an integer codepoint
							uint codePoint;
							if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint))) {
								return "";
							}
							// convert the integer codepoint to a unicode char and add to string
							s.Append(Char.ConvertFromUtf32((int) codePoint));
							// skip 4 chars
							index += 4;
						} else {
							break;
						}
					}

				} else {
					s.Append(c);
				}

			}

			if (!complete) {
				if(_throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
				success = false;
				return isJsonType?new JsonString(null):null;
			}
			return isJsonType?(object)new JsonString(s.ToString()):s.ToString();
		}

		protected object ParseNumber(char[] json, ref int index, Type type, ref bool success) {
			EatWhitespace(json, ref index);

			int lastIndex = GetLastIndexOfNumber(json, index);
			int charLength = (lastIndex - index) + 1;

			if (_numberType == typeof (JsonNumber)) { 
				// convert an integer to Int32/Int64/Decimal using the samllest possible type
				// convert a float to Double
				var s = new string(json, index, charLength);
				var startIndex=index;
				index = lastIndex + 1;
				// Int64.MinValue -9223372036854775808
				// Int64.MaxValue 9223372036854775807
				// Double.MaxValue 1.7976931348623157E+308
				// Double.MinValue -1.7976931348623157E+308
				// Double.Epsilon
				// Decimal.MaxValue	79228162514264337593543950335
				// Decimal.MinValue -79228162514264337593543950335
				if(s.Contains(".")||s.Contains("e")||s.Contains("E")) {
					double doubleValue;
					var isDouble = Double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue);
					if (!isDouble) {
						decimal dec;
						var isDecimal=Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out dec);
						if (!isDecimal) return s;//???
						return new JsonNumber(dec);
					}
					return new JsonNumber(doubleValue);
				} else {
					long int64;
					var isInt64 = Int64.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out int64);
					if (!isInt64) {
						decimal dec;
						var isDecimal=Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out dec);
						if (!isDecimal) return s;//???
						return new JsonNumber(dec);
					}
					//if (int64 >= Byte .MinValue && int64 <= Byte .MaxValue) return new JsonNumber((Byte ) int64);
					//if (int64 >= Int16.MinValue && int64 <= Int16.MaxValue) return new JsonNumber((Int16) int64);
					if (int64 >= Int32.MinValue && int64 <= Int32.MaxValue) return new JsonNumber((Int32) int64);
					return new JsonNumber(int64);
				}
			}else if(_numberType==typeof(double)) {
				double number;
				success = Double.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);	
				if(!success && _throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
				index = lastIndex + 1;
				return number;
			} else if(_numberType==typeof(float)) {
				float number;
				success = Single.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);
				if(!success && _throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
				index = lastIndex + 1;
				return number;
			} else if(_numberType==typeof(decimal)) {
				decimal number;
				success = Decimal.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);
				if(!success && _throwOnParserError) throw new FormatException("Invalid JSON string! Position:"+index);
				index = lastIndex + 1;
				return number;
			} else {
				throw new InvalidOperationException("Type of number not supported! "+ _numberType.Name);
			}
		}

		protected JsonNumber ParseNumber3(char[] json, ref int index, ref bool success) {
			EatWhitespace(json, ref index);

			int lastIndex = GetLastIndexOfNumber(json, index);
			int charLength = (lastIndex - index) + 1;

			// convert an integer to Int32/Int64/Decimal using the samllest possible type
			// convert a float to Double
			var s = new string(json, index, charLength);
			var startIndex=index;
			index = lastIndex + 1;
			// Int64.MinValue -9223372036854775808
			// Int64.MaxValue 9223372036854775807
			// Double.MaxValue 1.7976931348623157E+308
			// Double.MinValue -1.7976931348623157E+308
			// Double.Epsilon
			// Decimal.MaxValue	79228162514264337593543950335
			// Decimal.MinValue -79228162514264337593543950335
			if(s.Contains(".")||s.Contains("e")||s.Contains("E")) {
				double doubleValue;
				var isDouble = Double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue);
				if (!isDouble) {
					decimal dec;
					var isDecimal=Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out dec);
					if (!isDecimal) return (JsonNumber) Error(startIndex, ref success);
					success = true; return new JsonNumber(dec);
				}
				return new JsonNumber(doubleValue);
			} else {
				long int64;
				var isInt64 = Int64.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out int64);
				if (!isInt64) {
					decimal dec;
					var isDecimal=Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out dec);
					if (!isDecimal) return (JsonNumber) Error(startIndex, ref success);
					success = true; 
					return new JsonNumber(dec);
				}
				//if (int64 >= Byte .MinValue && int64 <= Byte .MaxValue) return new JsonNumber((Byte ) int64);
				//if (int64 >= Int16.MinValue && int64 <= Int16.MaxValue) return new JsonNumber((Int16) int64);
				if (int64 >= Int32.MinValue && int64 <= Int32.MaxValue) return new JsonNumber((Int32) int64);
				success = true; 
				return new JsonNumber(int64);
			}
		}

		protected static int GetLastIndexOfNumber(char[] json, int index) {
			int lastIndex;

			for (lastIndex = index; lastIndex < json.Length; lastIndex++) {
				if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1) {
					break;
				}
			}
			return lastIndex - 1;
		}

		protected static void EatWhitespace(char[] json, ref int index) {
			for (; index < json.Length; index++) {
				if (" \t\n\r".IndexOf(json[index]) == -1) {
					break;
				}
			}
		}

		protected static JsonToken LookAhead(char[] json, int index) {
			int saveIndex = index;
			return NextToken(json, ref saveIndex);
		}

		protected static JsonToken NextToken(char[] json, ref int index) {
			EatWhitespace(json, ref index);

			if (index == json.Length) {
				return JsonSerializer.JsonToken.None;
			}

			char c = json[index];
			index++;
			switch (c) {
				case '{':
					return JsonSerializer.JsonToken.CurlyOpen;
				case '}':
					return JsonSerializer.JsonToken.CurlyClose;
				case '[':
					return JsonSerializer.JsonToken.SquaredOpen;
				case ']':
					return JsonSerializer.JsonToken.SquaredClose;
				case ',':
					return JsonSerializer.JsonToken.Comma;
				case '"':
					return JsonSerializer.JsonToken.String;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
					return JsonSerializer.JsonToken.Number;
				case ':':
					return JsonSerializer.JsonToken.Colon;
			}
			index--;

			int remainingLength = json.Length - index;

			// false
			if (remainingLength >= 5) {
				if (json[index] == 'f' &&
				    json[index + 1] == 'a' &&
				    json[index + 2] == 'l' &&
				    json[index + 3] == 's' &&
				    json[index + 4] == 'e') {
					index += 5;
					return JsonSerializer.JsonToken.False;
				}
			}

			// true
			if (remainingLength >= 4) {
				if (json[index] == 't' &&
				    json[index + 1] == 'r' &&
				    json[index + 2] == 'u' &&
				    json[index + 3] == 'e') {
					index += 4;
					return JsonSerializer.JsonToken.True;
				}
			}

			// null
			if (remainingLength >= 4) {
				if (json[index] == 'n' &&
				    json[index + 1] == 'u' &&
				    json[index + 2] == 'l' &&
				    json[index + 3] == 'l') {
					index += 4;
					return JsonSerializer.JsonToken.Null;
				}
			}

			// NaN
			if (remainingLength >= 3) {
				if (json[index] == 'N' &&
				    json[index + 1] == 'a' &&
				    json[index + 2] == 'N'){
					index += 3;
					return JsonSerializer.JsonToken.NaN;
				}
			}
			// Infinity
			if (remainingLength >= 8) {
				if (json[index    ] == 'I' &&
				    json[index + 1] == 'n' &&
					json[index + 2] == 'f' &&
					json[index + 3] == 'i' &&
					json[index + 4] == 'n' &&
					json[index + 5] == 'i' &&
					json[index + 6] == 't' &&
				    json[index + 7] == 'y'){
					index += 8;
					return JsonSerializer.JsonToken.Infinity;
				}
			}

			// undefined
			if (remainingLength >= 9) {
				if (json[index    ] == 'u' &&
				    json[index + 1] == 'n' &&
					json[index + 2] == 'd' &&
					json[index + 3] == 'e' &&
					json[index + 4] == 'f' &&
					json[index + 5] == 'i' &&
					json[index + 6] == 'n' &&
					json[index + 7] == 'e' &&
				    json[index + 8] == 'd'){
					index += 9;
					return JsonSerializer.JsonToken.Undefined;
				}
			}

			return JsonSerializer.JsonToken.None;
		}

		protected bool SerializeValue(object value, MemberInfo memberInfo, StringBuilder builder) {
			bool success = true;
			JsonType customJsonType = GetCustomJsonType(value);

			if(value == null) {
				builder.Append("null");
			} else if (value.GetType().IsEnum) {
				if(customJsonType==JsonType.String) success = SerializeString(GetStringValue(value), builder);
				if(customJsonType==JsonType.Numeric) success = SerializeNumber(GetNumericValue(value), builder);
			} else if (value is string) {
				success = SerializeString((string) value, builder);
			} else if (value is char) {
				success = SerializeString(new string((char)value,1), builder);
			} else if (value is Boolean) {
				builder.Append(((bool)value)?"true":"false");
			} else if (value.GetType().IsPrimitive) { //Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Double, Single / Boolean, Char
				success = SerializeNumber(Convert.ToDouble(value), builder);
			} else if (value is DateTime) {
				throw new NotSupportedException("{A3009E2B-0976-4593-8B9B-C54A3635476B}");
			} else if (value is TimeSpan) {
				throw new NotSupportedException("{B2D35EF0-9495-46D5-8338-AB4D88175C9D}");
			} else if (value is Guid) {
				success = SerializeString(((Guid) value).ToString("B"), builder);
			} else if (value is IDictionary) {
				success = SerializeObject(value, builder);
			} else if (value is IList || value.GetType().IsArray  ) { //TODO supported array types
				success = SerializeArray(value, builder);
			} else if (value is IConvertible) {
				var convertible = (IConvertible) value;
				switch (convertible.GetTypeCode()) {
					case TypeCode.Boolean : return SerializeValue(convertible.ToBoolean(CultureInfo.InvariantCulture),null,builder);
					case TypeCode.Byte    : return SerializeValue(convertible.ToByte    (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.Char    : return SerializeValue(convertible.ToChar    (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.Decimal : return SerializeValue(convertible.ToDecimal (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.Double  : return SerializeValue(convertible.ToDouble  (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.Int16   : return SerializeValue(convertible.ToInt16   (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.Int32   : return SerializeValue(convertible.ToInt32   (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.Int64   : return SerializeValue(convertible.ToInt64   (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.SByte   : return SerializeValue(convertible.ToSByte   (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.Single  : return SerializeValue(convertible.ToSingle  (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.String  : return SerializeValue(convertible.ToString  (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.UInt16  : return SerializeValue(convertible.ToUInt16  (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.UInt32  : return SerializeValue(convertible.ToUInt32  (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.UInt64  : return SerializeValue(convertible.ToUInt64  (CultureInfo.InvariantCulture),null,builder);
					case TypeCode.DateTime: return SerializeValue(convertible.ToDateTime(CultureInfo.InvariantCulture),null,builder);
//					case TypeCode.Object  : return SerializeValue(convertible.ToObject  (CultureInfo.InvariantCulture),null,builder);
//					case TypeCode.DBNull  : return SerializeValue(convertible.ToDBNull  (CultureInfo.InvariantCulture),null,builder);
//					case TypeCode.Empty   : return SerializeValue(convertible.ToEmpty   (CultureInfo.InvariantCulture),null,builder);
					default: throw new NotSupportedException("Type of value not supported! {69C03170-EA65-4156-8388-86C0E6984CA4}");
				}
			} else {
				success = SerializeObject(value, builder);
			}
			return success;
		}

		private double GetNumericValue(object value) { return double.Parse(Enum.Format(value.GetType(), value, "D"),NumberStyles.Integer,CultureInfo.InvariantCulture); }

		private JsonType GetCustomJsonType(object value) {
			if(value==null) return JsonType.None;
			var jsonTypeAttribute=(JsonTypeAttribute)value.GetType().GetCustomAttributes(typeof (JsonTypeAttribute), false).FirstOrDefault();
			if(jsonTypeAttribute==null)return JsonType.None;
			return jsonTypeAttribute.Type;
		}

		private string GetStringValue(object enumVal) {
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attribute = memInfo[0].GetCustomAttributes(typeof(JsonNameAttribute), false).FirstOrDefault();
			if(attribute!=null) return ((JsonNameAttribute)attribute).Name;
			return enumVal.ToString();
		}

		protected bool SerializeObject(object anObject, StringBuilder builder) {
			if(anObject is IDictionary) return SerializeIDictionary((IDictionary) anObject, builder); // Hashtable, DictionaryBase, Dictionary<>

			builder.Append("{");

			var type = anObject.GetType();
			var memberInfos = new List<MemberInfo>();
//			memberInfos.AddRange(type.GetMembers(BindingFlags.Instance|BindingFlags.Public/*|BindingFlags.NonPublic*/));
//			for (int i = 0; i < memberInfos.Count; i++) {
//				if(memberInfos[i].MemberType==MemberTypes.Field)continue;
//				if(memberInfos[i].MemberType==MemberTypes.Property)continue;
//				memberInfos.RemoveAt(i--);
//			}
			memberInfos.AddRange(type.GetFields(BindingFlags.Instance|BindingFlags.Public/*|BindingFlags.NonPublic*/));
			memberInfos.AddRange(type.GetProperties(BindingFlags.Instance|BindingFlags.Public/*|BindingFlags.NonPublic*/));

			bool first = true;
			foreach (var memberInfo in memberInfos) {
				if(memberInfo.MemberType==MemberTypes.Property && ((PropertyInfo)memberInfo).GetIndexParameters().Length>0)
					continue;//indexer are not supported
				var ignore = (JsonIgnoreAttribute)memberInfo.GetCustomAttributes(typeof (JsonIgnoreAttribute), false).FirstOrDefault();
				if(ignore!=null) continue;
				var n=(JsonNameAttribute) memberInfo.GetCustomAttributes(typeof (JsonNameAttribute), false).FirstOrDefault();
				
				var key = n != null ? n.Name : memberInfo.Name;
				object value = null;
				switch (memberInfo.MemberType) {
					case MemberTypes.Field   : value=((FieldInfo)memberInfo).GetValue(anObject);break;
					case MemberTypes.Property: value=((PropertyInfo)memberInfo).GetValue(anObject,null);break;
				}

				if (!first) builder.Append(", ");

				SerializeString(key, builder);
				builder.Append(":");
				if (!SerializeValue(value, memberInfo, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append("}");
			return true;
		}

		protected bool SerializeIDictionary(IDictionary anObject, StringBuilder builder) {
			builder.Append("{");

			IDictionaryEnumerator e = anObject.GetEnumerator();
			bool first = true;
			while (e.MoveNext()) {
				string key = e.Key.ToString();
				object value = e.Value;

				if (!first) builder.Append(", ");

				SerializeString(key, builder);
				builder.Append(":");
				if (!SerializeValue(value, null, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append("}");
			return true;
		}

		protected bool SerializeArray(object anArray, StringBuilder builder) {
			if(anArray is ArrayList) return SerializeArrayList((ArrayList) anArray, builder);
			if(anArray is IEnumerable) return SerializeIEnumerable((ICollection) anArray, builder);
			if(anArray.GetType().IsArray) return SerializeNativeArray((Array)anArray, builder);

			throw new InvalidOperationException("Array not serializable!");
//			return false;
		}

		protected bool SerializeNativeArray(Array anArray, StringBuilder builder) {
			if(anArray.Rank>0) throw new InvalidOperationException("Array not serializable!");

			builder.Append("[");

			bool first = true;
			var length = anArray.GetLength(0);
			for (int i = 0; i < length; i++) {
				object value = anArray.GetValue(i);

				if (!first) builder.Append(", ");

				if (!SerializeValue(value, null, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append("]");
			return true;
		}

		protected bool SerializeIEnumerable(IEnumerable anArray, StringBuilder builder) {
			builder.Append("[");

			bool first = true;
			foreach (var value in anArray) {
				if (!first) builder.Append(", ");

				if (!SerializeValue(value, null, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append("]");
			return true;
		}

		protected bool SerializeArrayList(ArrayList anArray, StringBuilder builder) {
			builder.Append("[");

			bool first = true;
			for (int i = 0; i < anArray.Count; i++) {
				object value = anArray[i];

				if (!first) builder.Append(", ");

				if (!SerializeValue(value, null, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append("]");
			return true;
		}

		protected bool SerializeString(string aString, StringBuilder builder) {
			builder.Append("\"");

			char[] charArray = aString.ToCharArray();
			for (int i = 0; i < charArray.Length; i++) {
				char c = charArray[i];
				if (c == '"') {
					builder.Append("\\\"");
				} else if (c == '\\') {
					builder.Append("\\\\");
				} else if (c == '\b') {
					builder.Append("\\b");
				} else if (c == '\f') {
					builder.Append("\\f");
				} else if (c == '\n') {
					builder.Append("\\n");
				} else if (c == '\r') {
					builder.Append("\\r");
				} else if (c == '\t') {
					builder.Append("\\t");
				} else {
					int codepoint = Convert.ToInt32(c);
					if ((codepoint >= 32) && (codepoint <= 126)) {
						builder.Append(c);
					} else {
						builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
					}
				}
			}

			builder.Append("\"");
			return true;
		}

		protected static bool SerializeNumber(double number, StringBuilder builder) {
			builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
			return true;
		}

		public class Undefined{public static readonly Undefined Value =new Undefined();private Undefined(){}}
	}

/* System.Json (Silverlight)
	JsonValue
		JsonArray      List<JsonValue> | JsonValue[]
		JsonObject     List<KeyValuePair<string,JsonValue>>
		JsonPrimitive  

	JsonType (enum)
		String
		Number
		Object
		Array
		Boolean
		
*/


	public abstract class JsonValue : IEnumerable,IConvertible,IComparable,IFormattable {

		protected readonly object InternalValue;

		protected JsonValue(object value) { InternalValue = value; }

		public virtual int Count{get{throw new NotSupportedException();}}
		public virtual JsonValue this[int index] {get{throw new NotSupportedException();} set{throw new NotSupportedException();}}
		public virtual JsonValue this[string key]{get{throw new NotSupportedException();} set{throw new NotSupportedException();}}
		public virtual JsonType JsonType{get{throw new NotSupportedException();}}

		public virtual bool ContainsKey(string key){ throw new InvalidOperationException();}
		public virtual IEnumerator GetEnumerator() { throw new NotImplementedException(); }

		public TypeCode GetTypeCode() { return Type.GetTypeCode(InternalValue.GetType()); }
		public bool     ToBoolean (IFormatProvider provider) { return Convert.ToBoolean (InternalValue); }
		public char     ToChar    (IFormatProvider provider) { return Convert.ToChar    (InternalValue); }
		public sbyte    ToSByte   (IFormatProvider provider) { return Convert.ToSByte   (InternalValue); }
		public byte     ToByte    (IFormatProvider provider) { return Convert.ToByte    (InternalValue); }
		public short    ToInt16   (IFormatProvider provider) { return Convert.ToInt16   (InternalValue); }
		public ushort   ToUInt16  (IFormatProvider provider) { return Convert.ToUInt16  (InternalValue); }
		public int      ToInt32   (IFormatProvider provider) { return Convert.ToInt32   (InternalValue); }
		public uint     ToUInt32  (IFormatProvider provider) { return Convert.ToUInt32  (InternalValue); }
		public long     ToInt64   (IFormatProvider provider) { return Convert.ToInt64   (InternalValue); }
		public ulong    ToUInt64  (IFormatProvider provider) { return Convert.ToUInt64  (InternalValue); }
		public float    ToSingle  (IFormatProvider provider) { return Convert.ToSingle  (InternalValue); }
		public double   ToDouble  (IFormatProvider provider) { return Convert.ToDouble  (InternalValue); }
		public decimal  ToDecimal (IFormatProvider provider) { return Convert.ToDecimal (InternalValue); }
		public DateTime ToDateTime(IFormatProvider provider) { return Convert.ToDateTime(InternalValue); }
		public string   ToString  (IFormatProvider provider) { return Convert.ToString  (InternalValue); }
		public object   ToType    (Type conversionType, IFormatProvider provider) { return ((IConvertible) InternalValue).ToType(conversionType, provider); }

		public int CompareTo(object obj) { return ((IComparable) InternalValue).CompareTo(obj); }

		public override string ToString() {return string.Format(CultureInfo.InvariantCulture,"{0}",InternalValue);}
		public string ToString(string format, IFormatProvider formatProvider) { return ((IFormattable) InternalValue).ToString(format, formatProvider); }

//		public static implicit operator JsonValue (DateTimeOffset value){return new JsonObject   (value);}
		public static implicit operator JsonValue (DateTime       value){return new JsonPrimitive(value);}
		public static implicit operator JsonValue (TimeSpan       value){return new JsonPrimitive(value);}
		public static implicit operator JsonValue (Guid           value){return new JsonPrimitive(value);}
		public static implicit operator JsonValue (Uri            value){return new JsonPrimitive(value);}
		public static implicit operator JsonValue (bool           value){return new JsonBool     (value);}
		public static implicit operator JsonValue (Char           value){return new JsonString   (value);}
		public static implicit operator JsonValue (String         value){return new JsonString   (value);}
		public static implicit operator JsonValue (Byte           value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (Double         value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (Int16          value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (Int32          value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (Int64          value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (SByte          value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (Single         value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (UInt16         value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (UInt32         value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (UInt64         value){return new JsonNumber   (value);}
		public static implicit operator JsonValue (Decimal        value){return new JsonNumber   (value);}


//		public static implicit operator DateTimeOffset(JsonValue value) { return Convert.To (value.InternalValue); }
		public static implicit operator DateTime      (JsonValue value) { return Convert.ToDateTime (value.InternalValue); }
//		public static implicit operator TimeSpan      (JsonValue value) { return Convert.To (value.InternalValue); }
//		public static implicit operator Guid          (JsonValue value) { return Convert.To (value.InternalValue); }
//		public static implicit operator Uri           (JsonValue value) { return Convert.To (value.InternalValue); }
		public static implicit operator String (JsonValue value) { return Convert.ToString (value.InternalValue); }
		public static implicit operator Char   (JsonValue value) { return Convert.ToChar   (value.InternalValue); }
		public static implicit operator bool   (JsonValue value) { return Convert.ToBoolean(value.InternalValue); }
		public static implicit operator Byte   (JsonValue value) { return Convert.ToByte   (value.InternalValue); }
		public static implicit operator Int16  (JsonValue value) { return Convert.ToInt16  (value.InternalValue); }
		public static implicit operator Int32  (JsonValue value) { return Convert.ToInt32  (value.InternalValue); }
		public static implicit operator Int64  (JsonValue value) { return Convert.ToInt64  (value.InternalValue); }
		public static implicit operator SByte  (JsonValue value) { return Convert.ToSByte  (value.InternalValue); }
		public static implicit operator UInt16 (JsonValue value) { return Convert.ToUInt16 (value.InternalValue); }
		public static implicit operator UInt32 (JsonValue value) { return Convert.ToUInt32 (value.InternalValue); }
		public static implicit operator UInt64 (JsonValue value) { return Convert.ToUInt64 (value.InternalValue); }
		public static implicit operator Single (JsonValue value) { return Convert.ToSingle (value.InternalValue); }
		public static implicit operator Double (JsonValue value) { return Convert.ToDouble (value.InternalValue); }
		public static implicit operator Decimal(JsonValue value) { return Convert.ToDecimal(value.InternalValue); }

//		public static implicit operator DateTimeOffset?(JsonValue value) { return (DateTimeOffset?)value.InternalValue??Convert.To???     (value.InternalValue); }
		public static implicit operator DateTime?      (JsonValue value) { return (DateTime?      )value.InternalValue??Convert.ToDateTime(value.InternalValue); }
//		public static implicit operator TimeSpan?      (JsonValue value) { return (TimeSpan?      )value.InternalValue??Convert.To???     (value.InternalValue); }
//		public static implicit operator Guid?          (JsonValue value) { return (Guid?          )value.InternalValue??Convert.To???     (value.InternalValue); }
		public static implicit operator Char?   (JsonValue value) { return (Char?   )value.InternalValue??Convert.ToChar   (value.InternalValue); }
		public static implicit operator Byte?   (JsonValue value) { return (Byte?   )value.InternalValue??Convert.ToByte   (value.InternalValue); }
		public static implicit operator Int16?  (JsonValue value) { return (Int16?  )value.InternalValue??Convert.ToInt16  (value.InternalValue); }
		public static implicit operator Int32?  (JsonValue value) { return (Int32?  )value.InternalValue??Convert.ToInt32  (value.InternalValue); }
		public static implicit operator Int64?  (JsonValue value) { return (Int64?  )value.InternalValue??Convert.ToInt64  (value.InternalValue); }
		public static implicit operator SByte?  (JsonValue value) { return (SByte?  )value.InternalValue??Convert.ToSByte  (value.InternalValue); }
		public static implicit operator UInt16? (JsonValue value) { return (UInt16? )value.InternalValue??Convert.ToUInt16 (value.InternalValue); }
		public static implicit operator UInt32? (JsonValue value) { return (UInt32? )value.InternalValue??Convert.ToUInt32 (value.InternalValue); }
		public static implicit operator UInt64? (JsonValue value) { return (UInt64? )value.InternalValue??Convert.ToUInt64 (value.InternalValue); }
		public static implicit operator Single? (JsonValue value) { return (Single? )value.InternalValue??Convert.ToSingle (value.InternalValue); }
		public static implicit operator Double? (JsonValue value) { return (Double? )value.InternalValue??Convert.ToDouble (value.InternalValue); }
		public static implicit operator Decimal?(JsonValue value) { return (Decimal?)value.InternalValue??Convert.ToDecimal(value.InternalValue); }

	}

	public class JsonObject : JsonValue,
		IDictionary<string, JsonValue>,
		ICollection<KeyValuePair<string, JsonValue>>,
		IEnumerable<KeyValuePair<string, JsonValue>>,
		IEnumerable {

		[Obsolete("Not implemented",true)]
		public JsonObject(DateTimeOffset dto):base(dto) {}

		public JsonObject(IEnumerable<KeyValuePair<string, JsonValue>> items)
			:base(new List<KeyValuePair<string, JsonValue>>(items)){}

		public JsonObject(params KeyValuePair<string, JsonValue>[] items)
			:base(new List<KeyValuePair<string, JsonValue>>(items)) {}

		private new List<KeyValuePair<string, JsonValue>> InternalValue { get { return (List<KeyValuePair<string, JsonValue>>) base.InternalValue; } }
		
		public override JsonType JsonType{get{return JsonType.Object;}}

		private void SetValueByKey(string key, JsonValue value) {
			var index = IndexOf(key);
			if(index<0) InternalValue.Add(new KeyValuePair<string, JsonValue>(key, value));
			else InternalValue[index]=new KeyValuePair<string, JsonValue>(key, value);
		}

		private JsonValue GetValueByKey(string key) {
			var index = IndexOf(key);
			if(index<0) return null;
			return InternalValue[index].Value;
		}
		private int IndexOf(string key) { return InternalValue.FindIndex(pair => pair.Key == key); }

		public override sealed int Count { get { return InternalValue.Count; } }
		public override sealed JsonValue this[string key] { get { return GetValueByKey(key); } set { SetValueByKey(key, value); } }
		public ICollection<string> Keys { get { return InternalValue.Select(pair => pair.Key).ToArray(); } }
		public ICollection<JsonValue> Values { get { return InternalValue.Select(pair => pair.Value).ToArray(); } }

		public void Add(KeyValuePair<string, JsonValue> item){InternalValue.Add(item);}
		public void Add(string key,JsonValue value){InternalValue.Add(new KeyValuePair<string, JsonValue>(key, value));}
		public void AddRange(IEnumerable<KeyValuePair<string, JsonValue>> items) {InternalValue.AddRange(items);}
		public void AddRange(params KeyValuePair<string, JsonValue>[] items){InternalValue.AddRange(items);}
		public void Clear(){InternalValue.Clear();}
		public override bool ContainsKey(string key) { return IndexOf(key) >= 0; }
		public void CopyTo(KeyValuePair<string, JsonValue>[] array,int arrayIndex){InternalValue.CopyTo(array,arrayIndex);}
		public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator() { return InternalValue.GetEnumerator(); }
		public bool Remove(string key) { var index = IndexOf(key); if(index<0)return false; InternalValue.RemoveAt(index); return true;}
		public bool TryGetValue(string key, out JsonValue value) {
			var index = IndexOf(key);
			if(index<0){value=null;return false;}
			else {value=InternalValue[index].Value;return true;}
		}

		bool ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item) { return IndexOf(item.Key) >= 0; } //???
		bool ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item) { return Remove(item.Key); } //???
		bool ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly { get { return false; } }

	}

	public class JsonArray : JsonValue, IList<JsonValue>/*, ICollection<JsonValue>, IEnumerable<JsonValue>, IEnumerable*/ {

		public JsonArray(IEnumerable<JsonValue> items)
			:base(new List<JsonValue>(items)) {}

		public JsonArray(params JsonValue[] items)
			:base(new List<JsonValue>(items)) {}

		protected new List<JsonValue> InternalValue{get{return (List<JsonValue>)base.InternalValue;} } 

		public override JsonType JsonType{get{return JsonType.Array;}}

		public void Add(JsonValue value) { InternalValue.Add(value); }
		public void AddRange(IEnumerable<JsonValue> items){InternalValue.AddRange(items);}
		public void AddRange(params JsonValue[] items){InternalValue.AddRange(items);}
		public void Clear(){InternalValue.Clear();}
		public bool Contains(JsonValue item) { return InternalValue.Contains(item); }
		public void CopyTo(JsonValue[] array,int arrayIndex){InternalValue.CopyTo(array,arrayIndex);}
		public int IndexOf(JsonValue item) { return InternalValue.IndexOf(item); }
		public void Insert(int index,JsonValue item){InternalValue.Insert(index, item);}
		public bool Remove(JsonValue item) { return InternalValue.Remove(item); }
		public void RemoveAt(int index){InternalValue.RemoveAt(index);}
		
		public bool IsReadOnly { get { return false; } }
		public override JsonValue this[int index] { get { return InternalValue[index]; } set { InternalValue[index]=value; } }

		IEnumerator<JsonValue> IEnumerable<JsonValue>.GetEnumerator() { return InternalValue.GetEnumerator(); }

	}

	public class JsonPrimitive : JsonValue {

		protected JsonPrimitive(object value):base(value) { }

		public JsonPrimitive(DateTime value):base(value) { }
		public JsonPrimitive(Guid     value):base(value) { }
		public JsonPrimitive(TimeSpan value):base(value) { }
		public JsonPrimitive(Uri      value):base(value) { }


		public override bool Equals(object obj) {
			if(Equals(obj,null)) return Equals(InternalValue,null);
			if(Equals(InternalValue,null)) return false;
			return Equals(obj.ToString(), InternalValue.ToString());
		}
	}

	public class JsonNull:JsonPrimitive {
		public static readonly JsonNull Value = new JsonNull();
		private JsonNull():base(Value){}
		public override JsonType JsonType{get{return JsonType.Null;}}
		public override bool Equals(object obj) { return Equals(obj, null) || Equals(obj, Value); }
		public override int GetHashCode() { return 0; }
	}
	
	public class JsonUndefined:JsonPrimitive {
		public static readonly JsonUndefined Value = new JsonUndefined();
		private JsonUndefined():base(Value){}
		public override JsonType JsonType{get{return JsonType.None;}}
	}

	public class JsonString:JsonPrimitive {

		public JsonString(string value):base(value) {}
		public JsonString(char   value):base(value) {}

		public override JsonType JsonType{get{return JsonType.String;}}

		public override bool Equals(object obj) {
			if(Equals(obj,null)) return Equals(InternalValue,null);
			if(Equals(InternalValue,null)) return false;
			return Equals(obj.ToString(), InternalValue.ToString());
		}

		public override int GetHashCode() {
			return Equals(InternalValue, null) ? 0 : InternalValue.ToString().GetHashCode();
		}
	}

	public class JsonBool:JsonPrimitive {

		public JsonBool(bool value):base(value) { }

		public override JsonType JsonType{get{return JsonType.Boolean;}}

		public override bool Equals(object obj) {
			if(Equals(obj,null)) return Equals(InternalValue,null);
			if(Equals(InternalValue,null)) return false;
			return Equals(obj.ToString(), InternalValue.ToString());
		}

		public override int GetHashCode() { return Equals(InternalValue, null) ? 0 : InternalValue.GetHashCode(); }
	}

	public class JsonNumber:JsonPrimitive {

		/*
			Byte   
			Int16  
			Int32  
			Int64			IComparable, IFormattable, IConvertible, IComparable<long>, IEquatable<long>
			SByte  
			UInt16 
			UInt32 
			UInt64 
			Single 
			Double			IComparable, IFormattable, 	IConvertible, IComparable<double>, IEquatable<double>
			Decimal			IFormattable, IComparable, 	IConvertible, IDeserializationCallback, IComparable<decimal>, IEquatable<decimal>
		*/

		public JsonNumber(Byte     value):base(value){ }
		public JsonNumber(Int16    value):base(value){ }
		public JsonNumber(Int32    value):base(value){ }
		public JsonNumber(Int64    value):base(value){ }
		public JsonNumber(SByte    value):base(value){ }
		public JsonNumber(UInt16   value):base(value){ }
		public JsonNumber(UInt32   value):base(value){ }
		public JsonNumber(UInt64   value):base(value){ }
		public JsonNumber(Single   value):base(value){ }
		public JsonNumber(Double   value):base(value){ }
		public JsonNumber(Decimal  value):base(value){ }
		public JsonNumber(Byte?    value):base(value){ }
		public JsonNumber(Int16?   value):base(value){ }
		public JsonNumber(Int32?   value):base(value){ }
		public JsonNumber(Int64?   value):base(value){ }
		public JsonNumber(SByte?   value):base(value){ }
		public JsonNumber(UInt16?  value):base(value){ }
		public JsonNumber(UInt32?  value):base(value){ }
		public JsonNumber(UInt64?  value):base(value){ }
		public JsonNumber(Single?  value):base(value){ }
		public JsonNumber(Double?  value):base(value){ }
		public JsonNumber(Decimal? value):base(value){ }

		public Type Type{get { return InternalValue.GetType(); }}

		public override JsonType JsonType{get{return JsonType.Numeric;}}

		public override string ToString() { return ToString(InternalValue); }

		private static string ToString(object value) {
			// 1.0 -> 1
			var s=string.Format(CultureInfo.InvariantCulture,"{0}",value);
			if (s.EndsWith(".0")) return s.Substring(0, s.Length - 2);
			return s;
		}

		public override bool Equals(object obj) {
			if(Equals(obj,null)) return Equals(InternalValue,null);
			if(Equals(InternalValue,null)) return false;
			return Equals(ToString(obj), ToString());
		}

		//protected bool Equals(JsonNumber other) { throw new NotImplementedException(); }
		public override int GetHashCode() { return Equals(InternalValue, null) ? 0 : InternalValue.ToString().GetHashCode(); }
	}									

}

//parse and show entire json in key-value pair
//    Hashtable HTList = (Hashtable)JSON.JsonDecode("completejsonstring");
//        public void GetData(Hashtable HT)
//        {           
//            IDictionaryEnumerator ienum = HT.GetEnumerator();
//            while (ienum.MoveNext())
//            {
//                if (ienum.Value is ArrayList)
//                {
//                    ArrayList arnew = (ArrayList)ienum.Value;
//                    foreach (object obj in arnew)                    
//                    {
//                        Hashtable hstemp = (Hashtable)obj;
//                        GetData(hstemp);
//                    }
//                }
//                else
//                {
//                    Console.WriteLine(ienum.Key + "=" + ienum.Value);
//                }
//            }
//        }
