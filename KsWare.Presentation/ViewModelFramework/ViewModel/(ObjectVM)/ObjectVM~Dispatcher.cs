using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core;

namespace KsWare.Presentation.ViewModelFramework {
	//
	// Implements "Dispatcher/Invoke" functionality for ObjectVM
	//  

	public partial class ObjectVM: INotifyPropertyChanged {

		/// <summary> Gets the <see cref="ApplicationDispatcher"/> this object is associated with.
		/// </summary> 
		/// <seealso cref="DispatcherObject.Dispatcher"/>
		public IDispatcher Dispatcher { get { return ApplicationDispatcher.Instance; } }

//		/// <summary> Determines whether the calling thread has access to this DispatcherObject.
//		/// </summary>
//		/// <returns>true if the calling thread has access to this object; otherwise, false.</returns>
//		/// <remarks>
//		/// <p>Only the thread the Dispatcher was created on may access the DispatcherObject.</p>
//		/// <p>Any thread can check to see whether it has access to this DispatcherObject.</p>
//		/// <p>The difference between CheckAccess and VerifyAccess is that CheckAccess returns a Boolean that specifies whether the calling thread has access to this DispatcherObject and VerifyAccess throws an exception if the calling thread does not have access to the this DispatcherObject.</p>
//		/// <p>Calling this method is identical to calling CheckAccess on the associated Dispatcher object.</p>
//		/// </remarks>
//		/// <seealso cref="DispatcherObject.CheckAccess"/>
//		public bool CheckAccess() { return DispatcherSlim.ApplicationDispatcher.IsInvokeRequired;}
//
//		/// <summary> Enforces that the calling thread has access to this DispatcherObject.
//		/// </summary>
//		/// <seealso cref="DispatcherObject.VerifyAccess"/>
//		public void VerifyAccess() { if(! DispatcherSlim.ApplicationDispatcher.IsInvokeRequired) throw new Exception(...);}

//		/// <summary> Gets a value indicating whether the caller must call an invoke method when making method calls to a control because the caller is on a different thread than the one the control was created on.
//		/// </summary>
//		public static bool IsInvokeRequired{ get { return ApplicationVM.IsInvokeRequired; } }

//		/// <summary> Executes a delegate on the application dispatcher.
//		/// </summary>
//		/// <param name="method">The method to execute</param>
//		/// <param name="args">The method arguments</param>
//		/// <returns></returns>
//		public static object InvokeIfRequired(Delegate method, params object[] args) { return ApplicationVM.InvokeIfRequired(method, args); }

//		/// <summary> Executes the specified delegate with the specified arguments synchronously on the application thread.
//		/// </summary>
//		/// <param name="method"></param>
//		/// <param name="args"></param>
//		/// <returns></returns>
//		public static object Invoke(Delegate method, params object[] args) {
//			//			if(Application.Current!=null) return Application.Current.Dispatcher.Invoke(method, args);
//			//			return method.DynamicInvoke(args);
//			return ApplicationVM.Current.Dispatcher.Invoke(method, args);
//		}

//		/// <summary> Executes the specified delegate asynchronously with the specified arguments on the application thread.
//		/// </summary>
//		/// <param name="method">The delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
//		/// <param name="args">An array of objects to pass as arguments to the given method. Can be null.</param>
//		/// <returns>An object, which is returned immediately after System.Windows.Threading.Dispatcher.BeginInvoke is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
//		public static DispatcherOperation BeginInvoke([NotNull] Delegate method, [CanBeNull] params object[] args) {
//			return ApplicationVM.Current.Dispatcher.BeginInvoke(method, args);
//		}

//		/// <summary> Short for: Application.Current.Dispatcher.BeginInvoke
//		/// </summary>
//		/// <param name="action"></param>
//		/// <returns></returns>
//		public static DispatcherOperation BeginInvoke([NotNull] Action action) {return ApplicationVM.Current.Dispatcher.BeginInvoke(action);}
		
//		/// <summary> Short for: Application.Current.Dispatcher.BeginInvoke 
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="action"></param>
//		/// <param name="arg"></param>
//		/// <returns></returns>
//		public static DispatcherOperation BeginInvoke<T>([NotNull] Action<T> action, T arg) {return ApplicationVM.Current.Dispatcher.BeginInvoke(action,new object[]{arg});}
		
//		/// <summary> Short for: Application.Current.Dispatcher.BeginInvoke 
//		/// </summary>
//		/// <typeparam name="T1"></typeparam>
//		/// <typeparam name="T2"></typeparam>
//		/// <param name="action"></param>
//		/// <param name="arg1"></param>
//		/// <param name="arg2"></param>
//		/// <returns></returns>
//		public static DispatcherOperation BeginInvoke<T1,T2>([NotNull] Action<T1,T2> action, T1 arg1, T2 arg2) {return ApplicationVM.Current.Dispatcher.BeginInvoke(action,new object[]{arg1,arg2});}
//		//		public static DispatcherOperation BeginInvokeEventHandler([NotNull] EventHandler handler, object sender, EventArgs e) {return ApplicationVM.Current.Dispatcher.BeginInvoke(handler,new object[]{sender,e});}
//		
//		/// <summary> Short for: Application.Current.Dispatcher.BeginInvoke 
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="handler"></param>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		/// <returns></returns>
//		public static DispatcherOperation BeginInvokeEventHandler<T>([NotNull] EventHandler<T> handler, object sender, T e) where T:EventArgs {return ApplicationVM.Current.Dispatcher.BeginInvoke(handler,new object[]{sender,e});}


//		/// <summary> BeginInvoke with delay
//		/// </summary>
//		/// <param name="action"></param>
//		/// <returns></returns>
//		public static IDisposable BeginInvoke(TimeSpan delay, [NotNull] Action action) { return new DelayedDispatcherTask(null, delay, action); }
//
//		public static object Invoke([NotNull] Action action) {return ApplicationVM.Current.Dispatcher.Invoke(action);}
//		public static object Invoke<T>([NotNull] Action<T> action, T arg) {return ApplicationVM.Current.Dispatcher.Invoke(action,new object[]{arg});}
//		public static object Invoke<T1,T2>([NotNull] Action<T1,T2> action, T1 arg1, T2 arg2) {return ApplicationVM.Current.Dispatcher.Invoke(action,new object[]{arg1,arg2});}
	}

}
