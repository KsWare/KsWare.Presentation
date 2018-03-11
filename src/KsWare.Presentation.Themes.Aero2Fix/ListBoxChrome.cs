// Type: Microsoft.Windows.Themes.ListBoxChrome
// Assembly: PresentationFramework.Aero2, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C98188E3-4E0A-48B2-B299-7C58CA504797
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\PresentationFramework.Aero2\v4.0_4.0.0.0__31bf3856ad364e35\PresentationFramework.Aero2.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace KsWare.Presentation.Themes.Aero2Fix { /*Microsoft.Windows.Themes*/


	public sealed class ListBoxChrome : Decorator {

		public static readonly DependencyProperty BackgroundProperty = Control.BackgroundProperty.AddOwner(typeof (ListBoxChrome), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty BorderBrushProperty = Border.BorderBrushProperty.AddOwner(typeof (ListBoxChrome), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty BorderThicknessProperty = Border.BorderThicknessProperty.AddOwner(typeof (ListBoxChrome), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(1.0), FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register("RenderMouseOver", typeof (bool), typeof (ListBoxChrome), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(ListBoxChrome.OnRenderMouseOverChanged)));
		public static readonly DependencyProperty RenderFocusedProperty = DependencyProperty.Register("RenderFocused", typeof (bool), typeof (ListBoxChrome), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(ListBoxChrome.OnRenderFocusedChanged)));
		private static object _resourceAccess = new object();
		private static SolidColorBrush _commonDisabledBackgroundOverlay;
		private static Pen _commonBorderPen;
		private static Pen _commonDisabledBorderOverlay;
		private static Pen _commonHoverBorderOverlay;
		private static Pen _commonFocusedBorderOverlay;
		private ListBoxChrome.LocalResources _localResources;

		public Brush Background { get { return (Brush) this.GetValue(ListBoxChrome.BackgroundProperty); } set { this.SetValue(ListBoxChrome.BackgroundProperty, (object) value); } }

		public Brush BorderBrush { get { return (Brush) this.GetValue(ListBoxChrome.BorderBrushProperty); } set { this.SetValue(ListBoxChrome.BorderBrushProperty, (object) value); } }

		public Thickness BorderThickness { get { return (Thickness) this.GetValue(ListBoxChrome.BorderThicknessProperty); } set { this.SetValue(ListBoxChrome.BorderThicknessProperty, (object) value); } }

		public bool RenderMouseOver { get { return (bool) this.GetValue(ListBoxChrome.RenderMouseOverProperty); } set { this.SetValue(ListBoxChrome.RenderMouseOverProperty, value); } }

		public bool RenderFocused { get { return (bool) this.GetValue(ListBoxChrome.RenderFocusedProperty); } set { this.SetValue(ListBoxChrome.RenderFocusedProperty, value); } }

		internal /*override*/ int EffectiveValuesInitialSize {
			get { return 9; }
		}

		private bool Animates {
			get {
				if (SystemParameters.ClientAreaAnimation && RenderCapability.Tier > 0) return this.IsEnabled;
				else return false;
			}
		}

		private static SolidColorBrush CommonDisabledBackgroundOverlay {
			get {
				if (ListBoxChrome._commonDisabledBackgroundOverlay == null) {
					lock (ListBoxChrome._resourceAccess) {
						if (ListBoxChrome._commonDisabledBackgroundOverlay == null) {
							SolidColorBrush local_0 = new SolidColorBrush(Color.FromRgb((byte) 244, (byte) 244, (byte) 244));
							local_0.Freeze();
							ListBoxChrome._commonDisabledBackgroundOverlay = local_0;
						}
					}
				}
				return ListBoxChrome._commonDisabledBackgroundOverlay;
			}
		}

		private static Pen CommonHoverBorderOverlay {
			get {
				if (ListBoxChrome._commonHoverBorderOverlay == null) {
					lock (ListBoxChrome._resourceAccess) {
						if (ListBoxChrome._commonHoverBorderOverlay == null) {
							Pen local_0 = new Pen();
							local_0.Thickness = 1.0;
							LinearGradientBrush local_1 = new LinearGradientBrush();
							local_1.StartPoint = new Point(0.0, 0.0);
							local_1.EndPoint = new Point(0.0, 20.0);
							local_1.MappingMode = BrushMappingMode.Absolute;
							local_1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 87, (byte) 148, (byte) 191), 0.05));
							local_1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 183, (byte) 213, (byte) 234), 0.07));
							local_1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 199, (byte) 226, (byte) 241), 1.0));
							local_0.Brush = (Brush) local_1;
							local_0.Freeze();
							ListBoxChrome._commonHoverBorderOverlay = local_0;
						}
					}
				}
				return ListBoxChrome._commonHoverBorderOverlay;
			}
		}

		private static Pen CommonFocusedBorderOverlay {
			get {
				if (ListBoxChrome._commonFocusedBorderOverlay == null) {
					lock (ListBoxChrome._resourceAccess) {
						if (ListBoxChrome._commonFocusedBorderOverlay == null) {
							Pen local_0 = new Pen();
							local_0.Thickness = 1.0;
							LinearGradientBrush local_1 = new LinearGradientBrush();
							local_1.StartPoint = new Point(0.0, 0.0);
							local_1.EndPoint = new Point(0.0, 20.0);
							local_1.MappingMode = BrushMappingMode.Absolute;
							local_1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 61, (byte) 123, (byte) 173), 0.05));
							local_1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 164, (byte) 201, (byte) 227), 0.07));
							local_1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 183, (byte) 217, (byte) 237), 1.0));
							local_0.Brush = (Brush) local_1;
							local_0.Freeze();
							ListBoxChrome._commonFocusedBorderOverlay = local_0;
						}
					}
				}
				return ListBoxChrome._commonFocusedBorderOverlay;
			}
		}

		private static Pen CommonDisabledBorderOverlay {
			get {
				if (ListBoxChrome._commonDisabledBorderOverlay == null) {
					lock (ListBoxChrome._resourceAccess) {
						if (ListBoxChrome._commonDisabledBorderOverlay == null) {
							Pen local_0 = new Pen();
							local_0.Thickness = 1.0;
							local_0.Brush = (Brush) new SolidColorBrush(Color.FromRgb((byte) 173, (byte) 178, (byte) 181));
							local_0.Freeze();
							ListBoxChrome._commonDisabledBorderOverlay = local_0;
						}
					}
				}
				return ListBoxChrome._commonDisabledBorderOverlay;
			}
		}

		private Brush BackgroundOverlay {
			get {
				if (!this.IsEnabled) return (Brush) ListBoxChrome.CommonDisabledBackgroundOverlay;
				else return (Brush) null;
			}
		}

		private Pen BorderOverlayPen {
			get {
				if (!this.IsEnabled) return ListBoxChrome.CommonDisabledBorderOverlay;
				if (this._localResources != null) {
					if (this._localResources.BorderOverlayPen == null) {
						this._localResources.BorderOverlayPen = ListBoxChrome.CommonHoverBorderOverlay.Clone();
						this._localResources.BorderOverlayPen.Brush.Opacity = 0.0;
					}
					return this._localResources.BorderOverlayPen;
				} else {
					if (this.RenderFocused) return ListBoxChrome.CommonFocusedBorderOverlay;
					if (this.RenderMouseOver) return ListBoxChrome.CommonHoverBorderOverlay;
					else return (Pen) null;
				}
			}
		}

		static ListBoxChrome() { UIElement.IsEnabledProperty.OverrideMetadata(typeof (ListBoxChrome), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.AffectsRender)); }

		private static void OnRenderMouseOverChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ListBoxChrome listBoxChrome = (ListBoxChrome) o;
			if (listBoxChrome.RenderFocused) return;
			if (listBoxChrome.Animates) {
				if ((bool) e.NewValue) {
					if (listBoxChrome._localResources == null) {
						listBoxChrome._localResources = new ListBoxChrome.LocalResources();
						listBoxChrome.InvalidateVisual();
					}
					DoubleAnimation doubleAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(0.3)));
					listBoxChrome.BorderOverlayPen.Brush.BeginAnimation(Brush.OpacityProperty, (AnimationTimeline) doubleAnimation);
				} else if (listBoxChrome._localResources == null) { listBoxChrome.InvalidateVisual(); } else {
					Duration duration = new Duration(TimeSpan.FromSeconds(0.2));
					DoubleAnimation doubleAnimation = new DoubleAnimation();
					doubleAnimation.Duration = duration;
					listBoxChrome.BorderOverlayPen.Brush.BeginAnimation(Brush.OpacityProperty, (AnimationTimeline) doubleAnimation);
				}
			} else {
				listBoxChrome._localResources = (ListBoxChrome.LocalResources) null;
				listBoxChrome.InvalidateVisual();
			}
		}

		private static void OnRenderFocusedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ListBoxChrome listBoxChrome = (ListBoxChrome) o;
			listBoxChrome._localResources = (ListBoxChrome.LocalResources) null;
			listBoxChrome.InvalidateVisual();
		}

		protected override Size MeasureOverride(Size availableSize) {
			Thickness borderThickness = this.BorderThickness;
			double val1_1 = borderThickness.Left + borderThickness.Right;
			double val1_2 = borderThickness.Top + borderThickness.Bottom;
			if (val1_1 > 0.0 || val1_2 > 0.0) {
				val1_1 += 2.0;
				val1_2 += 2.0;
			}
			UIElement child = this.Child;
			Size size;
			if (child != null) {
				Size availableSize1 = new Size();
				bool flag1 = availableSize.Width < val1_1;
				bool flag2 = availableSize.Height < val1_2;
				if (!flag1) availableSize1.Width = availableSize.Width - val1_1;
				if (!flag2) availableSize1.Height = availableSize.Height - val1_2;
				child.Measure(availableSize1);
				size = child.DesiredSize;
				if (!flag1) size.Width += val1_1;
				if (!flag2) size.Height += val1_2;
			} else size = new Size(Math.Min(val1_1, availableSize.Width), Math.Min(val1_2, availableSize.Height));
			return size;
		}

		protected override Size ArrangeOverride(Size finalSize) {
			Rect finalRect = new Rect();
			Thickness borderThickness = this.BorderThickness;
			double num1 = borderThickness.Left + borderThickness.Right;
			double num2 = borderThickness.Top + borderThickness.Bottom;
			if (num1 > 0.0 || num2 > 0.0) {
				++borderThickness.Left;
				++borderThickness.Top;
				num1 += 2.0;
				num2 += 2.0;
			}
			if (finalSize.Width > num1 && finalSize.Height > num2) {
				finalRect.X = borderThickness.Left;
				finalRect.Y = borderThickness.Top;
				finalRect.Width = finalSize.Width - num1;
				finalRect.Height = finalSize.Height - num2;
			}
			if (this.Child != null) this.Child.Arrange(finalRect);
			return finalSize;
		}

		protected override void OnRender(DrawingContext dc) {
			Rect bounds = new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight);
			Thickness borderThickness = this.BorderThickness;
			double num1 = borderThickness.Left + borderThickness.Right;
			double num2 = borderThickness.Top + borderThickness.Bottom;
			bool flag = borderThickness.Left == 1.0 && borderThickness.Right == 1.0 && borderThickness.Top == 1.0 && borderThickness.Bottom == 1.0;
			double num3 = num1 != 0.0 || num2 != 0.0 ? 1.0 : 0.0;
			double num4 = 2.0*num3;
			if (bounds.Width > num1 && bounds.Height > num2) {
				Rect rectangle = new Rect(bounds.Left + borderThickness.Left, bounds.Top + borderThickness.Top, bounds.Width - num1, bounds.Height - num2);
				Brush background = this.Background;
				if (background != null) dc.DrawRectangle(background, (Pen) null, rectangle);
				if (bounds.Width > num1 + num4 && bounds.Height > num2 + num4) {
					rectangle = new Rect(bounds.Left + borderThickness.Left + num3, bounds.Top + borderThickness.Top + num3, bounds.Width - num1 - num4, bounds.Height - num2 - num4);
					Brush backgroundOverlay = this.BackgroundOverlay;
					if (backgroundOverlay != null) dc.DrawRoundedRectangle(backgroundOverlay, (Pen) null, rectangle, 1.0, 1.0);
				}
			}
			if (num3 <= 0.0 || bounds.Width < num1 || bounds.Height < num2) return;
			if (flag) {
				Rect rectangle = new Rect(bounds.Left + 0.5, bounds.Top + 0.5, bounds.Width - 1.0, bounds.Height - 1.0);
				Pen borderPen = ListBoxChrome.GetBorderPen(this.BorderBrush);
				Pen borderOverlayPen = this.BorderOverlayPen;
				if (borderPen != null) dc.DrawRoundedRectangle((Brush) null, borderPen, rectangle, 1.0, 1.0);
				if (borderOverlayPen == null) return;
				dc.DrawRoundedRectangle((Brush) null, borderOverlayPen, rectangle, 1.0, 1.0);
			} else {
				Geometry borderGeometry = ListBoxChrome.GetBorderGeometry(borderThickness, bounds);
				if (this.BorderBrush == null) return;
				dc.DrawGeometry(this.BorderBrush, (Pen) null, borderGeometry);
			}
		}

		private static Pen GetBorderPen(Brush border) {
			Pen pen = (Pen) null;
			if (border != null) {
				if (ListBoxChrome._commonBorderPen == null) {
					lock (ListBoxChrome._resourceAccess) {
						if (ListBoxChrome._commonBorderPen == null) {
							if (!border.IsFrozen && border.CanFreeze) {
								border = border.Clone();
								border.Freeze();
							}
							Pen local_1 = new Pen(border, 1.0);
							if (local_1.CanFreeze) {
								local_1.Freeze();
								ListBoxChrome._commonBorderPen = local_1;
							}
						}
					}
				}
				if (ListBoxChrome._commonBorderPen != null && border == ListBoxChrome._commonBorderPen.Brush) { pen = ListBoxChrome._commonBorderPen; } else {
					if (!border.IsFrozen && border.CanFreeze) {
						border = border.Clone();
						border.Freeze();
					}
					pen = new Pen(border, 1.0);
					if (pen.CanFreeze) pen.Freeze();
				}
			}
			return pen;
		}

		private static Geometry GetBorderGeometry(Thickness thickness, Rect bounds) {
			return (Geometry) new PathGeometry() {
				Figures = {
					new PathFigure() {
						StartPoint = new Point(bounds.Left, bounds.Top),
						Segments = {
							(PathSegment) new LineSegment(new Point(bounds.Left, bounds.Bottom), false),
							(PathSegment) new LineSegment(new Point(bounds.Right, bounds.Bottom), false),
							(PathSegment) new LineSegment(new Point(bounds.Right, bounds.Top), false)
						},
						IsClosed = true
					},
					new PathFigure() {
						StartPoint = new Point(bounds.Left + thickness.Left, bounds.Top + thickness.Top),
						Segments = {
							(PathSegment) new LineSegment(new Point(bounds.Left + thickness.Left, bounds.Bottom - thickness.Bottom), false),
							(PathSegment) new LineSegment(new Point(bounds.Right - thickness.Right, bounds.Bottom - thickness.Bottom), false),
							(PathSegment) new LineSegment(new Point(bounds.Right - thickness.Right, bounds.Top + thickness.Top), false)
						},
						IsClosed = true
					}
				}
			};
		}

		private class LocalResources {

			public Pen BorderOverlayPen;

		}

	}

}