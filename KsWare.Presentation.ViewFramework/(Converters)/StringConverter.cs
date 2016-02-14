using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class StringConverter:IValueConverter {

		public static StringConverter Default = new StringConverter();
		private static readonly System.ComponentModel.StringConverter internalConverter = new System.ComponentModel.StringConverter();


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			//return internalConverter.ConvertFrom(value); BUG??! cannot convert from System.Int32
			return string.Format("{0}", value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
