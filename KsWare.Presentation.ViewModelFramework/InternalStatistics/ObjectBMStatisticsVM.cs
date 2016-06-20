using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.InternalStatistics {

//TODO	[DefaultView(typeof(BaseStatisticsView))]
	public class ObjectBMStatisticsVM : BaseStatisticsVM {

		public ObjectBMStatisticsVM() {
			RegisterChildren(_=>this);
			Caption = "ObjectBM";

			m_Items.Add(new StatisticEntryVM("Number of created instances"    ,()=>ObjectBM.StatisticsːNumberOfCreatedInstances));
			m_Items.Add(new StatisticEntryVM("Number of living instances"     ,()=>ObjectBM.StatisticsːNumberOfInstances));
			m_Items.Add(new StatisticEntryVM("Number of Dispose() invokations",()=>ObjectBM.StatisticsːMethodInvocationːDisposeːCount));
			m_Items.Add(new StatisticEntryVM("Number of Destructor invocations",()=>ObjectBM.StatisticsːMethodInvocationːDestructorːCount));
		}

	}

}