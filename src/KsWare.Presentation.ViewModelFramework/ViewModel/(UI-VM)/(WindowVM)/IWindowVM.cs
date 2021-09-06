namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> IWindowVM
	/// </summary>
	/// <see cref="System.Windows.Window"/>
	public interface IWindowVM {

		ActionVM CloseAction { get; }

		void Show();

//		void Close();

		/// <summary>
		/// Gets or sets the <see cref="IWindowVM">window</see> that owns this <see cref="IWindowVM"/>.
		/// </summary>
		/// <value>A <see cref="IWindowVM"/> object that represents the owner of this <see cref="IWindowVM"/>.</value>
		/// <seealso cref="System.Windows.Window.Owner"/>
		IWindowVM Owner { get; set; }
	}

}