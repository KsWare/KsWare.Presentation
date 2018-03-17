using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KsWare.Presentation.ViewFramework {

	/// <summary> Binding with pefix and suffix.
	/// </summary>
	/// <remarks> Note: This is a <see cref="MultiBinding"/>! 
	/// Because this <see cref="BindingWithPrefixAndSuffix"/> mimes a simple <see cref="Binding"/> the behavoir of derived properties could be other as expected. 
	/// The results of all internal preconfigured bindings are joined as strings. 
	/// The <see cref="StringFormat"/> property specifies the format of the main binding (specified by <see cref="Path"/>). 
	/// The <see cref="Prefix"/> and <see cref="Suffix"/> properties are constant strings.
	/// If you want dynamic (resp. bounded) prefix/suffix you should use <see cref="MultiBinding"/> and optional the <see cref="StringJoinMultiValueConverter"/> directly.
	/// </remarks>
	[MarkupExtensionReturnType(typeof (string))]
	public class BindingWithPrefixAndSuffix:BindingUsesMultiBinding {

		private Binding _binding1;
		private Binding _binding3;
		private StringJoinMultiValueConverter _converter;

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class 
		/// </summary>
		public BindingWithPrefixAndSuffix() {
			Converter = _converter=StringJoinMultiValueConverter.Default;
			Bindings.Add(_binding1 =new Binding());
			Bindings.Add(MainBinding=new Binding());
			Bindings.Add(_binding3 =new Binding());
		}

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class with an initial path and a prefix/suffix.
		/// </summary>
		/// <param name="prefix">The prefix.</param>
		/// <param name="path">The initial System.Windows.Data.Binding.Path for the binding.</param>
		/// <param name="suffix">The suffix.</param>
		public BindingWithPrefixAndSuffix(string prefix,string path,string suffix) {
			Converter = _converter=new StringJoinMultiValueConverter();
			Bindings.Add(_binding1 =new Binding{Source = prefix});
			Bindings.Add(MainBinding=new Binding(path));
			Bindings.Add(_binding3 =new Binding{Source = suffix});
		}
		

		public string Prefix { get => (string) _binding1.Source; set => _binding1.Source = value; }

		/// <summary> Gets or sets the path to the binding source property.
		/// </summary>
		/// <value>The path to the binding source. The default is null.</value>
		public PropertyPath Path { get => MainBinding.Path; set => MainBinding.Path = value; }

		public string Suffix { get => (string) _binding3.Source; set => _binding3.Source = value; }

		/// <summary>  Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.
		/// </summary> 
		/// <returns>
		/// A string that specifies how to format the binding if it displays the bound value as a string.
		/// </returns>
		[DefaultValue(null)]
		public new string StringFormat { get => _converter.StringFormat[1]; set => _converter.StringFormat[1] = value; }
	}

	/// <summary> Binding with prefix.
	/// </summary>
	/// <remarks>For a detailed description see <see cref="BindingWithPrefixAndSuffix"/></remarks>
	[MarkupExtensionReturnType(typeof (string))]
	public class BindingWithPrefix : BindingWithPrefixAndSuffix {

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class with an initial path and a prefix.
		/// </summary>
		/// <param name="prefix">The prefix.</param>
		/// <param name="path">The initial System.Windows.Data.Binding.Path for the binding.</param>
		public BindingWithPrefix(string prefix,string path):base(prefix,path,null) {}
	}

	/// <summary> Binding with suffix.
	/// </summary>
	/// <remarks>For a detailed description see <see cref="BindingWithPrefixAndSuffix"/></remarks>
	[MarkupExtensionReturnType(typeof (string))]
	public class BindingWithSuffix : BindingWithPrefixAndSuffix {

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class with an initial path and a suffix.
		/// </summary>
		/// <param name="path">The initial System.Windows.Data.Binding.Path for the binding.</param>
		/// <param name="suffix">The suffix.</param>
		public BindingWithSuffix(string path,string suffix):base(null,path,suffix) {}
	}

}