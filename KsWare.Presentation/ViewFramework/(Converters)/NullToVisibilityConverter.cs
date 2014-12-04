using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace OSIS.UI.KsWare.Presentation.ViewFramework
{
	public static class NullToVisibilityConverter
	{

		public static readonly IValueConverter NullCollapsedNotNullVisible = new BaseConverter(value => value==null ? Visibility.Collapsed : Visibility.Visible);
		public static readonly IValueConverter NullHiddendNotNullVisible = new BaseConverter(value => value==null ? Visibility.Hidden : Visibility.Visible);
		public static readonly IValueConverter NullVisibleNotNullCollapsed = new BaseConverter(value => value==null ? Visibility.Visible : Visibility.Collapsed);
		public static readonly IValueConverter NullVisibleNotNullHidden = new BaseConverter(value => value==null ? Visibility.Visible : Visibility.Hidden);


		private class BaseConverter:IValueConverter
		{

			private Func<object,Visibility> ConvertFunc;

			public BaseConverter(Func<object, Visibility> convert ) {
				ConvertFunc = convert;
			}

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
				return ConvertFunc(value);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
				throw new NotImplementedException();
			}

		}
	}



}
