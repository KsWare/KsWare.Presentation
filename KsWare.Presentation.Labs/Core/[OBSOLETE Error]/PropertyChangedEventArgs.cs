using System;
using System.Windows;

namespace KsWare.Presentation {

	[Obsolete("Use ValueChangedEventArgs",true)]
	public sealed class PropertyChangedEventArgs<T> : EventArgs {

		public PropertyChangedEventArgs() {
			OldValueSpecified = false;
			NewValueSpecified = false;
		}

		public PropertyChangedEventArgs(T oldValue, T newValue) {
			OldValue = oldValue;	
			OldValueSpecified = true;
			NewValue= newValue;
			NewValueSpecified = true;
		}

		public PropertyChangedEventArgs(DependencyPropertyChangedEventArgs e) {
			OldValue = (T)e.OldValue;
			OldValueSpecified = true;
			NewValue= (T)e.NewValue;
			NewValueSpecified = true;
		}

		public T OldValue { get; private set; }

		public bool OldValueSpecified { get; private set; }

		public T NewValue { get; private set; }

		public bool NewValueSpecified { get; private set; }
	}
}
