using System;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace KsWare.Presentation.ViewFramework.Behaviors.DragDrop {

	public class FrameworkElementRedGreenAdorner : Adorner {

		private AdornerLayer _AdornerLayer;

		public FrameworkElementRedGreenAdorner(UIElement adornedElement) : base(adornedElement) {
			IsHitTestVisible = false;
			_AdornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
			_AdornerLayer.Add(this);
		}

		public DropPosition Position { get; set; }

		public DragDropEffects DropEffect { get; set; }

		internal void Update() {
			_AdornerLayer.Update(AdornedElement);
			Visibility = Visibility.Visible;
		}

		public void Remove() {
			Visibility = Visibility.Collapsed;
		}

		const    double renderRadius = 5.0;
		readonly Brush  edgeBrushNone = new SolidColorBrush(Colors.Red){Opacity = 0.5};
		readonly Brush  edgeBrushMove = new SolidColorBrush(Colors.LimeGreen){Opacity = 0.5};
		readonly Pen    linePenNone   = new Pen(new SolidColorBrush(Colors.Red), 1.5);
		readonly Pen    linePenMove   = new Pen(new SolidColorBrush(Colors.LimeGreen), 1.5);
		readonly Pen    edgePen = new Pen(new SolidColorBrush(Colors.White), 1.5);
		readonly Pen    edgePen2 = new Pen(new SolidColorBrush(Colors.White), 2.5);

		protected override void OnRender(DrawingContext dc) {
			var adornedElementRect = new Rect(AdornedElement.DesiredSize);
			
			Brush edgeBrush;
			Pen linePen;

			switch (DropEffect) {
				case DragDropEffects.None: linePen=linePenNone; break;
				case DragDropEffects.Move: linePen=linePenMove; break;
				default: goto case DragDropEffects.Move;
			}

			switch (DropEffect) {
				case DragDropEffects.None: edgeBrush=edgeBrushNone; break;
				case DragDropEffects.Move: edgeBrush=edgeBrushMove; break;
				default: goto case DragDropEffects.Move;
			}

//			// Draw a circle at each corner.
//			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
//			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
//			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
//			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
			var gc = new GeometryConverter();
			switch (Position) {
				case DropPosition.Top:
					//dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.TopLeft, renderRadius, renderRadius);
					
					//dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.TopRight, renderRadius, renderRadius);
					dc.DrawGeometry(edgeBrush,linePen,(Geometry) gc.ConvertFrom("M"+(adornedElementRect.TopRight.X).ToStringEnUs()+","+((int)adornedElementRect.TopRight.Y-3).ToStringEnUs()+" l0,6 l-5,-3 z"));
					dc.DrawLine(linePen,adornedElementRect.TopLeft, adornedElementRect.TopRight);
					if (DropEffect == DragDropEffects.None) {
						dc.DrawEllipse(edgeBrush, edgePen, new Point(adornedElementRect.TopLeft.X,adornedElementRect.TopLeft.Y), 6, 6);
						dc.DrawLine(edgePen2,new Point(adornedElementRect.TopLeft.X-3,adornedElementRect.TopLeft.Y), new Point(adornedElementRect.TopLeft.X+3,adornedElementRect.TopLeft.Y));
					} else {
						dc.DrawGeometry(edgeBrush,linePen,(Geometry) gc.ConvertFrom("M"+(adornedElementRect.TopLeft.X).ToStringEnUs()+","+(adornedElementRect.TopLeft.Y-3).ToStringEnUs()+" l0,6 l5,-3 z"));
					}
					break;
				case DropPosition.Bottom:
					//dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
					
					//dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.BottomRight, renderRadius, renderRadius);
					dc.DrawGeometry(edgeBrush,linePen,(Geometry) gc.ConvertFrom("M"+(adornedElementRect.BottomRight.X).ToStringEnUs()+","+((int)adornedElementRect.BottomRight.Y-3).ToStringEnUs()+" l0,6 l-5,-3 z"));
					dc.DrawLine(linePen,adornedElementRect.BottomLeft, adornedElementRect.BottomRight);

					if (DropEffect == DragDropEffects.None) {
						dc.DrawEllipse(edgeBrush, edgePen, new Point(adornedElementRect.BottomLeft.X,adornedElementRect.BottomLeft.Y), 6, 6);
						dc.DrawLine(edgePen2,new Point(adornedElementRect.BottomLeft.X-3,adornedElementRect.BottomLeft.Y), new Point(adornedElementRect.BottomLeft.X+3,adornedElementRect.BottomLeft.Y));
					} else {
						dc.DrawGeometry(edgeBrush,linePen,(Geometry) gc.ConvertFrom("M"+(adornedElementRect.BottomLeft.X).ToStringEnUs()+","+(adornedElementRect.BottomLeft.Y-3).ToStringEnUs()+" l0,6 l5,-3 z"));
					}
					break;
				case DropPosition.Left:
					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.TopLeft, renderRadius, renderRadius);
					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
					dc.DrawLine(linePen,adornedElementRect.TopLeft, adornedElementRect.BottomLeft);
					break;
				case DropPosition.Right:
					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.TopRight, renderRadius, renderRadius);
					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.BottomRight, renderRadius, renderRadius);
					dc.DrawLine(linePen,adornedElementRect.TopRight, adornedElementRect.BottomRight);
					break;
				case DropPosition.BottomLeft:
					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
					break;
				case DropPosition.BottomRight:
					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.BottomRight, renderRadius, renderRadius);
					break;
					case DropPosition.TopLeft:
					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.TopLeft, renderRadius, renderRadius);
					break;
				case DropPosition.TopRight:
					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.TopRight, renderRadius, renderRadius);
					break;
				case DropPosition.Over:
					
