using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

/*
 * SelectParentListBoxItemAtExpandedProperty
 */

namespace KsWare.Presentation.ViewFramework.Behaviors {

	/// <summary> Provides extensions for <see cref="Expander"/>
	/// </summary>
	public class ExpanderExtension {

		public static readonly DependencyProperty SelectParentListBoxItemAtExpandedProperty =
			DependencyProperty.RegisterAttached("SelectParentListBoxItemAtExpanded", typeof(bool), typeof(ExpanderExtension), new UIPropertyMetadata(false, AtSelectParentListBoxItemAtExpandedChanged));
		
		public static bool GetSelectParentListBoxItemAtExpanded(DependencyObject obj) {
			return (bool)obj.GetValue(SelectParentListBoxItemAtExpandedProperty);
		}

		/// <summary> Sets a value indicating whether the parent <see cref="ListBoxItem"/> is selected if the <see cref="Expander" /> is expanded.
		/// </summary>
		/// <param name="obj">The <see cref="Expander"/></param>
		/// <param name="value"><c>true</c> to select parent ListBoxItem at expanded; otherwise, <c>false</c></param>
		public static void SetSelectParentListBoxItemAtExpanded(DependencyObject obj, bool value) {
			obj.SetValue(SelectParentListBoxItemAtExpandedProperty, value);
		}

		private static void AtSelectParentListBoxItemAtExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var expander = d as Expander;
			if(expander==null) return;

			if((bool)e.NewValue) {
				expander.Expanded += AtExpanded;
			} else {
				expander.Expanded -= AtExpanded;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private static void AtExpanded(object sender, RoutedEventArgs e) {
			var elmt = (DependencyObject)sender;

			while(true) {
				elmt=VisualTreeHelper.GetParent(elmt);
				if(elmt is ListBoxItem) {
					((ListBoxItem)elmt).IsSelected = true;
					return;
				}
				if(elmt==null)return;
				//Debug.WriteLine(elmt.GetType().Name);
			}
		}


	}
}
