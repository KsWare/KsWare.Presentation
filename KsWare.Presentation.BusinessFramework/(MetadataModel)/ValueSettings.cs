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

		private readonly List<object> m_IsAvailableObjections = new List<object>();
		private readonly List<object> m_IsReadOnlyObjections = new List<object>();
		private ValueValidator m_Validator;
		private TValue m_DefaultValue;
		private TValue m_Minimum;
		private bool m_MinimumSpecified;
		private TValue m_Maximum;
		private bool m_MaximumSpecified;
		private IEnumerable<TValue> m_IncludeValues;
		private bool m_IncludeValuesSpecified;
		private IEnumerable<TValue> m_ExcludeValues;
		private bool m_ExcludeValuesSpecified;
		private TValue m_DecimalAccuracy;
		private bool m_DecimalAccuracySpecified;		
		
		/// <summary> Initializes a new instance of the <see cref="ValueSettings{T}"/> class.
		/// </summary>
		public ValueSettings() {
			Default       = GetDefaultValue();
			Minimum       = GetMinimumDefault(out m_MinimumSpecified);
			Maximum       = GetMaximumDefault(out m_MaximumSpecified);
			IncludeValues = GetDefaultIncludeValues(out m_IncludeValuesSpecified);
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
			var v = m_IsAvailableObjections.Count == 0;
			if (value) {
			    m_IsAvailableObjections.Add(token);
			} else {
			    m_IsAvailableObjections.Remove(token);
			}
			if(v!=(m_IsAvailableObjections.Count==0)) {
				IsAvailable = m_IsAvailableObjections.Count == 0;
				OnPropertyChanged(ValueSettingName.IsAvailable);
			}
			return m_IsAvailableObjections.Count == 0;
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
			var v = m_IsReadOnlyObjections.Count == 0;
			if (value) {
			    m_IsReadOnlyObjections.Add(token);
			} else {
			    m_IsReadOnlyObjections.Remove(token);
			}
			if(v!=(m_IsReadOnlyObjections.Count==0)) {
				IsReadOnly = m_IsReadOnlyObjections.Count == 0;
				OnPropertyChanged(ValueSettingName.IsReadOnly);
			}
			return m_IsReadOnlyObjections.Count == 0;
		}

		/// <summary> Gets or sets the default value.
		/// </summary>
		/// <value>The default value.</value>
		public TValue Default {
			get {return m_DefaultValue;}
			set {
				if (Equals(m_DefaultValue, value)) return;
				m_DefaultValue = value;
				OnPropertyChanged(ValueSettingName.Default);
			}
		}

		/// <summary> Gets or sets the minimum value.
		/// </summary>
		/// <value>The minimum value.</value>
		public TValue Minimum {
			get {return m_Minimum;}
			set {
				if (Equals(m_Minimum, value)) return;
				m_Minimum = value;
				OnPropertyChanged(ValueSettingName.Minimum);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="Minimum"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="Minimum"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool MinimumSpecified {
			get {return m_MinimumSpecified;}
			set {
				if (Equals(m_MinimumSpecified, value)) return;
				m_MinimumSpecified = value;
				OnPropertyChanged(ValueSettingName.Minimum);
			}
		}

		/// <summary> Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		public TValue Maximum {
			get {return m_Maximum;}
			set {
				if (Equals(m_Maximum, value)) return;
				m_Maximum = value;
				OnPropertyChanged(ValueSettingName.Maximum);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="Minimum"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="Minimum"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool MaximumSpecified {
			get {return m_MaximumSpecified;}
			set {
				if (Equals(m_MaximumSpecified, value)) return;
				m_MaximumSpecified = value;
				OnPropertyChanged(ValueSettingName.Maximum);
			}
		}

		/// <summary> Gets the list of valid values.
		/// </summary>
		/// <value>The list of valid values.</value>
		public IEnumerable<TValue> IncludeValues {
			get {return m_IncludeValues;}
			set {
				if (Equals(m_IncludeValues, value)) return;
				m_IncludeValues = value;
				OnPropertyChanged(ValueSettingName.IncludeValues);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="IncludeValues"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="IncludeValues"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool IncludeValuesSpecified {
			get {return m_IncludeValuesSpecified;}
			set {
				if (Equals(m_IncludeValuesSpecified, value)) return;
				m_IncludeValuesSpecified = value;
				OnPropertyChanged(ValueSettingName.IncludeValues);
			}
		}

		/// <summary> Gets the list of invalid values.
		/// </summary>
		/// <value>The list of invalid values.</value>
		public IEnumerable<TValue> ExcludeValues {
			get {return m_ExcludeValues;}
			set {
				if (Equals(m_ExcludeValues, value)) return;
				m_ExcludeValues = value;
				OnPropertyChanged(ValueSettingName.ExcludeValues);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="ExcludeValues"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="ExcludeValues"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool ExcludeValuesSpecified {
			get {return m_ExcludeValuesSpecified;}
			set {
				if (Equals(m_ExcludeValuesSpecified, value)) return;
				m_ExcludeValuesSpecified = value;
				OnPropertyChanged(ValueSettingName.ExcludeValues);
			}
		}


		/// <summary> Gets or sets the decimal accuracy.
		/// </summary>
		/// <value>The decimal accuracy.</value>
		public TValue DecimalAccuracy {
			get {return m_DecimalAccuracy;}
			set {
				if (Equals(m_DecimalAccuracy, value)) return;
				m_DecimalAccuracy = value;
				OnPropertyChanged(ValueSettingName.DecimalAccuracy);
			}
		}

		/// <summary> Gets or sets a value indicating whether <see cref="DecimalAccuracy"/> is specified.
		/// </summary>
		/// <value><see langword="true"/> if <see cref="DecimalAccuracy"/> is specified; otherwise, <see langword="false"/>.</value>
		public bool DecimalAccuracySpecified {
			get {return m_DecimalAccuracySpecified;}
			set {
				if (Equals(m_DecimalAccuracySpecified, value)) return;
				m_DecimalAccuracySpecified = value;
				OnPropertyChanged(ValueSettingName.DecimalAccuracy);
			}
		}

		public bool AllowNull{get;set;}

		public bool AllowEmpty{get;set;}

		public TValue NullValue{get;set;}

		public Exception Validate(IValueBM businessValue, TValue value) { 
			if(m_Validator==null) {
				m_Validator = new ValueValidator();
			}
			return m_Validator.Validate(value, businessValue, false);
		}

		#endregion

		#region explicit implementation of IValueSettings (for invariant compatibility)

		object IValueSettings.Default{get {return Default;}set {Default = (TValue) value;}}

		object IValueSettings.Minimum{get {return Minimum;}set {Minimum = (TValue) value;}}

		object IValueSettings.Maximum{get {return Maximum;}set {Maximum = (TValue) value;}}

		object IValueSettings.DecimalAccuracy{get {return DecimalAccuracy;}set {DecimalAccuracy = (TValue) value;}}

		object IValueSettings.NullValue{get {return NullValue;}set {NullValue = (TValue) value;}}

		IEnumerable IValueSettings.IncludeValues{get {return IncludeValues;}set {IncludeValues = (IEnumerable<TValue>) value;}}
																
		IEnumerable IValueSettings.ExcludeValues { get { return ExcludeValues; } set { ExcludeValues = (IEnumerable<TValue>)value; } }

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