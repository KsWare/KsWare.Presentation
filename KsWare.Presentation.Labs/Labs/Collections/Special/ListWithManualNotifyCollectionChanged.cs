using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace KsWare.Presentation.Collections.Special {

	public class ListWithManualNotifyCollectionChanged<T> : List<T>, INotifyCollectionChanged {

		private DispatcherOperation _lastDispatcherOperation;

		public void NotifyCollectionChanged() {
			//OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			if (_lastDispatcherOperation != null) {
				//Pending: The operation is pending and is still in the System.Windows.Threading.Dispatcher queue.
				if(_lastDispatcherOperation.Status==DispatcherOperationStatus.Pending) return;
			}
			_lastDispatcherOperation = ApplicationDispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, OnCollectionChanged, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs) {
			var handler = CollectionChanged;
			if(handler==null) return;
			handler(this, eventArgs);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}

}