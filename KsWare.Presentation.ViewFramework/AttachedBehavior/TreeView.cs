using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	/// <summary>
	/// Class BindableSelectedItemBehavior.
	/// </summary>
	/// <example> 
	///	<code>
	/// xmlns:ab="clr-namespace:KsWare.Presentation.ViewFramework.AttachedBehavior;assembly=KsWare.Presentation.ViewFramework"
	/// 
	/// &lt;TreeView ItemsSource="{Binding .Items}" ab:BindableSelectedItemBehavior.SelectedItem="{Binding SelectedItem, Mode=TwoWay}"&gt;
	/// </code> 
	/// </example>
	public class BindableSelectedItemBehavior:BehaviorBase<TreeView> {

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached(
			"SelectedItem", typeof (object), typeof (BindableSelectedItemBehavior), new PropertyMetadata(default(object),AtSelectedItemChanged));

		public static void SetSelectedItem(DependencyObject element, object value) { element.SetValue(SelectedItemProperty, value); }

		public static object GetSelectedItem(DependencyObject element) { return (object) element.GetValue(SelectedItemProperty); }
		
		private static void AtSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Debug.WriteLine(e.NewValue);
			var b=Attach<BindableSelectedItemBehavior>(d);

			if (e.NewValue is TreeViewItem) {
				var treeViewItem = (TreeViewItem) e.NewValue;
				treeViewItem.SetValue(TreeViewItem.IsSelectedProperty, true);
//			} else if (e.NewValue is ISelectable) {
//				var selectable = (ISelectable) e.NewValue;
//				selectable.IsSelected = true;
			} else {
				var treeViewItem = GetTreeViewItem(b.AssociatedObject, e.NewValue);
				treeViewItem?.SetValue(TreeViewItem.IsSelectedProperty, true);
			}
		}

		// https://msdn.microsoft.com/en-us/library/ff407130(v=vs.110).aspx

		/// <summary>
		/// Recursively search for an item in this subtree.
		/// </summary>
		/// <param name="container">
		/// The parent ItemsControl. This can be a TreeView or a TreeViewItem.
		/// </param>
		/// <param name="item">
		/// The item to search for.
		/// </param>
		/// <returns>
		/// The TreeViewItem that contains the specified item.
		/// </returns>
		internal static TreeViewItem GetTreeViewItem(ItemsControl container, object item) {
			if (container == null) return null;

			if (container.DataContext == item) return container as TreeViewItem;

			// Expand the current container
			if (container is TreeViewItem && !((TreeViewItem) container).IsExpanded) {
				container.SetValue(TreeViewItem.IsExpandedProperty, true);
			}

			// Try to generate the ItemsPresenter and the ItemsPanel.
			// by calling ApplyTemplate.  Note that in the 
			// virtualizing case even if the item is marked 
			// expanded we still need to do this step in order to 
			// regenerate the visuals because they may have been virtualized away.

			container.ApplyTemplate();
			ItemsPresenter itemsPresenter =
				(ItemsPresenter) container.Template.FindName("ItemsHost", container);
			if (itemsPresenter != null) {
				itemsPresenter.ApplyTemplate();
			}
			else {
				// The Tree template has not named the ItemsPresenter, 
				// so walk the descendents and find the child.
				itemsPresenter = FindVisualChild<ItemsPresenter>(container);
				if (itemsPresenter == null) {
					container.UpdateLayout();

					itemsPresenter = FindVisualChild<ItemsPresenter>(container);
				}
			}

			Panel itemsHostPanel = (Panel) VisualTreeHelper.GetChild(itemsPresenter, 0);


			// Ensure that the generator for this panel has been created.
			UIElementCollection children = itemsHostPanel.Children;

			var virtualizingPanel = itemsHostPanel as TreeViewVirtualizingStackPanel;

			for (int i = 0, count = container.Items.Count; i < count; i++) {
				TreeViewItem subContainer;
				if (virtualizingPanel != null) {
					// Bring the item into view so 
					// that the container will be generated.
					virtualizingPanel.BringIndexIntoView(i);

					subContainer = (TreeViewItem) container.ItemContainerGenerator.ContainerFromIndex(i);
				}
				else {
					subContainer = (TreeViewItem) container.ItemContainerGenerator.ContainerFromIndex(i);

					// Bring the item into view to maintain the 
					// same behavior as with a virtualizing panel.
					subContainer.BringIntoView();
				}

				if (subContainer != null) {
					// Search the next level for the object.
					TreeViewItem resultContainer = GetTreeViewItem(subContainer, item);
					if (resultContainer != null) {
						return resultContainer;
					}
					else {
						// The object is not under this TreeViewItem
						// so collapse it.
						subContainer.IsExpanded = false;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Search for an element of a certain type in the visual tree.
		/// </summary>
		/// <typeparam name="T">The type of element to find.</typeparam>
		/// <param name="visual">The parent element.</param>
		/// <returns></returns>
		private static T FindVisualChild<T>(Visual visual) where T : Visual {
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++) {
				Visual child = (Visual) VisualTreeHelper.GetChild(visual, i);
				if (child != null) {
					T correctlyTyped = child as T;
					if (correctlyTyped != null) {
						return correctlyTyped;
					}

					T descendent = FindVisualChild<T>(child);
					if (descendent != null) {
						return descendent;
					}
				}
			}

			return null;
		}

		private BindableSelectedItemBehavior(TreeView dependencyObject):base(dependencyObject) {}

		protected override void OnAttached() {
			base.OnAttached();

			AssociatedObject.SelectedItemChanged += AtTreeViewSelectedItemChanged;
		}
//
//		protected override void OnDetaching() {
//			base.OnDetaching();
//
//			if (AssociatedObject != null) {
//				AssociatedObject.SelectedItemChanged -= AtTreeViewSelectedItemChanged;
//			}
//		}

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
