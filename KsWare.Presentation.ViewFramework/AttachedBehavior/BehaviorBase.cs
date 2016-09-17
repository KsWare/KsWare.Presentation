using System;
using System.Reflection;
using System.Windows;


namespace KsWare.Presentation.ViewFramework.AttachedBehavior {

	public class BehaviorBase<TAssociatedObject> where TAssociatedObject:DependencyObject {

		private static readonly DependencyProperty BehaviorProperty = DependencyProperty.RegisterAttached(
			"Behavior", typeof (BehaviorBase<TAssociatedObject>), typeof (BehaviorBase<TAssociatedObject>), new PropertyMetadata(null));

		protected static void SetBehavior(DependencyObject element, BehaviorBase<TAssociatedObject> value) { element.SetValue(BehaviorProperty, value); }

		protected static BehaviorBase<TAssociatedObject> GetBehavior(DependencyObject element) { return (BehaviorBase<TAssociatedObject>) element.GetValue(BehaviorProperty); }


		public TAssociatedObject AssociatedObject { get; set; }

		protected BehaviorBase(TAssociatedObject dependencyObject) { AssociatedObject = dependencyObject; }

		protected static T Attach<T>(DependencyObject d) where T:BehaviorBase<TAssociatedObject> {
			var b = GetBehavior(d);
			if (b == null) {
				b = (T)Activator.CreateInstance(typeof(T),BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,new object[] {d},null);
				SetBehavior(d, b);
				b.OnAttached();
			}
			return (T) b;
		}

		protected virtual void OnAttached() {
			
		}

	}

}