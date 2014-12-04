using KsWare.Presentation.Core;

namespace KsWare.Presentation.InternalStatistics {

	[DefaultView(typeof(BaseStatisticsView))]
	public class WeakEventManagerStatisticsVM : BaseStatisticsVM {

		public WeakEventManagerStatisticsVM() {
			RegisterChildren(_=>this);
			Caption = "WeakEventManager";

			Items.Add(new StatisticEntryVM("Number of registered handlers",()=>EventUtil.WeakEventManager.Count));
			Items.Add(new StatisticEntryVM("Number of Raise invocation"   ,()=>EventUtil.WeakEventManager.StatisticsːRaiseːInvocationCount));
		}

	}

}