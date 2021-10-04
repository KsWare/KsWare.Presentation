using System;
using System.Windows;

namespace KsWare.Presentation {

	/// <summary> Provides arguments for the ValueChanged event
	/// </summary>
	public class ValueChangedEventArgs : EventArgs {
		private readonly object _oldValue;
		private readonly object _newValue;

		/// <summary> Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="newValue">The new value or <see cref="DependencyProperty.UnsetValue"/>.</param>
		/// <param name="oldValue">The old value or <see cref="DependencyProperty.UnsetValue"/>.</param>
		public ValueChangedEventArgs(object newValue, object oldValue) {
			_oldValue = oldValue;
			_newValue = newValue;
		}

		/// <summary> Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="newValue">The new value. or <see cref="DependencyProperty.UnsetValue"/></param>
		public ValueChangedEventArgs(object newValue) {
			_oldValue = DependencyProperty.UnsetValue;
			_newValue = newValue;
		}

		protected ValueChangedEventArgs(ValueChangedEventArgs e) {
			_oldValue = e._oldValue;
			_newValue = e._newValue;
		}

		/// <summary>Initializes a new instance of the <see cref="ValueChangedEventArgs" /> class.</summary>
		public ValueChangedEventArgs() {
			_oldValue = DependencyProperty.UnsetValue;
			_newValue = DependencyProperty.UnsetValue;
		}

		/// <summary>Initializes a new instance of the <see cref="T:KsWare.Presentation.ValueChangedEventArgs" /> class from <see cref="DependencyPropertyChangedEventArgs"/>.</summary>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		public ValueChangedEventArgs(DependencyPropertyChangedEventArgs e) {
			_oldValue = e.OldValue;
			_newValue = e.NewValue;
		}

		/// <summary> Gets the old value or <c>null</c> if old value is unknown.
		/// </summary>
		/// <value>The old value or <c>null</c>.</value>
		public object OldValue => OldValueSpecified ? _oldValue : null;

		/// <summary> Gets a value whether the <see cref="OldValue"/>-property is specified.
		/// </summary>
		/// <value><c>true</c> the <see cref="OldValue"/> is specified; else <c>false</c>.</value>
		public bool OldValueSpecified => OldValue != DependencyProperty.UnsetValue;

		/// <summary> Gets the new value or <c>null</c> if new value is unknown.
		/// </summary>
		/// <value>The new value or <c>null</c>.</value>
		public object NewValue => NewValueSpecified ? _newValue : null;

		/// <summary> Gets a value whether the <see cref="NewValue"/>-property is specified.
		/// </summary>
		/// <value><c>true</c> the <see cref="NewValue"/> is specified; else <c>false</c>.</value>
		public bool NewValueSpecified => NewValue != DependencyProperty.UnsetValue;

		public static ValueChangedEventArgs ConvertFrom<T>(DependencyPropertyChangedEventArgs e) {
			return new ValueChangedEventArgs(e);
		}

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

		private ValueChangedEventArgs(ValueChangedEventArgs e) : base(e) {
			// type validation
			_ = e.NewValueSpecified ? (T)e.NewValue : default(T);
			_ = e.OldValueSpecified ? (T)e.OldValue : default(T);
		}

		/// <summary> Gets the previous value
		/// </summary>
		/// <value>The previous data or <see cref="DependencyProperty.UnsetValue"/>.</value>
		public new T OldValue => OldValueSpecified ? (T)base.OldValue : default(T);

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value</value>
		public new T NewValue => NewValueSpecified ? (T)base.NewValue : default(T);

		public static ValueChangedEventArgs<T> ConvertFrom(ValueChangedEventArgs e) {
			return new ValueChangedEventArgs<T>(e);
		}
	}

	public delegate void ValueChangedEventHandler<TValue>(object sender, ValueChangedEventArgs<TValue> e);

	public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
}