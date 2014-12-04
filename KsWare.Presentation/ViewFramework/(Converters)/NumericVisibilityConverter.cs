using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace OSIS.UI.KsWare.Presentation.ViewFramework
{
	public class NumericVisibilityConverter:IValueConverter
	{

		private static IValueConverter _collapseEq0ElseVisible;
		
		public static IValueConverter CollapseEq0ElseVisible {
			get { 
				if(_collapseEq0ElseVisible==null) _collapseEq0ElseVisible=new CollapseEq0ElseVisibleConverter();
				return _collapseEq0ElseVisible;
			}
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}

		
		private class CollapseEq0ElseVisibleConverter:IValueConverter
		{

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
				if(value==null) return Visibility.Collapsed;
				var doubleValue = System.Convert.ToDouble(value);
				if(System.Math.Abs(doubleValue-0)<(double.Epsilon*10)) return Visibility.Collapsed;
				return Visibility.Visible;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
				throw new NotImplementedException();
			}	

		}
	}
}
