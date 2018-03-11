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
using System.Windows.Navigation;
using System.Windows.Shapes;
using JetBrains.Annotations;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.ViewModelFramework;


namespace KsWare.Presentation.UITestApp {
	/// <summary>
	/// Interaction logic for StringViewModelChangeBusinessObject.xaml
	/// </summary>
	public partial class StringViewModelChangeBusinessObject : UserControl {

		public StringViewModelChangeBusinessObject() {
			StringABM=new StringBM() {MːData = ""};
			StringBBM=new StringBM() {MːData = ""};
			StringVM=new StringVM();

			InitializeComponent();
		}

		public StringBM StringABM { get; set; }
		public StringBM StringBBM { get; set; }

		public StringVM StringVM { get; [UsedImplicitly] private set; }



		private void SetNull(object sender, RoutedEventArgs e) { StringVM.MːBusinessObject = null; }
		private void SetA(object sender, RoutedEventArgs e) { StringVM.MːBusinessObject = StringABM; }
		private void SetB(object sender, RoutedEventArgs e) { StringVM.MːBusinessObject = StringBBM; }

	}
}
