using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using KsWare.Presentation.Providers;
using KsWare.Presentation.Core;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Business object
	/// </summary>
	public partial class ObjectBM {

		private Lazy<BackingFieldsStore> m_LazyFields;

		private void InitPartFields() {
			m_LazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		protected void OnPropertyChanged(string propertyName) {
			OnBusinessPropertyChanged(new BusinessPropertyChangedEventArgs(propertyName));
		}

		public BackingFieldsStore Fields { get { return m_LazyFields.Value; } }

	}
}
