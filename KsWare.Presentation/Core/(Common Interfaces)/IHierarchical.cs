using System;
using System.Collections.Generic;

namespace KsWare.Presentation {

	/// <summary> Generic interface for hierarchical item
	/// </summary>
	/// <typeparam name="T">Type of hierarchical object</typeparam>
	public interface IHierarchical<T> {

		/// <summary> Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		T Parent{get;set;}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		event EventHandler ParentChanged;

		IWeakEventSource<EventHandler>  ParentChangedEvent { get;}

		/// <summary> Gets the children.
		/// </summary>
		/// <value>The children.</value>
		ICollection<T> Children{get;}

		/// <summary> Gets or sets the name of the member in the hierarchy.
		/// </summary>
		/// <value>The name of the member.</value>
		string MemberName {get;set;}

		/// <summary>
		/// Gets the member path.
		/// </summary>
		/// <value>The member path.</value>
		string MemberPath{get;}
	}
}
