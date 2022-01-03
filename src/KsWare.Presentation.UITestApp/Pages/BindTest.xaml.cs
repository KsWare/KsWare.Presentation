using System.Windows.Controls;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.UITestApp.Pages {

	/// <summary>
	/// Interaction logic for BindTest.xaml
	/// </summary>
	public partial class BindTest : Page {

		public BindTest() {
			InitializeComponent();
		}
	}

	public class BindTestVM : ObjectVM {

		public string Text { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }
	}

}
