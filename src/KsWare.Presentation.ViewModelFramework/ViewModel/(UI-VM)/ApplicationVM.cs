using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Threading;
using System.Windows;

namespace KsWare.Presentation.ViewModelFramework {

	public class ApplicationVM : ObjectVM {

		//TODO support for multiple AppDomains

		#region Static

		private static ApplicationVM s_Current;
		private static bool s_IsShuttingDown;
		private static object s_GlobalLock;

		static ApplicationVM() { ApplicationInit(); }

		[SecurityCritical,SecuritySafeCritical]
		private static void ApplicationInit() {
			s_GlobalLock = new object();
		}

		/// <summary> Gets the <see cref="ApplicationVM"/> object for the current AppDomain.
		/// </summary>
		/// <value>The current.</value>
		/// <exception cref="System.NullReferenceException">ApplicationVM.Current is null!</exception>
		/// <remarks>
		/// <see cref="ApplicationVM"/> is a per-AppDomain singleton type that implements the static Current property to provide shared access to the <see cref="ApplicationVM"/> instance for the current AppDomain. This design guarantees that state managed by <see cref="ApplicationVM"/>, including shared resources and state, is available from a single, shared location.
		/// <p>This property is thread safe and is available from any thread.</p>
		/// </remarks>
		public static ApplicationVM Current {
			get {
				if (s_Current == null) {
					if(!IsInDesignMode) throw new NullReferenceException("ApplicationVM.Current is null!");
					else { throw new NullReferenceException("ApplicationVM.Current is null!" + "\r\n" + new StackTrace(true).ToString()); }
				}
				return s_Current;
			} 
			private set => s_Current = value;
		}

		internal static bool IsShuttingDown {
			[SecuritySafeCritical, SecurityCritical]
			get {
				if (s_IsShuttingDown) return s_IsShuttingDown;
//				if (BrowserInteropHelper.IsBrowserHosted) {
//					Application current = Application.Current;
//					if (current != null && current.CheckAccess()) {
//						IBrowserCallbackServices callbackServices = current.BrowserCallbackServices;
//						if (callbackServices != null) return callbackServices.IsShuttingDown();
//						else return false;
//					}
//				}
				return false;
			}
			set { lock (s_GlobalLock) s_IsShuttingDown = value; }
		}

		public static void TestCleanup() {
			if(s_Current!=null) s_Current.Dispose();
			s_Current = null;
		}

		/// <summary> Gets a value indicating whether the caller must call an invoke method when making method calls to a control because the caller is on a different thread than the one the control was created on.
		/// </summary>
		public static bool IsInvokeRequired{
			get {
				if(s_Current==null) return false;
				return ApplicationDispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId;
			}
		}

		/// <summary> Executes a delegate on the application dispatcher.
		/// </summary>
		/// <param name="method">The method to execute</param>
		/// <param name="args">The method arguments</param>
		/// <returns></returns>
		public static new object InvokeIfRequired(Delegate method, params object[] args) {
			if(s_Current==null) return method.DynamicInvoke(args);
			return ApplicationDispatcher.InvokeIfRequired(method, args);
		}

		#endregion

		private readonly Application _application;
		private Type _startupUri;
		private bool _appIsShutdown;


		/// <summary> Initializes a new instance of the <see cref="ApplicationVM"/> class.
		/// </summary>
		public ApplicationVM() : this(System.Windows.Application.Current) { }

