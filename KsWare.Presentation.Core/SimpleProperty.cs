using System.ComponentModel;
using KsWare.Presentation;

namespace KsWare.Presentation.Core
{
	public class SimpleProperty<T>
	{
		private T _value;
//		private string _propertyId;
//		private string _name;

		public SimpleProperty() {}

		public T Value {
			get { return _value; }
			set {
				if(Equals(_value,value)) return;
				_value = value;
				EventUtil.Raise(ValueChanged,this,new PropertyChangedEventArgs("Value"),"{74391406-BAB2-4F26-8333-73E6423D8402}");
			}
		}

		public event PropertyChangedEventHandler ValueChanged;
	}
}
