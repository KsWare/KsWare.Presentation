using System.Windows.Data;
using System.Windows.Markup;

namespace KsWare.Presentation.ViewFramework {
	
	/// <summary> Provides a <see cref="Binding"/> for validation
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
	[MarkupExtensionReturnType(typeof (object))]
	public class BindingWithValidation:Binding {

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class 
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>..
		/// </summary>
		public BindingWithValidation() {
			ValidatesOnDataErrors = true;
			ValidatesOnExceptions = true;
			UpdateSourceTrigger   = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
		}

		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>.
		/// </summary>
		/// <param name="path">The initial <see cref="System.Windows.Data.Binding.Path" /> for the binding.</param>
		public BindingWithValidation(string path) : base(path) {
			ValidatesOnDataErrors = true;
			ValidatesOnExceptions = true;
			UpdateSourceTrigger   = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
		}
	}

}
