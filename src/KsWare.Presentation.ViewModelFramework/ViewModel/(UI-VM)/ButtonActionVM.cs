using System.Windows.Controls;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary>
	/// Provides a <see cref="ActionVM"/> with common properties from <see cref="Button"/>.
	/// </summary>
	/// <seealso cref="KsWare.Presentation.ViewModelFramework.ActionVM" />
	public class ButtonActionVM : ActionVM {

		/// <inheritdoc cref="ContentControl.Content"/>
		/// <seealso cref="ContentControl.Content"/>
		public object Content { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		/// <inheritdoc cref="System.Windows.FrameworkElement.ToolTip"/>
		/// <seealso cref="System.Windows.FrameworkElement.ToolTip"/>
		public object ToolTip { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		/// <inheritdoc cref="Button.IsDefault"/>
		/// <seealso cref="Button.IsDefault"/>
		public bool IsDefault { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
		
		/// <inheritdoc cref="Button.IsCancel"/>
		/// <seealso cref="Button.IsCancel"/>
		public bool IsCancel { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
	}
}
