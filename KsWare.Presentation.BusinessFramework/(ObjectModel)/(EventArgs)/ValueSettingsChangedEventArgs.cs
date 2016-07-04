using System;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides data for ValueSettingsChanged-event
	/// </summary>
	public class ValueSettingsChangedEventArgs: EventArgs {

		private readonly ValueSettingName _PropertyName;

		/// <summary> Initializes a new instance of the <see cref="ValueSettingsChangedEventArgs"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the changed property.</param>
		public ValueSettingsChangedEventArgs(ValueSettingName propertyName) { _PropertyName = propertyName; }

		/// <summary> Gets the name of the changed property.
		/// </summary>
		/// <value>The name of the changed property.</value>
		public ValueSettingName PropertyName{get {return _PropertyName;}}
	}
}