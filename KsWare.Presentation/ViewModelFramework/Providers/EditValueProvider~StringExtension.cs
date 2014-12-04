using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderStringExtension:IEditValueProviderExtension {
		string Value { get; set; }
	}

	partial class EditValueProvider {

		class StringExtension : Extension, IEditValueProviderStringExtension {

			private string        m_String             ;
			private bool          m_StringIsInitialized;

			public StringExtension(EditValueProvider provider) : base(provider) {}

			/// <summary> Gets or sets the editable value as string.
			/// </summary>
			/// <value>The editable value.</value>
			public string Value {
				get {
					if(!m_StringIsInitialized) {
						UpdateValue(false);
						m_StringIsInitialized = UpdateValue(false);
					}
					return m_String;
				}
				set {
					if (Equals(m_String, value)) return;
					m_String = value; m_StringIsInitialized=true;

					OnPropertyChanged("Value");
					UpdateSource();
				}
			}

			/// <summary> Updates the source (using <see cref="m_String"/>).
			/// </summary>
			/// <remarks></remarks>
			private void UpdateSource() { // StringˑUpdateSource()
				if(ViewModel==null||Metadata==null){throw new InvalidOperationException("Provider is not initialized!"+ "\n\t" + "UniqueID: {99DAF505-92CB-4A61-AA15-8C3AC9195750}"); }

				object newValue;
				try {
					newValue = Provider.TypeConverter.ConvertTo(m_String, ViewModel.ValueType); //throws an exception if conversation failed
				} catch(Exception ex) {
					((IErrorProviderController)ViewModel.Metadata.ErrorProvider).SetError(ex.Message); //TODO localize message
					return;
				}

				Provider.UpdateSource(newValue);
			}

			/// <summary> Updates the <see cref="m_String"/> and returns <c>true</c> if value has changes.
			/// </summary>
			/// <param name="raiseEvents">if set to <c>true</c> raise events.</param>
			/// <returns><c>true</c> if value has changes, <c>false</c> otherwise.</returns>
			internal bool UpdateValue(bool raiseEvents) {
				string newStringValue;
				//TODO move special conversion to ValueProviderStringConverter
				if (!ViewModel.HasValue) {
					newStringValue = null;
				} else if (ViewModel.Value is Double) {
					var value = (double) ViewModel.Value;
					if (double.IsNaN(value)) newStringValue = "";
					else if(double.IsNegativeInfinity(value)) newStringValue = "-";
					else if(double.IsPositiveInfinity(value)) newStringValue = "+";
					else newStringValue = (string)Provider.TypeConverter.ConvertFrom(value);
				} else if (ViewModel.Value is Single) {
					var value = (float) ViewModel.Value;
					if (float.IsNaN(value)) newStringValue = "";
					else if(float.IsNegativeInfinity(value)) newStringValue = "-";
					else if(float.IsPositiveInfinity(value)) newStringValue = "+";
					else newStringValue = (string)Provider.TypeConverter.ConvertFrom(value);
				} else {
					newStringValue = (string)Provider.TypeConverter.ConvertFrom(ViewModel.Value);
				}

				if (Equals(newStringValue, m_String)) return false;
				m_String = newStringValue;
				if (raiseEvents) OnPropertyChanged("Value"); 
				return true;
			}
		}

	}
}
