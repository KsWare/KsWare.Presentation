using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KsWare.Presentation.ViewFramework.Controls {

	//TODO rename. name is misleading. => HitTestTransparentImage?
	//TODO extracted to its own package
	public class TransparentImage : Image {

		public static readonly DependencyProperty AlphaThresholdProperty = DependencyProperty.Register(
			"AlphaThreshold", typeof(byte), typeof(TransparentImage), new PropertyMetadata(default(byte)));

		public byte AlphaThreshold { get => (byte) GetValue(AlphaThresholdProperty); set => SetValue(AlphaThresholdProperty, value); }

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
			if (pixels[3] <= AlphaThreshold ) return null; //HitTest transparent
			return new PointHitTestResult(this, hitTestParameters.HitPoint); 
		}

		protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters) {
			// Do something similar here, possibly checking every pixel within
			// the hitTestParameters.HitGeometry.Bounds rectangle
			return base.HitTestCore(hitTestParameters);
		}
	}
}
