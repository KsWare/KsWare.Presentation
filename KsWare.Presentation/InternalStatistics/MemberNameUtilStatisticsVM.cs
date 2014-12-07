using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.Core;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.InternalStatistics {

	[DefaultView(typeof(BaseStatisticsView))]
	public class MemberNameUtilStatisticsVM : BaseStatisticsVM {

		public MemberNameUtilStatisticsVM() {
			RegisterChildren(_=>this);
			Caption = "MemberNameUtil";

			m_Items.Add(new StatisticEntryVM("Number of registered expressions"  ,()=>MemberNameUtil.StatisticsːCount));
			m_Items.Add(new StatisticEntryVM("Number of 'GetName' invocation"    ,()=>MemberNameUtil.StatisticsːRaiseːGetCount));
		}

	}

}