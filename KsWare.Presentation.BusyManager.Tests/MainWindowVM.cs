using System;
using System.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.BusyManagerTestApp {

	public class MainWindowVM : WindowVM {

		public MainWindowVM() {
			RegisterChildren(()=>this);

			Button1Action.MːDoAction = DoButton1;
			Button2Action.MːDoAction = DoButton2;
			Button3Action.MːDoAction = DoButton3;
		}

		private void DoButton1() {
			using (BusyManager.Instance.Context(Button1Action)) {
				/*do anything long operation*/

				// optional continue the long operation asynchronous.
				new Thread(DoButton1Proc).Start();
				BusyManager.Instance.ContinueAsync(Button1Action);
			}

		}

		private void DoButton2() {
			using (BusyManager.Instance.Context(Button2Action)) {
				/*do anything long operation*/

				// optional continue the long operation asynchronous.
				new Thread(DoButton2Proc).Start();
				BusyManager.Instance.ContinueAsync(Button2Action);
			}
		}

		private void DoButton3() {
			using (BusyManager.Instance.Context(Button3Action)) {
				/*do anything long operation*/

				// optional continue the long operation asynchronous.
				new Thread(DoButton3Proc).Start();
				BusyManager.Instance.ContinueAsync(Button3Action);
			}
		}

		private void DoButton1Proc() {
			try {
				ApplicationDispatcher.BeginInvoke(() => OutputText.Value = "Action 1 background.");
				// continue the long operation asynchronous.
				Thread.Sleep(5000);
			}
			finally {
				BusyManager.Instance.EndAsync(Button1Action);
				ApplicationDispatcher.BeginInvoke(() => OutputText.Value = "Action 1 done.");
			}
		}

		private void DoButton2Proc() {
			try {
				ApplicationDispatcher.BeginInvoke(() => OutputText.Value = "Action 2 background.");
				// continue the long operation asynchronous.
				Thread.Sleep(5000);
			}
			finally {
				BusyManager.Instance.EndAsync(Button2Action);
				ApplicationDispatcher.BeginInvoke(() => OutputText.Value = "Action 2 done.");
			}
		}

		private void DoButton3Proc() {
			try {
				ApplicationDispatcher.BeginInvoke(() => OutputText.Value = "Action 3 background.");
				// continue the long operation asynchronous.
				Thread.Sleep(5000);
			}
			finally {
				BusyManager.Instance.EndAsync(Button3Action);
				ApplicationDispatcher.BeginInvoke(() => OutputText.Value = "Action 3 done.");
			}
		}

		public ActionVM Button1Action { get; [UsedImplicitly] private set; }

		public ActionVM Button2Action { get; [UsedImplicitly] private set; }

		public ActionVM Button3Action { get; [UsedImplicitly] private set; }

		public StringVM OutputText { get; [UsedImplicitly] private set; }
	}

}
