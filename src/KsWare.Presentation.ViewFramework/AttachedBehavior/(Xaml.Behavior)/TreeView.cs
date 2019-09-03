using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	/// <summary>
	/// Class BindableSelectedItemBehaviorV40.
	/// </summary>
	/// <example>
	/// <remarks> Requires NuGet Microsoft.Xaml.Behaviors.WPF 1.0 </remarks>
	/// <code>
	///		xmlns:e="http://schemas.microsoft.com/xaml/behaviors"
	/// 
	///		&lt;e:Interaction.Behaviors&gt;
	///		&lt;behaviors:BindableSelectedItemBehavior SelectedItem="{Binding SelectedAction.Target, Mode=TwoWay}" /&gt;
	///		&lt;/e:Interaction.Behaviors&gt;
	/// </code>
	/// </example>
	public class BindableSelectedItemBehaviorV40 : Behavior<TreeView> {
		/*
			Reference: Microsoft.Xaml.Behaviors.dll (NuGet: Microsoft.Xaml.Behaviors.WPF)
		 
				<e:Interaction.Behaviors>
					<behaviors:BindableSelectedItemBehavior SelectedItem="{Binding SelectedAction.Target, Mode=TwoWay}" />
				</e:Interaction.Behaviors>
		 */

		#region SelectedItem Property

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof (object), typeof (BindableSelectedItemBehaviorV40), new UIPropertyMetadata(null, AtSelectedItemChanged));

		public object SelectedItem {get => (object) GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

		private static void AtSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			var b = (BindableSelectedItemBehaviorV40) sender;
			if (e.NewValue is TreeViewItem) {
				var treeViewItem = (TreeViewItem) e.NewValue;
				treeViewItem.SetValue(TreeViewItem.IsSelectedProperty, true);
//			} else if (e.NewValue is ISelectable) {
//				var selectable = (ISelectable) e.NewValue;
//				selectable.IsSelected = true;
			} else {
				var treeViewItem = BindableSelectedItemBehavior.GetTreeViewItem(b.AssociatedObject, e.NewValue);
				treeViewItem?.SetValue(TreeViewItem.IsSelectedProperty, true);
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

	public class TreeViewItemAttachedV40 {

		public static readonly DependencyProperty InputBindingsProperty =
			DependencyProperty.RegisterAttached("InputBindings", typeof (System.Windows.Input.InputBindingCollection), typeof (TreeViewItemAttachedV40), new PropertyMetadata(null, AtInputBindingsPropertyChanged));

		public static void SetInputBindings(TreeViewItem element, System.Windows.Input.InputBindingCollection value) { element.SetValue(InputBindingsProperty, value); }

		public static System.Windows.Input.InputBindingCollection GetInputBindings(TreeViewItem element) { return (System.Windows.Input.InputBindingCollection) element.GetValue(InputBindingsProperty); }

		private static void AtInputBindingsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs ea) {
			var item =(TreeViewItem)dependencyObject;
			item.InputBindings.Clear();
			item.InputBindings.AddRange(GetInputBindings(item));
		}
		
	}
}
