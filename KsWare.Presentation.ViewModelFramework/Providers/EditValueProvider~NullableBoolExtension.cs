using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderNullableBoolExtension:IEditValueProviderExtension {

	}

	partial class EditValueProvider {

		class NullableBoolExtension : Extension, IEditValueProviderNullableBoolExtension {

			private bool? m_Value;
			private bool m_ValueIsInitialized;

			public NullableBoolExtension(EditValueProvider provider) : base(provider) {}

			/// <summary> Gets or sets the editable value as <see cref="Nullable{Boolean}"/>.
			/// </summary>
			/// <value>The editable value.</value>
			/// <remarks>Use <see cref="BoolNullable"/> to bind e.g. <see cref="CheckBox.IsChecked"/> property</remarks>
			public bool? Value {
				get {
					if (!m_ValueIsInitialized) {UpdateValue(false);m_ValueIsInitialized=true;}
					return m_Value;
				}
				set {
					if(Equals(m_Value,value)) return;
					m_Value = value;
					m_ValueIsInitialized = true;
					UpdateSource();
				}
			}

			private void UpdateSource() {
				if (ViewModel.ValueType==typeof(bool)) {
					if (m_Value == null) UpdateSource(false);
					else UpdateSource(m_Value);
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
				
				if (Equals(m_Value, newValue)) return false;
				m_Value = newValue;
				if (raiseEvents) OnPropertyChanged("Value"); 
				return true;
			}

		}

	}
}
