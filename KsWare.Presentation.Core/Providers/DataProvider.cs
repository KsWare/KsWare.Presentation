/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : DataProvider.cs
 * OriginalNamespace: KsWare.Presentation.Providers
 * OrigibalAssembly : KsWare.Presentation.Core
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using KsWare.Presentation.Providers;

namespace KsWare.Presentation.Core.Providers {

	/// <summary> Interface for data providers
	/// </summary>
	public interface IDataProvider:IProvider  {

		/// <summary> Occurs when <see cref="Data"/>-property has been changed.
		/// </summary>
		event EventHandler<DataChangedEventArgs> DataChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IEventSource<EventHandler<DataChangedEventArgs>>  DataChangedEvent { get; }
			
		/// <summary> Gets or sets the data validation callback.
		/// </summary>
		/// <value>The data validation callback.</value>
		DataValidatingCallbackHandler DataValidatingCallback{get;set;}

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		object Data{get;set;}

		object TryGetData(out Exception exception);

		/// <summary> Validates the specified data.
		/// </summary>
		/// <param name="data">The data to be validated.</param>
		/// <returns><see langword="null"/> is the data is valid; else <see cref="Exception"/></returns>
		Exception Validate(object data);

		/// <summary> Notifies that the provided data has been changed
		/// </summary>
		/// <remarks>
		/// Call this method if the provided data does not implement <see cref="INotifyCollectionChanged"/>
		/// </remarks>
		void NotifyDataChanged();

		/// <summary> Gets or sets a value indicating whether this instance has been automatically (on demand) created.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has been automatically created; otherwise, <c>false</c>.
		/// </value>
		bool IsAutoCreated { get; set; }

	}

	/// <summary> Generic DataProvider interface
	/// </summary>
	/// <typeparam name="TData">Type of provided data</typeparam>
	public interface IDataProvider<TData>:IProvider  {

		/// <summary> Gets or sets the data.
		/// </summary>
		/// <value>The provided data.</value>
		TData Data{get;set;}

		TData TryGetData(out Exception exception);

		/// <summary> Validates the specified data.
		/// </summary>
		/// <param name="data">The data to be validated.</param>
		/// <returns><see langword="null"/> is the data is valid; else <see cref="Exception"/></returns>
		Exception Validate(TData data);

		/// <summary> Gets or sets the data validation callback.
		/// </summary>
		/// <value>The data validation callback.</value>
		DataValidatingCallbackHandler DataValidatingCallback{get;set;}

		/// <summary> Notifies that the data has been changed
		/// </summary>
		/// <remarks>
		/// Call this method if the data does not implement <see cref="INotifyCollectionChanged"/>
		/// </remarks>
		void NotifyDataChanged();

	}

	/// <summary> DataProvider base class
	/// </summary>
	public abstract class DataProvider:IDataProvider {

		private object _Parent;
		private bool? _IsAutoCreated;
		private Lazy<EventSourceStore> _LazyWeakEventStore;

