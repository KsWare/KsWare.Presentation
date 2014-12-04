using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework {

	public partial class ObjectVM {

		private Lazy<BackingFieldsStore> m_LazyFields;

		private void InitPartFields() {
			m_LazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		public BackingFieldsStore Fields { get { return m_LazyFields.Value; } }

	}

}
