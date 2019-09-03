using System.Windows.Controls;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.UITestApp.Pages
{
	/// <summary>
	/// Interaction logic for VisibilityBindingTests.xaml
	/// </summary>
	public partial class VisibilityBindingTests : Page
	{
		public VisibilityBindingTests()
		{
			InitializeComponent();
		}


	}
		public class VisibilityBindingTests_MyDataContext : ObjectVM
		{
			private bool _boolValue;
			private int _intValue;

			public bool BoolValue {
				get => _boolValue;
				set {
					if (Equals(_boolValue, value)) return;
					_boolValue = value;
					OnPropertyChanged("BoolValue");
				}
			}
			public int IntValue {
				get => _intValue;
				set {
					if (Equals(_intValue, value)) return;
					_intValue = value;
					OnPropertyChanged("IntValue");
				}
			}
	}

}
