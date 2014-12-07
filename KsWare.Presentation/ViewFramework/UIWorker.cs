using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace KsWare.Presentation.ViewFramework {

	public class UIWorker {

		private Thread m_Thread;

		public Action OnInit { get; set; }
		public Action OnAction { get; set; }
		public Action OnSuccess { get; set; }
		public Action<Exception> OnError { get; set; }
		public Action OnFinally { get; set; }
		public IDispatcher Dispatcher { get; set; }

		public void Start() {
			if (Dispatcher == null) Dispatcher = ApplicationDispatcher.CurrentDispatcher;
			m_Thread=new Thread(Proc) {
				Name = "UIWorker",
				IsBackground = true
			};	
			m_Thread.Start();
		}

		private void Proc() { 
			Dispatcher.Invoke(new Action(delegate {try{OnInit   ();}catch(Exception ex){try{OnError(ex);}catch(Exception ex2){/*TODO*/}}}));
			try{OnAction();}catch(Exception ex){Dispatcher.Invoke(new Action(delegate  {try{OnError(ex);}catch(Exception ex2){/*TODO*/}}));}
			Dispatcher.Invoke(new Action(delegate {try{OnSuccess();}catch(Exception ex){try{OnError(ex);}catch(Exception ex2){/*TODO*/}}}));
			Dispatcher.Invoke(new Action(delegate {try{OnFinally();}catch(Exception ex){try{OnError(ex);}catch(Exception ex2){/*TODO*/}}}));
		}
	}
}
