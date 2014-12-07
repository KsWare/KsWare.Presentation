using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation {

	/// <summary> Provides event utilities
	/// </summary>
	public static partial class EventUtil {

		private static readonly List<EventObject> s_EventObjects=new List<EventObject>();
		private static long InvocationCount;

		/// <summary> Gets a value indicating whether exceptions will be rethrown.
		/// </summary>
		/// <value><see langword="true"/> if exceptions will be rethrown; otherwise, <see langword="false"/>. </value>
//		public static bool RethrowExceptions{get;set;}
		public static bool RethrowExceptions{get {return true;}}

		/// <summary> Helps to safely raise an event.
		/// </summary>
		/// <param name="delegate">The event delegate (<see cref="EventHandler"/>, <see cref="EventHandler{T}"/></param>
		/// <param name="sender">The sender</param>
		/// <param name="e">The event arguments</param>
		/// <param name="uniqueId">A unique ID</param>
		/// <exception cref="MultiTargetInvocationException"></exception>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="uniqueId")]
		public static void Raise(Delegate @delegate, object sender, EventArgs e, string uniqueId) { 
			if(@delegate==null){return;} 

//			try{ @delegate.DynamicInvoke(new[] {sender, e});}
//			catch (Exception ex) {
//				if(Assembly.GetEntryAssembly()==null) {
//					Debug.WriteLine("=>An unhandled exception has occurred. Trying to continue."+"\r\n"+ex.ToString());
//					return;
//				}
//				throw;
//			}
//			return;

			var exceptions=new List<TargetInvocationException>();
			var invocationList = @delegate.GetInvocationList();

			#region TRACE
//			Debug.WriteLine("=>Raise event: " + "("+invocationList.Length+" target"+(invocationList.Length!=1?"s":"")+")");
//			foreach (var d in invocationList) {
//				var targetType=d.Target.GetType().FullName;
//				var method=d.Method.ToString();
//				Debug.WriteLine("=>\t"+"Target: "+targetType+ " " + method);
//			}
//			Debug.WriteLine("=>\t"+"Raising method: "+sender.GetType().FullName+"."+new StackFrame(1).GetMethod());
//			
			// MemberPath (ObjectVM/ObjectBM)
//			var memberPathProperty=sender.GetType().GetProperty("MemberPath");
//			if(memberPathProperty!=null){
//				var memberPath=memberPathProperty.GetValue(sender,null);
//				Debug.WriteLine("=>\t"+"MemberPath: "+memberPath);
//			}
			#endregion

			var isInvokeRequired = ApplicationDispatcher.IsInvokeRequired;
			foreach (var d in invocationList) {
				#region DEBUG
//				Debug.WriteLine(string.Format("=>Raise event: #{0} {1} {2}",
//					++InvocationCount, 
//					DebugUtil.FormatTypeName(@delegate),
//					"Target: "+ DebugUtil.FormatTypeName(d.Target)+"."+d.Method.Name
				//				));
				#endregion
				// EXPERIMENTAL ==> 
				// workaround for TargetInvocationException --> InvalidOperationException: 
				// The calling thread cannot access this object because a different thread owns it.
				// e.g. System.Windows.Input.CanExecuteChangedEventManager+HandlerSink, PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
				if      (isInvokeRequired && d.Target.GetType().FullName == "System.Windows.Input.CanExecuteChangedEventManager+HandlerSink") InvokeAppDispatcher(d, sender, e, exceptions);
				else if (isInvokeRequired && d.Target.GetType().FullName == "System.Windows.Data.ListCollectionView"                        ) InvokeAppDispatcher(d, sender, e, exceptions);
		/*??*/	else if (isInvokeRequired && d.Target.GetType().Assembly.GetName().Name=="PresentationCore"                                 ) InvokeAppDispatcher(d, sender, e, exceptions);
				else if (isInvokeRequired) InvokeDynamic(d, sender, e, exceptions);
				//<==
				else InvokeDynamic(d, sender, e, exceptions);
			}

			if(exceptions.Count>0) {
				var ex = new MultiTargetInvocationException(exceptions);
				#region DESIGNER
				if (ObjectVM.IsInDesignMode) {

//					Trace.Write("=>Begin:" + exceptionCount + "######################################################################################################################################"+"\n");
					Trace.Write("=>An unhandled exception has occured. Trying to continue." + "\n" + ex.ToString()+"\n");
					foreach (var exception in exceptions) {
						Trace.Write("--------------------------------------------------------------------------------------------------------------------------------------"+"\n"
							+exception.ToString()+"\n");
					}
//					Trace.Write("=>End :" + exceptionCount + "######################################################################################################################################"+"\n");
					return;
				}
				#endregion
				throw ex;
			}
		}

		private static void InvokeAppDispatcher(Delegate d, object sender, EventArgs e, List<TargetInvocationException> exceptions) {
			ApplicationDispatcher.Invoke(() => InvokeDynamic(d,sender,e,exceptions));
		}

		private static void InvokeDynamic(Delegate d, object sender, EventArgs e, List<TargetInvocationException> exceptions) {
			try {
//				d.DynamicInvoke(sender, e);
				d.Method.Invoke(d.Target, new [] {sender, e});

			}
			catch (TargetInvocationException ex) {
				exceptions.Add(ex);
//				Trace.WriteLine("=>Start:" + invocationCount + "######################################################################################################################################");
//				Trace.WriteLine("=>" + ex.InnerException);
//				Trace.WriteLine("=>Stop :" + invocationCount + "######################################################################################################################################");

//				var innerEx = ex.InnerException;
//					Log.AddInternal(1,"ERROR","Invoke event method failed!",innerEx,uniqueId,
//					new LP("EventHandler",d.Method.DeclaringType.FullName+" "+d.Method.ToString())
//				);
				Debug.WriteLine("ERROR: Unhandled exception in "+ d.Method.ToString() + " in " + d.Method.DeclaringType.FullName + "\r\n\t" +
					ex.StackTrace[0]);
			} catch(Exception ex) {
				/* Should not occur */
				Debugger.Break();
				throw new NotImplementedException("{F12A1016-F2AB-4939-8BB9-4B833AD4F5C2}");
			}
		}

//NOT USED
//		/// <summary> Helps to safely raise an event.
//		/// </summary>
//		/// <param name="delegate">The event delegate (<see cref="Action"/>, <see cref="ThreadStart"/>) </param>
//		/// <param name="uniqueId">A unique ID</param>
//		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
//		public static void Raise(Delegate @delegate, string uniqueId) {
//			if(@delegate==null) {
//				return;
//			} else {
//				Exception rethrow=null;
//				foreach (var d in @delegate.GetInvocationList()) {
//					try {d.DynamicInvoke();}
//					catch (TargetInvocationException ex) {
//						var innerEx = ex.InnerException;
//						if(rethrow==null) rethrow = innerEx;
//						Log.AddInternal(1,"ERROR","Invoke event method failed!",innerEx,uniqueId,
//							new LP("EventHandler",d.Method.DeclaringType.FullName+" "+d.Method.ToString())
//						);
//						if(RethrowExceptions) throw;
//					}
//				}
//			}
//		}

		/// <summary> Raises the Disposed-event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="disposedEvent">The disposed event.</param>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static void RaiseDisposedEvent(object sender, EventHandler disposedEvent) {
			if (disposedEvent == null) return;
			var args = EventArgs.Empty;
			Delegate[] invocationList = disposedEvent.GetInvocationList();
			foreach (Delegate @delegate in invocationList) {
				try{@delegate.DynamicInvoke(sender, args);}
				catch (Exception ex) {
					//REVIEW: use log
					Debug.WriteLine("=>WARNING: Invoking Disposed event handler failed! " +
					"\r\n\t"+"Exception:     " + ex.GetType().FullName +
					"\r\n\t"+"Message:       " + ex.Message+
					"\r\n\t"+"Target Type:   " + @delegate.Target.GetType().FullName+
					"\r\n\t"+"Target Method: " + @delegate.Method.ToString()
					,sender.GetType().Name);
					//### Break
					if (Debugger.IsAttached)
						Debugger.Break();
				}
			}
		}


		public static void Register(object owner, string key, object obj) {
			s_EventObjects.Add(new EventObject(owner,key,obj));
		}
		public static void Register<T>(object owner, string key, T obj, Action<T> action) {
			s_EventObjects.Add(new EventObject(owner,key,obj));
			action(obj);
		}

		public static object Release(object owner, string key) {
			for(int i=0;i<s_EventObjects.Count;i++) {
				var eo=s_EventObjects[i];
				if(eo.Key== key) {
					if(eo.OwnerRef.IsAlive && eo.OwnerRef.Target==owner) {
						s_EventObjects.Remove(eo);
						if(eo.OwnerRef.IsAlive) {
							return eo.ObjectRef.Target;
						} else {
							return null;
						}
					}
				}
			}
			return null;
		}

		private static ArrayList ReleaseAll(object owner, string key) {
			var objs = new ArrayList();
			for(int i=0;i<s_EventObjects.Count;i++) {
				var eo=s_EventObjects[i];
				if(eo.Key== key) {
					if(eo.OwnerRef.IsAlive && eo.OwnerRef.Target==owner) {
						s_EventObjects.Remove(eo);
						if(eo.ObjectRef.IsAlive) {
							objs.Add(eo.ObjectRef.Target);
						}
					}
				}
			}
			return objs;
		}

		public static object Release(object owner, string key, object obj) {
			for(int i=0;i<s_EventObjects.Count;i++) {
				var eo=s_EventObjects[i];
				if(eo.Key== key) {
					if(eo.OwnerRef.IsAlive && eo.OwnerRef.Target==owner) {
						if(eo.ObjectRef.IsAlive && eo.ObjectRef.Target==obj) {
							return eo.ObjectRef.Target;
						}
					}
				}
			}
			return null;
		}

		public static void ReleaseAll<T>(object owner, string key, Action<T> action) {
			foreach (var obj in ReleaseAll(owner, key)) action((T) obj);
		}

		private static void RegisterList<T>(object owner, string key, IEnumerable items, Action<T> action) {
			if(items==null)return;
			foreach (var item in items) {
				action((T) item);
				Register(owner, key, item);
			}
		}

		private static void ReleaseList<T>(object owner, string key, IEnumerable items, Action<T> action) {
			if(items==null)return;
			foreach (var item in items) {
				Release(owner, key, item);
				action((T) item);
			}
		}

		public static void HandleCollectionChanged<T>(object owner, string key, IEnumerable list, NotifyCollectionChangedEventArgs e, Action<T> add, Action<T> remove) {
			RegisterList(owner,key,e.NewItems, add);
			ReleaseList(owner,key,e.OldItems, remove);
			if(e.Action==NotifyCollectionChangedAction.Reset && e.NewItems==null && e.OldItems==null) {
				ReleaseAll(owner, key, remove);
				RegisterList(owner,key,list, add);
			}
		}

//		public static void HandleCollectionChanged2<T>(object owner, string key, IEnumerable listVM, NotifyCollectionChangedEventArgs e, Action<T> add, Action<T, EventHandler> remove) {
//			throw new NotImplementedException();
//		}

//		public static void HandleCollectionChanged3<T>(object owner, string key, IEnumerable listVM, NotifyCollectionChangedEventArgs e, EventHandler handler, Action<T, EventHandler> add,Action<T, EventHandler> remove) {
//			RegisterList(owner,key,e.NewItems, add);
//			ReleaseList(owner,key,e.OldItems, remove);
//			if(e.Action==NotifyCollectionChangedAction.Reset && e.NewItems==null && e.OldItems==null) {
//				ReleaseAll(owner, key, remove);
//				RegisterList(owner,key,list, add);
//			}
//		}

		private class EventObject
		{
			private readonly WeakReference _owner;
			private readonly string _key;
			private readonly WeakReference _object;

			public EventObject(object owner, string key, object obj) {
				_owner  = new WeakReference(owner);
				_key    = key;
				_object = new WeakReference(obj);
			}

			public WeakReference OwnerRef {
				get { return _owner; }
			}

			public string Key {
				get { return _key; }
			}

			public WeakReference ObjectRef {
				get { return _object; }
			}
		}
	}

}
