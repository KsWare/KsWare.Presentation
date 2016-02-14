using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework
{

	/// <summary> Provides a thickness converter 
	/// </summary>
	/// <example>
	/// Converts a double value to negative top thickness (Parameter: "-Top")
	/// equivalent to <c>new Thickness(0, -value, 0, 0)</c>
	/// <code>
	/// xmlns:vf="clr-namespace:KsWare.Presentation.ViewFramework;assembly=OSIS.UI"
	/// ...
	/// &gt;vf:SpecialThicknessConverter x:Key="ThicknessConverter"/>	
	/// ...
	/// &gt;Control Margin="{Binding RelativeSource={RelativeSource TemplatedParent}, Path=Distance, Converter={StaticResource ThicknessConverter}, ConverterParameter=-Top}"/>
	/// </code>
	/// </example>
	/// <remarks>
	/// </remarks>
	public class SpecialThicknessConverter:IValueConverter //ALIAS: SingleValueThicknessConverter
	{
		private static readonly IFormatProvider enus=CultureInfo.CreateSpecificCulture("en-US");
		private static readonly ThicknessConverter converter=new ThicknessConverter();

		/// <summary> Converts a numeric value value to <see cref="Thickness"/>.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double v;
			if     (value is string){v=double.Parse((string)value, enus);}
			else if(value is double){v=(double)value;}
			else if(value is float ){v=(float )value;}
			else if(value is long  ){v=(long  )value;}
			else if(value is int   ){v=(int   )value;}
			else if(value is short ){v=(short )value;}
			else if(value is byte  ){v=(byte  )value;}
			else if(value is sbyte ){v=(sbyte )value;}
			else if(value is ushort){v=(ushort)value;}
			else if(value is uint  ){v=(uint  )value;}
			else if(value is ulong ){v=(ulong )value;}
			else if(value is Decimal ){v=Decimal.ToDouble((Decimal)value);}
			else {v = 0; Debug.WriteLine("WARNING: Invalid type of value! "+this.GetType().FullName);}

			switch (parameter as string) {
				case "Left"   : return new Thickness(v, 0, 0, 0);
				case "Top"    : return new Thickness(0, v, 0, 0);
				case "Right"  : return new Thickness(0, 0, v, 0);
				case "Bottom" : return new Thickness(0, 0, 0, v);
				case "-Left"  : return new Thickness(-v, 0, 0, 0);
				case "-Top"   : return new Thickness(0, -v, 0, 0);
				case "-Right" : return new Thickness(0, 0, -v, 0);
				case "-Bottom": return new Thickness(0, 0, 0, -v);
				default:
//					// ConverterParameter='0 * 0 0'
//					string s=((string) parameter).Replace("*", v.ToString(enus));

					// ConverterParameter='0 -{0} 0 0'      <-- OK
					// ConverterParameter='0 -{0:+23} 0 0'  <-- Not implemented
					string s = (string) parameter;
					Match match = Regex.Match(s, @"(\{)((?>\{(?<d>)|\}(?<-d>)|.?)*(?(d)(?!)))(\})", RegexOptions.Compiled);
					//TODO: parse and use additional arguments
					s = s.Replace(match.Value, v.ToString(enus));

					return converter.ConvertFromString(s);
			}
		}

		/// <summary> [NOT IMPLEMENTED] Converts a thickness to a singe value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException("{4694CC1F-2823-4B75-93DA-841C806B5B94}");
		}
	}
 
}