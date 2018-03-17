using System.ComponentModel;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	/// <summary>
	/// Internal base class for all multi value bindings (e.g. <see cref="BindingWithPrefixAndSuffix"/>)
	/// </summary>
	/// <seealso cref="System.Windows.Data.MultiBinding" />
	public class BindingUsesMultiBinding : MultiBinding {

		/// <summary> The main binding
		/// </summary>
		protected Binding MainBinding;

		/// <summary>
		/// Initialisiert eine neue Instanz der <see cref="T:System.Windows.Data.Binding"/>-Klasse.
		/// </summary>
		public BindingUsesMultiBinding() { }

//		/// <summary>
//		/// Initialisiert eine neue Instanz der <see cref="T:System.Windows.Data.Binding"/>-Klasse mit einem Anfangspfad.
//		/// </summary>
//		/// <param name="path">Der anfängliche <see cref="P:System.Windows.Data.Binding.Path"/> für die Bindung.</param>
//		public BindingUsesMultiBinding(string path) : base(path) { }

		#region BindingBase
//			/// <summary>
//			/// Gibt einen Wert zurück, mit dem angegeben wird, ob Serialisierungsprozesse den tatsächlichen Wert der <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/>-Eigenschaft für Instanzen dieser Klasse serialisieren sollen.
//			/// </summary>
//			/// 
//			/// <returns>
//			/// true, wenn der <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/>-Eigenschaftswert serialisiert werden soll, andernfalls false.
//			/// </returns>
//			[EditorBrowsable(EditorBrowsableState.Never)]
//			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//			public bool ShouldSerializeFallbackValue();

//			/// <summary>
//			/// Gibt einen Wert zurück, der angibt, ob die <see cref="P:System.Windows.Data.BindingBase.TargetNullValue"/>-Eigenschaft serialisiert werden soll.
//			/// </summary>
//			/// 
//			/// <returns>
//			/// true, wenn die <see cref="P:System.Windows.Data.BindingBase.TargetNullValue"/>-Eigenschaft serialisiert werden soll, andernfalls false.
//			/// </returns>
//			[EditorBrowsable(EditorBrowsableState.Never)]
//			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//			public bool ShouldSerializeTargetNullValue();

//			/// <summary>
//			/// Gibt ein Objekt zurück, das für die Eigenschaft festgelegt werden soll, auf die diese Bindung und Erweiterung angewendet werden.
//			/// </summary>
//			/// 
//			/// <returns>
//			/// Der Wert, der für die Bindungsziel-Eigenschaft festgelegt werden soll.
//			/// </returns>
//			/// <param name="serviceProvider">Das Objekt, das Dienste für die Markuperweiterung bereitstellen kann. Kann null sein. Weitere Informationen finden Sie im Abschnitt mit den Hinweisen.</param>
//			public override sealed object ProvideValue(IServiceProvider serviceProvider);

//			/// <summary>
//			/// Ruft den Wert ab, der verwendet werden soll, wenn die Bindung keinen Wert zurückgeben kann, oder legt diesen fest.
//			/// </summary>
//			/// 
//			/// <returns>
//			/// Der Standardwert ist <see cref="F:System.Windows.DependencyProperty.UnsetValue"/>.
//			/// </returns>
//			public object FallbackValue { get; set; }

		/// <summary>
		/// Ruft eine Zeichenfolge ab, die angibt, wie die Bindung formatiert werden soll, wenn diese den gebundenen Wert als Zeichenfolge anzeigt, oder legt diese fest.
		/// </summary>
		/// 
		/// <returns>
		/// Eine Zeichenfolge, die angibt, wie die Bindung formatiert werden soll, wenn diese den gebundenen Wert als Zeichenfolge anzeigt.
		/// </returns>
		[DefaultValue(null)]
		public new string StringFormat { get => MainBinding.StringFormat; set => MainBinding.StringFormat = value; }

//			/// <summary>
//			/// Ruft den Wert ab, der im Ziel verwendet wird, wenn der Wert der Quelle null ist, oder legt diesen fest.
//			/// </summary>
//			/// 
//			/// <returns>
//			/// Der Wert, der im Ziel verwendet wird, wenn der Wert der Quelle null ist.
//			/// </returns>
//			public object TargetNullValue { get; set; }

//			/// <summary>
//			/// Ruft den Namen der <see cref="T:System.Windows.Data.BindingGroup"/> ab, zu der diese Bindung gehört, oder legt diesen fest.
//			/// </summary>
//			/// 
//			/// <returns>
//			/// Der Name der <see cref="T:System.Windows.Data.BindingGroup"/>, zu der diese Bindung gehört.
//			/// </returns>
//			[DefaultValue("")]
//			public string BindingGroupName { get; set; }

//			/// <summary>
//			/// Ruft die Zeitdauer in Millisekunden ab, die gewartet wird, ehe die Bindungsquelle aktualisiert wird, nachdem sich der Wert im Ziel geändert hat, oder legt diese fest.
//			/// </summary>
//			/// 
//			/// <returns>
//			/// Die Zeitdauer in Millisekunden, die gewartet werden soll, bevor die Bindungsquelle aktualisiert wird.
//			/// </returns>
//			[DefaultValue(0)]
//			public int Delay { get; set; }

		#endregion

		#region Binding
//		/// <summary>
//		/// Wird als <see cref="P:System.ComponentModel.PropertyChangedEventArgs.PropertyName"/> von <see cref="T:System.ComponentModel.PropertyChangedEventArgs"/> verwendet, um anzugeben, dass sich eine Indexereigenschaft geändert hat.
//		/// </summary>
//		public const string IndexerName = "Item[]";
//
//		/// <summary>
//		/// Bezeichnet das angefügte Ereignis<see cref="E:System.Windows.Data.Binding.SourceUpdated"/>.
//		/// </summary>
//		public static readonly RoutedEvent SourceUpdatedEvent;
//
//		/// <summary>
//		/// Bezeichnet das angefügte Ereignis<see cref="E:System.Windows.Data.Binding.TargetUpdated"/>.
//		/// </summary>
//		public static readonly RoutedEvent TargetUpdatedEvent;
//
//		/// <summary>
//		/// Bezeichnet die angefügte Eigenschaft<see cref="P:System.Windows.Data.Binding.XmlNamespaceManager"/>.
//		/// </summary>
//		public static readonly DependencyProperty XmlNamespaceManagerProperty;

		/// <summary>
		/// Wird als zurückgegebener Wert verwendet, um das Bindungsmodul anzuweisen, keine Aktion auszuführen.
		/// </summary>
		public static readonly object DoNothing=Binding.DoNothing;


//		/// <summary>
//		/// Fügt einen Handler für das <see cref="E:System.Windows.Data.Binding.SourceUpdated"/>-Ereignis hinzu, das ein angefügtes Ereignis ist.
//		/// </summary>
//		/// <param name="element">Das <see cref="T:System.Windows.UIElement"/> oder das <see cref="T:System.Windows.ContentElement"/>, das das Ereignis überwacht.</param><param name="handler">Der hinzuzufügende Handler.</param>
//		public static void AddSourceUpdatedHandler(DependencyObject element, EventHandler<DataTransferEventArgs> handler);
//
//		/// <summary>
//		/// Entfernt einen Handler für das <see cref="E:System.Windows.Data.Binding.SourceUpdated"/>-Ereignis, das ein angefügtes Ereignis ist.
//		/// </summary>
//		/// <param name="element">Das <see cref="T:System.Windows.UIElement"/> oder das <see cref="T:System.Windows.ContentElement"/>, das das Ereignis überwacht.</param><param name="handler">Der zu entfernende Handler.</param>
//		public static void RemoveSourceUpdatedHandler(DependencyObject element, EventHandler<DataTransferEventArgs> handler);
//
//		/// <summary>
//		/// Fügt einen Handler für das <see cref="E:System.Windows.Data.Binding.TargetUpdated"/>-Ereignis hinzu, das ein angefügtes Ereignis ist.
//		/// </summary>
//		/// <param name="element">Das <see cref="T:System.Windows.UIElement"/> oder das <see cref="T:System.Windows.ContentElement"/>, das das Ereignis überwacht.</param><param name="handler">Der hinzuzufügende Handler.</param>
//		public static void AddTargetUpdatedHandler(DependencyObject element, EventHandler<DataTransferEventArgs> handler);
//
//		/// <summary>
//		/// Entfernt einen Handler für das <see cref="E:System.Windows.Data.Binding.TargetUpdated"/>-Ereignis, das ein angefügtes Ereignis ist.
//		/// </summary>
//		/// <param name="element">Das <see cref="T:System.Windows.UIElement"/> oder das <see cref="T:System.Windows.ContentElement"/>, das das Ereignis überwacht.</param><param name="handler">Der zu entfernende Handler.</param>
//		public static void RemoveTargetUpdatedHandler(DependencyObject element, EventHandler<DataTransferEventArgs> handler);
//
//		/// <summary>
//		/// Gibt ein XML-Namespace-Managerobjekt zurück, das von der Bindung verwendet wird, die an das angegebene Objekt angefügt ist.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Ein zurückgegebenes Objekt, das zum Anzeigen von XML-Namespaces verwendet wird, die sich auf die Bindung des übergebenen Objektelements beziehen. Dieses Objekt sollte in <see cref="T:System.Xml.XmlNamespaceManager"/> umgewandelt werden.
//		/// </returns>
//		/// <param name="target">Das Objekt, aus dem Namespaceinformationen abgerufen werden sollen.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="target"/>-Parameter darf nicht null sein.</exception>
//		public static XmlNamespaceManager GetXmlNamespaceManager(DependencyObject target);
//
//		/// <summary>
//		/// Legt ein Namespace-Managerobjekt fest, das von der Bindung verwendet wird, die an das bereitgestellte Element angefügt ist.
//		/// </summary>
//		/// <param name="target">Das Objekt, aus dem Namespaceinformationen abgerufen werden sollen.</param><param name="value">Der <see cref="T:System.Xml.XmlNamespaceManager"/>, der im übergebenen Element für die Namespaceauswertung verwendet werden soll.</param><exception cref="T:System.ArgumentNullException"><paramref name="target"/> ist null.</exception>
//		public static void SetXmlNamespaceManager(DependencyObject target, XmlNamespaceManager value);
//
//		/// <summary>
//		/// Gibt an, ob die <see cref="P:System.Windows.Data.Binding.ValidationRules"/>-Eigenschaft beibehalten werden soll.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn der Eigenschaftswert geändert wurde und nicht mehr dem Standardwert entspricht, andernfalls false.
//		/// </returns>
//		[EditorBrowsable(EditorBrowsableState.Never)]
//		public bool ShouldSerializeValidationRules();
//
//		/// <summary>
//		/// Gibt an, ob die <see cref="P:System.Windows.Data.Binding.Path"/>-Eigenschaft beibehalten werden soll.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn der Eigenschaftswert geändert wurde und nicht mehr dem Standardwert entspricht, andernfalls false.
//		/// </returns>
//		[EditorBrowsable(EditorBrowsableState.Never)]
//		public bool ShouldSerializePath();
//
//		/// <summary>
//		/// Gibt an, ob die <see cref="P:System.Windows.Data.Binding.Source"/>-Eigenschaft beibehalten werden soll.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn der Eigenschaftswert geändert wurde und nicht mehr dem Standardwert entspricht, andernfalls false.
//		/// </returns>
//		[EditorBrowsable(EditorBrowsableState.Never)]
//		public bool ShouldSerializeSource();
//
//		/// <summary>
//		/// Ruft eine Auflistung von Regeln ab, die die Gültigkeit von Benutzereingaben überprüfen.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Eine Auflistung von <see cref="T:System.Windows.Controls.ValidationRule"/>-Objekten.
//		/// </returns>
//		public new Collection<ValidationRule> ValidationRules { get { return MainBinding.ValidationRules; } }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der angibt, ob die <see cref="T:System.Windows.Controls.ExceptionValidationRule"/> eingeschlossen werden soll, oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn die <see cref="T:System.Windows.Controls.ExceptionValidationRule"/> eingeschlossen werden soll, andernfalls false.
//		/// </returns>
//		[DefaultValue(false)]
//		public bool ValidatesOnExceptions { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der angibt, ob die <see cref="T:System.Windows.Controls.DataErrorValidationRule"/> eingeschlossen werden soll, oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn die <see cref="T:System.Windows.Controls.DataErrorValidationRule"/> eingeschlossen werden soll, andernfalls false.
//		/// </returns>
//		[DefaultValue(false)]
//		public bool ValidatesOnDataErrors { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der angibt, ob die <see cref="T:System.Windows.Controls.NotifyDataErrorValidationRule"/> eingeschlossen werden soll, oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn die <see cref="T:System.Windows.Controls.NotifyDataErrorValidationRule"/> eingeschlossen werden soll, andernfalls false. Die Standardeinstellung ist true.
//		/// </returns>
//		[DefaultValue(true)]
//		public bool ValidatesOnNotifyDataErrors { get; set; }
//
//		/// <summary>
//		/// Ruft den Pfad zur Bindungsquell-Eigenschaft ab oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Der Pfad zur Bindungsquelle. Der Standardwert ist null.
//		/// </returns>
//		public PropertyPath Path { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; set; }
//
//		/// <summary>
//		/// Ruft eine XPath-Abfrage ab, die den Wert der zu verwendenden XML-Bindungsquelle zurückgibt, oder legt diese fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Die XPath-Abfrage. Der Standardwert ist null.
//		/// </returns>
//		[DefaultValue(null)]
//		public string XPath { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der die Richtung des Datenflusses in der Bindung angibt, oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Einer der <see cref="T:System.Windows.Data.BindingMode"/>-Werte. Der Standardwert <see cref="F:System.Windows.Data.BindingMode.Default"/> gibt den Standardwert für den Bindungsmodus der Ziel-Abhängigkeitseigenschaft zurück. Der Standardwert ändert sich jedoch für jede Abhängigkeitseigenschaft. Im Allgemeinen verfügen Steuerelementeigenschaften, die vom Benutzer bearbeitet werden können (z. B. Textfelder und Kontrollkästchen) standardmäßig über bidirektionale Bindungen, während die meisten anderen Eigenschaften standardmäßig unidirektionale Bindungen aufweisen. Eine programmgesteuerte Möglichkeit zu bestimmen, ob eine Abhängigkeitseigenschaft über eine unidirektionale oder bidirektionale Bindung verfügt, besteht darin, die Eigenschaftenmetadaten der Eigenschaft mithilfe von <see cref="M:System.Windows.DependencyProperty.GetMetadata(System.Type)"/> abzurufen und anschließend den booleschen Wert der <see cref="P:System.Windows.FrameworkPropertyMetadata.BindsTwoWayByDefault"/>-Eigenschaft zu überprüfen.
//		/// </returns>
//		[DefaultValue(BindingMode.Default)]
//		public BindingMode Mode { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der die zeitliche Steuerung von Bindungsquell-Aktualisierungen bestimmt, oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Einer der <see cref="T:System.Windows.Data.UpdateSourceTrigger"/>-Werte. Der Standardwert <see cref="F:System.Windows.Data.UpdateSourceTrigger.Default"/> gibt den <see cref="T:System.Windows.Data.UpdateSourceTrigger"/>-Standardwert der Ziel-Abhängigkeitseigenschaft zurück. Der Standardwert für die meisten Abhängigkeitseigenschaften ist jedoch <see cref="F:System.Windows.Data.UpdateSourceTrigger.PropertyChanged"/>, während die <see cref="P:System.Windows.Controls.TextBox.Text"/>-Eigenschaft den Standardwert <see cref="F:System.Windows.Data.UpdateSourceTrigger.LostFocus"/> aufweist. Eine programmgesteuerte Möglichkeit, den <see cref="P:System.Windows.Data.Binding.UpdateSourceTrigger"/>-Standardwert einer Abhängigkeitseigenschaft zu bestimmen, besteht darin, die Eigenschaftenmetadaten der Eigenschaft mit <see cref="M:System.Windows.DependencyProperty.GetMetadata(System.Type)"/> abzurufen und anschließend den Wert der <see cref="P:System.Windows.FrameworkPropertyMetadata.DefaultUpdateSourceTrigger"/>-Eigenschaft zu überprüfen.
//		/// </returns>
//		[DefaultValue(UpdateSourceTrigger.Default)]
//		public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der angibt, ob das <see cref="E:System.Windows.Data.Binding.SourceUpdated"/>-Ereignis ausgelöst werden soll, wenn ein Wert vom Bindungsziel zur Bindungsquelle übertragen wird, oder legt diesen Wert fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn das <see cref="E:System.Windows.Data.Binding.SourceUpdated"/>-Ereignis ausgelöst werden soll, wenn der Bindungsquell-Wert aktualisiert wird, andernfalls false. Die Standardeinstellung ist false.
//		/// </returns>
//		[DefaultValue(false)]
//		public bool NotifyOnSourceUpdated { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der angibt, ob das <see cref="E:System.Windows.Data.Binding.TargetUpdated"/>-Ereignis ausgelöst werden soll, wenn ein Wert von der Bindungsquelle an das Bindungsziel übertragen wird, oder legt diesen Wert fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn das <see cref="E:System.Windows.Data.Binding.TargetUpdated"/>-Ereignis ausgelöst werden soll, wenn der Bindungsziel-Wert aktualisiert wird, andernfalls false. Die Standardeinstellung ist false.
//		/// </returns>
//		[DefaultValue(false)]
//		public bool NotifyOnTargetUpdated { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der angibt, ob das angefügte Ereignis<see cref="E:System.Windows.Controls.Validation.Error"/> für das gebundene Objekt ausgelöst werden soll, oder legt diesen Wert fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// true, wenn das angefügte Ereignis<see cref="E:System.Windows.Controls.Validation.Error"/> für das gebundene Objekt bei einem Validierungsfehler während der Quellaktualisierungen ausgelöst werden soll, andernfalls false. Die Standardeinstellung ist false.
//		/// </returns>
//		[DefaultValue(false)]
//		public bool NotifyOnValidationError { get; set; }
//
//		/// <summary>
//		/// Ruft den zu verwendenden Konverter ab oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Ein Wert vom Typ <see cref="T:System.Windows.Data.IValueConverter"/>. Der Standardwert ist null.
//		/// </returns>
//		[DefaultValue(null)]
//		public IValueConverter Converter { get; set; }
//
//		/// <summary>
//		/// Ruft den Parameter ab, der an den <see cref="P:System.Windows.Data.Binding.Converter"/> übergeben wird, oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Der Parameter, der an den <see cref="P:System.Windows.Data.Binding.Converter"/> übergeben wird. Der Standardwert ist null.
//		/// </returns>
//		[DefaultValue(null)]
//		public object ConverterParameter { get; set; }
//
//		/// <summary>
//		/// Ruft die Kultur ab, in der der Konverter ausgewertet werden soll, oder legt diese fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Der Standardwert ist null.
//		/// </returns>
//		[DefaultValue(null)]
//		[TypeConverter(typeof (CultureInfoIetfLanguageTagConverter))]
//		public CultureInfo ConverterCulture { get; set; }
//
//		/// <summary>
//		/// Ruft das Objekt ab, das als Bindungsquelle verwendet werden soll, oder legt dieses fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Das Objekt, das als Bindungsquelle verwendet werden soll.
//		/// </returns>
//		public object Source { get; set; }
//
//		/// <summary>
//		/// Ruft die Bindungsquelle ab, indem deren Speicherort relativ zur Position des Bindungsziels angegeben wird, oder legt diese fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Ein <see cref="T:System.Windows.Data.RelativeSource"/>-Objekt, das den relativen Speicherort der zu verwendenden Bindungsquelle angibt. Der Standardwert ist null.
//		/// </returns>
//		[DefaultValue(null)]
//		public RelativeSource RelativeSource { get; set; }
//
//		/// <summary>
//		/// Ruft den Namen des Elements ab, das als Bindungsquell-Objekt verwendet werden soll, oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Der Wert der Name-Eigenschaft oder x:Name-Direktive des relevanten Elements. Sie können nur auf im Code erstellte Elemente verweisen, wenn diese über RegisterName im entsprechenden <see cref="T:System.Windows.NameScope"/> registriert werden. Weitere Informationen finden Sie unter WPF-XAML-Namescopes. Der Standardwert ist null.
//		/// </returns>
//		[DefaultValue(null)]
//		public string ElementName { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der angibt, ob die <see cref="T:System.Windows.Data.Binding"/> Werte asynchron abrufen und festlegen soll, oder legt diesen fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Der Standardwert ist null.
//		/// </returns>
//		[DefaultValue(false)]
//		public bool IsAsync { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; set; }
//
//		/// <summary>
//		/// Ruft opake Daten ab, die an den asynchronen Datenverteiler übergeben werden, oder legt diese fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Daten, die an den asynchronen Datenverteiler übergeben werden.
//		/// </returns>
//		[DefaultValue(null)]
//		public object AsyncState { get; set; }
//
//		/// <summary>
//		/// Ruft einen Wert ab, der angibt, ob der <see cref="P:System.Windows.Data.Binding.Path"/> relativ zum Datenelement oder zum <see cref="T:System.Windows.Data.DataSourceProvider"/>-Objekt ausgewertet werden soll, oder legt diesen Wert fest.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// false, um den Pfad relativ zum Datenelement selbst auszuwerten, andernfalls true. Die Standardeinstellung ist false.
//		/// </returns>
//		[DefaultValue(false)]
//		public bool BindsDirectlyToSource { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; set; }
//
//		/// <summary>
//		/// Ruft einen Handler ab, mit dem Sie benutzerdefinierte Logik für das Behandeln von Ausnahmen bereitstellen können, die beim Aktualisieren des Bindungsquell-Werts durch das Bindungsmodul auftreten, oder legt diesen Handler fest. Dies gilt nur, wenn Sie der Bindung eine <see cref="T:System.Windows.Controls.ExceptionValidationRule"/> zugeordnet haben.
//		/// </summary>
//		/// 
//		/// <returns>
//		/// Eine Methode, die benutzerdefinierte Logik für das Behandeln von Ausnahmen bereitstellt, die beim Aktualisieren des Bindungsquell-Werts durch das Bindungsmodul auftreten.
//		/// </returns>
//		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
//		public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter { get; set; }
		
		#endregion

	}

}