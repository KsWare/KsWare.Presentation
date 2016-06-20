using System;
using System.Globalization;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class HexConverter:IValueConverter {

		public static readonly HexConverter Default=new HexConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			string format = null;
			if (parameter != null) {
				format = parameter.ToString();
				if(format.StartsWith("{0")){/*ok*/} 
				else format="{0:" + format +"}";
			} else {
				switch (Type.GetTypeCode(value.GetType())) {
					case TypeCode.SByte : format="{0:X2}" ;break;
					case TypeCode.Byte  : format="{0:X2}" ;break;
					case TypeCode.Int16 : format="{0:X4}" ;break;
					case TypeCode.UInt16: format="{0:X4}" ;break;
					case TypeCode.Int32 : format="{0:X8}" ;break;
					case TypeCode.UInt32: format="{0:X8}" ;break;
					case TypeCode.Int64 : format="{0:X16}";break;
					case TypeCode.UInt64: format="{0:X16}";break;
				}
			}

			switch (Type.GetTypeCode(targetType)) {
				case TypeCode.String: {
					switch (Type.GetTypeCode(value.GetType())) {
						case TypeCode.SByte : return string.Format(culture,format,(SByte )value);
						case TypeCode.Byte  : return string.Format(culture,format,(Byte  )value);
						case TypeCode.Int16 : return string.Format(culture,format,(Int16 )value);
						case TypeCode.UInt16: return string.Format(culture,format,(UInt16)value);
						case TypeCode.Int32 : return string.Format(culture,format,(Int32 )value);
						case TypeCode.UInt32: return string.Format(culture,format,(UInt32)value);
						case TypeCode.Int64 : return string.Format(culture,format,(Int64 )value);
						case TypeCode.UInt64: return string.Format(culture,format,(UInt64)value);
						default             : throw new NotSupportedException();
					}
				}
				case TypeCode.SByte : return SByte .Parse(value.ToString(),NumberStyles.HexNumber,culture);
				case TypeCode.Byte  : return Byte  .Parse(value.ToString(),NumberStyles.HexNumber,culture);
				case TypeCode.Int16 : return Int16 .Parse(value.ToString(),NumberStyles.HexNumber,culture);
				case TypeCode.UInt16: return UInt16.Parse(value.ToString(),NumberStyles.HexNumber,culture);
				case TypeCode.Int32 : return Int32 .Parse(value.ToString(),NumberStyles.HexNumber,culture);
				case TypeCode.UInt32: return UInt32.Parse(value.ToString(),NumberStyles.HexNumber,culture);
				case TypeCode.Int64 : return Int64 .Parse(value.ToString(),NumberStyles.HexNumber,culture);
				case TypeCode.UInt64: return UInt64.Parse(value.ToString(),NumberStyles.HexNumber,culture);
				default             : throw new NotSupportedException();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return Convert(value, targetType, parameter, culture); }

	}

}