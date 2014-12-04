using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using KsWare.Presentation.ViewFramework.Controls;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	public class DataContextErrorVisualizer {

		#region ShowErrorsProperty
		
		/// <summary> The Text property
		/// </summary>
		public static readonly DependencyProperty ShowErrorsProperty =
			DependencyProperty.RegisterAttached("ShowErrors", typeof (bool), typeof (DataContextErrorVisualizer), new UIPropertyMetadata(false, AtShowErrorsChanged));

		/// <summary>
		/// Gets the ShowErrors.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>System.String.</returns>
		public static bool GetShowErrors(DependencyObject obj) {
			return (bool) obj.GetValue(ShowErrorsProperty);
		}

		/// <summary>
		/// Sets the ShowErrors.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetShowErrors(DependencyObject obj, bool value) {
			obj.SetValue(ShowErrorsProperty, value);
		}

		private static void AtShowErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var frameworkElement = (FrameworkElement)d;
			frameworkElement.DataContextChanged+=AtDataContextChanged;
			frameworkElement.Loaded+=FrameworkElementOnLoaded;
			Update(frameworkElement);
		}

		#endregion

		#region ShowDataContextType

		/// <summary> The ShowDataContextType property
		/// </summary>
		public static readonly DependencyProperty ShowDataContextTypeProperty =
			DependencyProperty.RegisterAttached("ShowDataContextType", typeof (bool), typeof (DataContextErrorVisualizer), new UIPropertyMetadata(false, AtShowErrorsChanged));

		/// <summary>
		/// Gets the ShowErrors.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>System.String.</returns>
		public static bool GetShowDataContextType(DependencyObject obj) {
			return (bool) obj.GetValue(ShowDataContextTypeProperty);
		}

		/// <summary>
		/// Sets the ShowErrors.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetShowDataContextType(DependencyObject obj, bool value) {
			obj.SetValue(ShowDataContextTypeProperty, value);
		}

		#endregion

		private static RectangleFrameworkElementAdorner GetAdorner(FrameworkElement frameworkElement) {
			if(frameworkElement.Tag!=null) return (RectangleFrameworkElementAdorner)frameworkElement.Tag;
			var adornerLayer = AdornerLayer.GetAdornerLayer(frameworkElement);
			if(adornerLayer==null) return null;
			var adorner = new RectangleFrameworkElementAdorner(frameworkElement,adornerLayer);
			frameworkElement.Tag = adorner;
			return adorner;
		}

		private static void AtDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Update((FrameworkElement)sender);
		}

		private static void FrameworkElementOnLoaded(object sender, RoutedEventArgs e) {
			Update((FrameworkElement)sender);
		}

		private static void Update(FrameworkElement frameworkElement) {
			var adorner = GetAdorner(frameworkElement);
			if(adorner!=null) {
				if(frameworkElement.DataContext==null) {
					adorner.Text = "";
					adorner.Update();
				} else {
					adorner.Text = frameworkElement.DataContext.GetType().Name;
					adorner.Update();
				}
			}
		}

	}
}