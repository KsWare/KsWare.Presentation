using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	public static class ToolTipBehavior {

		public static readonly DependencyProperty CustomHandlerProperty = DependencyProperty.RegisterAttached(
			"CustomHandler", typeof (ICustomToolTipBehavior), typeof (ToolTipBehavior), new PropertyMetadata(null,AtCustomHandlerChanged));

		public static void SetCustomHandler(ToolTip element, ICustomToolTipBehavior value) { element.SetValue(CustomHandlerProperty, value); }

		public static ICustomToolTipBehavior GetCustomHandler(ToolTip element) { return (ICustomToolTipBehavior) element.GetValue(CustomHandlerProperty); }

		private static void AtCustomHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var toolTip = (ToolTip) d;
			var newValue=(ICustomToolTipBehavior)e.NewValue;
			if(newValue==null /*unexpected*/) return;
			newValue.Connect(toolTip);
			//TODO kux Null ref newValue
		}

//		static ToolTipBehavior() {
//			var frameworkPropertyMetadata = new FrameworkPropertyMetadata(ToolTipService.InitialShowDelayProperty.DefaultMetadata.DefaultValue,FrameworkPropertyMetadataOptions.Inherits);
//			ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof (FrameworkElement),frameworkPropertyMetadata);
//		}

	}

	public interface ICustomToolTipBehavior {

		void Connect(ToolTip toolTip);

	}

}
