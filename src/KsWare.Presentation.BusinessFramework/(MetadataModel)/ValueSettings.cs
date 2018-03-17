using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


[module: SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Scope="member", Target="KsWare.Presentation.BusinessFramework.IValueSettings`1.#Default", MessageId="Default")]
[module: SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Scope="member", Target="KsWare.Presentation.BusinessFramework.IValueSettings`1.#Step", MessageId="Step")]
[module: SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Scope="member", Target="KsWare.Presentation.BusinessFramework.IValueSettings.#Default", MessageId="Default")]
[module: SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Scope="member", Target="KsWare.Presentation.BusinessFramework.IValueSettings.#Step", MessageId="Step")]

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> ValueSettings interface
	/// </summary>
	public interface IValueSettings {

		/// <summary> Gets or sets the default value.
		/// </summary>
		/// <value>The default value.</value>
		object Default{get;set;}

		/// <summary> Gets or sets the minimum value.
		/// </summary>
		/// <value>The minimum.</value>
		object Minimum{get;set;}

		/// <summary> Gets or sets a value indicating whether <see cref="Minimum"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="Minimum"/> is specified; otherwise, <see langword="false"/>.</value>
		bool MinimumSpecified{get;set;}

		/// <summary> Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		object Maximum{get;set;}
		
		/// <summary> Gets or sets a value indicating whether <see cref="Maximum"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="Maximum"/> is specified; otherwise, <see langword="false"/>.</value>
		bool MaximumSpecified{get;set;}

		/// <summary> Gets or sets the list of valid values.
		/// </summary>
		/// <value>The list of valid values.</value>
		IEnumerable IncludeValues{get;set;}
		
		/// <summary> Gets or sets a value indicating whether <see cref="IncludeValues"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="IncludeValues"/> is specified; otherwise, <see langword="false"/>.</value>
		bool IncludeValuesSpecified{get;set;}
		
		/// <summary> Gets or sets the list of invalid values.
		/// </summary>
		/// <value>The list of invalid values.</value>
		IEnumerable ExcludeValues{get;set;}
		
		/// <summary> Gets or sets a value indicating whether <see cref="ExcludeValues"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="ExcludeValues"/> is specified; otherwise, <see langword="false"/>.</value>
		bool ExcludeValuesSpecified{get;set;}

		/// <summary> Gets or sets the step size.
		/// </summary>
		/// <value>The step size.</value>
		object DecimalAccuracy{get;set;}
		
		/// <summary> Gets or sets a value indicating whether <see cref="DecimalAccuracy"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="DecimalAccuracy"/> is specified; otherwise, <see langword="false"/>.</value>
		bool DecimalAccuracySpecified{get;set;}

		/// <summary> Gets or sets a value indicating whether <see langword="null"/> is allowed.
		/// </summary>
		/// <value><see langword="true"/> if <see langword="null"/> is allowed; otherwise, <see langword="false"/>.</value>
		bool AllowNull{get;set;}
		
		/// <summary> Gets or sets a value indicating whether an empty string is allowed.
		/// </summary>
		/// <value><see langword="true"/> if empty string is allowed; otherwise, <see langword="false"/>.</value>
		bool AllowEmpty{get;set;}

		/// <summary> Gets or sets the null value.
		/// </summary>
		/// <value>The null value.</value>
		object NullValue{get;set;}

		/// <summary> Gets a value indicating whether the business value is currently available.
		/// </summary>
		/// <value><see langword="true"/> if the business value is currently available; otherwise, <see langword="false"/>. </value>
		bool IsAvailable{get;}
		
		/// <summary> <b>Internal use only</b>
		/// </summary>
		/// <param name="token"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool SetAvailable(object token, bool value);

		/// <summary> Gets a value indicating whether the business value is read only.
		/// </summary>
		/// <value><see langword="true"/> if the business value is read only; otherwise, <see langword="false"/>. </value>
		bool IsReadOnly{get;}
		
		/// <summary> <b>Internal use only</b>
		/// </summary>
		/// <param name="token"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool SetReadOnly(object token, bool value);

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="businessValue">The business value property.</param>
		/// <param name="value">The value to verify.</param>
		/// <returns><see langword="true"/> if the value is valid otherwise, <see langword="false"/>.</returns>
		Exception Validate(IValueBM businessValue, object value);

		/// <summary> Occurs when a settings property has been changed.
		/// </summary>
		event EventHandler<ValueSettingsChangedEventArgs> PropertyChanged;
	}

	/// <summary> Generic ValueSettings interface
	/// </summary>
	public interface IValueSettings<T> {

		/// <summary> Gets or sets the default value.
		/// </summary>
		/// <value>The default value.</value>
		T Default{get;set;}

		/// <summary> Gets or sets the minimum value.
		/// </summary>
		/// <value>The minimum.</value>
		T Minimum{get;set;}

		/// <summary> Gets or sets a value indicating whether <see cref="Minimum"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="Minimum"/> is specified; otherwise, <see langword="false"/>.</value>
		bool MinimumSpecified{get;set;}

		/// <summary> Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		T Maximum{get;set;}
		
		/// <summary> Gets a value indicating whether <see cref="Maximum"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="Maximum"/> is specified; otherwise, <see langword="false"/>.</value>
		bool MaximumSpecified{get;set;}

		/// <summary> Gets the list of valid values.
		/// </summary>
		/// <value>The list of valid values.</value>
		IEnumerable<T> IncludeValues{get;set;}
		
		/// <summary> Gets a value indicating whether <see cref="IncludeValues"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="IncludeValues"/> is specified; otherwise, <see langword="false"/>.</value>
		bool IncludeValuesSpecified{get;set;}
		
		/// <summary> Gets the list of invalid values.
		/// </summary>
		/// <value>The list of invalid values.</value>
		IEnumerable<T> ExcludeValues{get;set;}
		
		/// <summary> Gets or sets a value indicating whether <see cref="ExcludeValues"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="ExcludeValues"/> is specified; otherwise, <see langword="false"/>.</value>
		bool ExcludeValuesSpecified{get;set;}

		/// <summary> Gets the step size.
		/// </summary>
		/// <value>The step size.</value>
		T DecimalAccuracy{get;set;}
		
		/// <summary> Gets a value indicating whether <see cref="DecimalAccuracy"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="DecimalAccuracy"/> is specified; otherwise, <see langword="false"/>.</value>
		bool DecimalAccuracySpecified{get;set;}

		/// <summary> Gets a value indicating whether <see langword="null"/> is allowed.
		/// </summary>
		/// <value><see langword="true"/> if <see langword="null"/> is allowed; otherwise, <see langword="false"/>.</value>
		bool AllowNull{get;set;}
		
		/// <summary> Gets a value indicating whether an empty string is allowed.
		/// </summary>
		/// <value><see langword="true"/> if empty string is allowed; otherwise, <see langword="false"/>.</value>
		bool AllowEmpty{get;set;}

		/// <summary> Gets or sets the null value.
		/// </summary>
		/// <value>The null value.</value>
		T NullValue{get;set;}

		/// <summary> Gets a value indicating whether the business value is currently available.
		/// </summary>
		/// <value><see langword="true"/> if the business value is currently available; otherwise, <see langword="false"/>. </value>
		bool IsAvailable{get;}

		/// <summary> <b>Internal use only</b>
		/// </summary>
		/// <param name="token"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool SetAvailable(object token, bool value);

		/// <summary> Gets a value indicating whether the business value is read only.
		/// </summary>
		/// <value><see langword="true"/> if the business value is read only; otherwise, <see langword="false"/>. </value>
		bool IsReadOnly{get;}

		/// <summary> <b>Internal use only</b>
		/// </summary>
		/// <param name="token"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool SetReadOnly(object token, bool value);

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="businessValue">The business value property.</param>
		/// <param name="value">The value to verify.</param>
		/// <returns><see langword="true"/> if the value is valid otherwise, <see langword="false"/>.</returns>
		Exception Validate(IValueBM businessValue, T value);

		/// <summary> Occurs when a settings property has been changed.
		/// </summary>
		event EventHandler<ValueSettingsChangedEventArgs> PropertyChanged;
	}

