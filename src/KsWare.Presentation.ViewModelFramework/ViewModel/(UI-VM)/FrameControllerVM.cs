using System;
using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewModelFramework {

	public class FrameControllerVM : UIElementControllerVM<Frame> {

		public FrameControllerVM() {
			RegisterChildren(()=>this);
		}

		/// <inheritdoc cref="Frame.Navigate(Uri)"/>
		public void Navigate(Uri source) => UIElement.Navigate(source);

		public void Navigate(FrameworkElement content) => UIElement.Navigate(content);

		public void Navigate(IObjectVM source) {
			if (source == null) {
				UIElement.Navigate(null);
			}
			else {
				var viewType=ViewResolver.Default.Resolve(source.GetType());
				var view=(FrameworkElement)Activator.CreateInstance(viewType);
				view.DataContext = source;
				UIElement.Navigate(view);				
			}
		}

	}
}
