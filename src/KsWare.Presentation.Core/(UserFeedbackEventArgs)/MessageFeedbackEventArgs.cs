using System;
using System.Reflection;
using System.Windows;

namespace KsWare.Presentation {

	/// <summary> Provides UserFeedbackEventArgs for the standard MessageDialog
	/// </summary>
	public class MessageFeedbackEventArgs : UserFeedbackEventArgs {

		/// <summary> Initializes a new instance of the <see cref="MessageFeedbackEventArgs"/> class.
		/// </summary>
		[Obsolete("Avoid using this contructor (MessageFeedbackEventArgs)")]
		public MessageFeedbackEventArgs(string messageBoxText) {
			MessageBoxText = messageBoxText;
			Caption       = ((AssemblyTitleAttribute)Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute),true)[0]).Title;
			Button        = MessageBoxButton.OK;
			Icon          = MessageBoxImage.None;
			DefaultResult = MessageBoxResult.OK;
			Options       = MessageBoxOptions.None;
		}

		/// <summary> Initializes a new instance of the <see cref="MessageFeedbackEventArgs"/> class.
		/// </summary>
		/// <param name="messageBoxText">The message box text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="button">The button.</param>
		/// <param name="icon">The icon.</param>
		/// <param name="defaultResult">The default result.</param>
		/// <param name="options">The options.</param>
		public MessageFeedbackEventArgs(string messageBoxText, string caption, MessageBoxButton button=MessageBoxButton.OK, MessageBoxImage icon=MessageBoxImage.None, MessageBoxResult defaultResult=MessageBoxResult.OK, MessageBoxOptions options=MessageBoxOptions.None) {
			MessageBoxText = messageBoxText;
			Caption = caption;
			Button = button;
			Icon = icon;
			DefaultResult = defaultResult;
			Options = options;
		}

		public override FeedbackType FeedbackType => FeedbackType.Message;

		/// <summary> Gets or sets the message box text.
		/// </summary>
		/// <value>The message box text.</value>
		public string MessageBoxText { get; set; }

		/// <summary> Gets or sets the caption.
		/// </summary>
		/// <value>The caption.</value>
		public string Caption { get; set; }

		/// <summary> Gets or sets the button.
		/// </summary>
		/// <value>The button.</value>
		public MessageBoxButton Button { get; set; }

		/// <summary> Gets or sets the icon.
		/// </summary>
		/// <value>The icon.</value>
		public MessageBoxImage Icon { get; set; }

		/// <summary> Gets or sets the default result.
		/// </summary>
		/// <value>The default result.</value>
		public MessageBoxResult DefaultResult { get; set; }

		/// <summary> Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		public MessageBoxOptions Options { get; set; }

		/// <summary> Gets or sets the dialog result.
		/// </summary>
		/// <value>The dialog result.</value>
		public MessageBoxResult DialogResult { get; set; }


		public MessageBoxResult ShowDialog() {
			DialogResult=MessageBox.Show(MessageBoxText,Caption,Button,Icon,DefaultResult,Options);
			return DialogResult;
		}
	}

}