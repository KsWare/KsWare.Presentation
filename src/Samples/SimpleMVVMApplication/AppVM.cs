using KsWare.Presentation.ViewModelFramework;

namespace SimpleMVVMApplication {

	public class AppVM : ApplicationVM {

		public AppVM() {
			RegisterChildren(() => this);
			StartupUri = typeof(MainWindowVM);
		}
	}
}
