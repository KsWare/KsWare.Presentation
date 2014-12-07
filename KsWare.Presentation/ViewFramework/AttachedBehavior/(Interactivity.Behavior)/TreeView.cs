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

	public class BindableSelectedItemBehavior : Behavior<TreeView> {
		/*
			Reference: System.Windows.Interactivity (C:\Program Files (x86)\Microsoft SDKs\Expression\Blend\.NETFramework\v4.0\Libraries\System.Windows.Interactivity.dll)
		 
				<e:Interaction.Behaviors>
					<behaviors:BindableSelectedItemBehavior SelectedItem="{Binding SelectedAction.Target, Mode=TwoWay}" />
				</e:Interaction.Behaviors>
		 */

		#region SelectedItem Property

		public object SelectedItem {
			get { return (object) GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof (object), typeof (BindableSelectedItemBehavior), new UIPropertyMetadata(null, AtSelectedItemChanged));

		private static void AtSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
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

		#endregion

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
			SelectedItem = e.NewValue;
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
