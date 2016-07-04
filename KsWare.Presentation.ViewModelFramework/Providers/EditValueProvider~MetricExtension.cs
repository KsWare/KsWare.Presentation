using System;
using System.Threading;

namespace KsWare.Presentation.ViewModelFramework.Providers {


	public interface IEditValueProviderMetricExtension:IEditValueProviderExtension {

		/// <summary> Gets or sets the editable value in micro(meter)
		/// </summary>
		/// <value>The micro(meter) value.</value>
		/// <remarks></remarks>
		string Micro { get; set; }

		/// <summary> Gets or sets the editable value in milli(meter)
		/// </summary>
		/// <value>The milli(meter) value.</value>
		/// <remarks></remarks>
		string Milli { get; set; }

		/// <summary> Gets or sets the editable value in centi(meter).
		/// </summary>
		/// <value>The centi(meter) value.</value>
		/// <remarks></remarks>
		string Centi { get; set;}

		/// <summary> Gets or sets the editable value in deci(meter)
		/// </summary>
		/// <value>The deci(meter) value.</value>
		/// <remarks></remarks>
		string Deci { get; set; }

		/// <summary> Gets or sets the editable value in kilo(meter)
		/// </summary>
		/// <value>The kilo(meter) value.</value>
		/// <remarks></remarks>
		string Kilo { get; set; }
	}


	partial class EditValueProvider {

		class MetricExtension : Extension, IEditValueProviderMetricExtension {

			private bool _stringIsInitialized;

			private string _micro;
			private string _milli;
			private string _centi;
			private string _value;
			private string _deci;
			private string _kilo;

			public MetricExtension(EditValueProvider provider) : base(provider) {}

			#region properties



			/// <summary> Gets or sets the editable value in micro(meter).
			/// </summary>
			/// <value>The micro(meter) value.</value>
			/// <remarks></remarks>
			public string Micro {
				get {
					if(!_stringIsInitialized) {
						UpdateValue(false);
						_stringIsInitialized = UpdateValue(false);
					}
					return _micro;
				}
				set {
	//				if (Equals(this.editValue, m)) return;
					_value = Convert(value, "µm", "m");
					_micro  = value;
					_milli  = Convert(value, "µm", "mm");;
					_centi  = Convert(value, "µm", "cm");
					_deci   = Convert(value, "µm", "dm");
					_kilo   = Convert(value, "µm", "km");
				
				
					OnPropertyChanged(nameof(Value));
					OnPropertyChanged(nameof(Kilo));
					OnPropertyChanged(nameof(Deci));
					OnPropertyChanged(nameof(Centi));
					OnPropertyChanged(nameof(Milli));
					OnPropertyChanged(nameof(Micro));

					UpdateSource();
				}
			}

			 /// <summary> Gets or sets the editable value in milli(meter).
			/// </summary>
			/// <value>The milli(meter) value.</value>
			/// <remarks></remarks>
			public string Milli {
				get {
					if(!_stringIsInitialized) {
						UpdateValue(false);
						_stringIsInitialized = UpdateValue(false);
					}
					return _milli;
				}
				set {
	//				if (Equals(this.editValue, m)) return;
					_value = Convert(value, "mm", "m");
					_micro  = Convert(value, "mm", "µm");
					_milli  = value;
					_centi  = Convert(value, "mm", "cm");
					_deci   = Convert(value, "mm", "dm");
					_kilo   = Convert(value, "mm", "km");
				
					OnPropertyChanged(nameof(Value));
					OnPropertyChanged(nameof(Kilo));
					OnPropertyChanged(nameof(Deci));
					OnPropertyChanged(nameof(Centi));
					OnPropertyChanged(nameof(Milli));
					OnPropertyChanged(nameof(Micro));
				
					UpdateSource();
				}
			}

			/// <summary> Gets or sets the editable value in centi(meter).
			/// </summary>
			/// <value>The centi(meter) value.</value>
			/// <remarks></remarks>
			public string Centi {
				get {
					if(!_stringIsInitialized) {
						UpdateValue(false);
						_stringIsInitialized = UpdateValue(false);
					}
					return _centi;
				}
				set {
	//				if (Equals(this.editValue, value)) return;
					_value = Convert(value, "cm", "m");
					_micro  = Convert(value, "cm", "µm");
					_milli  = Convert(value, "cm", "mm");
					_centi  = value;
					_deci   = Convert(value, "cm", "dm");
					_kilo   = Convert(value, "cm", "km");
				

					OnPropertyChanged(nameof(Value));
					OnPropertyChanged(nameof(Kilo));
					OnPropertyChanged(nameof(Deci));
					OnPropertyChanged(nameof(Centi));
					OnPropertyChanged(nameof(Milli));
					OnPropertyChanged(nameof(Micro));
				
					UpdateSource();
				}
			}

