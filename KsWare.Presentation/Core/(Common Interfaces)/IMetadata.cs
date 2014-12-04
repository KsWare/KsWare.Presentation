/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : IMetadata.cs
 * OriginalNamespace: KsWare.Presentation
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Scope="type", Target="KsWare.Presentation.IMetadata")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Scope="type", Target="KsWare.Presentation.IMetadata`1")]

namespace KsWare.Presentation {

	/// <summary> Interface for framework object metadata (ObjectBM,ObjectVM)
	/// </summary>
	public interface IMetadata:IParentSupport {
		
	}

	/// <summary> Generic interface for metadata
	/// </summary>
	/// <typeparam name="TParent">The type of the Parent.</typeparam>
	public interface IMetadata<TParent>:IParentSupport<TParent> {

	}
}
