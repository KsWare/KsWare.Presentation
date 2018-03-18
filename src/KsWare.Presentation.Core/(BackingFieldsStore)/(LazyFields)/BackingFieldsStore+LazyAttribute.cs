using System;

namespace KsWare.Presentation {

	partial class BackingFieldsStore {

		public sealed class LazyAttribute : Attribute {

			public LazyAttribute(Type valueFactory) {
				ValueFactory = valueFactory;
			}

			public Type ValueFactory { get; set; }
		}
	}


}
