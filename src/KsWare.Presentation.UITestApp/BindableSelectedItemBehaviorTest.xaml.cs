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
	/// Interaction logic for BindableSelectedItemBehaviorTest.xaml
	/// </summary>
	public partial class BindableSelectedItemBehaviorTest : Window {

		public BindableSelectedItemBehaviorTest() {
			InitializeComponent();
		}
	}

	public class BindableSelectedItemBehaviorTestVM : ObjectVM {

		public BindableSelectedItemBehaviorTestVM() {
			RegisterChildren(()=>this);
			Items.Add(new ItemVM {DisplayName = "Item A"});
			Items.Add(new ItemVM {DisplayName = "Item B"});
			Items.Add(new ItemVM {DisplayName = "Item C"});
		}

		public ListVM<ItemVM> Items { get; [UsedImplicitly] private set; }

		public ItemVM SelectedItem { get => Fields.GetValue<ItemVM>(); set => Fields.SetValue(value); }

	}

	public class ItemVM:ObjectVM {

		public ItemVM() {
			RegisterChildren(()=>this);

		}
		public string DisplayName { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }
	}

}
