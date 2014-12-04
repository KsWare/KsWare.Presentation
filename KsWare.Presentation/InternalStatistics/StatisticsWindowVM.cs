using System;
using System.Windows;
using System.Windows.Threading;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

	public class StatisticsWindowVM:WindowVM {

		private static StatisticsWindowVM s_Instance;
		private DispatcherTimer m_Timer;

		public static StatisticsWindowVM Instance {
			get {
				if (s_Instance == null) {
					s_Instance = new StatisticsWindowVM();
				}
				return s_Instance;
			}
		}

		public StatisticsWindowVM() {
			UIAccess.IsDirectAccessEnabled = true;
			RegisterChildren(_=>this);

			Modules.Add(new WeakEventManagerStatisticsVM());
			Modules.Add(new MemberNameUtilStatisticsVM());
			Modules.Add(new ObjectVMStatisticsVM());
			Modules.Add(new ObjectBMStatisticsVM());

			if(Application.Current==null) return;
			m_Timer=new DispatcherTimer(TimeSpan.FromMilliseconds(1000), DispatcherPriority.Send, AtTimerTick, Application.Current.Dispatcher);
		}

		private void AtTimerTick(object sender, EventArgs e) {
			foreach (var module in Modules) { module.Refresh(); }
		}

		public ListVM<IContentModuleVM> Modules { get; private set; } 
	}

	public interface IContentModuleVM : IObjectVM {
		string Caption { get; }
		void Refresh();
	}

}
