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
		protected DisposablesCollection D {get => Disposables; set {}}

	}

	public class DisposablesCollection {

		private List<object> _internalList=new List<object>();

		public T Add<T>(T disposableObject) where T : class {
			if(_internalList==null) return disposableObject;
			_internalList.Add(disposableObject);
			return disposableObject;
		}

		public void Add<T>(params T[] disposableObjects) where T:class {
			if(_internalList==null) return;
			if(disposableObjects!=null) _internalList.AddRange(disposableObjects);
		}

		public void Remove<T>(T obj) where T:class { _internalList.Remove(obj); }

		public void Dispose() {
			foreach (var o in _internalList) {
				var d = (IDisposable) o;
				if(d!=null) d.Dispose();
			}
			_internalList.Clear();
			_internalList = null;
		}

		public T RegisterEventHandler<T>( T eventHandler) {
//			_InternalList.Add(new Disposable(eventHandler));
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
