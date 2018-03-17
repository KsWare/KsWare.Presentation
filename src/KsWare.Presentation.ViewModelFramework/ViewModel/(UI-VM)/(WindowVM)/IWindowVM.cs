namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> IWindowVM
	/// </summary>
	/// <see cref="System.Windows.Window"/>
	public interface IWindowVM {

		ActionVM CloseAction { get; }

		void Show();

//		void Close();
	}

}