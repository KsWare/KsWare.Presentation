using System;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> [EXPERIMENTAL] AsyncActionVM
	/// </summary>
	/// <remarks></remarks>
	public class AsyncActionVM:ActionVM {

		/// <summary> Initializes a new instance of the <see cref="ActionActiveProgressVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public AsyncActionVM() {}

		protected override void OnActionProviderChanged() {
			base.OnActionProviderChanged();

			Metadata.ActionProvider.BeforeExecute+=AtBeforeExecute;
			Metadata.ActionProvider.AfterExecute+=AtAfterExecute;
		}

		private void AtBeforeExecute(object sender, EventArgs eventArgs) {
			IsActive = true;
		}

		private void AtAfterExecute(object sender, EventArgs eventArgs) {
			IsActive = false;
		}

	}


}