		/// <summary> Initializes a new instance of the <see cref="ApplicationVM"/> class.
		/// </summary>
		/// <param name="application">The <see cref="System.Windows.Application"/>.</param>
		/// <exception cref="System.InvalidOperationException">ApplicationVM already initialized!</exception>
		public ApplicationVM(Application application) {
			lock (s_GlobalLock) {
				//if (s_AppCreatedInThisAppDomain) throw new InvalidOperationException(SR.Get("MultiSingleton"));
				if(s_Current!=null) throw new InvalidOperationException("ApplicationVM allready initialized!");
				s_Current = this;
				IsShuttingDown = false;
				//s_AppCreatedInThisAppDomain = true;
			}
			//Debug.WriteLine(string.Format("ApplicationVM: {1}, ManagedThreadId={0}",Thread.CurrentThread.ManagedThreadId,Thread.CurrentThread.GetApartmentState()));


			if(application==null) Debug.WriteLine("WARNING: Initialize new ApplicationVM without an Application instance!"+"\n\t"+"ID:{4709307E-02BA-4478-9324-40772FCD6C1B}");
			_application = application;

			WindowsInternal       = RegisterChild(() => WindowsInternal, new WindowVMCollection());
			NonAppWindowsInternal = RegisterChild(() => NonAppWindowsInternal, new WindowVMCollection());
			RegisterChildren(()=>this);

			if (_application != null) { //for UnitTest the application can be null
				_application.Startup     += AtStartup;                             //=> Occurs when the Run method of the Application object is called.
				_application.Activated   += (s, e) => OnApplicationActivated();
				_application.Deactivated += (s, e) => OnApplicationDeactivated();
				_application.Exit        += AtExit;                                // => Occurs just before an application shuts down, and cannot be canceled.
				//m_Application.SessionEnding	=> Occurs when the user ends the Windows session by logging off or shutting down the operating system.
				//Navigation: FragmentNavigation, LoadCompleted, Navigated, Navigating, NavigationProgress, NavigationStopped, NavigationFailed, SetCookie, GetCookie.
			}
			//this.Dispatcher.UnhandledException+=
			//this.Dispatcher.UnhandledExceptionFilter+=

			Fields[nameof(CurrentCulture)].ValueChanged+= delegate {
				Dispatcher.Thread.CurrentCulture = CurrentCulture;
				Dispatcher.Thread.CurrentUICulture = CurrentCulture;
			};	

			#region Unit test only
			if (_application == null) {
				//Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => {
					OnStartup(null);
					DoStartup(null, explicitStartup: true);
				//}),null);
			}
			#endregion
		}

		/// <summary> Gets or sets a UI that is automatically shown when an application starts.
		/// </summary>
		public Type StartupUri {
			get => _startupUri;
			set {
//				if(!typeof(IObjectVM).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException("value","Type of IObjectVM expected!");
				if(!typeof(WindowVM).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"Type of WindowVM expected!");
				//TODO support standard startup UI resources
				/*  Type				Window				Application type
					--------------------------------------------------------------------------------------
				OK	Window				Window				Standalone only					WindowVM
				?	NavigationWindow	NavigationWindow	Standalone only
				?	Page				NavigationWindow	Standalone/browser-hosted
				?	UserControl			NavigationWindow	Standalone/browser-hosted
				?	FlowDocument		NavigationWindow	Standalone/browser-hosted
				?	PageFunction<T>		NavigationWindow	Standalone/browser-hosted
				*/
				_startupUri = value;
			}
		}

		internal WindowVMCollection WindowsInternal { get; private set; }
		internal WindowVMCollection NonAppWindowsInternal { get; private set; }

		// Type: System.Windows.Application
		// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		//		public WindowCollection Windows {
		//			get {
		//				this.VerifyAccess();
		//				return this.WindowsInternal.Clone();
		//			}
		//		}

		/// <summary> [EXPERIMENTAL] Gets the instantiated windows in an application.
		/// </summary>
		/// <value>A <see cref="WindowVMCollection"/> that contains references to all window objects in the current AppDomain.</value>
		public WindowVMCollection Windows => WindowsInternal;

		// Type: System.Windows.Application
		// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		//		public Window MainWindow {
		//			get {
		//				this.VerifyAccess();
		//				return this._mainWindow;
		//			}
		//			set {
		//				this.VerifyAccess();
		//				if (this._mainWindow is RootBrowserWindow || this.BrowserCallbackServices != null && this._mainWindow == null && !(value is RootBrowserWindow)) 
		//					throw new InvalidOperationException(SR.Get("CannotChangeMainWindowInBrowser"));
		//				if (value == this._mainWindow) return;
		//				this._mainWindow = value;
		//			}
		//		}

