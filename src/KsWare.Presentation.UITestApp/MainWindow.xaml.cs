using System.Windows;
using JetBrains.Annotations;
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

	public class MainWindowVM : WindowVM {

		public MainWindowVM() {
			RegisterChildren(()=>this);
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to ShowDialog
		/// </summary>
		public ActionVM ShowDialogAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="ShowDialogAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoShowDialog() {
			var result = new SampleDialogWindowVM().ShowDialog();
			var s = result.HasValue ? result.ToString() : "null";
			MessageBox.Show($"DialogResult: {s}");
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Exit
		/// </summary>
		public ActionVM ExitAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="ExitAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoExit() {
			Close();
		}

	}
}
