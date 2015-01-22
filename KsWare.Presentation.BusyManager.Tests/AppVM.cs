using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.BusyManagerTestApp {

	public class AppVM:ApplicationVM {

		public AppVM() {
			StartupUri = typeof (MainWindowVM);
			RegisterChildren(()=>this);
		}

	}
}
