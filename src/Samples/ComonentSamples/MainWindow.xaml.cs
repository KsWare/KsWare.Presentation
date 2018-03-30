using System.Windows;
using System.Windows.Navigation;

namespace KsWare.Presentation.ComponentSamples {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			//TODO move to behavior
			DataContextChanged += (s, e) => {
				AppVM.TocNavigationService = NavigationFrame.NavigationService;
				AppVM.ContentNavigationService = ContentFrame.NavigationService;
			};
		}
	}
}
