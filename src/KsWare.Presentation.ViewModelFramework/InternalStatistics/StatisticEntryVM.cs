using System;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

	public class StatisticEntryVM:SlimObjectVM {

		private object _value;

		public StatisticEntryVM() {}

		public StatisticEntryVM(string description, Func<object> refreshFunction) {
			Description = description;
			Function = refreshFunction;
		}

		public string Name{ get; set; }

		public string Description{ get; set; }

		public object Value {
			get { return _value; }
			set {
				if (Equals(value, _value)) return;
				_value = value;
				OnPropertyChanged(nameof(Value));
			}
		}

		public Func<object> Function { get; set; }

		public void RefreshValue() { Value = Function(); }

	}

}