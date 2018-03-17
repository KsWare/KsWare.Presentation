using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using M=System.Math; // because OSIS.Math!

namespace KsWare.Presentation.ViewFramework.Controls
{
	//TODO: reuseability: HACK in AtLayoutUpdated Dock is allways set to Bottom

	[TemplatePart(Name="PART_Arrow",Type=typeof(FrameworkElement))]
	public class PopupChrome:HeaderedContentControl
	{
		private static readonly IValueConverter __borderThickness2StrokeThicknessConverter=new MyBorderThickness2StrokeThicknessConverter();

		public static IValueConverter BorderThickness2StrokeThicknessConverter => __borderThickness2StrokeThicknessConverter;

		#region ArrowOffset

		public static readonly DependencyProperty ArrowOffsetProperty =
			DependencyProperty.Register("ArrowOffset", typeof(Double), typeof(PopupChrome), new PropertyMetadata(default(Double)));

		public Double ArrowOffset {
			get => (Double)GetValue(ArrowOffsetProperty);
			set => SetValue(ArrowOffsetProperty, value);
		}
		#endregion

		#region PopupPlacement
		public static readonly DependencyProperty PopupPlacementProperty =
			DependencyProperty.Register("PopupPlacement", typeof(PlacementMode), typeof(PopupChrome), new PropertyMetadata(default(PlacementMode)));

		public PlacementMode PopupPlacement {
			get => (PlacementMode)GetValue(PopupPlacementProperty);
			set => SetValue(PopupPlacementProperty, value);
		}
		#endregion

		#region PlacementTarget
		public static readonly DependencyProperty PlacementTargetProperty =
			DependencyProperty.Register("PlacementTarget", typeof(UIElement), typeof(PopupChrome), new PropertyMetadata(default(UIElement)));

		public UIElement PlacementTarget {
			get => (UIElement)GetValue(PlacementTargetProperty);
			set => SetValue(PlacementTargetProperty, value);
		}
		#endregion

		private Popup _popup;
		private ContextMenu _contextMenu;
		private FrameworkElement PART_Arrow;
		private Window _window;


