using System;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> Specific <see cref="ErrorProvider"/> with connection to business layer
	/// </summary>
	/// <remarks></remarks>
	public class BusinessValueErrorProvider:ErrorProvider {

		/// <summary> Initializes a new instance of the <see cref="BusinessValueErrorProvider"/> class.
		/// </summary>
		public BusinessValueErrorProvider() {}

		/// <summary> Called when parent of metadata has been changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void AtMetadataParentChanged(object sender, EventArgs e) {
			base.AtMetadataParentChanged(sender, e);
			((IBusinessValueDataProvider) ViewModel.Metadata.DataProvider).BusinessValueChanged+=AtBusinessValueChanged;
		}

		/// <summary> Gets the ValueVM.Metadata.DataProvider
		/// </summary>
		/// <value>The ValueVM.Metadata.DataProvider or null if not available</value>
		private IBusinessValueDataProvider DataProvider {
			get {
				if (ViewModel == null) return null;
				return (IBusinessValueDataProvider) ViewModel.Metadata.DataProvider;
			}
		}

		private void AtBusinessValueChanged(object sender, DataChangedEventArgs e) { 
			DataProvider.BusinessValue.SettingsChanged+=AtBusinessValueSettingsChanged;
		}

		private void AtBusinessValueSettingsChanged(object sender, ValueSettingsChangedEventArgs valueSettingsChangedEventArgs) { 
			//TODO revalidate after settings changed, resend edit value?!
		}
	}
}