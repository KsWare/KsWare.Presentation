using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Interop;
using Point = System.Windows.Point;
using System.Drawing;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	/// <summary>
	///     Makes a Window Borderless without performance drop as when using AllowTransparency = 'true'.
	///     The Window keeps it's shadow and aero resize behavior but looses the rounded corners.
	///     @todo Desktop Dpi in some calculations might be wrong for multiple monitors?
	/// </summary>
	public class CustomWindowBehavior:Behavior<Window> {

		#region ResizeBorderWidth

		/// <summary>
		///     The width of the resize border.
		/// </summary>
		public static readonly DependencyProperty ResizeBorderWidthProperty=DependencyProperty.Register(
			"ResizeBorderWidth", typeof(double), typeof(CustomWindowBehavior), new PropertyMetadata(8.0));

		/// <summary>
		///     Gets or sets the width of the resize border.
		/// </summary>
		/// <value> The width of the resize border.</value>
		public double ResizeBorderWidth {
			get => (double)GetValue(ResizeBorderWidthProperty);
			set => SetValue(ResizeBorderWidthProperty, value);
		}

		#endregion

		#region TitleBarName

		/// <summary>
		///     Gets or sets the name of the title bar in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the title bar.
		/// </value>
		public static readonly DependencyProperty TitleBarNameProperty=DependencyProperty.Register(
			"TitleBarName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_titleBar"));

		/// <summary>
		///     Gets or sets the name of the title bar in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the title bar.
		/// </value>
		public string TitleBarName {
			get => (string)GetValue(TitleBarNameProperty);
			set => SetValue(TitleBarNameProperty, value);
		}

		public FrameworkElement TitleBar { get; set; }

		#endregion

		#region BorderName

		/// <summary>
		///     Gets or sets the name of the border in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the border.
		/// </value>
		public static readonly DependencyProperty BorderNameProperty=DependencyProperty.Register(
			"BorderName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_border"));

		/// <summary>
		///     Gets or sets the name of the border in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the border.
		/// </value>
		public string BorderName {
			get => (string)GetValue(BorderNameProperty);
			set => SetValue(BorderNameProperty, value);
		}

		public Border Border { get; set; }

		#endregion

		#region ResizeGripName

		/// <summary>
		///     The name of the resize grip in the window itself or it's template.
		/// </summary>
		public static readonly DependencyProperty ResizeGripNameProperty=DependencyProperty.Register(
			"ResizeGripName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_resizeGrip"));

		/// <summary>
		///     Gets or sets the name of the resize grip in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the resize grip.
		/// </value>
		public string ResizeGripName {
			get => (string)GetValue(ResizeGripNameProperty);
			set => SetValue(ResizeGripNameProperty, value);
		}

		public UIElement ResizeGrip { get; set; }

		#endregion

		#region CloseButtonName

		/// <summary>
		///     The name of the close button in the window itself or it's template.
		/// </summary>
		public static readonly DependencyProperty CloseButtonNameProperty=DependencyProperty.Register(
			"CloseButtonName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_closeButton"));

		/// <summary>
		///     Gets or sets the name of the close button in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the close button.
		/// </value>
		public string CloseButtonName {
			get => (string)GetValue(CloseButtonNameProperty);
			set => SetValue(CloseButtonNameProperty, value);
		}

		public Button CloseButton { get; set; }

		#endregion

		#region MinimizeButtonName

		/// <summary>
		///     The name of the minimize button in the window itself or it's template.
		/// </summary>
		public static readonly DependencyProperty MinimizeButtonNameProperty=DependencyProperty.Register(
			"MinimizeButtonName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_minimizeButton"));

		/// <summary>
		///     Gets or sets the name of the minimize button in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the minimize button.
		/// </value>
		public string MinimizeButtonName {
			get => (string)GetValue(MinimizeButtonNameProperty);
			set => SetValue(MinimizeButtonNameProperty, value);
		}

		public Button MinimizeButton { get; set; }

		#endregion

		#region MaximizeButtonName

		/// <summary>
		///     The name of the maximize button in the window itself or it's template.
		/// </summary>
		public static readonly DependencyProperty MaximizeButtonNameProperty=DependencyProperty.Register(
			"MaximizeButtonName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_maximizeButton"));

		/// <summary>
		///     Gets or sets the name of the maximize button in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the maximize button.
		/// </value>
		public string MaximizeButtonName {
			get => (string)GetValue(MaximizeButtonNameProperty);
			set => SetValue(MaximizeButtonNameProperty, value);
		}

		public Button MaximizeButton { get; set; }

		#endregion

		#region RestoreButtonName

		/// <summary>
		///     The name of the restore button in the window itself or it's template.
		/// </summary>
		public static readonly DependencyProperty RestoreButtonNameProperty=DependencyProperty.Register(
			"RestoreButtonName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_restoreButton"));

		/// <summary>
		///     Gets or sets the name of the restore button in the window itself or it's template.
		/// </summary>
		/// <value>The name of the restore button.</value>
		public string RestoreButtonName {
			get => (string)GetValue(RestoreButtonNameProperty);
			set => SetValue(RestoreButtonNameProperty, value);
		}

		public Button RestoreButton { get; set; }

		#endregion

		#region FullScreenButtonName

		/// <summary>
		///     The name of the full screen button in the window itself or it's template.
		/// </summary>
		public static readonly DependencyProperty FullScreenButtonNameProperty=DependencyProperty.Register(
			"FullScreenButtonName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_fullScreenButton"));

		/// <summary>
		///     Gets or sets the name of the "full screen" button in the window itself or it's template.
		/// </summary>
		/// <value>The name of the "<i>full screen</i>" button.</value>
		public string FullScreenButtonName {
			get => (string)GetValue(FullScreenButtonNameProperty);
			set => SetValue(FullScreenButtonNameProperty, value);
		}

		public Button FullScreenButton { get; set; }

		#endregion

		#region ExitFullScreenButtonName

		/// <summary>
		///     The name of the "exit full screen" button in the window itself or it's template.
		/// </summary>
		public static readonly DependencyProperty ExitFullScreenButtonNameProperty=DependencyProperty.Register(
			"ExitFullScreenButtonName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_exitFullScreenButton"));

		/// <summary>
		///     Gets or sets the name of the full screen button in the window itself or it's template.
		/// </summary>
		/// <value>The name of the "exit full screen" button.</value>
		public string ExitFullScreenButtonName {
			get => (string)GetValue(ExitFullScreenButtonNameProperty);
			set => SetValue(ExitFullScreenButtonNameProperty, value);
		}

		public Button ExitFullScreenButton { get; set; }

		#endregion

		#region IsFullscreen
		public static readonly DependencyProperty IsFullscreenProperty =
			DependencyProperty.Register("IsFullscreen", typeof (bool), typeof (CustomWindowBehavior), new PropertyMetadata(false,delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) { ((CustomWindowBehavior) o).AtIsFullscreenChanged(o, e); }));


		private void AtIsFullscreenChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ApplyIsFullscreen();
			EventUtil.Raise(IsFullscreenChanged,this,EventArgs.Empty,"{EB3C63D9-F0D9-4499-9705-465898C7281C}");
		}

		public bool IsFullscreen {
			get => (bool) GetValue(IsFullscreenProperty);
			set => SetValue(IsFullscreenProperty, value);
		}

		public EventHandler IsFullscreenChanged;
		#endregion

		#region NoDwmBorderName

		/// <summary>
		///     The name of the fallback border if dwm composition is disabled in the window itself or it's template.
		/// </summary>
		public static readonly DependencyProperty NoDwmBorderNameProperty=DependencyProperty.Register(
			"NoDwmBorderName", typeof(string), typeof(CustomWindowBehavior), new PropertyMetadata("_noDwmBorder"));

		/// <summary>
		///     Gets or sets the name of the fallback border if dwm composition is disabled in the window itself or it's template.
		/// </summary>
		/// <value>
		///     The name of the fallback border if dwm composition is disabled.
		/// </value>
		public string NoDwmBorderName {
			get => (string)GetValue(NoDwmBorderNameProperty);
			set => SetValue(NoDwmBorderNameProperty, value);
		}

		public Border NoDwmBorder { get; set; }

		#endregion

		public CustomWindowBehavior() {
			IsEnabled=true;
		}

		#region Behavior Implementation

		private IntPtr _hwnd;
		private HwndSource _hwndSource;
		private DependencyPropertyChangeNotifier _windowStatePropertyChangeNotifier;
		private Thickness _borderThickness;

		/// <summary>
		///     Called after the behavior is attached to an AssociatedObject.
		/// </summary>
		protected override void OnAttached() {
			if(AssociatedObject.IsInitialized)
				AddHwndHook();
			else
				AssociatedObject.SourceInitialized+=AtAssociatedObjectSourceInitialized;

			if(AssociatedObject.IsLoaded)
				ConfigureSpecialWindowParts();
			else
				AssociatedObject.Loaded+=AssociatedObjectLoaded;

			AssociatedObject.WindowStyle=WindowStyle.None;
			AssociatedObject.ResizeMode=ResizeMode.CanResize;

			base.OnAttached();
		}

		/// <summary>
		///     Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
		/// </summary>
		protected override void OnDetaching() {
			RemoveHwndHook();
			AssociatedObject.Loaded-=AssociatedObjectLoaded;
			if(_windowStatePropertyChangeNotifier!=null) {
				_windowStatePropertyChangeNotifier.Dispose();
				_windowStatePropertyChangeNotifier=null;
			}
			base.OnDetaching();
		}

		/// <summary>
		///     Adds the HWND hook.
		/// </summary>
		private void AddHwndHook() {
			_hwndSource=(HwndSource)PresentationSource.FromVisual(AssociatedObject);
			_hwndSource.AddHook(HwndHook);
			_hwnd=new WindowInteropHelper(AssociatedObject).Handle;
		}

		/// <summary> Removes the HWND hook.
		/// </summary>
		private void RemoveHwndHook() {
			AssociatedObject.SourceInitialized-=AtAssociatedObjectSourceInitialized;
			_hwndSource.RemoveHook(HwndHook);
		}

		/// <summary> Handles the SourceInitialized event of the AssociatedObject control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///     The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		private void AtAssociatedObjectSourceInitialized(object sender, EventArgs e) {
			AddHwndHook();
		}

		/// <summary> HwndHook.
		/// </summary>
		/// <param name="hWnd">The hWnd.</param>
		/// <param name="message">The message.</param>
		/// <param name="wParam">The wParam.</param>
		/// <param name="lParam">The lParam.</param>
		/// <param name="handled">
		///     if set to <c>true</c> [handled].
		/// </param>
		/// <returns></returns>
		private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled) {
			IntPtr returnValue=IntPtr.Zero;
			if(!IsEnabled) return returnValue;

			switch((WM)message) {
				case WM.NCCALCSIZE: {
					// Hides the border
					handled=true;
					break;
				}
				case WM.NCPAINT: {
					UpdateNoDwmBorderVisibility();

					// Works for Windows Vista and higher
					if(Environment.OSVersion.Version.Major>=6) {
						var m=new MARGINS {cxBottomHeight=1, cxLeftWidth=1, cxRightWidth=1, cxTopHeight=1};
						NativeFunctions.DwmExtendFrameIntoClientArea(_hwnd, ref m);
					}
					handled=true;
					break;
				}
				case WM.DWMCOMPOSITIONCHANGED:
					UpdateNoDwmBorderVisibility();
					break;
				case WM.NCACTIVATE: {
					// As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam does not
					// repaint the nonclient area to reflect the state change.
					returnValue=NativeFunctions.DefWindowProc(hWnd, message, wParam, new IntPtr(-1));
					handled=true;
					break;
				}
				case WM.GETMINMAXINFO: {
					WmGetMinMaxInfo(hWnd, lParam);
					handled=true;
					break;
				}
				case WM.NCHITTEST: {
					// Compute in what kind of client area we consider the mouse to be
					var deviceMousePosition=new Point(Util.GetXlparam(lParam), Util.GetYlparam(lParam));
					Rect deviceWindowPosition=Util.GetWindowRect(hWnd);

					handled=true;
					returnValue=new IntPtr(HitTestNca(hWnd, deviceMousePosition, deviceWindowPosition));
					break;
				}
			}

			return returnValue;
		}

		/// <summary> Handles the get min max info message.
		/// </summary>
		/// <param name="hwnd">The HWND.</param>
		/// <param name="lParam">The lParam.</param>
		private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
			var mmi=(MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

			// Adjust the maximized size and position to fit the work area of the correct monitor
			const int MONITOR_DEFAULTTONEAREST=0x00000002;
			IntPtr monitor=NativeFunctions.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

			if(monitor!=IntPtr.Zero) {
				var monitorInfo=new MONITORINFO();
				NativeFunctions.GetMonitorInfo(monitor, monitorInfo);
				RECT rcWorkArea=monitorInfo.rcWork;
				RECT rcMonitorArea=monitorInfo.rcMonitor;
				mmi.ptMaxPosition.x=Math.Abs(rcWorkArea.left-rcMonitorArea.left);
				mmi.ptMaxPosition.y=Math.Abs(rcWorkArea.top-rcMonitorArea.top);
				mmi.ptMaxSize.x=Math.Abs(rcWorkArea.right-rcWorkArea.left);
				// -1 to keep auto hiding taskbar working, otherwise we have fullscreen mode.
				mmi.ptMaxSize.y=Math.Abs(rcMonitorArea.bottom-rcMonitorArea.top)==Math.Abs(rcWorkArea.bottom-rcWorkArea.top)?Math.Abs(rcWorkArea.bottom-rcWorkArea.top-1):Math.Abs(rcWorkArea.bottom-rcWorkArea.top);

				Point minSize=Util.LogicalToDevice(new Point(AssociatedObject.MinWidth, AssociatedObject.MinHeight), hwnd);
				mmi.ptMinTrackSize=new POINT((int)Math.Ceiling(minSize.X), (int)Math.Ceiling(minSize.Y));
			}

			Marshal.StructureToPtr(mmi, lParam, true);
		}

		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
		private int HitTestNca(IntPtr hWnd, Point deviceMousePosition, Rect deviceWindowPosition) {
			Point logicalMousePosition=Util.DeviceToLogical(deviceMousePosition, hWnd);
			Rect logicalWindowPosition=Util.DeviceToLogical(deviceWindowPosition, hWnd);

			bool isMaximized=AssociatedObject.WindowState==WindowState.Maximized;

			// Determine if hit test is for resizing, default middle (1,1).
			int uRow=1;
			int uCol=1;
			bool onResizeBorder=false;
			double resizeBorderThickness=isMaximized?0:ResizeBorderWidth;

			// Determine if the point is at the top or bottom of the window.
			if(logicalMousePosition.Y>=logicalWindowPosition.Top&&logicalMousePosition.Y<logicalWindowPosition.Top+resizeBorderThickness) {
				onResizeBorder=(logicalMousePosition.Y<logicalWindowPosition.Top+resizeBorderThickness);
				uRow=0; // top (caption or resize border)
			} else if(logicalMousePosition.Y<logicalWindowPosition.Bottom&&logicalMousePosition.Y>=logicalWindowPosition.Bottom-resizeBorderThickness) {
				uRow=2; // bottom
			}

			// Determine if the point is at the left or right of the window.
			if(logicalMousePosition.X>=logicalWindowPosition.Left&&logicalMousePosition.X<logicalWindowPosition.Left+resizeBorderThickness) {
				uCol=0; // left side
			} else if(logicalMousePosition.X<logicalWindowPosition.Right&&logicalMousePosition.X>=logicalWindowPosition.Right-resizeBorderThickness) {
				uCol=2; // right side
			}

			// If the cursor is in one of the top edges by the caption bar, but below the top resize border,
			// then resize left-right rather than diagonally.
			if(uRow==0&&uCol!=1&&!onResizeBorder) {
				uRow=1;
			}

			// Detect ResizeGrip

			var htBorders=new[,] {
				{HT.TOPLEFT, HT.TOP, HT.TOPRIGHT},
				{HT.LEFT, HT.CLIENT, HT.RIGHT},
				{HT.BOTTOMLEFT, HT.BOTTOM, HT.BOTTOMRIGHT}
			};

			HT ht=htBorders[uRow, uCol];

			if(ht==HT.TOP&&!onResizeBorder)
				ht=HT.CLIENT;

			// Detect whether the Caption or the SizeGrip is hit
			if(ht==HT.CLIENT) {
				Point mousePositionInWindow=deviceMousePosition;
				mousePositionInWindow.Offset(-deviceWindowPosition.X, -deviceWindowPosition.Y);
				mousePositionInWindow=Util.DeviceToLogical(mousePositionInWindow, hWnd);
				var hit=AssociatedObject.InputHitTest(mousePositionInWindow) as FrameworkElement;

				if(hit!=null) {
					if(hit.Name==TitleBarName || hit==TitleBar)
						ht=HT.CAPTION;
					else if(hit.Name==ResizeGripName || hit==ResizeGrip)
						ht=HT.BOTTOMRIGHT;
				}
			}

			return (int)ht;
		}

		private void UpdateNoDwmBorderVisibility() {
			var noDwmBorder=NoDwmBorder??FindName(NoDwmBorderName) as Border;
			if(noDwmBorder==null) return;

			bool isCompositionEnabled=false;
			if(Environment.OSVersion.Version.Major>=6) NativeFunctions.DwmIsCompositionEnabled(out isCompositionEnabled);
			noDwmBorder.Visibility=!isCompositionEnabled?Visibility.Visible:Visibility.Collapsed;
		}

		private void AssociatedObjectLoaded(object sender, RoutedEventArgs e) {
			ConfigureSpecialWindowParts();
		}

		private object FindName(string name) {
			return name!=null?AssociatedObject.Template!=null?AssociatedObject.Template.FindName(name, AssociatedObject)??AssociatedObject.FindName(name):AssociatedObject.FindName(name):null;
		}

		private void ConfigureSpecialWindowParts() {
			var closeButton=CloseButton??FindName(CloseButtonName) as Button;
			if(closeButton!=null && closeButton.Command==null)
				closeButton.Command=new RelayCommand(p => AssociatedObject.Close());

			var minimizeButton=MinimizeButton??FindName(MinimizeButtonName) as Button;
			if(minimizeButton!=null && minimizeButton.Command==null)
				minimizeButton.Command=new RelayCommand(p => AssociatedObject.WindowState=WindowState.Minimized, p => AssociatedObject.WindowState!=WindowState.Minimized);

			var maximizeButton=MaximizeButton??FindName(MaximizeButtonName) as Button;
			if(maximizeButton!=null && maximizeButton.Command==null)
				maximizeButton.Command=new RelayCommand(p => AssociatedObject.WindowState=WindowState.Maximized, p => AssociatedObject.WindowState!=WindowState.Maximized);

			var restoreButton=RestoreButton??FindName(RestoreButtonName) as Button;
			if(restoreButton!=null && restoreButton.Command==null)
				restoreButton.Command=new RelayCommand(p => AssociatedObject.WindowState=WindowState.Normal, p => AssociatedObject.WindowState!=WindowState.Normal);

			var fullscreenButton=FullScreenButton??FindName(FullScreenButtonName) as Button;
			if(fullscreenButton!=null && fullscreenButton.Command==null)
				fullscreenButton.Command=new RelayCommand(p => IsFullscreen=true, p => !IsFullscreen);

			var exitfullscreenButton=ExitFullScreenButton??FindName(ExitFullScreenButtonName) as Button;
			if(exitfullscreenButton!=null && exitfullscreenButton.Command==null)
				exitfullscreenButton.Command=new RelayCommand(p => IsFullscreen=false, p => IsFullscreen);
			if(exitfullscreenButton!=null)exitfullscreenButton.Visibility=Visibility.Collapsed;

			var resizeGrip=ResizeGrip??FindName(ResizeGripName) as UIElement;
			if(resizeGrip!=null)
				resizeGrip.Visibility=AssociatedObject.WindowState==WindowState.Maximized||IsFullscreen?Visibility.Collapsed:Visibility.Visible;

			var border=Border??FindName(BorderName) as Border;
			if(border!=null)
				_borderThickness=border.BorderThickness;

			if(maximizeButton!=null&&restoreButton!=null) {
				_windowStatePropertyChangeNotifier=new DependencyPropertyChangeNotifier(AssociatedObject, Window.WindowStateProperty);
				_windowStatePropertyChangeNotifier.ValueChanged+=AtWindowStateChanged;

				maximizeButton.Visibility=AssociatedObject.WindowState==WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
				restoreButton .Visibility=AssociatedObject.WindowState==WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void AtWindowStateChanged(object sender, EventArgs e) {
			if(!IsEnabled) return;
			var resizeGrip=ResizeGrip??FindName(ResizeGripName) as UIElement;
			if(resizeGrip!=null)
				resizeGrip.Visibility=AssociatedObject.WindowState==WindowState.Maximized?Visibility.Collapsed:Visibility.Visible;

//			var border=Border??FindName(BorderName) as Border;
//			if(border!=null)
//				border.Visibility=AssociatedObject.WindowState==WindowState.Maximized?Visibility.Collapsed:Visibility.Visible;

			var minimizeButton=MinimizeButton??FindName(MinimizeButtonName) as Button;
			var maximizeButton=MaximizeButton??FindName(MaximizeButtonName) as Button;
			var restoreButton=RestoreButton??FindName(RestoreButtonName) as Button;
			var fullscreenButton=FullScreenButton??FindName(FullScreenButtonName) as Button;

			if(minimizeButton!=null&&minimizeButton.Command is RelayCommand)
				((RelayCommand)minimizeButton.Command).OnCanExecuteChanged();
			if(maximizeButton!=null&&maximizeButton.Command is RelayCommand)
				((RelayCommand)maximizeButton.Command).OnCanExecuteChanged();
			if(restoreButton!=null&&restoreButton.Command is RelayCommand)
				((RelayCommand)restoreButton.Command).OnCanExecuteChanged();
			if(fullscreenButton!=null&&fullscreenButton.Command is RelayCommand)
				((RelayCommand)fullscreenButton.Command).OnCanExecuteChanged();

			if(maximizeButton!=null&&restoreButton!=null) {
				maximizeButton.Visibility=AssociatedObject.WindowState==WindowState.Maximized?Visibility.Collapsed:Visibility.Visible;
				restoreButton.Visibility=AssociatedObject.WindowState==WindowState.Maximized?Visibility.Visible:Visibility.Collapsed;
			}
		}

		#endregion

		public bool IsEnabled { get; set; }

		private void ApplyIsFullscreen() {
			if(InDesignMode)return;

			var exitfullscreenButton=ExitFullScreenButton??FindName(ExitFullScreenButtonName) as Button;
			var border=Border??FindName(BorderName) as Border;

			if (IsFullscreen) {
				IsEnabled = false;
				AssociatedObject.Topmost = true;
				if(AssociatedObject.WindowState==WindowState.Maximized) AssociatedObject.WindowState=WindowState.Normal;
				AssociatedObject.WindowState = WindowState.Maximized;
				if(border!=null) border.BorderThickness = new Thickness(0);
				if(exitfullscreenButton!=null) exitfullscreenButton.Visibility=Visibility.Visible;
			} else {
				IsEnabled = true;
				AssociatedObject.Topmost = false;
				AssociatedObject.WindowState = WindowState.Normal;
				AssociatedObject.WindowStyle = WindowStyle.SingleBorderWindow; AssociatedObject.WindowStyle = WindowStyle.None;//Hack!
				if(border!=null) border.BorderThickness = _borderThickness;
				if(exitfullscreenButton!=null) exitfullscreenButton.Visibility=Visibility.Collapsed;
			}
		}

		/// <summary> Indicates whether or not the framework is in design-time mode.
		/// </summary>        
		private static bool InDesignMode {
			get {
				var prop=DesignerProperties.IsInDesignModeProperty;
				var inDesignMode=(bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

				if(!inDesignMode&&Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
					inDesignMode=true;

				return inDesignMode;
			}
		}

		#region Native

		// ReSharper disable InconsistentNaming
		// ReSharper disable UnusedMember.Global
		// ReSharper disable FieldCanBeMadeReadOnly.Global
		// ReSharper disable UnassignedField.Global

		/// <summary>
		///     Win32 HT... constants.
		/// </summary>
		public enum HT
		{

			/// <summary>
			///     On the screen background or on a dividing line between windows (same as HTNOWHERE, except that the
			///     DefWindowProc function produces a system beep to indicate an error).
			/// </summary>
			ERROR = -2,

			/// <summary>
			///     In a window currently covered by another window in the same thread (the message will be sent to underlying
			///     windows in the same thread until one of them returns a code that is not HTTRANSPARENT).
			/// </summary>
			TRANSPARENT = -1,

			/// <summary>
			///     On the screen background or on a dividing line between windows.
			/// </summary>
			NOWHERE = 0,

			/// <summary>
			///     In a client area.
			/// </summary>
			CLIENT = 1,

			/// <summary>
			///     In a title bar.
			/// </summary>
			CAPTION = 2,

			/// <summary>
			///     In a window menu or in a Close button in a child window.
			/// </summary>
			SYSMENU = 3,

			/// <summary>
			///     In a size box (same as HTSIZE).
			/// </summary>
			GROWBOX = 4,

			/// <summary>
			///     In a size box (same as HTGROWBOX).
			/// </summary>
			SIZE = GROWBOX,

			/// <summary>
			///     In a menu.
			/// </summary>
			MENU = 5,

			/// <summary>
			///     In a horizontal scroll bar.
			/// </summary>
			HSCROLL = 6,

			/// <summary>
			///     In the vertical scroll bar.
			/// </summary>
			VSCROLL = 7,

			/// <summary>
			///     In a Minimize button.
			/// </summary>
			MINBUTTON = 8,

			/// <summary>
			///     In a Maximize button.
			/// </summary>
			MAXBUTTON = 9,

			/// <summary>
			///     In the left border of a resizable window (the user can click the mouse to resize the window horizontally).
			/// </summary>
			LEFT = 10,

			/// <summary>
			///     In the right border of a resizable window (the user can click the mouse to resize the window horizontally).
			/// </summary>
			RIGHT = 11,

			/// <summary>
			///     In the upper-horizontal border of a window.
			/// </summary>
			TOP = 12,

			/// <summary>
			///     In the upper-left corner of a window border.
			/// </summary>
			TOPLEFT = 13,

			/// <summary>
			///     In the upper-right corner of a window border.
			/// </summary>
			TOPRIGHT = 14,

			/// <summary>
			///     In the lower-horizontal border of a resizable window (the user can click the mouse to resize the window vertically).
			/// </summary>
			BOTTOM = 15,

			/// <summary>
			///     In the lower-left corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).
			/// </summary>
			BOTTOMLEFT = 16,

			/// <summary>
			///     In the lower-right corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).
			/// </summary>
			BOTTOMRIGHT = 17,

			/// <summary>
			///     In the border of a window that does not have a sizing border.
			/// </summary>
			BORDER = 18,

			/// <summary>
			///     In a Minimize button.
			/// </summary>
			REDUCE = MINBUTTON,

			/// <summary>
			///     In a Maximize button.
			/// </summary>
			ZOOM = MAXBUTTON,

			/// <summary>
			///     In a Close button.
			/// </summary>
			CLOSE = 20,

			/// <summary>
			///     In a Help button.
			/// </summary>
			HELP = 21

		}

		/// <summary>
		///     Win32 SW_... constants.
		/// </summary>
		public enum SW
		{

			/// <summary>
			///     Minimizes a window, even if the thread that owns the window is not responding.
			///     This flag should only be used when minimizing windows from a different thread.
			/// </summary>
			FORCEMINIMIZE = 11,

			/// <summary>
			///     Hides the window and activates another window.
			/// </summary>
			HIDE = 0,

			/// <summary>
			///     Maximizes the specified window.
			/// </summary>
			MAXIMIZE = 3,

			/// <summary>
			///     Minimizes the specified window and activates the next top-level window in the Z order.
			/// </summary>
			MINIMIZE = 6,

			/// <summary>
			///     Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position.
			///     An application should specify this flag when restoring a minimized window.
			/// </summary>
			RESTORE = 9,

			/// <summary>
			///     Activates the window and displays it in its current size and position.
			/// </summary>
			SHOW = 5,

			/// <summary>
			///     Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the
			///     CreateProcess function by the program that started the application.
			/// </summary>
			SHOWDEFAULT = 10,

			/// <summary>
			///     Activates the window and displays it as a maximized window.
			/// </summary>
			SHOWMAXIMIZED = 3,

			/// <summary>
			///     Activates the window and displays it as a minimized window.
			/// </summary>
			SHOWMINIMIZED = 2,

			/// <summary>
			///     Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except
			///     the window is not activated.
			/// </summary>
			SHOWMINNOACTIVE = 7,

			/// <summary>
			///     Displays the window in its current size and position. This value is similar to SW_SHOW, except that
			///     the window is not activated.
			/// </summary>
			SHOWNA = 8,

			/// <summary>
			///     Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL,
			///     except that the window is not activated.
			/// </summary>
			SHOWNOACTIVATE = 4,

			/// <summary>
			///     Activates and displays a window. If the window is minimized or maximized, the system restores it to
			///     its original size and position. An application should specify this flag when displaying the window for
			///     the first time.
			/// </summary>
			SHOWNORMAL = 1

		}

		/// <summary>
		///     Win32 SWP_... constants.
		/// </summary>
		[Flags]
		public enum SWP
		{

			/// <summary>
			///     Retains the current position (ignores X and Y parameters).
			/// </summary>
			NOMOVE = 0x0002,

			/// <summary>
			///     Retains the current size (ignores the cx and cy parameters).
			/// </summary>
			NOSIZE = 0x0001,

			/// <summary>
			///     Displays the window.
			/// </summary>
			SHOWWINDOW = 0x0040,

			/// <summary>
			///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of
			///     either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
			/// </summary>
			NOACTIVATE = 0x0010

		}

		/// <summary>
		///     Win32 WM_... constants.
		/// </summary>
		public enum WM
		{

			/// <summary>
			///     Sent to a window when the size or position of the window is about to change. An application can use this message to override the window's
			///     default maximized size and position, or its default minimum or maximum tracking size.
			/// </summary>
			GETMINMAXINFO = 0x24,

			/// <summary>
			///     Sent to a window when its nonclient area needs to be changed to indicate an active or inactive state.
			/// </summary>
			NCACTIVATE = 0x86,

			/// <summary>
			///     Sent when the size and position of a window's client area must be calculated. By processing this message, an application can control the
			///     content of the window's client area when the size or position of the window changes.
			/// </summary>
			NCCALCSIZE = 0x83,

			/// <summary>
			///     The WM_NCPAINT message is sent to a window when its frame must be painted.
			/// </summary>
			NCPAINT = 0x85,

			/// <summary>
			///     Sent to a window in order to determine what part of the window corresponds to a particular screen coordinate. This can happen, for example,
			///     when the cursor moves, when a mouse button is pressed or released, or in response to a call to a function such as WindowFromPoint. If the
			///     mouse is not captured, the message is sent to the window beneath the cursor. Otherwise, the message is sent to the window that has
			///     captured the mouse.
			/// </summary>
			NCHITTEST = 0x0084,

			/// <summary>
			///     Informs all top-level windows that Desktop Window Manager (DWM) composition has been enabled or disabled.
			/// </summary>
			DWMCOMPOSITIONCHANGED = 0x031E

		}

		/// <summary>
		///     Returns the rectangle of the window identified by given handle.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="GetWindowRect")]
		public static Rect GetWindowRect(IntPtr hwnd) {
			RECT rect;
			if(!NativeFunctions.GetWindowRect(hwnd, out rect))
				throw new InvalidOperationException("GetWindowRect Error: "+Marshal.GetLastWin32Error());

			return new Rect(rect.left, rect.top, rect.Width, rect.Height);
		}

		/// <summary>
		///     Win32 MARGINS structure.
		/// </summary>
		public struct MARGINS
		{

			/// <summary>
			///     Width of the left border that retains its size.
			/// </summary>
			public int cxLeftWidth;

			/// <summary>
			///     Width of the right border that retains its size.
			/// </summary>
			public int cxRightWidth;

			/// <summary>
			///     Height of the top border that retains its size.
			/// </summary>
			public int cxTopHeight;

			/// <summary>
			///     Height of the bottom border that retains its size.
			/// </summary>
			public int cxBottomHeight;

			public override bool Equals(object obj) {
				var b=(MARGINS)obj; 
				return 
					cxLeftWidth.Equals(b.cxLeftWidth) && 
					cxRightWidth.Equals(b.cxRightWidth)&& 
					cxTopHeight.Equals(b.cxTopHeight)&& 
					cxBottomHeight.Equals(b.cxBottomHeight);
			}
			public override int GetHashCode() {throw new NotImplementedException();}
			public static bool operator == (MARGINS a, MARGINS b){return a.Equals(b);}
			public static bool operator != (MARGINS a, MARGINS b){return !a.Equals(b);}
		}

		/// <summary>
		///     Contains information about a window's maximized size and position and its minimum and maximum tracking size.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{

			/// <summary>
			///     Reserved; do not use.
			/// </summary>
			public POINT ptReserved;

			/// <summary>
			///     The maximized width (x member) and the maximized height (y member) of the window. For top-level windows, this value is based on
			///     the width of the primary monitor.
			/// </summary>
			public POINT ptMaxSize;

			/// <summary>
			///     The position of the left side of the maximized window (x member) and the position of the top of the maximized window (y member).
			///     For top-level windows, this value is based on the position of the primary monitor.
			/// </summary>
			public POINT ptMaxPosition;

			/// <summary>
			///     The minimum tracking width (x member) and the minimum tracking height (y member) of the window. This value can be obtained
			///     programmatically from the system metrics SM_CXMINTRACK and SM_CYMINTRACK (see the GetSystemMetrics function).
			/// </summary>
			public POINT ptMinTrackSize;

			/// <summary>
			///     The maximum tracking width (x member) and the maximum tracking height (y member) of the window. This value is based on the
			///     size of the virtual screen and can be obtained programmatically from the system metrics SM_CXMAXTRACK and SM_CYMAXTRACK (see the GetSystemMetrics function).
			/// </summary>
			public POINT ptMaxTrackSize;

			public override bool Equals(object obj) {
				var b=(MINMAXINFO)obj; 
				return 
					ptReserved.Equals(b.ptReserved) && 
					ptMaxSize.Equals(b.ptMaxSize)&& 
					ptMaxPosition.Equals(b.ptMaxPosition)&& 
					ptMinTrackSize.Equals(b.ptMinTrackSize)&&
					ptMaxTrackSize.Equals(b.ptMaxTrackSize);
			}
			public override int GetHashCode() {throw new NotImplementedException();}
			public static bool operator == (MINMAXINFO a, MINMAXINFO b){return a.Equals(b);}
			public static bool operator != (MINMAXINFO a, MINMAXINFO b){return !a.Equals(b);}
		};


		/// <summary>
		///     The MONITORINFO structure contains information about a display monitor.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public class MONITORINFO
		{

			/// <summary>
			///     The size of the structure, in bytes.
			/// </summary>
			public int cbSize=Marshal.SizeOf(typeof(MONITORINFO));

			/// <summary>
			///     A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. Note that if the monitor is not the
			///     primary display monitor, some of the rectangle's coordinates may be negative values.
			/// </summary>
			public RECT rcMonitor;

			/// <summary>
			///     A RECT structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates. Note that if the
			///     monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
			/// </summary>
			public RECT rcWork;

			/// <summary>
			///     A set of flags that represent attributes of the display monitor.
			///     The following flag is defined.
			///     MONITORINFOF_PRIMARY    This is the primary display monitor.
			/// </summary>
			public int dwFlags;

		}

		/// <summary>
		///     Contains information about the mouse's location in screen coordinates.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
		public struct MOUSEMOVEPOINT
		{

			/// <summary>
			///     The x-coordinate of the mouse.
			/// </summary>
			public int x;

			/// <summary>
			///     The y-coordinate of the mouse.
			/// </summary>
			public int y;

			/// <summary>
			///     The time stamp of the mouse coordinate.
			/// </summary>
			public int time;

			/// <summary>
			///     Additional information associated with this coordinate.
			/// </summary>
			public IntPtr dwExtraInfo;

			/// <summary>
			///     Returns x,y coordinates.
			/// </summary>
			public override string ToString() {
				return $"{x},{y}";
			}

		}

		/// <summary>
		///     Contains wrappers for Win32 functions.
		/// </summary>
		public static class NativeFunctions
		{

			/// <summary>
			///     Extends the window frame into the client area.
			/// </summary>
			/// <param name="hWnd">The handle to the window in which the frame will be extended into the client area.</param>
			/// <param name="pMarInset">A pointer to a MARGINS structure that describes the margins to use when extending the frame into the client area.</param>
			/// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
			[DllImport("dwmapi.dll")]
			public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

			/// <summary>
			///     Obtains a value that indicates whether Desktop Window Manager (DWM) composition is enabled.
			///     Applications can listen for composition state changes by handling the WM_DWMCOMPOSITIONCHANGED notification.
			/// </summary>
			/// <param name="enabled">A pointer to a value that, when this function returns successfully, receives TRUE if DWM composition is enabled; otherwise, FALSE.</param>
			/// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
			[DllImport("dwmapi.dll")]
			public static extern int DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] out bool enabled);

			/// <summary>
			///     Retrieves the dimensions of the bounding rectangle of the specified window.
			///     The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
			/// </summary>
			/// <param name="hWnd">A handle to the window.</param>
			/// <param name="lpRect">A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</param>
			/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
			[DllImport("user32.dll", SetLastError=true)]
			[return:MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

			/// <summary>
			///     The GetMonitorInfo function retrieves information about a display monitor.
			/// </summary>
			/// <param name="hMonitor">A handle to the display monitor of interest.</param>
			/// <param name="lpmi">
			///     A pointer to a MONITORINFO or MONITORINFOEX structure that receives information about the specified display monitor.
			///     You must set the cbSize member of the structure to sizeof(MONITORINFO) or sizeof(MONITORINFOEX) before calling the GetMonitorInfo function.
			///     Doing so lets the function determine the type of structure you are passing to it.
			///     The MONITORINFOEX structure is a superset of the MONITORINFO structure.
			///     It has one additional member: a string that contains a name for the display monitor.
			///     Most applications have no use for a display monitor name, and so can save some bytes by using a MONITORINFO structure.
			/// </param>
			/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
			[DllImport("user32")]
			[return:MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

			/// <summary>
			///     The MonitorFromWindow function retrieves a handle to the display monitor that has the
			///     largest area of intersection with the bounding rectangle of a specified window.
			/// </summary>
			/// <param name="handle">A handle to the window of interest.</param>
			/// <param name="flags">
			///     Determines the function's return value if the window does not intersect any display monitor.
			///     This parameter can be one of the following values.
			///     Value                       Meaning
			///     MONITOR_DEFAULTTONEAREST    Returns a handle to the display monitor that is nearest to the window.
			///     MONITOR_DEFAULTTONULL       Returns NULL.
			///     MONITOR_DEFAULTTOPRIMARY    Returns a handle to the primary display monitor.
			/// </param>
			/// <returns>
			///     If the window intersects one or more display monitor rectangles, the return value is an HMONITOR handle to the display monitor
			///     that has the largest area of intersection with the window. If the window does not intersect a display monitor, the return value depends
			///     on the value of dwFlags.
			/// </returns>
			[DllImport("User32")]
			public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

			/// <summary>
			///     Calls the default window procedure to provide default processing for any window messages that an application does not process. This function
			///     ensures that every message is processed. DefWindowProc is called with the same parameters received by the window procedure.
			/// </summary>
			/// <param name="hwnd">A handle to the window procedure that received the message.</param>
			/// <param name="msg">The message.</param>
			/// <param name="wParam">Additional message information. The content of this parameter depends on the value of the Msg parameter.</param>
			/// <param name="lParam">Additional message information. The content of this parameter depends on the value of the Msg parameter.</param>
			/// <returns>The return value is the result of the message processing and depends on the message.</returns>
			[DllImport("user32.dll")]
			public static extern IntPtr DefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

			/// <summary>
			///     Sets the specified window's show state.
			/// </summary>
			/// <param name="hWnd">A handle to the window.</param>
			/// <param name="command">
			///     Controls how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow,
			///     if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value
			///     should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of
			///     the following values.
			///     SW_...
			/// </param>
			/// <returns>If the window was previously visible, the return value is nonzero. If the window was previously hidden, the return value is zero.</returns>
			[DllImport("user32.dll", EntryPoint="ShowWindow")]
			[return:MarshalAs(UnmanagedType.Bool)]
			public static extern bool ShowWindow(IntPtr hWnd, SW command);

			/// <summary>
			///     Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen.
			///     The topmost window receives the highest rank and is the first window in the Z order.
			/// </summary>
			/// <param name="hWnd">A handle to the window.</param>
			/// <param name="hWndInsertAfter">
			///     A handle to the window to precede the positioned window in the Z order.
			///     This parameter must be a window handle or one of the following values.
			///     Value                   Meaning
			///     HWND_BOTTOM (HWND)1     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its
			///     topmost status and is placed at the bottom of all other windows.
			///     HWND_NOTOPMOST(HWND)-2  Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if
			///     the window is already a non-topmost window.
			///     HWND_TOP(HWND)0         Places the window at the top of the Z order.
			///     HWND_TOPMOST(HWND)-1    Places the window above all non-top
			/// </param>
			/// <param name="x">The new position of the left side of the window, in client coordinates.</param>
			/// <param name="y">The new position of the top of the window, in client coordinates.</param>
			/// <param name="cx">The new width of the window, in pixels.</param>
			/// <param name="cy">The new height of the window, in pixels.</param>
			/// <param name="wFlags">
			///     The window sizing and positioning flags. This parameter can be a combination of the following values.
			///     SWP_...
			/// </param>
			/// <returns></returns>
			[DllImport("user32.dll", EntryPoint="SetWindowPos")]
			public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP wFlags);

			/// <summary>
			///     Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
			/// </summary>
			/// <param name="hWnd">A handle to the window.</param>
			/// <param name="lpwndpl">
			///     A pointer to the WINDOWPLACEMENT structure that receives the show state and position information. Before calling GetWindowPlacement,
			///     set the length member to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails if lpwndpl-> length is not set correctly.
			/// </param>
			/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
			[DllImport("user32.dll", SetLastError=true)]
			[return:MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

			/// <summary>
			///     Retrieves a handle to a window whose class name and window name match the specified strings. The function searches child windows,
			///     beginning with the one following the specified child window. This function does not perform a case-sensitive search.
			/// </summary>
			/// <param name="parentHandle">
			///     A handle to the parent window whose child windows are to be searched.
			///     If hwndParent is NULL, the function uses the desktop window as the parent window.
			///     The function searches among windows that are child windows of the desktop.
			///     If hwndParent is HWND_MESSAGE, the function searches all message-only windows.
			/// </param>
			/// <param name="childAfter">
			///     A handle to a child window. The search begins with the next child window in the Z order.
			///     The child window must be a direct child window of hwndParent, not just a descendant window.
			///     If hwndChildAfter is NULL, the search begins with the first child window of hwndParent. Note that if both hwndParent
			///     and hwndChildAfter are NULL, the function searches all top-level and message-only windows.
			/// </param>
			/// <param name="className">
			///     The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function.
			///     The atom must be placed in the low-order word of lpszClass; the high-order word must be zero.
			///     If lpszClass is a string, it specifies the window class name. The class name can be any name registered with RegisterClass
			///     or RegisterClassEx, or any of the predefined control-class names, or it can be MAKEINTATOM(0x8000). In this latter case, 0x8000
			///     is the atom for a menu class. For more information, see the Remarks section of this topic.
			/// </param>
			/// <param name="windowTitle">The window name (the window's title). If this parameter is NULL, all window names match.</param>
			/// <returns>
			///     If the function succeeds, the return value is a handle to the window that has the specified class and window names.
			///     If the function fails, the return value is NULL. To get extended error information, call GetLastError.
			/// </returns>
			[DllImport("user32.dll", SetLastError=true)]
			public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

			/// <summary>
			///     Destroys the specified window. The function sends WM_DESTROY and WM_NCDESTROY messages to the window to deactivate it and remove
			///     the keyboard focus from it. The function also destroys the window's menu, flushes the thread message queue, destroys timers, removes
			///     clipboard ownership, and breaks the clipboard viewer chain (if the window is at the top of the viewer chain).
			///     If the specified window is a parent or owner window, DestroyWindow automatically destroys the associated child or owned windows
			///     when it destroys the parent or owner window. The function first destroys child or owned windows, and then it destroys the parent or owner window.
			///     DestroyWindow also destroys modeless dialog boxes created by the CreateDialog function.
			/// </summary>
			/// <param name="hWnd">A handle to the window to be destroyed.</param>
			/// <returns>
			///     If the function succeeds, the return value is nonzero.
			///     If the function fails, the return value is zero. To get extended error information, call GetLastError.
			/// </returns>
			[DllImport("user32.dll", SetLastError=true)]
			[return:MarshalAs(UnmanagedType.Bool)]
			public static extern bool DestroyWindow(IntPtr hWnd);

			/// <summary>
			///     Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
			/// </summary>
			/// <param name="hWnd">A handle to the window.</param>
			/// <param name="lpdwProcessId">
			///     A pointer to a variable that receives the process identifier. If this parameter is not NULL, GetWindowThreadProcessId copies
			///     the identifier of the process to the variable; otherwise, it does not.
			/// </param>
			/// <returns>The return value is the identifier of the thread that created the window.</returns>
			[DllImport("user32.dll", SetLastError=true)]
			public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

			/// <summary>
			///     Retrieves a history of up to 64 previous coordinates of the mouse or pen.
			/// </summary>
			/// <param name="cbSize">The size, in bytes, of the MOUSEMOVEPOINT structure.</param>
			/// <param name="pointsIn">
			///     A pointer to a MOUSEMOVEPOINT structure containing valid mouse coordinates (in screen coordinates).
			///     It may also contain a time stamp.
			///     The GetMouseMovePointsEx function searches for the point in the mouse coordinates history.
			///     If the function finds the point, it returns the last nBufPoints prior to and including the supplied point.
			///     If your application supplies a time stamp, the GetMouseMovePointsEx function will use it to differentiate between two equal points
			///     that were recorded at different times.
			///     An application should call this function using the mouse coordinates received from the WM_MOUSEMOVE message and convert them to
			///     screen coordinates.
			/// </param>
			/// <param name="pointsBufferOut">A pointer to a buffer that will receive the points. It should be at least cbSize* nBufPoints in size.</param>
			/// <param name="nBufPoints">The number of points to be retrieved.</param>
			/// <param name="resolution">
			///     The resolution desired. This parameter can be one of the following values.
			///     GMMP_...
			/// </param>
			/// <returns></returns>
			[DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto, SetLastError=true)]
			public static extern int GetMouseMovePointsEx(uint cbSize, [In] ref MOUSEMOVEPOINT pointsIn, [Out] MOUSEMOVEPOINT[] pointsBufferOut, int nBufPoints, uint resolution);

		}

		/// <summary>
		///     Represents an x- and y-coordinate pair in two-dimensional space.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{

			/// <summary>
			///     The x-coordinate of this Point.
			/// </summary>
			public int x;

			/// <summary>
			///     The y-coordinate of this Point.
			/// </summary>
			public int y;

			/// <summary>
			///     Initializes a new instance of the <see cref="POINT" /> struct.
			/// </summary>
			/// <param name="x">The x.</param>
			/// <param name="y">The y.</param>
			public POINT(int x, int y) {
				this.x=x;
				this.y=y;
			}

			public override bool Equals(object obj) {var b=(POINT)obj; return x.Equals(b.x) && y.Equals(b.y);}
			public override int GetHashCode() {throw new NotImplementedException();}
			public static bool operator == (POINT a, POINT b){return a.Equals(b);}
			public static bool operator != (POINT a, POINT b){return !a.Equals(b);}
		}

		/// <summary>
		///     The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack=0)]
		public struct RECT
		{

			/// <summary>
			///     The x-coordinate location of the left side of the rectangle.
			/// </summary>
			public int left;

			/// <summary>
			///     The y-coordinate location of the top side of the rectangle.
			/// </summary>
			public int top;

			/// <summary>
			///     Specifies the x-coordinate of the lower-right corner of a rectangle.
			/// </summary>
			public int right;

			/// <summary>
			///     Specifies the y-coordinate of the lower-right corner of a rectangle.
			/// </summary>
			public int bottom;

			/// <summary>
			///     Gets the width.
			/// </summary>
			public int Width => Math.Abs(right -left);

			/// <summary>
			///     Gets the height.
			/// </summary>
			public int Height => bottom -top;

			/// <summary>
			///     Initializes a new instance of the <see cref="RECT" /> struct.
			/// </summary>
			/// <param name="left">The left.</param>
			/// <param name="top">The top.</param>
			/// <param name="right">The right.</param>
			/// <param name="bottom">The bottom.</param>
			public RECT(int left, int top, int right, int bottom) {
				this.left=left;
				this.top=top;
				this.right=right;
				this.bottom=bottom;
			}


			public override bool Equals(object obj) {
				var b=(RECT)obj; 
				return 
					left.Equals(b.left) && 
					top.Equals(b.top)&& 
					right.Equals(b.right)&& 
					bottom.Equals(b.bottom);
			}
			public override int GetHashCode() {throw new NotImplementedException();}
			public static bool operator == (RECT a, RECT b){return a.Equals(b);}
			public static bool operator != (RECT a, RECT b){return !a.Equals(b);}
		}

		/// <summary>
		///     Container for no further categorized utility methods.
		/// </summary>
		public static class Util
		{

			/// <summary>
			///     Extracts x coordinate from the lParam (Message) value.
			/// </summary>
			public static int GetXlparam(IntPtr lParam) {
				return LoWord(lParam.ToInt32());
			}

			/// <summary>
			///     Extracts y coordinate from the lParam (Message) value.
			/// </summary>
			public static int GetYlparam(IntPtr lParam) {
				return HiWord(lParam.ToInt32());
			}

			/// <summary>
			///     Returns the hi 16-bit word of given 32-bit integer.
			/// </summary>
			public static int HiWord(int i) {
				return (short)(i>>16);
			}

			/// <summary>
			///     Returns the lo 16-bit word of given 32-bit integer.
			/// </summary>
			public static int LoWord(int i) {
				return (short)(i&0xFFFF);
			}

			/// <summary>
			///     Returns whether window identified by given handle is minimized.
			/// </summary>
			public static bool IsWindowMinimized(IntPtr hwnd) {
				var placement=new WINDOWPLACEMENT();

				return NativeFunctions.GetWindowPlacement(hwnd, ref placement)
					   &&(placement.showCmd==(uint)SW.SHOWMINIMIZED||placement.showCmd==(int)SW.MINIMIZE||placement.showCmd==(uint)SW.SHOWMINNOACTIVE||placement.showCmd==(uint)SW.FORCEMINIMIZE);
			}

			/// <summary>
			///     Returns the rectangle of the window identified by given handle.
			/// </summary>
			[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="GetWindowRect")]
			public static Rect GetWindowRect(IntPtr hwnd) {
				RECT rect;
				if(!NativeFunctions.GetWindowRect(hwnd, out rect))
					throw new InvalidOperationException("GetWindowRect Error: "+Marshal.GetLastWin32Error());

				return new Rect(rect.left, rect.top, rect.Width, rect.Height);
			}

			/// <summary>
			///     Converts given point from logical to device coordinates using desktop obtained by given window handle.
			/// </summary>
			public static Point LogicalToDevice(Point logicalPoint, IntPtr hwnd) {
				Graphics desktop=Graphics.FromHwnd(hwnd);

				return new Point(logicalPoint.X*(desktop.DpiX/96), logicalPoint.Y*(desktop.DpiY/96));
			}

			/// <summary>
			///     Converts given point from device to logical coordinates using desktop obtained by given window handle.
			/// </summary>
			public static Point DeviceToLogical(Point devicePoint, IntPtr hwnd) {
				Graphics desktop=Graphics.FromHwnd(hwnd);

				return new Point(devicePoint.X/(desktop.DpiX/96), devicePoint.Y/(desktop.DpiY/96));
			}

			/// <summary>
			///     Converts given rect from device to logical coordinates using desktop obtained by given window handle.
			/// </summary>
			public static Rect DeviceToLogical(Rect deviceRectangle, IntPtr hwnd) {
				Graphics desktop=Graphics.FromHwnd(hwnd);

				return new Rect(
					new Point(deviceRectangle.Left/(desktop.DpiX/96), deviceRectangle.Top/(desktop.DpiY/96)),
					new Point(deviceRectangle.Right/(desktop.DpiX/96), deviceRectangle.Bottom/(desktop.DpiY/96)));
			}

		}

		/// <summary>
		///     Contains information about the placement of a window on the screen.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPLACEMENT
		{

			/// <summary>
			///     The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member
			///     to sizeof(WINDOWPLACEMENT).
			///     GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
			/// </summary>
			public uint length;

			/// <summary>
			///     The flags that control the position of the minimized window and the method by which the window is restored. This member can be
			///     one or more of the following values.
			///     WPF_...
			/// </summary>
			public uint flags;

			/// <summary>
			///     The current show state of the window. This member can be one of the following values.
			///     SW_...
			/// </summary>
			public uint showCmd;

			/// <summary>
			///     The coordinates of the window's upper-left corner when the window is minimized.
			/// </summary>
			public POINT ptMinPosition;

			/// <summary>
			///     The coordinates of the window's upper-left corner when the window is maximized.
			/// </summary>
			public POINT ptMaxPosition;

			/// <summary>
			///     The window's coordinates when the window is in the restored position.
			/// </summary>
			public RECT rcNormalPosition;

			public override bool Equals(object obj) {
				var b=(WINDOWPLACEMENT)obj; 
				return 
					length.Equals(b.length) && 
					flags.Equals(b.flags)&& 
					showCmd.Equals(b.showCmd)&& 
					ptMinPosition.Equals(b.ptMinPosition)&& 
					ptMaxPosition.Equals(b.ptMaxPosition)&& 
					rcNormalPosition.Equals(b.rcNormalPosition);
			}
			public override int GetHashCode() {throw new NotImplementedException();}
			public static bool operator == (WINDOWPLACEMENT a, WINDOWPLACEMENT b){return a.Equals(b);}
			public static bool operator != (WINDOWPLACEMENT a, WINDOWPLACEMENT b){return !a.Equals(b);}
		}

		#endregion
	}


	//TODO: replace CommandBase+RelayCommand
	///<summary>
	/// Base class for commands.
	///</summary>
	[Obsolete("Obsolete. Don't use in new projects!")]
	public abstract class CommandBase {

		///<summary> Trigger refresh of can execute state by Command Manager.
		///</summary>
		public void Update() { OnCanExecuteChanged(); }

		///<summary> Trigger refresh of can execute state by Command Manager.
		///</summary>
		public abstract void OnCanExecuteChanged();
	}

	///<summary>
	/// Command implementation for use in viewmodels, invoking a delegate with or without parameter when the command is executed
	/// as well as another delegate with or without parameter when can execute is calculated.
	///</summary>
	[Obsolete("Obsolete. Don't use in new projects!")]
	public class RelayCommand : CommandBase, ICommand {

		#region Fields

		private readonly Action<object> _execute;
		private readonly Predicate<object> _canExecute;

		#endregion // Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="execute">The execute.</param>
		public RelayCommand(Action execute)
			: this(p => execute(), null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="execute">The execute.</param>
		public RelayCommand(Action<object> execute)
			: this(execute, null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="execute">The execute.</param>
		/// <param name="canExecute">The can execute.</param>
		public RelayCommand(Action execute, Func<bool> canExecute)
			: this(p => execute(), p => canExecute()) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="execute">The execute.</param>
		/// <param name="canExecute">The can execute.</param>
		public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
			if (execute == null) throw new ArgumentNullException(nameof(execute));

			_execute = execute;
			_canExecute = canExecute;

			// force a refresh of our command if suggested by command manager
			// why we need a member variable for the handler, ee static weak events: http://www.codeproject.com/Articles/29922/Weak-Events-in-C#heading0011
			_onCanExecuteChangedHandler = (s, e) => OnCanExecuteChanged();
			CommandManager.RequerySuggested += _onCanExecuteChangedHandler;
		}

		#endregion // Constructors

		#region ICommand Members

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		[DebuggerStepThrough]
		public virtual bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Required to hold a strong reference to the delegate added to CommandManager.RequerySuggested.
		/// </summary>
		private readonly EventHandler _onCanExecuteChangedHandler;

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		public virtual void Execute(object parameter)
		{
			_execute(parameter);
		}

		#endregion // ICommand Members

		/// <summary> Should be called to force a refresh of the commands <see cref="ICommand.CanExecute"/> method.
		/// </summary>
		public override void OnCanExecuteChanged() {
			if (Application.Current == null) {
				Debug.Assert(Application.Current != null, "Application Dispatcher not initialized. Command call not possible.");
				return;
				//m1jpa: Crashes fxp when initializing equipment
				//throw new Exception("Application Dispatcher not initialized. Command call not possible.");
			}

			var f = CanExecuteChanged;
			if (f != null) ApplicationDispatcher.BeginInvoke(() => f(this, EventArgs.Empty));
		}
	}
}