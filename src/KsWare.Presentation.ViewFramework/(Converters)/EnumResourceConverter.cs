using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KsWare.Presentation.ViewFramework {

	public class EnumResourceConverter:IValueConverter {

		public Uri BaseUri { get; set; }
		public string FileMask { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			object res = null;

			if (value == null) return null;
			
			if (value is Enum) {
				var attr=GetAttribute<ResourceAttribute>((Enum) value);
				if (attr == null) return null;
				var name = string.IsNullOrEmpty(attr.Name)?((Enum)value).ToString():attr.Name;

				// @"/GalaxyLegendWindows;component\Controls\(ImageControls)\MineImage\Resources\"
				string url=BaseUri.OriginalString;
				url = "pack://application:,,," + url;
				url += FileMask.Replace("*", name);

				var res1 = new BitmapImage();
				res1.BeginInit();
				res1.UriSource = new Uri(url);
				res1.EndInit();
				res = res1;
			}
		
			if (typeof (ImageSource).IsAssignableFrom(targetType)) return res;
			if (typeof (Image).IsAssignableFrom(targetType)) return new Image{Source = (ImageSource)res};
			return null;
		}

//		public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap bitmap) {
//			using (System.IO.MemoryStream stream) {
//				bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
//				stream.Position = 0;
//
//				var result = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOptions.OnLoad);
//				result.Freeze();
//				return result;
//			}
//		}

		public static T GetAttribute<T>(Enum enumValue) where T : Attribute {
			T attribute;

			MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

			if (memberInfo != null) {
				attribute = (T) memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
				return attribute;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
	}
}
