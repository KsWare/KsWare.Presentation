using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	// http://stackoverflow.com/questions/1977929/wpf-listbox-with-a-listbox-ui-virtualization-and-scrolling
	// Note that in .Net 4.5 there's no need for this hack as you can set VirtualizingPanel.ScrollUnit="Pixel". 
	// use VirtualizingPanel.ScrollUnit="Pixel"

	/*	<ListBox>
		   <ListBox.ItemsPanel>
			  <ItemsPanelTemplate>
				 <VirtualizingStackPanel PixelBasedScrollingBehavior.IsEnabled="True">
				  </VirtualizingStackPanel>
			   </ItemsPanelTemplate>
		   </ListBox.ItemsPanel>
		</ListBox>	
	 */

	[Obsolete("In .Net 4.5 there's no need for this hack as you can set VirtualizingPanel.ScrollUnit=\"Pixel\".")]
	public static class PixelBasedScrollingBehavior {

		public static bool GetIsEnabled(DependencyObject obj) {
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(DependencyObject obj, bool value) {
			obj.SetValue(IsEnabledProperty, value);
		}

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PixelBasedScrollingBehavior), new UIPropertyMetadata(false, HandleIsEnabledChanged));

		private static void HandleIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var vsp = d as VirtualizingStackPanel;
			if(vsp==null) return;

			var property = typeof(VirtualizingStackPanel).GetProperty("IsPixelBased",BindingFlags.NonPublic|BindingFlags.Instance);

			if(property==null) {
				throw new InvalidOperationException("Pixel-based scrolling behavior hack no longer works!");
			}

			if((bool)e.NewValue==true) {
				property.SetValue(vsp, true, new object[0]);
			} else {
				property.SetValue(vsp, false, new object[0]);
			}
		}

	}

}
