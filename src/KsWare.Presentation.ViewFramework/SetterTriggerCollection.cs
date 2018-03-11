using System.Collections.Generic;
using System.Windows;

namespace KsWare.Presentation.ViewFramework {

	public class SetterTriggerCollection:List<TriggerBase> {}

	public static class StyleSetter {

		public static readonly DependencyProperty TriggersProperty = DependencyProperty.RegisterAttached(
			"Triggers", typeof (SetterTriggerCollection), typeof (StyleSetter), new PropertyMetadata(null));

		public static void SetTriggers(FrameworkElement frameworkElement, IEnumerable<TriggerBase> collection) {
			frameworkElement.SetValue(TriggersProperty, collection);

			//frameworkElement.Triggers.Clear();
			foreach (var trigger in collection) 
				frameworkElement.Triggers.Add(trigger);
		}

		public static SetterTriggerCollection GetTriggers(FrameworkElement frameworkElement) {
			return (SetterTriggerCollection) frameworkElement.GetValue(TriggersProperty);
		}
	}
}
