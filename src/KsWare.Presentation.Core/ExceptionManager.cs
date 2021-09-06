using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace KsWare.Presentation {

	/// <summary>  Provides a global manager for (unhandled) exceptions.
	/// </summary>
	public static class ExceptionManager {

		private static bool _catchUnhandledExceptionsDebug           = true;//false is default
		private static bool _catchUnhandledExceptions;
		private static List<Dispatcher>                               _dispatchers=new List<Dispatcher>() ; 

		private static List<RoutedUnhandledExceptionEventHandler> _managedHandlers=new List<RoutedUnhandledExceptionEventHandler>();
		private static object _handlerSyncRoot=new object();
		private static int _handlerEntryCount;

		/// <summary> Registers a dispatcher.
		/// </summary>
		/// <param name="dispatcher">The dispatcher.</param>
		/// <remarks>
		/// Call this method to catch <seealso cref="Dispatcher.UnhandledException">unhandled exceptions</seealso> from the specified <see cref="Dispatcher"/>.
		/// <seealso cref="Dispatcher.UnhandledException">Dispatcher.UnhandledException</seealso>
		/// </remarks>
		public static void RegisterDispatcher(Dispatcher dispatcher) {
			if (dispatcher == null) return;
			if (_dispatchers.Contains(dispatcher)) return;
			_dispatchers.Add(dispatcher);
			if (_catchUnhandledExceptions) {
				if (Debugger.IsAttached && _catchUnhandledExceptionsDebug == false) {
					/*don't catch exceptions*/
				}
				else dispatcher.UnhandledException += AtDispatcherOnUnhandledException;
			}
			dispatcher.ShutdownFinished += AtDispatcherOnShutdownFinished;
		}

		private static void AtDispatcherOnShutdownFinished(object sender, EventArgs eventArgs) {
			var dispatcher = (Dispatcher) sender;
			_dispatchers.Remove(dispatcher);
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
			foreach (var dispatcher in _dispatchers) dispatcher.UnhandledException+=AtDispatcherOnUnhandledException;
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
			get => _catchUnhandledExceptions;
			set {
				if (value) {
					if (Debugger.IsAttached && _catchUnhandledExceptionsDebug==false) { /*don't catch exceptions*/} 
					else InitExceptionHandlers();
				} else {
					foreach (var dispatcher in _dispatchers) 
						dispatcher         .UnhandledException      -= AtDispatcherOnUnhandledException;
					AppDomain.CurrentDomain.UnhandledException      -= AtAppDomainOnUnhandledException;
					TaskScheduler          .UnobservedTaskException -= AtTaskSchedulerOnUnobservedTaskException;
				}
				_catchUnhandledExceptions = value;
			}
		}

		/// <summary> Occurs when an unhandled exception is caught.
		/// </summary>
		/// <remarks>The last registered handler gets the event as first.</remarks>
		public static event RoutedUnhandledExceptionEventHandler UnhandledException {
			add => _managedHandlers.Insert(0, value);
			remove => _managedHandlers.Remove(value);
		}

		private static void OnUnhandledException(RoutedUnhandledExceptionEventArgs e) {
			Debug.WriteLine("ExceptionManager.OnUnhandledException "+e.SourceEventArgs.GetType().Name + " " + e.Exception.GetType().Name);
			lock (_handlerSyncRoot) {
				try {
					_handlerEntryCount++;
					if (_handlerEntryCount > 1) {
						//Environment.Exit(1);
					}
					foreach (var handler in _managedHandlers) {
						handler.Invoke(typeof (ExceptionManager), e);
						if (e.IsHandled) return;
					}
					//no one has handled this exception
					//Environment.Exit(1);
				} 
				//catch (Exception ex) { Environment.Exit(1);}
				finally { _handlerEntryCount--; }
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

		private bool _isHandled;

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
			get => _isHandled;
			set {
				if(_isHandled && value==false) throw new InvalidOperationException("Can not set Handled to false!");
				if(_isHandled==value) return;
				_isHandled = true;

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
