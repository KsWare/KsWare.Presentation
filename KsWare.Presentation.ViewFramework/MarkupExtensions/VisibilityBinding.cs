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
			ConverterParameter = new VisibilityConverterParameter();
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
		/// <param name="compareValue"> </param>
		public VisibilityBinding(string path, VisibilityConverter.Expression expression, object compareValue) : base(path) {
			Mode=System.Windows.Data.BindingMode.OneWay;
			Converter = VisibilityConverter.Get(expression);
			ConverterParameter = new VisibilityConverterParameter {CompareValue = compareValue};
		}

//		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
//		/// with additional
//		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
//		/// <see cref="Binding.ValidatesOnExceptions"/> and
//		/// <see cref="Binding.UpdateSourceTrigger"/>
//		/// set to <c>true</c>.
//		/// </summary>
//		/// <param name="path">The initial <see cref="System.Windows.Data.Binding.Path" /> for the binding.</param>
//		/// <param name="expression"> </param>
//		/// <param name="designtimeVisibility"> </param>
//		public VisibilityBinding(string path, VisibilityConverter.Expression expression, Visibility designtimeVisibility) : base(path) {
//			Mode=System.Windows.Data.BindingMode.OneWay;
//			Converter = VisibilityConverter.Get(expression);
//			ConverterParameter = new VisibilityConverterParameter {DesigntimeVisibility = designtimeVisibility};
//		}

//		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
//		/// with additional
//		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
//		/// <see cref="Binding.ValidatesOnExceptions"/> and
//		/// <see cref="Binding.UpdateSourceTrigger"/>
//		/// set to <c>true</c>.
//		/// </summary>
//		/// <param name="path">The initial <see cref="System.Windows.Data.Binding.Path" /> for the binding.</param>
//		/// <param name="expression"> </param>
//		/// <param name="designtimeVisibility"> </param>
//		/// <param name="compareValue"></param>
//		public VisibilityBinding(string path, VisibilityConverter.Expression expression, Visibility designtimeVisibility, object compareValue) : base(path) {
//			Mode=System.Windows.Data.BindingMode.OneWay;
//			Converter = VisibilityConverter.Get(expression);
//			ConverterParameter = _parameter = new VisibilityConverterParameter {DesigntimeVisibility = designtimeVisibility,CompareValue = compareValue};
//		}

		public Visibility? DesigntimeVisibility {
			set {
				var parameter = ConverterParameter as VisibilityConverterParameter;
				if (parameter == null) {
					ConverterParameter = parameter=new VisibilityConverterParameter();
				}
				parameter.DesigntimeVisibility = value;
			}
			get {
				var parameter = ConverterParameter as VisibilityConverterParameter;
				return parameter!=null ? parameter.DesigntimeVisibility : null;
			}
		}

		public object CompareValue {
			set {
				var parameter = ConverterParameter as VisibilityConverterParameter;
				if (parameter == null) {
					ConverterParameter = parameter=new VisibilityConverterParameter();
				}
				parameter.CompareValue = value;
			}
			get {
				var parameter = ConverterParameter as VisibilityConverterParameter;
				return parameter!=null ? parameter.CompareValue : null;
			}
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
