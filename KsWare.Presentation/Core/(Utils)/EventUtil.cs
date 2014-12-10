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
		public static void Raise(Delegate @delegate, object sender, EventArgs e, string uniqueId) { 
			if(@delegate==null){return; }

			#region simple invoke (conditional)
#if(false) // simple invoke the delegate w/o any other stuff (DynamicInvoke is slow!)
			try{
				@delegate.DynamicInvoke(new[] {sender, e});
			} catch (Exception ex) {
				if(ObjectVM.IsInDesignMode) return; // ignore designtime error
				throw; // possible breakpoint for debug
			}
			return;
#endif
			#endregion

			var exceptions=new List<TargetInvocationException>();
			var invocationList = @delegate.GetInvocationList();

			#region Trace (conditional)
#if(false)			
			Debug.WriteLine("=>Raise event: " + "("+invocationList.Length+" target"+(invocationList.Length!=1?"s":"")+")");
			foreach (var d in invocationList) {
				var targetType=d.Target.GetType().FullName;
				var method=d.Method.ToString();
				Debug.WriteLine("=>\t"+"Target: "+targetType+ " " + method);
			}
			Debug.WriteLine("=>\t"+"Raising method: "+sender.GetType().FullName+"."+new StackFrame(1).GetMethod());
			
			// MemberPath (ObjectVM/ObjectBM)
			var memberPathProperty=sender.GetType().GetProperty("MemberPath");
			if(memberPathProperty!=null){
				var memberPath=memberPathProperty.GetValue(sender,null);
				Debug.WriteLine("=>\t"+"MemberPath: "+memberPath);
			}
#endif
			#endregion

			var isInvokeRequired = ApplicationDispatcher.IsInvokeRequired;
			foreach (var d in invocationList) {
				#region DEBUG (conditional)
#if(false)
				Debug.WriteLine(string.Format("=>Raise event: #{0} {1} {2}",
					++InvocationCount, 
					DebugUtil.FormatTypeName(@delegate),
					"Target: "+ DebugUtil.FormatTypeName(d.Target)+"."+d.Method.Name
				));
#endif
				#endregion
				// EXPERIMENTAL ==> 
				// workaround for TargetInvocationException --> InvalidOperationException: 
				//   "The calling thread cannot access this object because a different thread owns it."
				// e.g. System.Windows.Input.CanExecuteChangedEventManager+HandlerSink, PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
				if (isInvokeRequired) {
					if      (d.Target.GetType().FullName == "System.Windows.Input.CanExecuteChangedEventManager+HandlerSink") InvokeAppDispatcher(d, sender, e, exceptions);
					else if (d.Target.GetType().FullName == "System.Windows.Data.ListCollectionView"                        ) InvokeAppDispatcher(d, sender, e, exceptions);
			/*??*/	else if (d.Target.GetType().Assembly.GetName().Name=="PresentationCore"                                 ) InvokeAppDispatcher(d, sender, e, exceptions);
					else InvokeDynamic(d, sender, e, exceptions);					
				}
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
				// A) 
//				d.DynamicInvoke(sender, e);

				// B) 
//				d.Method.Invoke(d.Target, new [] {sender, e});

				// C)
				
				var d4 = d as System.EventHandler<KsWare.Presentation.Core.Providers.DataChangedEventArgs>;
				if (d4 != null) {
					d4(sender, (KsWare.Presentation.Core.Providers.DataChangedEventArgs) e);
					//Console.WriteLine("DataChangedEventArgs");
					return;
				}

				var d5 = d as System.EventHandler<ValueChangedEventArgs>;
				if (d5 != null) {
					d5(sender, (ValueChangedEventArgs) e);
					//Console.WriteLine("ValueChangedEventArgs");
					return;
				}

				var d3 = d as System.ComponentModel.PropertyChangedEventHandler;
				if (d3 != null) {
					d3(sender, (System.ComponentModel.PropertyChangedEventArgs) e);
					//Console.WriteLine("PropertyChangedEventArgs");
					return;
				}


				// Resharper: Expression is allways null
				var d2 = d as System.EventHandler; 
				if (d2 != null) {
					d2(sender, e);
					//Console.WriteLine("EventHandler");
					return;
				}

				// Resharper: Expression is allways null
				var d8 = d as System.EventHandler<ValueChangedEventArgs<KsWare.Presentation.Core.Providers.IDataProvider>>;
				if (d8 != null) {
					d8(sender, (ValueChangedEventArgs<KsWare.Presentation.Core.Providers.IDataProvider>) e);
					//Console.WriteLine("ValueChangedEventArgs<IDataProvider>");
					return;
				}

				// Resharper: Expression is allways null
//				var d9 = d as System.EventHandler<NotifyCollectionChangedEventArgs>;
				var d9 = d as NotifyCollectionChangedEventHandler;
				if (d9 != null) {
					d9(sender, (NotifyCollectionChangedEventArgs) e);
					//Console.WriteLine("NotifyCollectionChangedEventArgs");
					return;
				}

				//Console.WriteLine(e.ToString());

				d.Method.Invoke(d.Target, new[] {sender, e}); //fallback

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
				var typeFullName = d.Method.DeclaringType != null ? d.Method.DeclaringType.FullName : "{unknown type}";
				var methodName = d.Method.ToString();
				Debug.WriteLine("ERROR: Unhandled exception in " + methodName + " in " + typeFullName + 
					"\n\t"+"ErrorID:"+"{7BD9F322-749C-4BB5-AE4C-67E38BD5CB3D}" +
					"\r\n" +
					ex.StackTrace[0]
				);
			}
			catch (Exception ex) {
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
		/// <remarks>Internally used by <see cref="ObjectVM.Dispose">ObjectVM.Dispose</see> and <see cref="ObjectBM.Dispose">ObjectBM.Dispose</see></remarks>
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
					"\n\t"+"Exception:     " + ex.GetType().FullName +
					"\n\t"+"Message:       " + ex.Message+
					"\n\t"+"Target Type:   " + @delegate.Target.GetType().FullName+
					"\n\t"+"Target Method: " + @delegate.Method.ToString()
					,sender.GetType().Name);
					//### Break
					if (Debugger.IsAttached)
						Debugger.Break();
				}
			}
		}

		// TODO Register/Release maybe obsolete, internaly used only 3 times
		
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

		private class EventObject {
			private readonly WeakReference m_WeakOwner;
			private readonly string m_Key;
			private readonly WeakReference m_WeakObject;

			public EventObject(object owner, string key, object obj) {
				m_WeakOwner  = new WeakReference(owner);
				m_Key    = key;
				m_WeakObject = new WeakReference(obj);
			}

			public WeakReference OwnerRef {
				get { return m_WeakOwner; }
			}

			public string Key {
				get { return m_Key; }
			}

			public WeakReference ObjectRef {
				get { return m_WeakObject; }
			}
		}
	}

}
