	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class SpecialBooleanConverter:IValueConverter {
		// REPLACED by BooleanComparingConverter (KsWare.Presentation.Converters)

		private static readonly DoubleConverter DoubleConverter=new DoubleConverter();

		/// <summary>
		/// Gets the default SpecialBooleanConverter.Expects operator in parameter. 
		/// [eq, ne, gt, lt, ge, le, is null, not null]
		/// </summary>
		public static readonly SpecialBooleanConverter Default=new SpecialBooleanConverter(CompareOperation.InParameter);

		public static readonly SpecialBooleanConverter Equal=new SpecialBooleanConverter(CompareOperation.Equal);
		public static readonly SpecialBooleanConverter NotEqual=new SpecialBooleanConverter(CompareOperation.Equal);

		public static readonly SpecialBooleanConverter LessThen=new SpecialBooleanConverter(CompareOperation.LessThen);
		public static readonly SpecialBooleanConverter GreaterThean=new SpecialBooleanConverter(CompareOperation.GreaterThean);
		public static readonly SpecialBooleanConverter LessThenOrEqual=new SpecialBooleanConverter(CompareOperation.LessThenOrEqual);
		public static readonly SpecialBooleanConverter GreaterTheanOrEqual=new SpecialBooleanConverter(CompareOperation.GreaterTheanOrEqual);
		public static readonly SpecialBooleanConverter IsNull=new SpecialBooleanConverter(CompareOperation.IsNull);
		public static readonly SpecialBooleanConverter IsNotNull=new SpecialBooleanConverter(CompareOperation.IsNotNull);

//		public static readonly SpecialBooleanConverter StartsWith=new SpecialBooleanConverter(CompareOperation.StartsWith);
//		public static readonly SpecialBooleanConverter EndsWith=new SpecialBooleanConverter(CompareOperation.EndsWith);
//		public static readonly SpecialBooleanConverter Contains=new SpecialBooleanConverter(CompareOperation.Contains);
//		public static readonly SpecialBooleanConverter IsNullOrEmpty=new SpecialBooleanConverter(CompareOperation.IsNullOrEmpty);
//		public static readonly SpecialBooleanConverter IsNullOrWhitespace=new SpecialBooleanConverter(CompareOperation.IsNullOrWhitespace);

		public enum CompareOperation {
			None,
			InParameter,

			Equal,		// eq
			NotEqual,	// ne

			LessThen,	//lt
			GreaterThean,	// gt
			LessThenOrEqual,	// le
			GreaterTheanOrEqual, // ge
			IsNull,		// eq null
			IsNotNull,	// ne null

//			StartsWith,				// ssw 
//			EndsWith,				// sew
//			Contains,				// scs
//			IsNullOrEmpty,			// sne
//			IsNullOrWhitespace,		// snw
		}

		private CompareOperation Operation { get; set; }

		public SpecialBooleanConverter() { Operation=CompareOperation.Equal;}

		public SpecialBooleanConverter(CompareOperation operation) { Operation = operation; }

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
			CompareOperation op;
			object cv;
			if (Operation == CompareOperation.InParameter) {
				var p=((string) parameter).Split(new char[] {' '}, 2);
				var ops = p[0];
				if (p[1].ToLowerInvariant() == "null" || p[1].ToLowerInvariant() == "{null}") ops = p[0]+" null"; //TODO nullable numeric?
				cv = p[1];
				switch (ops.ToLowerInvariant()) {
					case "eq"       : op=CompareOperation.Equal; break;
					case "ne"       : op=CompareOperation.NotEqual; break;
					case "not null" : op=CompareOperation.IsNotNull; break;
					case "ne null"  : op=CompareOperation.IsNotNull; break;
					case "is null"  : op=CompareOperation.IsNull; break;
					case "eq null"  : op=CompareOperation.IsNull; break;
					default         : op=CompareOperation.None; break;
				}
			}
			else {
				op = Operation;
				cv = parameter;
			}

			switch (op) {
				case CompareOperation.IsNotNull : return value!=null ;
				case CompareOperation.IsNull    : return value==null;
			}
			if (value != null) {
				switch (op) {
					case CompareOperation.Equal     : return value.Equals(System.Convert.ChangeType(cv,value.GetType()));
					case CompareOperation.NotEqual  : return !value.Equals(System.Convert.ChangeType(cv,value.GetType()));
				}				
			}

			return false;
		}

		public bool CompareNumeric(object value, Type targetType, object parameter, CultureInfo culture) {
			CompareOperation op;
			object cv;
			if (Operation == CompareOperation.InParameter) {
				var p=((string) parameter).Split(new char[] {' '}, 2);
				var ops = p[0];
				if (p[1].ToLowerInvariant() == "null" || p[1].ToLowerInvariant() == "{null}") ops = p[0]+" null"; //TODO nullable numeric?
				cv = p[1];
				switch (ops.ToLowerInvariant()) {
					case "eq": op=CompareOperation.Equal; break;
					case "gt": op=CompareOperation.GreaterThean; break;
					case "lt": op=CompareOperation.LessThen; break;
					case "ge": op=CompareOperation.GreaterTheanOrEqual; break;
					case "le": op=CompareOperation.LessThenOrEqual; break;
					case "ne": op=CompareOperation.NotEqual; break;
					default  : op=CompareOperation.None; break;
				}
			}
			else {
				op = Operation;
				cv = parameter;
			}

			var v0 = System.Convert.ToDouble(value);
			var v1 = (double)DoubleConverter.ConvertFrom(cv);
			switch (op) {
				case CompareOperation.Equal              : return v0==v1;
				case CompareOperation.GreaterThean       : return v0>v1;
				case CompareOperation.LessThen           : return v0<v1;
				case CompareOperation.GreaterTheanOrEqual: return v0>=v1;
				case CompareOperation.LessThenOrEqual    : return v0<=v1;
				case CompareOperation.NotEqual           : return v0!=v1;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}