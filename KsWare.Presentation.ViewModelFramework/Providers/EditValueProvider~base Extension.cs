using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderExtension : INotifyPropertyChanged /*TODO ,IDataErrorInfo*/ {

	}

	partial class EditValueProvider {

		/// <summary> EditValueProvider extension base
		/// </summary>
		abstract class Extension:INotifyPropertyChanged,IDataErrorInfo {

			private EditValueProvider m_Provider;
			private Dictionary<string,string> m_ErrorInfos=new Dictionary<string, string>();
			private string m_ErrorInfo;

			protected Extension(EditValueProvider provider) {
				m_Provider = provider;
			}

			/// <summary> Gets the associated provider.
			/// </summary>
			/// <value>The associated provider.</value>
			protected EditValueProvider Provider { get { return m_Provider; } }

			/// <summary> Gets the associated view model.
			/// </summary>
			/// <value>The associated view model.</value>
			protected IValueVM ViewModel { get { return m_Provider.ViewModel; } }

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
			
			protected void ResetError() { m_ErrorInfo = null; }

			protected void ResetError(string property) { m_ErrorInfos.Remove(property); }

			protected void SetError(string message) { m_ErrorInfo = message; }

			protected void SetError(string property, string message) {
				if(m_ErrorInfos.ContainsKey(property)) m_ErrorInfos[property] = message;
				else m_ErrorInfos.Add(property,message);
			}

			protected bool HasError { get { return !string.IsNullOrEmpty(m_ErrorInfo) || m_ErrorInfos.Count > 0; } }

			string IDataErrorInfo.this[string columnName] {
				get {
					string value;
					if (m_ErrorInfos.TryGetValue(columnName, out value)) return value;
					return ViewModel.Metadata.ErrorProvider.HasError == false ? "" : ViewModel.Metadata.ErrorProvider.ErrorMessage;
				}
			}

			string IDataErrorInfo.Error {
				get {
					if (!string.IsNullOrEmpty(m_ErrorInfo)) return m_ErrorInfo;
					return ViewModel.Metadata.ErrorProvider.HasError == false ? "" : ViewModel.Metadata.ErrorProvider.ErrorMessage;
				}
			}
		}
	}
}
