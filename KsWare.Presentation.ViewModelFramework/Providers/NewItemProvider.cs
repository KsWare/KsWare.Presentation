using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> NewItemProvider-interface
	/// </summary>
	/// <remarks>
	/// The NewItemProvider is used in <see cref="ListVM{T}"/> to create new items and can also assign the underlying data.
	/// The NewItemProvider can be specified in the metadata of a <see cref="ListVM{T}"/>
	/// </remarks>
	public interface INewItemProvider:IViewModelProvider {

		/// <summary> Creates the item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		T CreateItem<T>(object data) /*where T:class,IObjectVM*/; 

		IDictionary<Type, Type> TypeMap { get; set; }
	}

	/// <summary> Provides the default <see cref="INewItemProvider"/>
	/// </summary>
	/// <remarks>
	/// The default logic to create a new ListVM item is 
	/// <code>new T {Metadata = {DataProvider = {Data = data}}}</code>.
	/// </remarks>
	public class DefaultNewItemProvider:ViewModelProvider,INewItemProvider {

		private IDictionary<Type, Type> m_TypeMap;

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported {get {return true;}}

		/// <summary> Creates a new item.
		/// </summary>
		/// <typeparam name="T">Type of item</typeparam>
		/// <param name="data">The data.</param>
		/// <returns>The new item</returns>
		/// <remarks>
		/// <p>Lookup sequence: 
		/// <see cref="TypeMap"/>, 
		/// <see cref="NewItemProviderTypeMapAttribute"/>, 
		/// <see cref="NewItemProviderItemFactoryAttribute"/>.</p> </remarks>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public T CreateItem<T>(object data) /*where T:class,IObjectVM*/ {
			// EXPERIMENTAL for compatibility, if T is no IObjectVM we return only the data
			if (!typeof (IObjectVM).IsAssignableFrom(typeof (T))) return (T)data; // TODO: verify this

			// ### this part is for IObjectVM items ###

			var businessType = data.GetType();
			var declaredViewModelType = typeof (T);
			Type viewmodelType = null;
			T newItem=default(T);
			if (m_TypeMap != null) {
				foreach (var p in m_TypeMap) {
					if(p.Key==businessType){viewmodelType=p.Value;break;}
					if(p.Value==businessType){viewmodelType=p.Key;break;}
				}				
			}
			if (newItem==null && viewmodelType == null && declaredViewModelType.IsInterface) {
				var mapAttributes = declaredViewModelType.GetCustomAttributes(typeof (NewItemProviderTypeMapAttribute),false).Cast<NewItemProviderTypeMapAttribute>().ToArray();
				foreach (var map in mapAttributes) {
					if(map.TypeA==businessType){viewmodelType=map.TypeB;break;}
					if(map.TypeB==businessType){viewmodelType=map.TypeA;break;}
				}
			}
			if (newItem==null && viewmodelType == null && declaredViewModelType.IsInterface) {
					//TODO complete usage of NewItemProviderItemFactoryAttribute
					var factoryType = declaredViewModelType.GetCustomAttributes(typeof (NewItemProviderItemFactoryAttribute),false).Cast<NewItemProviderItemFactoryAttribute>().Select(x=>x.Factory).FirstOrDefault();
					if (factoryType != null) {
						var factory = (INewItemProvider)Activator.CreateInstance(factoryType);
						newItem=factory.CreateItem<T>(data);
					}
			}
			if (newItem == null && viewmodelType == null) {
				if (!declaredViewModelType.IsInterface && !declaredViewModelType.IsAbstract) {
					viewmodelType = declaredViewModelType;
				}
			}

			if(newItem==null && viewmodelType==null) {
				var err = "No matching ViewModel type found. BusinessModel type:" + businessType.FullName + ", ErrorID:{B9405E9B-DA5D-4D41-95E2-90032C57E865}";
				Debug.WriteLine(err); throw new InvalidOperationException(err);;
			}


			#region use T.New(object)
			var theNewMethod=typeof (T).GetMethod("New", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new []{typeof (object)}, null);
			if(theNewMethod!=null) {
				newItem = (T)theNewMethod.Invoke(null, new object[] {data});
				return newItem;
			}
			#endregion

			if (newItem == null) {
				try {newItem=(T)Activator.CreateInstance(viewmodelType);} catch (Exception ex) {
					Debug.WriteLine(ex.ToString());DebuggerːBreak("TypeInitializationException "+viewmodelType);
					throw new TypeInitializationException(viewmodelType.FullName, ex);
				}				
			}
			
			AssignMetaData((IObjectVM)newItem, data);
			AssignData((IObjectVM)newItem, data);
			return newItem;
		}

		public IDictionary<Type, Type> TypeMap {
			get {
				if(m_TypeMap==null)m_TypeMap = new Dictionary<Type, Type>();
				return m_TypeMap;
			}
			set { m_TypeMap = value; }
		}

		protected virtual void AssignMetaData(IObjectVM item, object data) {
//			if(BusinessObjectVM.IsBusinessObjectVM(item.GetType())) {
//				var tMetadata= typeof (BusinessObjectMetadata<>).MakeGenericType(BusinessObjectVM.GetBusinessType(item.GetType()));
//				item.Metadata = (ViewModelMetadata) Activator.CreateInstance(tMetadata);
//			} else {
//				//uses default M
//			}
		}

		protected virtual void AssignData(IObjectVM item, object data) {
			if(item.Metadata.DataProvider!=null && item.Metadata.DataProvider.IsSupported) {
				if(item.Metadata.DataProvider is IBusinessValueDataProvider) {
					((IBusinessValueDataProvider) item.Metadata.DataProvider).BusinessValue = (IValueBM)data;
				} else {
					item.Metadata.DataProvider.Data = data;
				}
			}
		}
	}

	[AttributeUsage(AttributeTargets.Interface,AllowMultiple = true)]
	public class NewItemProviderTypeMapAttribute:Attribute {

		public NewItemProviderTypeMapAttribute() {}

		public NewItemProviderTypeMapAttribute(Type typeA, Type typeB) {
			TypeA = typeA;
			TypeB = typeB;
		}
		
		public Type TypeA { get; set; }
		public Type TypeB { get; set; }
	}



	/// <summary> Class NewItemProviderItemFactoryAttribute.
	/// </summary>
	/// <example><code>
	/// [NewItemProviderItemFactory(typeof(MyViewModelFactory))]
	///	public interface IMyViewModelInterface {
	///		/* .. interface members ..*/
	/// }
	/// public class MyViewModelFactory : INewItemProvider {
	///		public object CreateInstance(Type type) { return Activator.CreateInstance(type); }
	///	}
	/// </code></example>
	[AttributeUsage(AttributeTargets.Interface,AllowMultiple = false)]
	public class NewItemProviderItemFactoryAttribute:Attribute {

		private Type m_Factory;

		public NewItemProviderItemFactoryAttribute() {}

		/// <summary> Initializes a new instance of the <see cref="NewItemProviderItemFactoryAttribute"/> class.
		/// </summary>
		/// <param name="factory">The type of the factory.
		/// The type must implement <see cref="INewItemProvider"/></param>
		public NewItemProviderItemFactoryAttribute(Type factory) {
			Factory = factory;
		}

		/// <summary> Gets or sets the type of the factory.
		/// The type must implement <see cref="ITypeFactory"/>
		/// </summary>
		/// <value>The factory.</value>
		public Type Factory {
			get { return m_Factory; }
			set {
				if(!typeof(ITypeFactory).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException("value", "Type does not implement ITypeFactory");
				m_Factory = value;
			}
		}

	}

	public interface ITypeFactory {

		object CreateInstance(Type type);

	}

	/// <summary> Provides a custom defined <see cref="INewItemProvider"/>
	/// </summary>
	public class CustomNewItemProvider:ViewModelProvider,INewItemProvider {

		private CreateNewItemCallbackHandler m_CreateNewItemCallbackHandler;

		/// <summary> Initializes a new instance of the <see cref="CustomNewItemProvider"/> class.
		/// </summary>
		/// <param name="createNewItemCallback">The create new item callback.</param>
		public CustomNewItemProvider(CreateNewItemCallbackHandler createNewItemCallback) { m_CreateNewItemCallbackHandler = createNewItemCallback; }

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported {get {return true;}}

//		/// <summary> Gets or sets the parent of this instance.
//		/// </summary>
//		/// <value>The parent of this instance.</value>
//		/// <remarks></remarks>
//		public object Parent {
//			get {return _parent;}
//			set {
//				MemberAccessUtil.DemandNotNull(value,"Parent cannot be null!",this,"{8E672971-9DD5-48CE-A701-AAA95B9DEB0C}");
//				MemberAccessUtil.DemandWriteOnce(_parent==null,null,this,"Parent","{C73E95C8-7EE0-4343-8685-2E51CB31F5B3}");
//				_parent = value;
//				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{AED31935-E532-4A5F-8940-0F59516281DC}");
//			}
//		}

//		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
//		/// </summary>
//		/// <remarks></remarks>
//		public event EventHandler ParentChanged;

		/// <summary> Gets or sets the "CreateNewItem" callback.
		/// </summary>
		/// <value>The create new item callback.</value>
		public CreateNewItemCallbackHandler CreateNewItemCallback {
			get {return m_CreateNewItemCallbackHandler;}
			set {
				if(m_CreateNewItemCallbackHandler!=null) throw new InvalidOperationException("CreateNewItemCallback already specified!");
				m_CreateNewItemCallbackHandler=value;
			}
		}

		/// <summary> Creates the item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public T CreateItem<T>(object data) /*where T:class,IObjectVM*/  { 
			if(m_CreateNewItemCallbackHandler==null) throw new InvalidOperationException("CreateNewItemCallback not specified!");
			return (T) m_CreateNewItemCallbackHandler(data);
		}

		public IDictionary<Type, Type> TypeMap { get; set; }
	}

	/// <summary> Delegate for "CreateNewItem" callback
	/// </summary>
	/// <param name="data">The data.</param>
	/// <returns></returns>
	/// <remarks></remarks>
	public delegate object CreateNewItemCallbackHandler(object data);
}
