using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	public class BindableSelectedItemBehavior:Behavior<TreeView> {

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached(
			"SelectedItem", typeof (object), typeof (BindableSelectedItemBehavior), new PropertyMetadata(default(object),AtSelectedItemChanged));

		public static void SetSelectedItem(DependencyObject element, object value) { element.SetValue(SelectedItemProperty, value); }

		public static object GetSelectedItem(DependencyObject element) { return (object) element.GetValue(SelectedItemProperty); }
		
		private static void AtSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (e.NewValue is TreeViewItem) {
				var treeViewItem = (TreeViewItem) e.NewValue;
				treeViewItem.SetValue(TreeViewItem.IsSelectedProperty, true);
			} else if (e.NewValue is ISelectable) {
				var selectable = (ISelectable) e.NewValue;
				selectable.IsSelected = true;
			} else {
				// The solution for this problem isn't trivial. 
//				var treeViewItem = FindTtreeViewItem(e.NewValue);
//				treeViewItem.SetValue(TreeViewItem.IsSelectedProperty, true);
			}
		}



		protected override void OnAttached() {
			base.OnAttached();

			AssociatedObject.SelectedItemChanged += AtTreeViewSelectedItemChanged;
		}

		protected override void OnDetaching() {
			base.OnDetaching();

			if (AssociatedObject != null) {
				AssociatedObject.SelectedItemChanged -= AtTreeViewSelectedItemChanged;
			}
		}

		private void AtTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			SetSelectedItem(AssociatedObject,e.NewValue);
		}
	}

	public class TreeViewItemAttached {

		public static readonly DependencyProperty InputBindingsProperty =
			DependencyProperty.RegisterAttached("InputBindings", typeof (System.Windows.Input.InputBindingCollection), typeof (TreeViewItemAttached), new PropertyMetadata(null, AtInputBindingsPropertyChanged));

		public static void SetInputBindings(TreeViewItem element, System.Windows.Input.InputBindingCollection value) { element.SetValue(InputBindingsProperty, value); }

		public static System.Windows.Input.InputBindingCollection GetInputBindings(TreeViewItem element) { return (System.Windows.Input.InputBindingCollection) element.GetValue(InputBindingsProperty); }

		private static void AtInputBindingsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs ea) {
			var item =(TreeViewItem)dependencyObject;
			item.InputBindings.Clear();
			item.InputBindings.AddRange(GetInputBindings(item));
		}
		
	}
}
