using System;
using System.Diagnostics;
using KsWare.Presentation.InternalStatistics;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.WeakEventsTestApp {

	public class MainWindowVM:WindowVM {

		static MainWindowVM s_instance;
		private TestViewModel1VM _testViewModel;
		private WeakReference _weakTestViewModel;

		public static MainWindowVM Instance {get { return s_instance ?? (s_instance = new MainWindowVM()); } }

		public MainWindowVM() {

			var wo = new WeakReference(new object());
			if (wo.IsAlive) { /*alles in Ordnung*/ }
			GC.Collect(); 
			if(wo.IsAlive) throw new Exception("GC.Collec() failed!??"); // Debug x86

			StatisticsWindowVM.Instance.Show();
			RegisterChildren(_=>this);

			CreateAction.MːDoAction = () => {
				_weakTestViewModel=new WeakReference(_testViewModel = new TestViewModel1VM());
//				DoForget=_TestViewModel.IsSelectedChangedEvent;
				_testViewModel.IsSelectedChangedEvent.Register(null, "AtIsSelectedChanged",AtIsSelectedChangedEvent);
			};
			DeleteAction.MːDoAction = () => {
//				_TestViewModel.IsSelectedChangedEvent.Release(this,"AtIsSelectedChanged");
//				_TestViewModel.Dispose();
				_testViewModel = null; 
				GC.Collect(); 
				EventManager.Collect();
				if(_weakTestViewModel.IsAlive) Debug.WriteLine("m_WeakTestViewModel.IsAlive");
			};
		}

		private void DoNothing(object o) {  }
		private object DoForget { set { } }

		private void AtIsSelectedChangedEvent(object sender, EventArgs eventArgs) {  }

		public ActionVM CreateAction { get; private set; }
		public ActionVM DeleteAction { get; private set; }

	}

	public class TestViewModel1VM:ObjectVM {

		public TestViewModel1VM() {
			//this.IsSelectedChangedEvent.Register(null, "AtIsSelectedChanged",AtIsSelectedChangedEvent);
		}

		~TestViewModel1VM() {
			Dispose(false);
		}


		protected override void Dispose(bool explicitDisposing) {
			Debug.WriteLine("TestViewModel1VM Dispose "+explicitDisposing);
			base.Dispose(explicitDisposing);
		}

		private void AtIsSelectedChangedEvent(object sender, EventArgs eventArgs) {  }

	}

}
