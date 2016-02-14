using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	/// <summary> Provides properties to implement CommandBindings/RoutedCommands for MVVM
	/// </summary>
	/// <example>
	/// <code>
	/// &lt;UserControl
	///     DataContext="{any IObjectVM}"
	///     behaviors:CommandBindingsBehavior.DataContext="{Binding .}"
	/// > 
	/// &lt;/UserControl>
	/// </code>
	/// </example>
	public class CommandBindingsBehavior {

		#region DataContextProperty

		/// <summary> The DataContext property
		/// </summary>
		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.RegisterAttached("DataContext", typeof (object), typeof (CommandBindingsBehavior), new UIPropertyMetadata(null, AtDataContextChanged));

		/// <summary> Gets the DataContext.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>System.String.</returns>
		public static object GetDataContext(DependencyObject obj) {
			return (object) obj.GetValue(DataContextProperty);
		}

		/// <summary> Sets the DataContext.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetDataContext(DependencyObject obj, object value) {
			obj.SetValue(DataContextProperty, value);
		}

		#endregion

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private static void AtDataContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
//			for (int i = 0; i < __registeredControls.Count; i++) {
//				var r = __registeredControls[i];
//				if(!r.IsAlive) {__registeredControls.RemoveAt(i);i--;continue;}
//				if(Equals(obj,r.Target)) return;
//			}
//			obj.Unloaded += AtControlOnUnloaded;

			var uiElement = (UIElement) obj;
			uiElement.CommandBindings.Clear();

			var vm = (IObjectVM) e.NewValue;

			if (vm != null) {
				foreach (var child in vm.Children) {
					if (child is ActionVM) {
						var action = (ActionVM) child;
						if (action.IsRoutedCommand) uiElement.CommandBindings.Add(action.CreateCommandBinding());
					}
				}
			}
		}

		private static void AtControlOnUnloaded(object sender, RoutedEventArgs e) {

		}

	}
}