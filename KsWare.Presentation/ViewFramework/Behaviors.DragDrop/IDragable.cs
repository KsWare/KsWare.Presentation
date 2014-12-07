using System;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.Behaviors.DragDrop {

	public interface IDragable {

		/// <summary>
		/// Returns whether drag is allowed.
		/// </summary>
		bool CanDrag { get; }

		/// <summary>
		/// Type of the data item
		/// </summary>
		Type DataType { get; }

		/// <summary>
		/// Remove the object from the collection
		/// </summary>
		void Remove(object data);

		void OnDrag();

		void OnDrop(DragDropEffects effects);
	}
}
