using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

	public class BaseStatisticsVM : ObjectVM, IContentModuleVM, INotifyPropertyChanged {
		
		protected List<StatisticEntryVM> _Items=new List<StatisticEntryVM>();
		private PropertyChangedEventHandler _propertyChanged;

		public string Caption { get; protected set; }
		public List<StatisticEntryVM> Items => _Items;

		public virtual void Refresh() {
			foreach (var item in _Items) { item.RefreshValue(); }
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {add => _propertyChanged += value; remove => _propertyChanged -= value; }

		[NotifyPropertyChangedInvocator]
		protected new void OnPropertyChanged(string propertyName) {
			if(_propertyChanged!=null)_propertyChanged(this,new PropertyChangedEventArgs(propertyName));
		}

	}

}