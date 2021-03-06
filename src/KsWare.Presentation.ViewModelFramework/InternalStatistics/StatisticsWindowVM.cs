﻿using System;
using System.Windows;
using System.Windows.Threading;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

	public class StatisticsWindowVM:WindowVM {

		private static StatisticsWindowVM s_instance;
		private DispatcherTimer _timer;

		public static StatisticsWindowVM Instance {
			get {
				if (s_instance == null) {
					s_instance = new StatisticsWindowVM();
				}
				return s_instance;
			}
		}

		public StatisticsWindowVM() {
			UIAccess.IsDirectAccessEnabled = true;
			RegisterChildren(()=>this);

			Modules.Add(new WeakEventManagerStatisticsVM());
			Modules.Add(new MemberNameUtilStatisticsVM());
			Modules.Add(new ObjectVMStatisticsVM());
			Modules.Add(new ObjectBMStatisticsVM());

			if(Application.Current==null) return;
			_timer=new DispatcherTimer(TimeSpan.FromMilliseconds(1000), DispatcherPriority.Send, AtTimerTick, Application.Current.Dispatcher);
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
