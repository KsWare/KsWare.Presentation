using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.Documentation {

	public class docːObjectVM {

		/// <summary> 
		/// With ʿunderlyingˑdataʾ is meant the access to ObjectVM.Metadata.DataProvider.Data. 
		/// Because this data is essential the <see cref="viewˑmodels"/> implement see <see cref="ObjectVM.MːData"/>.
		/// <see cref="viewˑmodels"/> which derive from <see cref="DataVM"/> have a <see cref="DataVM{TData}.Data"/> property.
		/// <see cref="businessˑviewˑmodels"/> (derived from <see cref="BusinessObjectVM{TBusinessObject}"/>) have a <see cref="BusinessObjectVM{TBusinessObject}.BusinessObject"/> property.
		/// </summary>
		public object underlyingˑdata;

		public object viewˑmodels;

		/// <summary> ʿbusinessˑviewˑmodelsʾ are  <see cref="viewˑmodels"/> which derive from <see cref="BusinessObjectVM{TBusinessObject}"/>
		/// </summary>
		public object businessˑviewˑmodels;

	}
}
