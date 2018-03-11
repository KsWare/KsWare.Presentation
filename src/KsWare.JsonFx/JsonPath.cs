using System;

namespace KsWare.JsonFx 
{

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
	public struct JsonPath 
	{
		private string m_StringValue;

		public JsonPath(string path) { m_StringValue = path; }

		public string StringValue { get { return m_StringValue; } set { m_StringValue = value; } }

		public static implicit operator string(JsonPath path) { return path.StringValue; }
		public static implicit operator JsonPath(string path) { return new JsonPath(path); }

		public static string[] SplitPath2(string path) {
			if(path==null) throw new ArgumentNullException("path");
			if (path.StartsWith("[")) {
				var p = path.IndexOf(']');
				if (p < 0) throw new ArgumentException("Invalid path!");
				if(path.Length - p - 2<=0) return new [] {path.Substring(1, p - 1)};
				var a=new [] {path.Substring(1, p - 1), path.Substring(p + 2, path.Length - p - 2)};
				return a;
			} else {
				var iDot = path.IndexOf('.');
				var iBracket = path.IndexOf('[');
				if (iDot < 0 && iBracket < 0) {
					return new[] {path};
				} if (iDot >= 0 && (iBracket<0 || iBracket>iDot)) { // first.second ==> "first","second"
					var a = new[] {path.Substring(0, iDot), path.Substring(iDot + 1, path.Length-iDot-1)};
					return a;
				} else if (iBracket >= 0 && (iDot<0 || iDot>iBracket)) { // first[second].third  ==> "first" | "[second].third"
					var a = new[] {path.Substring(0, iBracket), path.Substring(iBracket, path.Length - iBracket)};
					return a;
				} else {
					throw new NotImplementedException("{D9466916-DB4F-453A-84B5-3A0F23232B51}");
				}
			}
		}

		public static string[] SplitPath2Rev(string path) {
			if(path==null) throw new ArgumentNullException("path");
			if (path.EndsWith("]")) {
				var p = path.LastIndexOf('[');
				if (p < 0) throw new ArgumentException("Invalid path!");
				if(p==0) return new [] {path.Substring(1, path.Length-2)};
				var a=new [] {path.Substring(0, p), path.Substring(p+1, path.Length-p-2)};
				return a;
			} else {
				var iDot = path.LastIndexOf('.');
				var iBracket = path.LastIndexOf(']');
				if (iDot < 0 && iBracket < 0) {
					return new[] {path};
				} if (iDot >= 0 && (iBracket<0 || iBracket<iDot)) { // first.second ==> "first","second"
					var a = new[] {path.Substring(0, iDot), path.Substring(iDot + 1, path.Length-iDot-1)};
					return a;
				} else {
					throw new NotImplementedException("{D9466916-DB4F-453A-84B5-3A0F23232B51}");
				}
			}
		}
	}
}
