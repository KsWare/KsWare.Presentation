using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.Core;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides the ability to create, configure, show, and manage the lifetime of windows.
	/// </summary>
	/// <remarks>
	/// <para>Note: Some properties make it possible to access the Window (as reference) directly.
	/// This direct access can be enabled using <see cref="UIAccessClass.IsDirectAccessEnabled">UI.IsDirectAccessEnabled</see>.
	/// This would violates the MVVM pattern and lets act this object as 'Controller' in a MVC pattern.</para>
	/// </remarks>
	public partial class WindowVM:DataVM<Window>,IWindowVM {

		private bool m_Disposed;
		private bool m_IsClosing;

		/// <summary> Initializes a new instance of the <see cref="WindowVM"/> class.
		/// </summary>
		public WindowVM() {//
			RegisterChildren(_=>this);

			UIAccess = new UIAccessClass();
			UIAccess.WindowChanged += AtWindowChanged;
			UIAccess.IsDirectAccessEnabled = true; //TODO revise. because experimantal usage, we need always enable direct access

			CloseAction.MːDoAction = DoClose;
			MinimizeAction.MːDoAction = DoMinimize;
			MaximizeAction.MːDoAction = DoMaximize;
			RestoreAction.MːDoAction = DoRestore;
			FullscreenAction.MːDoAction = DoFullscreen;
			Initialize();
		}

		/// <summary> Provides direct access to UI properties
		/// </summary>
		public UIAccessClass UIAccess { get; private set; }

		/// <summary> Gets the title view model.
		/// </summary>
		/// <value> The view model for title.</value>
		public StringVM Title { get; private set; }

		public ActionVM MinimizeAction { get; private set; }
		public ActionVM MaximizeAction { get; private set; }
		public ActionVM RestoreAction { get; private set; }
		public ActionVM FullscreenAction { get; private set; }
		public ActionVM CloseAction { get; private set; }

		public void Show() {
			ApplicationVM.Current.WindowsInternal.Show(this);
		}

		private void AtWindowChanged(object sender, EventArgs e) {
			if (UIAccess.Window != null) {
				UIAccess.Window.Closed      += AtWindowClosed;
				UIAccess.Window.Activated   += AtWindowActivated;
				UIAccess.Window.Deactivated += AtWindowDeactivated;				
			}
		}

		private void AtWindowDeactivated(object sender, EventArgs e) {IsActivated = false;}
		private void AtWindowActivated(object sender, EventArgs e) { IsActivated = true; }

		/// <summary> Gets a value indicating whether the window is the foreground window.
		/// </summary>
		/// <value><c>true</c> the window is the foreground window; otherwise, <c>false</c>.</value>
		public bool IsActivated { get { return Fields.Get<bool>("IsActivated"); } private set { Fields.Set("IsActivated", value); } }

		private void AtWindowClosed(object sender, EventArgs e) {
			
		}

		private void AtCloseActionExecuted(object sender, ExecutedEventArgs e) {
			//RequestUserFeedback(new CommandUserFeedback{Command=ApplicationCommands.Close});
			if (UIAccess.Window != null) {
				UIAccess.Window.Close(); // --> AtWindowClosed
			}
		}

		private void DoMinimize() {  }

		private void DoMaximize() { }

		private void DoRestore() {  }

		private void DoFullscreen() {  }

		private void DoClose() {  }

		// Type: System.Windows.Window
		// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		private void UpdateWindowListsOnClose() {
//			WindowCollection ownedWindowsInternal = this.OwnedWindowsInternal;
//			while (ownedWindowsInternal.Count > 0) ownedWindowsInternal[0].InternalClose(false, true);
//			if (!this.IsOwnerNull) this.Owner.OwnedWindowsInternal.Remove(this);
//			if (!this.IsInsideApp) return;
			if (ApplicationVM.Current.Dispatcher.Thread == ApplicationDispatcher.CurrentDispatcher.Thread) {
				App.WindowsInternal.Remove(this);
//				if (!this._appShuttingDown && (this.App.Windows.Count == 0 && this.App.ShutdownMode == ShutdownMode.OnLastWindowClose || this.App.MainWindow == this && this.App.ShutdownMode == ShutdownMode.OnMainWindowClose)) this.App.CriticalShutdown(0);
//				this.TryClearingMainWindow();
			}
			else App.NonAppWindowsInternal.Remove(this);
		}

		// Type: System.Windows.Window
		// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		private void Initialize() {
			if(IsInDesignMode) return;
//			this.BypassLayoutPolicies = true;
//			if (!this.IsInsideApp) return;
			if (ApplicationVM.Current.Dispatcher.Thread == ApplicationDispatcher.CurrentDispatcher.Thread) {
				App.WindowsInternal.Add(this);
				if (App.MainWindow != null) return;
				App.MainWindow = this;
			}
			else this.App.NonAppWindowsInternal.Add(this);
		}

		internal bool IsDisposed {get { return m_Disposed; }}

		private ApplicationVM App { get { return ApplicationVM.Current; } }
