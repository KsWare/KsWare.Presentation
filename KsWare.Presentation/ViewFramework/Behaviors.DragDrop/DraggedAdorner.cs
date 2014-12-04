using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace KsWare.Presentation.ViewFramework.Behaviors.DragDrop {

	public class DraggedAdorner : Adorner {

		private AdornerLayer m_AdornerLayer;
		private ContentPresenter m_ContentPresenter;
		private double m_Left;
		private double m_Top;

		public DraggedAdorner(object dragDropData, DataTemplate dragDropTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
			: base(adornedElement) {
			m_AdornerLayer = adornerLayer;

			m_ContentPresenter = new ContentPresenter();
			m_ContentPresenter.Content = dragDropData;
			m_ContentPresenter.ContentTemplate = dragDropTemplate;
			m_ContentPresenter.Opacity = 0.7;

			m_AdornerLayer.Add(this);
		}

		public void SetPosition(double left, double top) {
			// -1 and +13 align the dragged adorner with the dashed rectangle that shows up
			// near the mouse cursor when dragging.
			m_Left = left - 1;
			m_Top = top + 13;
			if (m_AdornerLayer != null) { m_AdornerLayer.Update(AdornedElement); }
		}

		protected override Size MeasureOverride(Size constraint) {
			m_ContentPresenter.Measure(constraint);
			return m_ContentPresenter.DesiredSize;
		}

		protected override Size ArrangeOverride(Size finalSize) {
			m_ContentPresenter.Arrange(new Rect(finalSize));
			return finalSize;
		}

		protected override Visual GetVisualChild(int index) { return m_ContentPresenter; }

		protected override int VisualChildrenCount { get { return 1; } }

		public override GeneralTransform GetDesiredTransform(GeneralTransform transform) {
			var result = new GeneralTransformGroup();
			result.Children.Add(base.GetDesiredTransform(transform));
			result.Children.Add(new TranslateTransform(m_Left, m_Top));

			return result;
		}

		public void Detach() { m_AdornerLayer.Remove(this); }

	}
}
