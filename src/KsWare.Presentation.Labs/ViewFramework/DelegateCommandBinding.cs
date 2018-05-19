using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace KsWare.Presentation.ViewFramework {

	/// <summary>
	/// Provides redirecting routed commands.
	/// </summary>
	/// <seealso cref="System.Windows.Input.CommandBinding" />
	/// <example>
	/// Binds the routed command <c>ApplicationCommands.Save</c> to  delegate command <c>SaveAction</c> in data context of root element.
	/// <code language="XAML">
	/// &lt;UserControl.CommandBindings&gt;
	///     &lt;shared:DelegateCommandBinding 
	///         Command="{x:Static ApplicationCommands.Save}"
	///         DelegateCommand="SaveAction" 
	///         Element="{ksv:RootElement}"
	///     /&gt;
	/// &lt;/UserControl.CommandBindings&gt;
	/// </code>
	/// </example>
	/// <remarks>Assigning of events is not necessary. If you want events use <see cref="CommandBinding"/>.</remarks>
	public class DelegateCommandBinding : CommandBinding {

		private static readonly Dictionary<string, PropertyInfo> ReflectedProperties = new Dictionary<string, PropertyInfo>();

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateCommandBinding"/> class.
		/// </summary>
		public DelegateCommandBinding() {
			CanExecute += AtCanExecute;
			Executed   += AtExecuted;
		}

		/// <summary>
		/// Gets or sets the delegate command.
		/// </summary>
		/// <value>The delegate command.</value>
		public string DelegateCommand { get; set; }

		/// <summary>
		/// Gets or sets the element [optional].
		/// </summary>
		/// <value>The element.</value>
		/// <remarks>If not specified the data context is the data context of the command sender.</remarks>
		public FrameworkElement Element { get; set; }

		private void AtExecuted(object sender, ExecutedRoutedEventArgs e) {
			var command = GetDelegateCommand(e.OriginalSource);
			command?.Execute(e.Parameter);
		}

		private void AtCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			var command = GetDelegateCommand(e.OriginalSource);
			e.CanExecute = command != null && command.CanExecute(e.Parameter);
		}

		private ICommand GetDelegateCommand(object source) {
			var dataContext = Element?.DataContext ?? (source as FrameworkElement)?.DataContext;
			if (dataContext == null) {
				return null;
			}

			try {
				var          type = dataContext.GetType();
				var          key  = type.FullName + "." + DelegateCommand;
				PropertyInfo property;
				if (!ReflectedProperties.TryGetValue(key, out property)) {
					var flags = BindingFlags.Instance | BindingFlags.Public;
					property = type.GetProperty(DelegateCommand, flags);
					if (property != null) {
						ReflectedProperties.Add(key, property);
					}
				}

				if (property == null) {
					throw new InvalidOperationException(
						$"Property for DelegateCommand not found! Type={type.FullName}, Property={DelegateCommand}");
				}

				var command = (ICommand) property.GetValue(dataContext);
				return command;
			}
			catch {
				throw;
			}
		}
	}

}