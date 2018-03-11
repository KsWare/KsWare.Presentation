using System;
using System.ComponentModel;
using System.Diagnostics;
using JetBrains.Annotations;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Business object
	/// </summary>
	public partial class ObjectBM: INotifyPropertyChanged {

		private Lazy<BackingFieldsStore> _lazyFields;
		private PropertyChangedEventHandler _propertyChanged;

		private void InitPartFields() {
			_lazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged(string propertyName) {
			OnBusinessPropertyChanged(new BusinessPropertyChangedEventArgs(propertyName));
			EventUtil.Raise(_propertyChanged,this,new PropertyChangedEventArgs(propertyName),"{5AF425CD-76F1-44D3-A3AE-D00B7811504D}");
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public BackingFieldsStore Fields { get { return _lazyFields.Value; } }
		public BackingFieldsStore FieldsːDebug { get { return _lazyFields.IsValueCreated ? _lazyFields.Value : null; } }


		/// <summary> Occurs when a property value changes. (Implements INotifyPropertyChanged.PropertyChanged)
		/// </summary>
		[Obsolete("For presentation infrastructure compatibility only!")]
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {add { _propertyChanged += value; } remove { _propertyChanged-=value; } }

	}
}
