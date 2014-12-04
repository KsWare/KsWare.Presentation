namespace KsWare.Presentation {

	/// <summary> [EXPERIMENTAL] Provides event arguments for busy handling
	/// </summary>
	public class BusyUserFeedbackEventArgs : UserFeedbackEventArgs {

		/// <summary>
		/// Initializes a new instance of the <see cref="BusyUserFeedbackEventArgs"/> class.
		/// </summary>
		/// <param name="isBusy">a value indicating whether the sender of the user feedback is busy.</param>
		public BusyUserFeedbackEventArgs(bool isBusy) { IsBusy = isBusy; }

		public override FeedbackType FeedbackType { get{return FeedbackType.Busy;} }

		/// <summary> Gets a value indicating whether the sender of the user feedback is busy.
		/// </summary>
		/// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
		public bool IsBusy { get; private set; }
	}

}