using JetBrains.Annotations;

namespace KsWare.Presentation {

	/// <summary> EventArgs for getting a resource string.
	/// </summary>
	[PublicAPI]
	public class StringResourceFeedbackEventArgs : UserFeedbackEventArgs {

		/// <summary> Initializes a new instance of the <see cref="StringResourceFeedbackEventArgs"/> class.
		/// </summary>
		public StringResourceFeedbackEventArgs(string resourceString) {
			ResourceString = resourceString;
		}

		public override FeedbackType FeedbackType => FeedbackType.StringResource;

		/// <summary> Gets or sets the resource string.
		/// </summary>
		/// <value>The resource string.</value>
		public string ResourceString { get; set; }
	}
}
