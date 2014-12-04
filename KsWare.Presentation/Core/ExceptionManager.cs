using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	/// <summary>  Provides a global manager for (unhandled) exceptions.
	/// </summary>
	public static class ExceptionManager {

		private static bool s_CatchUnhandledExceptionsDebug           = true;//false is default
		private static bool s_CatchUnhandledExceptions;
		private static List<Dispatcher>                               s_Dispatchers=new List<Dispatcher>() ; 

		private static List<RoutedUnhandledExceptionEventHandler> s_ManagedHandlers=new List<RoutedUnhandledExceptionEventHandler>();
		private static object s_HandlerSyncRoot=new object();
		private static int s_HandlerEntryCount;

		/// <summary> Registers a dispatcher.
		/// </summary>
		/// <param name="dispatcher">The dispatcher.</param>
		/// <remarks>
		/// Call this method to catch <seealso cref="Dispatcher.UnhandledException">unhandled exceptions</seealso> from the specified <see cref="Dispatcher"/>.
		/// <seealso cref="Dispatcher.UnhandledException">Dispatcher.UnhandledException</seealso>
		/// </remarks>
		public static void RegisterDispatcher(Dispatcher dispatcher) {
			if(dispatcher==null) return;
			if(s_Dispatchers.Contains(dispatcher)) return;
			s_Dispatchers.Add(dispatcher);
			if (s_CatchUnhandledExceptions) {
				if (Debugger.IsAttached && s_CatchUnhandledExceptionsDebug==false) { /*don't catch exceptions*/} 
				else dispatcher.UnhandledException+=AtDispatcherOnUnhandledException;
			}
			dispatcher.ShutdownFinished+=AtDispatcherOnShutdownFinished;
		}

		private static void AtDispatcherOnShutdownFinished(object sender, EventArgs eventArgs) {
			var dispatcher = (Dispatcher) sender;
			s_Dispatchers.Remove(dispatcher);
			dispatcher.UnhandledException -= AtDispatcherOnUnhandledException;
		}

		private static void AtDispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
			// e.Exception;
			// e.Dispatcher;
			// e.Handled;

			OnUnhandledException(new RoutedUnhandledExceptionEventArgs(e));
			e.Handled = true;
		}

		/// <summary> Initializes the exception handlers.
		/// </summary>
		private static void InitExceptionHandlers() {
			foreach (var dispatcher in s_Dispatchers) dispatcher.UnhandledException+=AtDispatcherOnUnhandledException;
			AppDomain    .CurrentDomain.UnhandledException           +=AtAppDomainOnUnhandledException;
			TaskScheduler              .UnobservedTaskException      +=AtTaskSchedulerOnUnobservedTaskException;
		}

		private static void AtTaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
			// e.Exception;
			// e.Observed;
			// e.SetObserved();	
			OnUnhandledException(new RoutedUnhandledExceptionEventArgs(e));
		}

		private static void AtAppDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			// e.IsTerminating
			// e.ExceptionObject

			var exception = e.ExceptionObject as Exception;
			if (exception == null) exception = new AppDomainUnloadedException("An unspecified exception has occured!");
			OnUnhandledException(new RoutedUnhandledExceptionEventArgs(e));
		}

		/// <summary> Gets or sets a value indicating whether unhandled exceptions shall by catched or not.
		/// </summary>
		/// <value><c>true</c> if unhandled exceptions are catched; otherwise, <c>false</c>.</value>
		public static bool CatchUnhandledExceptions {
			get { return s_CatchUnhandledExceptions; }
			set {
				if (value) {
					if (Debugger.IsAttached && s_CatchUnhandledExceptionsDebug==false) { /*don't catch exceptions*/} 
					else InitExceptionHandlers();
				} else {
					foreach (var dispatcher in s_Dispatchers) 
						dispatcher         .UnhandledException      -= AtDispatcherOnUnhandledException;
					AppDomain.CurrentDomain.UnhandledException      -= AtAppDomainOnUnhandledException;
					TaskScheduler          .UnobservedTaskException -= AtTaskSchedulerOnUnobservedTaskException;
				}
				s_CatchUnhandledExceptions = value;
			}
		}

		/// <summary> Occurs when an unhandled exception is caught.
		/// </summary>
		/// <remarks>The last registered handler gets the event as first.</remarks>
		public static event RoutedUnhandledExceptionEventHandler UnhandledException {
			add    { s_ManagedHandlers.Insert(0,value); } 
			remove { s_ManagedHandlers.Remove(value); }
		}

		private static void OnUnhandledException(RoutedUnhandledExceptionEventArgs e) {
			Debug.WriteLine("ExceptionManager.OnUnhandledException "+e.SourceEventArgs.GetType().Name + " " + e.Exception.GetType().Name);
			lock (s_HandlerSyncRoot) {
				try {
					s_HandlerEntryCount++;
					if (s_HandlerEntryCount > 1) {
						//Environment.Exit(1);
					}
					foreach (var handler in s_ManagedHandlers) {
						handler.Invoke(typeof (ExceptionManager), e);
						if (e.IsHandled) return;
					}
					//no one has handled this exception
					//Environment.Exit(1);
				} 
				//catch (Exception ex) { Environment.Exit(1);}
				finally { s_HandlerEntryCount--; }
			}
		}
		
		private static void OnUnhandledExceptionFallback(RoutedUnhandledExceptionEventArgs e) {
			var assembly = Assembly.GetEntryAssembly()??Assembly.GetExecutingAssembly();
			var location = assembly.Location;
			var exe = Path.GetFileNameWithoutExtension(location);
			var file = Path.Combine(Path.GetTempPath(), string.Format("{0} {1:yyyy-MM-dd}.errordump", exe, DateTime.Now));
			var isSaved = false;

			#region save in daily file
			try {
				using (var w=new StreamWriter(file,true,Encoding.UTF8)) {
					w.WriteLine("=== {0:yyyy-MM-dd HH:mm:ss} Process:{1} Location:{2}===",DateTime.Now,Process.GetCurrentProcess().Id,location);
					w.WriteLine("IsTerminating {0}",e.IsTerminating);
					w.WriteLine(e.Exception.ToString());
				}
				Trace.WriteLine("Error Log: "+file);
				isSaved = true;
			}
			catch (Exception) { }
			#endregion

			#region fallback, save in individual file
			if (!isSaved) {
				file = Path.Combine(Path.GetTempPath(), string.Format("{0} {1:yyyy-MM-dd HHmmss}.errordump", exe, DateTime.Now));
				try {
					using (var w=new StreamWriter(file,true,Encoding.UTF8)) {
						w.WriteLine("=== {0:yyyy-MM-dd HH:mm:ss} ===",DateTime.Now);
						w.WriteLine("IsTerminating {0}",e.IsTerminating);
						w.WriteLine(e.Exception.ToString());
					}
					isSaved = true;
					Trace.WriteLine("Error Log: "+file);
				}
				catch (Exception) {}
			}
			#endregion

			// Create user friendly message
			var efea = new ExceptionFeedbackEventArgs(
				e.Exception,"An error has occured. " + (
					e.IsTerminating
						?"Application can not continue and will exit."+"\n"+"Try to restart the application. "
						:"You can try to continue."+"\n"+"It is suggested to save your work and restart the application."
				) +
				
				"\n"+"If the error occurs again, please contact support." +
				"\n"+"We apologize for the inconvenience.",
				"Details are logged into temp directory:"+
				"\n " +Path.GetDirectoryName(file) +
				"\n " + Path.GetFileName(file),
				"Sorry, an error has occurred :-( "
			);
//			OnUserFeedbackRequested(this,efea);
		}

		/// <summary> Handles an exception as an unhandled exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <remarks>With this method you can try/catch exceptions to get defined exit points but w/o handling this exception directly. 
		/// The exception is handled like all other unhadled exceptions. </remarks>
		public static void ExceptionCaught(Exception exception) {
			OnUnhandledException(new RoutedUnhandledExceptionEventArgs(exception));
		}

	}

	public enum ExceptionSource {

		None,

		/// <summary> AppDomain.CurrentDomain.UnhandledException </summary>
		AppDomain,

		/// <summary> Application.DispatcherUnhandledException
		/// </summary>
		Dispatcher,

		/// <summary> TaskScheduler.UnobservedTaskException </summary>
		TaskScheduler
	}

	public delegate void RoutedUnhandledExceptionEventHandler(object sender, RoutedUnhandledExceptionEventArgs e);

	[Serializable]
	public class RoutedUnhandledExceptionEventArgs : EventArgs {

		private bool m_IsHandled;

		public RoutedUnhandledExceptionEventArgs(DispatcherUnhandledExceptionEventArgs ea) {
			SourceEventArgs = ea;
			Exception     = ea.Exception;
			IsTerminating = false;
			//IsHandled     = ea.Handled;
		}

		public RoutedUnhandledExceptionEventArgs(System.UnhandledExceptionEventArgs ea) {
			SourceEventArgs = ea;
			Exception     = (ea.ExceptionObject as Exception)??(
				ea.IsTerminating
				? (Exception)new AppDomainUnloadedException("An unspecified exception has occured!")
				: (Exception)new ApplicationException("An unspecified exception has occured!")
				);
			IsTerminating = ea.IsTerminating;
			//IsHandled     = false;
		}

		public RoutedUnhandledExceptionEventArgs(UnobservedTaskExceptionEventArgs ea) {
			SourceEventArgs = ea;
			Exception     = ea.Exception;
			IsTerminating = false;
			//IsHandled     = ea.Observed;
		}


		public RoutedUnhandledExceptionEventArgs(Exception exception) {
			Exception     = exception;
			IsTerminating = false;
			//IsHandled     = false;
		}

		public EventArgs SourceEventArgs { get; private set; }

		/// <summary> Gets the exception.
		/// </summary>
		/// <value>The exception.</value>
		/// <seealso cref="DispatcherUnhandledExceptionEventArgs"/>
		/// <seealso cref="UnhandledExceptionEventArgs"/>
		/// <seealso cref="UnobservedTaskExceptionEventArgs"/>
		public Exception Exception { get; private set; }

		/// <summary> Indicates whether the common language runtime is terminating.
		/// </summary>
		/// <value><c>true</c> if the runtime is terminating; otherwise, <c>false</c>.</value>
		/// <seealso cref="UnhandledExceptionEventArgs"/>
		public bool IsTerminating { get; private set; }

		public bool IsHandled {
			get { return m_IsHandled; }
			set {
				if(m_IsHandled && value==false) throw new InvalidOperationException("Can not set Handled to false!");
				if(m_IsHandled==value) return;
				m_IsHandled = true;

				//REVISE
				if (SourceEventArgs is DispatcherUnhandledExceptionEventArgs) {
					((DispatcherUnhandledExceptionEventArgs) SourceEventArgs).Handled = true;
				} else if (SourceEventArgs is UnhandledExceptionEventArgs) {
					
				}else if (SourceEventArgs is UnobservedTaskExceptionEventArgs) {
					((UnobservedTaskExceptionEventArgs) SourceEventArgs).SetObserved();
				}
			}
		}

	}

}
