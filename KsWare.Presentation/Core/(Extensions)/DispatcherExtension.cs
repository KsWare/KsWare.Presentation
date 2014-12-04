//OBSOLETE. Use DispatcherSlim
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Security.Permissions;
//using System.Text;
//using System.Threading;
//using System.Windows.Threading;
//
//
//[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope="namespace", Target="System.Windows.Threading")]
//namespace System.Windows.Threading {
//
//	/// <summary>
//	/// Provides additional functionality for <see cref="Dispatcher"/>
//	/// </summary>
//	public static class DispatcherExtension {
//
//		/// <summary> Gets a value indicating whether the caller must call an invoke method when making method calls to the UI because the caller is on a different thread than the one the UI was created on.
//		/// </summary>
//		public static bool IsInvokeRequired(this Dispatcher dispatcher) {
//			return dispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId;
//		}
//
//		/// <summary>
//		/// Executes a delegate on the application dispatcher.
//		/// </summary>
//		/// <param name="dispatcher">The dispatcher.</param>
//		/// <param name="method">The method to execute</param>
//		/// <param name="args">The method arguments</param>
//		/// <returns></returns>
//		public static object InvokeIfRequired(this Dispatcher dispatcher, Delegate method, params object[] args) {
//			if(IsInvokeRequired(dispatcher)) return dispatcher.Invoke(method, args);
//			return method.DynamicInvoke(args);
//		}
//
//		/// <summary> Processes all Dispatcher messages currently in the dispatcher queue.
//		/// </summary>
//		/// <param name="dispatcher">The dispatcher.</param>
//		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)] 
//		public static void DoEvents(this Dispatcher dispatcher) { 
//			DispatcherFrame frame = new DispatcherFrame(); 
//			dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame); 
//			Dispatcher.PushFrame(frame); 
//		} 
// 
//		private static object ExitFrame(object frame) { 
//			((DispatcherFrame)frame).Continue = false; 
//			return null; 
//		} 
//
//	}
//}
