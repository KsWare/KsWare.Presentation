using System;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides arguments for the BusinessPropertyChanged event
	/// </summary>
	public class BusinessPropertyChangedEventArgs: EventArgs
	{
		private readonly string _PropertyName;

		public BusinessPropertyChangedEventArgs(string propertyName) {
			_PropertyName = propertyName;
		}

		public string PropertyName {
			get { return _PropertyName; }
		}
	}
}