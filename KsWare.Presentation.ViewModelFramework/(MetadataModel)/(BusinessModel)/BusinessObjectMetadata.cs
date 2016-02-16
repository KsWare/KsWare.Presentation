using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Special metadata for view models with connection to business layer
	/// </summary>
	/// <typeparam name="T">Type of ObjectBM</typeparam>
	/// <remarks></remarks>
	public class BusinessObjectMetadata<T>:ViewModelMetadata where T:IObjectBM {

//NOT USED at present
//		public BusinessObjectMetadata(Func<T> getter, Action<T> setter) {
//			DataProvider = new BusinessObjectDataProvider<T>(getter,setter);
//		}

		/// <summary> Initializes a new instance of the <see cref="BusinessObjectMetadata{T}"/> class.
		/// </summary>
		/// <remarks></remarks>
		public BusinessObjectMetadata() {
			DataProvider = new LocalDataProvider<T>();
		}
	}
}