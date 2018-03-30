using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ComponentSamples {

	public class NavigationPageVM : ObjectVM {
		public NavigationPageVM() {
			RegisterChildren(()=>this);
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Navigate
		/// </summary>
		/// <seealso cref="DoNavigate"/>
		public ActionVM NavigateAction { get; [UsedImplicitly] private set; }

		public NavigationService NavigationService { get => Fields.GetValue<NavigationService>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="NavigateAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoNavigate(object parameter) {
			AppVM.ContentNavigationService.Navigate(new Uri((string) parameter, UriKind.Relative));
		}
	}
}
