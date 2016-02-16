using System.Windows.Documents;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.Behaviors.DragDrop {

	public class ListBoxAdornerManager {

		private AdornerLayer m_AdornerLayer;
		private ListBoxAdorner m_Adorner;

		private bool m_ShouldCreateNewAdorner = false;

		internal ListBoxAdornerManager(AdornerLayer layer) { m_AdornerLayer = layer; }

		internal void Update(UIElement adornedElement, bool isAboveElement) {
			if (m_Adorner != null && !m_ShouldCreateNewAdorner) {
				//exit if nothing changed
				if (m_Adorner.AdornedElement == adornedElement && m_Adorner.IsAboveElement == isAboveElement) return;
			}
			Clear();
			//draw new adorner
			m_Adorner = new ListBoxAdorner(adornedElement, m_AdornerLayer);
			m_Adorner.IsAboveElement = isAboveElement;
			m_Adorner.Update();
			m_ShouldCreateNewAdorner = false;
		}


		/// <summary>
		/// Remove the adorner 
		/// </summary>
		internal void Clear() {
			if (m_Adorner != null) {
				m_Adorner.Remove();
				m_ShouldCreateNewAdorner = true;
			}
		}


	}

}