//		private bool IsInsideApp { get { return Application.Current != null; } }


		protected override void Dispose(bool explicitDisposing) {
			if (explicitDisposing) {
				m_Disposed = true;
				UpdateWindowListsOnClose();
				m_IsClosing = false;
			}
			base.Dispose(explicitDisposing);
		}



		// ###

		/// <summary> Provides direct access to the UI (<see cref="Window"/>).
		/// </summary>
		public sealed class UIAccessClass:INotifyPropertyChanged,IDisposable {

			private readonly BackingFieldsStore Fields;

			public UIAccessClass() {
				Fields=new BackingFieldsStore(this,OnPropertyChanged);
				Fields[_=>Window].ValueChanged+=(s,e) => EventUtil.Raise(WindowChanged,this, new ValueChangedEventArgs<Window>((Window) e.PreviousValue,(Window) e.NewValue),"{5AC93EA1-BB23-49CE-86F4-7DA533825945}");
			}

			/// <summary> Gets or sets a value indicating whether direct UI access is enabled.
			/// </summary>
			/// <value><c>true</c> if direct UI access is enabled; otherwise, <c>false</c>.</value>
			/// <remarks>
			/// If <see cref="IsDirectAccessEnabled"/> is set to <c>true</c> The view model can access the <see cref="Window"/> directly. That means read/write properties, register for events, call methods, etc.
			/// <para>Note: This feature should be used only if <see cref="Binding"/> is not possible.</para>
			/// <para>Because direct access to Window (reference) setting to true violates the MVVM pattern and lets act this object as 'Controller' in a MVC pattern)</para>
			/// </remarks>
			public bool IsDirectAccessEnabled { get; set; }

			/// <summary> Gets the <see cref="Window"/>.
			/// </summary>
			/// <value>The <see cref="Window"/>.</value>
			public Window Window { get { DemandAccess(); return Fields.Get<Window>("Window"); } }

			/// <summary> Gets a value indicating whether this instance has window.
			/// </summary>
			/// <value><c>true</c> if this instance has window; otherwise, <c>false</c>.</value>
			public bool HasWindow { get { return Fields.Get<Window>("Window") != null; } }

			internal void AssignWindowInternal(Window window) {
				if(window==null) throw new ArgumentNullException("window");
				DemandAssign();
				Fields.Set("Window", window);
			}

			public event EventHandler<ValueChangedEventArgs<Window>> WindowChanged;

			/// <summary> Creates an instance of the specified type using that type's default constructor.
			/// </summary>
			/// <param name="view">The type of window to create. </param>
			public void CreateInstance(Type view) {
				DemandAccess();DemandAssign();
				var window = (Window) Activator.CreateInstance(view);
				AssignWindowInternal(window);
			}

			/// <summary> Creates an instance of the specified window type using the constructor that best matches the specified parameters.
			/// </summary>
			/// <param name="view">The type of window to create. </param>
			/// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If args is an empty array or null, the constructor that takes no parameters (the default constructor) is invoked. </param>
			public void CreateInstance(Type view, params object[] args) {
				DemandAccess();DemandAssign();
				var window = (Window) Activator.CreateInstance(view,args);
				AssignWindowInternal(window);
			}

			private void DemandAccess() {if(!IsDirectAccessEnabled) throw new InvalidOperationException("Direct UI access is not allowed!");}

			private void DemandAssign() {if(HasWindow) throw new InvalidOperationException("Window already assigned!");}

//			[Obsolete("Use indexer",true)]
//			public void RegisterPropertyChangedHandler<TProperty>(Expression<Func<object, TProperty>> viewModelProperty, EventHandler propertyChangedEventHandler) {
//				Fields.RegisterPropertyChangedHandler(viewModelProperty, propertyChangedEventHandler);
//			}

//			[Obsolete("Use indexer",true)]
//			public void ReleasePropertyChangedHandler<TProperty>(Expression<Func<object, TProperty>> viewModelProperty, EventHandler propertyChangedEventHandler) {
//				Fields.ReleasePropertyChangedHandler(viewModelProperty, propertyChangedEventHandler);
//			}

			#region INotifyPropertyChanged
			public event PropertyChangedEventHandler PropertyChanged;

			[NotifyPropertyChangedInvocator]
			private void OnPropertyChanged(string propertyName) {
				var handler = PropertyChanged;
				if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
			}
			#endregion

			#region Implementation of IDisposable

			public void Dispose() {
				Fields.Dispose();
			}

			#endregion
		}
	}

	public class DialogWindowVM : WindowVM {

		/// <summary> Opens a window and returns only when the newly opened window is closed.
		/// </summary>
		/// <returns>A Nullable{T} value of type Boolean that specifies whether the activity was accepted (true) or canceled (false). The return value is the value of the DialogResult property before a window closes.</returns>
		/// <seealso cref="Window.ShowDialog"/>
		public bool? ShowDialog() { return UIAccess.Window.ShowDialog(); }
	}

	/// <summary> IWindowVM
	/// </summary>
	/// <see cref="System.Windows.Window"/>
	public interface IWindowVM {
		ActionVM CloseAction { get; }
		void Show();
//		void Close();
	}

	/// <summary> [DRAFT] IDialogWindowVM
	/// </summary>
	/// <see cref="System.Windows.DialogWindow"/>
	public interface IDialogWindowVM {
		ActionVM CloseAction { get; }
//		ActionVM HelpAction { get; } //???
		bool? DialogResult { get; set; }
	}

	/// <summary> [DRAFT] IOverlayWindowVM
	/// </summary>
	/// <see cref="System.Windows.Controls.Primitives.Popup"/>
	public interface IOverlayWindowVM {
		
	}
}
