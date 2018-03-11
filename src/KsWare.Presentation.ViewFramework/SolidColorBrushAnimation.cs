using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace KsWare.Presentation.ViewFramework { // System.Windows.Media.Animation {

	/// <summary> Animates the value of a SolidColorBrush.Color property between two target values using linear interpolation over a specified <see cref="Timeline.Duration"/>. 
	/// </summary>
	public class SolidColorBrushAnimation:ObjectAnimationBase {

		private ColorAnimation _colorAnimation=new ColorAnimation();
		private SolidColorBrush _from;
		private SolidColorBrush _to;

//		public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
//			"From", typeof (SolidColorBrush), typeof (SolidColorBrushAnimation), new FrameworkPropertyMetadata(default(SolidColorBrush), (d, e) => ((SolidColorBrushAnimation) d).FromPropertyChanged(new ValueChangedEventArgs<SolidColorBrush>(e))));
//
//		public SolidColorBrush From { get { return (SolidColorBrush) GetValue(FromProperty); } set { SetValue(FromProperty, value); } }
//
		private void FromPropertyChanged(ValueChangedEventArgs<SolidColorBrush> e) {
			_colorAnimation.From = e.NewValue !=null ? e.NewValue.Color : (Color?)null;
		}
//
//		public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
//			"To", typeof (SolidColorBrush), typeof (SolidColorBrushAnimation), new FrameworkPropertyMetadata(default(SolidColorBrush),(d,e)=>((SolidColorBrushAnimation)d).ToPropertyChanged(new ValueChangedEventArgs<SolidColorBrush>(e))));
//
//		public SolidColorBrush To { get { return (SolidColorBrush) GetValue(ToProperty); } set { SetValue(ToProperty, value); } }
//
		private void ToPropertyChanged(ValueChangedEventArgs<SolidColorBrush> e) {
			_colorAnimation.To = e.NewValue !=null ? e.NewValue.Color : (Color?)null;
		}


		public SolidColorBrush From {
			get { return _from; }
			set { _from = value;  FromPropertyChanged(new ValueChangedEventArgs<SolidColorBrush>(value));}
		}

		public SolidColorBrush To {
			get { return _to; }
			set { _to = value;  ToPropertyChanged(new ValueChangedEventArgs<SolidColorBrush>(value));}
		}
		
		/// <summary> Creates a new instance of the <see cref="SolidColorBrushAnimation"/>. (Overrides Freezable.CreateInstanceCore().)
		/// </summary>
		/// <returns>The new instance.</returns>
		protected override Freezable CreateInstanceCore() {
			return new SolidColorBrushAnimation();
		}


//		protected override void CloneCore(Freezable sourceFreezable) { base.CloneCore(sourceFreezable); }
//
//		protected override void CloneCurrentValueCore(Freezable sourceFreezable) { base.CloneCurrentValueCore(sourceFreezable); }
//
//		/// <summary> Makes this <see cref="T:System.Windows.Media.Animation.Timeline" /> unmodifiable or determines whether it can be made unmodifiable.
//		/// </summary>
//		/// <param name="isChecking">true to check if this instance can be frozen; false to freeze this instance.</param>
//		/// <returns>If <paramref name="isChecking" /> is true, this method returns true if this instance can be made read-only, or false if it cannot be made read-only. If <paramref name="isChecking" /> is false, this method returns true if this instance is now read-only, or false if it cannot be made read-only, with the side effect of having begun to change the frozen status of this object.</returns>
//		protected override bool FreezeCore(bool isChecking) {
//			var retbase= base.FreezeCore(isChecking);
////			_ColorAnimation.Freeze();
//			return true;
//		}
//
//		/// <summary> Makes this instance a clone of the specified <see cref="T:System.Windows.Media.Animation.Timeline" /> object.
//		/// </summary>
//		/// <param name="sourceFreezable">The <see cref="T:System.Windows.Media.Animation.Timeline" /> instance to clone.</param>
//		protected override void GetAsFrozenCore(Freezable sourceFreezable) {
//			var sourceAnimation = (SolidColorBrushAnimation) sourceFreezable;
//			base.GetCurrentValueAsFrozenCore(sourceFreezable);
//			CopyCommon(sourceAnimation);
//		}
//
//		/// <summary> Makes this instance a frozen clone of the specified <see cref="T:System.Windows.Media.Animation.Timeline" />. Resource references, data bindings, and animations are not copied, but their current values are.
//		/// </summary>
//		/// <param name="sourceFreezable">The <see cref="T:System.Windows.Media.Animation.Timeline" /> to copy and freeze.</param>
//		protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable) {
//			var sourceAnimation = (SolidColorBrushAnimation) sourceFreezable;
//			base.GetCurrentValueAsFrozenCore(sourceFreezable);
//			CopyCommon(sourceAnimation);
//		}

		protected override object GetCurrentValueCore(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock) {
//			Color defaultOriginValue_=Colors.Yellow;
//			Color defaultDestinationValue_=Colors.Yellow;
//			if(defaultOriginValue is SolidColorBrush) defaultOriginValue_=((SolidColorBrush)defaultOriginValue).Color;
//			if(defaultOriginValue is Color)defaultOriginValue_=(Color)defaultOriginValue;
//			if(defaultDestinationValue is SolidColorBrush) defaultDestinationValue_=((SolidColorBrush)defaultDestinationValue).Color;
//			if(defaultDestinationValue is Color)defaultDestinationValue_=(Color)defaultDestinationValue;
//			var color =_ColorAnimation.GetCurrentValue(defaultOriginValue_,defaultDestinationValue_,animationClock);

			var color =_colorAnimation.GetCurrentValue(
				((SolidColorBrush)defaultOriginValue).Color,
				((SolidColorBrush)defaultDestinationValue??Brushes.Black).Color,
				animationClock
			);
			return new SolidColorBrush(color);
		}


		private void CopyCommon(SolidColorBrushAnimation sourceAnimation) {
			//m_ColorAnimation = sourceAnimation._ColorAnimation;
			To = sourceAnimation.To;
			From = sourceAnimation.From;
		}
	}

	public static class SolidColorBrushAnimationBinder {


		public static readonly DependencyProperty FromProperty = DependencyProperty.RegisterAttached("From",
			typeof (SolidColorBrush),
			typeof (SolidColorBrushAnimationBinder),
			new PropertyMetadata(null, FromChanged));

		public static void SetFrom(SolidColorBrushAnimation animation, object value) { animation.SetValue(FromProperty, value); }

		public static object GetFrom(SolidColorBrushAnimation animation) { return animation.GetValue(FromProperty); }

		private static void FromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var animation = d as SolidColorBrushAnimation;
			if (animation == null) return;
			animation.From = (SolidColorBrush) e.NewValue;
		}

		public static readonly DependencyProperty ToProperty = DependencyProperty.RegisterAttached(
			"To", typeof (SolidColorBrush), typeof (SolidColorBrushAnimationBinder), new PropertyMetadata(default(SolidColorBrush),ToChanged));

		public static void SetTo(SolidColorBrushAnimation animtion, SolidColorBrush value) { animtion.SetValue(ToProperty, value); }

		public static SolidColorBrush GetTo(SolidColorBrushAnimation animation) { return (SolidColorBrush) animation.GetValue(ToProperty); }

		private static void ToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var animation = d as SolidColorBrushAnimation;
			if (animation == null) return;
			animation.To = (SolidColorBrush) e.NewValue;
		}

	}
}
