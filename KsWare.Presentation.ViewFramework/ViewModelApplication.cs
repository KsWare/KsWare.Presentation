using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewFramework {

/*
<vf:ViewModelApplication
	x:TypeArguments="local:AppVM" 
	x:Class="MyCompany.MyApplication.App"
	xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	xmlns:vf="clr-namespace:KsWare.Presentation.ViewFramework;assembly=KsWare.Presentation"
	xmlns:local="clr-namespace:MyCompany.MyApplication"
>
	<Application.Resources>         
	</Application.Resources>
</vf:ViewModelApplication>
*/

	/// <summary> Application base class for use with view model (<see cref="ApplicationVM"/>)
	/// </summary>
	/// <remarks>
	/// <para>Exception handling: <br/>
	/// By default, all exceptions are catched during startup.
	/// Except if an debugger is attached.<br/>
	/// Set <see cref="CatchUnhandledExceptions"/> to false (e.g. App.xaml) to disable global exception handling (after startup).
	/// See also <see cref="ExceptionManager"/>.
	/// </para>
	/// </remarks>
	public class ViewModelApplication : Application {

		protected readonly IObjectVM m_RootVM ;

		public bool CatchUnhandledExceptions {
			get { return ExceptionManager.CatchUnhandledExceptions; }
			set { ExceptionManager.CatchUnhandledExceptions = value; }
		}

		public ViewModelApplication():this(typeof(ApplicationVM)) {}

		protected ViewModelApplication(Type rootVMType) {//
			ExceptionManager.RegisterDispatcher(Dispatcher.CurrentDispatcher);
			ExceptionManager.CatchUnhandledExceptions = true;
			ExceptionManager.UnhandledException+=AtExceptionManagerOnUnhandledException;

			if (typeof (ApplicationVM).IsAssignableFrom(rootVMType)) {
				m_RootVM=(IObjectVM) Activator.CreateInstance(rootVMType);
			} else {
				var initializeMethod = rootVMType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
				if(initializeMethod!=null) initializeMethod.Invoke(null, null);
				var defaultProperty = rootVMType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
				if (defaultProperty != null) m_RootVM = (IObjectVM) defaultProperty.GetValue(null, null);
			}

			if (m_RootVM != null) {
				m_RootVM.UserFeedbackRequested+=OnUserFeedbackRequested;
			}
		}

		private void AtExceptionManagerOnUnhandledException(object sender, RoutedUnhandledExceptionEventArgs e) {
			var assembly = Assembly.GetEntryAssembly()??Assembly.GetExecutingAssembly();
			var location = assembly.Location;
			var exeName = Path.GetFileName(location);
			var exeTitle = Path.GetFileNameWithoutExtension(location);
//			var version = assembly.GetCustomAttributes(typeof (AssemblyFileVersionAttribute), false).Cast<AssemblyFileVersionAttribute>().Select(x => x.Version).FirstOrDefault();
			var version = assembly.GetName(false).Version.ToString(); // AssemblyVersion
			var file = Path.Combine(Path.GetTempPath(), $"{exeTitle} {version} {DateTime.Now:yyyy-MM-dd}.errordump");
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
				file = Path.Combine(Path.GetTempPath(), $"{exeName} {version} {DateTime.Now:yyyy-MM-dd HHmmss}.errordump");
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
				exception:
					e.Exception,
				message:
					"Sorry, an error has occurred :-("+ "\n"+
					"\n" + 
					(e.IsTerminating
							? "Application can not continue and will exit."+"\n"+
							  "Try to restart the application."+"\n"
							: "You can try to continue."+"\n"+
							  "It is suggested to save your work and restart the application."+"\n"
					) +
					"\n"+
					"If the error occurs again, please contact support." + "\n"+
					"We apologize for the inconvenience.",
				detailMessage:
					"Details are logged into temp directory:"+
					"\n " +Path.GetDirectoryName(file) +
					"\n " + Path.GetFileName(file),
				caption: 
					exeTitle
			);
			e.IsHandled = true;
			OnUserFeedbackRequested(this,efea);			
		}

		public virtual ApplicationVM ViewModel { get { return (ApplicationVM) m_RootVM; } }

		protected virtual void OnUserFeedbackRequested(object sender, UserFeedbackEventArgs e) {
			try {
				if (e is OpenFileFeedbackEventArgs) {
					var dlg = ((OpenFileFeedbackEventArgs) e).CreateDialog();
					((OpenFileFeedbackEventArgs) e).Update(dlg.ShowDialog(), dlg);
				} else if (e is SaveFileFeedbackEventArgs) {
					var dlg = ((SaveFileFeedbackEventArgs) e).CreateDialog();
					((SaveFileFeedbackEventArgs) e).Update(dlg.ShowDialog(), dlg);
				} else if (e is ExceptionFeedbackEventArgs) {
					var ea = (ExceptionFeedbackEventArgs) e;
					ea.DialogResult = MessageBox.Show(
						ea.MessageBoxText + "\n\n" + ea.DetailMessage, 
						ea.Caption, 
						ea.Button, ea.Icon, ea.DefaultResult, ea.Options);
					ea.Handled = true;
				} else if (e is DetailMessageFeedbackEventArgs) {
					var ea = (DetailMessageFeedbackEventArgs) e;
					ea.DialogResult = MessageBox.Show(
						ea.MessageBoxText + "\n\n" + ea.DetailMessage, 
						ea.Caption,
						ea.Button, ea.Icon, ea.DefaultResult, ea.Options);
					ea.Handled = true;
				} else if (e is MessageFeedbackEventArgs) {
					var ea = (MessageFeedbackEventArgs) e;
					ea.DialogResult = MessageBox.Show(
						ea.MessageBoxText, 
						ea.Caption, 
						ea.Button, ea.Icon, ea.DefaultResult, ea.Options);
					ea.Handled = true;
				}
			} catch (Exception ex) { 
				//Uupps, here get somthing wrong..
				Debug.WriteLine("{0}.OnUserFeedbackRequested: {1}",GetType().Name,ex.GetType().Name);
			}

		}
	}

	public class ViewModelApplication<TRootVM> : ViewModelApplication where TRootVM: class,IObjectVM {

		public ViewModelApplication():base(typeof(TRootVM)) {}

		[Obsolete("Use ViewModel property!")]
		public TRootVM RootVM { get { return (TRootVM)m_RootVM; }}

		public new TRootVM ViewModel { get { return (TRootVM) m_RootVM; } }

	}

}