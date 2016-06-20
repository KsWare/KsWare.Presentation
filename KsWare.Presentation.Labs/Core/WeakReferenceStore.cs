using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	internal class WeakReferenceStore:INotifyPropertyChanged,IEnumerable<WeakReference> {

		private List<WeakReference> m_WeakReferences =new List<WeakReference>();
		
		public void Add(WeakReference r) {
			Cleanup();
			m_WeakReferences.Add(r);
		}

		public void Add(object o) {
			Cleanup();
			m_WeakReferences.Add(new WeakReference(o));
		}

		public int Count { get { return m_WeakReferences.Count;} }

		public List<object> Targets {
			get {
				var targets = new List<object>();
				lock (m_WeakReferences) {
					foreach (var r in m_WeakReferences) {
						object o;
						if (!r.IsAlive) continue; else try { o = r.Target;} catch{continue;} 
						targets.Add(o);
					}
				}
				return targets;
			}
		} 

		private void Cleanup() {
			var count = m_WeakReferences.Count;
			for (int i = 0; i < m_WeakReferences.Count; i++) {
				if (!m_WeakReferences[i].IsAlive) m_WeakReferences.RemoveAt(i--);
			}
			if(m_WeakReferences.Count!=count) OnPropertyChanged("Count");
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

			private WeakReferenceStore m_WeakReferenceList;
			private int m_CurrentIndex;

			public Enumerator(WeakReferenceStore weakReferenceList) {
				m_WeakReferenceList = weakReferenceList;
			}

			#region Implementation of IDisposable

			public void Dispose() { m_WeakReferenceList = null; }

			#endregion

			#region Implementation of IEnumerator

			public bool MoveNext() {
				while (true) {
					m_CurrentIndex++;
					if (m_CurrentIndex >= m_WeakReferenceList.m_WeakReferences.Count) return false;
					var r = m_WeakReferenceList.m_WeakReferences[m_CurrentIndex];
					if(r.IsAlive) {Current = r;return true;}
				}
			}

			public void Reset() { m_CurrentIndex = -1; }
			public WeakReference Current { get; private set; }

			object IEnumerator.Current {get { return Current; } }

			#endregion
		}
	}
}