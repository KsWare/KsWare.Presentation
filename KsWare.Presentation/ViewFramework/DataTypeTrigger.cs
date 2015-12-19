using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class DataTypeTrigger:DataTrigger {

		private Type _type;
		private Style _style;

		public DataTypeTrigger() {
			Binding=new Binding(".") {Converter = DataTypeConverter.Default};
		}

		public Type Type {
			get { return _type; }
			set {
				_type = value;
				Value = DataTypeConverter.GetString(_type);
			}
		}

		/// <summary> [EXPERIMENTAL] Sets the <see cref="Setters"/> from a style (copy from Style.Setters).
		/// </summary>
		/// <value>The setter from style.</value>
		public Style SetterFromStyle {
			get { return _style; }
			set {
				_style = value;
				foreach (var setter in _style.Setters) Setters.Add(setter);
			}
		}

		private class DataTypeConverter : IValueConverter {

			public static readonly DataTypeConverter Default = new DataTypeConverter();

			public static string GetString(Type type) {
				return "{" + type.FullName + "}";
			}

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
				if (value == null) return "{null}";
				return GetString(value.GetType());
			}
			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
		}

	}
}
