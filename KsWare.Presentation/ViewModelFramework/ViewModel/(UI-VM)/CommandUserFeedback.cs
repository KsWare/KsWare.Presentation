using System.Windows.Input;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides a <see cref="ICommand"/> user feedback
	/// </summary>
	public class CommandUserFeedback : UserFeedbackEventArgs {

		/// <summary> Initializes a new instance of the <see cref="CommandUserFeedback"/> class.
		/// </summary>
		public CommandUserFeedback() {}

		/// <summary> Initializes a new instance of the <see cref="CommandUserFeedback"/> class.
		/// </summary>
		/// <param name="command">The command.</param>
		public CommandUserFeedback(ICommand command) {
			Command=command;
		}

		/// <summary> Initializes a new instance of the <see cref="CommandUserFeedback"/> class.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="commandParameter">The command parameter.</param>
		public CommandUserFeedback(ICommand command, object commandParameter) {
			Command = command;
			CommandParameter = commandParameter;
		}

		public override FeedbackType FeedbackType { get{return FeedbackType.Command;} }

		/// <summary> Gets or sets the command.
		/// </summary>
		/// <value> The command. </value>
		public ICommand Command { get; set; }

		/// <summary> Gets or sets the command parameter.
		/// </summary>
		/// <value> The command parameter. </value>
		public object CommandParameter { get; set; }

	}
}