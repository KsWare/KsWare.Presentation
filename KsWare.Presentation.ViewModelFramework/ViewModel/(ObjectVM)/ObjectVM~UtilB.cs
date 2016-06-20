using System;
using System.Collections;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework 
{
	partial class ObjectVM
	{

		/// <summary> Provides mapping using the <see cref="CustomDataProvider{TData}"/>
		/// </summary>
		protected class CMap {

			public static ListVM<TItemVM> ListVM<TItemVM,TDataList>(Func<TDataList> getter, Action<TDataList> setter) 
				where TItemVM:class, IObjectVM 
				where TDataList:IEnumerable 
			{
				return new ListVM<TItemVM > {
					Metadata = new ListViewModelMetadata {
						DataProvider    = new CustomDataProvider<TDataList>(getter,setter),
					}
				};
			}

			public static ListVM<TItemVM> ListVM<TItemVM,TDataList>(Func<TDataList> getter, Action<TDataList> setter, INewItemProvider newItemProvider) 
				where TItemVM:class, IObjectVM 
				where TDataList:IEnumerable 
			{
				return new ListVM<TItemVM > {
					Metadata = new ListViewModelMetadata {
						DataProvider    = new CustomDataProvider<TDataList>(getter,setter),
						NewItemProvider = newItemProvider
					}
				};
			}

		}

	}

}
