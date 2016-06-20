using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior 
{

	/// <summary> [DRAFT] Binding for focus members
	/// </summary>
	/// <remarks>
	/// This class will map focus properties/methods to a bindable property (<see cref="IsKeyboardFocusedProperty"/>)
	/// </remarks>
	public static class FocusBehavior
	{
		// 

		#region focus members
		//	TextBox t;
		//	t.Focus()                        ;  method
		//	t.IsFocused                      ?  get
		//	t.IsKeyboardFocused              ?  get
		//	t.IsKeyboardFocusedChanged       += event
		//	t.IsKeyboardFocusWithin          ?  get
		//	t.IsKeyboardFocusedWithinChanged += event
		//	t.GotFocus                       += event
		//	t.GotKeyboardFocus               += event
		//	t.Focusable                      =  get/set
		//	t.FocusableChanged               += event
		#endregion

		#region IsKeyboardFocused Property

		//		private static Dictionary<int,WeakReference> s_InitializedOwners =new Dictionary<int, WeakReference>();
		

		#region private OwnerIsKeyboardFocused Property

		private static readonly DependencyProperty OwnerIsKeyboardFocusedProperty = DependencyProperty.RegisterAttached(
			"OwnerIsKeyboardFocused", typeof (bool), typeof (FocusBehavior),
			new PropertyMetadata(false, (o,e) => SetIsKeyboardFocused(o, (bool)e.NewValue))
		);

//		private static bool GetOwnerIsKeyboardFocused(DependencyObject d) { return (bool) d.GetValue(OwnerIsKeyboardFocusedProperty); }
//		private static void SetOwnerIsKeyboardFocused(DependencyObject d, bool value) { d.SetValue(OwnerIsKeyboardFocusedProperty, value); }

		#endregion

		/// <summary>IsKeyboardFocused </summary>
		public static readonly DependencyProperty IsKeyboardFocusedProperty = DependencyProperty.RegisterAttached(
			"IsKeyboardFocused", typeof (bool), typeof (FocusBehavior),
			new FrameworkPropertyMetadata(false, AtIsKeyboardFocusedChanged, AtIsKeyboardFocusedCoerceValue){BindsTwoWayByDefault=true}
		);

		private static object AtIsKeyboardFocusedCoerceValue(DependencyObject o, object value) {

			// A)
			if(BindingOperations.GetBinding(o, OwnerIsKeyboardFocusedProperty)==null) {
				BindingOperations.SetBinding(o, OwnerIsKeyboardFocusedProperty,
					new Binding("IsKeyboardFocused") {Source = o, Mode = BindingMode.OneWay}
				);
			}

			#region B)
			//	var hashCode = o.GetHashCode();
			//	if(!s_InitializedOwners.ContainsKey(hashCode)) {
			//		s_InitializedOwners.Add(hashCode,new WeakReference(o));
			//		BindingOperations.SetBinding(o, OwnerIsKeyboardFocusedProperty,
			//			new Binding("IsKeyboardFocused") {Source = o, Mode = BindingMode.OneWay}
			//		);
			//	}
			//	//TODO cleanup InitializedOwners
			#endregion

			return value;
		}

		/// <summary> Gets a value indicating keyboard focused.
		/// </summary>
		/// <param name="d">The DependencyObject</param>
		/// <returns><see langword="true"/> keyboard is focused; else <see langword="false"/></returns>
		/// <remarks></remarks>
		public static bool GetIsKeyboardFocused(DependencyObject d) { return (bool) d.GetValue(IsKeyboardFocusedProperty); }

		/// <summary> Sets the keyboard focus
		/// </summary>
		/// <param name="d">The DependencyObject</param>
		/// <param name="value">The new value</param>
		[UsedImplicitly]
		public static void SetIsKeyboardFocused(DependencyObject d, bool value) { d.SetValue(IsKeyboardFocusedProperty, value); }

		private static void AtIsKeyboardFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if((bool) e.NewValue) {
				var uiElement = ((UIElement)d);
				if(!uiElement.Focusable) SetIsKeyboardFocused(d, false); //reset bound property to false
				else if (!uiElement.IsKeyboardFocused) uiElement.Focus();
			}
		}

		#region PerformanceTest for AtIsKeyboardFocusedCoerceValue 
//		private static void OnIsKeyboardFocusedCoerceValuePerformanceTest() {
//			const int loops = 10000000;
//			var o = new TextBox();
//			TimeSpan t0, t1, t2_0, t2_10, t2_100, t2_1000;
//
//			var start = DateTime.Now;
//			for (int i = 0; i < loops;i++ ) {
//				
//			}
//			t0=new TimeSpan(DateTime.Now.Ticks-start.Ticks);
//
//			start = DateTime.Now;
//			for (int i = 0; i < loops;i++ ) {
//				BindingOperations.GetBinding(o, OwnerIsKeyboardFocusedProperty);
//			}
//			t1=new TimeSpan(DateTime.Now.Ticks-start.Ticks);
//
//			start = DateTime.Now;
//			for (int i = 0; i < loops;i++ ) {
//				var hashCode = o.GetHashCode();
//				s_InitializedOwners.ContainsKey(hashCode);
//			}
//			t2_0=new TimeSpan(DateTime.Now.Ticks-start.Ticks);
//
//			for (int i = s_InitializedOwners.Count; i < 10; i++) {var tb=new TextBox();s_InitializedOwners.Add(tb.GetHashCode(),new WeakReference(tb));}
//			start = DateTime.Now;
//			for (int i = 0; i < loops;i++ ) {
//				var hashCode = o.GetHashCode();
//				s_InitializedOwners.ContainsKey(hashCode);
//			}
//			t2_10=new TimeSpan(DateTime.Now.Ticks-start.Ticks);
//
//			for (int i = s_InitializedOwners.Count; i < 100; i++) {var tb=new TextBox();s_InitializedOwners.Add(tb.GetHashCode(),new WeakReference(tb));}
//			start = DateTime.Now;
//			for (int i = 0; i < loops;i++ ) {
//				var hashCode = o.GetHashCode();
//				s_InitializedOwners.ContainsKey(hashCode);
//			}
//			t2_100=new TimeSpan(DateTime.Now.Ticks-start.Ticks);
//
//			for (int i = s_InitializedOwners.Count; i < 1000; i++) {var tb=new TextBox();s_InitializedOwners.Add(tb.GetHashCode(),new WeakReference(tb));}
//			start = DateTime.Now;
//			for (int i = 0; i < loops;i++ ) {
//				var hashCode = o.GetHashCode();
//				s_InitializedOwners.ContainsKey(hashCode);
//			}
//			t2_1000=new TimeSpan(DateTime.Now.Ticks-start.Ticks);
//
		//			Debug.WriteLine("=>{0,-10} {1} {2,3:N0}x {3,5:N1}ns","0"     ,t0      ,1                    , (double)t0     .Ticks/loops*100);
		//			Debug.WriteLine("=>{0,-10} {1} {2,3:N0}x {3,5:N1}ns","1"     ,t1     ,t1.Ticks/t0.Ticks     , (double)t1     .Ticks/loops*100);
		//			Debug.WriteLine("=>{0,-10} {1} {2,3:N0}x {3,5:N1}ns","2:0"   ,t2_0   ,t2_0.Ticks/t0.Ticks   , (double)t2_0   .Ticks/loops*100);
		//			Debug.WriteLine("=>{0,-10} {1} {2,3:N0}x {3,5:N1}ns","2:10"  ,t2_10  ,t2_10.Ticks/t0.Ticks  , (double)t2_10  .Ticks/loops*100);
		//			Debug.WriteLine("=>{0,-10} {1} {2,3:N0}x {3,5:N1}ns","2:100" ,t2_100 ,t2_100.Ticks/t0.Ticks , (double)t2_100 .Ticks/loops*100);
		//			Debug.WriteLine("=>{0,-10} {1} {2,3:N0}x {3,5:N1}ns","2:1000",t2_1000,t2_1000.Ticks/t0.Ticks, (double)t2_1000.Ticks/loops*100);
//		}
		#endregion

		#endregion
	}
}
