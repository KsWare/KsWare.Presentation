using System.Windows;

namespace KsWare.Presentation {

	/// <summary>Provides EventArgs for the extended message dialog
	/// </summary>
	public class DetailMessageFeedbackEventArgs:MessageFeedbackEventArgs {

		/// <summary> Initializes a new instance of the <see cref="MessageFeedbackEventArgs"/> class.
		/// </summary>
		/// <param name="messageBoxText">The message box text.</param>
		/// <param name="detailMessage"> The detail message </param>
		/// <param name="caption">The caption.</param>
		/// <param name="button">The button.</param>
		/// <param name="icon">The icon.</param>
		/// <param name="defaultResult">The default result.</param>
		/// <param name="options">The options.</param>
		public DetailMessageFeedbackEventArgs(string messageBoxText, string detailMessage, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options):base(messageBoxText,caption, button, icon, defaultResult, options) {
			DetailMessage=detailMessage;
		}

		public override FeedbackType FeedbackType => FeedbackType.DetailMessage;

		/// <summary> Gets or sets the message detail text.
		/// </summary>
		/// <value>The message box text.</value>
		public string DetailMessage { get; set; }		
	}


}
