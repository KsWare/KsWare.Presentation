using System.Windows;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	/// <summary>  Provides properties to extent a ContextMenu
	/// </summary>
	public class ContextMenuExtension {

		#region HeaderProperty
		
		/// <summary> The Header property
		/// </summary>
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.RegisterAttached("Header", typeof (object), typeof (ContextMenuExtension), new UIPropertyMetadata(false));

		/// <summary> Gets the Header.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public static object GetHeader(DependencyObject obj) {
			return obj.GetValue(HeaderProperty);
		}

		/// <summary> Sets the Header.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetHeader(DependencyObject obj, object value) {
			obj.SetValue(HeaderProperty, value);
		}

		#endregion

	}
}