/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : ValueProviderStringConverter.cs
 * OriginalNamespace: KsWare.Presentation.ViewModelFramework
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using KsWare.Presentation.ViewModelFramework.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides <see cref="StringConverter"/> specifically for <see cref="EditValueProvider"/>/<see cref="DisplayValueProvider"/>
	/// </summary>
	/// <remarks></remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public class ValueProviderStringConverter:TypeConverter,IValueConverter {

		public static readonly ReadOnlyCollection<Type>  SupportedTypes = new List<Type> {
				typeof(Enum),//typeof(Enum?),
				typeof(Guid), typeof(Guid?),
				typeof(Boolean), typeof(Boolean?),
				//TODO
			}.AsReadOnly();

		// char[]   => string
		// string[] => "string" ... "string" (verbatim strings)
		// int[]    => int ... int

// ReSharper disable InconsistentNaming
		private static readonly CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US"); 
		private static readonly CultureInfo invariant = CultureInfo.InvariantCulture; 
// ReSharper restore InconsistentNaming

		public static ValueProviderStringConverter Default=new ValueProviderStringConverter();

		private Lazy<ConversionOptions> _lazyOptions=new Lazy<ConversionOptions>(()=>new ConversionOptions());

		public ConversionOptions Options => _lazyOptions.Value;

		/// <summary> Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return true; //TODO
//			/* Enum  */ if(sourceType.IsEnum) return true;
//			/* Enum? */ //RESERVED implement nullable enum
//			/* Guid  */ if(sourceType==typeof(Guid)) return true;
//			/* Guid? */ if(sourceType==typeof(Guid?)) return true;
//			/* Bool  */ if(sourceType==typeof(Boolean)) return true;
//			if(sourceType.IsPrimitive) return true;
//			if(sourceType.IsArray && CanConvertFrom(context,sourceType.GetElementType())) return true;
//			return false;
		}

		/// <summary> Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
		/// <param name="value">The <see cref="Object"/> to convert.</param>
		/// <returns>
		/// An <see cref="Object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="NotSupportedException">The conversion cannot be performed. </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (culture == null) culture = CultureInfo.CurrentCulture; //default behavior of all converters
//			if(value==null) return null;
//			//RESERVED if(value is DateTime ) return ((DateTime) value).ToString();
//			//RESERVED if(value is DateTime?) return ((DateTime?) value).Value.ToString();
//			return Convert.ToString(value,culture);

			var options = Options.Clone();
			if(options.Culture==null) options.Culture = culture;
//			options.UseHex= parameter is string && ((string) parameter).Contains("hex=true");

			if (value == null) return null;
			if (value.GetType().IsArray) return ConvertArrayToString(value, options);
			return ConvertValueToString(value, options);
		}


		/// <summary> Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return true; //TODO
//			/* Enum  */ if(destinationType.IsEnum) return true;
//			/* Enum? */ //RESERVED implement nullable enum
//			/* Guid  */ if(destinationType==typeof(Guid)) return true;
//			/* Guid? */ if(destinationType==typeof(Guid?)) return true;
//			/* Bool  */ if(destinationType==typeof(Boolean)) return true;
//			if (destinationType == typeof (TimeSpan)) return true;
//			if (destinationType == typeof (TimeSpan?)) return true;
//			if(destinationType.IsPrimitive) return true;
//			if(destinationType.IsArray && CanConvertTo(context,destinationType.GetElementType())) return true;
//			return false;
		}

		/// <summary> Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="Object"/> to convert.</param>
		/// <param name="destinationType">The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.</param>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
		/// <exception cref="NotSupportedException">The conversion cannot be performed. </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (culture == null) culture = CultureInfo.CurrentCulture; //default behavior of all converters
			string stringValue = value == null ? null : value.ToString();

//			return Convert.ChangeType(value, destinationType, culture);
//			//return base.ConvertTo(context, culture, value, destinationType);

			var options = Options.Clone();
			if(options.Culture==null) options.Culture = culture;
