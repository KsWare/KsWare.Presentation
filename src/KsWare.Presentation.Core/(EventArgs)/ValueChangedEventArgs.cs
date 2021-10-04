using System;
using System.Windows;

namespace KsWare.Presentation {

	/// <summary> Provides arguments for the XxxChanged event
	/// </summary>
	public class ValueChangedEventArgs : EventArgs {

		/// <summary> Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="newValue">The new value or <see cref="DependencyProperty.UnsetValue"/>.</param>
		/// <param name="oldValue"></param>
		public ValueChangedEventArgs(object newValue, object oldValue) {
			OldValue = oldValue;
			NewValue = newValue;
		}

		/// <summary> Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="newValue">The new value. or <see cref="DependencyProperty.UnsetValue"/></param>
		public ValueChangedEventArgs(object newValue) {
			OldValue = DependencyProperty.UnsetValue;
			NewValue = newValue;
		}

		public ValueChangedEventArgs() {
			OldValue = DependencyProperty.UnsetValue;
			NewValue = DependencyProperty.UnsetValue;
		}

		public ValueChangedEventArgs(DependencyPropertyChangedEventArgs e) {
			OldValue = e.OldValue;
			NewValue      = e.NewValue;
		}

		/// <summary> Gets the previous value or <see cref="DependencyProperty.UnsetValue"/> if previous value is unknown
		/// </summary>
		/// <value>The previous data or <see cref="DependencyProperty.UnsetValue"/>.</value>
		public object OldValue { get; private set; }


		/// <summary> Gets a value whether the <see cref="OldValue"/>-property is specified.
		/// </summary>
		/// <value>The previous value specified.</value>
		public bool PreviousValueSpecified => OldValue != DependencyProperty.UnsetValue;

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value or <see cref="DependencyProperty.UnsetValue"/></value>
		public object NewValue { get; private set; }

		/// <summary> Gets a value whether the <see cref="NewValue"/>-property is specified.
		/// </summary>
		/// <value>The new value is specified.</value>
		public bool NewValueSpecified => OldValue != DependencyProperty.UnsetValue;

		public static ValueChangedEventArgs<T> CreateFrom<T>(T newValue) {
			return new ValueChangedEventArgs<T>(newValue);
		}

		public static ValueChangedEventArgs<T> CreateFrom<T>(T newValue, T oldValue) {
			return new ValueChangedEventArgs<T>(newValue, oldValue);
		}
	}

	public sealed class ValueChangedEventArgs<T> : ValueChangedEventArgs {

		public ValueChangedEventArgs() : base() { }

		public ValueChangedEventArgs(T newValue, T oldValue) : base(newValue, oldValue) {}

		public ValueChangedEventArgs(T newValue) : base(newValue) { }

		public ValueChangedEventArgs(DependencyPropertyChangedEventArgs e) : base(e) {}

		/// <summary> Gets the previous value
		/// </summary>
		/// <value>The previous data or <see cref="DependencyProperty.UnsetValue"/>.</value>
		public new T PreviousValue => PreviousValueSpecified ? (T)base.OldValue : default(T);

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value</value>
		public new T NewValue => NewValueSpecified ? (T)base.NewValue : default(T);

		public static ValueChangedEventArgs<T> CreateFrom(ValueChangedEventArgs e) {
			if (e.PreviousValueSpecified) return new ValueChangedEventArgs<T>((T)e.NewValue, (T)e.OldValue);
			if (e.NewValueSpecified) return new ValueChangedEventArgs<T>((T)e.NewValue);
			return new ValueChangedEventArgs<T>();
		}
	}

	public delegate void ValueChangedEventHandler<TValue>(object sender, ValueChangedEventArgs<TValue> e);

	public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
}