/*
	public class ValueSettings: IValueSettings {

		private object defaultValue;
		private List<object> availableObjection = new List<object>();
		private List<object> readOnlyObjection = new List<object>();
		private ValueValidator validator;

		public bool IsAvailable{get;private set;}

		public bool SetAvailable(object token, bool value) {
			if (value) {
			    availableObjection.Add(token);
			} else {
			    availableObjection.Remove(token);
			}
			return availableObjection.Count == 0;
		}

		public bool IsReadOnly{get;private set;}

		public bool SetReadOnly(object token, bool value) {
			if (value) {
			    readOnlyObjection.Add(token);
			} else {
			    readOnlyObjection.Remove(token);
			}
			return readOnlyObjection.Count == 0;
		}

		public object Default{get;set;}
		public object Minimum{get;set;}
		public bool MinimumSpecified{get;set;}
		public object Maximum{get;set;}
		public bool MaximumSpecified{get;set;}
		public IEnumerable IncludeValues{get;set;}
		public bool IncludeValuesSpecified{get;set;}
		public IEnumerable ExcludeValues{get;set;}
		public bool ExcludeValuesSpecified{get;set;}
		public object Step{get;set;}
		public bool StepSpecified{get;set;}

		public bool AllowNull{get;set;}
		public bool AllowEmpty{get;set;}

		public object NullValue{get;set;}

		public bool Validate(IValueBM businessValue, object value) { 
			if(this.validator==null) {
				this.validator = new ValueValidator();
			}
			return this.validator.Validate(value, businessValue, false);
		}
	}
*/

