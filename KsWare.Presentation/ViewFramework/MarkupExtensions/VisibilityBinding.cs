using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KsWare.Presentation.ViewFramework {
	
	/// <summary> Provides a <see cref="Binding"/> for visibility
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
	public class VisibilityBinding:Binding {

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class 
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>..
		/// </summary>
		[Obsolete("Not yet implemented",true)]
		public VisibilityBinding() {
			Mode=System.Windows.Data.BindingMode.OneWay;
		}

		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>.
		/// </summary>
		/// <param name="path">The initial <see cref="System.Windows.Data.Binding.Path" /> for the binding.</param>
		/// <param name="expression"> </param>
		public VisibilityBinding(string path, VisibilityConverter.Expression expression) : base(path) {
			Mode=System.Windows.Data.BindingMode.OneWay;
			Converter = VisibilityConverter.Get(expression);
		}

		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>.
		/// </summary>
		/// <param name="path">The initial <see cref="System.Windows.Data.Binding.Path" /> for the binding.</param>
		/// <param name="expression"> </param>
		/// <param name="parameter"> </param>
		public VisibilityBinding(string path, VisibilityConverter.Expression expression, object parameter) : base(path) {
			Mode=System.Windows.Data.BindingMode.OneWay;
			Converter = VisibilityConverter.Get(expression);
			ConverterParameter = parameter;
		}
	}


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
			Mode=System.Windows.Data.BindingMode.OneWay;
		}

		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>.
		/// </summary>
		/// <param name="path">The initial <see cref="System.Windows.Data.Binding.Path" /> for the binding.</param>
		public CommandVisibilityBinding(VisibilityConverter.Expression expression) : base("IsEnabled") {
			RelativeSource=new RelativeSource(RelativeSourceMode.Self);
			Mode=System.Windows.Data.BindingMode.OneWay;
			Converter = VisibilityConverter.Get(expression);
		}
	}

}
