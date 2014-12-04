using System;
using System.ComponentModel;

namespace KsWare.Presentation {

	/// <summary> [DRAFT]
	/// </summary>
	public class DataBindingAttribute:Attribute {

		public DataBindingAttribute() {}

		public DataBindingAttribute(string source, BindingMode mode, UpdateSourceTrigger updateSourceTrigger) {
			Source = source;
			Mode = mode;
			UpdateSourceTrigger = updateSourceTrigger;
		}
		public DataBindingAttribute(Type sourceType, string source, BindingMode mode, UpdateSourceTrigger updateSourceTrigger) {
			SourceType = sourceType;
			Source = source;
			Mode = mode;
			UpdateSourceTrigger = updateSourceTrigger;
		}

		public DataBindingAttribute(string source, BindingMode mode) {
			Source = source;
			Mode = mode;
		}
		public DataBindingAttribute(Type sourceType, string source, BindingMode mode) {
			SourceType = sourceType;
			Source = source;
			Mode = mode;
		}

		public string Source { get; set; }
		public Type SourceType { get; set; }
		public BindingMode Mode { get; set; }
		public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
	}

	/// <seealso cref="System.Windows.Data.BindingMode"/>
	public enum BindingMode {Default,OneTime,OneWay,OneWayToSource,TwoWay}

	/// <summary> [EXPERIMENTAL] Describes the timing of binding source updates.
	/// </summary>
	/// <seealso cref="System.Windows.Data.UpdateSourceTrigger"/>
	public enum UpdateSourceTrigger {

		Default,

		/// <summary> Updates the binding source only when you call the <see cref="UpdateSource"/> method.
		/// </summary>
		Explicit,

		/// <summary> Updates the binding source immediately whenever the binding target property changes.
		/// </summary>
		PropertyChanged

		/*,LostFocus*/
	}
}