namespace KsWare.Presentation.ViewModelFramework {

	public class UIElementVM<T> : ObjectVM {

		public UIElementVM() {
			RegisterChildren(()=>this);
		}
	}
}