using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.UITestApp.Pages {
	/// <summary>
	/// Interaction logic for BusyIndicator.xaml
	/// </summary>
	public partial class BusyIndicatorPage : Page {

		public BusyIndicatorPage() {
			InitializeComponent();
			DataContext=new BusyIndicatorVM();
		}
	}

	public class BusyIndicatorVM :ObjectVM {

		public BusyIndicatorVM() {
			RegisterChildren(() => this);

			var t=new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Normal, (s, e) => IsBusy = !IsBusy,
				System.Windows.Threading.Dispatcher.CurrentDispatcher);
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Busy
		/// </summary>
		/// <seealso cref="DoBusy"/>
		public ActionVM BusyAction { get; [UsedImplicitly] private set; }

		public bool IsBusy { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="BusyAction"/>
		/// </summary>
		[UsedImplicitly]
		private async void DoBusy() {
			RequestUserFeedback(new BusyUserFeedbackEventArgs(true));
			await Task.Delay(2000);
			RequestUserFeedback(new BusyUserFeedbackEventArgs(false));
		}
	}
}
