using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using KsWare.Presentation.Providers;

namespace KsWare.Presentation.BusinessFramework.Providers {

	/// <summary> NewItemProvider-interface
	/// </summary>
	/// <remarks>
	/// The NewItemProvider is used in <see cref="ListBM{T}"/> to create new items and can also assign the underlying data.
	/// The NewItemProvider can be specified in the metadata of a <see cref="ListBM{T}"/>
	/// </remarks>
	public interface INewItemProvider:IProvider {
		/// <summary> Creates a new item.
		/// </summary>
		/// <typeparam name="TItem">Type of item</typeparam>
		/// <param name="data">The data.</param>
		/// <returns>The new item</returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		TItem CreateItem<TItem>(object data) /*where TItem:IObjectBM*/; 
	}

	/// <summary> Provides the default <see cref="INewItemProvider"/>
	/// </summary>
	/// <remarks>
	/// The default logic to create a new ListBM item is 
	/// <code>new T {Metadata = {DataProvider = {Data = data}}}</code>.
	/// </remarks>
	public class DefaultNewItemProvider:Provider, INewItemProvider {

		/// <summary> Initializes a new instance of the <see cref="DefaultNewItemProvider" /> class.
		/// </summary>
		public DefaultNewItemProvider() {
			TypeMap=new Dictionary<Type, Type>();
		}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value>	<see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>. </value>
		public override bool IsSupported {get {return true;}}

		/// <summary> Creates a new item.
		/// </summary>
		/// <typeparam name="TItem">Type of item</typeparam>
		/// <param name="data">The data.</param>
		/// <returns>The new item</returns>
		public TItem CreateItem<TItem>(object data)/* where TItem:IObjectBM*/ {
			var isBusinessModel = typeof (IObjectBM).IsAssignableFrom(typeof (TItem));
			if (!isBusinessModel) return (TItem) data; // ???

			var dataType = data.GetType();
			Type businessType = null;
			foreach (var p in TypeMap) {
				if(p.Key==dataType){businessType=p.Value;break;}
				if(p.Value==dataType){businessType=p.Key;break;}
			}
			if (businessType == null) {
				//TODO check for interface
				businessType = typeof (TItem);
			}

			if(businessType==null) {
				var err = "No matching business type found. DataType:" + dataType.FullName + ", ErrorID:{EDF9BA7E-7D28-4F5B-9A4F-D06BF7B86023}";
				Debug.WriteLine(err); throw new InvalidOperationException(err);
			}
			var bm=(IObjectBM)Activator.CreateInstance(businessType);

			// TODO revise: possible assiging of default provider (on demand).
			if(bm.Metadata.DataProvider!=null && bm.Metadata.DataProvider.IsSupported) {
				bm.Metadata.DataProvider.Data = data;
			}

			return (TItem)bm;
		}

		public IDictionary<Type, Type> TypeMap { get; set; }


	}

	/// <summary> Provides a custom defined <see cref="INewItemProvider"/>
	/// </summary>
	public class CustomNewItemProvider:Provider,INewItemProvider {

		private CreateNewItemCallbackHandler m_CreateNewItemCallback;

		/// <summary> Initializes a new instance of the <see cref="CustomNewItemProvider"/> class.
		/// </summary>
		/// <param name="createNewItemCallback">The create new item callback.</param>
		public CustomNewItemProvider(CreateNewItemCallbackHandler createNewItemCallback) {
			m_CreateNewItemCallback = createNewItemCallback;
		}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>. </value>
		public override bool IsSupported {get {return true;}}

		/// <summary> Gets or sets the create new item callback.
		/// </summary>
		/// <value>The create new item callback.</value>
		public CreateNewItemCallbackHandler CreateNewItemCallback {
			get {return m_CreateNewItemCallback;}
			set {
				if(m_CreateNewItemCallback!=null) throw new InvalidOperationException("CreateNewItemCallback already specified!");
				m_CreateNewItemCallback=value;
			}
		}

		/// <summary> Creates a new item.
		/// </summary>
		/// <typeparam name="TItem">Type of item</typeparam>
		/// <param name="data">The data.</param>
		/// <returns>The new item</returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TItem CreateItem<TItem>(object data) /*where TItem:IObjectBM*/  { 
			if(m_CreateNewItemCallback==null) throw new InvalidOperationException("CreateNewItemCallback not specified!");
			return (TItem) m_CreateNewItemCallback(data);
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public delegate object CreateNewItemCallbackHandler(object data);
}
