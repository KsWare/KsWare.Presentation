using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using KsWare.Presentation.ViewFramework.Behaviors;

#pragma warning disable 1591

namespace KsWare.Presentation.ViewFramework.Controls {

	/// <summary> Interaction logic for WindowChrome.xaml
	/// </summary>
	[TemplatePart(Name="PART_TitleBar"        ,Type=typeof(Panel ))]
	[TemplatePart(Name="PART_Border"          ,Type=typeof(Border))]
	[TemplatePart(Name="PART_WindowButtons"   ,Type=typeof(WindowChromeButtons))]
	[TemplatePart(Name="PART_ExitFullscreen"  ,Type=typeof(Button))]
	public partial class WindowChrome:ContentControl {

		const Key FullscreenHotkey = Key.F11;

		private Window _window;
		WindowChromeBehavior _windowChromeBehavior;

		// ReSharper disable InconsistentNaming
		private Panel PART_TitleBar;
		private WindowChromeButtons PART_WindowButtons;
		private Button PART_Minimize;
		private Button PART_Restore;
		private Button PART_Maximize;
		private Button PART_Fullscreen;
		private Button PART_Close;
		private Border PART_Border;
		private Button PART_ExitFullscreen;
		// ReSharper restore InconsistentNaming

		public static readonly DependencyProperty IsFullscreenProperty =
			DependencyProperty.Register("IsFullscreen", typeof (bool), typeof (WindowChrome), new PropertyMetadata(false, (o, e) => ((WindowChrome)o).AtIsFullscreenChanged(o, e)));

		private void AtIsFullscreenChanged(object sender, DependencyPropertyChangedEventArgs e) {
			_windowChromeBehavior.IsFullscreen = (bool)e.NewValue;
		}

		public bool IsFullscreen {
			get => (bool) GetValue(IsFullscreenProperty);
			set => SetValue(IsFullscreenProperty, value);
		}

		public WindowChrome() {
			
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			
			PART_WindowButtons=(WindowChromeButtons)Template.FindName("PART_WindowButtons", this);
			PART_Border         = (Border)Template.FindName("PART_Border", this);
			PART_TitleBar       = (Panel )Template.FindName("PART_TitleBar", this);
			PART_Minimize       = PART_WindowButtons.PART_Minimize;
			PART_Restore        = PART_WindowButtons.PART_Restore;
			PART_Maximize       = PART_WindowButtons.PART_Maximize;
			PART_Fullscreen     = PART_WindowButtons.PART_Fullscreen;
			PART_Close          = PART_WindowButtons.PART_Close;
			PART_ExitFullscreen = (Button)Template.FindName("PART_ExitFullscreen", this);

			_window = Window;
			if(!DesignerProperties.GetIsInDesignMode(this) && _window!=null) {
				_windowChromeBehavior = new WindowChromeBehavior {
					ResizeBorderWidth    = 3, 
					TitleBar             = PART_TitleBar, 
					Border               = PART_Border,
					CloseButton          = PART_Close, 
					MaximizeButton       = PART_Maximize, 
					MinimizeButton       = PART_Minimize, 
					RestoreButton        = PART_Restore,
					FullScreenButton     = PART_Fullscreen,
					ExitFullScreenButton = PART_ExitFullscreen,
					VirtualCaptionHeight = 50
				};
				_windowChromeBehavior.IsFullscreenChanged+= (s,e) => IsFullscreen = _windowChromeBehavior.IsFullscreen;
				Interaction.GetBehaviors(Window).Add(_windowChromeBehavior);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private Window Window {
			get {
				if (DesignerProperties.GetIsInDesignMode(this)) return null;
				if(_window!=null) return _window;

				DependencyObject e = this;
				while (e!=null) {
					if(e is Window) {
						_window =  (Window)e;
						return _window;
					}
					e=VisualTreeHelper.GetParent(e);
				}
				return null;
			}
		}

		static WindowChrome() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof (WindowChrome), new FrameworkPropertyMetadata(typeof (WindowChrome)));
		}

	}
}