//			options.UseHex= parameter is string && ((string) parameter).Contains("hex=true");

			if (value == null) return null;
			if (destinationType.IsArray) return ConvertStringToArray(stringValue, destinationType, options);
			return ConvertStringToValue(stringValue, destinationType, options);
		}

		private bool StringToBool(string s,object v,ConversionOptions options) {
			switch ((s??"").ToLower(enUS)) {
				case "false": case "falsch":case "no" :case "nein": return false;
				case "true" : case "wahr"  :case "yes":case "ja"  : return true;
				default: return System.Convert.ToBoolean(v,options.Culture);
			}
		}

		private object ConvertStringToArray(string value, Type targetType, ConversionOptions options) {
			var elementType = targetType.GetElementType();
			var elementTypeCode = Type.GetTypeCode(elementType);
			if (elementTypeCode == TypeCode.Char) return value.ToCharArray();

			var sItems=string.IsNullOrEmpty(value) ? new string[0] :  value.Split(' ');
			var array = Array.CreateInstance(elementType, sItems.Length);
			for (int i = 0; i < sItems.Length; i++) {
				var s = sItems[i];
				if (elementType == typeof (string)) s = UnEscapeVarbatinString(s);
				array.SetValue(ConvertStringToValue(s,elementType,options),i);
			}
			return array;
		}

		private string UnEscapeVarbatinString(string s) {
			return s.Substring(0, s.Length - 2).Replace("\"\"", "\"");
		}

		private string EscapeStringVarbatin(string s) { return "\"" + s.Replace("\"", "\"\"") + "\""; }

		private object ConvertStringToValue(string value, Type destinationType, ConversionOptions options) {
			/* Enum      */ if(destinationType.IsEnum && value!=null) return Enum.Parse(destinationType, value);
			/* Enum?     */ //TODO nullable enum
			/* Guid      */ if(destinationType==typeof(Guid     )) return value==null ? Guid.Empty : Guid.Parse(value);
			/* Guid?     */ if(destinationType==typeof(Guid?    )) return string.IsNullOrWhiteSpace(value) ? (Guid?) null : (Guid.Parse(value));
			/* TimeSpan  */ if(destinationType==typeof(TimeSpan )) return TimeSpan.Parse(value,options.Culture);
			/* TimeSpan? */ if(destinationType==typeof(TimeSpan?)) return string.IsNullOrEmpty(value) ? (TimeSpan?)null : TimeSpan.Parse(value,options.Culture);

			switch (Type.GetTypeCode(destinationType)) {
				case TypeCode.Empty   : return null;
				case TypeCode.Byte    : return Byte  .Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.SByte   : return SByte .Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.Int16   : return Int16 .Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.Int32   : return Int32 .Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.Int64   : return Int64 .Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.UInt16  : return UInt16.Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.UInt32  : return UInt32.Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.UInt64  : return UInt64.Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.Single  : return StringToSingle(value,options);
				case TypeCode.Double  : return StringToDouble(value,options);
				case TypeCode.Boolean : return StringToBool(value,value,options);
				case TypeCode.String  : return (String ) value;
				case TypeCode.Decimal : return Decimal.Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Integer, options.Culture);
				case TypeCode.Char    : return Char.Parse(value);
				case TypeCode.DateTime: return DateTime.Parse(value,options.Culture);
				case TypeCode.DBNull:
//				case TypeCode.Object:
//					if (targetType == typeof (CanError)) return new CanError((UInt16)ConvertStringToValue(value, typeof (UInt16), useHex));
//					else throw new InvalidCastException();
				default:
					throw new InvalidCastException();
			}			
		}

		private object StringToSingle(string value, ConversionOptions options) {
			if(string.IsNullOrWhiteSpace(value)) return Single.NaN;
			if (value.Trim() == "-"            ) return Single.NegativeInfinity;
			if (value.Trim() == "+"            ) return Single.PositiveInfinity;
			return Single.Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Float, options.Culture);