		/// <summary> [EXPERIMENTAL] Gets or sets the main window of the application. 
		/// </summary>
		/// <value>A <see cref="WindowVM"/> that is designated as the main application window.</value>
		[Hierarchy(HierarchyType.Reference)]
		public WindowVM MainWindow { get => Fields.GetValue<WindowVM>(); set => Fields.SetValue(value); }
		
		public CultureInfo CurrentCulture { get => Fields.GetValue<CultureInfo>(); set => Fields.SetValue(value); }
		
		/// <summary> Gets or sets a value indicating whether the application is in foreground.
		/// </summary>
		/// <value><c>true</c> if the application is in foreground; otherwise, <c>false</c>.</value>
		public bool IsActive { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		// /// <summary> Gets the <see cref="Dispatcher"/> this object is associated with. 
		// /// </summary>
		// /// <value>The dispatcher.</value>
		// public IDispatcher Dispatcher { get; private set; }

		protected virtual void OnStartup(StartupEventArgs e) {}
		protected virtual void OnApplicationActivated() { IsActive = true; }
		protected virtual void OnApplicationDeactivated() { IsActive = false; }
		protected virtual void OnExit(ExitEventArgs e) {  }

		private void AtStartup(object sender, StartupEventArgs e) { DoStartup(e, explicitStartup: false); OnStartup(e);}

		private void AtExit(object sender, ExitEventArgs e) { DoShutdown(e,explicitShutdown:false); /*-->OnExit(e);*/ }

// Type: System.Windows.Application
// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
//		internal void DoStartup() {
//			if (!(this.StartupUri != (Uri) null)) return;
//			if (!this.StartupUri.IsAbsoluteUri) this.StartupUri = new Uri(this.ApplicationMarkupBaseUri, this.StartupUri);
//			if (BaseUriHelper.IsPackApplicationUri(this.StartupUri)) {
//				NavigatingCancelEventArgs e = new NavigatingCancelEventArgs(BindUriHelper.GetUriRelativeToPackAppBase(this.StartupUri), (object) null, (CustomContentState) null, (object) null, NavigationMode.New, (WebRequest) null, (object) null, true);
//				this.FireNavigating(e, true);
//				if (e.Cancel) return;
//				this.ConfigAppWindowAndRootElement(Application.LoadComponent(this.StartupUri, false), this.StartupUri);
//			}
//			else {
//				this.NavService = new NavigationService((INavigator) null);
//				this.NavService.AllowWindowNavigation = true;
//				this.NavService.PreBPReady += new BPReadyEventHandler(this.OnPreBPReady);
//				this.NavService.Navigate(this.StartupUri);
//			}
//		}

		// original: Application → OnStartup;DoStartup
		// this: Application.Startup ↯ AtStartup → DoStartup

		private void DoStartup(StartupEventArgs e, bool explicitStartup) {
			if (StartupUri == null) return;
			var vm = (WindowVM) Activator.CreateInstance(StartupUri);
			vm.Show();
		}

		/// <summary> Shuts down an application.
		/// </summary>
		public void Shutdown(int exitCode=0) {
			if (IsShuttingDown) return;
			if (_application != null) {
				IsShuttingDown = true;
				_application.Shutdown(exitCode);// → CriticalShutdown ↷ ShutdownCallback → ShutdownImpl → DoShutdown → OnExit
				// → AtExit → DoShutdown → OnExit
			} else {
				IsShuttingDown = true;
				DoShutdown(null,explicitShutdown:true);
			}
		}

//		internal void CriticalShutdown(int exitCode) {
//			this.VerifyAccess();
//			if (Application.IsShuttingDown) return;
//			this.SetExitCode(exitCode);
//			Application.IsShuttingDown = true;
//			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) new DispatcherOperationCallback(this.ShutdownCallback), (object) null);
//		}

		// Type: System.Windows.Application
		// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		internal virtual void DoShutdown(ExitEventArgs e, bool explicitShutdown) {
			while (WindowsInternal.Count > 0) {
				if (!WindowsInternal[0].IsDisposed) WindowsInternal[0].Dispose();
				else WindowsInternal.RemoveAt(0);
			}
			WindowsInternal = null;
//			var e = new ExitEventArgs(this._exitCode);
			try { OnExit(e); }
			finally {
//				SetExitCode(e._exitCode);
//				lock (Application._globalLock) Application._appInstance = (Application) null;
				MainWindow = null;
//				this._htProps = (HybridDictionary) null;
				NonAppWindowsInternal = null;
//				if (this._parkingHwnd != null) this._parkingHwnd.Dispose();
//				if (this._events != null) this._events.Dispose();
//				PreloadedPackages.Clear();
//				AppSecurityManager.ClearSecurityManager();
				_appIsShutdown = true;
			}
		}

