using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace KsWare.Presentation
{

	/// <summary>  Geometry Util
	/// </summary>
	public static class GeometryUtil
	{

		/// <summary>
		/// Scales the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor.</param>
		/// <returns></returns>
		public static double Scale(double value, double factor) {
			if(factor<1.0) {
				return System.Math.Round(value * factor,3);
			}
			else {
				return value * factor;
			}
		}

		/// <summary> Scales a <see cref="Point"/>
		/// </summary>
		/// <param name="point">The <see cref="Point"/> to scale</param>
		/// <param name="factorX">Scale factor X</param>
		/// <param name="factorY">Scale factor Y</param>
		/// <returns>The scaled <see cref="Point"/></returns>
		public static Point Scale(Point point, double factorX, double factorY) {
			if(factorX<1.0)
				return new Point(System.Math.Round(point.X * factorX,3), System.Math.Round(point.Y * factorY,3));
			else
				return new Point(point.X * factorX, point.Y * factorY);
		}

		/// <summary>
		/// Scales the specified size.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <param name="factorX">The factor X.</param>
		/// <param name="factorY">The factor Y.</param>
		/// <returns></returns>
		public static Size Scale(Size size, double factorX, double factorY) {
			if (factorX < 1.0)
				return new Size(System.Math.Round(size.Width * factorX, 3), System.Math.Round(size.Height * factorY, 3));
			else
				return new Size(size.Width * factorX, size.Height * factorY);
		}

		/// <summary> Scales the specified rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <param name="factorX">The factor X.</param>
		/// <param name="factorY">The factor Y.</param>
		/// <returns></returns>
		public static Rect Scale(Rect rect, double factorX, double factorY) {
			if (factorX < 1.0) {
				double x = System.Math.Round(rect.X * factorX, 3);
				double y = System.Math.Round(rect.Y * factorY, 3);
				double w = System.Math.Round(rect.Width * factorX, 3);
				double h = System.Math.Round(rect.Height * factorY, 3);
				return new Rect(x, y, w, h);
			}
			else {
				return new Rect(rect.X * factorX, rect.Y * factorY, rect.Width * factorX, rect.Height * factorY);
			}
		}

		/// <summary> Scales the specified geometry.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="factorX">The factor X.</param>
		/// <param name="factorY">The factor Y.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public static Geometry Scale(Geometry geometry, double factorX, double factorY) {
			if (geometry == null)
				return null;//REVIEW ASK throw exception?
			if (geometry is PathGeometry)
				return Scale((PathGeometry)geometry, factorX, factorY);
			if (geometry is RectangleGeometry)
				return Scale((RectangleGeometry)geometry, factorX, factorY);
			if (geometry is EllipseGeometry)
				return Scale((EllipseGeometry)geometry, factorX, factorY);
			throw new NotImplementedException("{9B836D82-AD1F-44A8-88E5-9E6A76C21902}");
		}

		/// <summary> Scales a <see cref="EllipseGeometry"/>
		/// </summary>
		/// <param name="geometry">The <see cref="EllipseGeometry"/> to scale</param>
		/// <param name="factorX">Scale factor X</param>
		/// <param name="factorY">Scale factor Y</param>
		/// <returns>The scaled <see cref="EllipseGeometry"/></returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static EllipseGeometry Scale(EllipseGeometry geometry, double factorX, double factorY) {
			var rect = geometry.Bounds;
			if (factorX < 1.0) {
				double x = System.Math.Round(rect.X * factorX, 3);
				double y = System.Math.Round(rect.Y * factorY, 3);
				double w = System.Math.Round(rect.Width * factorX, 3);
				double h = System.Math.Round(rect.Height * factorY, 3);
				var rect2 = new Rect(x, y, w, h);
				return new EllipseGeometry(rect2);
			}
			else {
				var rect2 = new Rect(rect.X * factorX, rect.Y * factorY, rect.Width * factorX, rect.Height * factorY);
				return new EllipseGeometry(rect2);
			}

		}

		/// <summary> Scales a <see cref="RectangleGeometry"/>
		/// </summary>
		/// <param name="geometry">The <see cref="RectangleGeometry"/> to scale</param>
		/// <param name="factorX">Scale factor X</param>
		/// <param name="factorY">Scale factor Y</param>
		/// <returns>The scaled <see cref="PathGeometry"/></returns>
		public static RectangleGeometry Scale(RectangleGeometry geometry, double factorX, double factorY) {
			var rect = geometry.Rect;
			if (factorX < 1.0) {
				double x = System.Math.Round(rect.X * factorX, 3);
				double y = System.Math.Round(rect.Y * factorY, 3);
				double w = System.Math.Round(rect.Width * factorX, 3);
				double h = System.Math.Round(rect.Height * factorY, 3);
				var rect2 = new Rect(x, y, w, h);
				return new RectangleGeometry(rect2);
			}
			else {
				var rect2 = new Rect(rect.X * factorX, rect.Y * factorY, rect.Width * factorX, rect.Height * factorY);
				return new RectangleGeometry(rect2);
			}
		}

		/// <summary> Scales a <see cref="PathGeometry"/>
		/// </summary>
		/// <param name="geometry">The <see cref="PathGeometry"/> to scale</param>
		/// <param name="factorX">Scale factor X</param>
		/// <param name="factorY">Scale factor Y</param>
		/// <returns>The scaled <see cref="PathGeometry"/></returns>
		public static PathGeometry Scale(PathGeometry geometry, double factorX, double factorY) {
			List<Point> points = GetPoints(geometry);
			List<Point> points2 = new List<Point>();
			foreach (var point in points) { points2.Add(Scale(point, factorX, factorY)); }
			return CreatePathGeometry(points2);
		}

		/// <summary> Moves the specified point.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <param name="moveX">The move X.</param>
		/// <param name="moveY">The move Y.</param>
		/// <returns></returns>
		public static Point Move(Point point, double moveX, double moveY) {
			return new Point(point.X+moveX,point.Y+moveY);
		}

		/// <summary> Moves the specified geometry.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="moveX">The move X.</param>
		/// <param name="moveY">The move Y.</param>
		/// <returns></returns>
		public static PathGeometry Move(PathGeometry geometry, double moveX, double moveY) {
			List<Point> points = GetPoints(geometry);
			List<Point> points2 = new List<Point>();
			foreach (var point in points) { points2.Add(Move(point, moveX, moveY)); }
			return CreatePathGeometry(points2);
		}
		
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private static List<Point> GetPoints(PathGeometry geometry) {
			List<Point> points = new List<Point>();
			points.Add(geometry.Figures[0].StartPoint);
			foreach (var segment in geometry.Figures[0].Segments) {
				if (segment is LineSegment) {
					points.Add(((LineSegment)segment).Point);
				}
				else if (segment is PolyLineSegment) {
					var seg = ((PolyLineSegment) segment);
					foreach (var point in seg.Points) { points.Add(point); }
				}
				else {
					throw new NotImplementedException("Segment type not implemented!" + "\r\n\tUniqueID: {8513FF5E-8958-467A-83CE-8C40C684A70E}");
				}
			}
			return points;
		} 

		private static PathGeometry CreatePathGeometry(List<Point> points) {
			var segments = new List<LineSegment>();
			foreach (var point in points) segments.Add(new LineSegment(point, true)); 
			segments.RemoveAt(0);
			var pathFigure = new PathFigure(points[0], segments, true);
			return new PathGeometry(new[] { pathFigure });
		}


		private static readonly CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");

		/// <summary> Gets the geometry type from string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static GeometryType GetGeometryType(string text) {
			if (text.StartsWith(GeometryType.Path.ToString(),StringComparison.Ordinal)) {
				return GeometryType.Path;
			}
			else if (text.StartsWith(GeometryType.Rectangle.ToString(),StringComparison.Ordinal)) {
				return GeometryType.Rectangle;
			}
			else if (text.StartsWith(GeometryType.Ellipse.ToString(),StringComparison.Ordinal)) {
				return GeometryType.Ellipse;
			}
			else {
				return GeometryType.Path;
			}
		}

		/// <summary> Gets the geometry from string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static Geometry GetGeometry(string text) {
			if (String.IsNullOrEmpty(text))
				return null;
			if (text.StartsWith(GeometryType.Path.ToString(),StringComparison.Ordinal)) {
				string content = text.Substring(GeometryType.Path.ToString().Length + 1);
				GeometryConverter conv = new GeometryConverter();
				StreamGeometry streamGeometry = (StreamGeometry)conv.ConvertFrom(content);
				PathGeometry geometry = streamGeometry.GetFlattenedPathGeometry();
				return geometry;
			} else if (text.StartsWith(GeometryType.Rectangle.ToString(),StringComparison.Ordinal)) {
				string content = text.Substring(GeometryType.Rectangle.ToString().Length + 1);
				string[] contents = content.Split(',');
				RectangleGeometry geometry = new RectangleGeometry();
				geometry.Rect = new Rect(Convert.ToDouble(contents[0], enUS),
											Convert.ToDouble(contents[1], enUS),
											Convert.ToDouble(contents[2], enUS),
											Convert.ToDouble(contents[3], enUS)
										);
				return geometry;
			} else if (text.StartsWith(GeometryType.Ellipse.ToString(),StringComparison.Ordinal)) {
				string content = text.Substring(GeometryType.Ellipse.ToString().Length + 1);
				string[] contents = content.Split(',');
				EllipseGeometry geometry = new EllipseGeometry();
				geometry.Center = new Point(Convert.ToDouble(contents[0], enUS) + Convert.ToDouble(contents[2], enUS) / 2.0, Convert.ToDouble(contents[1], enUS) + Convert.ToDouble(contents[3], enUS) / 2.0);
				geometry.RadiusX = Convert.ToDouble(contents[2], enUS) / 2.0;
				geometry.RadiusY = Convert.ToDouble(contents[3], enUS) / 2.0;
				return geometry;
			} else if (text.StartsWith("Text:",StringComparison.Ordinal)) {
				string content = text.Substring("Text:".Length);
				string[] contents = content.Split(',');
				RectangleGeometry geometry = new RectangleGeometry();
				geometry.Rect = new Rect(Convert.ToDouble(contents[0], enUS),
											Convert.ToDouble(contents[1], enUS),
											Convert.ToDouble(contents[2], enUS),
											Convert.ToDouble(contents[3], enUS)
										);
				return geometry;
			} else {
				throw new InvalidOperationException("This is not a valid geometry object in string");
			}
		}

		/// <summary>  Gets the string from geometry.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public static string GetString(Geometry geometry) {
			string geometryString;
			if (geometry is PathGeometry) {
				PathGeometry g = geometry.GetFlattenedPathGeometry();
				geometryString = GeometryType.Path.ToString() + ":" + g.ToString(enUS);
			}
			else if (geometry is RectangleGeometry) {
				RectangleGeometry g = (RectangleGeometry)geometry;
				geometryString = GeometryType.Rectangle.ToString() + ":" + g.Rect.ToString(enUS);
			}
			else if (geometry is EllipseGeometry) {
				EllipseGeometry g = (EllipseGeometry)geometry;
				geometryString = GeometryType.Ellipse.ToString() + ":" + g.Bounds.ToString(enUS);
			}
			else {
				throw new InvalidOperationException("This is not a valid geometry object");
			}
			return geometryString;
		}

		/// <summary> Gets the path geometry for rect.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="top">The top.</param>
		/// <param name="right">The right.</param>
		/// <param name="bottom">The bottom.</param>
		/// <returns></returns>
		public static PathGeometry GetPathGeometryForRect(double left, double top, double right, double bottom) {
			
			List<Point> points = new List<Point>();
			points.Add(new Point(left, top));
			points.Add(new Point(right, top));
			points.Add(new Point(right, bottom));
			points.Add(new Point(left, bottom));

			var segments=new List<PathSegment>();
			for (int i = 1; i < points.Count; i++) segments.Add(new LineSegment(points[i], true));
			var figure=new PathFigure(points[0],segments,true);

			PathGeometry geometry = new PathGeometry(new []{figure});
			return geometry;
		}

		private static readonly Pen NoPen = CreateNoPen();

		private static Pen CreateNoPen() {
			var pen = new Pen(Brushes.Black, 0);
			pen.Freeze();
			return pen;
		}

		/// <summary> Gets the bounds of the specified <paramref name="geometry"/>.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <returns>The bounds</returns>
		/// <remarks></remarks>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public static Rect GetBounds(Geometry geometry) {
			double x, y, h, w;
			if(geometry==null) {
				//ASK return NaN / Empty or throw exception
/*OPTION:*/		return new Rect(double.NaN,double.NaN,double.NaN,double.NaN);
//OPTION:		return Rect.Empty;
//OPTION:		throw new ArgumentNullException("geometry");
			} else if(geometry is RectangleGeometry) {
				var rectangle = (RectangleGeometry) geometry;
				var rect = rectangle.Rect;
				x = rect.X;
				y = rect.Y;
				w = rect.Width;
				h = rect.Height;
			} else if(geometry is EllipseGeometry) {
				var ellipse = (EllipseGeometry) geometry;
				var rect = ellipse.Bounds;
				x = rect.X;
				y = rect.Y;
				w = rect.Width;
				h = rect.Height;
			} else if(geometry is PathGeometry) {
				var path = (PathGeometry) geometry;
				var rect = path.GetRenderBounds(NoPen);
				x = rect.X;
				y = rect.Y;
				w = rect.Width;
				h = rect.Height;
			} else {
				throw new NotImplementedException("{79BF2DBE-5492-4012-9D7B-CAE8F7FA971E}");
			}
			return new Rect(x, y, w, h);
		}

		/// <summary> Create a new <see cref="PathGeometry"/> from specified <paramref name="points"/>.
		/// </summary>
		/// <param name="points">The points for the new <see cref="PathGeometry"/></param>
		/// <returns></returns>
		/// <remarks>The new <see cref="PathGeometry"/></remarks>
		public static PathGeometry CreatePathGeometry(IEnumerable<Point> points) {
			Point startPoint=new Point();
			List<PathSegment> segments=null;
			foreach (var point in points) {
				if(segments==null) {
					startPoint = point;
					segments = new List<PathSegment>();
				} else {
					segments.Add(new LineSegment(point,true));
				}
			}
			if(segments==null || segments.Count==0) {
				throw new ArgumentException("Invalid point list!",nameof(points));
			}
			var pathGeometry = new PathGeometry(new []{new PathFigure(startPoint,segments,true)});
			return pathGeometry;
		}

		/// <summary> Create a new <see cref="PathGeometry"/> from specified <paramref name="nodes"/>.
		/// </summary>
		/// <param name="nodes">The nodes for the new <see cref="PathGeometry"/></param>
		/// <returns></returns>
		/// <remarks>The new <see cref="PathGeometry"/></remarks>
		public static PathGeometry CreatePathGeometry(IEnumerable<PathNodeData> nodes) {
			var geometryData = CreatePathGeometryData(nodes);
			return CreatePathGeometry(geometryData);
		}

		/// <summary> Creates the path geometry data.
		/// </summary>
		/// <param name="nodes">The nodes.</param>
		/// <returns></returns>
		public static PathGeometryData CreatePathGeometryData(IEnumerable<PathNodeData> nodes) {
			PathGeometryData geometryData = new PathGeometryData();
			PathFigureData figure=null;
			PathSegmentData segment=null;
			foreach (var node in nodes) {
				if(node.SegmentType==SegmentType.StartPoint) {
					figure=new PathFigureData{StartPoint=node.Point,IsClosed=node.IsClosed};
					geometryData.Figures.Add(figure);
				} else 
					switch (node.SegmentType) {
						case SegmentType.ArcSegment: 
							throw new NotImplementedException("{C3DB91D2-6511-4274-B60F-99DF3CADF0A3}");
						case SegmentType.BezierSegment: 
							throw new NotImplementedException("{D811AFE1-EB44-4671-9F27-E7609AEF8B5B}");
						case SegmentType.LineSegment: {
							figure.Segments.Add(new PathSegmentData {Nodes={node}, SegmentType = node.SegmentType});
							break;
						}
						case SegmentType.PolyBezierSegment:
							throw new NotImplementedException("{3C01C459-4539-4D87-8206-DF1903491B63}");
						case SegmentType.PolyLineSegment: {
							if(node.SegmentIndex==0) {
								segment=new PathSegmentData {Nodes = {node},SegmentType = node.SegmentType};
								figure.Segments.Add(segment);
							} else {
								segment.Nodes.Add(node);
							}
							break;
						}
						case SegmentType.PolyQuadraticBezierSegment:
							throw new NotImplementedException("{61171A80-E52E-4C93-B7DE-DCF9C65B93F3}");
						case SegmentType.QuadraticBezierSegment:
							throw new NotImplementedException("{64CA6FB1-A5B2-4F5E-BE48-5A2E85F6B6B9}");
						default:
							throw new NotImplementedException("{E1D79545-F131-4196-B964-A74CBB99DBFD}");
					}
			}
			return geometryData;
		}

		/// <summary>
		/// Creates the path geometry.
		/// </summary>
		/// <param name="geometryData">The geometry data.</param>
		/// <returns></returns>
		public static PathGeometry CreatePathGeometry(PathGeometryData geometryData) {
			var figures = new List<PathFigure>();
			foreach (var figureData in geometryData.Figures) {
				var segments = new List<PathSegment>();
				foreach (var segmentData in figureData.Segments) {
					switch (segmentData.SegmentType) {
						case SegmentType.ArcSegment:
							throw new NotImplementedException("{FD35628A-FB82-491E-B042-ED2F893D342B}");
						case SegmentType.BezierSegment:
							throw new NotImplementedException("{1B510116-A6D7-448B-99FF-D9F67210E8D3}");
						case SegmentType.LineSegment: {
							segments.Add(new LineSegment(segmentData.Nodes[0].Point,segmentData.Nodes[0].IsStroked));
							break;
						}
						case SegmentType.PolyBezierSegment:
							throw new NotImplementedException("{1EF8B9A9-BC3E-439C-A14E-3C56812D2002}");
						case SegmentType.PolyLineSegment: {
							var points = new List<Point>();
							foreach (var nodeData in segmentData.Nodes) points.Add(nodeData.Point);
							segments.Add(new PolyLineSegment(points, segmentData.Nodes[0].IsStroked));
							break;
						}
						case SegmentType.PolyQuadraticBezierSegment:
							throw new NotImplementedException("{CD280F38-FC56-4838-A5D2-2BE04B5801C8}");
						case SegmentType.QuadraticBezierSegment:
							throw new NotImplementedException("{7DE58DAC-9A43-4657-90A5-7B38F5748C4C}");
					}
				}
				figures.Add(new PathFigure(figureData.StartPoint, segments, figureData.IsClosed)); 
			}
			return new PathGeometry(figures);
		}

		/// <summary> Gets the path nodes.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]	
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public static List<PathNodeData> GetPathNodes(PathGeometry geometry) {
			List<PathNodeData> list=new List<PathNodeData>();

			foreach (var figure in geometry.Figures) {
				var figureIndex = geometry.Figures.IndexOf(figure);
				foreach (PathSegment segment in figure.Segments) {
					var segmentIndex = figure.Segments.IndexOf(segment);

					//Debug.WriteLine("=>    Segment {0}", figure.Segments.IndexOf(segment));
					//Debug.WriteLine("=>        Type: {0}", segment.GetType().Name);

					if(segment is ArcSegment) {
						//ArcSegment arcSegment = (ArcSegment) segment;
						//Debug.WriteLine("=>        Point: {0}", arcSegment.Point);
						throw new NotImplementedException("{47377672-B823-4DAA-AE0D-BAA9380FC565}");
					}else if(segment is BezierSegment) {
						//BezierSegment arcSegment = (BezierSegment) segment;
						//Debug.WriteLine("=>        Point1: {0}", arcSegment.Point1);
						//Debug.WriteLine("=>        Point2: {0}", arcSegment.Point2);
						//Debug.WriteLine("=>        Point3: {0}", arcSegment.Point3);//EndPoint
						throw new NotImplementedException("{5BA74993-0DE8-489A-9140-C393B3B7FDEA}");
					}else if(segment is LineSegment) {
						LineSegment lineSegment = segment as LineSegment;
						//Debug.WriteLine("=>        Point: {0}", lineSegment.Point);
						var p = lineSegment.Point;
						list.Add(new PathNodeData{Point=p, FigureIndex=figureIndex, SegmentIndex=segmentIndex, SegmentType=SegmentType.LineSegment, IsStroked = segment.IsStroked});
					}else if(segment is PolyBezierSegment) {
						PolyBezierSegment polyBezierSegment = (PolyBezierSegment) segment;
						foreach (Point point in polyBezierSegment.Points) {
							//Debug.WriteLine("=>        Point: {0}", point);
						}
						throw new NotImplementedException("{D9A2D0E5-2DE7-4C0B-A47B-F9C8F44E576E}");
					}else if(segment is PolyLineSegment) {
						PolyLineSegment polyLineSegment = (PolyLineSegment) segment;
						for(int pi=0;pi<polyLineSegment.Points.Count;pi++){
							Point point=polyLineSegment.Points[pi];
							list.Add(new PathNodeData{Point=point, SegmentType=SegmentType.PolyLineSegment, FigureIndex=figureIndex,SegmentIndex=segmentIndex, PolyIndex=pi, IsStroked = segment.IsStroked});
						}
					}else if(segment is PolyQuadraticBezierSegment) {
						PolyQuadraticBezierSegment polyQuadraticBezierSegment = (PolyQuadraticBezierSegment) segment;
						foreach (Point point in polyQuadraticBezierSegment.Points) {
							//Debug.WriteLine("=>        Point: {0}", point);
						}
						throw new NotImplementedException("{9B28B878-7A8A-4592-ACB4-07C14145FB3A}");
					}else if(segment is QuadraticBezierSegment) {
						//QuadraticBezierSegment quadraticBezierSegment = (QuadraticBezierSegment) segment;
						//Debug.WriteLine("=>        Point1: {0}", quadraticBezierSegment.Point1);
						//Debug.WriteLine("=>        Point2: {0}", quadraticBezierSegment.Point2);
						throw new NotImplementedException("{6E6AF17B-D66E-4738-BE40-7F5930E466B7}");
					}else {
						throw new NotImplementedException("{0B7B88C1-68BA-4636-8B66-3EC706B4B4A4}");
					}
				}				
			}
			return list;
		}

		/// <summary> Path geometry data with figures to save and load xml
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public class PathGeometryData
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PathGeometryData"/> class.
			/// </summary>
			public PathGeometryData() {
				Figures = new List<PathFigureData>();
			}

			/// <summary>
			/// Gets or sets the figures.
			/// </summary>
			/// <value>The figures.</value>
			[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
			public List<PathFigureData> Figures { get; private set; }
		}

		/// <summary> Path figures to save and load xml
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public class PathFigureData
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PathFigureData"/> class.
			/// </summary>
			public PathFigureData() {Segments=new List<PathSegmentData>();}

			/// <summary>
			/// Gets or sets the start point.
			/// </summary>
			/// <value>The start point.</value>
			public Point StartPoint { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this instance is closed.
			/// </summary>
			/// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
			public bool IsClosed { get; set; }

			/// <summary> Gets or sets the segments.
			/// </summary>
			/// <value>The segments.</value>
			[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
			public List<PathSegmentData> Segments { get; private set; }
		}

		/// <summary>
		/// Path segmenet data with figures to save and load xml
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public class PathSegmentData
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PathSegmentData"/> class.
			/// </summary>
			public PathSegmentData() {
				Nodes=new List<PathNodeData>();
			}

			/// <summary>
			/// Gets or sets the nodes.
			/// </summary>
			/// <value>The nodes.</value>
			[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
			public List<PathNodeData> Nodes { get; private set; }

			/// <summary>
			/// Gets or sets the type of the segment.
			/// </summary>
			/// <value>The type of the segment.</value>
			public SegmentType SegmentType { get; set; }
		}


	}


	/// <summary> Segment types to save and load xml
	/// </summary>
	public enum SegmentType
	{
		/// <summary> StartPoint </summary>
		StartPoint,

		/// <summary> LineSegment </summary>
		LineSegment,

		/// <summary> ArcSegment </summary>
		ArcSegment,

		/// <summary> BezierSegment </summary>
		BezierSegment,

		/// <summary> QuadraticBezierSegment </summary>
		QuadraticBezierSegment,

		/// <summary> PolyArcSegment </summary>
		PolyArcSegment,

		/// <summary> PolyLineSegment </summary>
		PolyLineSegment,

		/// <summary> PolyBezierSegment </summary>
		PolyBezierSegment,

		/// <summary> PolyQuadraticBezierSegment </summary>
		PolyQuadraticBezierSegment,
	}

	/// <summary>
	/// Type of geometry object to laod from xml and save to xml 
	/// </summary>
	public enum GeometryType
	{
		/// <summary> </summary>
		None,

		/// <summary> Path </summary>
		Path,

		/// <summary> Rectangle </summary>
		Rectangle,

		/// <summary> Ellipse </summary>
		Ellipse
	}

}

