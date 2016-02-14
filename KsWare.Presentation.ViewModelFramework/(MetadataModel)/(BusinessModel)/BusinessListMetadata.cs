using System;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides meta data for a <see cref="ListVM{T}"/> with underlying <see cref="ListBM{T}"/>
	/// </summary>
	/// <typeparam name="T">Type of ListBM{T}-item</typeparam>
	public class BusinessListMetadata<T>:ListViewModelMetadata where T:IObjectBM {

		/// <summary> Initializes a new instance of the <see cref="BusinessListMetadata{T}"/> class.
		/// </summary>
		public BusinessListMetadata() {
			DataProvider = new LocalDataProvider<IListBM<T>>();
		}
	}
}