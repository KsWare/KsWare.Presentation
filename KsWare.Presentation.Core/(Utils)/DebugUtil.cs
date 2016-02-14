using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace KsWare.Presentation.Core {

	public static class DebugUtil {

		/// <summary> Formats the name of the type.
		/// </summary>
		/// <param name="o">The object or the type</param>
		/// <returns>A readable type name</returns>
		/// <remarks>
		/// Generic types names like "List'1" will be readable "List&lt;String&gt;"
		/// </remarks>
		public static string FormatTypeName(object o) {return FormatTypeName(o, false, true );}

		public static string FormatTypeFullName(object o) {return FormatTypeName(o, true,true);}
		public static string FormatTypeFullName(object o, bool encloseInCurlyBrackets) {return FormatTypeName(o, true,encloseInCurlyBrackets);}

		public static string FormatTypeName(object o, bool encloseInCurlyBrackets) { return FormatTypeName(o, encloseInCurlyBrackets, false); }

		public static string FormatTypeName(object o,bool fullName, bool encloseInCurlyBrackets) {
			string n;
			if (o == null) {
				n="Null";
			} else {
				Type t = o is Type ? (Type)o : o.GetType();
				if (!t.IsGenericType) {
					n=t.Name;
				} else {
					n=t.Name.Split('`')[0];
					n += "<";
					var a=t.GetGenericArguments();
					foreach (var at in a) {
						if(n[n.Length-1]!='<') n += ",";
						n += FormatTypeName(at,false,false); //?? always short name in generic parameters?
					}
					n += ">";					
				}
				if (fullName) {
					n = t.Namespace + "." + n; //TODO support also nested types
				}
			}
			if (encloseInCurlyBrackets) n = "{" + n + "}";
			return n;
		}

		public static object Try(Func<object> getter) {
			try {return getter();} catch (Exception) {return null;}
		}

		public static TResult Try<TResult>(Func<TResult> getter) {
			try {return getter();} catch (Exception) {return default(TResult);}
		}
		public static TResult Try<TResult,TP0>(Func<TP0,TResult> getter,TP0 p0) {
			try {return getter(p0);} catch (Exception) {return default(TResult);}
		}
		public static TResult Try<TResult>(Func<TResult> getter, TResult defaultResult) {
			try {return getter();} catch (Exception) {return defaultResult;}
		}

		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		public static void Break() {if(Debugger.IsAttached) Debugger.Break();}

		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		public static void Break(string message) {
			if (!Debugger.IsAttached) {
				Debug.WriteLine("=== Debugger Breakpoint [skipped] ===\n"+message);
				return;
			}
			Debug.WriteLine("=== Debugger Breakpoint ===\n"+message);
			var result = MessageBox.Show(message, "Debugger Breakpoint", MessageBoxButton.OKCancel, MessageBoxImage.Information);
			if(result!=MessageBoxResult.OK)return;
			Debugger.Break();
		}

		public static Type FindStackAncestor2(Type type) {
			var stackTrace = new StackTrace(1, false);
			foreach (var frame in stackTrace.GetFrames()) {
				var methodBase = frame.GetMethod();
				var reflectedType = methodBase.ReflectedType;
				if (type.IsAssignableFrom(reflectedType)) return reflectedType;
			}
			return null;
		}

		public static StackFrame FindStackFrame(Type type,int skipStackFrames) {
			var stackTrace = new StackTrace(skipStackFrames+1, true);
			foreach (var frame in stackTrace.GetFrames()) {
				var methodBase = frame.GetMethod();
				var reflectedType = methodBase.ReflectedType;
				if (type.IsAssignableFrom(reflectedType)) return frame;
			}
			return null;
		}

		/// <summary> Formats a parameter if the condition is true.
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="parameterValue">The function to get the parameter value</param>
		/// <returns>System.String.</returns>
		/// <remarks>If the function call fails for any reason "{unknown}" is returned </remarks>
		public static string P(bool condition, string parameterName, Func<object> parameterValue) {
			return condition ? P(parameterName, parameterValue) : "";
		}

		/// <summary> Formats a parameter if the condition is true.
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="parameterValue">The parameter value.</param>
		/// <returns>System.String.</returns>
		public static string P(bool condition, string parameterName, object parameterValue) {
			return condition ? P(parameterName, parameterValue) : "";
		}

		/// <summary> Formats a parameter.
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="parameterValue">The function to get the parameter value</param>
		/// <returns>System.String.</returns>
		/// <remarks>If the function call fails for any reason "{unknown}" is returned </remarks>
		public static string P(string parameterName, Func<object> parameterValue) {
			return string.Format("\n\t{0}: {1}", parameterName, Try(parameterValue,"{unknown}"));
		}

		/// <summary> Formats a parameter.
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="parameterValue">The parameter value.</param>
		/// <returns>System.String.</returns>
		public static string P(string parameterName, object parameterValue) {
			return string.Format("\n\t{0}: {1}", parameterName, parameterValue);
		}

		public static string Indent(string s) {return Indent(s,1);}

		public static string Indent(string s,int count) {
			var lines = s.Split(new []{"\r\n","\r","\n"},StringSplitOptions.None);
			if(lines.Length<=1) return s; //nothing to do
			bool needIndent = false;
			for (int i = 1; i < lines.Length; i++) {
				if(!lines[i].StartsWith("\t",StringComparison.Ordinal)){needIndent=true;break;}
			}

			if(needIndent) {
				for (int i = 1; i < lines.Length; i++) {
					lines[i] = "\t" + lines[i];
				}				
			}

			return string.Join("\n", lines);
		}

		public static string Indent2(string s) {
			var lines = s.Split(new []{"\r\n","\r","\n"},StringSplitOptions.None);
			if(lines.Length<=1) return s; //nothing to do
			bool needIndent = false;
			for (int i = 1; i < lines.Length; i++) {
				if(!lines[i].StartsWith("\t",StringComparison.Ordinal)){needIndent=true;break;}
			}

			if(needIndent) {
				for (int i = 1; i < lines.Length; i++) {
					lines[i] = "\t" + lines[i];
				}				
			}

			return string.Join("\n", lines);
		}


		public static void Dump(object obj) {
			if(!DumpPropertyValue(obj))return;
			Dump(obj, 1);
		}

		public static void Dump(object obj,int indent) {
			var ind = new string(' ', indent*4);
			var tObj = obj.GetType();
			if (obj is IEnumerable) {
				int i = 0;
				foreach (var item in (IEnumerable)obj) {
					DumpProperty(i++,item,indent);
					if (i >= 4) {
						Debug.Write(ind+string.Format("..."));
						break;
					}
				}
			} else {
				var propertyInfos = tObj.GetProperties(BindingFlags.Instance | BindingFlags.Public);
				foreach (var propertyInfo in propertyInfos) {
					DumpProperty(obj,propertyInfo,indent);
				}
			}
			
		}

		private static void DumpProperty(object obj, PropertyInfo propertyInfo,int indent) {
			var ind = new string(' ', indent*4);
			if(propertyInfo.GetIndexParameters().Length>0){
				Debug.Write(ind+string.Format("{0}[...] {1} ",propertyInfo.Name,FormatTypeName(propertyInfo.PropertyType,true)));
			}else{
				Debug.Write(ind+string.Format("{0} {1} ",propertyInfo.Name,FormatTypeName(propertyInfo.PropertyType,true)));			
				var value = propertyInfo.GetValue(obj, null);
				var ext=DumpPropertyValue(value);
				if(!ext)return;
				Dump(value,indent+1);
			}
		}
		
		private static void DumpProperty(int index, object item, int indent) {
			var ind = new string(' ', indent*4);
			Debug.Write(ind+string.Format("[{0}] ",index));			
			var ext=DumpPropertyValue(item);
			if(!ext)return;
			Dump(item,indent+1);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private static bool DumpPropertyValue(object value) {
			if (value == null) {Debug.WriteLine( "{Null}");return false;}
			var tValue = value.GetType();
			if (value is string) {Debug.WriteLine("\"" + ((string) value) + "\"");return false;}
			if (value is char  ) {Debug.WriteLine("\'" + ((char) value) + "\'");return false;}
			if (value is bool  ) {Debug.WriteLine("" + ((bool) value) + "");return false;}
			if (value is Guid  ) {Debug.WriteLine("" + ((Guid) value).ToString() + "");return false;}
			if (tValue.IsEnum  ) {Debug.WriteLine(value.ToString());return false;}

			if     (value is IList) Debug.Write(string.Format("Count: {0} ",((IList)value).Count));
			else if(tValue.IsArray) Debug.Write(string.Format("Length: {0} ", ((Array)value).GetLength(0)));
			Debug.WriteLine(FormatTypeName(tValue, true));
			return true;
		}

		public static string FormatValue(byte[] byteArray) {
			if (byteArray == null) return "{Null}";
			var sb = new StringBuilder();
			foreach (var b in (IEnumerable<byte>)(byteArray)) { sb.AppendFormat("{0:X2} ",b); }
			return sb.ToString().TrimEnd();
		}


		public static string ToString(object obj) {
			if (obj == null) return "{Null}";
			switch (Type.GetTypeCode(obj.GetType())) {
				case TypeCode.String: return "\"" + obj + "\"";
				case TypeCode.Char:return "'" + obj + "'";
				default: return string.Format(CultureInfo.InvariantCulture,"{0}",obj);
			}
		}

		public static string FormatDelegate(Delegate method) {
//			if(method.Method.Name.StartsWith("<")) Debugger.Break(); //TODO
			return FormatTypeName(method.Target) + "." + method.Method.Name;
		}

		public static string FormatMethod(MethodBase method) {
			// {<>c__DisplayClassd}.<OnCanExecuteChanged>b__c
//			if(method.Name.StartsWith("<")) Debugger.Break();
//			if(method.Name.StartsWith(".ctor")) Debugger.Break();
//			if(method.Name.StartsWith(".cctor")) Debugger.Break();
			return FormatTypeName(method.ReflectedType) + "." + method.Name;
		}

	}
}
