using System;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides arguments for the BusinessPropertyChanged event
	/// </summary>
	public class BusinessPropertyChangedEventArgs: EventArgs
	{
		private readonly string m_PropertyName;

		public BusinessPropertyChangedEventArgs(string propertyName) {
			m_PropertyName = propertyName;
		}

		public string PropertyName {
			get { return m_PropertyName; }
		}
	}
}