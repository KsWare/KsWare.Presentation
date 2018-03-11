using System;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderNullableBoolExtension:IEditValueProviderExtension {

	}

	partial class EditValueProvider {

		class NullableBoolExtension : Extension, IEditValueProviderNullableBoolExtension {

			private bool? _value;
			private bool _valueIsInitialized;

			public NullableBoolExtension(EditValueProvider provider) : base(provider) {}

			/// <summary> Gets or sets the editable value as <see cref="Nullable{Boolean}"/>.
			/// </summary>
			/// <value>The editable value.</value>
			/// <remarks>Use <see cref="BoolNullable"/> to bind e.g. <see cref="CheckBox.IsChecked"/> property</remarks>
			public bool? Value {
				get {
					if (!_valueIsInitialized) {UpdateValue(false);_valueIsInitialized=true;}
					return _value;
				}
				set {
					if(Equals(_value,value)) return;
					_value = value;
					_valueIsInitialized = true;
					UpdateSource();
				}
			}

			private void UpdateSource() {
				if (ViewModel.ValueType==typeof(bool)) {
					if (_value == null) UpdateSource(false);
					else UpdateSource(_value);
				} else {
					throw new InvalidOperationException("NullableBool is not supported for this view model! ErrorID:{9410E750-C087-46FD-B06A-2C0F45DFC33F}");
				}
			}

			private void UpdateSource(object value) {
				if(HasError)return;
				ViewModel.Value = value;
			}

			/// <summary> Updates <see cref="Value"/> and returns <c>true</c> if value has changes.
			/// </summary>
			/// <param name="raiseEvents">if set to <c>true</c> raise events.</param>
			/// <returns><c>true</c> if value has changes, <c>false</c> otherwise.</returns>
			internal bool UpdateValue(bool raiseEvents) {
				bool? newValue;

				if (ViewModel.ValueType == typeof (bool)) {
					newValue = (bool) ViewModel.Value;
				} else {
					throw new InvalidOperationException("NullableBool is not supported for this view model! ErrorID:{C8B4B237-5E97-40FC-A8A1-58A495F9A83E}");
				}
				
				if (Equals(_value, newValue)) return false;
				_value = newValue;
				if (raiseEvents) OnPropertyChanged(nameof(Value)); 
				return true;
			}

		}

	}
}