		protected DataProvider() {
			_LazyWeakEventStore=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		public EventSourceStore EventSources{get { return _LazyWeakEventStore.Value; }}

		#region Implementation of IProvider

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		public abstract bool IsSupported{get;}

		/// <summary> Gets or sets a value indicating whether this instance is auto created.
		/// </summary>
		/// <value> <c>true</c> if this instance is auto created; otherwise, <c>false</c>. </value>
		public bool IsAutoCreated {
			get { return _IsAutoCreated==true; }
			set {
				MemberAccessUtil.DemandWriteOnce(!_IsAutoCreated.HasValue,"The property can only be written once!",this,nameof(IsAutoCreated),"{E7279D65-F0FA-42BE-812F-45BA404524C8}");
				_IsAutoCreated = value;
			}
		}

		public bool IsInUse { get; set; }

		/// <summary> Gets or sets the parent of this provider.
		/// </summary>
		/// <value>The parent of this provider.</value>
		public object Parent {
			[CanBeNull]
			get {return _Parent;}
			[NotNull]
			set {
				if(value==null) throw new InvalidOperationException("Parent cannot be null!");
				MemberAccessUtil.DemandWriteOnce(this._Parent==null,null,this,nameof(Parent),"{B673604D-F920-4C88-80C7-416EDC0EB027}");
				this._Parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{DE125E06-06F1-4A38-93A0-216E3ED97CE0}");
				EventManager.Raise<EventHandler,EventArgs>(_LazyWeakEventStore,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ParentChanged;

		/// <summary> Gets the event source for the event which occurs when the <see cref="IParentSupport.Parent"/> property has been changed.
		/// </summary>
		/// <value>The event source.</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); } }

		#endregion

		#region Implementation of IDataProvider

		/// <summary> Occurs when <see cref="Data"/>-property has been changed.
		/// </summary>
		[Obsolete("Use DataChangedEvent")]
		public event EventHandler<DataChangedEventArgs> DataChanged;

		/// <summary> Gets the event source for the event which occurs when the <see cref="Data"/> property has been changed.
		/// </summary>
		/// <value>The event source.</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler<DataChangedEventArgs>> DataChangedEvent { get { return EventSources.Get<EventHandler<DataChangedEventArgs>>("DataChangedEvent"); } }

		/// <summary> Gets or sets the data validation callback.
		/// </summary>
		/// <value>The data validation callback.</value>
		public DataValidatingCallbackHandler DataValidatingCallback{get;set;}

		/// <summary> Gets or sets the data.
		/// </summary>
		/// <value>The provided data.</value>
		public abstract object Data{get;set;}

		/// <summary> Raises the <see cref="DataChanged"/>-event
		/// </summary>
		/// <param name="previousData">The previous data.</param>
		/// <param name="newData">The new data.</param>
		protected void OnDataChanged(object previousData, object newData) {
			EventUtil.Raise(DataChanged,this,new DataChangedEventArgs(previousData,newData),"{C1098BD7-DB9F-472F-87FE-15AD7538E886}");
			EventManager.Raise<EventHandler<DataChangedEventArgs>,DataChangedEventArgs>(_LazyWeakEventStore, "DataChangedEvent", new DataChangedEventArgs(previousData,newData));
		}

		/// <summary> Tries to get data.
		/// </summary>
		/// <param name="exception">The exception if failed or null</param>
		/// <returns>The data.</returns>
		public virtual object TryGetData(out Exception exception) {
			try {
				exception = null;
				return Data;
			} catch (Exception ex) {
				exception = ex;
				return null;
			}
		}

		/// <summary> Validates the specified data.
		/// </summary>
		/// <param name="data">The data to be validated.</param>
		/// <returns><see langword="true"/> if the data is valid; else <see langword="false"/>
		/// </returns>
		public virtual Exception Validate(object data) {
			if(DataValidatingCallback!=null) {
				var result = DataValidatingCallback(this, data);
				if(result!=null) return result;
			}
			return null;
		}

		/// <summary> Notifies that the data has been changed
		/// </summary>
		/// <remarks>
		/// Call this method if the data does not implement <see cref="INotifyCollectionChanged"/>
		/// </remarks>
		public void NotifyDataChanged() { 
			object newData=Data;
			if(Equals(newData,PreviousData)) {
				if(newData is IEnumerable) {/* force AtDataChanged*/} 
				else {return;}
			}
			OnDataChanged(PreviousData, newData);
			PreviousData = newData;
		}

		/// <summary>
		/// Gets or sets the previous data.
		/// </summary>
		/// <value>The previous data.</value>
		protected object PreviousData{get;set;}

		#endregion

		/// <summary> Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary> Gets the event source for the event which occurs when a property value changes.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent {
			get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{B82994F3-CE86-4C05-8F34-8CED8399CDAC}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(_LazyWeakEventStore,"PropertyChangedEvent", args);
		}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {
			//TODO implement Dispose
		}
	}

	/// <summary> Generic DataProvider base class
	/// </summary>
	public abstract class DataProvider<T>:IDataProvider<T>,IDataProvider {

		private bool _isDisposed;
		private object _parent;
		private bool? _isAutoCreated;
		protected Lazy<EventSourceStore> _LazyWeakEventStore;

		/// <summary> Initializes a new instance of the <see cref="DataProvider{T}" /> class.
		/// </summary>
		protected DataProvider() {
			_LazyWeakEventStore=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		protected EventSourceStore EventStore{get { return _LazyWeakEventStore.Value; }}
		protected Lazy<EventSourceStore> LazyWeakEventStore{get { return _LazyWeakEventStore; }}

		#region Implementation of IProvider

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		public virtual bool IsSupported{get {return false;}}

		/// <summary> Gets or sets the parent of this provider.
		/// </summary>
		/// <value>The parent of this provider.</value>
		public object Parent {
			[CanBeNull]
			get {return _parent;}
			[NotNull]
			set {
				MemberAccessUtil.DemandNotNull(value,"Parent cannot be null!",this,"{28F3510D-FB94-44ED-BE16-B6A16B9F0C91}");
				MemberAccessUtil.DemandWriteOnce(_parent==null,null,this,nameof(Parent),"{65A545AD-0209-4F59-9593-10B97243E955}");
				_parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{05300EF3-E6DA-42C6-9592-D374725E8913}");
				EventManager.Raise<EventHandler,EventArgs>(LazyWeakEventStore,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ParentChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler> ParentChangedEvent { get { return EventStore.Get<EventHandler>("ParentChangedEvent"); } }

		/// <summary> Gets or sets a value indicating whether this instance is auto created.
		/// </summary>
		/// <value> <c>true</c> if this instance is auto created; otherwise, <c>false</c>. </value>
		public bool IsAutoCreated {
			get { return _isAutoCreated==true; }
			set {
				MemberAccessUtil.DemandWriteOnce(!_isAutoCreated.HasValue,"The property can only be written once!",this,nameof(IsAutoCreated),"{8E2584E1-C321-4DD8-98F1-FEDC25B402FB}");
				_isAutoCreated = value;
			}
		}

		public bool IsInUse { get; set; }

		#endregion

		#region Implementation of IDataProvider<T>

		/// <summary> Occurs when the provided data has been changed.
		/// </summary>
		public event EventHandler<DataChangedEventArgs> DataChanged;

		
		/// <summary> Adds a <see cref="DataChanged"/>-event handler. Supports object initializer.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
		public EventHandler<DataChangedEventArgs> DataChangedHandler {set {this.DataChanged += value;}}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler<DataChangedEventArgs>> DataChangedEvent { get { return EventStore.Get<EventHandler<DataChangedEventArgs>>("DataChangedEvent"); } }

		/// <summary> Gets or sets the data validating callback.
		/// </summary>
		/// <value>The data validating callback.</value>
		public DataValidatingCallbackHandler DataValidatingCallback{get;set;}

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public abstract T Data{get;set;}

		public virtual T TryGetData(out Exception exception) {
			try {
				exception = null;
				return Data;
			} catch (Exception ex) {
				exception = ex;
				return default(T);
			}
		}

		/// <summary> Validates the specified data.
		/// </summary>
		/// <param name="data">The data to be validated.</param>
		/// <returns><see langword="null"/> is the data is valid; else <see cref="Exception"/></returns>
		public virtual Exception Validate(T data) {
			if(DataValidatingCallback!=null) {
				var result = DataValidatingCallback(this, data);
				if(result!=null) return result;
			}
			return null;
		}

		/// <summary> Raises the <see cref="DataChanged"/>-event
		/// </summary>
		/// <param name="previousData">The previous data.</param>
		/// <param name="newData">The new data.</param>
		protected virtual void OnDataChanged(object previousData, object newData) {
			var args = new DataChangedEventArgs(previousData, newData);
			EventUtil.Raise(DataChanged,this,args,"{0DACB323-4EE1-49A4-802C-5B9E470E095B}");
			EventManager.Raise<EventHandler<DataChangedEventArgs>,DataChangedEventArgs>(LazyWeakEventStore,"DataChangedEvent", args);
		}

		/// <summary> Notifies that the provided data has been changed
		/// </summary>
		/// <remarks>
		/// Call this method if the provided data does not implement <see cref="INotifyCollectionChanged"/>
		/// </remarks>
		public virtual void NotifyDataChanged() {
			Exception exception;
			T newData = TryGetData(out exception);
			if (exception != null) {
				//TODO 
			}
			if(Equals(newData,PreviousData)) {
				if(!(newData is IEnumerable)) return;
				// else force OnDataChanged 
				//TODO check if content changed (compare list items)
			}
			OnDataChanged(PreviousData, newData);
			PreviousData = newData;	
		}

		#endregion

		#region Implementation of IDataProvider

		object IDataProvider.Data{get{return Data;} set{Data=(T) value;}}
		object IDataProvider.TryGetData(out Exception exception){return TryGetData(out exception);}
		Exception IDataProvider.Validate(object data){ return Validate((T) data); }

		#endregion

		/// <summary>
		/// Gets or sets the previous data.
		/// </summary>
		/// <value>The previous data.</value>
		protected T PreviousData{get;set;}

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent {
			get { return EventStore.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{B854ADA7-993C-44B8-B730-7523336B62B6}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(LazyWeakEventStore,"PropertyChangedEvent", args);
		}

		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak() {DebugUtil.Break();}

		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak(string message) {DebugUtil.Break(message);}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {
			//TODO implement Dispose
			if(_isDisposed)return;_isDisposed=true;
			if (_LazyWeakEventStore.IsValueCreated) _LazyWeakEventStore.Value.Dispose();
			_LazyWeakEventStore = null;
			_parent = null;
		}
	}




	/// <summary></summary>
	/// <param name="sender"></param>
	/// <param name="newData"></param>
	public delegate Exception DataValidatingCallbackHandler(object sender, object newData);

	/// <summary></summary>
	/// <typeparam name="TData">Type of data</typeparam>
	/// <param name="sender"></param>
	/// <param name="newData"></param>
	public delegate Exception DataValidatingCallbackHandler<in TData>(object sender, TData newData);

}
