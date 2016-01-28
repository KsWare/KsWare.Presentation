using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public class VisibilityConverter:IValueConverter {

		private static readonly DependencyObject s_DependencyObject=new DependencyObject();

		public static VisibilityConverter TrueVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter TrueVisibleElseHidden { get; private set; }
		public static VisibilityConverter TrueCollapsedElseVisible { get; private set; }
		public static VisibilityConverter TrueHiddenElseVisible { get; private set; }

		public static VisibilityConverter FalseVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter FalseVisibleElseHidden { get; private set; }
		public static VisibilityConverter FalseCollapsedElseVisible { get; private set; }
		public static VisibilityConverter FalseHiddenElseVisible { get; private set; }

		public static VisibilityConverter NullCollapsedElseVisible { get; private set; }
		public static VisibilityConverter NullHiddenElseVisible { get; private set; }
		public static VisibilityConverter NullVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter NullVisibleElseHidden { get; private set; }


		/// <summary> Returns <see cref="Visibility.Visible"/> the value is Null or 0; else <see cref="Visibility.Hidden"/>
		/// </summary>
		/// <value>The value</value>
		/// <example>Usage: <code>&lt;Grid Visibility="{Binding MyListProperty.Count, Converter={x:Static vfw:VisibilityConverter.NullOr0VisibleElseHidden}}"&gt;</code></example>
		public static VisibilityConverter NullOr0VisibleElseHidden { get; private set; }
		public static VisibilityConverter NullOr0VisibleElseCollabsed { get; private set; }
		public static VisibilityConverter NullOr0CollapsedElseVisible { get; private set; }
		public static VisibilityConverter NullOr0HiddenElseVisible { get; private set; }

		public static VisibilityConverter EqualVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter EqualVisibleElseHidden { get; private set; }
		public static VisibilityConverter EqualCollapsedElseVisible { get; private set; }
		public static VisibilityConverter EqualHiddenElseVisible { get; private set; }

		public static VisibilityConverter NotEqualVisibleElseCollapsed { get; private set; }
		public static VisibilityConverter NotEqualVisibleElseHidden { get; private set; }
		public static VisibilityConverter NotEqualCollapsedElseVisible { get; private set; }
		public static VisibilityConverter NotEqualHiddenElseVisible { get; private set; }

		private const Visibility Visible   = Visibility.Visible;
		private const Visibility Collapsed = Visibility.Collapsed;
		private const Visibility Hidden    = Visibility.Hidden;
		private const Value IsTrue     = Value.IsTrue;
		private const Value IsFalse    = Value.IsFalse;
		private const Value IsNull     = Value.IsNull;
		private const Value IsEqual    = Value.IsEqual;
		private const Value IsNotEqual = Value.IsNotEqual;
		private const Value IsNullOr0  = Value.IsNullOr0;

		private static Dictionary<Expression, VisibilityConverter> _converters;

		static VisibilityConverter() {

			_converters=new Dictionary<Expression, VisibilityConverter>();

			_converters.Add(Expression.TrueVisibleElseCollapsed       , TrueVisibleElseCollapsed    = new VisibilityConverter(IsTrue     ,Visible  ,Collapsed));
			_converters.Add(Expression.TrueVisibleElseHidden          , TrueVisibleElseHidden       = new VisibilityConverter(IsTrue     ,Visible  ,Hidden   ));
			_converters.Add(Expression.TrueHiddenElseVisible          , TrueHiddenElseVisible       = new VisibilityConverter(IsTrue     ,Hidden   ,Visible  ));
			_converters.Add(Expression.TrueCollapsedElseVisible       , TrueCollapsedElseVisible    = new VisibilityConverter(IsTrue     ,Collapsed,Visible  ));

			_converters.Add(Expression.FalseVisibleElseCollapsed      , FalseVisibleElseCollapsed    = new VisibilityConverter(IsFalse   ,Visible  ,Collapsed));
			_converters.Add(Expression.FalseVisibleElseHidden         , FalseVisibleElseHidden       = new VisibilityConverter(IsFalse   ,Visible  ,Hidden   ));
			_converters.Add(Expression.FalseCollapsedElseVisible      , FalseCollapsedElseVisible    = new VisibilityConverter(IsFalse   ,Collapsed,Visible  ));
			_converters.Add(Expression.FalseHiddenElseVisible         , FalseHiddenElseVisible       = new VisibilityConverter(IsFalse   ,Hidden   ,Visible  ));

			_converters.Add(Expression.NullVisibleElseHidden          , NullVisibleElseHidden        = new VisibilityConverter(IsNull    ,Visible  ,Hidden   ));
			_converters.Add(Expression.NullVisibleElseCollapsed       , NullVisibleElseCollapsed     = new VisibilityConverter(IsNull    ,Visible  ,Collapsed));
			_converters.Add(Expression.NullCollapsedElseVisible       , NullCollapsedElseVisible     = new VisibilityConverter(IsNull    ,Collapsed,Visible  ));
			_converters.Add(Expression.NullHiddenElseVisible          , NullHiddenElseVisible        = new VisibilityConverter(IsNull    ,Hidden   ,Visible  ));

			_converters.Add(Expression.NullOr0VisibleElseHidden       , NullOr0VisibleElseHidden     = new VisibilityConverter(IsNullOr0 ,Visible  ,Hidden   ));
			_converters.Add(Expression.NullOr0VisibleElseCollapsed    , NullOr0VisibleElseCollabsed  = new VisibilityConverter(IsNullOr0 ,Visible  ,Collapsed));
			_converters.Add(Expression.NullOr0CollapsedElseVisible    , NullOr0CollapsedElseVisible  = new VisibilityConverter(IsNullOr0 ,Collapsed,Visible  ));
			_converters.Add(Expression.NullOr0HiddenElseVisible       , NullOr0HiddenElseVisible     = new VisibilityConverter(IsNullOr0 ,Hidden   ,Visible  ));
						    
			_converters.Add(Expression.EqualVisibleElseCollapsed      , EqualVisibleElseCollapsed    = new VisibilityConverter(IsEqual   ,Visible  ,Collapsed));
			_converters.Add(Expression.EqualVisibleElseHidden         , EqualVisibleElseHidden       = new VisibilityConverter(IsEqual   ,Visible  ,Hidden   ));
			_converters.Add(Expression.EqualCollapsedElseVisible      , EqualCollapsedElseVisible    = new VisibilityConverter(IsEqual   ,Collapsed,Visible  ));
			_converters.Add(Expression.EqualHiddenElseVisible         , EqualHiddenElseVisible       = new VisibilityConverter(IsEqual   ,Hidden   ,Visible  ));

			_converters.Add(Expression.NotEqualVisibleElseCollapsed   , NotEqualVisibleElseCollapsed = new VisibilityConverter(IsNotEqual,Visible  ,Collapsed));
			_converters.Add(Expression.NotEqualVisibleElseHidden      , NotEqualVisibleElseHidden    = new VisibilityConverter(IsNotEqual,Visible  ,Hidden   ));
			_converters.Add(Expression.NotEqualCollapsedElseVisible   , NotEqualCollapsedElseVisible = new VisibilityConverter(IsNotEqual,Collapsed,Visible  ));
			_converters.Add(Expression.NotEqualHiddenElseVisible      , NotEqualHiddenElseVisible    = new VisibilityConverter(IsNotEqual,Hidden   ,Visible  ));

		}


		public VisibilityConverter() {
			Designer  = DependencyProperty.UnsetValue;
		}

		private VisibilityConverter(Value condition) {
			Designer  = DependencyProperty.UnsetValue;
		}

		private VisibilityConverter(Value condition, Visibility conditionIsTrue, Visibility conditionIsFalse) {
			Designer  = DependencyProperty.UnsetValue;
			Condition = condition;
			True      = conditionIsTrue;
			Else      = conditionIsFalse;
		}

		private enum Value {
			IsNull,
			IsNullOr0,
			IsTrue,
			IsFalse,
			IsEqual,
			IsNotEqual,
		}

		private Visibility True { get; set; }

		private Visibility Else { get; set; }

		private Value Condition { get; set; }

		private object Designer { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			var p=parameter as VisibilityConverterParameter;
			var v=parameter as Visibility?;
			
			if(v.HasValue) p=new VisibilityConverterParameter {DesigntimeVisibility = v};
			if(p==null) p=new VisibilityConverterParameter {CompareValue=parameter};

			if (IsInDesignMode && p.DesigntimeVisibility.HasValue) return p.DesigntimeVisibility.Value;
			if (IsInDesignMode && Designer != DependencyProperty.UnsetValue) value = Designer;

			switch (Condition) {
				case IsTrue :case IsEqual   : return GetBool       (value,p.CompareValue) ? True : Else; 
				case IsFalse:case IsNotEqual: return ! GetBool     (value,p.CompareValue) ? True : Else;
				case IsNull                 : return GetIsNull     (value,p.CompareValue) ? True : Else;
//				case IsNotNull              : return ! GetIsNull   (value,p.CompareValue) ? True : Else;
				case IsNullOr0              : return GetIsNullOr0  (value,p.CompareValue) ? True : Else;
				default                     : throw new NotImplementedException("Condition not implemented! ErrorID:{99A1E051-01FA-4DCC-BC55-F79100A3D381}");
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

			switch (this.Condition) {
				case Value.IsEqual: case Value.IsNotEqual: {
					if (value == null && parameter==null) return true;
					if (value == null || parameter==null) return false;
					return ConvertEqual(value, parameter);
				}
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

		private static readonly DoubleConverter DoubleConverter=new DoubleConverter();

		private static bool ConvertEqual(object value, object parameter) {
			switch (value.GetType().Name) {
				case "Double": {
					var dValue = (Double) value;
					Double dParameter;
					if (parameter is Double) dParameter = (Double) parameter;
					else {
						var sParameter = string.Format(CultureInfo.InvariantCulture, "{0}", parameter);
						if(!Double.TryParse(sParameter,NumberStyles.Float,CultureInfo.InvariantCulture, out dParameter)) return false;
					}
					if (double.IsNaN(dValue) || double.IsNaN(dParameter) || double.IsInfinity(dValue) || double.IsInfinity(dParameter))
						return dValue.Equals(dParameter);
					return ((Single) dValue).Equals((Single) dParameter);
				}
				default: {
					var a = string.Format(CultureInfo.InvariantCulture, "{0}", value);
					var b = string.Format(CultureInfo.InvariantCulture, "{0}", parameter);
					return a.Equals(b);					
				}
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException("Not implemented {1F3CC807-B0EE-4B3C-98C6-1483B7C176FF}"); }

		public static VisibilityConverter Get(Expression expression) {
			VisibilityConverter converter;
			if (!_converters.TryGetValue(expression, out converter)) {
				throw new NotImplementedException("Converter not found! ErrorID:{1D7CC6E3-2466-47F2-B2C2-BEBA23C636CA}");
			}
			return converter;
		}

		public enum Expression {
			None,

			TrueVisibleElseCollapsed      , 
			TrueCollapsedElseVisible      , 
			TrueVisibleElseHidden         , 
			TrueHiddenElseVisible         , 

			FalseVisibleElseCollapsed      ,
			FalseCollapsedElseVisible      ,
			FalseVisibleElseHidden         ,
			FalseHiddenElseVisible         ,

			NullVisibleElseCollapsed       ,
			NullVisibleElseHidden          ,
			NullCollapsedElseVisible       ,
			NullHiddenElseVisible          ,

			NullOr0VisibleElseHidden       ,
			NullOr0VisibleElseCollapsed    ,
			NullOr0CollapsedElseVisible    ,
			NullOr0HiddenElseVisible       ,

			EqualVisibleElseCollapsed      ,
			EqualCollapsedElseVisible      ,
			EqualVisibleElseHidden         ,
			EqualHiddenElseVisible         ,

			NotEqualVisibleElseCollapsed   ,
			NotEqualVisibleElseHidden      ,
			NotEqualCollapsedElseVisible   ,
			NotEqualHiddenElseVisible      ,
		}

	}

	public class VisibilityConverterParameter {

		public object CompareValue { get; set; }

		public Visibility? DesigntimeVisibility { get; set; }
	}

//	public class VisibilityConverterMV : IMultiValueConverter {
//
//		public static readonly VisibilityConverterMV Default=new VisibilityConverterMV();
//
//		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
//			return Visibility.Visible;
//		}
//
//		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
//	}
}
