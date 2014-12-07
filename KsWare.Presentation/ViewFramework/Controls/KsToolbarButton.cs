using System;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.Controls {

	/// <summary> Interaction logic for KsToolbarButton
	/// </summary>
	public partial class KsToolbarButton : KsButton {

		public KsToolbarButton() { }

		public void OnCanExecuteChanged(object sender, EventArgs args) { }

		static KsToolbarButton() { DefaultStyleKeyProperty.OverrideMetadata(typeof (KsToolbarButton), new FrameworkPropertyMetadata(typeof (KsToolbarButton))); }

	}

}
