using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KsWare.Presentation.ViewFramework.Behaviors;
using Microsoft.Xaml.Behaviors;

#pragma warning disable 1591

namespace KsWare.Presentation.ViewFramework.Controls
{
	/// <summary> Interaction logic for WindowChrome.xaml
	/// </summary>
	[TemplatePart(Name="PART_TitleBar"      ,Type=typeof(Panel ))]
	[TemplatePart(Name="PART_Border"        ,Type=typeof(Border))]
	[TemplatePart(Name="PART_Panel"         ,Type=typeof(Panel ))]
	[TemplatePart(Name="PART_Minimize"      ,Type=typeof(Button))]
	[TemplatePart(Name="PART_Restore"       ,Type=typeof(Button))]
	[TemplatePart(Name="PART_Maximize"      ,Type=typeof(Button))]
	[TemplatePart(Name="PART_Fullscreen"    ,Type=typeof(Button))]
	[TemplatePart(Name="PART_Close"         ,Type=typeof(Button))]
	[TemplatePart(Name="PART_ExitFullscreen",Type=typeof(Button))]
	public partial class WindowChrome2
	{
		private Window _window;

		// ReSharper disable InconsistentNaming
		private Panel PART_TitleBar;
		private Button PART_Minimize;
		private Button PART_Restore;
		private Button PART_Maximize;
		private Button PART_Fullscreen;
		private Button PART_Close;
//		private Border PART_Border;
//		private Panel PART_Panel;
		private Button PART_ExitFullscreen;
		// ReSharper restore InconsistentNaming

		WindowChromeBehavior _WindowChromeBehavior;

		public WindowChrome2() {
			InitializeComponent();
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			
//			PART_Border         = (Border)Template.FindName("PART_Border"        , this);
//			PART_Panel          = (Panel )Template.FindName("PART_Panel"         , this);
			PART_TitleBar       = (Panel )Template.FindName("PART_TitleBar"      , this);
			PART_Minimize       = (Button)Template.FindName("PART_Minimize"      , this);
			PART_Restore        = (Button)Template.FindName("PART_Restore"       , this);
			PART_Maximize       = (Button)Template.FindName("PART_Maximize"      , this);
			PART_Fullscreen     = (Button)Template.FindName("PART_Fullscreen"    , this);
			PART_Close          = (Button)Template.FindName("PART_Close"         , this);
			PART_ExitFullscreen = (Button)Template.FindName("PART_ExitFullscreen", this);

			_window = Window;
			if(!DesignerProperties.GetIsInDesignMode(this) && _window!=null) {
				_WindowChromeBehavior = new WindowChromeBehavior {
					ResizeBorderWidth    = 3, 
					TitleBar             = PART_TitleBar, 
//					Border               = PART_Border,
					CloseButton          = PART_Close, 
					MaximizeButton       = PART_Maximize, 
					MinimizeButton       = PART_Minimize, 
					RestoreButton        = PART_Restore,
					FullScreenButton     = PART_Fullscreen,
					ExitFullScreenButton = PART_ExitFullscreen,
					NoCollapseTitleBar=false,
					VirtualCaptionHeight = 50
				};
				Interaction.GetBehaviors(Window).Add(_WindowChromeBehavior);
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
	}
}
