using System;
using System.Collections.Generic;

namespace KsWare.Presentation.ViewModelFramework {

	partial class ObjectVM /*:  */ {

		private void InitPartDisposables() {
			Disposables=new DisposablesCollection();
		}

		public DisposablesCollection Disposables { get; private set; } 

		//  GC.KeepAlive()
		/// <summary>
		/// Stores the object in the disposables collection (<see cref="Disposables"/>). Always use <c>D+=myObjectInstance</c>
		/// </summary>
		protected DisposablesCollection D {get { return Disposables; }set {}}

	}

	public class DisposablesCollection {

		private List<object> m_InternalList=new List<object>();

		public T Add<T>(T disposableObject) where T : class {
			if(m_InternalList==null) return disposableObject;
			m_InternalList.Add(disposableObject);
			return disposableObject;
		}

		public void Add<T>(params T[] disposableObjects) where T:class {
			if(m_InternalList==null) return;
			if(disposableObjects!=null) m_InternalList.AddRange(disposableObjects);
		}

		public void Remove<T>(T obj) where T:class { m_InternalList.Remove(obj); }

		public void Dispose() {
			foreach (var o in m_InternalList) {
				var d = (IDisposable) o;
				if(d!=null) d.Dispose();
			}
			m_InternalList.Clear();
			m_InternalList = null;
		}

		public T RegisterEventHandler<T>( T eventHandler) {
//			m_InternalList.Add(new Disposable(eventHandler));
			return eventHandler;
		}

		public class Disposable:IDisposable {

			public Disposable(object eventHandler) {  }

			public void Dispose() {
				
			}
		}

		// Overloadable Operators:  +, -, *, /, %, &, |, ^, <<, >>
		public static DisposablesCollection operator +(DisposablesCollection col, object d) { col.Add(d); return col;}

	}
}
