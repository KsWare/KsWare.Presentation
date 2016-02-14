using System.Diagnostics.CodeAnalysis;

namespace KsWare.Presentation.BusinessFramework {

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