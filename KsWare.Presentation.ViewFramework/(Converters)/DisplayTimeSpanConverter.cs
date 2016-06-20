using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class DisplayTimeSpanConverter:IValueConverter {

		public static readonly DisplayTimeSpanConverter HHHmmss=new DisplayTimeSpanConverter{StringFormat="hhh:mm:ss"};

		public DisplayTimeSpanConverter() {
			
		}

		public string StringFormat { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) return null;
			var timespan = (TimeSpan) value;
			if (StringFormat == null) return timespan.ToString();
			bool custom=false;
			new[] { "ddd", "hhh","mmm", "sss" }.ForEach(s1 => { if (StringFormat.Contains(s1)) custom = true; });
			if(!custom) return timespan.ToString(StringFormat);

			var s = StringFormat;
			s = s.Replace("ddd", ((int)timespan.TotalDays   ).ToString(culture));
			s = s.Replace("dd" , ((int)timespan.Days        ).ToString("D2",culture));
			s = s.Replace("d"  , ((int)timespan.Days        ).ToString("D1",culture));
			s = s.Replace("hhh", ((int)timespan.Hours       ).ToString(     culture));
			s = s.Replace("hh" , ((int)timespan.TotalHours  ).ToString("D2",culture));
			s = s.Replace("h"  , ((int)timespan.Hours       ).ToString("D1",culture));
			s = s.Replace("mmm", ((int)timespan.TotalMinutes).ToString(     culture));
			s = s.Replace("mm" , ((int)timespan.Minutes     ).ToString("D2",culture));
			s = s.Replace("m"  , ((int)timespan.Minutes     ).ToString("D1",culture));
			s = s.Replace("sss", ((int)timespan.TotalSeconds).ToString(     culture));
			s = s.Replace("ss" , ((int)timespan.Seconds     ).ToString("D2",culture));
			s = s.Replace("s"  , ((int)timespan.Seconds     ).ToString("D1",culture));
			return s;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
	}
}