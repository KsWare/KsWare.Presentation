using System.Windows.Navigation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ComponentSamples {

	public class AppVM : ApplicationVM {

		public AppVM() {
			RegisterChildren(() => this);
			StartupUri = typeof(MainWindowVM);
		}

		public static NavigationService TocNavigationService { get; set; }
		public static NavigationService ContentNavigationService { get; set; }
	}
}
