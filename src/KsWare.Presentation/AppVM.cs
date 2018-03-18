using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation {

	public class AppVM : ApplicationVM {

		public AppVM() {
			RegisterChildren(() => this);
			StartupUri = typeof(MainWindowVM);
		}
	}
}
