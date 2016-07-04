using System.Windows.Documents;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.Behaviors.DragDrop {

	public class ListBoxAdornerManager {

		private AdornerLayer _adornerLayer;
		private ListBoxAdorner _adorner;

		private bool _shouldCreateNewAdorner = false;

		internal ListBoxAdornerManager(AdornerLayer layer) { _adornerLayer = layer; }

		internal void Update(UIElement adornedElement, bool isAboveElement) {
			if (_adorner != null && !_shouldCreateNewAdorner) {
				//exit if nothing changed
				if (_adorner.AdornedElement == adornedElement && _adorner.IsAboveElement == isAboveElement) return;
			}
			Clear();
			//draw new adorner
			_adorner = new ListBoxAdorner(adornedElement, _adornerLayer);
			_adorner.IsAboveElement = isAboveElement;
			_adorner.Update();
			_shouldCreateNewAdorner = false;
		}


		/// <summary>
		/// Remove the adorner 
		/// </summary>
		internal void Clear() {
			if (_adorner != null) {
				_adorner.Remove();
				_shouldCreateNewAdorner = true;
			}
		}


	}

}
