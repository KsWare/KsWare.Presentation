using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KsWare.Presentation.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides an enum type business object
	/// </summary>
	/// <typeparam name="T">An enum type</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public class EnumBM<T>:ValueBM<T> {

		/// <summary> Creates and returns default metadata for the <see cref="EnumBM{T}"/>
		/// </summary>
		/// <returns>The <see cref="BusinessValueMetadata"/> for <see cref="EnumBM{T}"/></returns>
		protected override BusinessMetadata CreateDefaultMetadata() {
			return CreateDefaultMetadataS(null);	
		}

		/// <summary> Creates and returns default metadata for the <see cref="EnumBM{T}"/>. 
		/// </summary>
		/// <param name="dataProvider">A <see cref="IDataProvider"/></param>
		/// <returns>The <see cref="BusinessValueMetadata"/> for <see cref="EnumBM{T}"/></returns>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static BusinessMetadata CreateDefaultMetadataS(IDataProvider dataProvider) {
			var metadata = new BusinessValueMetadata {
			    DataProvider  = dataProvider ?? new LocalDataProvider<T>(),
			    Settings      = new ValueSettings<T>()
			};
			return metadata;	
		}
	}
}