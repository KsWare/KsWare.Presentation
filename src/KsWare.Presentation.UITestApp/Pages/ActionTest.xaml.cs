using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.UITestApp.Pages {
	/// <summary>
	/// Interaction logic for ActionTest.xaml
	/// </summary>
	public partial class ActionTest : Page {

		public ActionTest() {
			InitializeComponent();
		}
	}

	public class ActionTestVM : ObjectVM {

		/// <inheritdoc />
		public ActionTestVM() {
			RegisterChildren(() => this);
		}

		public bool ThrowException { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
		public ActionVM Test1Action { get; private set; }
		public ActionVM Test2Action { get; private set; }
		public ActionVM Test3Action { get; private set; }
		public ActionVM Test4Action { get; private set; }
		public string Text { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		private async void DoTest1() {
			Text += "1 ";
			MessageBox.Show("1");
			if (ThrowException) throw new Exception("1");
			await Task.Delay(1000);
		}

		private async void DoTest2(object parameter) {
			Text += "2 ";
			MessageBox.Show("2");
			if (ThrowException) throw new Exception("2");
			await Task.Delay(1000);
		}

		private async Task DoTest3Async() {
			Text += "3 ";
			MessageBox.Show("3");
			if (ThrowException) throw new Exception("3");
			await Task.Delay(1000);
		}

		private async Task DoTest4Async(object parameter) {
			Text += "4 ";
			MessageBox.Show("4");
			if (ThrowException) throw new Exception("4");
			await Task.Delay(1000);
		}
	}
}
