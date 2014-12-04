using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewFramework.Behaviors.DragDrop {

	public interface IDropable {

		/// <summary> Type of the data item
		/// </summary>
		Type DataType { get; }

		/// <summary> Drop data into the collection.
		/// </summary>
		/// <param name="data">The data to be dropped</param>
		/// <param name="index">optional: The index location to insert the data</param>
		/// <param name="dropPosition">optional: the drop position</param>
		void Drop(object data, int index = -1, DropPosition dropPosition = DropPosition.Over);

		bool CanDrop(object data, int index = -1, DropPosition dropPosition = DropPosition.Over);
	}

	public enum DropPosition {
		None,
		Over,
		Left,
		Top,
		Right,
		Bottom,
		TopLeft,
		TopRight,
		BottomRight,
		BottomLeft,
	}
}
