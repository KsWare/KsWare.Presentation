using System.Windows.Controls.Primitives;

namespace KsWare.Presentation.ViewModelFramework {

	public class RangeBaseVM<T> : UIElementVM<T> where T:RangeBase {

		/// <summary> Gets or sets the highest possible Value of the range element
		/// </summary>
		public double Maximum { get { return Fields.Get<double>("Maximum"); } set { Fields.Set("Maximum", value); } }
		
		/// <summary> Gets or sets the Minimum possible Value of the range element.
		/// </summary>
		public double Minimum { get { return Fields.Get<double>("Minimum"); } set { Fields.Set("Minimum", value); } }
		
		/// <summary> Gets or sets the current magnitude of the range control.
		/// </summary>
		public double Value { get { return Fields.Get<double>("Value"); } set { Fields.Set("Value", value); } }
		
		/// <summary> Gets or sets a value to be added to or subtracted from the Value of a RangeBase control. 
		/// </summary>
		public double LargeChange { get { return Fields.Get<double>("LargeChange"); } set { Fields.Set("LargeChange", value); } }
		
		/// <summary> Gets or sets a Value to be added to or subtracted from the Value of a RangeBase control
		/// </summary>
		public double SmallChange { get { return Fields.Get<double>("SmallChange"); } set { Fields.Set("SmallChange", value); } }
	}

	public class ScrollBarVM : RangeBaseVM<ScrollBar> {

		/// <summary> Gets or sets the amount of the scrollable content that is currently visible. 
		/// </summary>
		public double ViewportSize { get { return Fields.Get<double>("ViewportSize"); } set { Fields.Set("ViewportSize", value); } }

	}
}