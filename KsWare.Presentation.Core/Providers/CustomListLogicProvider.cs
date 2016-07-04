
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
using JetBrains.Annotations;
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
	[PublicAPI]
	public class NoListLogicProvider:IListLogicProvider {

		private IMetadata _parent;
		EventHandler _parentChanged;
		private Lazy<EventSourceStore> _lazyWeakEventProperties;

		/// <summary>
		/// Initializes a new instance of the <see cref="NoListLogicProvider"/> class.
		/// </summary>
		public NoListLogicProvider() {
			_lazyWeakEventProperties=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}
		public EventSourceStore EventSources{get { return _lazyWeakEventProperties.Value; }}

		/// <summary> Gets the metadata which holds this provider.
		/// </summary>
		/// <value>The metadata.</value>
		public IMetadata Metadata {get {return (IMetadata) _parent;}}

		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object IParentSupport.Parent {
			get {return _parent;}
			set {
				_parent = (IMetadata) value;
				EventUtil.Raise(_parentChanged,this,EventArgs.Empty,"{202C8927-F9CF-49A3-A07B-6B37837B724B}");
				EventManager.Raise<EventHandler,EventArgs>(_lazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="IParentSupport.Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		event EventHandler IParentSupport.ParentChanged {add { _parentChanged += value; }remove { _parentChanged -= value; }}

		/// <summary> Gets the event source for the event which occurs when the <see cref="IParentSupport.Parent"/> property has been changed.
		/// </summary>
		/// <value>The event source.</value>
		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); } }

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value> Always <see langword="false"/>.
		/// </value>
		public bool IsSupported {get {return false;}}

		public bool IsAutoCreated { get { return false; } set { throw new NotSupportedException("{C5790F29-09F4-4EA0-A8AE-078A79EB230E}"); } }

		public bool IsInUse { get { return true; } set { throw new NotSupportedException("{FFEB6A77-1CB9-446F-9038-7546936D5455}"); } }

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
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(_lazyWeakEventProperties,"PropertyChangedEvent",args);
		}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {}

	}

	/// <summary> Provides custom list logic
	/// </summary>
	public class CustomListLogicProvider:Provider,IListLogicProvider {

		private NotifyCollectionChangedEventHandler _collectionChangingCallback;
		private NotifyCollectionChangedEventHandler _collectionChangedCallback;

		/// <summary> Initializes a new instance of the <see cref="CustomListLogicProvider"/> class.
		/// </summary>
		/// <param name="collectionChangingCallback">The collection changing callback.</param>
		/// <param name="collectionChangedCallback">The collection changed callback.</param>
		public CustomListLogicProvider(NotifyCollectionChangedEventHandler collectionChangingCallback, NotifyCollectionChangedEventHandler collectionChangedCallback) {
			_collectionChangingCallback = collectionChangingCallback;
			_collectionChangedCallback = collectionChangedCallback;
		}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><c><see langword="true"/></c> if this instance is supported; otherwise, <c><see langword="true"/></c>. </value>
		public override bool IsSupported {get {return true;}}


		/// <summary> Gets the metadata which holds this provider.
		/// </summary>
		/// <value>The metadata.</value>
		public IMetadata Metadata{get {return (IMetadata) Parent;}}

		/// <summary> Gets or sets the collection changing callback.
		/// </summary>
		/// <value>The collection changing callback.</value>
		public NotifyCollectionChangedEventHandler CollectionChangingCallback {
			get {return this._collectionChangingCallback;}
			set {
				MemberAccessUtil.DemandWrite(Parent==null,null,this,nameof(CollectionChangingCallback),"{B51E7C51-F3B0-4D28-972B-65789E5EF9B8}");
				_collectionChangingCallback = value;
			}
		}

		/// <summary> Gets or sets the collection changed callback.
		/// </summary>
		/// <value>The collection changed callback.</value>
		public NotifyCollectionChangedEventHandler CollectionChangedCallback {
			get {return _collectionChangedCallback;}
			set {
				MemberAccessUtil.DemandWrite(Parent==null,null,this,nameof(CollectionChangingCallback),"{2BB7B891-3DB6-46BF-A82F-3037C299B037}");
				_collectionChangedCallback = value;
			}
		}

		/// <summary> Called before the collections is changed.
		/// </summary>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		public void CollectionChanging(NotifyCollectionChangedEventArgs e) {
			if(Metadata==null) throw new InvalidOperationException("Provider not initialized! Metadata not specified!");
			if(Metadata.Parent==null) throw new InvalidOperationException("Provider not initialized! Metadata.Parent not specified!");
			if(_collectionChangingCallback!=null) _collectionChangingCallback(Metadata.Parent, e);
		}

		/// <summary> Called after the collections is changed.
		/// </summary>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		public void CollectionChanged(NotifyCollectionChangedEventArgs e) {
			if(Metadata==null) throw new InvalidOperationException("Provider not initialized! Metadata not specified!");
			if(Metadata.Parent==null) throw new InvalidOperationException("Provider not initialized! Metadata.Parent not specified!");
			if(_collectionChangedCallback!=null) _collectionChangedCallback(Metadata.Parent, e);
		}


	}
}