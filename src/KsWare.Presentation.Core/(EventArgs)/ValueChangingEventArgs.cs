using System;

namespace KsWare.Presentation {

	/// <summary> Provides arguments for the XxxChanging event
	/// </summary>
	public class ValueChangingEventArgs: EventArgs {

		/// <summary> Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousValue">The previous value</param>
		/// <param name="newValue">The new value.</param>
		public ValueChangingEventArgs(object previousValue, object newValue) {
			PreviousValue          = previousValue;
			PreviousValueSpecified = true;//!=DependencyProperty.UnsetValue;
			NewValue               = newValue;
			CanCancel              = false;
		}

		public ValueChangingEventArgs(object newValue) {
			PreviousValue          = null;//DependencyProperty.UnsetValue
			PreviousValueSpecified = false;
			NewValue               = newValue;
			CanCancel              = false;
		}

		public ValueChangingEventArgs(object previousValue, object newValue, bool canCancel) {
			PreviousValue          = previousValue;
			PreviousValueSpecified = true;//!=DependencyProperty.UnsetValue;
			NewValue               = newValue;
			CanCancel              = canCancel;
		}

		/// <summary> Gets the previous value
		/// </summary>
		/// <value>The previous data</value>
		public object PreviousValue { get; private set; }

		/// <summary> Gets a value wether the previous value has been specified.
		/// </summary>
		/// <value><see langword="true"/> if the previous value has been specified; else <see langword="false"/></value>
		public bool PreviousValueSpecified { get; private set; }

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value.</value>
		public object NewValue { get; private set;  }

		public bool CanCancel{get ; private set; }

		public bool Cancel { get; set; }

		public string Cause { get; set; }
	}

	/// <summary> Provides arguments for the XxxChanging event
	/// </summary>
	public class ValueChangingEventArgs<T> : ValueChangingEventArgs {

		/// <summary> Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
		/// </summary>
		/// <param name="previousValue">The previous value</param>
		/// <param name="newValue">The new value.</param>
		public ValueChangingEventArgs(T previousValue, object newValue)
			:base(previousValue,newValue) {}

		public ValueChangingEventArgs(T newValue)
			:base(newValue) {}

		public ValueChangingEventArgs(T previousValue, T newValue, bool canCancel)
			:base(previousValue,newValue,canCancel) {}

		/// <summary> Gets the previous value
		/// </summary>
		/// <value>The previous data</value>
		public new T PreviousValue { get { return (T)base.PreviousValue; }}

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value.</value>
		public new T NewValue { get { return (T)base.NewValue; }  }
	}

}