//			return Convert.ChangeType(value, destinationType, culture);
		}

		private object StringToDouble(string value, ConversionOptions options) {
			if (string.IsNullOrWhiteSpace(value)) return Double.NaN;
			if (value.Trim() == "-"             ) return Double.NegativeInfinity;
			if (value.Trim() == "+"             ) return Double.PositiveInfinity;
			return Double.Parse(value, options.UseHex ? NumberStyles.AllowHexSpecifier : NumberStyles.Float, options.Culture);
//			return Convert.ChangeType(value, destinationType, culture);
		}

		private string ConvertArrayToString(object value, ConversionOptions options) {
			var array = (Array) value;
			var length = array.GetLength(0);
			var elementTypeCode = Type.GetTypeCode(value.GetType().GetElementType());
			if(elementTypeCode==TypeCode.Char) return new string((char[])array);
			var sb = new StringBuilder();
			for (int i = 0; i < length; i++) {
				if (i > 0) sb.Append(options.ArrayElementSeparator);
				var elementString = ConvertValueToString(array.GetValue(i), options);
				if (elementTypeCode == TypeCode.String) elementString =EscapeStringVarbatin(elementString);
				sb.Append(elementString);
			}
			return sb.ToString();
		}

		private string ConvertValueToString(object value, ConversionOptions options) {
			var valueTypeCode = value != null ? Type.GetTypeCode(value.GetType()) : TypeCode.Empty;
			if(value==null) return "";
			switch (valueTypeCode) {
				case TypeCode.Empty   : return "";
				case TypeCode.Byte    : return ((Byte    ) value).ToString(options.UseHex ? "X2"  : null, options.Culture);
				case TypeCode.SByte   : return ((SByte   ) value).ToString(options.UseHex ? "X2"  : null, options.Culture);
				case TypeCode.Int16   : return ((Int16   ) value).ToString(options.UseHex ? "X4"  : null, options.Culture);
				case TypeCode.Int32   : return ((Int32   ) value).ToString(options.UseHex ? "X8"  : null, options.Culture);
				case TypeCode.Int64   : return ((Int64   ) value).ToString(options.UseHex ? "X16" : null, options.Culture);
				case TypeCode.UInt16  : return ((UInt16  ) value).ToString(options.UseHex ? "X4"  : null, options.Culture);
				case TypeCode.UInt32  : return ((UInt32  ) value).ToString(options.UseHex ? "X8"  : null, options.Culture);
				case TypeCode.UInt64  : return ((UInt64  ) value).ToString(options.UseHex ? "X16" : null, options.Culture);
				case TypeCode.Single  : return ((Single  ) value).ToString(options.Culture);
				case TypeCode.Double  : return ((Double  ) value).ToString(options.Culture);
				case TypeCode.Decimal : return ((Decimal ) value).ToString(options.Culture);
				case TypeCode.String  : return  (String  ) value;
				case TypeCode.Boolean : return  (Boolean ) value ? "True" : "False";
				case TypeCode.DateTime: return ((DateTime) value).ToString("O", options.Culture); //TODO default DateTime format
				case TypeCode.Char    : return ((Char    ) value).ToStringEnUs();
				case TypeCode.Object:
//					if (value is CanError) return ((CanError) value).Value.ToString("X4");
//					else throw new InvalidCastException();
					                    return string.Format(options.Culture, "{0}", value);
				default:
					throw new InvalidCastException();
			}
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (culture == null) culture = CultureInfo.CurrentCulture; //default behavior of all converters
			var options = new ConversionOptions();
			options.UseHex= parameter is string && ((string) parameter).Contains("hex=true");
			if(options.Culture==null) options.Culture = culture;

			if (value == null) return null;
			if (targetType.IsArray) return ConvertStringToArray((string) value, targetType, options);
			if (value.GetType().IsArray) return ConvertArrayToString(value, options);
			if (targetType == typeof (string)) return ConvertValueToString(value, options);
			return ConvertStringToValue((string)value, targetType, options);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return Convert(value, targetType, parameter, culture); }


		public class ConversionOptions {

			private static Type[] UseHexFilter = new Type[] {typeof (Byte), typeof (SByte), typeof (Int16), typeof (UInt16), typeof (Int32), typeof (UInt32), typeof (Int64), typeof (UInt64)};

			public ConversionOptions() {
				UseHex = false;
				Culture = null;
				ArrayElementSeparator = " ";
			}

			/// <summary> Gets or sets a value indicating that the numeric string represents a hexadecimal value. 
			/// </summary>
			/// <value><c>true</c> if numeric string represents a hexadecimal value; otherwise, <c>false</c>.</value>
			/// <remarks>
			/// This UseHex is used on: <see cref="Byte"/>, <see cref="SByte"/>, <see cref="Int16"/>, <see cref="UInt16"/>, <see cref="Int32"/>, <see cref="UInt32"/>, <see cref="Int64"/>, <see cref="UInt64"/></remarks>
			/// <seealso cref="NumberStyles.AllowHexSpecifier"/>
			public bool UseHex { get; set; }

			/// <summary> Gets or sets the <see cref="System.Globalization.CultureInfo"/> to use as the current culture.
			/// </summary>
			/// <value>The culture to use.</value>
			/// <remarks> Setting this property will override the current used culture and force that only this culture is used. By default this property sould be <c>null</c>. </remarks>
			public IFormatProvider Culture{ get; set; }

			/// <summary> Gets or sets the array element separator.
			/// </summary>
			/// <value>The array element separator.</value>
			public string ArrayElementSeparator{ get; set; }


			public ConversionOptions Clone() {
				return new ConversionOptions() {
					UseHex                = UseHex,
					Culture               = Culture,
					ArrayElementSeparator = ArrayElementSeparator,
				};
			}
		}
	}
}
