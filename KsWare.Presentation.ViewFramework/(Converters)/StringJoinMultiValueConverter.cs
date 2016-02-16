//CREATED: 2014-06-03

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class StringJoinMultiValueConverter:IMultiValueConverter {

		public static readonly StringJoinMultiValueConverter Default=new StringJoinMultiValueConverter();
		
		public Dictionary<int,string> StringFormat { get; set; }

		public StringJoinMultiValueConverter() {
			StringFormat=new Dictionary<int, string>();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == null || targetType==typeof(Object)) targetType = typeof (string);
			if(targetType != typeof(string)) throw new ArgumentOutOfRangeException("targetType", "StringJoinMultiValueConverter does not support conversion to "+targetType.FullName+".");
			if(values==null) return null;
			var separator = parameter == null ? "" : parameter.ToString();

			var sb = new StringBuilder();
			for (int i = 0; i < values.Length; i++) {
				if(i>0) sb.Append(separator);
				string format;
				if (StringFormat != null && StringFormat.TryGetValue(i, out format)) format = "{0:" + format + "}";
				else format = "{0}";
				sb.AppendFormat(culture, format, values[i]);
			}
			return sb.ToString();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

	}

}