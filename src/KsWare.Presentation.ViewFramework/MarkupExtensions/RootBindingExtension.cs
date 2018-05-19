using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace KsWare.Presentation.ViewFramework {

	/// <summary>
	/// XAML markup extension to bind directly to root data context.
	/// </summary>
	/// <example>
	/// Example usage in XAML:
	/// <code>
	/// <![CDATA[
	/// <UserControl x:Class="MyProject.Views.SampleView"
	///              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	///              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	///              xmlns:m="clr-namespace:YourNamespace.MarkupExtensions;assembly=YourAssemblyName"
	///              .... >
	///     <Grid x:Name="LayoutRoot" Background="White">
	///         <ListBox ItemsSource="{Binding Names}">
	///             <ListBox.ItemTemplate>
	///                 <DataTemplate>
	///                     <StackPanel Orientation="Horizontal">
	///                         <TextBlock Text="{Binding}"/>
	///                         <Button Content="{m:RootBinding Path=ButtonContent}" 
	///                                 Command="{m:RootBinding Path=MessageBoxCommand}"/>
	///                     </StackPanel>
	///                 </DataTemplate>
	///             </ListBox.ItemTemplate>
	///         </ListBox>
	///     </Grid>
	/// </UserControl>
	/// ]]>
	/// </code>
	/// </example>
	/// <author>Damian Schenkelman</author>
	/// <author>Jeffrey Sadeli</author>
	/// <seealso href="http://blogs.southworks.net/dschenkelman/2011/06/26/binding-to-view-model-properties-in-data-templates-the-rootbinding-markup-extension/"/>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/system.windows.data.binding.aspx"/>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/ms750413.aspx"/>
	/// <seealso href="http://loosexaml.wordpress.com/2009/04/09/reference-to-self-in-xaml/"/>
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "public API")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification         = "public API")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification               = "public API")]
	public class RootBindingExtension : MarkupExtension {
		private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

		private bool requiresFrameworkElementRootObject = true;
		private BindingMode mode = BindingMode.Default;
		private UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default;
		private string bindingGroupName = string.Empty;
		private object fallbackValue = DependencyProperty.UnsetValue;

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RootBindingExtension"/> class.
		/// </summary>
		public RootBindingExtension() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RootBindingExtension"/> class.
		/// </summary>
		/// <param name="path">The binding path.</param>
		public RootBindingExtension(string path) : this() { Path = path; }

		#endregion

		#region property getters and setters

		/// <summary>
		/// Gets or sets a value indicating whether root object is required to be of type <see cref="FrameworkElement"/>.
		/// Default value is <c>True</c>.
		/// </summary>
		/// <remarks>
		/// In typical scenario, you want to leave this option as the default (<c>True</c>).
		/// Change this option to <c>False</c> if the designer (e.g. Visual Studio) has problems.
		/// </remarks>
		[DefaultValue(true)]
		public bool RequiresFrameworkElementRootObject {
			get => requiresFrameworkElementRootObject;
			set => requiresFrameworkElementRootObject = value;
		}

		// ----------------------------------------------------------------------------------------

		/// <summary>
		/// Gets or sets the binding path.
		/// </summary>
		[ConstructorArgument("path")]
		public string Path { get; set; }

		/// <summary>
		/// Gets or sets the binding mode.
		/// Default value is <see cref="System.Windows.Data.BindingMode.Default"/>.
		/// </summary>
		[DefaultValue(BindingMode.Default)]
		public BindingMode Mode { get => mode; set => mode = value; }

		/// <summary>
		/// Gets or sets the binding update source trigger.
		/// Default value is <see cref="System.Windows.Data.UpdateSourceTrigger.Default"/>.
		/// </summary>
		[DefaultValue(UpdateSourceTrigger.Default)]
		public UpdateSourceTrigger UpdateSourceTrigger {
			get => updateSourceTrigger;
			set => updateSourceTrigger = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to raise the <see cref="Binding.SourceUpdatedEvent"/>
		/// when a value is transferred from the binding target to the binding source.
		/// Default value is <c>False</c>.
		/// </summary>
		[DefaultValue(false)]
		public bool NotifyOnSourceUpdated { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to raise the <see cref="Binding.TargetUpdatedEvent"/>
		/// when a value is transferred from the binding target to the binding source.
		/// Default value is <c>False</c>.
		/// </summary>
		[DefaultValue(false)]
		public bool NotifyOnTargetUpdated { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to raise the <see cref="System.Windows.Controls.Validation.ErrorEvent"/>
		/// attached event on the bound object.
		/// Default value is <c>False</c>.
		/// </summary>
		[DefaultValue(false)]
		public bool NotifyOnValidationError { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to include the <see cref="System.Windows.Controls.DataErrorValidationRule"/>.
		/// Default value is <c>False</c>.
		/// </summary>
		[DefaultValue(false)]
		public bool ValidatesOnDataErrors { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to include the <see cref="System.Windows.Controls.ExceptionValidationRule"/>.
		/// Default value is <c>False</c>.
		/// </summary>
		[DefaultValue(false)]
		public bool ValidatesOnExceptions { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is async.
		/// Default value is <c>False</c>.
		/// </summary>
		[DefaultValue(false)]
		public bool IsAsync { get; set; }

		/// <summary>
		/// Gets or sets the name of the <see cref="BindingGroup"/> to which this binding belongs.
		/// Default value is empty string.
		/// </summary>
		[DefaultValue("")]
		public string BindingGroupName { get => bindingGroupName; set => bindingGroupName = value; }

		/// <summary>
		/// Gets or sets the converter to use.
		/// Default value is <c>null</c>.
		/// </summary>
		[DefaultValue(null)]
		public IValueConverter Converter { get; set; }

		/// <summary>
		/// Gets or sets the culture to which to evaluate the converter.
		/// Default value is <c>null</c>.
		/// </summary>
		[DefaultValue(null)]

		public CultureInfo ConverterCulture { get; set; }

		/// <summary>
		/// Gets or sets the parameter to pass to converter.
		/// Default value is <c>null</c>.
		/// </summary>
		[DefaultValue(null)]

		public object ConverterParameter { get; set; }

		/// <summary>
		/// Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.
		/// Default value is <c>null</c>.
		/// </summary>
		[DefaultValue(null)]
		public string StringFormat { get; set; }

		/// <summary>
		/// Gets or sets the value to use when the binding is unable to return a value.
		/// Default value is <see cref="DependencyProperty.UnsetValue"/>.
		/// </summary>
		public object FallbackValue { get => fallbackValue; set => fallbackValue = value; }

		#endregion

		/// <summary>
		/// When implemented in a derived class, returns an object that is set as the value of the target property 
		/// for this markup extension.
		/// </summary>
		/// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
		/// <returns>The object value to set on the property where the extension is applied.</returns>
		public override object ProvideValue(IServiceProvider serviceProvider) {
			var rootProvider = (IRootObjectProvider) serviceProvider.GetService(typeof(IRootObjectProvider));

			if (rootProvider == null) {
				if (IsInDesignMode) {
					return null;
				}

				throw new InvalidOperationException("IRootObjectProvider is null.") {Data = {{"Source", "RootBindingExtension"}}};
			}

			object rootObject = rootProvider.RootObject;

			if (rootObject == null) {
				if (IsInDesignMode) {
					return ProvideValueHelper(typeof(object));
				}

				throw new InvalidOperationException("Root object is null.") {Data = {{"Source", "RootBindingExtension"}}};
			}

			if (RequiresFrameworkElementRootObject && !IsInDesignMode) {
				var element = rootObject as FrameworkElement;
				if (element == null) {
					throw new InvalidOperationException(
						$"Root object's '{rootObject.GetType()}' type is not of type FrameworkElement.") {
						Data = {{"Source", "RootBindingExtension"}}
					};
				}
			}

			return ProvideValueHelper(rootObject.GetType());
		}

		/// <summary>
		/// Helper method for provide value to create binding object.
		/// </summary>
		/// <param name="ancestorType">Type of the ancestor.</param>
		/// <returns>The binding object.</returns>
		private Binding ProvideValueHelper(Type ancestorType) {
			string bindingPath = string.IsNullOrEmpty(Path) ? "DataContext" : "DataContext." + Path;

			return new Binding() {
				Path                    = new PropertyPath(bindingPath),
				Mode                    = Mode,
				UpdateSourceTrigger     = UpdateSourceTrigger,
				NotifyOnSourceUpdated   = NotifyOnSourceUpdated,
				NotifyOnTargetUpdated   = NotifyOnTargetUpdated,
				NotifyOnValidationError = NotifyOnValidationError,
				ValidatesOnDataErrors   = ValidatesOnDataErrors,
				ValidatesOnExceptions   = ValidatesOnExceptions,
				IsAsync                 = IsAsync,
				BindingGroupName        = BindingGroupName,
				RelativeSource =
					new RelativeSource() {Mode = RelativeSourceMode.FindAncestor, AncestorType = ancestorType,},
				Converter          = Converter,
				ConverterCulture   = ConverterCulture,
				ConverterParameter = ConverterParameter,
				StringFormat       = StringFormat,
				FallbackValue      = FallbackValue,
			};
		}
	}

}