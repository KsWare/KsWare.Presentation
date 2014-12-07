using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	// FrameworkElement.PreviewKeyDown += ObjectVM.UI.EventConnector.PreviewKeyDown

	public class ViewModelBehavior {

		public static readonly DependencyProperty HandlePreviewKeyDownProperty = DependencyProperty.RegisterAttached("HandlePreviewKeyDown", 
			typeof(bool), typeof(ViewModelBehavior), new PropertyMetadata(default(bool),AtHandlePreviewKeyDownChanged));

		public static bool GetHandlePreviewKeyDown(UIElement element) {
			return (bool)element.GetValue(HandlePreviewKeyDownProperty);
		}

		public static void SetHandlePreviewKeyDown(UIElement element, bool value) {
			element.SetValue(HandlePreviewKeyDownProperty, value);
		}

		private static readonly DependencyPropertyKey IsDataContextChangedAttachedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsDataContextChangedAttached", 
			typeof(bool), typeof(ViewModelBehavior), new PropertyMetadata(default(bool)));


		private static void AtHandlePreviewKeyDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var elmt = (FrameworkElement)d;

			if(!(bool)elmt.GetValue(IsDataContextChangedAttachedPropertyKey.DependencyProperty)) {
				elmt.DataContextChanged+=AtDataContextChanged;
				elmt.SetValue(IsDataContextChangedAttachedPropertyKey,true);
			}

			elmt.Focusable = true;

			if(elmt.DataContext!=null) {
				if(GetHandlePreviewKeyDown(elmt)) {
					var o = elmt.DataContext as ObjectVM;
					if(o!=null) {
						elmt.PreviewKeyDown += o.UI.EventConnector.PreviewKeyDown;
					}
				} else {
					var o = elmt.DataContext as ObjectVM;
					if(o!=null) {
						elmt.PreviewKeyDown -= o.UI.EventConnector.PreviewKeyDown;
					}
				}
			}
		}

		private static void AtDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var elmt = (FrameworkElement)sender;
			var oldValue = e.OldValue as ObjectVM;
			var newValue = e.NewValue as ObjectVM;

			if(oldValue!=null) {
				elmt.PreviewKeyDown -= oldValue.UI.EventConnector.PreviewKeyDown;
			}
			if(newValue!=null) {
				if(GetHandlePreviewKeyDown(elmt)) {
					elmt.PreviewKeyDown+=newValue.UI.EventConnector.PreviewKeyDown;
					elmt.Focus();//HACK
				}
			}
		}

		private static Window GetWindow(DependencyObject elmt) {
			var p = elmt;
			while(p!=null) {
				if(p is Window) return (Window)p;
				p = VisualTreeHelper.GetParent(p);
			}
			return null;
		}

	}
}
