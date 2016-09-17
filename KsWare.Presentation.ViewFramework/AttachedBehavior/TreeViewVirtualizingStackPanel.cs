using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;


namespace KsWare.Presentation.ViewFramework {

	/// <summary>
	/// <see cref="VirtualizingStackPanel"/> with a public <see cref="VirtualizingStackPanel.BringIndexIntoView"/> method
	/// </summary>
	/// <seealso cref="System.Windows.Controls.VirtualizingStackPanel" />
	public class TreeViewVirtualizingStackPanel : VirtualizingStackPanel {

		/// <summary>
		/// Generates the item at the specified index position and brings it into view.
		/// </summary>
		/// <value>The position of the item to generate and make visible.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The index position does not exist in the child collection.</exception>
		/// <remarks>This a public alias for  <see cref="VirtualizingStackPanel.BringIndexIntoView"/></remarks>
		public new void BringIndexIntoView(int index) { base.BringIndexIntoView(index); }

	}

}
