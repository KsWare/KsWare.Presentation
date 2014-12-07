using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	public class BusyAdornerVisual:UserControl {

		static BusyAdornerVisual() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof (BusyAdornerVisual), new FrameworkPropertyMetadata(typeof (BusyAdornerVisual)));
		}
	}
}
