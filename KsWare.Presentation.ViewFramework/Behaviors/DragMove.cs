using System.Windows;
using System.Windows.Input;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	/// <summary>
	/// Allows a window to be dragged by a mouse with its left button down over an exposed area of the window's client area.
	/// </summary>
	public class DragMove {

		#region Enabled
		
		/// <summary> The Enabled property
		/// </summary>
		public static readonly DependencyProperty EnabledProperty =
			DependencyProperty.RegisterAttached("Enabled", typeof (bool), typeof (DragMove), new UIPropertyMetadata(default(bool), AtEnabledChanged));

		/// <summary> Gets the Enabled.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>bool</returns>
		public static bool GetEnabled(Window obj) {
			return (bool) obj.GetValue(EnabledProperty);
		}

		/// <summary> Sets the Enabled.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetEnabled(Window obj, bool value) {
			obj.SetValue(EnabledProperty, value);
		}

		private static void AtEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var target = (Window) d;
			var newValue = (bool) e.NewValue;
			if (newValue) target.MouseDown += AtMouseDown;
			else          target.MouseDown -= AtMouseDown;
		}

		private static void AtMouseDown(object sender, MouseButtonEventArgs e) {
			var window = (Window) sender;
			 if (Mouse.LeftButton == MouseButtonState.Pressed) window.DragMove();
		}

		#endregion
	}
}
