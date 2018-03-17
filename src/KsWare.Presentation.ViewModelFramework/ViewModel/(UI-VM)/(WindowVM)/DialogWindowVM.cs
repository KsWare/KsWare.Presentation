using System.Windows;

namespace KsWare.Presentation.ViewModelFramework {

	public class DialogWindowVM : WindowVM,IDialogWindowVM {

		/// <summary> Opens a window and returns only when the newly opened window is closed.
		/// </summary>
		/// <returns>A Nullable{T} value of type Boolean that specifies whether the activity was accepted (true) or canceled (false). The return value is the value of the DialogResult property before a window closes.</returns>
		/// <seealso cref="Window.ShowDialog"/>
		public bool? ShowDialog() {
			if (!UIAccess.HasWindow) return ApplicationVM.Current.WindowsInternal.ShowDialog(this);
			else return UIAccess.Window.ShowDialog();
		}

		/// <summary>
		/// Gets or sets the dialog result value, which is the value that is returned from the <see cref="ShowDialog"/> method.
		/// </summary>
		/// <value>A System.Nullable value of type System.Boolean. The default is false.</value>
		public bool? DialogResult { get => UIAccess.Window.DialogResult; set => UIAccess.Window.DialogResult = value; }

	}

}