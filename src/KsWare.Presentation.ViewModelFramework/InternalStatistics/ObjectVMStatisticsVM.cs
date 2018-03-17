using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

//TODO	[DefaultView(typeof(BaseStatisticsView))]
	public class ObjectVMStatisticsVM : BaseStatisticsVM {

		public ObjectVMStatisticsVM() {
			RegisterChildren(()=>this);
			Caption = "ObjectVM";

			_Items.Add(new StatisticEntryVM("Number of created instances"    ,()=>ObjectVM.StatisticsːNumberOfCreatedInstances));
			_Items.Add(new StatisticEntryVM("Number of living instances"     ,()=>ObjectVM.StatisticsːNumberOfInstances));
			_Items.Add(new StatisticEntryVM("Number of Dispose() invocations",()=>ObjectVM.StatisticsːMethodInvocationːDisposeːExplicitːCount));
			_Items.Add(new StatisticEntryVM("Number of Destructor invocations",()=>ObjectVM.StatisticsːMethodInvocationːDestructorːCount));
		}

	}

}