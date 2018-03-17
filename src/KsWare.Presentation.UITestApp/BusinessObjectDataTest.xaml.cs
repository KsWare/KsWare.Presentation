using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;


namespace KsWare.Presentation.UITestApp {
	/// <summary>
	/// Interaction logic for BusinessObjectDataTest.xaml
	/// </summary>
	public partial class BusinessObjectDataTest : UserControl {

		public BusinessObjectDataTest() {
			InitializeComponent();
			DataContext=new BusinessObjectDataTestVM();
		}
	}

	public class BusinessObjectDataTestVM : BusinessObjectVM<BusinessObjectDataTestBM> {

		public BusinessObjectDataTestVM() {
			RegisterChildren(()=>this);
			BusinessObject=new BusinessObjectDataTestBM {Data = new BusinessObjectDataTestData {String = "Test"} };
			String.ValueChangedEvent.add = (s, e) => { Debug.WriteLine($"BusinessObjectDataTestVM: String.ValueChanged {String.Value}"); };
		}

		public StringVM String { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Refresh
		/// </summary>
		public ActionVM RefreshAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="RefreshAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoRefresh() {
			OnPropertyChanged("");
		}

		protected override void OnBusinessObjectChanged(BusinessObjectDataTestBM o, BusinessObjectDataTestBM p) {
			base.OnBusinessObjectChanged(o, p);

			String.MːBusinessObject = o.String;
		}

	}

	public class BusinessObjectDataTestBM : DataBM<BusinessObjectDataTestData> {

		public BusinessObjectDataTestBM() {
			RegisterChildren(()=>this);
			String.ValueChangedEvent.add = (s, e2) => {if(Data!=null)  Data.String = String.Value;};
			String.ValueChangedEvent.add = (s, e) => { Debug.WriteLine($"BusinessObjectDataTestBM: String.ValueChanged {String.Value}"); };
		}

		public StringBM String { get; [UsedImplicitly] private set; }

		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);

			String.Value = Data?.String;
		}

	}

	public class BusinessObjectDataTestData:INotifyPropertyChanged {

		private string _string;

		public BusinessObjectDataTestData() {
			
		}

		public string String {
			get => _string;
			set {
				if(_string == value) return;
				_string = value;
				OnPropertyChanged(nameof(String));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
