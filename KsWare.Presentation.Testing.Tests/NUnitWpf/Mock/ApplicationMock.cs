using System;
using System.Windows;

namespace KsWare.Test.Presentation.NUnitWpf.Mock {

	public class ApplicationMock:Application {

		public static readonly ApplicationMockSettings MockSettings=new ApplicationMockSettings();

		public ApplicationMock() {
			if(MockSettings.Constructor!=null) MockSettings.Constructor(this,EventArgs.Empty);
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			if(MockSettings.OnStartup!=null) MockSettings.OnStartup(this, e);
		}

		protected override void OnSessionEnding(SessionEndingCancelEventArgs e) {
			if(MockSettings.OnSessionEnding!=null) MockSettings.OnSessionEnding(this, e);
			base.OnSessionEnding(e);
		}
	}

	public class ApplicationMockSettings {

		public EventHandler Constructor;
		public StartupEventHandler OnStartup;
		public SessionEndingCancelEventHandler OnSessionEnding;
	}
}
