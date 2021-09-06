using System;
using System.IO;
using System.Windows;
using KsWare.Presentation.ViewModelFramework;


namespace KsWare.Presentation.UITestApp {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}
	}

	public class PageInfoVM : ObjectVM {

		public Uri Uri { get => Fields.GetValue<Uri>(); set => Fields.SetValue(value); }

		public string DisplayName { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public FrameworkElement View { get => Fields.GetValue<FrameworkElement>(); set => Fields.SetValue(value); }
	}
}
