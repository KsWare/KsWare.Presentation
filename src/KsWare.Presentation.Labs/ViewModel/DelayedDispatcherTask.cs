using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation {

	/// <summary> [EXPERIMENTAL]
	/// </summary>
	public class DelayedDispatcherTask:IDisposable,IAsyncResult {

		private readonly IDispatcher _dispatcher;
		private readonly TimeSpan _delay;
		private readonly Action _action;
		private static long s_BeginInvokeDelayCount;

		private object _asyncState;
		private WeakReference _vmRef;
		private Thread _thread;
		private WaitHandle _asyncWaitHandle;
		private DispatcherOperation _dispatcherOperation;
		private readonly object _dispatcherOperationLock=new object();


		public DelayedDispatcherTask(IObjectVM vm, TimeSpan delay, Action action) {
			_delay      = delay;
			_action     = action;
			_vmRef      = vm==null?null:new WeakReference(vm);
			_dispatcher = ApplicationVM.Current.Dispatcher;

			var ts = new ThreadStart(Run);
			_thread = new Thread(ts){
				IsBackground = true,
				Name = "BeginInvokeDelay#"+(++s_BeginInvokeDelayCount)
			};

			//m_AsyncWaitHandle=new ManualResetEvent(false);
			_thread.Start();
		}

		private void Run() {
			try {
				Thread.Sleep(_delay);

				if (Application.Current == null) return;
				lock (_dispatcherOperationLock) _dispatcherOperation = _dispatcher.BeginInvoke(_action);
			}
			catch (ThreadAbortException ex) {
				Exception = ex;
			}
			catch (ThreadInterruptedException ex) {
				Exception = ex;
			}
			catch (Exception ex) {
				Exception = ex;
			}
			finally {
				lock (this) {
					IsCompleted = true;
					if(_asyncWaitHandle!=null) ((ManualResetEvent) _asyncWaitHandle).Set();					
				}
			}
		}

		void IDisposable.Dispose() {
			if (!IsCompleted) {
				IsCanceled = true;
				_thread.Abort("DisposeRequest");
			}
		}

		public void Cancel() {
			if (!IsCompleted) {
				IsCanceled = true;
				_thread.Abort("CancelRequest");
			}
		}

		public bool IsCompleted { get; private set; }
		public bool IsCanceled { get; private set; }
		public Exception Exception { get; private set; }

		public WaitHandle AsyncWaitHandle {
			get {
				if (_asyncWaitHandle == null) {
					lock (this) {
						if(IsCompleted) return new ManualResetEvent(true);
						_asyncWaitHandle=new ManualResetEvent(false);						
					}
				}
				return _asyncWaitHandle;
			} 
		}

		object IAsyncResult.AsyncState => _asyncState;

		bool IAsyncResult.CompletedSynchronously => false;

		public DispatcherOperation DispatcherOperation { get { lock (_dispatcherOperationLock) return _dispatcherOperation; } }

		public DispatcherOperationStatus Status {
			get { lock (_dispatcherOperationLock) return _dispatcherOperation == null ? DispatcherOperationStatus.Pending : _dispatcherOperation.Status; }
		}

	}


	public static class DispatcherWrapperExtension {

		public static DelayedDispatcherTask BeginInvoke(this DispatcherWrapper wrapper, TimeSpan delay, Action method) {return new DelayedDispatcherTask(null, delay, method); }


	}
}