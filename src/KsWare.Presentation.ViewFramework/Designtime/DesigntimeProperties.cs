using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

//TODO: remove. extracted to KsWare.Presentation.ViewFramework.Common
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

		#region Background

		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(Designtime), new PropertyMetadata(default(Brush),AtBackgroundChanged));

		private static void AtBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!DesignerProperties.GetIsInDesignMode(d)) return;
			((Control)d).Background = (Brush) e.NewValue;
		}

		public static void SetBackground(DependencyObject element, Brush value) { element.SetValue(BackgroundProperty, value); }

		public static Brush GetBackground(DependencyObject element) { return (Brush) element.GetValue(BackgroundProperty); }

		#endregion

		#region Style

		public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached("Style", typeof(Style), typeof(Designtime), new PropertyMetadata(default(Style),AtStyleChanged));

		private static void AtStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!DesignerProperties.GetIsInDesignMode(d)) return;
			((FrameworkElement) d).Style = (Style) e.NewValue;
		}

		public static void SetStyle(DependencyObject element, Style value) { element.SetValue(StyleProperty, value); }

		public static Style GetStyle(DependencyObject element) { return (Style) element.GetValue(StyleProperty); }

		#endregion

		#region Properties

// TODO: WIP
//		private static readonly DependencyPropertyKey PropertiesPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Properties", typeof(DesigntimePropertyCollection), typeof(Designtime), new PropertyMetadata(default(DesigntimePropertyCollection)));
//
//		public static DependencyProperty PropertiesProperty => PropertiesPropertyKey.DependencyProperty;
//
//		public static void SetProperties(DependencyObject element, DesigntimePropertyCollection value) { element.SetValue(PropertiesProperty, value); }
//
//		public static DesigntimePropertyCollection GetProperties(DependencyObject element) { return (DesigntimePropertyCollection) element.GetValue(PropertiesProperty); }

		private static readonly DependencyPropertyKey PropertiesPropertyKey =
			DependencyProperty.RegisterAttachedReadOnly("PropertiesInternal", typeof(DesigntimePropertyCollection), typeof(Designtime),
				new UIPropertyMetadata(null));

		public static readonly DependencyProperty PropertiesProperty = PropertiesPropertyKey.DependencyProperty;

		private static void SetProperties(DependencyObject obj, DesigntimePropertyCollection value) {
			obj.SetValue(PropertiesPropertyKey, value);
		}

		public static DesigntimePropertyCollection GetProperties(DependencyObject obj) {
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (obj.GetValue(PropertiesProperty) is DesigntimePropertyCollection col) return col;
			col = new DesigntimePropertyCollection(obj);
			SetProperties(obj, col);
			return col;
		}

		#endregion
	}

// TODO: WIP
	public class DesigntimePropertyCollection : ObservableCollection<DesigntimeProperty> {

		public DesigntimePropertyCollection(DependencyObject associatedObject) {
			if(!DesignerProperties.GetIsInDesignMode(associatedObject)) return; // do nothing at runtime
			AssociatedObject = associatedObject;
		}
		public DependencyObject AssociatedObject { get; }

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (!DesignerProperties.GetIsInDesignMode(AssociatedObject)) return; // do nothing at runtime
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					e.NewItems.Cast<DesigntimeProperty>().ForEach(OnPropertyAdded);
					break;
			}
		}

		private void OnPropertyAdded(DesigntimeProperty designtimeProperty) {
			var pi=AssociatedObject.GetType().GetProperty(designtimeProperty.Name,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var value = System.Convert.ChangeType(designtimeProperty.Value, pi.PropertyType);
			AssociatedObject.GetType().InvokeMember(designtimeProperty.Name,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty, null,
				AssociatedObject, new object[] {value});
		}
	}

	public class DesigntimeProperty : DependencyObject {

		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DesigntimeProperty), new PropertyMetadata(default(string)));

		public string Name { get => (string) GetValue(NameProperty); set => SetValue(NameProperty, value); }

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DesigntimeProperty), new PropertyMetadata(default(object)));

		public object Value { get => (object) GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
	}
}

// Attached Read-only Collection Dependency Properties
// https://digitaltapestry.wordpress.com/2008/07/28/attached-read-only-collection-dependency-properties/