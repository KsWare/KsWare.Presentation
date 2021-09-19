using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides the ability to create, configure, show, and manage the lifetime of windows.
	/// </summary>
	/// <remarks>
	/// <para>Note: Some properties make it possible to access the Window (as reference) directly.
	/// This direct access can be enabled using <see cref="UIAccessClass.IsDirectAccessEnabled">UI.IsDirectAccessEnabled</see>.
	/// This would violates the MVVM pattern and lets act this object as 'Controller' in a MVC pattern.</para>
	/// </remarks>
	public partial class WindowVM : DataVM<Window>, IWindowVM {

		private bool _disposed;
		private bool _isClosing;
		private WindowProperties _fullscreenRestore;

		/// <summary> Initializes a new instance of the <see cref="WindowVM"/> class.
		/// </summary>
		public WindowVM() {//
			RegisterChildren(()=>this);

			UIAccess = new UIAccessClass();
			UIAccess.WindowChanged += AtWindowChanged;
			UIAccess.IsDirectAccessEnabled = true; //TODO revise. because experimental usage, we need always enable direct access

			CloseAction.MːDoAction = DoClose;
			MinimizeAction.MːDoAction = DoMinimize;
			MaximizeAction.MːDoAction = DoMaximize;
			RestoreAction.MːDoAction = DoRestore;
			FullscreenAction.MːDoAction = DoFullscreen;

			Fields[nameof(IsFullScreen)].ValueChangedEvent.add = AtIsFullScreenChanged;
			Fields[nameof(Owner)].ValueChangedEvent.add = (s, e) => UpdateViewOwner();

			Initialize();
		}

		/// <summary> Provides direct access to UI properties
		/// </summary>
		public UIAccessClass UIAccess { get; private set; }

		/// <summary> Gets the title view model.
		/// </summary>
		/// <value> The view model for title.</value>
		public StringVM Title { get; [UsedImplicitly] private set; }

		/// <inheritdoc cref="IWindowVM.Owner"/>
		public IWindowVM Owner { get => Fields.GetValue<IWindowVM>(); set => Fields.SetValue(value); }

		public ActionVM MinimizeAction { get; [UsedImplicitly] private set; }
		public ActionVM MaximizeAction { get;[UsedImplicitly]  private set; }
		public ActionVM RestoreAction { get; [UsedImplicitly] private set; }
		public ActionVM FullscreenAction { get; [UsedImplicitly] private set; }
		public ActionVM CloseAction { get; [UsedImplicitly] private set; }

		/// <summary> Shows the view.
		/// </summary>
		/// <remarks>
		/// <para>The type for the view can be configured by a <see cref="DefaultViewAttribute"/>. If no attribute is specified the following naming conventions are used: <br/>
		/// (name of view model without "VM" | "ViewModel") + (optional "View").</para>
		/// <para>For example: for "MainWindowVM" a <see cref="Window"/> class with class name "MainWindow" or "MainWindowView" is searched.</para>
		/// </remarks>
		/// <seealso cref="DefaultViewAttribute"/>
		public void Show() {
			if(!UIAccess.HasWindow) ApplicationVM.Current.WindowsInternal.Show(this);
			else UIAccess.Window.Show();
		}

		/// <summary> Manually closes the window.
		/// </summary>
		public void Close() {
			if(UIAccess.HasWindow) UIAccess.Window.Close();
		}

		private void AtWindowChanged(object sender, ValueChangedEventArgs<Window> e) {
			if (e.PreviousValue != null) {
				UIAccess.Window.Closing     -= AtWindowClosing;
				UIAccess.Window.Closed      -= AtWindowClosed;
				UIAccess.Window.Activated   -= AtWindowActivated;
				UIAccess.Window.Deactivated -= AtWindowDeactivated;
			}
			if (e.NewValue !=null) {
				UIAccess.Window.Closing     += AtWindowClosing;
				UIAccess.Window.Closed      += AtWindowClosed;
				UIAccess.Window.Activated   += AtWindowActivated;
				UIAccess.Window.Deactivated += AtWindowDeactivated;
				UpdateViewOwner();
			}
			OnPropertyChanged(nameof(IsOpen));
		}

		private void AtWindowDeactivated(object sender, EventArgs e) {IsActivated = false;}
		private void AtWindowActivated(object sender, EventArgs e) { IsActivated = true; }

		/// <summary> Gets a value indicating whether the window is the foreground window.
		/// </summary>
		/// <value><c>true</c> the window is the foreground window; otherwise, <c>false</c>.</value>
		public bool IsActivated { get => Fields.GetValue<bool>(); private set => Fields.SetValue(value); }

		private void AtWindowClosing(object sender, CancelEventArgs e) {
			Closing?.Invoke(this, e);
			EventManager.Raise<CancelEventHandler, CancelEventArgs>(LazyWeakEventStore, nameof(ClosingEvent), e);
		}

		public event CancelEventHandler Closing;

		/// <summary> Gets the event source for the event which occurs when window is closing.
		/// </summary>
		/// <value>The window closing event source.</value>
		/// <seealso cref="Window.Closing"/>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IEventSource<CancelEventHandler> ClosingEvent => EventSources.Get<CancelEventHandler>();

		private void AtWindowClosed(object sender, EventArgs e) {
			UIAccess.Window.DataContext = null;
			UIAccess.ReleaseWindowInternal();
			Closed?.Invoke(this, e);
			EventManager.Raise<EventHandler, EventArgs>(LazyWeakEventStore, nameof(ClosedEvent), e);
		}

		public event EventHandler Closed;

		/// <summary> Gets the event source for the event which occurs when window is closed.
		/// </summary>
		/// <value>The window closed event source.</value>
		/// <seealso cref="Window.Closed"/>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IEventSource<EventHandler> ClosedEvent => EventSources.Get<EventHandler>();

		private void AtCloseActionExecuted(object sender, ExecutedEventArgs e) {
			//RequestUserFeedback(new CommandUserFeedback{Command=ApplicationCommands.Close});
			if (UIAccess.Window != null) {
				UIAccess.Window.Close(); // --> AtWindowClosed
			}
		}

		protected virtual void DoMinimize() {
			if (!UIAccess.HasWindow) return;
			if (_fullscreenRestore != null)
				_fullscreenRestore.RestoreFromFullScreen(UIAccess.Window, WindowState.Minimized);
			else
				UIAccess.Window.WindowState=WindowState.Minimized;
		}

		protected virtual void DoMaximize() {
			if (!UIAccess.HasWindow) return;
			if (_fullscreenRestore != null)
				_fullscreenRestore.RestoreFromFullScreen(UIAccess.Window, WindowState.Maximized);
			else
				UIAccess.Window.WindowState = WindowState.Maximized;
		}

		protected virtual void DoRestore() {
			if (!UIAccess.HasWindow) return;
			if (_fullscreenRestore != null)
				_fullscreenRestore.RestoreFromFullScreen(UIAccess.Window, WindowState.Normal);
			else
				UIAccess.Window.WindowState = WindowState.Normal;
		}

		protected virtual void DoFullscreen() {IsFullScreen = true;}

		public bool IsFullScreen { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		private void AtIsFullScreenChanged(object sender, ValueChangedEventArgs e) {
			if (IsFullScreen) {
				_fullscreenRestore = WindowProperties.PrepareFullScreenRestore(UIAccess.Window);
				UIAccess.Window.WindowStyle = WindowStyle.None;
				UIAccess.Window.WindowState = WindowState.Maximized;
			}
			else {
				_fullscreenRestore.RestoreFromFullScreen(UIAccess.Window);
				_fullscreenRestore = null;
			}
		}

		protected virtual void DoClose() {  Close();}

		public bool IsOpen {
			get => UIAccess.HasWindow;
			set {
				if (value) {
					if(!UIAccess.HasWindow) Show();
				} else {
					if(UIAccess.HasWindow) Close();
				}
			}
		}

		// Type: System.Windows.Window
		// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		private void UpdateWindowListsOnClose() {
//			WindowCollection ownedWindowsInternal = this.OwnedWindowsInternal;
//			while (ownedWindowsInternal.Count > 0) ownedWindowsInternal[0].InternalClose(false, true);
//			if (!this.IsOwnerNull) this.Owner.OwnedWindowsInternal.Remove(this);
//			if (!this.IsInsideApp) return;
			if (ApplicationVM.Current.Dispatcher.Thread == ApplicationDispatcher.Thread) {
				App.WindowsInternal.Remove(this);
//				if (!this._appShuttingDown && (this.App.Windows.Count == 0 && this.App.ShutdownMode == ShutdownMode.OnLastWindowClose || this.App.MainWindow == this && this.App.ShutdownMode == ShutdownMode.OnMainWindowClose)) this.App.CriticalShutdown(0);
//				this.TryClearingMainWindow();
			}
			else App.NonAppWindowsInternal.Remove(this);
		}

		private void UpdateViewOwner() {
			// TODO for IWindowVM silently ignored!
			if (Owner is WindowVM w && UIAccess.HasWindow) UIAccess.Window.Owner = w.UIAccess.Window; 
		}

		// Type: System.Windows.Window
		// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		private void Initialize() {
			if(IsInDesignMode) return;
//			this.BypassLayoutPolicies = true;
//			if (!this.IsInsideApp) return;
			if (ApplicationVM.Current.Dispatcher.Thread == ApplicationDispatcher.Instance.Thread) {
				App.WindowsInternal.Add(this);
				if (App.MainWindow != null) return;
				App.MainWindow = this;
			}
			else this.App.NonAppWindowsInternal.Add(this);
		}

		internal bool IsDisposed => _disposed;

		private ApplicationVM App => ApplicationVM.Current;
//		private bool IsInsideApp { get { return Application.Current != null; } }


		protected override void Dispose(bool explicitDisposing) {
			if (explicitDisposing) {
				_disposed = true;
				UpdateWindowListsOnClose();
				_isClosing = false;
			}
			base.Dispose(explicitDisposing);
		}

	}

}
