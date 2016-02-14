using System;
using System.Globalization;
using System.Windows.Data;
using KsWare.Presentation.Core;

namespace KsWare.Presentation.ViewFramework {


	/// <summary> Provides a <see cref="IValueConverter"/> to convert a binding value into its type name.
	/// </summary>
	/// <example>Shows the current binding result:<code>&lt;TextBlock Text="{Binding ., Converter={x:Static viewFramework:DebugTypeNameConverter.Default}}" Foreground="White" Background="Blue"/&gt;</code></example>
	public class DebugTypeNameConverter:IValueConverter {

		public static readonly DebugTypeNameConverter Default = new DebugTypeNameConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return DebugUtil.FormatTypeName(value, true);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}