#pragma warning disable 1591

	/// <summary>  Provides value settings
	/// </summary>
	/// <typeparam name="TValue">Type of value</typeparam>
	public class ValueSettings<TValue>: IValueSettings<TValue>, IValueSettings {

		#region static helpers

		private static TValue GetMinimumDefault(out bool specified) {
			Type t = typeof(TValue).NullableBaseType();
			switch (Type.GetTypeCode(t)) {
				case TypeCode.Byte    : specified=true; return (TValue)(object) Byte    .MinValue;
				case TypeCode.Char    : specified=true; return (TValue)(object) Char    .MinValue;
				case TypeCode.DateTime: specified=true; return (TValue)(object) DateTime.MinValue;
				case TypeCode.Decimal : specified=true; return (TValue)(object) Decimal .MinValue;
				case TypeCode.Double  : specified=true; return (TValue)(object) Double  .MinValue;
				case TypeCode.Int16   : specified=true; return (TValue)(object) Int16   .MinValue;
				case TypeCode.Int32   : specified=true; return (TValue)(object) Int32   .MinValue;
				case TypeCode.Int64   : specified=true; return (TValue)(object) Int64   .MinValue;
				case TypeCode.SByte   : specified=true; return (TValue)(object) SByte   .MinValue;
				case TypeCode.Single  : specified=true; return (TValue)(object) Single  .MinValue;
				case TypeCode.UInt16  : specified=true; return (TValue)(object) UInt16  .MinValue;
				case TypeCode.UInt32  : specified=true; return (TValue)(object) UInt32  .MinValue;
				case TypeCode.UInt64  : specified=true; return (TValue)(object) UInt64  .MinValue;
				case TypeCode.Boolean : specified=true; return default(TValue); // not specified
				case TypeCode.String  : specified=false; return default(TValue); // not specified
				case TypeCode.DBNull  : specified=false; return default(TValue); // not specified
				case TypeCode.Empty   : specified=false; return default(TValue); // not specified
				case TypeCode.Object  : specified=false; return default(TValue); // not specified
			}
			specified=false; return default(TValue);// not specified
		}

		private static TValue GetMaximumDefault(out bool specified) {
			Type t = typeof(TValue).NullableBaseType();
			switch (Type.GetTypeCode(t)) {
				case TypeCode.Byte    : specified=true; return (TValue)(object) Byte    .MaxValue;
				case TypeCode.Char    : specified=true; return (TValue)(object) Char    .MaxValue;
				case TypeCode.DateTime: specified=true; return (TValue)(object) DateTime.MaxValue;
				case TypeCode.Decimal : specified=true; return (TValue)(object) Decimal .MaxValue;
				case TypeCode.Double  : specified=true; return (TValue)(object) Double  .MaxValue;
				case TypeCode.Int16   : specified=true; return (TValue)(object) Int16   .MaxValue;
				case TypeCode.Int32   : specified=true; return (TValue)(object) Int32   .MaxValue;
				case TypeCode.Int64   : specified=true; return (TValue)(object) Int64   .MaxValue;
				case TypeCode.SByte   : specified=true; return (TValue)(object) SByte   .MaxValue;
				case TypeCode.Single  : specified=true; return (TValue)(object) Single  .MaxValue;
				case TypeCode.UInt16  : specified=true; return (TValue)(object) UInt16  .MaxValue;
				case TypeCode.UInt32  : specified=true; return (TValue)(object) UInt32  .MaxValue;
				case TypeCode.UInt64  : specified=true; return (TValue)(object) UInt64  .MaxValue;
				case TypeCode.Boolean : specified=false; return default(TValue); // not specified
				case TypeCode.String  : specified=false; return default(TValue); // not specified
				case TypeCode.DBNull  : specified=false; return default(TValue); // not specified
				case TypeCode.Empty   : specified=false; return default(TValue); // not specified
				case TypeCode.Object  : specified=false; return default(TValue); // not specified
			}
			specified=false; return default(TValue);// not specified
		}

		private static bool AllowNullDefault{get {
			Type t = typeof (TValue);
			if(t==typeof(string)) return true;
			if(t.IsNullable()) return true;
			return t.IsValueType == false;
		}}

		private static bool AllowEmptyDefault{get {
			Type t = typeof (TValue);
			if(t==typeof(string)) return true;
			if(t==typeof(object)) return true;
			return false;
		}}

		private static IEnumerable<TValue> GetDefaultIncludeValues(out bool specified) {
			if(typeof(TValue).IsEnum) {
				specified = true;
				return Enum.GetValues(typeof (TValue)).ToList<TValue>();
			} else {
				specified = false;
				return new List<TValue>();
			}

		}

		private static TValue GetDefaultValue() {
//			if(typeof(TValue).IsEnum) {
//				return (TValue)(object) 0;
//			} else {
//				return default(TValue);
//			}
			return default(TValue);
		}

		#endregion

		private readonly List<object> _isAvailableObjections = new List<object>();
		private readonly List<object> _isReadOnlyObjections = new List<object>();
		private ValueValidator _validator;
		private TValue _defaultValue;
		private TValue _minimum;
		private bool _minimumSpecified;
		private TValue _maximum;
		private bool _maximumSpecified;
		private IEnumerable<TValue> _includeValues;
		private bool _includeValuesSpecified;
		private IEnumerable<TValue> _excludeValues;
		private bool _excludeValuesSpecified;
		private TValue _decimalAccuracy;
		private bool _decimalAccuracySpecified;		
		
		/// <summary> Initializes a new instance of the <see cref="ValueSettings{T}"/> class.
		/// </summary>
		public ValueSettings() {
			Default       = GetDefaultValue();
			Minimum       = GetMinimumDefault(out _minimumSpecified);
			Maximum       = GetMaximumDefault(out _maximumSpecified);
			IncludeValues = GetDefaultIncludeValues(out _includeValuesSpecified);
			ExcludeValues = new List<TValue>();
			AllowNull     = AllowNullDefault;
			AllowEmpty    = AllowEmptyDefault;
		}

		public event EventHandler<ValueSettingsChangedEventArgs> PropertyChanged;
		

		#region Implementation of IValueSettings<T>



		/// <summary> Gets or sets a value indicating whether this the value is available.
		/// </summary>
		/// <value><see langword="true"/> if the value is available; otherwise, <see langword="false"/>.</value>
		public bool IsAvailable{get;private set;}

		/// <summary> Sets the availability of the value.
		/// </summary>
		/// <param name="token">a unique token.</param>
		/// <param name="value"><see langword="false"/> to add a available ojection; otherwise, <see langword="true"/> to remove a available ojection.</param>
		/// <value><see langword="true"/> if the value is available; otherwise, <see langword="false"/>.</value>
		public bool SetAvailable(object token, bool value) {
			var v = _isAvailableObjections.Count == 0;
			if (value) {
			    _isAvailableObjections.Add(token);
			} else {
			    _isAvailableObjections.Remove(token);
			}
			if(v!=(_isAvailableObjections.Count==0)) {
				IsAvailable = _isAvailableObjections.Count == 0;
				OnPropertyChanged(ValueSettingName.IsAvailable);
			}
			return _isAvailableObjections.Count == 0;
		}

		/// <summary> Gets or sets a value indicating whether the business value is read only.
		/// </summary>
		/// <value><see langword="true"/> if the business value is read only; otherwise, <see langword="false"/>.</value>
		public bool IsReadOnly{get;private set;}

		/// <summary> Sets the writeability of the business value.
		/// </summary>
		/// <param name="token">a unique token.</param>
		/// <param name="value"><see langword="false"/> to add a write objection; otherwise, <see langword="true"/> to remove a write objection.</param>
		/// <value><see langword="true"/> if the business value is available; otherwise, <see langword="false"/>.</value>
		public bool SetReadOnly(object token, bool value) {
			var v = _isReadOnlyObjections.Count == 0;
			if (value) {
			    _isReadOnlyObjections.Add(token);
			} else {
			    _isReadOnlyObjections.Remove(token);
			}
			if(v!=(_isReadOnlyObjections.Count==0)) {
				IsReadOnly = _isReadOnlyObjections.Count == 0;
				OnPropertyChanged(ValueSettingName.IsReadOnly);
			}
			return _isReadOnlyObjections.Count == 0;
		}

		/// <summary> Gets or sets the default value.
		/// </summary>
		/// <value>The default value.</value>
		public TValue Default {
			get => _defaultValue;
			set {
				if (Equals(_defaultValue, value)) return;
				_defaultValue = value;
				OnPropertyChanged(ValueSettingName.Default);
			}
		}

		/// <summary> Gets or sets the minimum value.
		/// </summary>
		/// <value>The minimum value.</value>
		public TValue Minimum {
			get => _minimum;
			set {
				if (Equals(_minimum, value)) return;
				_minimum = value;
				OnPropertyChanged(ValueSettingName.Minimum);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="Minimum"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="Minimum"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool MinimumSpecified {
			get => _minimumSpecified;
			set {
				if (Equals(_minimumSpecified, value)) return;
				_minimumSpecified = value;
				OnPropertyChanged(ValueSettingName.Minimum);
			}
		}

		/// <summary> Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		public TValue Maximum {
			get => _maximum;
			set {
				if (Equals(_maximum, value)) return;
				_maximum = value;
				OnPropertyChanged(ValueSettingName.Maximum);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="Minimum"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="Minimum"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool MaximumSpecified {
			get => _maximumSpecified;
			set {
				if (Equals(_maximumSpecified, value)) return;
				_maximumSpecified = value;
				OnPropertyChanged(ValueSettingName.Maximum);
			}
		}

		/// <summary> Gets the list of valid values.
		/// </summary>
		/// <value>The list of valid values.</value>
		public IEnumerable<TValue> IncludeValues {
			get => _includeValues;
			set {
				if (Equals(_includeValues, value)) return;
				_includeValues = value;
				OnPropertyChanged(ValueSettingName.IncludeValues);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="IncludeValues"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="IncludeValues"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool IncludeValuesSpecified {
			get => _includeValuesSpecified;
			set {
				if (Equals(_includeValuesSpecified, value)) return;
				_includeValuesSpecified = value;
				OnPropertyChanged(ValueSettingName.IncludeValues);
			}
		}

		/// <summary> Gets the list of invalid values.
		/// </summary>
		/// <value>The list of invalid values.</value>
		public IEnumerable<TValue> ExcludeValues {
			get => _excludeValues;
			set {
				if (Equals(_excludeValues, value)) return;
				_excludeValues = value;
				OnPropertyChanged(ValueSettingName.ExcludeValues);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="ExcludeValues"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="ExcludeValues"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool ExcludeValuesSpecified {
			get => _excludeValuesSpecified;
			set {
				if (Equals(_excludeValuesSpecified, value)) return;
				_excludeValuesSpecified = value;
				OnPropertyChanged(ValueSettingName.ExcludeValues);
			}
		}


		/// <summary> Gets or sets the decimal accuracy.
		/// </summary>
		/// <value>The decimal accuracy.</value>
		public TValue DecimalAccuracy {
			get => _decimalAccuracy;
			set {
				if (Equals(_decimalAccuracy, value)) return;
				_decimalAccuracy = value;
				OnPropertyChanged(ValueSettingName.DecimalAccuracy);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="DecimalAccuracy"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="DecimalAccuracy"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool DecimalAccuracySpecified {
			get => _decimalAccuracySpecified;
			set {
				if (Equals(_decimalAccuracySpecified, value)) return;
				_decimalAccuracySpecified = value;
				OnPropertyChanged(ValueSettingName.DecimalAccuracy);
			}
		}

		public bool AllowNull{get;set;}

		public bool AllowEmpty{get;set;}

		public TValue NullValue{get;set;}

		public Exception Validate(IValueBM businessValue, TValue value) { 
			if(_validator==null) {
				_validator = new ValueValidator();
			}
			return _validator.Validate(value, businessValue, false);
		}

		#endregion

		#region explicit implementation of IValueSettings (for invariant compatibility)

		object IValueSettings.Default{get => Default; set => Default = (TValue) value; }

		object IValueSettings.Minimum{get => Minimum; set => Minimum = (TValue) value; }

		object IValueSettings.Maximum{get => Maximum; set => Maximum = (TValue) value; }

		object IValueSettings.DecimalAccuracy{get => DecimalAccuracy; set => DecimalAccuracy = (TValue) value; }

		object IValueSettings.NullValue{get => NullValue; set => NullValue = (TValue) value; }

		IEnumerable IValueSettings.IncludeValues{get => IncludeValues; set => IncludeValues = (IEnumerable<TValue>) value; }
																
		IEnumerable IValueSettings.ExcludeValues { get => ExcludeValues; set => ExcludeValues = (IEnumerable<TValue>)value; }

		public Exception Validate(IValueBM businessValue, object value) {return Validate(businessValue, (TValue)value);}

		#endregion

		private void OnPropertyChanged(ValueSettingName property) {
			if(PropertyChanged!=null)PropertyChanged(this,new ValueSettingsChangedEventArgs(property));
		}
	}

	#pragma warning restore 1591

	/// <summary>
	/// Value Setting
	/// </summary>
	public enum ValueSettingName {

		/// <summary> No property </summary>
		None,

		/// <summary> All or some properties </summary>
		All,

		/// <summary> The <see cref="IValueSettings.Default"/> property </summary>
		Default,

		/// <summary> The <see cref="IValueSettings.IsAvailable"/> property </summary>
		IsAvailable,

		/// <summary> The <see cref="IValueSettings.IsReadOnly"/> property </summary>
		IsReadOnly,

		/// <summary> The <see cref="IValueSettings.Minimum"/> property </summary>
		Minimum,

		/// <summary> The <see cref="IValueSettings.Maximum"/> property </summary>
		Maximum,

		/// <summary> The <see cref="IValueSettings.DecimalAccuracy"/> property </summary>
		DecimalAccuracy,

		/// <summary> The <see cref="IValueSettings.IncludeValues"/> property </summary>
		IncludeValues,

		/// <summary> The <see cref="IValueSettings.ExcludeValues"/> property </summary>
		ExcludeValues,

		/// <summary> The <see cref="IValueSettings.NullValue"/> property </summary>
		NullValue,
	}

	[AttributeUsage(AttributeTargets.Property,Inherited = true,AllowMultiple = false)]
	public class ValueSettingsAttribute : Attribute {

		public ValueSettingsAttribute() {}

		public object Minimum{get;set;}
		public object Maximum{get;set;}
		public bool? IsReadOnly { get; set; }
	}

}