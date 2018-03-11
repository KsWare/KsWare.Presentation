using System;
using System.Globalization;
using JetBrains.Annotations;

namespace KsWare {

	public static class StringExtension {

		public static int CopareTo(this string strA, string strB, StringComparison comparisonType) {
			return String.Compare(strA, strB, System.StringComparison.CurrentCultureIgnoreCase);
		}

		public static string FormatInvariant(this string format, params object[] args) { return StringUtil.FormatInvariant(format, args); }
	}

	public static class StringUtil {

		[StringFormatMethod("format")]
		public static string FormatInvariant([NotNull]string format, params object[] args) {
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}

		[ContractAnnotation("s:null => null")]
		public static string TrimLeft(string s, int count) {
			if (s == null) return null;
			return s.Substring(Math.Min(count, s.Length));
		}

		[ContractAnnotation("s:null => null")]
		public static string TrimRight(string s, int count) {
			if (s == null) return null;
			return s.Substring(0, Math.Max(s.Length-count,0));
		}
	}
}
