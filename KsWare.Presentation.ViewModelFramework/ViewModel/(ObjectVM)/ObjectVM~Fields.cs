using System;
using System.Diagnostics;


namespace KsWare.Presentation.ViewModelFramework {

	public partial class ObjectVM {

		private Lazy<BackingFieldsStore> _lazyFields;

		private void InitPartFields() {
			_lazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public BackingFieldsStore Fields { get { return _lazyFields.Value; } }
		public BackingFieldsStore FieldsːDebug { get { return _lazyFields.IsValueCreated ? _lazyFields.Value : null; } }
	}

}
