using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.BusyManagerTestApp {

	public class AppVM:ApplicationVM {

		public AppVM() {
			StartupUri = typeof (MainWindowVM);
			RegisterChildren(()=>this);
		}

	}
}
