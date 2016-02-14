using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace KsWare.Presentation.ViewFramework {
	
	public partial class ThemeSelector : DependencyObject {

		public static void ApplyTheme(FrameworkElement targetElement, Uri dictionaryUri) {
			var resources= targetElement != null ? targetElement.Resources :  Application.Current.Resources;

			try {
				// find if already has a theme applied  
				var existingDictionaries = resources.MergedDictionaries.OfType<ThemeResourceDictionary>().ToList();
				var index = existingDictionaries.Count > 0 ? resources.MergedDictionaries.IndexOf(existingDictionaries[0]) : -1;

				// remove the existing dictionaries  
				foreach (ThemeResourceDictionary thDictionary in existingDictionaries) resources.MergedDictionaries.Remove(thDictionary);

				if (dictionaryUri != null) {
					var themeDictionary = new ThemeResourceDictionary {Source = dictionaryUri};
					// add the new dictionary to the collection of merged dictionaries of the target object  
					resources.MergedDictionaries.Insert(index >= 0 ? index : 0, themeDictionary);
				}
			}
			catch (Exception ex) {
				Debug.WriteLine("ThemeSelector caught an "+ ex.GetType().Name+"\r\n"+ex.ToString());
			}
			finally { }
		}
	}

	public partial class ThemeSelector /* attached property */ {

		public static readonly DependencyProperty CurrentThemeProperty =
			DependencyProperty.RegisterAttached("CurrentTheme", typeof (Uri),typeof (ThemeSelector),new UIPropertyMetadata(null, OnCurrentThemeChanged));

		public static Uri GetCurrentTheme(FrameworkElement obj) { return (Uri) obj.GetValue(CurrentThemeProperty); }

		public static void SetCurrentTheme(FrameworkElement obj, Uri value) { obj.SetValue(CurrentThemeProperty, value); }

		private static void OnCurrentThemeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (obj is FrameworkElement) /* works only on FrameworkElement objects*/ {
				ApplyTheme(obj as FrameworkElement, GetCurrentTheme((FrameworkElement) obj));
			}
		}

	}

	public class ThemeResourceDictionary : ResourceDictionary {}

}
