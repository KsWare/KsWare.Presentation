using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using KsWare.Presentation.ViewFramework.Controls.Appearance;

namespace KsWare.Presentation.ViewFramework.Controls
{

	/// <summary> Provides an extended <see cref="Button"/>
	/// </summary>
	/// <remarks>
	/// - Header <br/>
	/// - Source (HeaderSource/ContentSource) <br/>
	/// - Toggle <br/>
	/// - Popup
	/// - DRAFT VisualStateConfiguration
	/// </remarks>
	public class KsButton:Button
	{

		#region HeaderedContentControl 

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (object), typeof (KsButton), new PropertyMetadata(null,(o, args) => ((KsButton)o).UpdateHasHeader()));

		public object Header {
			get { return (object) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		
		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof (DataTemplate), typeof (KsButton), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate HeaderTemplate {
			get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty HeaderStringFormatProperty =
			DependencyProperty.Register("HeaderStringFormat", typeof (string), typeof (KsButton), new PropertyMetadata(default(string)));

		public string HeaderStringFormat {
			get { return (string) GetValue(HeaderStringFormatProperty); }
			set { SetValue(HeaderStringFormatProperty, value); }
		}

		public static readonly DependencyProperty HorizontalHeaderAlignmentProperty =
			DependencyProperty.Register("HorizontalHeaderAlignment", typeof (HorizontalAlignment), typeof (KsButton), new PropertyMetadata(default(HorizontalAlignment)));

		public HorizontalAlignment HorizontalHeaderAlignment {
			get { return (HorizontalAlignment) GetValue(HorizontalHeaderAlignmentProperty); }
			set { SetValue(HorizontalHeaderAlignmentProperty, value); }
		}

		public static readonly DependencyProperty HeaderPaddingProperty =
			DependencyProperty.Register("HeaderPadding", typeof (Thickness), typeof (KsButton), new PropertyMetadata(default(Thickness)));

		public Thickness HeaderPadding {
			get { return (Thickness) GetValue(HeaderPaddingProperty); }
			set { SetValue(HeaderPaddingProperty, value); }
		}

		public static readonly DependencyProperty VerticalHeaderAlignmentProperty =
			DependencyProperty.Register("VerticalHeaderAlignment", typeof (VerticalAlignment), typeof (KsButton), new PropertyMetadata(default(VerticalAlignment)));

		public VerticalAlignment VerticalHeaderAlignment {
			get { return (VerticalAlignment) GetValue(VerticalHeaderAlignmentProperty); }
			set { SetValue(VerticalHeaderAlignmentProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateSelectorProperty =
			DependencyProperty.Register("HeaderTemplateSelector ", typeof (DataTemplateSelector), typeof (KsButton), new PropertyMetadata(default(DataTemplateSelector)));

		public DataTemplateSelector HeaderTemplateSelector  {
			get { return (DataTemplateSelector) GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}

		public static readonly DependencyProperty HasHeaderProperty =
			DependencyProperty.Register("HasHeader", typeof (bool), typeof (KsButton), new PropertyMetadata(true));

		[DefaultValue(true)]
		public bool HasHeader {
			get { return (bool) GetValue(HasHeaderProperty); }
			private set { SetValue(HasHeaderProperty, value); }
		}

		private void UpdateHasHeader() {
			HasHeader = Header!=null;
		}

		#endregion

		#region ContentSource Property

		private ResourcePresenter _cachedContentResourcePresenter;

		/// <summary> Provides the ContentSourceProperty
		/// </summary>
		public static readonly DependencyProperty ContentSourceProperty = DependencyProperty.Register(
			"ContentSource", typeof (Uri), typeof (KsButton), new PropertyMetadata(null,
			(d, e) => ((KsButton) d).UpdateContent()));

		/// <summary> Gets or sets the source for <see cref="ContentControl.Content"/>.
		/// </summary>
		/// <value>The source uri.</value>
		/// <example>
		/// <c>ContentSource="pack://application:,,,/AssemblyName;component/Folder/File.ext"</c>
		/// </example>
		/// <example>
		/// using ContentSourceParameter0: <br/>
		/// <c>ContentSource="pack://application:,,,/AssemblyName;component/Folder/{0}"</c>
		/// </example>
		/// <remarks>
		/// <blockquote><b>Note: </b>Use the <see cref="ContentSource"/> property alternative to <see cref="ContentControl.Content"/>. Do not use both concurrently!</blockquote>
		/// The <see cref="ContentSource"/> property ist used to show content from ressources (xaml/baml or bitmap images).<br/>
		/// An <see cref="ResourcePresenter"/> (using a <see cref="UIElementConverter"/>) will be created and assigned to <see cref="ContentControl.Content"/>.
		/// </remarks>
		public Uri ContentSource {get {return (Uri) GetValue(ContentSourceProperty);}set {SetValue(ContentSourceProperty, value);}}

		public static readonly DependencyProperty ContentSourceParameter0Property =
			DependencyProperty.Register("ContentSourceParameter0", typeof(string), typeof(KsButton), new PropertyMetadata(null,
			(d, e) => ((KsButton) d).UpdateContent()));

		public string ContentSourceParameter0 {get { return (string)GetValue(ContentSourceParameter0Property); }set { SetValue(ContentSourceParameter0Property, value); }}

		public static readonly DependencyProperty ContentSourceParameter1Property =
			DependencyProperty.Register("ContentSourceParameter1", typeof(string), typeof(KsButton), new PropertyMetadata(null,
			(d, e) => ((KsButton) d).UpdateContent()));

		public string ContentSourceParameter1 {
			get { return (string)GetValue(ContentSourceParameter1Property); }
			set { SetValue(ContentSourceParameter1Property, value); }
		}

		public static readonly DependencyProperty ContentSourceParameter2Property =
			DependencyProperty.Register("ContentSourceParameter2", typeof(string), typeof(KsButton), new PropertyMetadata(null,
			(d, e) => ((KsButton) d).UpdateContent()));

		public string ContentSourceParameter2 {
			get { return (string)GetValue(ContentSourceParameter2Property); }
			set { SetValue(ContentSourceParameter2Property, value); }
		}

		private void UpdateContent() {
			UpdateContent(ContentSource, new []{ContentSourceParameter0,ContentSourceParameter1,ContentSourceParameter2});
		}
		
		private void UpdateContent(Uri contentSource, string[] contentSourceParameters) {
			//if(DesignerProperties.GetIsInDesignMode(this)) return;
			if(contentSource==null) {
				ClearValue(ContentProperty);
			} else {
				if(_cachedContentResourcePresenter==null) 
					_cachedContentResourcePresenter = new ResourcePresenter();
				if(!ReferenceEquals(Content,_cachedContentResourcePresenter))
					Content = _cachedContentResourcePresenter;

				var v = contentSource;
				string s = contentSource.OriginalString;
				for(int i = 0; i<contentSourceParameters.Length; i++) {
					string p = contentSourceParameters[i];
					if(!string.IsNullOrEmpty(p)) {
						s=s.Replace("{"+i+"}", p);
					}
				}
				if(s!=null) v=new Uri(s,contentSource.IsAbsoluteUri?UriKind.Absolute :UriKind.Relative);

				_cachedContentResourcePresenter.Source = v;
			}
		}

		#endregion

		#region HeaderSource Property

		private ResourcePresenter _cachedHeaderResourcePresenter;

		/// <summary> Provides the HeaderSourceProperty
		/// </summary>
		public static readonly DependencyProperty HeaderSourceProperty = DependencyProperty.Register(
			"HeaderSource", typeof (Uri), typeof (KsButton), new PropertyMetadata(null,
			(d, e) => ((KsButton) d).UpdateHeader()));

		/// <summary> Gets or sets the source for <see cref="KsButton.Header"/>.
		/// </summary>
		/// <value>The source uri.</value>
		/// <example>
		/// HeaderSource="pack://application:,,,/AssemblyName;component/Folder/File.ext"/>
		/// </example>
		/// <remarks>
		/// <blockquote><b>Note: </b>Use the <see cref="HeaderSource"/> property alternative to <see cref="KsButton.Header"/>. Do not use both concurrently!</blockquote>
		/// The <see cref="HeaderSource"/> property ist used to show content from ressources (xaml/baml or bitmap images).<br/>
		/// An <see cref="ResourcePresenter"/> (using a <see cref="UIElementConverter"/>) will be created and assigned to <see cref="KsButton.Header"/>.
		/// </remarks>
		public Uri HeaderSource {
			get {return (Uri) GetValue(HeaderSourceProperty);}
			set {SetValue(HeaderSourceProperty, value);}
		}
		
		public static readonly DependencyProperty HeaderSourceParameter0Property =
			DependencyProperty.Register("HeaderSourceParameter0", typeof(string), typeof(KsButton), new PropertyMetadata(null,
			(d, e) => ((KsButton) d).UpdateHeader()));

		public string HeaderSourceParameter0 {get { return (string)GetValue(HeaderSourceParameter0Property); }set { SetValue(HeaderSourceParameter0Property, value); }}

		public static readonly DependencyProperty HeaderSourceParameter1Property =
			DependencyProperty.Register("HeaderSourceParameter1", typeof(string), typeof(KsButton), new PropertyMetadata(null,
			(d, e) => ((KsButton) d).UpdateHeader()));

		public string HeaderSourceParameter1 {get { return (string)GetValue(HeaderSourceParameter1Property); }set { SetValue(HeaderSourceParameter1Property, value); }}

		public static readonly DependencyProperty HeaderSourceParameter2Property =
			DependencyProperty.Register("HeaderSourceParameter2", typeof(string), typeof(KsButton), new PropertyMetadata(null,
			(d, e) => ((KsButton) d).UpdateHeader()));

		public string HeaderSourceParameter2 {get { return (string)GetValue(HeaderSourceParameter2Property); }set { SetValue(HeaderSourceParameter2Property, value); }}

		private void UpdateHeader() {UpdateHeader(HeaderSource,new []{HeaderSourceParameter0,HeaderSourceParameter1,HeaderSourceParameter2});}

		private void UpdateHeader(Uri headerSource, string[] headerSourceParameters) {
			if(headerSource==null) {
				ClearValue(HeaderProperty);
			} else {
				if(_cachedHeaderResourcePresenter==null) 
					_cachedHeaderResourcePresenter = new ResourcePresenter();
				if(!ReferenceEquals(Header,_cachedHeaderResourcePresenter))
					Content = _cachedHeaderResourcePresenter;

				var v = headerSource;
				string s = headerSource.OriginalString;
				for(int i = 0; i<headerSourceParameters.Length; i++) {
					string p = headerSourceParameters[i];
					if(!string.IsNullOrEmpty(p)) {
						s=s.Replace("{"+i+"}", p);
					}
				}
				if(s!=null) v=new Uri(s,headerSource.IsAbsoluteUri?UriKind.Absolute :UriKind.Relative);

				_cachedHeaderResourcePresenter.Source = v;
			}
		}

		#endregion

		#region IsChecked

		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register("IsChecked", typeof(bool?), typeof(KsButton), 
			new PropertyMetadata(default(bool?),(o, e) => ((KsButton)o).AtIsCheckedChanged(e)));

		/// <summary> Gets or sets whether the <see cref="ToggleButton"/> is checked.
		/// </summary>
		/// <returns>true if the <see cref="ToggleButton"/> is checked; false if the <see cref="ToggleButton"/> is unchecked; otherwise null. The default is false.</returns>
		/// <seealso cref="ToggleButton.IsChecked"/>
		[Bindable(BindableSupport.Yes,BindingDirection.TwoWay)]
		public bool? IsChecked {get { return (bool?)GetValue(IsCheckedProperty); }set { SetValue(IsCheckedProperty, value); }}
		
		#endregion

		#region CanToggle

		public static readonly DependencyProperty CanToggleProperty =
			DependencyProperty.Register("CanToggle", typeof(bool), typeof(KsButton), 
			new PropertyMetadata(default(bool),(o,e)=>((KsButton)o).AtCanToggleChanged(new ValueChangedEventArgs<bool>(e))));

		public bool CanToggle {get { return (bool)GetValue(CanToggleProperty); }set { SetValue(CanToggleProperty, value); }}

		#endregion

		#region Popup Property
		/// <summary> The popup property
		/// </summary>
		public static readonly DependencyProperty PopupProperty =
			DependencyProperty.Register("Popup", typeof(Popup), typeof(KsButton), 
			new FrameworkPropertyMetadata(default(Popup),(d, args) => ((KsButton)d).AtPopupChanged(args)));

		/// <summary> Gets or sets the popup.
		/// </summary>
		/// <value>
		/// The popup.
		/// </value>
		public Popup Popup {
			get { return (Popup)GetValue(PopupProperty); }
			set { SetValue(PopupProperty, value); }
		}
		#endregion

		#region VisualStateConfiguration

		public static readonly DependencyProperty VisualStateConfigurationProperty =
			DependencyProperty.Register("VisualStateConfiguration", typeof(List<KsButtonVisualState>), typeof(KsButton), new PropertyMetadata(null));

		private DateTime _popupClosingTime;

		public List<KsButtonVisualState> VisualStateConfiguration {get { return (List<KsButtonVisualState>)GetValue(VisualStateConfigurationProperty); }set { SetValue(VisualStateConfigurationProperty, value); }}

		#endregion

		/// <summary> Initializes a new instance of the <see cref="KsButton"/> class.
		/// </summary>
		public KsButton() {
			HasHeader = true;
		}

		private void AtPopupChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (Popup)e.OldValue;
			var newValue = (Popup)e.NewValue;
			if(oldValue!=null) oldValue.Closed-=AtPopupOnClosed;
			if(newValue!=null) newValue.Closed+=AtPopupOnClosed;
			IsChecked = newValue!=null && newValue.IsOpen;
		}

		private void AtPopupOnClosed(object sender, EventArgs eventArgs) {
			IsChecked = false;
			_popupClosingTime = DateTime.Now;
		}

		private void AtCanToggleChanged(ValueChangedEventArgs<bool> e) {
			if (!IsChecked.HasValue) IsChecked = false;
		}

		private void AtIsCheckedChanged(DependencyPropertyChangedEventArgs e) {
			var newValue = (bool?)e.NewValue;
			if(Popup!=null && newValue.HasValue) Popup.IsOpen = newValue.Value;
		}

		protected override void OnClick() {
			base.OnClick();

			if(Popup!=null && CanToggle) {
				if((DateTime.Now-_popupClosingTime).TotalMilliseconds<500) return;
				if(IsChecked==true) {
					Popup.IsOpen = false;
					IsChecked = false;
				} else {
					Popup.PlacementTarget = this;
					Popup.IsOpen = true;
					IsChecked = true;					
				}
			} else if(CanToggle) IsChecked = IsChecked!=true;
		}

		//static KsButton() {DefaultStyleKeyProperty.OverrideMetadata(typeof(KsButton),new FrameworkPropertyMetadata(typeof(KsButton)));}
	}
}

namespace KsWare.Presentation.ViewFramework.Controls.Appearance
{
	public class KsButtonVisualState:DependencyObject
	{

		#region Button

		public static readonly DependencyProperty TemplateProperty =
			DependencyProperty.Register("Template", typeof(ControlTemplate), typeof(KsButtonVisualState), new PropertyMetadata(default(ControlTemplate)));

		public ControlTemplate Template {
			get { return (ControlTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}

		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(object), typeof(KsButtonVisualState), new PropertyMetadata(default(object)));

		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public static readonly DependencyProperty ContentStringFormatProperty =
			DependencyProperty.Register("ContentStringFormat", typeof(string), typeof(KsButtonVisualState), new PropertyMetadata(default(string)));

		public string ContentStringFormat {
			get { return (string)GetValue(ContentStringFormatProperty); }
			set { SetValue(ContentStringFormatProperty, value); }
		}

		public static readonly DependencyProperty ContentTemplateProperty =
			DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(KsButtonVisualState), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}

		public static readonly DependencyProperty ContentTemplateSelectorProperty =
			DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector ), typeof(KsButtonVisualState), new PropertyMetadata(default(DataTemplateSelector )));

		public DataTemplateSelector  ContentTemplateSelector {
			get { return (DataTemplateSelector )GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}

		#endregion
		
		#region FrameworkElement
		
		public static readonly DependencyProperty MarginProperty =
			DependencyProperty.Register("Margin", typeof(Thickness), typeof(KsButtonVisualState), new PropertyMetadata(default(Thickness)));

		public Thickness Margin {
			get { return (Thickness)GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}

		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height", typeof(double), typeof(KsButtonVisualState), new PropertyMetadata(default(double)));

		public double Height {
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}

		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof(double), typeof(KsButtonVisualState), new PropertyMetadata(default(double)));

		public double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		public static readonly DependencyProperty MinHeightProperty =
			DependencyProperty.Register("MinHeight", typeof(double), typeof(KsButtonVisualState), new PropertyMetadata(default(double)));

		public double MinHeight {
			get { return (double)GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}

		public static readonly DependencyProperty MaxHeightProperty =
			DependencyProperty.Register("MaxHeight", typeof(double), typeof(KsButtonVisualState), new PropertyMetadata(default(double)));

		public double MaxHeight {
			get { return (double)GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); }
		}

		public static readonly DependencyProperty MinWidthProperty =
			DependencyProperty.Register("MinWidth", typeof(double), typeof(KsButtonVisualState), new PropertyMetadata(default(double)));

		public double MinWidth {
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}

		public static readonly DependencyProperty MaxWidthProperty =
			DependencyProperty.Register("MaxWidth", typeof(double), typeof(KsButtonVisualState), new PropertyMetadata(default(double)));

		public double MaxWidth {
			get { return (double)GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}

		public static readonly DependencyProperty StyleProperty =
			DependencyProperty.Register("Style", typeof(Style), typeof(KsButtonVisualState), new PropertyMetadata(default(Style)));

		public Style Style {
			get { return (Style)GetValue(StyleProperty); }
			set { SetValue(StyleProperty, value); }
		}

		public static readonly DependencyProperty ToolTipProperty =
			DependencyProperty.Register("ToolTip", typeof(object), typeof(KsButtonVisualState), new PropertyMetadata(default(object)));

		public object ToolTip {
			get { return (object)GetValue(ToolTipProperty); }
			set { SetValue(ToolTipProperty, value); }
		}

		#endregion

		#region HeaderedContentControl

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (object), typeof (KsButtonVisualState), new PropertyMetadata(null));

		public object Header {
			get { return (object) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof (DataTemplate), typeof (KsButtonVisualState), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate HeaderTemplate {
			get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty HeaderStringFormatProperty =
			DependencyProperty.Register("HeaderStringFormat", typeof (string), typeof (KsButtonVisualState), new PropertyMetadata(default(string)));

		public string HeaderStringFormat {
			get { return (string) GetValue(HeaderStringFormatProperty); }
			set { SetValue(HeaderStringFormatProperty, value); }
		}

		public static readonly DependencyProperty HorizontalHeaderAlignmentProperty =
			DependencyProperty.Register("HorizontalHeaderAlignment", typeof (HorizontalAlignment), typeof (KsButtonVisualState), new PropertyMetadata(default(HorizontalAlignment)));

		public HorizontalAlignment HorizontalHeaderAlignment {
			get { return (HorizontalAlignment) GetValue(HorizontalHeaderAlignmentProperty); }
			set { SetValue(HorizontalHeaderAlignmentProperty, value); }
		}

		public static readonly DependencyProperty HeaderPaddingProperty =
			DependencyProperty.Register("HeaderPadding", typeof (Thickness), typeof (KsButtonVisualState), new PropertyMetadata(default(Thickness)));

		public Thickness HeaderPadding {
			get { return (Thickness) GetValue(HeaderPaddingProperty); }
			set { SetValue(HeaderPaddingProperty, value); }
		}

		public static readonly DependencyProperty VerticalHeaderAlignmentProperty =
			DependencyProperty.Register("VerticalHeaderAlignment", typeof (VerticalAlignment), typeof (KsButtonVisualState), new PropertyMetadata(default(VerticalAlignment)));

		public VerticalAlignment VerticalHeaderAlignment {
			get { return (VerticalAlignment) GetValue(VerticalHeaderAlignmentProperty); }
			set { SetValue(VerticalHeaderAlignmentProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateSelectorProperty =
			DependencyProperty.Register("HeaderTemplateSelector ", typeof (DataTemplateSelector), typeof (KsButtonVisualState), new PropertyMetadata(default(DataTemplateSelector)));

		public DataTemplateSelector HeaderTemplateSelector  {
			get { return (DataTemplateSelector) GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}

		#endregion

		#region ContentSource Property

		/// <summary> Provides the ContentSourceProperty
		/// </summary>
		public static readonly DependencyProperty ContentSourceProperty = DependencyProperty.Register(
			"ContentSource", typeof (Uri), typeof (KsButtonVisualState), new PropertyMetadata(null));

		/// <summary> Gets or sets the source for <see cref="ContentControl.Content"/>.
		/// </summary>
		/// <value>The source uri.</value>
		/// <example>
		/// <c>ContentSource="pack://application:,,,/AssemblyName;component/Folder/File.ext"</c>
		/// </example>
		/// <example>
		/// using ContentSourceParameter0: <br/>
		/// <c>ContentSource="pack://application:,,,/AssemblyName;component/Folder/{0}"</c>
		/// </example>
		/// <remarks>
		/// <blockquote><b>Note: </b>Use the <see cref="ContentSource"/> property alternative to <see cref="ContentControl.Content"/>. Do not use both concurrently!</blockquote>
		/// The <see cref="ContentSource"/> property ist used to show content from ressources (xaml/baml or bitmap images).<br/>
		/// An <see cref="ResourcePresenter"/> (using a <see cref="UIElementConverter"/>) will be created and assigned to <see cref="ContentControl.Content"/>.
		/// </remarks>
		public Uri ContentSource {
			get {return (Uri) GetValue(ContentSourceProperty);}
			set {SetValue(ContentSourceProperty, value);}
		}

		public static readonly DependencyProperty ContentSourceParameter0Property =
			DependencyProperty.Register("ContentSourceParameter0", typeof(string), typeof(KsButtonVisualState), new PropertyMetadata(null));

		public string ContentSourceParameter0 {
			get { return (string)GetValue(ContentSourceParameter0Property); }
			set { SetValue(ContentSourceParameter0Property, value); }
		}

		public static readonly DependencyProperty ContentSourceParameter1Property =
			DependencyProperty.Register("ContentSourceParameter1", typeof(string), typeof(KsButtonVisualState), new PropertyMetadata(null));

		public string ContentSourceParameter1 {
			get { return (string)GetValue(ContentSourceParameter1Property); }
			set { SetValue(ContentSourceParameter1Property, value); }
		}

		public static readonly DependencyProperty ContentSourceParameter2Property =
			DependencyProperty.Register("ContentSourceParameter2", typeof(string), typeof(KsButtonVisualState), new PropertyMetadata(null));

		public string ContentSourceParameter2 {
			get { return (string)GetValue(ContentSourceParameter2Property); }
			set { SetValue(ContentSourceParameter2Property, value); }
		}

		#endregion

		#region HeaderSource Property

		/// <summary> Provides the HeaderSourceProperty
		/// </summary>
		public static readonly DependencyProperty HeaderSourceProperty = DependencyProperty.Register(
			"HeaderSource", typeof (Uri), typeof (KsButton), new PropertyMetadata(null));

		/// <summary> Gets or sets the source for <see cref="KsButton.Header"/>.
		/// </summary>
		/// <value>The source uri.</value>
		/// <example>
		/// HeaderSource="pack://application:,,,/AssemblyName;component/Folder/File.ext"/>
		/// </example>
		/// <remarks>
		/// <blockquote><b>Note: </b>Use the <see cref="HeaderSource"/> property alternative to <see cref="KsButton.Header"/>. Do not use both concurrently!</blockquote>
		/// The <see cref="HeaderSource"/> property ist used to show content from ressources (xaml/baml or bitmap images).<br/>
		/// An <see cref="ResourcePresenter"/> (using a <see cref="UIElementConverter"/>) will be created and assigned to <see cref="KsButton.Header"/>.
		/// </remarks>
		public Uri HeaderSource {
			get {return (Uri) GetValue(HeaderSourceProperty);}
			set {SetValue(HeaderSourceProperty, value);}
		}
		
		public static readonly DependencyProperty HeaderSourceParameter0Property =
			DependencyProperty.Register("HeaderSourceParameter0", typeof(string), typeof(KsButton), new PropertyMetadata(null));

		public string HeaderSourceParameter0 {get { return (string)GetValue(HeaderSourceParameter0Property); }set { SetValue(HeaderSourceParameter0Property, value); }}

		public static readonly DependencyProperty HeaderSourceParameter1Property =
			DependencyProperty.Register("HeaderSourceParameter1", typeof(string), typeof(KsButton), new PropertyMetadata(null));

		public string HeaderSourceParameter1 {get { return (string)GetValue(HeaderSourceParameter1Property); }set { SetValue(HeaderSourceParameter1Property, value); }}

		public static readonly DependencyProperty HeaderSourceParameter2Property =
			DependencyProperty.Register("HeaderSourceParameter2", typeof(string), typeof(KsButton), new PropertyMetadata(null));

		public string HeaderSourceParameter2 {get { return (string)GetValue(HeaderSourceParameter2Property); }set { SetValue(HeaderSourceParameter2Property, value); }}

		#endregion

		#region Conditions

		public KsButtonVisualStateCondition<bool?> IsChecked { get; set; }
		//ButtonBase
		public KsButtonVisualStateCondition<bool> IsPressed { get; set; } 
		//UIElement
		public KsButtonVisualStateCondition<bool> IsEnabled { get; set; }
		public KsButtonVisualStateCondition<bool> IsFocused { get; set; }
		public KsButtonVisualStateCondition<bool> IsKeybordFocused { get; set; }
		public KsButtonVisualStateCondition<bool> IsKeyboardFocusWithin  { get; set; }
		public KsButtonVisualStateCondition<bool> IsMouseCaptured { get; set; }
		public KsButtonVisualStateCondition<bool> IsMouseCaptureWithin { get; set; }
		public KsButtonVisualStateCondition<bool> IsMouseDirectlyOver   { get; set; }
		public KsButtonVisualStateCondition<bool> IsMouseOver { get; set; }
		public KsButtonVisualStateCondition<bool> IsStylusCaptured { get; set; }
		public KsButtonVisualStateCondition<bool> IsStylusCaptureWithin { get; set; }
		public KsButtonVisualStateCondition<bool> IsStylusDirectlyOver { get; set; }
		public KsButtonVisualStateCondition<bool> IsStylusOver { get; set; }
		public KsButtonVisualStateCondition<bool> IsVisible { get; set; }

		#endregion
	}

	public class KsButtonVisualStateCondition<T>
	{
		public string Property { get; set; }
		public bool IsEnabled { get; set; }
		public T Value { get; set; }
	}
}
