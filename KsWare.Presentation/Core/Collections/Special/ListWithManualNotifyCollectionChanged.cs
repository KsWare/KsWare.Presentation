using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace KsWare.Presentation.Collections.Special {

	public class ListWithManualNotifyCollectionChanged<T> : List<T>, INotifyCollectionChanged {

		private DispatcherOperation m_LastDispatcherOperation;

		public void NotifyCollectionChanged() {
			//OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			if (m_LastDispatcherOperation != null) {
				//Pending: The operation is pending and is still in the System.Windows.Threading.Dispatcher queue.
				if(m_LastDispatcherOperation.Status==DispatcherOperationStatus.Pending) return;
			}
			m_LastDispatcherOperation = ApplicationDispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, OnCollectionChanged, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs) {
			var handler = CollectionChanged;
			if(handler==null) return;
			handler(this, eventArgs);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}

}