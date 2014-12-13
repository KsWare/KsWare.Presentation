/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : IParentSupport.cs
 * OriginalNamespace: KsWare.Presentation
 * OriginalAssembly : KsWare.Presentation.Core
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace KsWare.Presentation 
{

	//TODO Revise usage of IParentSupport (oldname: IOwnerSupport) maybe use IHirarchical

	/// <summary> Interface for parent support
	/// </summary>
	public interface IParentSupport 
	{
		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		object Parent{get;set;}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		event EventHandler ParentChanged;

		IEventSource<EventHandler> ParentChangedEvent { get; }
	}

	/// <summary> Generic interface for parent support
	/// </summary>
	/// <typeparam name="TParent">Type of parent</typeparam>
	public interface IParentSupport<TParent> 
	{
		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		TParent Parent{get;set;}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		event EventHandler ParentChanged;

		IEventSource<EventHandler> ParentChangedEvent { get; }
	}
}
