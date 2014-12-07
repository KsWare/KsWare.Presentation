using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using KsWare.Presentation.Core;
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

		private static void AtBindToBusyUserRequestChanged(FrameworkElement d, DependencyPropertyChangedEventArgs e) {
			var data = GetInternalData(d);
			if (data == null) { data=new BusyAdornerBehaviorData(d); SetInternalData(d,data);}
			var newValue = (bool)e.NewValue;
			data.BindToBusyUserRequest = newValue;
		}

		private static void AtIsBusyChanged(FrameworkElement d, DependencyPropertyChangedEventArgs e) {
			var data = GetInternalData(d);
			if (data == null) { data=new BusyAdornerBehaviorData(d); SetInternalData(d,data);}
			var newValue = (bool)e.NewValue;
			data.Adorner = newValue ? new BusyAdorner(d) : null;
		}

	}

	public class BusyAdornerBehaviorData {

		private readonly FrameworkElement m_DependencyObject;
		private Adorner m_Adorner;
		private AdornerLayer m_AdornerLayer;
		private bool m_BindToBusyUserRequest;

		public BusyAdornerBehaviorData(FrameworkElement dependencyObject) {
			m_DependencyObject = dependencyObject;
			m_DependencyObject.Unloaded += AtFrameworkElementUnloaded;
			m_AdornerLayer=AdornerLayer.GetAdornerLayer(m_DependencyObject);
			if (m_AdornerLayer == null) {
				m_DependencyObject.Initialized += InitAdornerLayer; // 1. 
				m_DependencyObject.SizeChanged += InitAdornerLayer; // 2. has AdornerLayer
				m_DependencyObject.Loaded      += InitAdornerLayer; // 3. ..
			}
		}

		private void AtDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) { A(); }

		private void InitAdornerLayer(object sender, EventArgs eventArgs) {
			if(m_AdornerLayer!=null) return;
			m_AdornerLayer=AdornerLayer.GetAdornerLayer(m_DependencyObject);
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
			if(m_AdornerLayer==null) return;
			m_AdornerLayer.Add(m_Adorner);
		}

		private void DetachAdorner() {
			if (m_AdornerLayer != null && m_Adorner != null) { m_AdornerLayer.Remove(m_Adorner); }
		}

	}

	public class BusyAdorner : Adorner {

		private Control m_Child;

		public BusyAdorner(UIElement adornedElement) : base(adornedElement) {
			//Loaded += AtLoaded;
			Child=new BusyAdornerVisual();
		}

		protected override int VisualChildrenCount{get{return 1;}}
		protected override Visual GetVisualChild(int index){if (index != 0) throw new ArgumentOutOfRangeException();return m_Child;}

		public Control Child {
			get { return m_Child; }
			set {
				if (m_Child != null) { RemoveVisualChild(m_Child); }
				m_Child = value;
				if (m_Child != null) { AddVisualChild(m_Child); }
			}
		}

		protected override Size MeasureOverride(Size constraint) {
			if (m_Child == null) return base.MeasureOverride(constraint);
			m_Child.Measure(constraint);
//			return m_Child.DesiredSize;
			return constraint;
		}

		protected override Size ArrangeOverride(Size finalSize) {
			if (m_Child == null) return base.ArrangeOverride(finalSize);
			m_Child.Arrange(new Rect(new Point(0, 0), finalSize));
			return new Size(m_Child.ActualWidth, m_Child.ActualHeight);
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
