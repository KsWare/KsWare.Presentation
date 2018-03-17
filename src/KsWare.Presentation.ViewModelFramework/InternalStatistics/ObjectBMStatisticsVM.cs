using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.InternalStatistics {

//TODO	[DefaultView(typeof(BaseStatisticsView))]
	public class ObjectBMStatisticsVM : BaseStatisticsVM {

		public ObjectBMStatisticsVM() {
			RegisterChildren(()=>this);
			Caption = "ObjectBM";

			_Items.Add(new StatisticEntryVM("Number of created instances"    ,()=>ObjectBM.StatisticsːNumberOfCreatedInstances));
			_Items.Add(new StatisticEntryVM("Number of living instances"     ,()=>ObjectBM.StatisticsːNumberOfInstances));
			_Items.Add(new StatisticEntryVM("Number of Dispose() invokations",()=>ObjectBM.StatisticsːMethodInvocationːDisposeːCount));
			_Items.Add(new StatisticEntryVM("Number of Destructor invocations",()=>ObjectBM.StatisticsːMethodInvocationːDestructorːCount));
		}

	}

}