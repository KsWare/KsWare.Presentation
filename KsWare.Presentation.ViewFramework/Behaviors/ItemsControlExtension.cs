using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	// ItemsControlExtension.GroupStyleSelector

	public class ItemsControlExtension {

		public static readonly DependencyProperty GroupStyleSelectorProperty =
			DependencyProperty.RegisterAttached("GroupStyleSelector", typeof(GroupStyleSelector), typeof(ItemsControlExtension), 
			new PropertyMetadata(null,AtGroupStyleSelectorPropertyChanged));

		public static void SetGroupStyleSelector(UIElement element, GroupStyleSelector value) {
			element.SetValue(GroupStyleSelectorProperty, value);
		}

		public static GroupStyleSelector GetGroupStyleSelector(UIElement element) {
			return (GroupStyleSelector)element.GetValue(GroupStyleSelectorProperty);
		}

		private static void AtGroupStyleSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var itemsControl = d as ItemsControl;
			if(itemsControl==null) return;
			var newValue = (GroupStyleSelector)e.NewValue;

			if(newValue!=null) {
				itemsControl.GroupStyleSelector=newValue.SelectTemplate;
			} else {
				itemsControl.GroupStyleSelector = null;
			}
		}

	}
}
