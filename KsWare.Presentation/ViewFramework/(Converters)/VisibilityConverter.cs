using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class VisibilityConverter:IValueConverter {

		private static DependencyObject s_DependencyObject=new DependencyObject();

		public static VisibilityConverter TrueVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter TrueVisibleElseCollapsed_DesignerTrue { get; private set; }
		public static VisibilityConverter FalseVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter TrueVisibleElseHidden { get; private set; }
		public static VisibilityConverter FalseVisibleElseHidden { get; private set; }
		public static VisibilityConverter NullCollapsedElseVisible { get; private set; }
		public static VisibilityConverter NullHiddenElseVisible { get; private set; }
		public static VisibilityConverter NullVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter NullVisibleElseHidden { get; private set; }
		public static VisibilityConverter FalseCollapsedElseVisible { get; private set; }

		/// <summary> Returns <see cref="Visibility.Visible"/> the value is Null or 0; else <see cref="Visibility.Hidden"/>
		/// </summary>
		/// <value>The value</value>
		/// <example>Usage: <code>&lt;Grid Visibility="{Binding MyListProperty.Count, Converter={x:Static vfw:VisibilityConverter.NullOr0VisibleElseHidden}}"&gt;</code></example>
		public static VisibilityConverter NullOr0VisibleElseHidden { get; private set; }

		public static VisibilityConverter EqualVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter EqualCollapsedElseVisible { get; private set; }

		private const Visibility Visible   = Visibility.Visible;
		private const Visibility Collapsed = Visibility.Collapsed;
		private const Visibility Hidden    = Visibility.Hidden;
		private const Value IsTrue    = Value.IsTrue;
		private const Value IsFalse    = Value.IsFalse;
		private const Value IsNull    = Value.IsNull;
		private const Value IsEqual    = Value.IsEqual;
		private const Value IsNullOr0    = Value.IsNullOr0;

		static VisibilityConverter() {
			TrueVisibleElseCollapsed              =new VisibilityConverter(IsTrue   ,Visible  ,Collapsed);
			TrueVisibleElseCollapsed_DesignerTrue =new VisibilityConverter(IsTrue   ,Visible  ,Collapsed) {Designer=true};
			FalseVisibleElseCollapsed             =new VisibilityConverter(IsFalse  ,Visible  ,Collapsed);
			TrueVisibleElseHidden                 =new VisibilityConverter(IsTrue   ,Visible  ,Hidden   );
			FalseVisibleElseHidden                =new VisibilityConverter(IsFalse  ,Visible  ,Hidden   );
			NullCollapsedElseVisible              =new VisibilityConverter(IsNull   ,Collapsed,Visible  );
			NullHiddenElseVisible                 =new VisibilityConverter(IsNull   ,Hidden   ,Visible  );
			FalseCollapsedElseVisible             =new VisibilityConverter(IsFalse  ,Collapsed,Visible  );
			EqualVisibleElseCollapsed             =new VisibilityConverter(IsEqual  ,Visible  ,Collapsed);
			EqualCollapsedElseVisible             =new VisibilityConverter(IsEqual  ,Collapsed,Visible  );
			NullOr0VisibleElseHidden              =new VisibilityConverter(IsNullOr0,Visible  ,Hidden   );
			NullVisibleElseCollapsed              =new VisibilityConverter(IsNull   ,Visible  ,Collapsed);
			NullVisibleElseHidden                 =new VisibilityConverter(IsNull   ,Visible  ,Hidden   );
		}

		public VisibilityConverter() {
			Designer = DependencyProperty.UnsetValue;
		}

		private VisibilityConverter(Value condition) {
			Designer = DependencyProperty.UnsetValue;
		}

		private VisibilityConverter(Value condition, Visibility conditionIsTrue, Visibility conditionIsFalse) {
			Designer = DependencyProperty.UnsetValue;
			Condition = condition;
			True = conditionIsTrue;
			Else = conditionIsFalse;
		}

		private enum Value {
			IsNull,
			IsNullOr0,
			IsTrue,
			IsFalse,
			IsEqual,
//			IsNotEqual,

		}

		private Visibility True { get; set; }

		private Visibility Else { get; set; }

		private Value Condition { get; set; }

		private object Designer { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			if (IsInDesignMode && Designer != DependencyProperty.UnsetValue) value = Designer;

			switch (Condition) {
				case IsTrue:case IsEqual: return GetBool(value,parameter) ? True : Else; 
				case IsFalse            : return ! GetBool(value,parameter) ? True : Else;
				case IsNull             : return GetIsNull(value,parameter) ? True : Else;
				case IsNullOr0          : return GetIsNullOr0(value,parameter) ? True : Else;
				default                 : throw new NotImplementedException("Not implemented {99A1E051-01FA-4DCC-BC55-F79100A3D381}");
			}
		}

		public static bool IsInDesignMode {
			get { return DesignerProperties.GetIsInDesignMode(s_DependencyObject); }
			set { DesignerProperties.SetIsInDesignMode(s_DependencyObject,value);}
		}

		private bool GetIsNull(object value, object parameter) {return Equals(value, null);}

		private bool GetIsNullOr0(object value, object parameter) {
			if(Equals(value, null)) return true;
			if(Equals(value, 0)) return true;
			if (value is ICollection) return ((ICollection) value).Count == 0;
			if (value.GetType().IsArray) return ((Array) value).GetLength(0) == 0;
			return false;
		}


		private bool GetBool(object value, object parameter) {

			if (this == EqualVisibleElseCollapsed || this == EqualCollapsedElseVisible) {
				if (value == null && parameter==null) return true;
				if (value == null || parameter==null) return false;
				return string.Format(CultureInfo.InvariantCulture, "{0}", value).Equals(string.Format(CultureInfo.InvariantCulture, "{0}", parameter));
			}

			if (value == null) return false;
			if (value is bool) return (bool) value;
#if(ReactiveUISupport)
			if (value is IObservable<bool>) {
				var obs = ((IObservable<bool>) value);
				var oph = new ObservableAsPropertyHelper<bool>(obs, b => {});
				return oph.Value;
			}
#endif
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException("Not implemented {1F3CC807-B0EE-4B3C-98C6-1483B7C176FF}"); }


		public static VisibilityConverter Get(Expression expression) {
			switch (expression) {
				case Expression.TrueVisibleElseCollapsed             	: return TrueVisibleElseCollapsed             	;
				case Expression.TrueVisibleElseCollapsed_DesignerTrue	: return TrueVisibleElseCollapsed_DesignerTrue	;
				case Expression.FalseVisibleElseCollapsed            	: return FalseVisibleElseCollapsed            	;
				case Expression.TrueVisibleElseHidden                	: return TrueVisibleElseHidden                	;
				case Expression.FalseVisibleElseHidden               	: return FalseVisibleElseHidden               	;
				case Expression.NullCollapsedElseVisible             	: return NullCollapsedElseVisible             	;
				case Expression.NullHiddenElseVisible                	: return NullHiddenElseVisible                	;
				case Expression.FalseCollapsedElseVisible            	: return FalseCollapsedElseVisible            	;
				case Expression.EqualVisibleElseCollapsed            	: return EqualVisibleElseCollapsed            	;
				case Expression.EqualCollapsedElseVisible            	: return EqualCollapsedElseVisible            	;
				case Expression.NullOr0VisibleElseHidden             	: return NullOr0VisibleElseHidden             	;
				case Expression.NullVisibleElseCollapsed             	: return NullVisibleElseCollapsed             	;
				case Expression.NullVisibleElseHidden                 	: return NullVisibleElseHidden                 	;
				default: throw new NotImplementedException("{1D7CC6E3-2466-47F2-B2C2-BEBA23C636CA}");
			}
		}

		public enum Expression {
			TrueVisibleElseCollapsed             	,
			TrueVisibleElseCollapsed_DesignerTrue	,
			FalseVisibleElseCollapsed            	,
			TrueVisibleElseHidden                	,
			FalseVisibleElseHidden               	,
			NullCollapsedElseVisible             	,
			NullHiddenElseVisible                	,
			FalseCollapsedElseVisible            	,
			EqualVisibleElseCollapsed            	,
			EqualCollapsedElseVisible            	,
			NullOr0VisibleElseHidden             	,
			NullVisibleElseCollapsed             	,
			NullVisibleElseHidden                 	,
			

		}
	}
}
