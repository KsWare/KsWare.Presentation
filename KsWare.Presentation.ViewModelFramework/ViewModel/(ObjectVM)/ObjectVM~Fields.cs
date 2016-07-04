using System;

namespace KsWare.Presentation.ViewModelFramework {

	public partial class ObjectVM {

		private Lazy<BackingFieldsStore> _lazyFields;

		private void InitPartFields() {
			_lazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		public BackingFieldsStore Fields { get { return _lazyFields.Value; } }

	}

}
