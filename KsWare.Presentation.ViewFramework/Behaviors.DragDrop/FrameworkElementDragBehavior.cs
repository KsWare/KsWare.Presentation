using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using KsWare.Presentation.ViewFramework.Behaviors.DragDrop;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	
	public class FrameworkElementDragBehavior : Behavior<FrameworkElement> {

		#region DragDropTemplate 

		public static readonly DependencyProperty DragDropTemplateProperty = DependencyProperty.Register(
			"DragDropTemplate", typeof (DataTemplate), typeof (FrameworkElementDragBehavior), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate DragDropTemplate { get { return (DataTemplate) GetValue(DragDropTemplateProperty); } set { SetValue(DragDropTemplateProperty, value); } }

		#endregion

		public static readonly DependencyProperty DragCursorProperty = DependencyProperty.Register(
			"DragCursor", typeof (DataTemplate), typeof (FrameworkElementDragBehavior), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate DragCursor { get { return (DataTemplate) GetValue(DragCursorProperty); } set { SetValue(DragCursorProperty, value); } }

		#region EnableTrace
		public static readonly DependencyProperty EnableTraceProperty = DependencyProperty.Register(
			"EnableTrace", typeof (bool), typeof (FrameworkElementDragBehavior), new PropertyMetadata(default(bool)));

		public bool EnableTrace { get { return (bool) GetValue(EnableTraceProperty); } set { SetValue(EnableTraceProperty, value); } }
		#endregion EnableTrace

		private bool m_IsLeftButtonDown;
		private DraggedAdorner m_DraggedAdorner;
		private Window m_TopWindow;
		private Point m_InitialMousePosition;
		private Vector m_InitialMouseOffset;
		private bool m_IsDragging;
		private DraggedAdorner m_DragCursorAdorner;

		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.PreviewMouseLeftButtonDown += AtMouseLeftButtonDown;
			AssociatedObject.PreviewMouseLeftButtonUp   += AtMouseLeftButtonUp;
			AssociatedObject.PreviewMouseMove           += AtMouseMove;
//			AssociatedObject.MouseLeave                 += AtMouseLeave;
		}

		private void AtMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			m_IsLeftButtonDown = true;
			m_TopWindow = FindWindow();
			m_InitialMousePosition = e.GetPosition(m_TopWindow);
		}

		private Window FindWindow() {
			var w=Window.GetWindow(AssociatedObject);
			if (w != null) return w;
			var o = (DependencyObject)AssociatedObject;
			while (o!=null) {
				if(o is Window) return (Window) o;
				o = VisualTreeHelper.GetParent(o);
			}
			return null;
		}

		private void AtMouseMove(object sender, MouseEventArgs e) {
			if(e.LeftButton!=MouseButtonState.Pressed) return;
			if(m_IsDragging) {Debug.WriteLine(e.GetPosition(m_TopWindow)); return;} 
			if (!IsMovementBigEnough(m_InitialMousePosition, e.GetPosition(m_TopWindow))) return;
			OnPreviewBeginDrag(e);
		}

		private void AtMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			m_IsLeftButtonDown = false;
		}

		private void AtMouseLeave(object sender, MouseEventArgs e) {
			if (!m_IsLeftButtonDown) return;
			OnPreviewBeginDrag(e);
		}

		private void OnPreviewBeginDrag(MouseEventArgs e) {
			//get the item's DataContext as the data to be transferred
			if (AssociatedObject.DataContext == null) {
				//Trace.WriteLine("WARNING: DataContext is null! UID:{2FE3886C-7CC6-4BA7-8996-1A6CA59B298F}"); 
				throw new InvalidOperationException("DataContext is null! UID:{2FE3886C-7CC6-4BA7-8996-1A6CA59B298F}");
				return;
			}
			var dragObject = AssociatedObject.DataContext as IDragable;
			if (dragObject == null) {
				//Trace.WriteLine("WARNING: DataContext not implements IDragable! UID:{27F3E817-9746-4785-9E8D-A1DE9F1DE7CE}");
				throw new InvalidOperationException("DataContext not implements IDragable! UID:{27F3E817-9746-4785-9E8D-A1DE9F1DE7CE}");
				return;
			}
			if (!dragObject.CanDrag) {
				if(EnableTrace) Trace.WriteLine("TRACE: CanDrag is false. UID:{28C396ED-CF95-402F-B41E-F8CAED9297E5}");
				return;
			}

			var allowedEffects=DragDropEffects.Move;
			dragObject.OnDrag();  
			
			m_InitialMouseOffset = m_InitialMousePosition - AssociatedObject.TranslatePoint(new Point(0, 0), m_TopWindow);
			
			// Adding events to the window to make sure dragged adorner comes up when mouse is not over a drop target.
			bool previousAllowDrop = m_TopWindow.AllowDrop;
			CheckTopWindowBehavior();
			m_TopWindow.AllowDrop = true;
			m_TopWindow.PreviewDragEnter         += TopWindow_DragEnter;
			m_TopWindow.PreviewDragOver          += TopWindow_DragOver;
			m_TopWindow.PreviewDragLeave         += TopWindow_DragLeave;
			m_TopWindow.PreviewQueryContinueDrag += TopWindow_QueryContinueDrag;
			m_TopWindow.QueryCursor              += TopWindow_QueryCursor;
			m_TopWindow.PreviewGiveFeedback      += TopWindow_GiveFeedback;

			ShowDraggedAdorner(e.GetPosition(m_TopWindow));
			var data = new DataObject();
			data.SetData(dragObject.DataType, AssociatedObject.DataContext);

			m_IsDragging = true;
			var dragDropEffects = System.Windows.DragDrop.DoDragDrop(AssociatedObject, data, allowedEffects);

			// Without this call, there would be a bug in the following scenario: Click on a data item, and drag
			// the mouse very fast outside of the window. When doing this really fast, for some reason I don't get 
			// the Window leave event, and the dragged adorner is left behind.
			// With this call, the dragged adorner will disappear when we release the mouse outside of the window,
			// which is when the DoDragDrop synchronous method returns.
			RemoveDraggedAdorner();

			Mouse.SetCursor(Cursors.Arrow); //TODO Cursors.Arrow??

			m_TopWindow.AllowDrop = previousAllowDrop;
			m_TopWindow.PreviewDragEnter         -= TopWindow_DragEnter;
			m_TopWindow.PreviewDragOver          -= TopWindow_DragOver;
			m_TopWindow.PreviewDragLeave         -= TopWindow_DragLeave;
			m_TopWindow.PreviewQueryContinueDrag -= TopWindow_QueryContinueDrag;
			m_TopWindow.QueryCursor              -= TopWindow_QueryCursor;
			m_TopWindow.PreviewGiveFeedback      -= TopWindow_GiveFeedback;

//			this.draggedData = null;
			m_IsDragging = false;

			dragObject.OnDrop(dragDropEffects);
		}

		private void CheckTopWindowBehavior() {
			var allowDrop = m_TopWindow.AllowDrop;
			var dropBehavior=Interaction.GetBehaviors(m_TopWindow).OfType<FrameworkElementDropBehavior>().FirstOrDefault();
		}

		private void TopWindow_QueryCursor(object sender, QueryCursorEventArgs e) {
//			e.Cursor = Cursors.None; 
//			e.Handled = true;
		}

		private void TopWindow_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
			if (DragCursor != null) {
				Mouse.SetCursor(Cursors.None);
				e.UseDefaultCursors = false;
				e.Handled = true;				
			}
		}


		private void TopWindow_DragEnter(object sender, DragEventArgs e) {
			ShowDraggedAdorner(e.GetPosition(m_TopWindow));
			e.Effects = DragDropEffects.None;
			//e.Handled = true;
		}

		private void TopWindow_DragOver(object sender, DragEventArgs e) {
			ShowDraggedAdorner(e.GetPosition(m_TopWindow));
			e.Effects = DragDropEffects.None;
			//e.Handled = true;
		}

		private void TopWindow_DragLeave(object sender, DragEventArgs e) {
			RemoveDraggedAdorner();
			//e.Handled = true;
		}

		private void TopWindow_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
			//e.Action=DragAction.Continue;

			if(e.EscapePressed) {e.Action=DragAction.Cancel;return;}

			//e.Action=DragAction.Drop|Continue|Cancel;
			//e.KeyStates
		}

		// Creates or updates the dragged Adorner. 
		private void ShowDraggedAdorner(Point currentPosition) {

			if (DragCursor != null) {
				if (m_DragCursorAdorner == null) {
					//var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
					var adornerLayer = AdornerLayer.GetAdornerLayer((Visual) m_TopWindow.Content);
					if (adornerLayer == null) { throw new InvalidOperationException("AdornerLayer not found! ErrorID:{BD0E7004-B8AA-4DE3-87E0-367050336E9C}");}
					m_DragCursorAdorner = new DraggedAdorner(AssociatedObject.DataContext, DragCursor, AssociatedObject, adornerLayer);
				}
				m_DragCursorAdorner.SetPosition(currentPosition.X - m_InitialMousePosition.X + m_InitialMouseOffset.X, currentPosition.Y - m_InitialMousePosition.Y + m_InitialMouseOffset.Y);
			}

//			if (m_DraggedAdorner == null) {
//				var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
//				m_DraggedAdorner = new DraggedAdorner(AssociatedObject.DataContext, GetDragDropTemplate(), AssociatedObject, adornerLayer);
//			}
//			m_DraggedAdorner.SetPosition(currentPosition.X - this.m_InitialMousePosition.X + this.m_InitialMouseOffset.X, currentPosition.Y - this.m_InitialMousePosition.Y + this.m_InitialMouseOffset.Y);
		}


		private DataTemplate GetDragDropTemplate() {
			//var dragDropTemplate = GetDragDropTemplate(AssociatedObject);
			var dragDropTemplate = DragDropTemplate;
			if (dragDropTemplate == null) {
				dragDropTemplate=new DataTemplate();

//				var rectangle = new FrameworkElementFactory(typeof(Rectangle));
//				rectangle.SetValue(Rectangle.HeightProperty, 50.0);
//				rectangle.SetValue(Rectangle.WidthProperty, 50.0);
//				rectangle.SetValue(Rectangle.FillProperty, Brushes.Blue);
//				dragDropTemplate.VisualTree = rectangle;

				var image=new FrameworkElementFactory(typeof(Image));
				image.SetValue(Image.StretchProperty, Stretch.None);
				image.SetValue(Image.SourceProperty, CreateImageSource(AssociatedObject));
//TEST			image.SetValue(Image.SourceProperty, new BitmapImage(new Uri(@"D:\Develop\KsWare\Projects\GalaxyLegendClone\GalaxyLegendWindows\Controls\(ImageControls)\ShipImage\Resources\ship_01.png")));
				dragDropTemplate.VisualTree = image;
			}
			return dragDropTemplate;
		}

		private void RemoveDraggedAdorner() {

			if (m_DragCursorAdorner != null) {
				m_DragCursorAdorner.Detach();
				m_DragCursorAdorner = null;
			}

			if (m_DraggedAdorner != null) {
				m_DraggedAdorner.Detach();
				m_DraggedAdorner = null;
			}
		}

		#region Utilities

		///
		/// Gets a JPG "screenshot" of the current UIElement
		///
		/// UIElement to screenshot
		/// Scale to render the screenshot
		/// JPG Quality
		/// Byte array of JPG data
		public static ImageSource CreateImageSource(FrameworkElement source) {
			var scale = 1.0;

//			var transform = source.LayoutTransform;
//			source.LayoutTransform = null;
//			// fix margin offset as well
//			Thickness margin = source.Margin;
//			source.Margin = new Thickness(0, 0,margin.Right - margin.Left, margin.Bottom - margin.Top);
//			// Get the size of canvas
//			Size size = new Size(source.ActualWidth, source.ActualHeight);
//			// force control to Update
//			source.Measure(size);
//			source.Arrange(new Rect(size));


			var actualHeight = source.RenderSize.Height;
			var actualWidth = source.RenderSize.Width;

			var renderHeight = actualHeight*scale;
			var renderWidth = actualWidth*scale;

			var renderTarget = new RenderTargetBitmap((int) renderWidth, (int) renderHeight, 96, 96, PixelFormats.Pbgra32);
			var sourceBrush = new VisualBrush(source);

			var drawingVisual = new DrawingVisual();
			var drawingContext = drawingVisual.RenderOpen();

			using (drawingContext) {
				drawingContext.PushTransform(new ScaleTransform(scale, scale));
				drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
			}
			renderTarget.Render(drawingVisual);

//			// return values as they were before
//			source.LayoutTransform = transform;
//			source.Margin = margin;

//			return BitmapFrame.Create(renderTarget);

//			PngBitmapEncoder png = new PngBitmapEncoder();
//			png.Frames.Add(BitmapFrame.Create(rtb));
//			MemoryStream stream = new MemoryStream();
//			png.Save(stream);
//			Image image = Image.FromStream(stream);

			var jpgEncoder = new JpegBitmapEncoder();
			jpgEncoder.QualityLevel = 90;
			jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

			
			Byte[] imageArray;
			using (var outputStream = new MemoryStream()) {
				jpgEncoder.Save(outputStream);
				imageArray = outputStream.ToArray();
			}

			jpgEncoder = new JpegBitmapEncoder();
			jpgEncoder.QualityLevel = 90;
			jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
			using (var outputStream = new FileStream(@"D:\test.jpg",FileMode.Create)) {
				jpgEncoder.Save(outputStream);
			}

//			return _imageArray;

			var bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = new MemoryStream(imageArray);
			bi.EndInit();

			return bi;
		}

		private static bool IsMovementBigEnough(Point initialMousePosition, Point currentPosition) {
			return (Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance);
		}

		#endregion
	}
}
/*
<Button 
	...
	xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
	xmlns:behaviors="clr-namespace:KsWare.Presentation.ViewFramework.Behaviors;assembly=KsWare.Presentation"
>
	<Button.Resources>
		<DataTemplate x:Key="DragDropTemplate" DataType="{x:Type viewModels:EquipmentSlotVM}">
			<controls:EquipmentImage Equipment="{Binding EquipmentId}" Level="{Binding LevelId}" Height="153" Width="153"/>
		</DataTemplate>
	</Button.Resources>
	<i:Interaction.Behaviors>
		<behaviors:FrameworkElementDragBehavior DragDropTemplate="{StaticResource DragDropTemplate}"/>
	</i:Interaction.Behaviors>
...
*/