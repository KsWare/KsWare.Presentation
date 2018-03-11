using System;
using System.Windows;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary>  [EXPERIMENTAL] Provides easy to use user feedback methods
	/// </summary>
	public class UserFeedbackManager:Singleton<UserFeedbackManager> {


		public object InputRequired(IObjectVM sender, string inputField) {
			sender.RequestUserFeedback(new MessageFeedbackEventArgs("Input required: "+inputField, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
			return null;
		}

		public object InputRequired(IObjectBM sender, string inputField) {
			sender.RequestUserFeedback(new MessageFeedbackEventArgs("Input required: "+inputField, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
			return null;
		}

		public BusyManager Busy { get { return BusyManager.Instance; } }

		public void Exception(IObjectVM sender, Exception exception) { sender.RequestUserFeedback(new ExceptionFeedbackEventArgs(exception)); }

	}

}
