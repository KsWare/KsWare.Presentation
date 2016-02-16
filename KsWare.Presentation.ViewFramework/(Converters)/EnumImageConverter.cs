using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class EnumImageConverter:IValueConverter {

		public Type EnumType { get; set; }
		public ResourceDictionary Resources { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string stringKey;
			object enumKey=null;
			bool enumKeySpecified=false;
			if (value is string) {
				stringKey = (string) value;
				if (Enum.IsDefined(EnumType, stringKey)) {
					enumKeySpecified = true;
					enumKey = Enum.Parse(EnumType, stringKey);
				}
			}else if (value.GetType().IsEnum) {
				stringKey = Enum.GetName(value.GetType(), value);
				enumKey = value;
				enumKeySpecified = true;
			} else {
				//throw new NotSupportedException();
				return null;
			}

			Image res;
			if (enumKeySpecified && Resources.Contains(enumKey)) {
				res = (Image) Resources[enumKey];
				res=new Image{Source = res.Source};
			} else if (Resources.Contains(stringKey)) {
				res = (Image) Resources[stringKey];
				res=new Image{Source = res.Source};
			} else {
				return null;
			}
			
			return res;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
