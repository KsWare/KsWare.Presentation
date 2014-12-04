using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

	public class BaseStatisticsVM : ObjectVM, IContentModuleVM, INotifyPropertyChanged {
		
		protected List<StatisticEntryVM> m_Items=new List<StatisticEntryVM>();
		private PropertyChangedEventHandler m_PropertyChanged;

		public string Caption { get; protected set; }
		public List<StatisticEntryVM> Items { get { return m_Items; } }

		public virtual void Refresh() {
			foreach (var item in m_Items) { item.RefreshValue(); }
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {add { m_PropertyChanged += value; }remove { m_PropertyChanged -= value; }}

		[NotifyPropertyChangedInvocator]
		protected new void OnPropertyChanged(string propertyName) {
			if(m_PropertyChanged!=null)m_PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
		}

	}

}