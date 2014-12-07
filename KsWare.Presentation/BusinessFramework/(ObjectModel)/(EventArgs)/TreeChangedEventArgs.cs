using System.Diagnostics.CodeAnalysis;
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


	/// <summary> Provides additional methods for <see cref="TreeChangedEventArgs"/>
	/// </summary>
	public static class TreeChangedEventArgsExtension {

		/// <summary> Gets the business object.
		/// </summary>
		/// <param name="args">The <see cref="KsWare.Presentation.BusinessFramework.TreeChangedEventArgs"/> instance containing the event data.</param>
		/// <returns>The business object</returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static IObjectBM GetBusinessObject(this TreeChangedEventArgs args) {
			if(args.OriginalSource==null) return null;
			//TODO: return null or throw exception if original source is no IObjectBM
			return (IObjectBM)args.OriginalSource;
		}
	}
}