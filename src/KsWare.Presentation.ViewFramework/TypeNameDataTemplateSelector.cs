using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace KsWare.Presentation.ViewFramework {

//TODO the name and sumary does no more reflect the full functionality of the class

	/// <summary> Provides a <see cref="DataTemplateSelector"/> 
	/// which maps the type name to a ressource key
	/// </summary>
	public class TypeNameDataTemplateSelector : DataTemplateSelector {

		private static readonly Lazy<TypeNameDataTemplateSelector> s_LazyDefault=new Lazy<TypeNameDataTemplateSelector>(()=>new TypeNameDataTemplateSelector());


		public static TypeNameDataTemplateSelector Default => s_LazyDefault.Value;

		/// <summary> Gets or sets the resource key for null data template.
		/// </summary>
		/// <value>The null data template key.</value>
		public string NullDataTemplateKey { get; set; }

		/// <summary> Gets or sets the resource key for unkown type template.
		/// </summary>
		/// <value>The unkown type template key.</value>
		/// 
		public string UnkownTypeTemplate { get; set; }

		/// <summary> Gets or sets the key suffix.
		/// </summary>
		/// <value>The suffix.</value>
		public string Suffix { get; set; }

		/// <summary> Gets or sets the key prefix.
		/// </summary>
		/// <value>The prefix.</value>
		public string Prefix { get; set; }

		/// <summary> Gets or sets a value indicating whether trace is enabled.
		/// </summary>
		/// <value><c>true</c> if trace is enabled; otherwise, <c>false</c>.</value>
		public bool Trace { get; set; }
		
		/// <summary> 
		/// </summary>
		public ResourceDictionary Resources { get; set; }

		public Type AnyTypeOfControlsAssembly { set => ControlsAssembly = value.Assembly; get => null; }

		/// <summary> Gets or sets the assembly which contains the views.
		/// </summary>
		/// <value>The control assembly.</value>
		public Assembly ControlsAssembly { get; set; }

		/// <summary> Initializes a new instance of the <see cref="TypeNameDataTemplateSelector"/> class.
		/// </summary>
		public TypeNameDataTemplateSelector() {
			Resources=new ResourceDictionary();
			TypeMap=new Dictionary<Type, Type>();
			NullDataTemplateKey = "NullDataTemplate";
			UnkownTypeTemplate = "UnkownTypeTemplate";
		}

		/// <summary> Returns a <see cref="T:System.Windows.DataTemplate" /> based on custom logic.
		/// </summary>
		/// <param name="item">The data object for which to select the template.</param>
		/// <param name="container">The data-bound object.</param>
		/// <returns>Returns a <see cref="T:System.Windows.DataTemplate" /> or null. The default value is null.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			var element = container as FrameworkElement;
			if (element == null) return null;

			string templateKey;
			string itemTypeName = (item == null ? "{Null}" : DebugUtil.FormatTypeName(item.GetType(),false));
			if(item==null) {
				templateKey = (Prefix??"") + NullDataTemplateKey + (Suffix??"");
			} else {
				templateKey = (Prefix??"") + itemTypeName + (Suffix??"");
			}

			DataTemplate dataTemplate=null;

			// find key
			dataTemplate=FindResource(templateKey,Resources,element,Trace);

			// find {x:Type} key
			if (dataTemplate == null && item != null)
				dataTemplate = FindResource(item.GetType(), Resources, element,Trace);

			// try DefaultViewAttribute
			if (dataTemplate == null && item != null) {
				var customAttributes = item.GetType().GetCustomAttributes(typeof (DefaultViewAttribute), true);
				if (customAttributes.Length == 0) {
					if(Trace) Debug.WriteLine("DefaultViewAttribute not specified. continue with default behavoir.");
				}
				foreach (DefaultViewAttribute defaultViewAttribute in customAttributes) {
					dataTemplate = CreateFromType(defaultViewAttribute.View);
					break;
				}
			}

			// try ***VM -> ***View
			if (dataTemplate == null && item != null) dataTemplate = ViewModel2View(item);

			// find UnkownTypeTemplate
			if(dataTemplate==null && itemTypeName!="{Null}") {
				dataTemplate=element.TryFindResource(UnkownTypeTemplate) as DataTemplate;
				if(Trace) Debug.WriteLine("INFO: Resource 'UnkownTypeTemplate' not found. Continue with default behavior.");
			}

			if(dataTemplate==null) {
				var el = (FrameworkElement)element;
				string s = el.GetType().Name;
				while (el.Parent!=null && el!=el.Parent) {
					el = (FrameworkElement)el.Parent;
					s += el.GetType().Name + " > ";
				}
				var errMsg = "\tKey: "+templateKey
					+"\n\t"+"Type: "+ itemTypeName
					+"\n\t"+"Root: " + el.GetType().Name
					+"\n\t"+"Path: " + s;
				if(Trace) Debug.WriteLine("DataTemplate not found!\n"+errMsg+"\nCreating error template.");
				dataTemplate = CreateUnkownTypeTemplate(errMsg);
				//throw new WarningException("DataTemplate not found!\n"+errMsg);
			}

			return dataTemplate;

		}

		public Dictionary<Type, Type> TypeMap { get; set; }
		

		/*\
		 * DeclaringType.Assembly
		 *		xxxVM -> xxx -> xxxView
		\*/

		private DataTemplate ViewModel2View(object vmObject) {
			DataTemplate dataTemplate = null;
			var vmType = vmObject.GetType();
			if (!vmType.Name.EndsWith("VM")) return null;
			if (vmType.IsGenericType) return null;

			var name = vmType.Name.Substring(0, vmType.Name.Length - 2) + "View";
			var viewTypes = new List<Type>();
			var assembly = ControlsAssembly??vmType.Assembly;
			foreach (var t in assembly.GetTypes()) {
				//if(typeof(Control).IsAssignableFrom(t)&& t.Name==name){viewType=t;break;}
				//if(typeof(Page   ).IsAssignableFrom(t)&& t.Name==name){viewType=t;break;}
				//if(typeof(Window ).IsAssignableFrom(t)&& t.Name==name){viewType=t;break;}
				if (typeof (Visual).IsAssignableFrom(t) && t.Name == name) { viewTypes.Add(t); }
			}
			Type viewType = null;
			if (viewTypes.Count == 1) {
				if (Trace)
					Debug.WriteLine("INFO: view type found. " +
							        "\n\tViewModelType: " + vmType.FullName +
							        "\n\tUsedType: " + viewTypes[0].FullName);
				viewType = viewTypes[0];
			}
			else if (viewTypes.Count > 1) {
				Debug.WriteLine("WARNING: Ambigous type! Using first." +
						        "\n\tViewModelType: " + vmType.FullName +
						        "\n\tTypeName: " + name +
						        "\n\tUsedType: " + viewTypes[0].FullName);
				viewType = viewTypes[0];
			}

			if (viewType != null) dataTemplate = CreateFromType(viewType);
			return dataTemplate;
		}

		private static DataTemplate FindResource(object key, ResourceDictionary resources, FrameworkElement element, bool trace) {
			DataTemplate dataTemplate;
			if (resources!=null && resources.Contains(key)) 
				dataTemplate = resources[key] as DataTemplate;
			else if (element != null) 
				dataTemplate = element.TryFindResource(key) as DataTemplate;
			else 
				dataTemplate = null;
			return dataTemplate;
		}

		private DataTemplate CreateFromType(Type viewType) {
			var dataTemplate = new DataTemplate();
			var view = new FrameworkElementFactory(viewType);
			dataTemplate.VisualTree = view;
			return dataTemplate;
		}

		private static DataTemplate CreateUnkownTypeTemplate(string errMsg) {
			var dataTemplate = new DataTemplate();

			var stackPanel = new FrameworkElementFactory(typeof(StackPanel));
//			stackPanel.SetValue(StackPanel.BackgroundProperty, Brushes.White);
			stackPanel.SetResourceReference(Panel.BackgroundProperty, SystemColors.WindowBrushKey);

			var title = new FrameworkElementFactory(typeof(TextBlock));
			title.SetValue(TextBlock.ForegroundProperty, Brushes.Red);
//			title.SetValue(TextBlock.BackgroundProperty, Brushes.White);
			title.SetResourceReference(TextBlock.BackgroundProperty, SystemColors.WindowBrushKey);
			title.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
			title.SetValue(TextBlock.TextProperty, "Unknown data type!");
			stackPanel.AppendChild(title);

			var msg = new FrameworkElementFactory(typeof(TextBox));
//			msg.SetValue(Control.ForegroundProperty, Brushes.Black);
			msg.SetResourceReference(Control.ForegroundProperty, SystemColors.ControlTextBrushKey);
//			msg.SetValue(Control.BackgroundProperty, Brushes.White);
			msg.SetValue(Control.BackgroundProperty, SystemColors.WindowBrushKey);			
			msg.SetValue(Control.FontWeightProperty, FontWeights.Normal);
			msg.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
			msg.SetValue(TextBox.TextProperty, errMsg);
			msg.SetValue(TextBoxBase.IsReadOnlyProperty, true);
			stackPanel.AppendChild(msg);

			dataTemplate.VisualTree = stackPanel;
			return dataTemplate;
		}
	}
}

