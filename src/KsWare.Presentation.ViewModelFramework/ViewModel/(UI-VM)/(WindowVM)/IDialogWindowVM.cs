﻿namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> [DRAFT] IDialogWindowVM
	/// </summary>
	/// <seealso cref="System.Windows.Window"/>
	public interface IDialogWindowVM:IWindowVM {

//		ActionVM HelpAction { get; } //???

		/// <summary>
		/// Gets or sets the dialog result value, which is the value that is returned from the System.Windows.Window.ShowDialog method.
		/// </summary>
		/// <value>A System.Nullable value of type System.Boolean. The default is false. </value>
		bool? DialogResult { get; set; }

	}

}