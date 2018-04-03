using System.Windows;
using KsWare.Presentation.ViewFramework;
using KsWare.Presentation.ViewModelFramework;


namespace KsWare.Presentation.UITestApp {

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App {
		public App() {
//			CatchUnhandledExceptions = false;
		}
	}

	public class AppVM : ApplicationVM {

		public AppVM() {
			RegisterChildren(()=>this);
			StartupUri = typeof(MainWindowVM);
		}

	}
}
