using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.Core {
	// ReSharper disable InconsistentNaming

	[TestClass]
	public class ShapeTests {

		/// <summary> 
		/// Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() { }

		/// <summary> 
		/// Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() { }

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void Common() {

			var l = new List<Point>(new[] { new Point(0, 0), new Point(0, 0), new Point(200, 0), new Point(0, 0) });
			var seg = new List<LineSegment>();
			for (int i = 1; i < l.Count; i++) { seg.Add(new LineSegment(l[i], true)); }
			new PathFigure(l[0], seg, true);
			var g = new PathGeometry(new[] { new PathFigure(l[0], seg, true) });

			RectangleGeometry r1 = new RectangleGeometry(new Rect(0, 0, 100, 100));
			Debug.WriteLine("=>" + r1);

			RectangleGeometry r2 = new RectangleGeometry(new Rect(20, 20, 10, 10));
			Debug.WriteLine("=>" + r2);
			if (g.FillContainsWithDetail(r2) == IntersectionDetail.FullyInside) {

			}


			var n = Geometry.Combine(r1, r2, GeometryCombineMode.Exclude, null);
			Debug.WriteLine("=>" + n);


		}
	}
}
