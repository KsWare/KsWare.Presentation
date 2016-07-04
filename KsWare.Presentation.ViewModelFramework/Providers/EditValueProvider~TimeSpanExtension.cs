using System;
using System.Globalization;

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

			private string _days;
			private string _hours;
			private string _minutes;
			private string _seconds;
			private string _milliseconds;

			private int _daysValue;
			private int _hoursValue;
			private int _minutesValue;
			private int _secondsValue;
			private int _millisecondsValue;

			private bool _daysHasError;
			private bool _hoursHasError;
			private bool _minutesHasError;
			private bool _secondsHasError;
			private bool _millisecondsHasError;

			private double _totalSecondsAsDouble;

			public TimeSpanExtension(EditValueProvider provider) : base(provider) {}

			public string Days {
				get {return _days; }
				set {
					if(Equals(_days,value))return;
					_days = value;
					OnPropertyChanged(nameof(Days));

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0) {
							_daysValue = result;
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
				get {return _daysHasError; }
				private set {
					if(Equals(_daysHasError,value))return;
					_daysHasError = value;
					OnPropertyChanged(nameof(DaysHasError));
				}
			}

			public string Hours {
				get {return _hours; }
				set {
					if(Equals(_hours,value))return;
					_hours = value;
					OnPropertyChanged(nameof(Hours));

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0 && result < 24) {
							_hoursValue=result;
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
				get {return _hoursHasError; }
				private set {
					if(Equals(_hoursHasError,value))return;
					_hoursHasError = value;
					OnPropertyChanged(nameof(HoursHasError));
				}
			}

			public string Minutes {
				get {return _minutes; }
				set {
					if(Equals(_minutes,value))return;
					_minutes = value;
					OnPropertyChanged(nameof(Minutes));

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0 && result < 60) {
							_minutesValue=result;
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
				get {return _minutesHasError; }
				private set {
					if(Equals(_minutesHasError,value))return;
					_minutesHasError = value;
					OnPropertyChanged(nameof(MinutesHasError));
				}
			}

			public string Seconds {
				get {return _seconds; }
				set {
					if(Equals(_seconds,value))return;
					_seconds = value;
					OnPropertyChanged(nameof(Seconds));

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0 && result < 60) {
							_secondsValue=result;
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
				get {return _secondsHasError; }
				private set {
					if(Equals(_secondsHasError,value))return;
					_secondsHasError = value;
					OnPropertyChanged(nameof(SecondsHasError));
				}
			}

			public string Milliseconds {
				get {return _milliseconds; }
				set {
					if(Equals(_milliseconds,value))return;
					_milliseconds = value;
					OnPropertyChanged(nameof(Milliseconds));

					int result;
					if(int.TryParse(value,NumberStyles.Integer,null,out result)){
						if (result >= 0 && result < 1000) {
							_millisecondsValue=result;
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
				get {return _millisecondsHasError; }
				private set {
					if(Equals(_millisecondsHasError,value))return;
					_millisecondsHasError = value;
					OnPropertyChanged(nameof(MillisecondsHasError));
				}
			}

			private void TriggerUpdateSource() {
				if(DaysHasError||HoursHasError||MinutesHasError||SecondsHasError||_millisecondsHasError)return;

				var value = new TimeSpan(_daysValue, _hoursValue, _minutesValue, _secondsValue, _millisecondsValue);
				ViewModel.Value = value;
			}

			private void TriggerError() {
				this.DoNothing();
			}

			internal bool UpdateValue(bool raiseEvents) {
				if (!ViewModel.HasValue) {
					_days=_hours=_minutes=_seconds=_milliseconds = null;
				} else if (ViewModel.Value is TimeSpan) {
					var timespan = (TimeSpan) this.ViewModel.Value;
					_daysValue            = timespan.Days;
					_hoursValue           = timespan.Hours;
					_minutesValue         = timespan.Minutes;
					_secondsValue         = timespan.Seconds;
					_millisecondsValue    = timespan.Milliseconds;
					_days                 = _daysValue        .ToStringEnUs();
					_hours                = _hoursValue       .ToStringEnUs();
					_minutes              = _minutesValue     .ToStringEnUs();
					_seconds              = _secondsValue     .ToStringEnUs();
					_milliseconds         = _millisecondsValue.ToStringEnUs();
					_daysHasError         = false;
					_hoursHasError        = false;
					_minutesHasError      = false;
					_secondsHasError      = false;
					_millisecondsHasError = false;
					_totalSecondsAsDouble = timespan.TotalSeconds;
	//			} else if (this.ParentValueVM.Value is Double) {
	//				
				} else {
					_days=_hours=_minutes=_seconds=_milliseconds = null;
				}
			
				if(raiseEvents) {
					OnPropertyChanged(nameof(Days));
					OnPropertyChanged(nameof(Hours));
					OnPropertyChanged(nameof(Minutes));
					OnPropertyChanged(nameof(Seconds));
					OnPropertyChanged(nameof(Milliseconds));

					OnPropertyChanged(nameof(DaysHasError));
					OnPropertyChanged(nameof(HoursHasError));
					OnPropertyChanged(nameof(MinutesHasError));
					OnPropertyChanged(nameof(SecondsHasError));
					OnPropertyChanged(nameof(MillisecondsHasError));

					OnPropertyChanged(nameof(TotalSecondsAsDouble));
				}
				return true;
			}

			public double TotalSecondsAsDouble {
				get {return _totalSecondsAsDouble; }
				set {
					if(Equals(_totalSecondsAsDouble,value))return;
					_totalSecondsAsDouble = value;
					OnPropertyChanged(nameof(TotalSecondsAsDouble));

					var v = System.TimeSpan.FromSeconds(_totalSecondsAsDouble);
					ViewModel.Value = v;
				}
			}
		}

	}
}
