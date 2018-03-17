namespace KsWare.Presentation.ViewModelFramework {

	public abstract class UserFeedbackVM<T> : DataVM<T> where T:UserFeedbackEventArgs {

		protected UserFeedbackVM() {
			RegisterChildren(()=>this);
			CloseAction.MːDoAction = DoClose;
		}

//		protected virtual void DoClose() {
//			var parent = TreeHelper.FindAnchor<OverlayCollection,WindowVM>(this);
//			if (parent is OverlayCollection) {
//				((OverlayCollection) parent).Remove(this);
//			}else if (parent is WindowVM) {
//				((WindowVM) parent).Close();
//			}
//			IsOpen = false;
//		}
		protected virtual void DoClose() {
			var parent = TreeHelper.FindAnchor<OverlayCollection>(this);
			if (parent is OverlayCollection) {
				((OverlayCollection) parent).Remove(this);
			}
			IsOpen = false;
		}

		public ActionVM CloseAction { get; private set; }
		public bool IsOpen { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
	}
}