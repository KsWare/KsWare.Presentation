using System.Windows.Controls.Primitives;

namespace KsWare.Presentation.ViewModelFramework {

	public class RangeBaseVM<T> : UIElementVM<T> where T:RangeBase {

		/// <summary> Gets or sets the highest possible Value of the range element
		/// </summary>
		public double Maximum { get => Fields.GetValue<double>(); set => Fields.SetValue(value); }
		
		/// <summary> Gets or sets the Minimum possible Value of the range element.
		/// </summary>
		public double Minimum { get => Fields.GetValue<double>(); set => Fields.SetValue(value); }
		
		/// <summary> Gets or sets the current magnitude of the range control.
		/// </summary>
		public double Value { get => Fields.GetValue<double>(); set => Fields.SetValue(value); }
		
		/// <summary> Gets or sets a value to be added to or subtracted from the Value of a RangeBase control. 
		/// </summary>
		public double LargeChange { get => Fields.GetValue<double>(); set => Fields.SetValue(value); }
		
		/// <summary> Gets or sets a Value to be added to or subtracted from the Value of a RangeBase control
		/// </summary>
		public double SmallChange { get => Fields.GetValue<double>(); set => Fields.SetValue(value); }
	}

	public class ScrollBarVM : RangeBaseVM<ScrollBar> {

		/// <summary> Gets or sets the amount of the scrollable content that is currently visible. 
		/// </summary>
		public double ViewportSize { get => Fields.GetValue<double>(); set => Fields.SetValue(value); }

	}
}