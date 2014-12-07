using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	// UIElementBehavior.SelectAllOnKeyboardFocus

	public static class UIElementBehavior {

		public static readonly DependencyProperty SelectAllOnKeyboardFocusProperty =
			DependencyProperty.RegisterAttached("SelectAllOnKeyboardFocus", typeof(bool), typeof(UIElementBehavior), new PropertyMetadata(default(bool), AtSelectAllOnKeyboardFocusChanged));

		public static void SetSelectAllOnKeyboardFocus(TextBox element, bool value) {
			element.SetValue(SelectAllOnKeyboardFocusProperty, value);
		}

		public static bool GetSelectAllOnKeyboardFocus(TextBox element) {
			return (bool)element.GetValue(SelectAllOnKeyboardFocusProperty);
		}

		private static void AtSelectAllOnKeyboardFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var tb = (TextBox)d;

			if((bool)e.NewValue) {
				tb.GotKeyboardFocus           += AtGotFocus;
//				tb.GotFocus                   += AtGotFocus;
//				tb.PreviewMouseLeftButtonDown += AtPreviewMouseLeftButtonDown;

				if(tb.IsFocused) tb.SelectAll();
			} else {
				tb.GotKeyboardFocus           -= AtGotFocus;
//				tb.GotFocus                   -= AtGotFocus;
//				tb.PreviewMouseLeftButtonDown -= AtPreviewMouseLeftButtonDown;
			}
		}

		private static void AtPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			var tb = (TextBox)sender;

			if(!tb.IsKeyboardFocusWithin) {
				e.Handled = true;
				tb.Focus();
			}
		}

		private static void AtGotFocus(object sender, RoutedEventArgs e) {
			var tb = (TextBox)sender;
			tb.SelectAll();
		}

	}

}


/*    /// <summary>
	/// Attached behavior for UIElement, if a property is set, calls UpdateSource on the binding, when enter press is recognized in the KeyDown handler
	/// of the UIElement the behavior is attached to.
	/// </summary>
	public static class UpdatePropertySourceWhenEnterPressedBehavior
	{
		/// <summary>
		/// The property the behavior should work with.
		/// </summary>
		public static readonly DependencyProperty PropertyProperty = DependencyProperty.RegisterAttached(
			"Property", typeof(DependencyProperty), typeof(UpdatePropertySourceWhenEnterPressedBehavior), new PropertyMetadata(null, OnPropertyChanged));

		/// <summary>
		/// Sets the property the behavior should work with on the given UIElement.
		/// </summary>
		public static void SetProperty(UIElement uie, DependencyProperty value)
		{
			uie.SetValue(PropertyProperty, value);
		}
		/// <summary>
		/// Gets the property the behavior should work with on the given UIElement.
		/// </summary>
		public static DependencyProperty GetProperty(UIElement uie)
		{
			return (DependencyProperty)uie.GetValue(PropertyProperty);
		}

		static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var uie = (UIElement)d;
			if (uie == null)
			    return; 
			
			if (e.OldValue != null)
			    uie.PreviewKeyDown -= HandlePreviewKeyDown; 
			if (e.NewValue != null)
			    uie.PreviewKeyDown += HandlePreviewKeyDown;
		}

		static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter) 
			    return;

			var uie = e.Source as UIElement;
			if (uie == null)
			    return;

			var property = GetProperty(uie);
			if (property == null)
			    return;

			var binding = BindingOperations.GetBindingExpression(uie, property);
			if (binding != null)
			    binding.UpdateSource();
		}
	}
*/

/*
	///<summary>
	/// Attached behavior to move focus to the next element (as determined by the FocusManager) when enter press is recognized in the KeyDown handler
	/// of the UIElement the behavior is enabled on.
	///</summary>
	public class MoveFocusWhenEnterPressedBehavior
	{
		///<summary>
		/// Installs or deinstalls the behavior.
		///</summary>
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
			"IsEnabled", typeof(bool), typeof(MoveFocusWhenEnterPressedBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

		/// <summary>
		/// Returns whether the behavior is enabled.
		/// </summary>
		public static bool GetIsEnabled(UIElement uie) { return (bool)uie.GetValue(IsEnabledProperty); }
		/// <summary>
		/// Sets whether the behavior is enabled.
		/// </summary>
		public static void SetIsEnabled(UIElement uie, bool value) { uie.SetValue(IsEnabledProperty, value); }

		static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var uie = (UIElement)d;

			if ((bool)e.NewValue)
			    uie.KeyDown += OnKeyDown;
			else
			    uie.KeyDown -= OnKeyDown;
		}

		static void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
			    return;

			var uie = e.Source as UIElement;
			if (uie == null)
			    return;

			uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}
	}
*/

/*
	/// <summary>
	/// Attached behavior for TextBox, if enabled selects all on focus gain and focuses on left click if not focused yet.
	/// </summary>
	public static class SelectAllOnFocusBehavior
	{
		/// <summary>
		/// Whether the behavior is enabled.
		/// </summary>
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
			"IsEnabled", typeof(bool), typeof(SelectAllOnFocusBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

		/// <summary>
		/// Returns whether the behavior is enabled on given TextBox.
		/// </summary>
		public static bool GetIsEnabled(TextBox tb) { return (bool)tb.GetValue(IsEnabledProperty); }
		/// <summary>
		/// Sets whether the behavior is enabled on given TextBox.
		/// </summary>
		public static void SetIsEnabled(TextBox tb, bool value) { tb.SetValue(IsEnabledProperty, value); }

		static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var tb = (TextBox)d;

			if ((bool)e.NewValue)
			{
			    tb.GotFocus += OnGotFocus;
			    tb.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;

			    if (tb.IsFocused)
			        tb.SelectAll();
			}
			else
			{
			    tb.GotFocus -= OnGotFocus;
			    tb.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
			}
		}

		static void OnGotFocus(object sender, RoutedEventArgs e)
		{
			var tb = (TextBox)sender;

			tb.SelectAll();
		}

		static void OnPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
		{
			var tb = (TextBox)sender;

			if (!tb.IsKeyboardFocusWithin)
			{
			    e.Handled = true;
			    tb.Focus();
			}
		}
	}
*/
