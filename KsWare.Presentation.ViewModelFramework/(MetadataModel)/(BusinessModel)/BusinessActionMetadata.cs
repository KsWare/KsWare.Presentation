using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Providers;

namespace KsWare.Presentation.ViewModelFramework
{


	/// <summary>  Metadata for <see cref="ActionVM"/> with connection to business layer
	/// </summary>
	/// <remarks></remarks>
	public class BusinessActionMetadata:ActionMetadata
	{

		//NOT USED at present
		//		public BusinessObjectMetadata(Func<T> getter, Action<T> setter) {
		//			DataProvider = new BusinessObjectDataProvider<T>(getter,setter);
		//		}

		/// <summary> Initializes a new instance of the <see cref="BusinessActionMetadata"/> class.
		/// </summary>
		/// <remarks></remarks>
		public BusinessActionMetadata() {
//			ActionProvider = CreateDefaultActionProvider();
		}

		/// <summary> Creates the default action provider.
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected override IActionProvider CreateDefaultActionProvider() { 
			return new BusinessActionProvider();
		}

	}
}