		/// <summary> Gets or sets a value indicating whether user feedbacks are forwarded to main window.
		/// </summary>
		/// <value><c>true</c> if user feedbacks are forwarded to main window; otherwise, <c>false</c>.</value>
		/// <seealso cref="WindowVM.HandleApplicationUserFeedbackRequests"/>
		public bool ForwardUserFeedbackToMainWindow { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary> Forwards user feedback to main window
		/// </summary>
		/// <param name="args"></param>
		protected override void RequestUserFeedbackCore(UserFeedbackEventArgs args) {
			if (ForwardUserFeedbackToMainWindow && MainWindow != null) {
				// reroute user feedback requests to main window
				MainWindow.HandleApplicationUserFeedbackRequests(args);
				if(args.Handled || args.AsyncHandled) return;
			}
			base.RequestUserFeedbackCore(args);
		}
	}

	public class WindowVMCollection : ListVM<WindowVM> {
		
		//called by WindowVM.Show
		internal void Show(WindowVM windowVM) {
			var tWin = FindView(windowVM.GetType());
			var window = (Window)Activator.CreateInstance(tWin);
			window.DataContext = windowVM;
			windowVM.UIAccess.AssignWindowInternal(window);
			window.Show();
		}

		internal bool? ShowDialog(WindowVM windowVM) {
			var tWin = FindView(windowVM.GetType());
			var window = (Window)Activator.CreateInstance(tWin);
			window.DataContext = windowVM;
			windowVM.UIAccess.AssignWindowInternal(window);
			return window.ShowDialog();
		}

		private Type FindView(Type viewModelType) {
			// if the logic is changed, update the description in WindowVM.Show()

			if (viewModelType == typeof (WindowVM)) return typeof (Window); 

			var defView=viewModelType.GetCustomAttributes(typeof (DefaultViewAttribute), false).OfType<DefaultViewAttribute>().Select(x=>x.View).FirstOrDefault();
			if(defView!=null) return defView;

			var n = viewModelType.Name;																								//"MainWindowVM"
			var fn = viewModelType.FullName;																						//"ProDevis.UI.ViewModels.MainWindowVM"
			var vmExt = new[]{"VM","ViewModel"};
			var bn=n;
			var bfn = fn;
			foreach (var ext in vmExt) {
				if (n.EndsWith(ext)) {
					bn = n.Substring(0, n.Length - ext.Length);																		//"MainWindow"
					bfn = fn.Substring(0, fn.Length - ext.Length);																	//"ProDevis.UI.ViewModels.MainWindow"
					break;
				}
			}
			var viewExt="View";
			var vn = bn + viewExt;																									//"MainWindowView"
			var fvn = bfn + viewExt;																								//"ProDevis.UI.ViewModels.MainWindowView"

			Type t = null;
			t = viewModelType.Assembly.GetType(fvn, false); if (t != null) return t;
			t = viewModelType.Assembly.GetType(bfn, false); if (t != null) return t;

			var allTypes = viewModelType.Assembly.GetTypes();
			var m=allTypes.Where(x => x.Name == vn).ToArray();
			if (m.Length == 1) return m[0];
			m=allTypes.Where(x => x.Name == bn).ToArray();
			if (m.Length == 1) return m[0];

			throw new TypeLoadException("View not found! Name: "+vn+"|"+bn +" ViewModel: "+viewModelType.FullName);
			// return null;
		}
	}
}