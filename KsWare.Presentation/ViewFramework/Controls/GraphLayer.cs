using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KsWare.Presentation.ViewFramework.Controls
{

	public abstract class GraphLayer:Control
	{

		public static readonly DependencyProperty LogicalBoundsProperty =
			DependencyProperty.Register("LogicalBounds", typeof(Rect), typeof(GraphLayer), new FrameworkPropertyMetadata(default(Rect)){AffectsRender = true});

		public Rect LogicalBounds {
			get { return (Rect)GetValue(LogicalBoundsProperty); }
			set { SetValue(LogicalBoundsProperty, value); }
		}

		private static readonly DependencyPropertyKey __actualLogicalBoundsPropertyKey =
			DependencyProperty.RegisterReadOnly("ActualLogicalBounds", typeof(Rect), typeof(GraphLayer), new PropertyMetadata(default(Rect)));

		public static readonly DependencyProperty ActualLogicalBoundsProperty = __actualLogicalBoundsPropertyKey.DependencyProperty;

		public Rect ActualLogicalBounds {
			get { return (Rect)GetValue(__actualLogicalBoundsPropertyKey.DependencyProperty); }
			protected set { SetValue(__actualLogicalBoundsPropertyKey, value); }
		}

		protected GraphLayer() {}

		public void Refresh() {
			InvalidateVisual();
		}

		protected abstract override void OnRender(DrawingContext dc);

		protected double CDoubleX(double value) {return NV(value)?value:((value-ActualLogicalBounds.Left)*(ActualWidth/ActualLogicalBounds.Width));}
		protected double CDoubleW(double value) {return NV(value)?value:((value)*(ActualWidth/ActualLogicalBounds.Width));}
		protected double CDoubleY(double value) {return NV(value)?value:(ActualHeight-((value-ActualLogicalBounds.Top)*(ActualHeight/ActualLogicalBounds.Height)));}
		protected double CDoubleH(double value) {return NV(value)?value:((value)*(ActualHeight/ActualLogicalBounds.Height));}
		protected Size CSize(Size size){return new Size(CDoubleW(size.Width),CDoubleH(size.Height));}
		protected Point CPoint(Point point) {return new Point(CDoubleX(point.X),CDoubleY(point.Y));}
		protected Rect CRect(Rect rect) {
			return new Rect(
				CDoubleX(rect.Left),
				CDoubleY(rect.Top)-CDoubleH(rect.Height),
				CDoubleW(rect.Width),
				CDoubleH(rect.Height)
			);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		protected bool NV(double value) {
			if(double.IsNaN(value)) return true;
			if(double.IsInfinity(value)) return true;
			return false;
		}

	}
}
