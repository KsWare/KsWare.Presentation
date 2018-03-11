namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderDateTimeExtension:IEditValueProviderExtension {

	}

	partial class EditValueProvider {

		class DateTimeExtension : Extension, IEditValueProviderDateTimeExtension {

			public DateTimeExtension(EditValueProvider provider) : base(provider) {}

			internal void UpdateValue(bool raiseEvents) {  }

		}

	}
}
