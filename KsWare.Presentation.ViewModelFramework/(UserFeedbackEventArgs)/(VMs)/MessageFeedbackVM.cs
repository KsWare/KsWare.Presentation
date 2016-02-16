using System.Windows;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	public abstract class MessageFeedbackVM<T> : UserFeedbackVM<T> where T:MessageFeedbackEventArgs {

		public MessageFeedbackVM() {
			RegisterChildren(_=>this);

			OkAction    .MːDoAction=DoOkClick;
			YesAction   .MːDoAction=DoYesClick;
			NoAction    .MːDoAction=DoNoClick;
			CancelAction.MːDoAction=DoCancelClick;
		}

		public string Caption { get { return Fields.Get<string>("Caption"); } set { Fields.Set("Caption", value); } }
		public string MessageBoxText { get { return Fields.Get<string>("MessageBoxText"); } set { Fields.Set("MessageBoxText", value); } }

		public ActionVM OkAction { get; private set; }
		public ActionVM YesAction { get; private set; }
		public ActionVM NoAction { get; private set; }
		public ActionVM CancelAction { get; private set; }
		

		private void DoOkClick() {
			Data.DialogResult=MessageBoxResult.OK;
			CloseAction.Execute(null);
		}

		private void DoYesClick() {
			Data.DialogResult=MessageBoxResult.Yes;
			CloseAction.Execute(null);
		}

		private void DoNoClick() {
			Data.DialogResult=MessageBoxResult.No;
			CloseAction.Execute(null);
		}

		private void DoCancelClick() {
			Data.DialogResult=MessageBoxResult.Cancel;
			CloseAction.Execute(null);
		}

		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);if(Data==null) return;

			Caption        = Data.Caption;
			MessageBoxText = Data.MessageBoxText;
			Data.DialogResult = Data.DefaultResult; //??

			var b = Data.Button;
			OkAction.SetCanExecute("NotAvailable",b==MessageBoxButton.OK||b==MessageBoxButton.OKCancel);
			YesAction.SetCanExecute("NotAvailable",b==MessageBoxButton.YesNo||b==MessageBoxButton.YesNoCancel);
			NoAction.SetCanExecute("NotAvailable",b==MessageBoxButton.YesNo||b==MessageBoxButton.YesNoCancel);
			CancelAction.SetCanExecute("NotAvailable",b==MessageBoxButton.OKCancel||b==MessageBoxButton.YesNoCancel);

			//TODO DefaultResult -> Focus
//			switch (Data.DefaultResult) {
//				case MessageBoxResult.OK    : OkAction    .UI.SetFocus();break;
//				case MessageBoxResult.Yes   : YesAction   .UI.SetFocus();break;
//				case MessageBoxResult.No    : NoAction    .UI.SetFocus();break;
//				case MessageBoxResult.Cancel: CancelAction.UI.SetFocus();break;
//			}
	
			//TODO Data.Icon

			//TODO Data.Options
//			if ((Data.Options & MessageBoxOptions.DefaultDesktopOnly ) != 0) { }
//			if ((Data.Options & MessageBoxOptions.RightAlign         ) != 0) { }
//			if ((Data.Options & MessageBoxOptions.RtlReading         ) != 0) { }
//			if ((Data.Options & MessageBoxOptions.ServiceNotification) != 0) { }

			Data.AsyncHandled = true;
		}

		#region Overrides of UserFeedbackVM<T>

		protected override void DoClose() {
			base.DoClose();
			Data.Handled = true;
			if (Data.AsyncCallback != null) Data.AsyncCallback(Data);
		}

		#endregion
	}

	public class MessageFeedbackVM : MessageFeedbackVM<MessageFeedbackEventArgs> {
		
	}
}