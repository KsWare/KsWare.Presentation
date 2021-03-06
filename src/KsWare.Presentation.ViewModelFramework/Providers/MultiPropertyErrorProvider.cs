﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary>  [EXPERIMENTAL] MultiPropertyErrorProvider
	/// </summary>
	public class MultiPropertyErrorProvider : ErrorProvider,IDataErrorInfo {

//		IList<ValidationFailure> validationErrors;
		private Lazy<Dictionary<string, List<ValidationRule>>> _validationRules=new Lazy<Dictionary<string, List<ValidationRule>>>(() => new Dictionary<string, List<ValidationRule>>()); 
		private List<ValidationResult> _validationResults;

		public void Add<TRet>(Expression<Func<object,TRet>> propertyExpression, ValidationRule rule) {
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			Add(memberName, rule);
		}

		public void Add<TRet>(Expression<Func<TRet>> propertyExpression, ValidationRule rule) {
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			Add(memberName, rule);
		}

		public void Add(string propertyName, ValidationRule rule) {
			if (!_validationRules.Value.ContainsKey(propertyName)) {
				_validationRules.Value.Add(propertyName,new List<ValidationRule>());
			}
			var validationRules = _validationRules.Value[propertyName];
			validationRules.Add(rule);
		}

		/// <summary> Gets the error message for the property with the given name.
		/// </summary>
		/// <param name="propertyName">The name of the property whose error message to get.</param>
		/// <returns>The error message for the property. The default is an empty string ("").</returns>
		string IDataErrorInfo.this[string propertyName] {
			get {

//				return this.ValidationErrors != null && this.ValidationErrors.Any(error => error.PropertyName == propertyName)
//                    ? string.Join(Environment.NewLine, (from error in this.ValidationErrors where error.PropertyName == propertyName select error.ErrorMessage).ToArray())
//                        : null;

				if (!_validationRules.IsValueCreated) return "";
				if (!_validationRules.Value.ContainsKey(propertyName)) return "";
				var value=GetPropertyValue(propertyName);
				var results=new List<ValidationResult>();
				foreach (var validationRule in _validationRules.Value[propertyName]) {
					var validationResult = validationRule.Validate(value, Thread.CurrentThread.CurrentUICulture);
					results.Add(validationResult);
				}
				var errorStrings = results.Where(result => !result.IsValid).Select(result => result.ErrorContent.ToString()).ToArray();
				var ret = string.Join("\n",errorStrings);
				return ret;
			}
		}

		/// <summary> Gets an error message indicating what is wrong with this object.
		/// </summary>
		string IDataErrorInfo.Error => ErrorMessage;

		private object GetPropertyValue(string propertyName) {
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			var propertyInfo = this.GetType().GetProperty(propertyName, bindingFlags);
			var value=propertyInfo.GetValue(this, bindingFlags, null, null, Thread.CurrentThread.CurrentUICulture);
			return value;
		}

		public override bool HasError { get => base.HasError; protected set => base.HasError = value; }

	}
}