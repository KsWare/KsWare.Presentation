/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : PropertyCache.cs
 * OriginalNamespace: 
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Linq;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides a cache for view model properties (<see cref="ViewModelProperty"/>)
	/// </summary>
	internal static class PropertyCache {
		
		private static readonly List<ViewModelProperty> InnerList=new List<ViewModelProperty>();

		internal static void Register(ViewModelProperty viewModelProperty) {
			lock (InnerList) {
				//TODO: validate
				RegisterInternal(viewModelProperty);
			}
		}

		private static void RegisterInternal(ViewModelProperty viewModelProperty) {
			InnerList.Add(viewModelProperty);
		}

		internal static ViewModelProperty GetProperty(string name, Type ownerType, bool autoRegister=false) {
			lock (InnerList) {
				var property = InnerList.FirstOrDefault(p => p.Name==name && p.OwnerType==ownerType);
				if(property==null && autoRegister) {
					property=new ViewModelProperty.RuntimeProperty(name,ownerType);
					RegisterInternal(property);
				} 
				return property;				
			}
		}
	}
}
