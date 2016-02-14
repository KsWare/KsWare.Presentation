using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace KsWare.Presentation.ViewFramework.Behaviors.DragDrop {

	public class ListBoxAdorner : Adorner {

		private AdornerLayer m_AdornerLayer;

		public bool IsAboveElement { get; set; }

		public ListBoxAdorner(UIElement adornedElement, AdornerLayer adornerLayer) : base(adornedElement) {
			this.m_AdornerLayer = adornerLayer;
			this.m_AdornerLayer.Add(this);
		}

		/// <summary> Update UI
		/// </summary>
		internal void Update() {
			m_AdornerLayer.Update(this.AdornedElement);
			Visibility = Visibility.Visible;
		}

		public void Remove() {
			Visibility = System.Windows.Visibility.Collapsed;
		}

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext) {
//            double width = this.AdornedElement.DesiredSize.Width;
//            double height = this.AdornedElement.DesiredSize.Height;

			var adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

			var renderBrush = new SolidColorBrush(Colors.Red);
			renderBrush.Opacity = 0.5;
			var renderPen = new Pen(new SolidColorBrush(Colors.White), 1.5);
			double renderRadius = 5.0;

			if (IsAboveElement) {
			    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
			    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
			} else {
			    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
			    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
			}
		}
	}
}
