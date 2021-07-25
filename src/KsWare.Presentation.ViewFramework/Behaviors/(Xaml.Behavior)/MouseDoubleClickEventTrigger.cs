using System;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	public class MouseDoubleClickEventTrigger: EventTrigger {

		protected override void OnEvent(EventArgs eventArgs) {
			var e = eventArgs as MouseButtonEventArgs;
			if (e == null) return;
			if (e.ClickCount == 2) base.OnEvent(eventArgs);
		}		

	}
}
