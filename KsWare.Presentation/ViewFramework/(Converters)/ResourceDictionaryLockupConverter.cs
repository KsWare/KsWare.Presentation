using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace KsWare.Presentation.ViewFramework {

	public class ResourceDictionaryLockupConverter:IValueConverter {

		public Type EnumType { get; set; }
		public ResourceDictionary Resources { get; set; }

		public ResourceDictionaryLockupConverter() {
			Resources=new ResourceDictionary();
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string stringKey;
			object enumKey=null;
			bool enumKeySpecified=false;

			if (value == null) {
				return GetDefaultValue(targetType);
			}else if (value is string) {
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
				return GetDefaultValue(targetType);
			}


			object res;
			if (enumKeySpecified && Resources.Contains(enumKey)) {
				res = Resources[enumKey];
			} else if (Resources.Contains(stringKey)) {
				res = Resources[stringKey];
			} else {
				return GetDefaultValue(targetType);
			}

			return ChangeType(res,targetType);
		}

		private object GetDefaultValue(Type targetType) {
			var res = Resources[EnumType];
			return ChangeType(res, targetType);
		}

		private object ChangeType(object value, Type targetType) {
			if (value is Image) {
				if      (typeof (ImageSource).IsAssignableFrom(targetType)) { value = ((Image) value).Source; }
				else if (typeof (Image      ).IsAssignableFrom(targetType)) { value = new Image {Source = ((Image) value).Source}; }
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
