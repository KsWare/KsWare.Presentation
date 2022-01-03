using System.Windows.Data;
using System.Windows.Markup;

namespace KsWare.Presentation.ViewFramework.GlobalNamespace {

	public class Bind: Binding {

		public static BindingMode DefaultMode { get; set; } = BindingMode.Default;
		public static bool DefaultNotifyOnValidationError { get; set; } = false;
		public static bool DefaultNotifyOnSourceUpdated { get; set; } = false;
		public static bool DefaultNotifyOnTargetUpdated { get; set; } = false;

		public Bind() {
			Mode = DefaultMode;
			NotifyOnValidationError = DefaultNotifyOnValidationError;
			NotifyOnSourceUpdated = DefaultNotifyOnSourceUpdated;
			NotifyOnTargetUpdated = DefaultNotifyOnTargetUpdated;
		}

		/// <inheritdoc />
		public Bind(BindingMode mode) {
			Mode = mode;
			NotifyOnValidationError = DefaultNotifyOnValidationError;
			NotifyOnSourceUpdated = DefaultNotifyOnSourceUpdated;
			NotifyOnTargetUpdated = DefaultNotifyOnTargetUpdated;
		}

		/// <inheritdoc />
		public Bind(BindingMode mode, string path) : base(path) {
			Mode = mode;
			NotifyOnValidationError = DefaultNotifyOnValidationError;
			NotifyOnSourceUpdated = DefaultNotifyOnSourceUpdated;
			NotifyOnTargetUpdated = DefaultNotifyOnTargetUpdated;
		}
	}

}
