using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderExtension : INotifyPropertyChanged /*TODO ,IDataErrorInfo*/ {

	}

	partial class EditValueProvider {

		/// <summary> EditValueProvider extension base
		/// </summary>
		abstract class Extension:INotifyPropertyChanged,IDataErrorInfo {

			private EditValueProvider _provider;
			private Dictionary<string,string> _errorInfos=new Dictionary<string, string>();
			private string _errorInfo;

			protected Extension(EditValueProvider provider) {
				_provider = provider;
			}

			/// <summary> Gets the associated provider.
			/// </summary>
			/// <value>The associated provider.</value>
			protected EditValueProvider Provider { get { return _provider; } }

			/// <summary> Gets the associated view model.
			/// </summary>
			/// <value>The associated view model.</value>
			protected IValueVM ViewModel { get { return _provider.ViewModel; } }

			/// <summary> Gets the associated metadata. 
			/// </summary>
			/// <value>The associated metadata.</value>
			protected ViewModelMetadata Metadata{[CanBeNull] get {return ((ViewModelMetadata) Provider.Metadata);}}

			/// <summary> Occurs when a property value changes.
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			/// <summary> Called when property changed. Raises the <see cref="PropertyChanged"/> event.
			/// </summary>
			/// <param name="propertyName">Name of the property.</param>
			[NotifyPropertyChangedInvocator]
			protected virtual void OnPropertyChanged(string propertyName) {
				var handler = PropertyChanged;
				if(handler!=null) handler(this, new PropertyChangedEventArgs(propertyName));
			}
			
			protected void ResetError() { _errorInfo = null; }

			protected void ResetError(string property) { _errorInfos.Remove(property); }

			protected void SetError(string message) { _errorInfo = message; }

			protected void SetError(string property, string message) {
				if(_errorInfos.ContainsKey(property)) _errorInfos[property] = message;
				else _errorInfos.Add(property,message);
			}

			protected bool HasError { get { return !string.IsNullOrEmpty(_errorInfo) || _errorInfos.Count > 0; } }

			string IDataErrorInfo.this[string columnName] {
				get {
					string value;
					if (_errorInfos.TryGetValue(columnName, out value)) return value;
					return ViewModel.Metadata.ErrorProvider.HasError == false ? "" : ViewModel.Metadata.ErrorProvider.ErrorMessage;
				}
			}

			string IDataErrorInfo.Error {
				get {
					if (!string.IsNullOrEmpty(_errorInfo)) return _errorInfo;
					return ViewModel.Metadata.ErrorProvider.HasError == false ? "" : ViewModel.Metadata.ErrorProvider.ErrorMessage;
				}
			}
		}
	}
}
