using System.Collections;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior 
{

	/// <summary>
	/// Interface for BehaviorBindingCollection
	/// </summary>
	public interface IBehaviorBindingCollection:IEnumerable 
	{
		/// <summary> Gets or sets the Owner of the binding
		/// </summary>
		DependencyObject Owner { get; set; }
	}

//	/// <summary> Collection to store the list of behaviors. 
//	/// </summary>
//	/// <remmarks>
//	/// This is done so that you can intiniate it from XAML
//	/// This inherits from freezable so that it gets inheritance context for DataBinding to work
//	/// </remarks>
//	public class BehaviorBindingCollection : FreezableCollection<BehaviorBinding>,IBehaviorBindingCollection {
//		/// <summary> Gets or sets the Owner of the binding
//		/// </summary>
//		public DependencyObject Owner { get; set; }
//	}

	/// <summary> Collection to store the list of behaviors. 
	/// </summary>
	/// <remarks>
	/// This is done so that you can intiniate it from XAML
	/// This inherits from freezable so that it gets inheritance context for DataBinding to work
	/// </remarks>
	public class BehaviorBindingCollection<T> : FreezableCollection<T>,IBehaviorBindingCollection where T:DependencyObject
	{
		/// <summary> Gets or sets the Owner of the binding
		/// </summary>
		public DependencyObject Owner { get; set; }
	}

	/// <summary> Collection to store the list of event behaviors. 
	/// </summary>
	/// <remarks>
	/// This is done so that you can intiniate it from XAML
	/// This inherits from freezable so that it gets inheritance context for DataBinding to work
	/// </remarks>
	public class EventBindingCollection : BehaviorBindingCollection<EventBinding> {}

	/// <summary> Collection to store the list of input behaviors. 
	/// </summary>
	/// <remarks>
	/// This is done so that you can intiniate it from XAML
	/// This inherits from freezable so that it gets inheritance context for DataBinding to work
	/// </remarks>
	public class InputBindingCollection : BehaviorBindingCollection<InputBinding> {}
}