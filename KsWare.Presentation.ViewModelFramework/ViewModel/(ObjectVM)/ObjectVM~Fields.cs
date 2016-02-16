using System;

namespace KsWare.Presentation.ViewModelFramework {

	public partial class ObjectVM {

		private Lazy<BackingFieldsStore> m_LazyFields;

		private void InitPartFields() {
			m_LazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		public BackingFieldsStore Fields { get { return m_LazyFields.Value; } }

	}

}
