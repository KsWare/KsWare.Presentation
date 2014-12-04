using System;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

	public class StatisticEntryVM:SlimObjectVM {

		private object m_Value;

		public StatisticEntryVM() {}

		public StatisticEntryVM(string description, Func<object> refreshFunction) {
			Description = description;
			Function = refreshFunction;
		}

		public string Name{ get; set; }

		public string Description{ get; set; }

		public object Value {
			get { return m_Value; }
			set {
				if (Equals(value, m_Value)) return;
				m_Value = value;
				OnPropertyChanged("Value");
			}
		}

		public Func<object> Function { get; set; }

		public void RefreshValue() { Value = Function(); }

	}

}