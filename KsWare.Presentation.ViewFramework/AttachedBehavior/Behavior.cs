using System.Windows;
using System.Collections.Specialized;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior 
{

	/// <summary> Provides attached beavior
	/// </summary>
	public static class Behavior 
	{

		#region EventBindings

		/// <summary>
		/// EventBindings Dependency Property
		/// </summary>
		public static readonly DependencyProperty EventBindingsProperty = DependencyProperty.RegisterAttached(
			"EventBindings", typeof(EventBindingCollection), typeof(Behavior),
			new FrameworkPropertyMetadata(null,AtBindingsPropertyChanged)
		);

		/// <summary> Gets the EventBindings.  
		/// </summary>
		public static EventBindingCollection GetEventBindings(DependencyObject d) {
			return  (EventBindingCollection) d.GetValue(EventBindingsProperty);
		}

		/// <summary> Sets the EventBindings.  
		/// </summary>
		public static void SetEventBindings(DependencyObject d, EventBindingCollection value) {
			d.SetValue(EventBindingsProperty, value);
		}

		#endregion

		#region InputBindings

		/// <summary>
		/// InputBindings Dependency Property
		/// </summary>
		public static readonly DependencyProperty InputBindingsProperty = DependencyProperty.RegisterAttached(
			"InputBindings", typeof(InputBindingCollection), typeof(Behavior),
			new FrameworkPropertyMetadata(null,AtBindingsPropertyChanged)
		);

		/// <summary> Gets the InputBindings.  
		/// </summary>
		public static InputBindingCollection GetInputBindings(DependencyObject d) {
			return  (InputBindingCollection) d.GetValue(InputBindingsProperty);
		}

		/// <summary> Sets the InputBindings.  
		/// </summary>
		public static void SetInputBindings(DependencyObject d, InputBindingCollection value) {
			d.SetValue(InputBindingsProperty, value);
		}

		#endregion

		/// <summary> Invoked if the <see cref="EventBindingsProperty"/> or <see cref="InputBindingsProperty"/> has been changed
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void AtBindingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var oldValue = (IBehaviorBindingCollection) e.OldValue;
			var newValue = (IBehaviorBindingCollection) e.NewValue;

			if(oldValue==newValue)return;

			if(oldValue!=null) {
				((INotifyCollectionChanged)oldValue).CollectionChanged -= AtBindingCollectionChanged;
				oldValue.Owner = null;
				foreach (BehaviorBinding item in oldValue) item.Behavior.Dispose();
			}

			if(newValue!=null) {
				newValue.Owner = d;
				((INotifyCollectionChanged)newValue).CollectionChanged += AtBindingCollectionChanged;
				foreach (BehaviorBinding item in newValue) item.Owner = d;
			}
		}

		private static void AtBindingCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			var owner = ((IBehaviorBindingCollection)sender).Owner;

			//when an item(s) is removed we should Dispose the BehaviorBinding

			switch (e.Action) {
					//when an item(s) is added we need to set the Owner property implicitly
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems != null) 
						foreach (BehaviorBinding item in e.NewItems) 
							item.Owner = owner;
					break;
				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems != null)
						foreach (BehaviorBinding item in e.OldItems) 
							item.Behavior.Dispose();
					break;

				case NotifyCollectionChangedAction.Replace:
					//here we have to set the owner property to the new item and unregister the old item
					if (e.NewItems != null)
						foreach (BehaviorBinding item in e.NewItems) 
							item.Owner = owner;

					if (e.OldItems != null)
						foreach (BehaviorBinding item in e.OldItems) 
							item.Behavior.Dispose();
					break;

				case NotifyCollectionChangedAction.Reset:
					if (e.OldItems != null)
						foreach (BehaviorBinding item in e.OldItems) 
							item.Behavior.Dispose();
					break;

				case NotifyCollectionChangedAction.Move:
				default:
					break;
			}
		}

	}
}