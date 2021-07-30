using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using KsWare.Presentation.Converters;

namespace KsWare.Presentation.ViewFramework {

	/// <summary> Provides a <see cref="Binding"/> command buttons visibility
	/// </summary>
	/// <remarks>
	/// Identically functionality as <see cref="Binding"/> with additional
	/// <see cref="Binding.ValidatesOnDataErrors"/>, 
	/// <see cref="Binding.ValidatesOnExceptions"/> and
	/// <see cref="Binding.UpdateSourceTrigger"/>
	/// set to <c>true</c>.
	/// <p/>
	/// </remarks>
	/// <example>
	/// <code>
	/// &lt;TextBox 
	///     Style="{StaticResource ValidatingTextBox}" 
	/// 	Text="{ext:BindingWithValidation Path=MyBindingPath}" &gt;
	/// &lt;/TextBox&gt;
	/// </code>
	/// </example>
	[MarkupExtensionReturnType(typeof (Visibility))]
	public class CommandVisibilityBinding:Binding {

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class 
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>..
		/// </summary>
		[Obsolete("Not yet implemented",true)]
		public CommandVisibilityBinding() {
			Mode = BindingMode.OneWay;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandVisibilityBinding"/> class.
		/// </summary>
		/// <param name="expression">One of <see cref="VisibilityConverter.Expression" /></param>
		/// <example><code>&lt;Button Command="..." Visibility="{ksv:CommandVisibilityBinding TrueVisibleElseHidden}"/></code>
		/// </example>
		public CommandVisibilityBinding(VisibilityConverter.Expression expression) : base("IsEnabled") {
			RelativeSource = new RelativeSource(RelativeSourceMode.Self);
			Mode           = BindingMode.OneWay;
			Converter      = VisibilityConverter.Get(expression);
		}
	}

}