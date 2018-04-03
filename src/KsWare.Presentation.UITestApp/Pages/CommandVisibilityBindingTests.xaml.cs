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

namespace KsWare.Presentation.UITestApp {

	/// <summary>
	/// Interaction logic for CommandVisibilityBindingTests.xaml
	/// </summary>
	public partial class CommandVisibilityBindingTests {

		public CommandVisibilityBindingTests() {
			InitializeComponent();
		}

		public static MockCommand MockCommand => new MockCommand();

	}

	public class MockCommand:DependencyObject,ICommand {

		//TODO change at runtime has no effect! 

		public static readonly DependencyProperty MockCanExecuteProperty = DependencyProperty.Register(
			"MockCanExecute", typeof (bool), typeof (MockCommand), new PropertyMetadata(false,(o,e) => ((MockCommand)o).AtMockCanExecuteChanged(e)));

		private void AtMockCanExecuteChanged(DependencyPropertyChangedEventArgs e) {
			CanExecuteChanged?.Invoke(this,EventArgs.Empty);
			CommandManager.InvalidateRequerySuggested();
		}

		public bool MockCanExecute { get => (bool) GetValue(MockCanExecuteProperty); set => SetValue(MockCanExecuteProperty, value); }

		public void Execute(object parameter) {  }
		public bool CanExecute(object parameter) { return MockCanExecute; }
		public event EventHandler CanExecuteChanged;

	}

}
