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
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.UITestApp.Pages {
	/// <summary>
	/// Interaction logic for BindableMenuTest.xaml
	/// </summary>
	public partial class BindableMenuTest : Page {
		public BindableMenuTest() {
			InitializeComponent();
		}
	}

	public class BindableMenuTestVM:ObjectVM{

		/// <inheritdoc />
		public BindableMenuTestVM() {
			RegisterChildren(() => this);

			MenuItemVM test,more,check;
			Menu.Add(test = new MenuItemVM { Caption = "_Test" });
			test.Items.Add(new MenuItemVM { Caption = "ActionCommand", CommandAction = { MːExecutedCallback = DoCommand } });
			test.Items.Add(new MenuItemVM { Caption = "Cut", Command = ApplicationCommands.Cut });
			test.Items.Add(new MenuItemVM { Caption = "-" });
			test.Items.Add(more = new MenuItemVM { Caption = "More" });
			more.Items.Add(check = new MenuItemVM { Caption = "Checker", IsCheckable = true });
			test.Items.Add(new MenuItemVM(CustomAction) { Caption = "Custom Action", CommandParameter = "Parameter"});
			// #1 synchronize 2 bool values (functional, not optimized for reuse)
			// Fields[nameof(IsChecked)].ValueChangedEvent.add = (s, e) => check.IsChecked = (bool)e.NewValue;
			// check.Fields[nameof(MenuItemVM.IsChecked)].ValueChangedEvent.add = (s, e) => IsChecked = (bool)e.NewValue;
			// #2 synchronize 2 bool values (experimental)
			FieldBindingOperations.SetBinding(Fields[nameof(IsChecked)], new FieldBinding(check.Fields[nameof(MenuItemVM.IsChecked)]));
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Custom
		/// </summary>
		/// <seealso cref="DoCustom"/>
		public ActionVM CustomAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="CustomAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoCustom(object parameter) {
			MessageBox.Show($"Custom Action - {parameter}");
		}

		private void DoCommand(object? sender, ExecutedEventArgs e) {
			MessageBox.Show($"ActionCommand {e.Parameter}");
		}

		public ListVM<MenuItemVM> Menu { get; [UsedImplicitly] private set; }

		public bool IsChecked { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
	}
}