						/// <summary> Gets or sets the editable value in base(meter).
			/// </summary>
			/// <value>The base(meter) value.</value>
			/// <remarks></remarks>
			public string Value {
				get {
					if(!_stringIsInitialized) {
						UpdateValue(false);
						_stringIsInitialized = UpdateValue(false);
					}
					return _centi;
				}
				set {
	//				if (Equals(this.editValue, value)) return;
					_value = Convert(value, "m", "m");
					_micro  = Convert(value, "m", "µm");
					_milli  = Convert(value, "m", "mm");
					_centi  = Convert(value, "m", "cm");
					_deci   = Convert(value, "m", "dm");
					_kilo   = Convert(value, "m", "km");
				

					OnPropertyChanged(nameof(Value));
					OnPropertyChanged(nameof(Kilo));
					OnPropertyChanged(nameof(Deci));
					OnPropertyChanged(nameof(Centi));
					OnPropertyChanged(nameof(Milli));
					OnPropertyChanged(nameof(Micro));
				
					UpdateSource();
				}
			}

			/// <summary> Gets or sets the editable value in deci(meter).
			/// </summary>
			/// <value>The deci(meter) value.</value>
			/// <remarks></remarks>
			public string Deci {
				get {
					if(!_stringIsInitialized) {
						UpdateValue(false);
						_stringIsInitialized = UpdateValue(false);
					}
					return _centi;
				}
				set {
	//				if (Equals(this.editValue, value)) return;
					_value = Convert(value, "dm", "m");
					_milli  = Convert(value, "dm", "mm");
					_deci   = value;
					_centi  = Convert(value, "dm", "cm");
					_kilo   = Convert(value, "dm", "km");
					_micro  = Convert(value, "dm", "µm");

					OnPropertyChanged(nameof(Value));
					OnPropertyChanged(nameof(Kilo));
					OnPropertyChanged(nameof(Deci));
					OnPropertyChanged(nameof(Centi));
					OnPropertyChanged(nameof(Milli));
					OnPropertyChanged(nameof(Micro));
				
					UpdateSource();
				}
			}

			/// <summary> Gets or sets the editable value in kilo(meter).
			/// </summary>
			/// <value>The kilo(meter) value.</value>
			/// <remarks></remarks>
			public string Kilo {
				get {
					if(!_stringIsInitialized) {
						UpdateValue(false);
						_stringIsInitialized = UpdateValue(false);
					}
					return _centi;
				}
				set {
	//				if (Equals(this.editValue, value)) return;
					_micro  = Convert(value, "km", "µm");
					_milli  = Convert(value, "km", "mm");
					_value  = Convert(value, "km", "m");
					_centi  = Convert(value, "km", "cm");
					_deci   = Convert(value, "km", "dm");
					_kilo   = value;

					OnPropertyChanged(nameof(Value));
					OnPropertyChanged(nameof(Kilo));
					OnPropertyChanged(nameof(Deci));
					OnPropertyChanged(nameof(Centi));
					OnPropertyChanged(nameof(Milli));
					OnPropertyChanged(nameof(Micro));
				
					UpdateSource();
				}
			}

			#endregion properties

			/// <summary> Updates the source (using <see cref="m_Value"/>).
			/// </summary>
			/// <remarks></remarks>
			private void UpdateSource() {
				if(ViewModel==null||Metadata==null){throw new InvalidOperationException("Provider is not initialized!"+ "\n\t" + "UniqueID: {99DAF505-92CB-4A61-AA15-8C3AC9195750}"); }

				object newValue;
				try {
					newValue = Provider.TypeConverter.ConvertTo(_value, ViewModel.ValueType); //throws an exception if conversation failed
				} catch(Exception ex) {
					((IErrorProviderController)ViewModel.Metadata.ErrorProvider).SetError(ex.Message); //TODO localize message
					return;
				}

				Provider.UpdateSource(newValue);
			}

			internal bool UpdateValue(bool raiseEvents) {
				_kilo  = null;
				_deci  = null;
				_value = null;
				_centi = null;
				_milli = null;
				_micro = null;
	
				if (!ViewModel.HasValue) {
				
				} else if (ViewModel.Value is Double) {
					var value = (double) ViewModel.Value;
					var s = (string)Provider.TypeConverter.ConvertFrom(value);
					if (!double.IsNaN(value) && !double.IsInfinity(value)) {
						_kilo  = Convert(s, "m", "km");
						_deci  = Convert(s, "m", "dm");
						_value = s;
						_centi = Convert(s, "m", "cm");
						_milli = Convert(s, "m", "mm");
						_micro = Convert(s, "m", "µm");
					}else {
						_kilo=_deci=_value=_centi=_milli=_micro = s;
					}
				} 

				if(raiseEvents) {
					OnPropertyChanged(nameof(Kilo));
					OnPropertyChanged(nameof(Deci));
					OnPropertyChanged(nameof(Value));
					OnPropertyChanged(nameof(Centi));
					OnPropertyChanged(nameof(Milli));
					OnPropertyChanged(nameof(Micro));
				}
				return true;
			}


