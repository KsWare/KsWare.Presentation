using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;


namespace KsWare.Presentation.UITestApp {
	/// <summary>
	/// Interaction logic for DialogWindow.xaml
	/// </summary>
	public partial class SampleDialogWindow : Window {

		public SampleDialogWindow() {
			InitializeComponent();
		}

	}

	public class SampleDialogWindowVM : DialogWindowVM {

		public SampleDialogWindowVM() {
			RegisterChildren(()=>this);

			Title.Value = "Sample Dialog Window";
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to OK
		/// </summary>
		public ActionVM OkAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="OkAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoOk() {
			DialogResult = true;
			Close();
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Cancel
		/// </summary>
		public ActionVM CancelAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="CancelAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoCancel() {
			DialogResult = false;
			Close();
		}
	}
}
