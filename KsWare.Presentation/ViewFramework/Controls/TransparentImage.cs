using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KsWare.Presentation.ViewFramework.Controls {

	public class TransparentImage : Image {

		private const byte Threshold = 3;

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			var source = (BitmapSource) Source;
			if (source == null) return base.HitTestCore(hitTestParameters);

			// Get value of current pixel
			var x = (int) (hitTestParameters.HitPoint.X/ActualWidth*source.PixelWidth);
			var y = (int) (hitTestParameters.HitPoint.Y/ActualHeight*source.PixelHeight);
			if (x >= source.PixelWidth || y >= source.PixelHeight || x < 0 || y < 0) return null;
			var pixels = new byte[4];
			source.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
			// Check alpha channel
//			Debug.WriteLine(pixels[3]);
			if (pixels[3] < Threshold ) return null;
			return new PointHitTestResult(this, hitTestParameters.HitPoint); 
		}

		protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters) {
			// Do something similar here, possibly checking every pixel within
			// the hitTestParameters.HitGeometry.Bounds rectangle
			return base.HitTestCore(hitTestParameters);
		}
	}
}
