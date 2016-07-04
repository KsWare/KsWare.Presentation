using System;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderStringExtension:IEditValueProviderExtension {
		string Value { get; set; }
	}

	partial class EditValueProvider {

		class StringExtension : Extension, IEditValueProviderStringExtension {

			private string        _String             ;
			private bool          _StringIsInitialized;

			public StringExtension(EditValueProvider provider) : base(provider) {}

			/// <summary> Gets or sets the editable value as string.
			/// </summary>
			/// <value>The editable value.</value>
			public string Value {
				get {
					if(!_StringIsInitialized) {
						UpdateValue(false);
						_StringIsInitialized = UpdateValue(false);
					}
					return _String;
				}
				set {
					if (Equals(_String, value)) return;
					_String = value; _StringIsInitialized=true;

					OnPropertyChanged(nameof(Value));
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
					newValue = Provider.TypeConverter.ConvertTo(_String, ViewModel.ValueType); //throws an exception if conversation failed
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

				if (Equals(newStringValue, _String)) return false;
				_String = newStringValue;
				if (raiseEvents) OnPropertyChanged(nameof(Value)); 
				return true;
			}
		}

	}
}
