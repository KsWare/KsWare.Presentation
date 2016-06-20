using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace KsWare.Presentation.ViewFramework.Behaviors
{
	/// <summary>
	/// Opens the context menu at left click (popup behavior)
	/// </summary>
	public class OpenContextMenuAtLeftClickBehavior
	{

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(OpenContextMenuAtLeftClickBehavior), new UIPropertyMetadata(false, HandleIsEnabledChanged));
		
		public static bool GetIsEnabled(DependencyObject obj) {
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(DependencyObject obj, bool value) {
			obj.SetValue(IsEnabledProperty, value);
		}

		private static readonly DependencyPropertyKey IsOpenPropertyKey =
			DependencyProperty.RegisterAttachedReadOnly("IsOpen", typeof(bool?), typeof(OpenContextMenuAtLeftClickBehavior), new PropertyMetadata(null));

		public static readonly DependencyProperty IsOpenProperty = IsOpenPropertyKey.DependencyProperty;
		
		private static void SetIsOpen(ContextMenu contextMenu, bool? value) {
			contextMenu.SetValue(IsOpenPropertyKey, value);
		}

		public static bool? GetIsOpen(ContextMenu contextMenu) {
			return (bool)contextMenu.GetValue(IsOpenPropertyKey.DependencyProperty);
		}

		private static void HandleIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var button = d as ButtonBase;
			if(button==null) return;

			if((bool)e.NewValue==true) {
				button.Click += AtClick;
			} else {
				button.Click -= AtClick;
			}
		}

		private static void AtClick(object sender, RoutedEventArgs e) {
			var contextMenu=((FrameworkElement)sender).ContextMenu;
			if(contextMenu==null)return;

			var flag = (bool?)contextMenu.GetValue(IsOpenPropertyKey.DependencyProperty);
			if(flag==null) {
				contextMenu.Closed+= (o, args) => ((ContextMenu)o).SetValue(IsOpenPropertyKey, false);
			} else if(flag==true) {
				contextMenu.SetValue(IsOpenPropertyKey,false);
				return;
			}

			contextMenu.Placement=PlacementMode.Custom;
			contextMenu.PlacementTarget = (UIElement)sender;
			contextMenu.IsOpen = true;

			contextMenu.SetValue(IsOpenPropertyKey,true);
		}
	}
}
