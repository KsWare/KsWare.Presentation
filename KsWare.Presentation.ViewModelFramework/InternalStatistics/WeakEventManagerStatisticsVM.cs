using KsWare.Presentation.Core;

namespace KsWare.Presentation.InternalStatistics {

//	[DefaultView(typeof(BaseStatisticsView))]
	public class WeakEventManagerStatisticsVM : BaseStatisticsVM {

		public WeakEventManagerStatisticsVM() {
			RegisterChildren(_=>this);
			Caption = "WeakEventManager";

			Items.Add(new StatisticEntryVM("Number of created sources"    ,()=>EventSource.StatisticsːInstancesˑCreated));
			Items.Add(new StatisticEntryVM("Number of registered sources" ,()=>EventSource.StatisticsːInstancesˑCurrent));
			Items.Add(new StatisticEntryVM("Number of created handlers"   ,()=>EventHandle.StatisticsːInstancesˑCreated));
			Items.Add(new StatisticEntryVM("Number of registered handlers",()=>EventHandle.StatisticsːInstancesˑCurrent));
			Items.Add(new StatisticEntryVM("Number of Raise invocation"   ,()=>EventManager.StatisticsːRaiseːInvocationCount));
		}

	}

}