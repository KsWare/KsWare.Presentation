using System;
using System.Net.Mime;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation {

	/// <summary> [EXPERIMENTAL]
	/// </summary>
	public class DelayedDispatcherTask:IDisposable,IAsyncResult {

		private readonly IDispatcher m_Dispatcher;
		private readonly TimeSpan m_Delay;
		private readonly Action m_Action;
		private static long s_BeginInvokeDelayCount;

		private object m_AsyncState;
		private WeakReference m_VMRef;
		private Thread m_Thread;
		private WaitHandle m_AsyncWaitHandle;
		private DispatcherOperation m_DispatcherOperation;
		private object m_DispatcherOperationLock=new object();


		public DelayedDispatcherTask(IObjectVM vm, TimeSpan delay, Action action) {
			m_Delay      = delay;
			m_Action     = action;
			m_VMRef      = vm==null?null:new WeakReference(vm);
			m_Dispatcher = ApplicationVM.Current.Dispatcher;

			var ts = new ThreadStart(Run);
			m_Thread = new Thread(ts){
				IsBackground = true,
				Name = "BeginInvokeDelay#"+(++s_BeginInvokeDelayCount)
			};

			//m_AsyncWaitHandle=new ManualResetEvent(false);
			m_Thread.Start();
		}

		private void Run() {
			try {
				Thread.Sleep(m_Delay);

				if (Application.Current == null) return;
				lock (m_DispatcherOperationLock) m_DispatcherOperation = m_Dispatcher.BeginInvoke(m_Action);
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
					if(m_AsyncWaitHandle!=null) ((ManualResetEvent) m_AsyncWaitHandle).Set();					
				}
			}
		}

		void IDisposable.Dispose() {
			if (!IsCompleted) {
				IsCanceled = true;
				m_Thread.Abort("DisposeRequest");
			}
		}

		public void Cancel() {
			if (!IsCompleted) {
				IsCanceled = true;
				m_Thread.Abort("CancelRequest");
			}
		}

		public bool IsCompleted { get; private set; }
		public bool IsCanceled { get; private set; }
		public Exception Exception { get; private set; }

		public WaitHandle AsyncWaitHandle {
			get {
				if (m_AsyncWaitHandle == null) {
					lock (this) {
						if(IsCompleted) return new ManualResetEvent(true);
						m_AsyncWaitHandle=new ManualResetEvent(false);						
					}
				}
				return m_AsyncWaitHandle;
			} 
		}

		object IAsyncResult.AsyncState { get { return m_AsyncState; } }

		bool IAsyncResult.CompletedSynchronously { get { return false; } }

		public DispatcherOperation DispatcherOperation { get { lock (m_DispatcherOperationLock) return m_DispatcherOperation; } }

		public DispatcherOperationStatus Status {
			get { lock (m_DispatcherOperationLock) return m_DispatcherOperation == null ? DispatcherOperationStatus.Pending : m_DispatcherOperation.Status; }
		}

	}


	public static class DispatcherWrapperExtension {

		public static DelayedDispatcherTask BeginInvoke(this DispatcherWrapper wrapper, TimeSpan delay, Action method) {return new DelayedDispatcherTask(null, delay, method); }


	}
}