using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> Interface for all error providers
	/// </summary>
	/// <remarks></remarks>
	public interface IErrorProvider:IViewModelProvider {

		/// <summary> Gets a value indicating whether this instance has error.
		/// </summary>
		/// <remarks></remarks>
		bool HasError{get;}

		/// <summary>  Gets the error message.
		/// </summary>
		/// <remarks></remarks>
		string ErrorMessage{get;}

		/// <summary> Gets write access for this provider.
		/// </summary>
		/// <returns><see cref="IErrorProviderController"/>.</returns>
		IErrorProviderController WriteAccess();

	}

	/// <summary> Interface to control an error provider
	/// </summary>
	/// <remarks></remarks>
	public interface IErrorProviderController {

		/// <summary> Resets the error.
		/// </summary>
		/// <remarks></remarks>
		void ResetError();

		/// <summary> Sets the error.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <remarks></remarks>
		void SetError([Localizable(true)]string errorMessage);

		void ResetError([Localizable(false)] string propertyName);

		void ResetError<T>(Expression<Func<object,T>> propertyExpression);

		void ResetError<T>(Expression<Func<T>> propertyExpression);

		void SetError([Localizable(false)] string propertyName, [Localizable(true)]string errorMessage);

		void SetError<T>(Expression<Func<object,T>> propertyExpression, [Localizable(true)]string errorMessage);

		void SetError<T>(Expression<Func<T>> propertyExpression, [Localizable(true)]string errorMessage);
	}

	/// <summary>Implementation for default ErrorProvider
	/// </summary>
	public class ErrorProvider: ViewModelValueProvider, IErrorProvider,IErrorProviderController,IDataErrorInfo {

		private bool   _hasError;
		private string _errorMessage;
		private Dictionary<string,string> _FieldErrors=new Dictionary<string, string>(); 

		/// <summary> Initializes a new instance of the <see cref="ErrorProvider"/> class.
		/// </summary>
		public ErrorProvider() {
 			_hasError     = false;
			_errorMessage = null;
		}

		#region Implementation of IViewModelProvider

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported{get {return true;}}

		#endregion

		#region IErrorFeedback

		/// <summary> Gets a value indicating whether this instance has error.
		/// </summary>
		/// <value><c>true</c> if this instance has error; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
		public virtual bool HasError {
			get {return this._hasError;}
			protected set {
				if (Equals(this._hasError, value)) return;
				this._hasError = value;
				OnPropertyChanged("HasError");
			}
		}

		/// <summary> Gets the error message.
		/// </summary>
		/// <value>The error message.</value>
		/// <remarks></remarks>
		public virtual string ErrorMessage {
			get {return this._errorMessage;}
			protected set {
				if (Equals(this._errorMessage, value)) return;
				this._errorMessage = value;
				OnPropertyChanged("ErrorMessage");
			}
		}

		#endregion

		#region IErrorProviderWriter

		/// <summary> Sets the error.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <remarks></remarks>
		public void SetError(string errorMessage) {
			_errorMessage = errorMessage;
			HasError = true;
		}

		/// <summary> Resets the error for IDataErrorInfo.this[string columnName]
		/// </summary>
		/// <param name="propertyName"></param>
		void IErrorProviderController.ResetError(string propertyName) {
			_FieldErrors.Remove(propertyName);
		}

		void IErrorProviderController.ResetError<T>(Expression<Func<object, T>> propertyExpression) {
			var propertyName = MemberNameUtil.GetPropertyName(propertyExpression);
			((IErrorProviderController)this).ResetError(propertyName);
		}

		void IErrorProviderController.ResetError<T>(Expression<Func<T>> propertyExpression) {
			var propertyName = MemberNameUtil.GetPropertyName(propertyExpression);
			((IErrorProviderController)this).ResetError(propertyName);
		}

		/// <summary> Sets the error for IDataErrorInfo.this[string columnName]
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="errorMessage"></param>
		void IErrorProviderController.SetError(string propertyName, string errorMessage) {
			_FieldErrors.Remove(propertyName);
			_FieldErrors.Add(propertyName,errorMessage);
		}

		void IErrorProviderController.SetError<T>(Expression<Func<object, T>> propertyExpression, string errorMessage) {
			var propertyName = MemberNameUtil.GetPropertyName(propertyExpression);
			((IErrorProviderController)this).SetError(propertyName,errorMessage);
		}
		void IErrorProviderController.SetError<T>(Expression<Func<T>> propertyExpression, string errorMessage) {
			var propertyName = MemberNameUtil.GetPropertyName(propertyExpression);
			((IErrorProviderController)this).SetError(propertyName,errorMessage);
		}

		/// <summary> Resets the error.
		/// </summary>
		/// <remarks></remarks>
		public void ResetError() {
			_errorMessage = null;
			HasError = false;
		}

		#endregion

		#region Implementation of IDataErrorInfo

		string IDataErrorInfo.this[string columnName] { get {string value;return _FieldErrors.TryGetValue(columnName, out value) ? value : null;}}

		string IDataErrorInfo.Error { get { return _errorMessage; } }

		#endregion

		/// <summary> Gets write access for this provider.
		/// </summary>
		/// <returns><see cref="IErrorProviderController"/>.</returns>
		IErrorProviderController IErrorProvider.WriteAccess() { return (IErrorProviderController) this; }
	}
}
