using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary>
	/// A view model for a <see cref="MenuItem"/>
	/// </summary>
	/// <example>
	/// Add merged resource dictionary
	/// <code> &lt;ResourceDictionary Source="/KsWare.Presentation.ViewFramework;component/Resources/ViewModelStyles/MenuItemVMStyle.xaml"/></code>
	/// Add style
	/// <code>&lt;Menu ItemContainerStyle="{DynamicResource MenuItemVMStyle}"/>
	/// </code>
	/// </example>
	public class MenuItemVM : UIElementVM<MenuItem> {

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuItemVM"/> class.
		/// </summary>
		public MenuItemVM() {
			RegisterChildren(() => this);
			Command = CommandAction;
			//Data.Command
			//Data.Icon;
			//Data.Header;
			//Data.IsCheckable;
			//Data.IsChecked;
		}

		/// <summary>Initializes a new instance of the <see cref="MenuItemVM" /> class with a custom <see cref="ActionVM"/>.</summary>
		/// <param name="commandAction">The command action.</param>
		public MenuItemVM(ActionVM commandAction) {
			CommandAction = commandAction;
			RegisterChildren(() => this);
			Command = CommandAction;
		}

		/// <inheritdoc cref="MenuItem.Header"/>
		public string Caption { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		/// <inheritdoc cref="MenuItem.Command"/>
		/// <summary>
		/// Gets the <see cref="ActionVM"/>.
		/// </summary>
		/// <seealso cref="DoCommand"/>
		public ActionVM CommandAction { get; [UsedImplicitly] private set; }

		/// <inheritdoc cref="MenuItem.Command"/>
		/// <summary>
		///	Gets or sets the <see cref="ICommand"/> for the MenuItem.
		/// </summary>
		/// <remarks>
		/// Default value is <see cref="CommandAction"/>.
		/// </remarks>
		public ICommand Command { get => Fields.GetValue<ICommand>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Gets or sets the command-specific data for a particular command.
		/// </summary>
		public object CommandParameter { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		/// <inheritdoc cref="MenuItem.IsChecked"/>
		public bool IsChecked { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <inheritdoc cref="MenuItem.IsCheckable"/>
		public bool IsCheckable { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="CommandAction"/>
		/// </summary>
		[UsedImplicitly]
		protected virtual void DoCommand(object parameter) {
			
		}

		/// <inheritdoc cref="MenuItem.Items"/>
		public ListVM<MenuItemVM> Items { get; [UsedImplicitly] private set; }

	}

}
