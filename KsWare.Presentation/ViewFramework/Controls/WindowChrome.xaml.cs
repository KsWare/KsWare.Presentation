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

		private Window m_Window;
		private decimal m_LastF11Timestamp;

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

		WindowChromeBehavior m_WindowChromeBehavior;

		const Key FullscreenHotkey = Key.F11;

		public static readonly DependencyProperty IsFullscreenProperty =
			DependencyProperty.Register("IsFullscreen", typeof (bool), typeof (WindowChrome), new PropertyMetadata(false, (o, e) => ((WindowChrome)o).AtIsFullscreenChanged(o, e)));

		private void AtIsFullscreenChanged(object sender, DependencyPropertyChangedEventArgs e) {
			m_WindowChromeBehavior.IsFullscreen = (bool)e.NewValue;
		}

		public bool IsFullscreen {
			get { return (bool) GetValue(IsFullscreenProperty); }
			set { SetValue(IsFullscreenProperty, value); }
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

			m_Window = Window;
			if(!DesignerProperties.GetIsInDesignMode(this) && m_Window!=null) {
				m_WindowChromeBehavior = new WindowChromeBehavior {
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
				m_WindowChromeBehavior.IsFullscreenChanged+= (s,e) => IsFullscreen = m_WindowChromeBehavior.IsFullscreen;
				Interaction.GetBehaviors(Window).Add(m_WindowChromeBehavior);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private Window Window {
			get {
				if (DesignerProperties.GetIsInDesignMode(this)) return null;
				if(m_Window!=null) return m_Window;

				DependencyObject e = this;
				while (e!=null) {
					if(e is Window) {
						m_Window =  (Window)e;
						return m_Window;
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
