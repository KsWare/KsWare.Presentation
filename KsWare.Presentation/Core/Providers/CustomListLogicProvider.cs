
/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : CustomListLogicProvider.cs
 * OriginalNamespace: ?KsWare.Presentation.ViewModelFramework ??
 * OriginalAssembly : ?KsWare.Presentation.ViewModelFramework ??
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using KsWare.Presentation;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.Providers;

namespace KsWare.Presentation.Core.Providers {

	/// <summary> Interface for list logic providers
	/// </summary>
	public interface IListLogicProvider:IMetadataProvider {

		/// <summary> Called before the collections is changed.
		/// </summary>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		void CollectionChanging(NotifyCollectionChangedEventArgs e);

		/// <summary> Called after the collections is changed.
		/// </summary>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		void CollectionChanged(NotifyCollectionChangedEventArgs e);
	}

	/// <summary> A list logic provider with no support
	/// </summary>
	public class NoListLogicProvider:IListLogicProvider {

		private IMetadata m_Parent;
		EventHandler m_ParentChanged;
		private Lazy<EventSourceStore> m_LazyWeakEventProperties;

		/// <summary>
		/// Initializes a new instance of the <see cref="NoListLogicProvider"/> class.
		/// </summary>
		public NoListLogicProvider() {
			m_LazyWeakEventProperties=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}
		public EventSourceStore EventSources{get { return m_LazyWeakEventProperties.Value; }}

		/// <summary> Gets the metadata which holds this provider.
		/// </summary>
		/// <value>The metadata.</value>
		public IMetadata Metadata {get {return (IMetadata) m_Parent;}}

		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object IParentSupport.Parent {
			get {return m_Parent;}
			set {
				m_Parent = (IMetadata) value;
				EventUtil.Raise(m_ParentChanged,this,EventArgs.Empty,"{202C8927-F9CF-49A3-A07B-6B37837B724B}");
				EventManager.Raise<EventHandler,EventArgs>(m_LazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="IParentSupport.Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		event EventHandler IParentSupport.ParentChanged {add { m_ParentChanged += value; }remove { m_ParentChanged -= value; }}

		/// <summary> Gets the event source for the event which occurs when the <see cref="IParentSupport.Parent"/> property has been changed.
		/// </summary>
		/// <value>The event source.</value>
		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); } }

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value> Always <see langword="false"/>.
		/// </value>
		public bool IsSupported {get {return false;}}

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void IListLogicProvider.CollectionChanging(NotifyCollectionChangedEventArgs e) { throw new NotSupportedException("{AD721A0A-C864-410B-965A-34E14178AE19}"); }

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void IListLogicProvider.CollectionChanged(NotifyCollectionChangedEventArgs e) { throw new NotSupportedException("{E04009F8-C47B-45C3-BA84-F79DF786BC38}"); }

		public event PropertyChangedEventHandler PropertyChanged;
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent{get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{A0E5F06F-2946-48D7-8C19-A85477A7F9E7}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventProperties,"PropertyChangedEvent",args);
		}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {}

	}

	/// <summary> Provides custom list logic
	/// </summary>
	public class CustomListLogicProvider:IListLogicProvider {

		private NotifyCollectionChangedEventHandler m_CollectionChangingCallback;
		private NotifyCollectionChangedEventHandler m_CollectionChangedCallback;
		private IMetadata m_Parent;
		private Lazy<EventSourceStore> m_LazyWeakEventProperties;

		/// <summary> Initializes a new instance of the <see cref="CustomListLogicProvider"/> class.
		/// </summary>
		/// <param name="collectionChangingCallback">The collection changing callback.</param>
		/// <param name="collectionChangedCallback">The collection changed callback.</param>
		public CustomListLogicProvider(NotifyCollectionChangedEventHandler collectionChangingCallback, NotifyCollectionChangedEventHandler collectionChangedCallback) {
			m_CollectionChangingCallback = collectionChangingCallback;
			m_CollectionChangedCallback = collectionChangedCallback;
			m_LazyWeakEventProperties=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}
		public EventSourceStore EventSources{get { return m_LazyWeakEventProperties.Value; }}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><c><see langword="true"/></c> if this instance is supported; otherwise, <c><see langword="true"/></c>. </value>
		public bool IsSupported {get {return true;}}

		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		public object Parent {
			get {return m_Parent;}
			set {
				SetParentPattern.Execute(ref m_Parent, (IMetadata)value, "Parent");				
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{7C5F9D39-4698-49C6-B40C-CB6EE6E9B287}");
				EventManager.Raise<EventHandler,EventArgs>(m_LazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ParentChanged;

		/// <summary> Gets the event source for the event which occurs when the <see cref="IParentSupport.Parent"/> property has been changed.
		/// </summary>
		/// <value>The event source.</value>
		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); } }

		/// <summary> Gets the metadata which holds this provider.
		/// </summary>
		/// <value>The metadata.</value>
		public IMetadata Metadata{get {return (IMetadata) this.Parent;}}

		/// <summary> Gets or sets the collection changing callback.
		/// </summary>
		/// <value>The collection changing callback.</value>
		public NotifyCollectionChangedEventHandler CollectionChangingCallback {
			get {return this.m_CollectionChangingCallback;}
			set {
				MemberAccessUtil.DemandWrite(m_Parent==null,null,this,"CollectionChangingCallback","{B51E7C51-F3B0-4D28-972B-65789E5EF9B8}");
				m_CollectionChangingCallback = value;
			}
		}

		/// <summary> Gets or sets the collection changed callback.
		/// </summary>
		/// <value>The collection changed callback.</value>
		public NotifyCollectionChangedEventHandler CollectionChangedCallback {
			get {return m_CollectionChangedCallback;}
			set {
				MemberAccessUtil.DemandWrite(m_Parent==null,null,this,"CollectionChangingCallback","{2BB7B891-3DB6-46BF-A82F-3037C299B037}");
				m_CollectionChangedCallback = value;
			}
		}

		/// <summary> Called before the collections is changed.
		/// </summary>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		public void CollectionChanging(NotifyCollectionChangedEventArgs e) {
			if(Metadata==null) throw new InvalidOperationException("Provider not initialized! Metadata not specified!");
			if(Metadata.Parent==null) throw new InvalidOperationException("Provider not initialized! Metadata.Parent not specified!");
			if(m_CollectionChangingCallback!=null) m_CollectionChangingCallback(Metadata.Parent, e);
		}

		/// <summary> Called after the collections is changed.
		/// </summary>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		public void CollectionChanged(NotifyCollectionChangedEventArgs e) {
			if(Metadata==null) throw new InvalidOperationException("Provider not initialized! Metadata not specified!");
			if(Metadata.Parent==null) throw new InvalidOperationException("Provider not initialized! Metadata.Parent not specified!");
			if(m_CollectionChangedCallback!=null) m_CollectionChangedCallback(Metadata.Parent, e);
		}

		/// <summary> Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary> Gets the event source for the event which occurs when a property value changes..
		/// </summary>
		/// <value>The event source</value>
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent {
			get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }
		}

		/// <summary> Raises the <see cref="PropertyChanged"/> event and <see cref="PropertyChangedEvent"/>;
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{CC50B20C-5242-4795-844C-9DF843A9B701}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventProperties,"PropertyChangedEvent", args);
		}

		/// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {Dispose(true); }

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="explicitDispose"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool explicitDispose) {
			//TODO implement Dispose
		}
	}
}