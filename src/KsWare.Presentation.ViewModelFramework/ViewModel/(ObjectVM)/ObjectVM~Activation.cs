namespace KsWare.Presentation.ViewModelFramework {

	public partial interface IObjectVM {

		/// <summary> Notifies this view model has been activated
		/// </summary>
		/// <param name="refferer">The refferer.</param>
		void NotifyActivated(IObjectVM refferer);

		/// <summary> Called when this view model has been deactivated.
		/// </summary>
		void NotifyDeactivated();

	}

	public partial class ObjectVM {

		/// <summary> Notifies this view model has been activated
		/// </summary>
		/// <param name="refferer">The refferer.</param>
		public void NotifyActivated(IObjectVM refferer) {
			OnActivated(refferer);
		}

		/// <summary> Notifies this view model has been activated
		/// </summary>
		public void NotifyDeactivated() {
			OnDeactivated();
		}

		/// <summary> Called when this view model has been activated.
		/// </summary>
		/// <param name="refferer">The refferer.</param>
		protected virtual void OnActivated(IObjectVM refferer) {
			
		}

		/// <summary> Called when this view model has been deactivated.
		/// </summary>
		protected virtual void OnDeactivated() {
			
		}
	}

}
