using System.Windows;
using System.Windows.Shapes;

namespace KsWare.Presentation {

	/// <summary> <see cref="Path"/> data for the nodes to save and load from xml
	/// </summary>
	public class PathNodeData {

		/// <summary>
		/// Gets or sets the type of the segment.
		/// </summary>
		/// <value>The type of the segment.</value>
		public SegmentType SegmentType { get; set; }

		/// <summary>
		/// Gets or sets the point.
		/// </summary>
		/// <value>The point.</value>
		public Point Point { get; set; }

		/// <summary>
		/// Gets or sets the index of the poly.
		/// </summary>
		/// <value>The index of the poly.</value>
		public int PolyIndex { get; set; }

		/// <summary>
		/// Gets or sets the index of the figure.
		/// </summary>
		/// <value>The index of the figure.</value>
		public int FigureIndex { get; set; }

		/// <summary>
		/// Gets or sets the index of the segment.
		/// </summary>
		/// <value>The index of the segment.</value>
		public int SegmentIndex { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is closed.
		/// </summary>
		/// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
		public bool IsClosed { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is stroked.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is stroked; otherwise, <c>false</c>.
		/// </value>
		public bool IsStroked { get; set; }
	}
}