		/// <summary> Initializes a new instance of the <see cref="PopupChrome"/> class.
		/// </summary>
		public PopupChrome() {
			LayoutUpdated+=AtLayoutUpdated;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private Popup ParentPopup {
			get {
				if (InDesignMode) return null;
				if(_popup!=null) return _popup;
				DependencyObject e = this;

				while (e!=null) {
					if(e is ContextMenu) return null; //not in a Popup
					if(e is Window     ) return null; //not in a Popup
					if(e is Popup      ) return _popup=(Popup) e;
					e=LogicalTreeHelper.GetParent(e);
				}
				return null;
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private ContextMenu ParentContextMenu {
			get {
				if (InDesignMode) return null;
				if(_contextMenu!=null) return _contextMenu;
				DependencyObject e = this;

				while (e!=null) {
					if(e is Popup      ) return null; //not in a ContextMenu
					if(e is Window     ) return null; //not in a ContextMenu
					if(e is ContextMenu) return _contextMenu=(ContextMenu) e;
					e=VisualTreeHelper.GetParent(e);
				}
				return null;
			}
		}
		
		private void AtLayoutUpdated(object sender, EventArgs eventArgs) {
			if(!IsVisible) return;

			var arrowDock      = GetDock(PopupPlacement);
			var arrowOffset    = ArrowOffset;
			var d              = (double)BorderThickness2StrokeThicknessConverter.Convert(BorderThickness, typeof(double), null, null);

			if(ParentPopup!=null) {
				PlacementTarget = ParentPopup.PlacementTarget;
				if(ParentPopup.Placement==PlacementMode.Custom && ParentPopup.CustomPopupPlacementCallback==null) {
					ParentPopup.CustomPopupPlacementCallback=CustomPlacementCallback;
				}
			} else if(ParentContextMenu!=null) {
				PlacementTarget = ParentContextMenu.PlacementTarget;
				if(ParentContextMenu.Placement==PlacementMode.Custom && ParentContextMenu.CustomPopupPlacementCallback==null) {
					ParentContextMenu.CustomPopupPlacementCallback=CustomPlacementCallback;
				}
			}

			if(PlacementTarget!=null) {
				if (!PlacementTarget.IsVisible || !PlacementTarget.IsArrangeValid)
			        return;
				var target = new Rect(PlacementTarget.PointToScreen(new Point(0, 0)),PlacementTarget.RenderSize);
				var popup  = new Rect(PointToScreen(new Point(0, 0)),RenderSize);
				var t2     = target.Width/2.0;
				var a2     = PART_Arrow.RenderSize.Width/2.0;
				arrowDock  = CalcDock(target, popup);

				arrowDock=Dock.Bottom; //HACK against bug

				switch(arrowDock) {
					case Dock.Left  :
					case Dock.Right : arrowOffset = target.Top -popup.Top  +t2-a2;break;
					case Dock.Top   : 
					case Dock.Bottom: arrowOffset = target.Left-popup.Left +t2-a2;break;
					default         : arrowOffset=0; break;
				}
			}

			if(arrowOffset<0) arrowOffset=0;
			if(arrowOffset>RenderSize.Width-PART_Arrow.RenderSize.Width) arrowOffset = RenderSize.Width-PART_Arrow.RenderSize.Width;

			switch(arrowDock) {
				case Dock.Right: 
					PART_Arrow.Margin=new Thickness(-d,arrowOffset,0,0);
					Grid.SetColumn(PART_Arrow,2);Grid.SetRow(PART_Arrow,1);
					Rotate(270);
					break;
				case Dock.Left: 
					Grid.SetColumn(PART_Arrow,0);Grid.SetRow(PART_Arrow,1);
					PART_Arrow.Margin=new Thickness(0,arrowOffset,-d,0);
					Rotate(90);
					break;
				case Dock.Bottom : 
					Grid.SetColumn(PART_Arrow,1);Grid.SetRow(PART_Arrow,2);
					PART_Arrow.Margin=new Thickness(arrowOffset,-d,0,0);
					Rotate(0);
					break;
				case Dock.Top: 
					Grid.SetColumn(PART_Arrow,1);Grid.SetRow(PART_Arrow,0);
					PART_Arrow.Margin=new Thickness(arrowOffset,0,0,-d);
					Rotate(180);
					break;
				default: 
					PART_Arrow.Margin=new Thickness(0,0,0,0);
					break;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private Window ParentWindow {
			get {
				if(InDesignMode) return null;
				if(PlacementTarget==null) return null;
				if(_window!=null) return _window;
				DependencyObject e = PlacementTarget;

				while (e!=null) {
					if(e is Window     ) return _window=(Window) e;
					e=VisualTreeHelper.GetParent(e);
				}
				return null;
			}
		}

		/// <summary> Placement.Top + HorizontalAlignment.Right relativ to parent window (instead of screen)
		/// </summary>
		/// <param name="popupSize"></param>
		/// <param name="targetSize"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		private CustomPopupPlacement[] CustomPlacementCallback(Size popupSize, Size targetSize, Point offset) {
			if(PlacementTarget==null) return new []{new CustomPopupPlacement(new Point(-popupSize.Width+targetSize.Width,popupSize.Height), PopupPrimaryAxis.Vertical)};
			var margin = new Thickness(0, 0, 10, 0);
			var btnPos  = PlacementTarget.PointToScreen(new Point(0, 0));
			var winPos = ParentWindow.PointToScreen(new Point(0, 0));
			
			//var relPos = new Point(0, -popupSize.Height);//Allign left at top of PlacementTarget
			var relPos = new Point((targetSize.Width-popupSize.Width)/2, -popupSize.Height);//Allign center at top of PlacementTarget

			var rightOverlap = (btnPos.X+relPos.X+popupSize.Width)>winPos.X+ParentWindow.RenderSize.Width-margin.Right;
			var leftOverpap = (btnPos.X+relPos.X)<winPos.X+margin.Left;

			if(rightOverlap&!leftOverpap) {
				var t = PlacementTarget.PointToScreen(new Point(0, 0));
				var w=ParentWindow.PointToScreen(new Point(ParentWindow.RenderSize.Width, 0));
				relPos=new Point(-popupSize.Width+(w.X-t.X)-margin.Right,-popupSize.Height);				
			}

			return new [] {new CustomPopupPlacement(relPos,PopupPrimaryAxis.Vertical )};
		}

		private static Dock CalcDock(Rect target, Rect popup) {
			if(popup.Bottom>=target.Top) return Dock.Bottom;
			if(popup.Top>=target.Bottom) return Dock.Top;

			var x1 = target.X+target.Width/2.0;
			var y1 = target.Y+target.Height/2.0;
			var x2 = popup.X+popup.Width/2.0;
			var y2 = popup.Y+popup.Height/2.0;

			var a = M.Atan2((y2-y1),(x2-x1))* 180.0/M.PI;
			var dock=(Dock)(-1);
			if( a>= -45 && a<  45) dock= Dock.Left;
			else if( a>=  45 && a< 135) dock= Dock.Top;
			else if( a<= -45 && a>-135) dock= Dock.Bottom;
			else dock= Dock.Right;
			return dock;
		}

		private static Dock GetDock(PlacementMode placementMode) {
			switch(placementMode) {
				case PlacementMode.Left  : return Dock.Right;
				case PlacementMode.Right : return Dock.Left;
				case PlacementMode.Top   : return Dock.Bottom;
				case PlacementMode.Bottom: return Dock.Top;
				default                  : return (Dock)(-1);
			}
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();

			PART_Arrow=(Template.FindName("PART_Arrow", this) as FrameworkElement)??new FrameworkElement();

			PART_Arrow.HorizontalAlignment = HorizontalAlignment.Left;
			PART_Arrow.VerticalAlignment   = VerticalAlignment.Top;
		}

		private void Rotate(int angle) {
			var transform = PART_Arrow.LayoutTransform as RotateTransform;
			if(transform==null) {
				PART_Arrow.LayoutTransform=new RotateTransform(angle);
			} else {
				if(M.Abs(transform.Angle-angle)<0.1)return;
				transform.Angle = angle;
			}
		}

		/// <summary>  Indicates whether or not the framework is in design-time mode.
		/// </summary>        
		public static bool InDesignMode {
			get {
			    var prop = DesignerProperties.IsInDesignModeProperty;
			    var inDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

			    if (!inDesignMode && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
			        inDesignMode = true;

			    return inDesignMode;
			}
		}

		#region Converter

		private class MyBorderThickness2StrokeThicknessConverter:IValueConverter
		{

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
				var t = (Thickness)value;
				return M.Max(M.Max(t.Left, t.Right), M.Max(t.Top, t.Bottom));
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
				throw new NotImplementedException();
			}

		}

		#endregion
	}
}
/* 
			                                                                             
	 OK             OK             !!             !!             OK             !!             !! 
			 |              |              |              |              |              |                 
┌────┐╔════╗ | ┌────┐       | ┌────┐       | ┌────┐       |       ╔════╗ |       ╔════╗ |       ╔════╗
│    │║    ║ | │    │╔════╗ | │    │       | │    │       | ┌────┐║    ║ |       ║    ║ |       ║    ║
└────┘╚════╝ | └────┘║    ║ | └────┘╔════╗ | └────┘       | │    │╚════╝ | ┌────┐╚════╝ |       ╚════╝
			 |       ╚════╝ |       ║    ║ |       ╔════╗ | └────┘       | │    │       | ┌────┐   
			 |              |       ╚════╝ |       ║    ║ |              | └────┘       | │    │   
			 |              |              |       ╚════╝ |              |              | └────┘        
			                                                                                           
			                                                                                         
		|          |           |               
┌────┐  | ┌────┐   | ┌────┐    | ┌────┐
│╔═══╪╗ | │ ╔══╪═╗ | │  ╔═╪══╗ | │   ╔╪═══╗  
└╫───┘║ | └─╫──┘ ║ | └──╫─┘  ║ | └───╫┘   ║
 ╚════╝ |   ╚════╝ |    ╚════╝ |     ╚════╝
		|          |           |            













*/