/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : Validator.cs
 * OriginalNamespace: KsWare.Presentation.BusinessFramework
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> [DRAFT] Validator interface
	/// </summary>
	public interface IValidator {

		/// <summary> [DRAFT] Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns><see langword="null"/>if the value is valid; else <see cref="Exception"/> </returns>
		Exception Validate(object value, bool throwOnInvalid);

		/// <summary> [DRAFT] Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="rules">The validation rules.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns><see langword="null"/>if the value is valid; else <see cref="Exception"/> </returns>
		Exception Validate(object value, object rules, bool throwOnInvalid);
	}

	/// <summary> Interface for business value validator
	/// </summary>
	public interface IBusinessValueValidator {

		/// <summary> [DRAFT] Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="businessValue">The <see cref="IValueBM"/>.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns><see langword="null"/>if the value is valid; else <see cref="Exception"/> </returns>
		Exception Validate(object value, IValueBM businessValue, bool throwOnInvalid);
	}


	/// <summary> [DRAFT] Validator
	/// </summary>
	public class Validator: IValidator {

		#region Implementation of IValidator

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		///  <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns><see langword="null"/>if the value is valid; else <see cref="Exception"/> </returns>
		public virtual Exception Validate(object value, bool throwOnInvalid) { return null; }

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="rules">The validation rules.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns><see langword="null"/>if the value is valid; else <see cref="Exception"/> </returns>
		public virtual Exception Validate(object value, object rules, bool throwOnInvalid) { return null; }

		#endregion
	}

	/// <summary>
	/// [DRAFT] ValueValidator
	/// </summary>
	public class ValueValidator: Validator, IBusinessValueValidator {

		#region override Validator [DRAFT]

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns><see langword="null"/>if the value is valid; else <see cref="Exception"/> </returns>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public override Exception Validate(object value, bool throwOnInvalid) {
			var result = base.Validate(value, throwOnInvalid);
			if (result!=null) return result;

			if (value is IValueBM) {
			    result = ValidateValueInternal(((IValueBM) value).Value, ((IValueBM) value).Settings, (IValueBM) value, throwOnInvalid);
			    if (result!=null) return result;
			} else {
			    throw new ArgumentException("Invalid type!", "value");
			}

			var ex=new ValueValidationException(value,new NotImplementedException("{9E172533-449F-46A9-BCE7-83B0ABDA409B}"));
			if (throwOnInvalid) throw ex;
			return ex;
		}

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="rules">The validation rules.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns><see langword="null"/>if the value is valid; else <see cref="Exception"/> </returns>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public override Exception Validate(object value, object rules, bool throwOnInvalid) {
			var result = base.Validate(value, rules, throwOnInvalid);
			if (result!=null) return result;

			if (value is IValueBM) {
			    result = ValidateValueInternal(((IValueBM) value).Value, (IValueSettings) rules, (IObjectBM) value, throwOnInvalid);
			    if (result!=null) return result;
			} else {
			    result = ValidateValueInternal(value, (IValueSettings) rules, null, throwOnInvalid);
			    if (result!=null) return result;
			}

			var ex=new ValueValidationException(value,new NotImplementedException("{81B8738C-B3E9-4A33-AD9D-955FB67DE310}"));
			if (throwOnInvalid) throw ex;
			return ex;
		}

		#endregion

		#region IBusinessValueValidator

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="businessValue">The <see cref="IValueBM"/>.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns><see langword="null"/>if the value is valid; else an <see cref="Exception"/>> </returns>
		public Exception Validate(object value, IValueBM businessValue, bool throwOnInvalid) {
			if (businessValue == null) throw new ArgumentNullException("businessValue");
			//string typeName = value.ValueType!=null ? value.ValueType.Name : null;

			return ValidateValueInternal(value, businessValue.Settings, businessValue, throwOnInvalid);
		}

		#endregion

		/// <summary> Validates a value
		/// </summary>
		/// <param name="value">The value to validate</param>
		/// <param name="rules">The rules for validation</param>
		/// <param name="businessObject">The business value which holds the value</param>
		/// <param name="throwOnInvalid">Throw on invalidValue</param>
		/// <returns><see langword="null"/>if the value is valid; else an <see cref="Exception"/>> </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]//REVISE: too complex
		protected virtual Exception ValidateValueInternal(object value, IValueSettings rules, IObjectBM businessObject, bool throwOnInvalid) {
			//		    var result=base.Validate(value, rules, throwOnError);
			//		    if(!throwOnError && !result) return false;

			#region Null Validator

			if (Equals(value, null)) {
			    if (!rules.AllowNull) {
					var ex=new ValueValidationException(value, businessObject,new ArgumentNullException("value","Value must be not null!"));
			        if (!throwOnInvalid) return ex;
			    	throw ex;
			    }
			    return null; // if null allowed all other test are skipped
			}

			#endregion

			// at this point value is never null

			#region Empty Validator

			if (value is string && Equals(value, string.Empty)) {
			    if (!rules.AllowEmpty) {
			    	var ex = new ValueValidationException(value, businessObject,new ArgumentOutOfRangeException("value",value,"Value must not be an empty string!"));
			        if (!throwOnInvalid) return ex;
			        throw ex;
			    }
			    return null; // if empty string allowed all other test are skipped
			}

			#endregion

			// at this point value is never null nor empty string

			if (value is IComparable) {
			    // this is true for all simple types (Int32, Boolean, etc.) and for String, DateTime, TimeSpan, Guid  

			    if (rules.MinimumSpecified) {
			        if (Comparer.Default.Compare(rules.Minimum, value) > 0) {
						var ex=new ValueValidationException(value, businessObject,new ArgumentOutOfRangeException("value",value,"Value must not be less then Minimum!"));
			            if (!throwOnInvalid) return ex;
			        	throw ex;
			        }
			    }
			    if (rules.MaximumSpecified) {
			        if (Comparer.Default.Compare(value, rules.Maximum) > 0) {
						var ex=new ValueValidationException(value, businessObject,new ArgumentOutOfRangeException("value",value,"Value must not be greater then Maximum!"));
			            if (!throwOnInvalid) return ex;
			        	throw ex;
			        }
			    }
			    if (rules.IncludeValuesSpecified) {
			        if (!rules.IncludeValues.Cast<object>().Any(a => Comparer.Default.Compare(a, value) == 0)) {
						var ex=new ValueValidationException(value, businessObject,new ArgumentOutOfRangeException("value",value,"Value must be in IncludeValues"));
			            if (!throwOnInvalid) return ex;
			        	throw ex;
			        }
			    }
				if (rules.ExcludeValuesSpecified && rules.ExcludeValues!=null) {
			        if (rules.ExcludeValues.Cast<object>().Any(a => Comparer.Default.Compare(a, value) == 0)) {
						var ex=new ValueValidationException(value, businessObject,new ArgumentOutOfRangeException("value",value,"Value must not be in EcludeValues!"));
			            if (!throwOnInvalid) return ex;
			        	throw ex;
			        }
			    }
			} else {
			    throw new NotImplementedException("Type is not comparable! ValidateValueInternal must be overridden! " + value.GetType().FullName + ", Implementing Type: " + this.GetType().FullName);
			}

			return null;
		}
	}
}