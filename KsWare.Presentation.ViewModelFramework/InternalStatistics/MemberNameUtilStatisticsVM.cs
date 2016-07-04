namespace KsWare.Presentation.InternalStatistics {

//TODO	[DefaultView(typeof(BaseStatisticsView))]
	public class MemberNameUtilStatisticsVM : BaseStatisticsVM {

		public MemberNameUtilStatisticsVM() {
			RegisterChildren(_=>this);
			Caption = "MemberNameUtil";

			_Items.Add(new StatisticEntryVM("Number of registered expressions"  ,()=>MemberNameUtil.StatisticsːCount));
			_Items.Add(new StatisticEntryVM("Number of 'GetName' invocation"    ,()=>MemberNameUtil.StatisticsːRaiseːGetCount));
		}

	}

}