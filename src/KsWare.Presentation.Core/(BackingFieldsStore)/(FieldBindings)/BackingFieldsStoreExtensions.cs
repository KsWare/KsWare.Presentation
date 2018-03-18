namespace KsWare.Presentation {

	public static class BackingFieldsStoreExtensions {

		public static void SetBinding(this BackingFieldsStore.IBackingFieldInfo target, FieldBinding binding) {
			FieldBindingOperations.SetBinding(target, binding);
		}

	}

}
