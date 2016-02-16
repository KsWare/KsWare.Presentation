using System;
using System.Collections.Generic;

namespace KsWare.Presentation.ViewModelFramework.Rx {

	internal class SimpleObservable<T>:IObservable<T> {
		
		List<IObserver<T>> observers = new List<IObserver<T>>();

		public SimpleObservable() {
			
		}

		public IDisposable Subscribe(IObserver<T> observer) {
			if (!observers.Contains(observer)) observers.Add(observer);

			return new Unsubscriber(observers, observer);
		}


		public void Completion() {
			foreach (var observer in observers) {
				observer.OnCompleted();
			}
			observers.Clear();
		}

		public void Error(Exception error) {
			foreach (var observer in observers) {
				observer.OnError(error);
			}
		}

		public void Raise(T value) {
			foreach (var observer in observers) {
				observer.OnNext(value);
			}
		}

		private class Unsubscriber : IDisposable {
			private List<IObserver<T>> _observers;
			private IObserver<T> _observer;

			public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer) {
				_observers = observers;
				_observer = observer;
			}

			public void Dispose() {
				if (! (_observer == null)) _observers.Remove(_observer);
			}
		}
	}
}
