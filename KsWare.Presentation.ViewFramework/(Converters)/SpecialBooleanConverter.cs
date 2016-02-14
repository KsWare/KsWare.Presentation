	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework
{

	public class SpecialBooleanConverter:IValueConverter
	{
		private static readonly DoubleConverter DoubleConverter=new DoubleConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value==null     ) return CompareObject (value, targetType, parameter, culture);
			if (IsNumeric(value)) return CompareNumeric(value, targetType, parameter, culture);
//			if (value is string ) return CompareString (value, targetType, parameter, culture);
//			if (value is bool   ) return CompareBool   (value, targetType, parameter, culture);
			return CompareObject(value, targetType, parameter, culture);;
		}

		private bool IsNumeric(object value) {
			if (value == null) return false;
			var t = value.GetType();
			return (t == typeof (Int16) || t == typeof (Int32) || t == typeof (Int64) || t == typeof (UInt16) || t == typeof (UInt32) || t == typeof (UInt64)
					|| t == typeof (byte) || t == typeof (SByte) || t == typeof (Decimal) || t == typeof (Single) || t == typeof (Double));
		}

		public bool CompareObject(object value, Type targetType, object parameter, CultureInfo culture) {
			var p=((string) parameter).Split(new char[] {' '}, 2);
			var op = p[0];
			if (p[1].ToLowerInvariant() == "null" || p[1].ToLowerInvariant() == "{null}") op = p[0]+" null";
			switch (op.ToLowerInvariant()) {
				case "not null" : return value!=null ;
				case "is null"  : return value==null;
//				case "eq"       : return null==v1;
//				case "nq"       : return null!=v1;
			}
			return false;
		}

		public bool CompareNumeric(object value, Type targetType, object parameter, CultureInfo culture) {
			var p=((string) parameter).Split(new char[] {' '}, 2);
			var op = p[0];
			if (p[1].ToLowerInvariant() == "null" || p[1].ToLowerInvariant() == "{null}") op = p[0]+" null";
			var v0 = System.Convert.ToDouble(value);
			var v1 = (double)DoubleConverter.ConvertFrom(p[1]);
			switch (op.ToLowerInvariant()) {
				case "eq": return v0==v1;
				case "gt": return v0>v1;
				case "lt": return v0<v1;
				case "ge": return v0>=v1;
				case "le": return v0<=v1;
				case "ne": return v0!=v1;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}