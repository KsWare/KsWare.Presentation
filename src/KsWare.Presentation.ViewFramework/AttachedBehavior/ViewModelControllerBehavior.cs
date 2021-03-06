﻿using System.Windows;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	/// <summary>
	/// Connects a UIElement with a controller.
	/// </summary>
	/// <seealso cref="UIElementControllerVM{UIElement}" />
	public class ViewModelControllerBehavior : BehaviorBase<UIElement> {

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(
			"ViewModel", 
			typeof(IUIElementControllerVM), 
			typeof(ViewModelControllerBehavior), 
			new FrameworkPropertyMetadata(default(IUIElementControllerVM),ViewModelPropertyChanged));

		/// <summary>
		/// Sets the controller view model.
		/// </summary>
		/// <param name="element">The UI element.</param>
		/// <param name="controller">The controller view model.</param>
		public static void SetViewModel(DependencyObject element, IUIElementControllerVM controller) { element.SetValue(ViewModelProperty, controller); }

		/// <summary>
		/// Gets the view model.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>UIElementControllerVM&lt;UIElement&gt;.</returns>
		/// <autogeneratedoc />
		public static IUIElementControllerVM GetViewModel(DependencyObject element) { return (IUIElementControllerVM) element.GetValue(ViewModelProperty); }

		private static void ViewModelPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
			var ab=Attach<ViewModelControllerBehavior>(dependencyObject);
			if (e.NewValue != null) {
				var vm = (IUIElementControllerVM) e.NewValue;
				vm.Connect(ab.AssociatedObject);
			}
		}

		protected ViewModelControllerBehavior(UIElement dependencyObject) : base(dependencyObject) {

		}
	}
}
