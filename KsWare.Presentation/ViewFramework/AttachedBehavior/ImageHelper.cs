using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	public static class ImageHelper {

		private static void SourceResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var element = d as Image;
			if (element == null) return;
			if(e.NewValue!=null) element.SetResourceReference(Image.SourceProperty, e.NewValue);
			else                  element.SetValue(Image.SourceProperty, null);
//			else                  element.ClearValue(Image.SourceProperty);
		}

		public static readonly DependencyProperty SourceResourceKeyProperty = DependencyProperty.RegisterAttached("SourceResourceKey",
			typeof (object),
			typeof (ImageHelper),
			new PropertyMetadata(String.Empty, SourceResourceKeyChanged));

		public static void SetSourceResourceKey(Image element, object value) { element.SetValue(SourceResourceKeyProperty, value); }

		public static object GetSourceResourceKey(Image element) { return element.GetValue(SourceResourceKeyProperty); }

	}

}
