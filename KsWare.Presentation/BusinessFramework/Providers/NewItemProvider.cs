using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
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
	public class DefaultNewItemProvider:INewItemProvider {

		private object m_Parent;
		protected Lazy<EventSourceStore> m_LazyWeakEventProperties;

		/// <summary> Initializes a new instance of the <see cref="DefaultNewItemProvider" /> class.
		/// </summary>
		public DefaultNewItemProvider() {
			TypeMap=new Dictionary<Type, Type>();
			m_LazyWeakEventProperties=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}
		public EventSourceStore EventSources{get { return m_LazyWeakEventProperties.Value; }}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value>	<see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>. </value>
		public bool IsSupported {get {return true;}}

		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		public object Parent {
			[CanBeNull]
			get {return m_Parent;}
			[NotNull]
			set {
				if(value==null) throw new InvalidOperationException("Parent cannot be null!");
				MemberAccessUtil.DemandWriteOnce(m_Parent==null,null,this,"Parent","{4C8DAA82-1C56-4AEA-93E3-06613761333A}");
				m_Parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{753002EE-5FFA-4D1B-BB67-5333CA80D826}");
				EventManager.Raise<EventHandler,EventArgs>(m_LazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ParentChanged;

		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); } }

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

		public event PropertyChangedEventHandler PropertyChanged;
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent{get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{B410DCA0-779B-4F54-9718-B3651E8E79C7}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventProperties,"PropertyChangedEvent", args);
		}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {
			if (explicitDispose) {
				if(m_LazyWeakEventProperties.IsValueCreated) m_LazyWeakEventProperties.Value.Dispose();
			}
		}
	}

	/// <summary> Provides a custom defined <see cref="INewItemProvider"/>
	/// </summary>
	public class CustomNewItemProvider:INewItemProvider {

		private CreateNewItemCallbackHandler m_CreateNewItemCallback;
		private object m_Parent;
		private Lazy<EventSourceStore> m_LazyWeakEventProperties;

		/// <summary> Initializes a new instance of the <see cref="CustomNewItemProvider"/> class.
		/// </summary>
		/// <param name="createNewItemCallback">The create new item callback.</param>
		public CustomNewItemProvider(CreateNewItemCallbackHandler createNewItemCallback) {
			m_CreateNewItemCallback = createNewItemCallback;
			m_LazyWeakEventProperties=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}
		public EventSourceStore EventSources{get { return m_LazyWeakEventProperties.Value; }}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>. </value>
		public bool IsSupported {get {return true;}}

		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		public object Parent {
			[CanBeNull]
			get {return m_Parent;}
			[NotNull]
			set {
				if(value==null) throw new InvalidOperationException("Parent cannot be null!");
				MemberAccessUtil.DemandWriteOnce(m_Parent==null,null,this,"Parent","{209D0BC2-5C85-4F28-B484-F10207C78CDD}");
				m_Parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{3E5C02E0-633C-48E9-8226-7DCAF8A14B28}");
				EventManager.Raise<EventHandler,EventArgs>(m_LazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ParentChanged;

		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); } }

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

		public event PropertyChangedEventHandler PropertyChanged;
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent{get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{B410DCA0-779B-4F54-9718-B3651E8E79C7}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventProperties,"PropertyChangedEvent", args);
		}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {
			if (explicitDispose) {
				if(m_LazyWeakEventProperties.IsValueCreated) m_LazyWeakEventProperties.Value.Dispose();
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public delegate object CreateNewItemCallbackHandler(object data);
}
