using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KsWare.Presentation.Core {

	/// <summary> Specifies the default view for the tagged view model
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class DefaultViewAttribute:Attribute {

		/// <summary> Specifies the default view for the tagged view model
		/// </summary>
		/// <param name="view">The view.</param>
		public DefaultViewAttribute(Type view) {
			View = view;
		}

		/// <summary> Gets or sets the  default view for the tagged view model.
		/// </summary>
		/// <value> The view. </value>
		public Type View { get; set; }
	}
}