//					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.TopLeft, renderRadius, renderRadius);
//					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.TopRight, renderRadius, renderRadius);
//					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
//					dc.DrawEllipse(edgeBrush, edgePen, adornedElementRect.BottomRight, renderRadius, renderRadius);
					dc.DrawRectangle(null, linePen,adornedElementRect);

					if (DropEffect == DragDropEffects.None) {
//						dc.DrawEllipse(edgeBrush, edgePen, new Point(adornedElementRect.TopLeft.X,adornedElementRect.TopLeft.Y), 6, 6);
//						dc.DrawLine(edgePen2,new Point(adornedElementRect.TopLeft.X-3,adornedElementRect.TopLeft.Y), new Point(adornedElementRect.TopLeft.X+3,adornedElementRect.TopLeft.Y));

//						dc.DrawEllipse(Brushes.White, edgePen, new Point(adornedElementRect.TopLeft.X,adornedElementRect.TopLeft.Y), 6, 6);
//						dc.DrawEllipse(Brushes.White,  new Pen(new SolidColorBrush(Colors.Red), 1.5), new Point(adornedElementRect.TopLeft.X,adornedElementRect.TopLeft.Y), 4, 4);
//						dc.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 1.5),new Point(adornedElementRect.TopLeft.X-3,adornedElementRect.TopLeft.Y-3), new Point(adornedElementRect.TopLeft.X+3,adornedElementRect.TopLeft.Y+3));

						dc.DrawEllipse(Brushes.White, edgePen, new Point(adornedElementRect.TopLeft.X+3.5,adornedElementRect.TopLeft.Y+4), 6, 6);
						dc.DrawEllipse(Brushes.White,  new Pen(new SolidColorBrush(Colors.Red), 1.5), new Point(adornedElementRect.TopLeft.X+4,adornedElementRect.TopLeft.Y+4), 4, 4);
						dc.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 1.5),new Point(adornedElementRect.TopLeft.X+1,adornedElementRect.TopLeft.Y+1), new Point(adornedElementRect.TopLeft.X+7,adornedElementRect.TopLeft.Y+7));
					}
					break;
			}

//			drawingContext.DrawRectangle(renderBrush,renderPen,adornedElementRect);
		}

	}
}
