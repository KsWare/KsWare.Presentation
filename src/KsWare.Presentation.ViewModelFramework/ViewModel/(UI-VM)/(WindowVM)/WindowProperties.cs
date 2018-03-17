using System.Windows;

namespace KsWare.Presentation.ViewModelFramework {

	public class WindowProperties {
		public ResizeMode ResizeMode { get; set; }
		public WindowState WindowState { get; set; }
		public WindowStyle WindowStyle { get; set; }
		public bool IsFullScreen { get; set; }

		public void RestoreFromFullScreen(Window window, WindowState newState) {
			window.ResizeMode  = ResizeMode;
			window.WindowStyle = WindowStyle;
			window.WindowState = newState;
		}

		internal void RestoreFromFullScreen(Window window) {
			window.ResizeMode  = ResizeMode;
			window.WindowStyle = WindowStyle;
			window.WindowState = WindowState;
		}

		public static WindowProperties PrepareFullScreenRestore(Window window) {
			return new WindowProperties {
				IsFullScreen =false,
				ResizeMode   =window.ResizeMode,
				WindowStyle  =window.WindowStyle,
				WindowState  =window.WindowState
			};
		}

	}

}