using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	internal class WeakReferenceStore:INotifyPropertyChanged,IEnumerable<WeakReference> {

		private List<WeakReference> _weakReferences =new List<WeakReference>();
		
		public void Add(WeakReference r) {
			Cleanup();
			_weakReferences.Add(r);
		}

		public void Add(object o) {
			Cleanup();
			_weakReferences.Add(new WeakReference(o));
		}

		public int Count { get { return _weakReferences.Count;} }

		public List<object> Targets {
			get {
				var targets = new List<object>();
				lock (_weakReferences) {
					foreach (var r in _weakReferences) {
						object o;
						if (!r.IsAlive) continue; else try { o = r.Target;} catch{continue;} 
						targets.Add(o);
					}
				}
				return targets;
			}
		} 

		private void Cleanup() {
			var count = _weakReferences.Count;
			for (int i = 0; i < _weakReferences.Count; i++) {
				if (!_weakReferences[i].IsAlive) _weakReferences.RemoveAt(i--);
			}
			if(_weakReferences.Count!=count) OnPropertyChanged(nameof(Count));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(/*[CallerMemberName]*/ string propertyName = null) {
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#region Implementation of IEnumerable

		public IEnumerator<WeakReference> GetEnumerator() { return new Enumerator(this); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		#endregion

		private class Enumerator : IEnumerator<WeakReference> {

			private WeakReferenceStore _weakReferenceList;
			private int _currentIndex;

			public Enumerator(WeakReferenceStore weakReferenceList) {
				_weakReferenceList = weakReferenceList;
			}

			#region Implementation of IDisposable

			public void Dispose() { _weakReferenceList = null; }

			#endregion

			#region Implementation of IEnumerator

			public bool MoveNext() {
				while (true) {
					_currentIndex++;
					if (_currentIndex >= _weakReferenceList._weakReferences.Count) return false;
					var r = _weakReferenceList._weakReferences[_currentIndex];
					if(r.IsAlive) {Current = r;return true;}
				}
			}

			public void Reset() { _currentIndex = -1; }
			public WeakReference Current { get; private set; }

			object IEnumerator.Current {get { return Current; } }

			#endregion
		}
	}
}