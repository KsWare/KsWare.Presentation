using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KsWare.Presentation {

	/// <summary> Provides arguments for the XxxChanged event
	/// </summary>
	public class ValueChangedEventArgs : EventArgs {

		/// <summary> Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousValue">The previous value</param>
		/// <param name="newValue">The new value.</param>
		public ValueChangedEventArgs(object previousValue, object newValue) {
			PreviousValue = previousValue;
			NewValue = newValue;
		}

		/// <summary> Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="newValue">The new value.</param>
		public ValueChangedEventArgs(object newValue) {
			PreviousValue = DependencyProperty.UnsetValue;
			NewValue = newValue;
		}

		public ValueChangedEventArgs() {
			PreviousValue = DependencyProperty.UnsetValue;
			NewValue = DependencyProperty.UnsetValue;
		}

		public ValueChangedEventArgs(DependencyPropertyChangedEventArgs e) {
			PreviousValue = e.OldValue;
			NewValue      = e.NewValue;
		}

		/// <summary> Gets the previous value or <see cref="DependencyProperty.UnsetValue"/> if previous value is unknown
		/// </summary>
		/// <value>The previous data or <see cref="DependencyProperty.UnsetValue"/>.</value>
		public object PreviousValue { get; private set; }


		/// <summary> Gets a value wether the <see cref="PreviousValue"/>-property is specified.
		/// </summary>
		/// <value>The previous value specified.</value>
		public bool PreviousValueSpecified {get {return PreviousValue != DependencyProperty.UnsetValue;}}

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value or <see cref="DependencyProperty.UnsetValue"/></value>
		public object NewValue { get; private set; }

		/// <summary> Gets a value wether the <see cref="NewValue"/>-property is specified.
		/// </summary>
		/// <value>The new value is specified.</value>
		public bool NewValueSpecified {get {return PreviousValue != DependencyProperty.UnsetValue;}}
	}

	public sealed class ValueChangedEventArgs<T> : ValueChangedEventArgs {

		public ValueChangedEventArgs() {}
		public ValueChangedEventArgs(T oldValue, T newValue):base(oldValue,newValue) {}
		public ValueChangedEventArgs(T newValue):base(newValue) {}
		public ValueChangedEventArgs(DependencyPropertyChangedEventArgs e):base(e) {}

		/// <summary> Gets the previous value
		/// </summary>
		/// <value>The previous data or <see cref="DependencyProperty.UnsetValue"/>.</value>
		public new T PreviousValue { get { return PreviousValueSpecified?(T)base.PreviousValue:default(T); }}

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value</value>
		public new T NewValue { get { return (T) base.NewValue; }}
	}

	public delegate void ValueChangedEventHandler<TValue>(object sender, ValueChangedEventArgs<TValue> e);
	public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
}