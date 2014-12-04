using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
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

			private bool m_StringIsInitialized;

			private string m_Micro;
			private string m_Milli;
			private string m_Centi;
			private string m_Value;
			private string m_Deci;
			private string m_Kilo;

			public MetricExtension(EditValueProvider provider) : base(provider) {}

			#region properties



			/// <summary> Gets or sets the editable value in micro(meter).
			/// </summary>
			/// <value>The micro(meter) value.</value>
			/// <remarks></remarks>
			public string Micro {
				get {
					if(!m_StringIsInitialized) {
						UpdateValue(false);
						m_StringIsInitialized = UpdateValue(false);
					}
					return m_Micro;
				}
				set {
	//				if (Equals(this.editValue, m)) return;
					m_Value = Convert(value, "µm", "m");
					m_Micro  = value;
					m_Milli  = Convert(value, "µm", "mm");;
					m_Centi  = Convert(value, "µm", "cm");
					m_Deci   = Convert(value, "µm", "dm");
					m_Kilo   = Convert(value, "µm", "km");
				
				
					OnPropertyChanged("Value");
					OnPropertyChanged("Kilo");
					OnPropertyChanged("Deci");
					OnPropertyChanged("Centi");
					OnPropertyChanged("Milli");
					OnPropertyChanged("Micro");

					UpdateSource();
				}
			}

			 /// <summary> Gets or sets the editable value in milli(meter).
			/// </summary>
			/// <value>The milli(meter) value.</value>
			/// <remarks></remarks>
			public string Milli {
				get {
					if(!m_StringIsInitialized) {
						UpdateValue(false);
						m_StringIsInitialized = UpdateValue(false);
					}
					return m_Milli;
				}
				set {
	//				if (Equals(this.editValue, m)) return;
					m_Value = Convert(value, "mm", "m");
					m_Micro  = Convert(value, "mm", "µm");
					m_Milli  = value;
					m_Centi  = Convert(value, "mm", "cm");
					m_Deci   = Convert(value, "mm", "dm");
					m_Kilo   = Convert(value, "mm", "km");
				
					OnPropertyChanged("Value");
					OnPropertyChanged("Kilo");
					OnPropertyChanged("Deci");
					OnPropertyChanged("Centi");
					OnPropertyChanged("Milli");
					OnPropertyChanged("Micro");
				
					UpdateSource();
				}
			}

			/// <summary> Gets or sets the editable value in centi(meter).
			/// </summary>
			/// <value>The centi(meter) value.</value>
			/// <remarks></remarks>
			public string Centi {
				get {
					if(!m_StringIsInitialized) {
						UpdateValue(false);
						m_StringIsInitialized = UpdateValue(false);
					}
					return m_Centi;
				}
				set {
	//				if (Equals(this.editValue, value)) return;
					m_Value = Convert(value, "cm", "m");
					m_Micro  = Convert(value, "cm", "µm");
					m_Milli  = Convert(value, "cm", "mm");
					m_Centi  = value;
					m_Deci   = Convert(value, "cm", "dm");
					m_Kilo   = Convert(value, "cm", "km");
				

					OnPropertyChanged("Value");
					OnPropertyChanged("Kilo");
					OnPropertyChanged("Deci");
					OnPropertyChanged("Centi");
					OnPropertyChanged("Milli");
					OnPropertyChanged("Micro");
				
					UpdateSource();
				}
			}

						/// <summary> Gets or sets the editable value in base(meter).
			/// </summary>
			/// <value>The base(meter) value.</value>
			/// <remarks></remarks>
			public string Value {
				get {
					if(!m_StringIsInitialized) {
						UpdateValue(false);
						m_StringIsInitialized = UpdateValue(false);
					}
					return m_Centi;
				}
				set {
	//				if (Equals(this.editValue, value)) return;
					m_Value = Convert(value, "m", "m");
					m_Micro  = Convert(value, "m", "µm");
					m_Milli  = Convert(value, "m", "mm");
					m_Centi  = Convert(value, "m", "cm");
					m_Deci   = Convert(value, "m", "dm");
					m_Kilo   = Convert(value, "m", "km");
				

					OnPropertyChanged("Value");
					OnPropertyChanged("Kilo");
					OnPropertyChanged("Deci");
					OnPropertyChanged("Centi");
					OnPropertyChanged("Milli");
					OnPropertyChanged("Micro");
				
					UpdateSource();
				}
			}

			/// <summary> Gets or sets the editable value in deci(meter).
			/// </summary>
			/// <value>The deci(meter) value.</value>
			/// <remarks></remarks>
			public string Deci {
				get {
					if(!m_StringIsInitialized) {
						UpdateValue(false);
						m_StringIsInitialized = UpdateValue(false);
					}
					return m_Centi;
				}
				set {
	//				if (Equals(this.editValue, value)) return;
					m_Value = Convert(value, "dm", "m");
					m_Milli  = Convert(value, "dm", "mm");
					m_Deci   = value;
					m_Centi  = Convert(value, "dm", "cm");
					m_Kilo   = Convert(value, "dm", "km");
					m_Micro  = Convert(value, "dm", "µm");

					OnPropertyChanged("Value");
					OnPropertyChanged("Kilo");
					OnPropertyChanged("Deci");
					OnPropertyChanged("Centi");
					OnPropertyChanged("Milli");
					OnPropertyChanged("Micro");
				
					UpdateSource();
				}
			}

			/// <summary> Gets or sets the editable value in kilo(meter).
			/// </summary>
			/// <value>The kilo(meter) value.</value>
			/// <remarks></remarks>
			public string Kilo {
				get {
					if(!m_StringIsInitialized) {
						UpdateValue(false);
						m_StringIsInitialized = UpdateValue(false);
					}
					return m_Centi;
				}
				set {
	//				if (Equals(this.editValue, value)) return;
					m_Micro  = Convert(value, "km", "µm");
					m_Milli  = Convert(value, "km", "mm");
					m_Value  = Convert(value, "km", "m");
					m_Centi  = Convert(value, "km", "cm");
					m_Deci   = Convert(value, "km", "dm");
					m_Kilo   = value;

					OnPropertyChanged("Value");
					OnPropertyChanged("Kilo");
					OnPropertyChanged("Deci");
					OnPropertyChanged("Centi");
					OnPropertyChanged("Milli");
					OnPropertyChanged("Micro");
				
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
					newValue = Provider.TypeConverter.ConvertTo(m_Value, ViewModel.ValueType); //throws an exception if conversation failed
				} catch(Exception ex) {
					((IErrorProviderController)ViewModel.Metadata.ErrorProvider).SetError(ex.Message); //TODO localize message
					return;
				}

				Provider.UpdateSource(newValue);
			}

			internal bool UpdateValue(bool raiseEvents) {
				m_Kilo  = null;
				m_Deci  = null;
				m_Value = null;
				m_Centi = null;
				m_Milli = null;
				m_Micro = null;
	
				if (!ViewModel.HasValue) {
				
				} else if (ViewModel.Value is Double) {
					var value = (double) ViewModel.Value;
					var s = (string)Provider.TypeConverter.ConvertFrom(value);
					if (!double.IsNaN(value) && !double.IsInfinity(value)) {
						m_Kilo  = Convert(s, "m", "km");
						m_Deci  = Convert(s, "m", "dm");
						m_Value = s;
						m_Centi = Convert(s, "m", "cm");
						m_Milli = Convert(s, "m", "mm");
						m_Micro = Convert(s, "m", "µm");
					}else {
						m_Kilo=m_Deci=m_Value=m_Centi=m_Milli=m_Micro = s;
					}
				} 

				if(raiseEvents) {
					OnPropertyChanged("Kilo");
					OnPropertyChanged("Deci");
					OnPropertyChanged("Value");
					OnPropertyChanged("Centi");
					OnPropertyChanged("Milli");
					OnPropertyChanged("Micro");
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