			/// <summary> Converts the specified value.
			/// </summary>
			/// <param name="value">The value.</param>
			/// <param name="inputUnit">The input unit.</param>
			/// <param name="outputUnit">The output unit.</param>
			/// <returns></returns>
			/// <remarks></remarks>
			private string Convert(string value,string inputUnit,string outputUnit) {
				if(this.ViewModel==null) throw new NotImplementedException("{B5EEEC55-E2DF-42A4-BCA9-DAE33AACB2AB}"); 

				var lang = Thread.CurrentThread.CurrentUICulture;
				try {
					object newValueO = Provider.TypeConverter.ConvertTo(value, this.ViewModel.ValueType); //throws an exception if conversation failed
					if(newValueO == null) throw new NotImplementedException("{1B300FEF-85AF-457E-B729-04F3DCE0710B}");
					else if(newValueO is double) {
						if(double.IsNaN((double)newValueO)) return "";
						double newValue=double.NaN;
							
						switch (inputUnit) {
							case "Ym": newValue = ((double) newValueO)*1000000000000000000000000.0;break;
							case "Zm": newValue = ((double) newValueO)*1000000000000000000000.0;break;
							case "Em": newValue = ((double) newValueO)*1000000000000000000.0;break;
							case "Pm": newValue = ((double) newValueO)*1000000000000000.0;break;
							case "Tm": newValue = ((double) newValueO)*1000000000000.0;break;
							case "Gm": newValue = ((double) newValueO)*1000000000.0;break;
							case "Mm": newValue = ((double) newValueO)*1000000.0;break;
							case "km": newValue = ((double) newValueO)*1000.0;break;
	//						case "Dm": newValue = ((double) newValueO)*10.0;break;
							case "m" : newValue = ((double) newValueO); break;
							case "dm": newValue = ((double) newValueO)/10.0;break;
							case "cm": newValue = ((double) newValueO)/100.0;break;
							case "mm": newValue = ((double) newValueO)/1000.0;break;
							case "µm": newValue = ((double) newValueO)/1000000.0;break;
							case "nm": newValue = ((double) newValueO)/1000000000.0;break;
							case "pm": newValue = ((double) newValueO)/1000000000000.0;break;
							case "fm": newValue = ((double) newValueO)/1000000000000000.0;break;
							case "am": newValue = ((double) newValueO)/1000000000000000000.0;break;
							case "zm": newValue = ((double) newValueO)/1000000000000000000000.0;break;
							case "ym": newValue = ((double) newValueO)/1000000000000000000000000.0;break;
							case "mi": newValue = ((double) newValueO)*1609.0;break;//Meile
							case "ft": newValue = ((double) newValueO)*1609.0*5280.0;;break;//Foot
	//						case "sm": newValue = ((double) newValueO)*1852.0;break;//See Meile
							case "in": newValue = ((double) newValueO)*0.0254;break;//Zoll
							default: throw new NotImplementedException("{1486A894-5AA8-4317-ADC9-903A8E59AFA1}");
						}
						switch (outputUnit) {
							case "Ym": return (newValue/1000000000000000000000000.0).ToString(lang);
							case "Zm": return (newValue/   1000000000000000000000.0).ToString(lang);
							case "Em": return (newValue/      1000000000000000000.0).ToString(lang);
							case "Pm": return (newValue/         1000000000000000.0).ToString(lang);
							case "Tm": return (newValue/            1000000000000.0).ToString(lang);
							case "Gm": return (newValue/               1000000000.0).ToString(lang);
							case "Mm": return (newValue/                  1000000.0).ToString(lang);
							case "km": return (newValue/                     1000.0).ToString(lang);
	//						case "Dm": return (newValue/                       10.0).ToString(lang);
							case "m" : return  newValue                             .ToString(lang);
							case "dm": return (newValue*                       10.0).ToString(lang); 
							case "cm": return (newValue*                      100.0).ToString(lang);
							case "mm": return (newValue*                     1000.0).ToString(lang);
							case "µm": return (newValue*                  1000000.0).ToString(lang);
							case "nm": return (newValue*               1000000000.0).ToString(lang); 
							case "pm": return (newValue*            1000000000000.0).ToString(lang); 
							case "fm": return (newValue*         1000000000000000.0).ToString(lang); 
							case "am": return (newValue*      1000000000000000000.0).ToString(lang); 
							case "zm": return (newValue*   1000000000000000000000.0).ToString(lang); 
							case "ym": return (newValue*1000000000000000000000000.0).ToString(lang); 
							case "mi": return (newValue/1609.0).ToString(lang); //Meile
							case "ft": return (newValue/1609.0/5280.0).ToString(lang); //Foot
	//						case "sm": return (newValue/1852.0).ToString(lang); //See Meile
							case "in": return (newValue/0.0254).ToString(lang); //Zoll
							default: throw new NotImplementedException("{F8EDE031-99A2-4C49-A785-ADFE490D7EB6}");
						}
					}
					throw new NotImplementedException("{2098F0B1-71F6-44BD-AF1B-970D175FD2C3} Type:"+newValueO.GetType().Name);
				} catch (Exception ex) {
					((IErrorProviderController) ViewModel.Metadata.ErrorProvider).SetError(ex.Message); //TODO localize message
					return "";
				}
			}

		}

	}
}
