
namespace KsWare.Presentation {

	/// <summary> Provides UserFeedbackEventArgs for the standard input dialog
	/// </summary>
	public class InputFeedbackEventArgs : UserFeedbackEventArgs {

		public InputFeedbackEventArgs(string prompt=null, string caption=null, string defaultValue=null) {
			Prompt = prompt;
			Caption = caption;
			DefaultValue = defaultValue;
		}

		public override FeedbackType FeedbackType { get{return FeedbackType.Input;} }

		public string Prompt { get; set; }
		public string Caption { get; set; }
		public string DefaultValue { get; set; }

	}
}