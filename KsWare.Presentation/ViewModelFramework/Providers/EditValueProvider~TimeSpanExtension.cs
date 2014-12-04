using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KsWare.Presentation.ViewModelFramework.Providers {


	public interface IEditValueProviderTimeSpanExtension:IEditValueProviderExtension {

		string Days { get; set; }
		string Hours { get; set; }
		string Minutes { get; set; }
		string Seconds { get; set; }
		string Milliseconds { get; set; }

		double TotalSecondsAsDouble { get; set; }
	}

	partial class EditValueProvider {

		class TimeSpanExtension:Extension,IEditValueProviderTimeSpanExtension {

			private string m_Days;
			private string m_Hours;
			private string m_Minutes;
			private string m_Seconds;
			private string m_Milliseconds;

			private int m_DaysValue;
			private int m_HoursValue;
			private int m_MinutesValue;
			private int m_SecondsValue;
			private int m_MillisecondsValue;

			private bool m_DaysHasError;
			private bool m_HoursHasError;
			private bool m_MinutesHasError;
			private bool m_SecondsHasError;
			private bool m_MillisecondsHasError;

			private double m_TotalSecondsAsDouble;

			public TimeSpanExtension(EditValueProvider provider) : base(provider) {}

			public string Days {
				get {return m_Days; }
				set {
					if(Equals(m_Days,value))return;
					m_Days = value;
					OnPropertyChanged("Days");

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0) {
							m_DaysValue = result;
							DaysHasError = false;
							TriggerUpdateSource();
							return;
						}
					}
					DaysHasError = true;
					TriggerError();
				}
			}

			public bool DaysHasError {
				get {return m_DaysHasError; }
				private set {
					if(Equals(m_DaysHasError,value))return;
					m_DaysHasError = value;
					OnPropertyChanged("DaysHasError");
				}
			}

			public string Hours {
				get {return m_Hours; }
				set {
					if(Equals(m_Hours,value))return;
					m_Hours = value;
					OnPropertyChanged("Hours");

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0 && result < 24) {
							m_HoursValue=result;
							HoursHasError = false;
							TriggerUpdateSource(); 
							return;
						}
					}
					HoursHasError = true;
					TriggerError();
				}
			}

			public bool HoursHasError {
				get {return m_HoursHasError; }
				private set {
					if(Equals(m_HoursHasError,value))return;
					m_HoursHasError = value;
					OnPropertyChanged("HoursHasError");
				}
			}

			public string Minutes {
				get {return m_Minutes; }
				set {
					if(Equals(m_Minutes,value))return;
					m_Minutes = value;
					OnPropertyChanged("Minutes");

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0 && result < 60) {
							m_MinutesValue=result;
							MinutesHasError = false;
							TriggerUpdateSource(); 
							return;
						}
					}
					MinutesHasError = true;
					TriggerError();
				}
			}

			public bool MinutesHasError {
				get {return m_MinutesHasError; }
				private set {
					if(Equals(m_MinutesHasError,value))return;
					m_MinutesHasError = value;
					OnPropertyChanged("MinutesHasError");
				}
			}

			public string Seconds {
				get {return m_Seconds; }
				set {
					if(Equals(m_Seconds,value))return;
					m_Seconds = value;
					OnPropertyChanged("Seconds");

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0 && result < 60) {
							m_SecondsValue=result;
							SecondsHasError = false;
							TriggerUpdateSource();
							return;
						}
					}
					SecondsHasError = true;
					TriggerError();
				}
			}

			public bool SecondsHasError {
				get {return m_SecondsHasError; }
				private set {
					if(Equals(m_SecondsHasError,value))return;
					m_SecondsHasError = value;
					OnPropertyChanged("SecondsHasError");
				}
			}

			public string Milliseconds {
				get {return m_Milliseconds; }
				set {
					if(Equals(m_Milliseconds,value))return;
					m_Milliseconds = value;
					OnPropertyChanged("Milliseconds");

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0 && result < 1000) {
							m_MillisecondsValue=result;
							MillisecondsHasError = false;
							TriggerUpdateSource();
							return;
						}
					}
					MillisecondsHasError = true;
					TriggerError();
				}
			}

			public bool MillisecondsHasError {
				get {return m_MillisecondsHasError; }
				private set {
					if(Equals(m_MillisecondsHasError,value))return;
					m_MillisecondsHasError = value;
					OnPropertyChanged("MillisecondsHasError");
				}
			}

			private void TriggerUpdateSource() {
				if(DaysHasError||HoursHasError||MinutesHasError||SecondsHasError||m_MillisecondsHasError)return;

				var value = new TimeSpan(m_DaysValue, m_HoursValue, m_MinutesValue, m_SecondsValue, m_MillisecondsValue);
				ViewModel.Value = value;
			}

			private void TriggerError() {
				this.DoNothing();
			}

			internal bool UpdateValue(bool raiseEvents) {
				if (!ViewModel.HasValue) {
					m_Days=m_Hours=m_Minutes=m_Seconds=m_Milliseconds = null;
				} else if (ViewModel.Value is TimeSpan) {
					var timespan = (TimeSpan) this.ViewModel.Value;
					m_DaysValue            = timespan.Days;
					m_HoursValue           = timespan.Hours;
					m_MinutesValue         = timespan.Minutes;
					m_SecondsValue         = timespan.Seconds;
					m_MillisecondsValue    = timespan.Milliseconds;
					m_Days                 = m_DaysValue        .ToStringEnUs();
					m_Hours                = m_HoursValue       .ToStringEnUs();
					m_Minutes              = m_MinutesValue     .ToStringEnUs();
					m_Seconds              = m_SecondsValue     .ToStringEnUs();
					m_Milliseconds         = m_MillisecondsValue.ToStringEnUs();
					m_DaysHasError         = false;
					m_HoursHasError        = false;
					m_MinutesHasError      = false;
					m_SecondsHasError      = false;
					m_MillisecondsHasError = false;
					m_TotalSecondsAsDouble = timespan.TotalSeconds;
	//			} else if (this.ParentValueVM.Value is Double) {
	//				
				} else {
					m_Days=m_Hours=m_Minutes=m_Seconds=m_Milliseconds = null;
				}
			
				if(raiseEvents) {
					OnPropertyChanged("Days");
					OnPropertyChanged("Hours");
					OnPropertyChanged("Minutes");
					OnPropertyChanged("Seconds");
					OnPropertyChanged("Milliseconds");

					OnPropertyChanged("DaysHasError");
					OnPropertyChanged("HoursHasError");
					OnPropertyChanged("MinutesHasError");
					OnPropertyChanged("SecondsHasError");
					OnPropertyChanged("MillisecondsHasError");

					OnPropertyChanged("TotalSecondsAsDouble");
				}
				return true;
			}

			public double TotalSecondsAsDouble {
				get {return m_TotalSecondsAsDouble; }
				set {
					if(Equals(m_TotalSecondsAsDouble,value))return;
					m_TotalSecondsAsDouble = value;
					OnPropertyChanged("TotalSecondsAsDouble");

					var v = System.TimeSpan.FromSeconds(m_TotalSecondsAsDouble);
					ViewModel.Value = v;
				}
			}
		}

	}
}
