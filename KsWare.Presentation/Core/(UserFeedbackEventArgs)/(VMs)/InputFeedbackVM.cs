using System;
using System.Windows;
using KsWare.Presentation.Core;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModelFramework {

	public class InputFeedbackVM : UserFeedbackVM<InputFeedbackEventArgs> {

		public InputFeedbackVM() {
			RegisterChildren(_=>this);

			OkAction    .MːDoAction=DoOkClick;
			CancelAction.MːDoAction=DoCancelClick;
		}

		public string Caption { get { return Fields.Get(() => Caption); } set { Fields.Set(() => Caption, value); } }
		public string Prompt { get { return Fields.Get(() => Prompt); } set { Fields.Set(() => Prompt, value); } }

		public ActionVM OkAction { get; private set; }
		public ActionVM CancelAction { get; private set; }
		public StringVM InputValue { get; private set; }

		private void DoOkClick() {
			Data.ReturnValue = InputValue.Value;
			CloseAction.Execute(null);
		}

		private void DoCancelClick() {
			Data.ReturnValue = InputValue.Value;
			CloseAction.Execute(null);
		}

		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);if(Data==null) return;

			Caption        = Data.Caption;
			Prompt = Data.Prompt;
			InputValue.Value = Data.DefaultValue;

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
}