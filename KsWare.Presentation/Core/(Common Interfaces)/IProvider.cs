using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KsWare.Presentation.Providers {

	/// <summary> Interface for providers
	/// </summary>
	public interface IProvider:IParentSupport,INotifyPropertyChanged,IDisposable {

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>. </value>
		bool IsSupported{get;}
	}

	/// <summary> Interface for metadata providers
	/// </summary>
	public interface IMetadataProvider:IProvider {

		/// <summary> Gets the metadata which holds this provider.
		/// </summary>
		/// <value>The metadata.</value>
		IMetadata Metadata{get;}
	}

	/// <summary> Generic interface for metadata providers
	/// </summary>
	/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
	public interface IMetadataProvider<TMetadata>:IParentSupport<TMetadata>, IProvider {
		/// <summary> Gets the metadata which holds this provider.
		/// </summary>
		/// <value>The metadata.</value>
		TMetadata Metadata{get;}
	}
}
