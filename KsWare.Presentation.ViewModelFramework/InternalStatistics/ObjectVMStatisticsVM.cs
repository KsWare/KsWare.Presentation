using KsWare.Presentation.Core;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

//TODO	[DefaultView(typeof(BaseStatisticsView))]
	public class ObjectVMStatisticsVM : BaseStatisticsVM {

		public ObjectVMStatisticsVM() {
			RegisterChildren(_=>this);
			Caption = "ObjectVM";

			m_Items.Add(new StatisticEntryVM("Number of created instances"    ,()=>ObjectVM.StatisticsːNumberOfCreatedInstances));
			m_Items.Add(new StatisticEntryVM("Number of living instances"     ,()=>ObjectVM.StatisticsːNumberOfInstances));
			m_Items.Add(new StatisticEntryVM("Number of Dispose() invocations",()=>ObjectVM.StatisticsːMethodInvocationːDisposeːExplicitːCount));
			m_Items.Add(new StatisticEntryVM("Number of Destructor invocations",()=>ObjectVM.StatisticsːMethodInvocationːDestructorːCount));
		}

	}

}