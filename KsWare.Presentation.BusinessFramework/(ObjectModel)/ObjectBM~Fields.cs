using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Business object
	/// </summary>
	public partial class ObjectBM: INotifyPropertyChanged {

		private Lazy<BackingFieldsStore> m_LazyFields;
		private PropertyChangedEventHandler m_PropertyChanged;

		private void InitPartFields() {
			m_LazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged(string propertyName) {
			OnBusinessPropertyChanged(new BusinessPropertyChangedEventArgs(propertyName));
			EventUtil.Raise(m_PropertyChanged,this,new PropertyChangedEventArgs(propertyName),"{5AF425CD-76F1-44D3-A3AE-D00B7811504D}");
		}

		public BackingFieldsStore Fields { get { return m_LazyFields.Value; } }


		/// <summary> Occurs when a property value changes. (Implements INotifyPropertyChanged.PropertyChanged)
		/// </summary>
		[Obsolete("For presentation infrastructure compatibility only!")]
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {add { m_PropertyChanged += value; } remove { m_PropertyChanged-=value; } }

	}
}
