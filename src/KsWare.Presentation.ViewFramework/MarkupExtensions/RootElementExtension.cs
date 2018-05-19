using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace KsWare.Presentation.ViewFramework {

	/// <summary>
	/// XAML markup extension to bind root element.
	/// </summary>
	/// <seealso cref="System.Windows.Markup.MarkupExtension" />
	/// <example>
	/// <code language="XAML"> 
	/// &lt;Binding Source={RootElement}/&gt;
	/// </code>
	/// </example>
	public class RootElementExtension : MarkupExtension {
		private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

		/// <inheritdoc />
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (IsInDesignMode) {
				return null;
			}

			var rootProvider = (IRootObjectProvider) serviceProvider.GetService(typeof(IRootObjectProvider));
			var rootObject   = rootProvider?.RootObject;

			if (rootObject == null) {
				throw new InvalidOperationException("Root object is null.");
			}

			var element = rootObject as FrameworkElement;
			if (element == null) {
				throw new InvalidOperationException(
					$"Root object's '{rootObject.GetType()}' type is not of type FrameworkElement.");
			}

			return element;
		}
	}

}