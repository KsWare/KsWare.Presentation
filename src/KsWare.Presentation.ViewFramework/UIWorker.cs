using System;
using System.Threading;

namespace KsWare.Presentation.ViewFramework {

	public class UIWorker {

		private Thread _thread;

		public Action OnInit { get; set; }
		public Action OnAction { get; set; }
		public Action OnSuccess { get; set; }
		public Action<Exception> OnError { get; set; }
		public Action OnFinally { get; set; }
		public IDispatcher Dispatcher { get; set; }

		public void Start() {
			if (Dispatcher == null) Dispatcher = ApplicationDispatcher.Instance;
			_thread=new Thread(Proc) {
				Name = "UIWorker",
				IsBackground = true
			};	
			_thread.Start();
		}

		private void Proc() { 
			Dispatcher.Invoke(new Action(delegate {try{OnInit   ();}catch(Exception ex){try{OnError(ex);}catch(Exception){/*TODO*/}}}));
			try{OnAction();}catch(Exception ex){Dispatcher.Invoke(new Action(delegate  {try{OnError(ex);}catch(Exception){/*TODO*/}}));}
			Dispatcher.Invoke(new Action(delegate {try{OnSuccess();}catch(Exception ex){try{OnError(ex);}catch(Exception){/*TODO*/}}}));
			Dispatcher.Invoke(new Action(delegate {try{OnFinally();}catch(Exception ex){try{OnError(ex);}catch(Exception){/*TODO*/}}}));
		}
	}
}
