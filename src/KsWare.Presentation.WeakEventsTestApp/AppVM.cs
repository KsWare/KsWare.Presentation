using System.Windows;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.WeakEventsTestApp {

	public class AppVM:ApplicationVM {

		public AppVM() {
			
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			MainWindowVM.Instance.Show();
		}

	}
}
