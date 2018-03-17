using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace KsWare.Presentation.ViewFramework.Controls
{

	public class DataContextErrorAdorner:ContentControl
	{

		private RectangleFrameworkElementAdorner _adorner;
		private Type _adornerType;

		public DataContextErrorAdorner() {
			DataContextChanged += AtDataContextChanged;
			AdornerType = typeof(RectangleFrameworkElementAdorner);
		}

		private RectangleFrameworkElementAdorner Adorner {
			get {
				if(_adorner!=null) return _adorner;
				var adornerLayer = AdornerLayer.GetAdornerLayer(this);
				if(adornerLayer==null) return null;
				_adorner = (RectangleFrameworkElementAdorner) Activator.CreateInstance(AdornerType,this,adornerLayer);
				return _adorner;
			}
		}

		private void AtDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(Adorner!=null)Adorner.Update();
//			if(e.NewValue==null) {
//				_adorner.Update();
//			} else {
//				_adorner.Remove();
//			}
		}

		public Type AdornerType {
			get => _adornerType;
			set {
				if(!typeof(RectangleFrameworkElementAdorner).IsAssignableFrom(value)) throw new InvalidOperationException("Invalid type of adorner. Type of RectangleFrameworkElementAdorner expected.");
				_adornerType = value;
			}
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static DataContextErrorAdorner() {
//			DefaultStyleKeyProperty.OverrideMetadata(typeof(DataContextErrorAdorner),new FrameworkPropertyMetadata(typeof(DataContextErrorAdorner)));
		}

	}

	public class RectangleFrameworkElementAdorner:Adorner
	{

		private readonly AdornerLayer _adornerLayer;

		public bool HasError { get; set; }

		public string Text { get; set; }

		public RectangleFrameworkElementAdorner(UIElement adornedElement, AdornerLayer adornerLayer):base(adornedElement) {
			_adornerLayer = adornerLayer;
			_adornerLayer.Add(this);
		}

		/// <summary>
		/// Update UI
		/// </summary>
		internal void Update() {
			_adornerLayer.Update(AdornedElement);
			Visibility = Visibility.Visible;
		}

		public void Remove() {
			Visibility = Visibility.Collapsed;
		}

		protected override void OnRender(DrawingContext dc) {
//			double width = AdornedElement.DesiredSize.Width;
//			double height = AdornedElement.DesiredSize.Height;

			var adornedElementRect = new Rect(AdornedElement.DesiredSize);

//			var renderBrush = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0));
			var renderPen1 = new Pen(new SolidColorBrush(Color.FromArgb(255,255,0,0)), 1.5);
			var renderPen2 = new Pen(new SolidColorBrush(Color.FromArgb(128,255,0,0)), 2.5);
			var renderPen3 = new Pen(new SolidColorBrush(Color.FromArgb(64,255,0,0)), 3.5);

			if(HasError) {
				dc.DrawRectangle(null, renderPen1, adornedElementRect);
				adornedElementRect.Inflate(-2, -2);
				dc.DrawRectangle(null, renderPen2, adornedElementRect);
				adornedElementRect.Inflate(-3, -3);
				dc.DrawRectangle(null, renderPen3, adornedElementRect);

				dc.DrawGeometry(new SolidColorBrush(Colors.Yellow),new Pen(new SolidColorBrush(Colors.Black),1 ),(Geometry)GeometryConverter.ConvertFrom("M0,7 L8,7 L4,0z"));
			} else {
				dc.DrawRectangle(null, new Pen(new SolidColorBrush(Color.FromArgb(255,0,128,0)), 1.5), adornedElementRect);
			}

			var textPos = adornedElementRect.TopLeft;textPos.Offset(8,0);
			var typeface=new Typeface(new FontFamily("Sergeo UI"),FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
			var textBrush = Brushes.Black;
			if(!string.IsNullOrWhiteSpace(Text)) dc.DrawText(new FormattedText(Text,CultureInfo.CurrentCulture,FlowDirection.LeftToRight, typeface,12,textBrush), textPos);
		}

		protected readonly GeometryConverter GeometryConverter=new GeometryConverter();

	}

}
