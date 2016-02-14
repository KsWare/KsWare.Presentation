using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderHexNumberExtension:IEditValueProviderExtension {
		string Value { get; set; }
	}

	partial class EditValueProvider {

		private class HexNumberExtension:Extension,IEditValueProviderHexNumberExtension {

			private bool m_HexNumberIsInitialized;
			private string m_HexNumber;
			private bool m_HexNumberValueGetIsUsed;

			public HexNumberExtension(EditValueProvider provider) : base(provider) {}

			public string Value {
				get {
					if (!m_HexNumberIsInitialized) { UpdateValue(false); m_HexNumberIsInitialized=true;}
					return m_HexNumber;
				}
				set {
					if(Equals(m_HexNumber,value)) return;
					m_HexNumber = value;
					OnPropertyChanged("Value");

					object newValue; bool hasError;byte b;UInt16 i16;UInt32 i32;UInt64 i64;
					switch (Type.GetTypeCode(ViewModel.ValueType)) {
						case TypeCode.SByte : hasError=!Byte  .TryParse(m_HexNumber,NumberStyles.HexNumber,null,out b  ); newValue=unchecked((SByte)b  ); break;
						case TypeCode.Int16 : hasError=!UInt16.TryParse(m_HexNumber,NumberStyles.HexNumber,null,out i16); newValue=unchecked((Int16)i16); break;
						case TypeCode.Int32 : hasError=!UInt32.TryParse(m_HexNumber,NumberStyles.HexNumber,null,out i32); newValue=unchecked((Int32)i32); break;
						case TypeCode.Int64 : hasError=!UInt64.TryParse(m_HexNumber,NumberStyles.HexNumber,null,out i64); newValue=unchecked((Int64)i64); break;
						case TypeCode.Byte  : hasError=!Byte  .TryParse(m_HexNumber,NumberStyles.HexNumber,null,out b  ); newValue=                 b   ; break;
						case TypeCode.UInt16: hasError=!UInt16.TryParse(m_HexNumber,NumberStyles.HexNumber,null,out i16); newValue=                 i16 ; break;
						case TypeCode.UInt32: hasError=!UInt32.TryParse(m_HexNumber,NumberStyles.HexNumber,null,out i32); newValue=                 i32 ; break;
						case TypeCode.UInt64: hasError=!UInt64.TryParse(m_HexNumber,NumberStyles.HexNumber,null,out i64); newValue=                 i64 ; break;
						default: throw new InvalidOperationException("HexNumber is not supported for this view model! ErrorID:{4B092978-71FA-4754-A17C-35987E9B6256}");
					}

					if (hasError) SetError("Value", "Invalid value");else ResetError("Value");
					if(!hasError) UpdateSource(newValue);
				}
			}

			private void UpdateSource(object value) {
				if(HasError)return;
				ViewModel.Value = value;
			}


			internal bool UpdateValue(bool raiseEvents) {
				if (!ViewModel.HasValue) {
					m_HexNumber = null;
				} else if (IsInteger(ViewModel.Value)) {
					m_HexNumber = string.Format("{0:X}", ViewModel.Value); //TODO use StringFormat
				} else {
					m_HexNumber = null;
				}

				ResetError("Value");
			
				if(raiseEvents) {
					OnPropertyChanged("Value");
				}
				return true;
			}
		}

	}
}
