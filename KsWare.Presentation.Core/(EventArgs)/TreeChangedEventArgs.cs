using KsWare.Presentation.Documentation;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides arguments for the TreeChanged-event
	/// </summary>
	/// <seealso cref="docːObjectBM.TreeChangedˑexample1"/>
	public class TreeChangedEventArgs:RoutedComponentEventArgs {

		/// <summary> Initializes a new instance of the <see cref="TreeChangedEventArgs"/> class.
		/// </summary>
		/// <param name="originalSource">The original source.</param>
		public TreeChangedEventArgs(object originalSource): base(originalSource) {}
	}

}