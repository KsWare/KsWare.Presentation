using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;


namespace KsWare.Presentation.ViewFramework.Designtime {

	public static class Designtime {

		#region Designtime.IsChecked Property

		public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached(
			"IsChecked", typeof (bool?), typeof (Designtime), new PropertyMetadata(default(bool?),AtIsCheckedChanged));

		public static void SetIsChecked(DependencyObject element, bool? value) { element.SetValue(IsCheckedProperty, value); }

		public static bool? GetIsChecked(DependencyObject element) { return (bool?) element.GetValue(IsCheckedProperty); }

		private static void AtIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!DesignerProperties.GetIsInDesignMode(d)) return;
			var propertyInfo = d.GetType().GetProperty("IsChecked");
			if(propertyInfo==null) return;
			propertyInfo.SetValue(d, e.NewValue,null);
		}

		#endregion

		#region Designtime.Content Property

		public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
			"Content", typeof (object), typeof (Designtime), new PropertyMetadata(default(object),AtContentChanged));

		public static void SetContent(DependencyObject element, object value) { element.SetValue(ContentProperty, value); }

		public static object GetContent(DependencyObject element) { return element.GetValue(ContentProperty); }

		private static void AtContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!DesignerProperties.GetIsInDesignMode(d)) return;
			var propertyInfo = d.GetType().GetProperty("Content");
			if(propertyInfo==null) return;
			propertyInfo.SetValue(d, e.NewValue,null);
		}

		#endregion

		#region Designtime.Source Property

		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
			"Source", typeof (ImageSource), typeof (Designtime), new PropertyMetadata(default(ImageSource),AtSourceChanged));

		public static void SetSource(Image element, Image value) { element.SetValue(SourceProperty, value); }

		public static ImageSource GetSource(Image element) { return (ImageSource) element.GetValue(SourceProperty); }

		private static void AtSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!DesignerProperties.GetIsInDesignMode(d)) return;
			((Image)d).Source = (ImageSource) e.NewValue;
		}

		#endregion

		#region Designtime.Visibility Property

		public static readonly DependencyProperty VisibilityProperty = DependencyProperty.RegisterAttached(
			"Visibility", typeof (Visibility), typeof (Designtime), new PropertyMetadata(default(Visibility),AtVisibilityChanged));

		public static void SetVisibility(UIElement element, Visibility value) { element.SetValue(VisibilityProperty, value); }

		public static Visibility GetVisibility(UIElement element) { return (Visibility) element.GetValue(VisibilityProperty); }

		private static void AtVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!DesignerProperties.GetIsInDesignMode(d)) return;
			((UIElement)d).Visibility = (Visibility) e.NewValue;
		}

		#endregion


	}

}
