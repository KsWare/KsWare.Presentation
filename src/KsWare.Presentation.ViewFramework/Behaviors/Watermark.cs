using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace KsWare.Presentation.ViewFramework.Behaviors {

	/// <summary> Provides properties to implement a watermark (TextBox, PasswordBox)
	/// </summary>
	public class Watermark {

		private static readonly List<WeakReference> RegisteredControls=new List<WeakReference>();

		#region TextProperty
		
		/// <summary> The Text property
		/// </summary>
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.RegisterAttached("Text", typeof (string), typeof (Watermark), new UIPropertyMetadata(null, AtTextChanged));

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>System.String.</returns>
		public static string GetText(DependencyObject obj) {
			return (string) obj.GetValue(TextProperty);
		}

		/// <summary>
		/// Sets the text.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetText(DependencyObject obj, string value) {
			obj.SetValue(TextProperty, value);
		}

		#endregion

		#region IsVisibleProperty

		/// <summary>
		/// The IsVisible property. Used by control implementers only.
		/// </summary>
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.RegisterAttached("IsVisible", typeof (bool), typeof (Watermark), new UIPropertyMetadata(false));

		/// <summary>
		/// Gets the IsVisible property.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
		public static bool GetIsVisible(DependencyObject obj) {
			return (bool) obj.GetValue(IsVisibleProperty);
		}

		/// <summary>
		/// Sets the IsVisible property
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public static void SetIsVisible(DependencyObject obj, bool value) {
			obj.SetValue(IsVisibleProperty, value);
		}

		#endregion

		private static void AtTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if     (obj is PasswordBox passwordBox) AtTextChanged(passwordBox);
			else if(obj is TextBox     textBox) AtTextChanged(textBox);
		}

		private static void AtTextChanged(PasswordBox passwordBox) {
			CheckShowWatermark(passwordBox);

			for (int i = 0; i < RegisteredControls.Count; i++) {
				var r = RegisteredControls[i];
				if(!r.IsAlive) {RegisteredControls.RemoveAt(i);i--;continue;}
				if(Equals(passwordBox,r.Target)) return;
			}

			passwordBox.PasswordChanged += AtPasswordBoxOnPasswordChanged;
			passwordBox.Unloaded += AtControlOnUnloaded;			
		}

		private static void AtTextChanged(TextBox textBox) {
			CheckShowWatermark(textBox);

			for (int i = 0; i < RegisteredControls.Count; i++) {
				var r = RegisteredControls[i];
				if(!r.IsAlive) {RegisteredControls.RemoveAt(i);i--;continue;}
				if(Equals(textBox,r.Target)) return;
			}
			
			textBox.TextChanged += AtTextBoxOnTextChanged;
			textBox.Unloaded += AtControlOnUnloaded;
			RegisteredControls.Add(new WeakReference(textBox));
		}

		private static void CheckShowWatermark(PasswordBox passwordBox) {
			passwordBox.SetValue(IsVisibleProperty, string.IsNullOrEmpty(passwordBox.Password));
		}

		private static void CheckShowWatermark(TextBox textBox) {
			textBox.SetValue(IsVisibleProperty, string.IsNullOrEmpty(textBox.Text));
		}

		private static void AtPasswordBoxOnPasswordChanged(object sender, RoutedEventArgs e) {
			var passwordBox = (PasswordBox)sender;
			CheckShowWatermark(passwordBox);
		}

		private static void AtControlOnUnloaded(object sender, RoutedEventArgs e) {
			var passwordBox = sender as PasswordBox;
			if(passwordBox!=null) passwordBox.PasswordChanged -= AtPasswordBoxOnPasswordChanged;
			var textBox = sender as TextBox;
			if(textBox!=null) textBox.TextChanged -= AtTextBoxOnTextChanged;
		}

		private static void AtTextBoxOnTextChanged(object sender, TextChangedEventArgs e) {
			var box = sender as TextBox;
			CheckShowWatermark(box);
		}

	}
}