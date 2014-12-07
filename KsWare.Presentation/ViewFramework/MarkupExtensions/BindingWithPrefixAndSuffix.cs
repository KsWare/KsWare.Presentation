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

		private Binding m_Binding1;
		private Binding m_Binding3;
		private StringJoinMultiValueConverter m_Converter;

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class 
		/// </summary>
		public BindingWithPrefixAndSuffix() {
			Converter = m_Converter=StringJoinMultiValueConverter.Default;
			Bindings.Add(m_Binding1 =new Binding());
			Bindings.Add(MainBinding=new Binding());
			Bindings.Add(m_Binding3 =new Binding());
		}

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class with an initial path and a prefix/suffix.
		/// </summary>
		/// <param name="prefix">The prefix.</param>
		/// <param name="path">The initial System.Windows.Data.Binding.Path for the binding.</param>
		/// <param name="suffix">The suffix.</param>
		public BindingWithPrefixAndSuffix(string prefix,string path,string suffix) {
			Converter = m_Converter=new StringJoinMultiValueConverter();
			Bindings.Add(m_Binding1 =new Binding{Source = prefix});
			Bindings.Add(MainBinding=new Binding(path));
			Bindings.Add(m_Binding3 =new Binding{Source = suffix});
		}
		

		public string Prefix { get { return (string) m_Binding1.Source; } set { m_Binding1.Source = value; }}

		/// <summary> Gets or sets the path to the binding source property.
		/// </summary>
		/// <value>The path to the binding source. The default is null.</value>
		public PropertyPath Path { get { return MainBinding.Path; } set { MainBinding.Path = value; }}

		public string Suffix { get { return (string) m_Binding3.Source; } set { m_Binding3.Source = value; }}

		/// <summary>  Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.
		/// </summary> 
		/// <returns>
		/// A string that specifies how to format the binding if it displays the bound value as a string.
		/// </returns>
		[DefaultValue(null)]
		public new string StringFormat { get { return m_Converter.StringFormat[1]; } set { m_Converter.StringFormat[1] = value; } }
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