namespace KsWare.Presentation.ViewModelFramework {

	public interface ISelectorVM<T>:IObjectVM {
		void Select(T item);
	}

}