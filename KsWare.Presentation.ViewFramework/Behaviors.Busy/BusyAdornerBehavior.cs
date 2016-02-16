using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	// based on article: "As though I did BusyIndicator" http://sysmagazine.com/posts/142243/

	public class BusyAdornerBehavior {

		#region InternalData Property
		public static readonly DependencyProperty InternalDataProperty = DependencyProperty.RegisterAttached(
			"InternalData", typeof (BusyAdornerBehaviorData), typeof (BusyAdornerBehavior), new PropertyMetadata(default(BusyAdornerBehaviorData)));

		public static void SetInternalData(FrameworkElement element, BusyAdornerBehaviorData value) { element.SetValue(InternalDataProperty, value); }

		public static BusyAdornerBehaviorData GetInternalData(FrameworkElement element) { return (BusyAdornerBehaviorData) element.GetValue(InternalDataProperty); }
		#endregion InternalData Property

		#region IsBusy Property

		public static readonly DependencyProperty IsBusyProperty = DependencyProperty.RegisterAttached(
			"IsBusy", typeof (bool), typeof (BusyAdornerBehavior), new PropertyMetadata(default(bool),(d,e) => AtIsBusyChanged((FrameworkElement) d,e)));

		public static void SetIsBusy(FrameworkElement element, bool value) {
			ApplicationDispatcher.Wrapper.InvokeIfRequired(element.SetValue, IsBusyProperty, (object)value);
		}

		public static bool GetIsBusy(FrameworkElement element) { return (bool) element.GetValue(IsBusyProperty); }
		#endregion

		#region BindToBusyUserRequest Property
		public static readonly DependencyProperty BindToBusyUserRequestProperty = DependencyProperty.RegisterAttached(
			"BindToBusyUserRequest", typeof (bool), typeof (BusyAdornerBehavior), new PropertyMetadata(default(bool),(d,e) => AtBindToBusyUserRequestChanged((FrameworkElement)d,e)));

		public static void SetBindToBusyUserRequest(FrameworkElement element, bool value) { element.SetValue(BindToBusyUserRequestProperty, value); }

		public static bool GetBindToBusyUserRequest(FrameworkElement element) { return (bool) element.GetValue(BindToBusyUserRequestProperty); }
		#endregion /BindToBusyUserRequest Property

		#region DataContext Property
		public static readonly DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached(
			"DataContext", typeof (ObjectVM), typeof (BusyAdornerBehavior), new PropertyMetadata(default(ObjectVM),(d,e) => AtDataContextChanged((FrameworkElement)d,e)));

		public static void SetDataContext(FrameworkElement element, bool value) { element.SetValue(DataContextProperty, value); }

		public static ObjectVM GetDataContext(FrameworkElement element) { return (ObjectVM) element.GetValue(DataContextProperty); }
		#endregion /DataContext Property

		#region Control.Background Property

		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
			"Background", typeof (Brush), typeof (BusyAdornerBehavior), new PropertyMetadata(default(Brush)));

		public static void SetBackground(FrameworkElement element, Brush value) { element.SetValue(BackgroundProperty, value); }

		public static Brush GetBackground(FrameworkElement element) { return (Brush) element.GetValue(BackgroundProperty); }
	
		#endregion Control.Background Property

		#region Control.Style Property

		public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached(
			"Style", typeof (Style), typeof (BusyAdornerBehavior), new PropertyMetadata(default(Style)));

		public static void SetStyle(FrameworkElement element, Style value) { element.SetValue(StyleProperty, value); }

		public static Style GetStyle(FrameworkElement element) { return (Style) element.GetValue(StyleProperty); }
	
		#endregion /Control.Style Property

		private static void AtBindToBusyUserRequestChanged(FrameworkElement d, DependencyPropertyChangedEventArgs e) {
			var data = GetInternalData(d);
			if (data == null) { data=new BusyAdornerBehaviorData(d); SetInternalData(d,data);}
			var newValue = (bool)e.NewValue;
			data.BindToBusyUserRequest = newValue;
		}

		private static void AtDataContextChanged(FrameworkElement d, DependencyPropertyChangedEventArgs e) {
			var data = GetInternalData(d);
			if (data == null) { data=new BusyAdornerBehaviorData(d); SetInternalData(d,data);}
			var newValue = (ObjectVM)e.NewValue;
			data.DataContext = newValue;
		}

		private static void AtIsBusyChanged(FrameworkElement d, DependencyPropertyChangedEventArgs e) {
			var data = GetInternalData(d);
			if (data == null) { data=new BusyAdornerBehaviorData(d); SetInternalData(d,data);}
			var newValue = (bool)e.NewValue;

			if (newValue) {
				var visual = new BusyAdornerVisual {
					Style      = GetStyle(d),
					Background = GetBackground(d),
				};
				data.Adorner = new BusyAdorner(d,visual);
			} else {
				data.Adorner = null;
			}
		}

	}

	public class BusyAdornerBehaviorData {

		private readonly FrameworkElement m_DependencyObject;
		private Adorner m_Adorner;
		private AdornerLayer m_AdornerLayer;
		private bool m_BindToBusyUserRequest;
		private ObjectVM m_DataContext;

		public BusyAdornerBehaviorData(FrameworkElement dependencyObject) {
			m_DependencyObject = dependencyObject;
			m_DependencyObject.Unloaded += AtFrameworkElementUnloaded;
//			m_AdornerLayer=AdornerLayer.GetAdornerLayer(m_DependencyObject);
//			if (m_AdornerLayer == null) {
//				m_DependencyObject.Initialized += InitAdornerLayer; // 1. 
//				m_DependencyObject.SizeChanged += InitAdornerLayer; // 2. has AdornerLayer
//				m_DependencyObject.Loaded      += InitAdornerLayer; // 3. ..
//			}
		}

		private void AtDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) { A(); }

		private void InitAdornerLayer(object sender, EventArgs eventArgs) {
			if(m_AdornerLayer!=null) return;
			m_AdornerLayer=AdornerLayer.GetAdornerLayer(m_DependencyObject);
//			if (m_AdornerLayer == null && m_DependencyObject is ContentControl /*Window*/) {
//				var visual = ((ContentControl) m_DependencyObject).Content as Visual;
//				if(visual==null) return;
//				m_AdornerLayer=AdornerLayer.GetAdornerLayer(visual);
//				if (m_AdornerLayer == null) {
//					var control = visual as FrameworkElement;
//					if(control==null) return;
//					if (!control.IsLoaded) control.Loaded += InitAdornerLayer;					
//				}
//			}
			if(m_AdornerLayer==null) return;
			m_DependencyObject.Loaded      -= InitAdornerLayer;
			m_DependencyObject.Initialized -= InitAdornerLayer;
			m_DependencyObject.SizeChanged -= InitAdornerLayer;
			if (m_Adorner != null) AttachAdorner();
		}

		private void AtFrameworkElementUnloaded(object sender, RoutedEventArgs e) {
			DetachAdorner();
		}

		public Adorner Adorner {
			get { return m_Adorner; }
			set {
				DetachAdorner();
				m_Adorner = value;
				if (value != null) { AttachAdorner(); }
			}
		}

		public bool BindToBusyUserRequest {
			get { return m_BindToBusyUserRequest; }
			set {
				m_BindToBusyUserRequest = value;
				if (value) {
					A();
					m_DependencyObject.DataContextChanged+=AtDataContextChanged;
				} else {
					m_DependencyObject.DataContextChanged-=AtDataContextChanged;
				}
			}
		}

		public ObjectVM DataContext {
			get { return m_DataContext; }
			set {
				if (m_DataContext != null) {
					m_DataContext.UserFeedbackRequested-=AtUserFeedbackRequested;
					m_BindToBusyUserRequest = false;
				}
				m_DataContext = value;
				if (m_DataContext != null) {
					m_DataContext.UserFeedbackRequested+=AtUserFeedbackRequested;
					m_BindToBusyUserRequest = true;
				}
			}
		}

		private void A() {
			var vm=m_DependencyObject.DataContext as ObjectVM;
			if (vm != null && m_BindToBusyUserRequest) {
//				vm.UserFeedbackRequestedEvent.Register(this,"AtUserFeedbackRequested", AtUserFeedbackRequested);
				vm.UserFeedbackRequested+=AtUserFeedbackRequested;
			}
		}

		private void AtUserFeedbackRequested(object sender, UserFeedbackEventArgs e) {
			if (!(e is BusyUserFeedbackEventArgs)) return;
			BusyAdornerBehavior.SetIsBusy(m_DependencyObject, ((BusyUserFeedbackEventArgs) e).IsBusy);
			e.Handled = true;
		}

		private void AttachAdorner() {
			if (m_Adorner == null) return;
			if(m_AdornerLayer==null) m_AdornerLayer=AdornerLayer.GetAdornerLayer(m_DependencyObject);

			#region Fallback uses m_DependencyObject.Content
//			if (m_AdornerLayer == null && m_DependencyObject is ContentControl /*Window*/) {
//				var visual = ((ContentControl) m_DependencyObject).Content as Visual;
//				if(visual==null) return;
//				m_AdornerLayer=AdornerLayer.GetAdornerLayer(visual);
//			}
			// now m_AdornerLayer is not null, but the adorner will not be visible! why ever?
			#endregion

			if (m_AdornerLayer==null) return;
			m_AdornerLayer.Add(m_Adorner);
		}

		private void DetachAdorner() {
			if (m_AdornerLayer != null && m_Adorner != null) { m_AdornerLayer.Remove(m_Adorner); }
		}

	}

	public class BusyAdorner : Adorner {

		private Control m_VisualChild;

		/// <summary> Initializes a new instance of the <see cref="T:System.Windows.Documents.Adorner" /> class.
		/// </summary>
		/// <param name="adornedElement">The element to bind the adorner to.</param>
		public BusyAdorner(UIElement adornedElement) : base(adornedElement) {
			//Loaded += AtLoaded;
			VisualChild=new BusyAdornerVisual();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BusyAdorner"/> class.
		/// </summary>
		/// <param name="adornedElement">The adorned element.</param>
		/// <param name="visualChild">The visual child.</param>
		public BusyAdorner(FrameworkElement adornedElement, BusyAdornerVisual visualChild) : base(adornedElement) {
			VisualChild = visualChild;
		}

		/// <summary> Gets the number of visual child elements within this element. Overrides <see cref="M:System.Windows.Media.Visual.VisualChildrenCount" />
		/// </summary>
		/// <value>The visual children count.</value>
		protected override int VisualChildrenCount{get{return 1;}}

		/// <summary> Returns a child at the specified index from a collection of child elements. Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />
		/// </summary>
		/// <param name="index">The zero-based index of the requested child element in the collection.</param>
		/// <returns>The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		protected override Visual GetVisualChild(int index){if (index != 0) throw new ArgumentOutOfRangeException();return m_VisualChild;}

		/// <summary> Gets or sets the visual child.
		/// </summary>
		/// <value>The visual child.</value>
		public Control VisualChild {
			get { return m_VisualChild; }
			set {
				if (m_VisualChild != null) { RemoveVisualChild(m_VisualChild); }
				m_VisualChild = value;
				if (m_VisualChild != null) { AddVisualChild(m_VisualChild); }
			}
		}

		protected override Size MeasureOverride(Size constraint) {
			if (m_VisualChild == null) return base.MeasureOverride(constraint);
			m_VisualChild.Measure(constraint);
//			return m_Child.DesiredSize;
			return AdornedElement.RenderSize;
//			return constraint;
		}

		protected override Size ArrangeOverride(Size finalSize) {
			if (m_VisualChild == null) return base.ArrangeOverride(finalSize);
			m_VisualChild.Arrange(new Rect(new Point(0, 0), finalSize));
			return new Size(m_VisualChild.ActualWidth, m_VisualChild.ActualHeight);
		}

		private void AtLoaded(object sender, RoutedEventArgs e) {
			var myDoubleAnimation = new DoubleAnimation {
				From = 1.0,
				To = 0.0,
				Duration = new Duration(TimeSpan.FromSeconds(1)),
				AutoReverse = true,
				RepeatBehavior = RepeatBehavior.Forever
			};

            var myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTarget(myStoryboard, this);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(OpacityProperty));

            myStoryboard.Begin(this);
		}

//		protected override void OnRender(DrawingContext drawingContext) {
//			var adornedControl = this.AdornedElement as FrameworkElement;
//
//			if (adornedControl == null) return;
//
//			Rect rect = new Rect(0, 0, adornedControl.ActualWidth, adornedControl.ActualHeight);
//
//			// Some arbitrary drawing implements.
//			SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
//			renderBrush.Opacity = 0.2;
//			Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
//			double renderRadius = 5.0;
//
//			double dist = 15;
//			double cntrX = rect.Width/2;
//			double cntrY = rect.Height/2;
//			double left = cntrX - dist;
//			double right = cntrX + dist;
//			double top = cntrY - dist;
//			double bottom = cntrY + dist;
//
////			// Draw four circles near to center.
//			drawingContext.PushTransform(new RotateTransform(45, cntrX, cntrY));
//
//			drawingContext.DrawEllipse(renderBrush, renderPen, new Point {X = left, Y = top}, renderRadius, renderRadius);
//			drawingContext.DrawEllipse(renderBrush, renderPen, new Point {X = right, Y = top}, renderRadius, renderRadius);
//			drawingContext.DrawEllipse(renderBrush, renderPen, new Point {X = right, Y = bottom}, renderRadius, renderRadius);
//			drawingContext.DrawEllipse(renderBrush, renderPen, new Point {X = left, Y = bottom}, renderRadius, renderRadius);
//
//
//		}

	}

}