/* Example:

	<Window.Resources>
		<vf:TypeNameDataTemplateSelector x:Key="ScreenSelector" />
		<DataTemplate x:Key="NullDataTemplate"><TextBlock Text="No Data"/></DataTemplate>
		<DataTemplate x:Key="MyWindowVM"><TextBlock Text="WindowVM"/></DataTemplate>
		<DataTemplate x:Key="{x:Type screens:ProductSetupMainScreenVM}"><screens:ProductMainScreenView/></DataTemplate>

		<vf:TypeNameDataTemplateSelector x:Key="ModalSelector" Prefix="Modal_"/>
		<DataTemplate x:Key="Modal_ErrorModalVM"><modals:ErrorModalView/></DataTemplate>
		<DataTemplate x:Key="Modal_WarningsModalVM"><modals:WarningsModalView/></DataTemplate>
	...
 
	 <ItemsControl ItemsSource="{Binding MyProperty}">
		<ItemsControl.ItemTemplateSelector>
			<viewFramework:TypeNameDataTemplateSelector>
				<viewFramework:TypeNameDataTemplateSelector.Resources>
					<DataTemplate x:Key="NullDataTemplate"><TextBlock Text="No Data"/></DataTemplate>
					<DataTemplate x:Key="MyWindowVM"><TextBlock Text="WindowVM"/></DataTemplate>
				</viewFramework:TypeNameDataTemplateSelector.Resources>
			</viewFramework:TypeNameDataTemplateSelector>
		</ItemsControl.ItemTemplateSelector>
	</ItemsControl>
*/

