/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : DataProvider.cs
 * OriginalNamespace: KsWare.Presentation.Providers
 * OrigibalAssembly : KsWare.Presentation.Core
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;
using KsWare.Presentation;
using KsWare.Presentation.Providers;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.Core;
using KsWare.JsonFx;

namespace KsWare.Presentation.Core.Providers {


	/// <summary> Interface for data providers
	/// </summary>
	public interface IDataProvider:IProvider  {

		/// <summary> Occurs when <see cref="Data"/>-property has been changed.
		/// </summary>
		event EventHandler<DataChangedEventArgs> DataChanged;

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

		private object m_Parent;
		private bool? m_IsAutoCreated;
		private Lazy<EventSourceStore> m_LazyWeakEventStore;

		protected DataProvider() {
			m_LazyWeakEventStore=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		public EventSourceStore EventSources{get { return m_LazyWeakEventStore.Value; }}

		#region Implementation of IProvider

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		public abstract bool IsSupported{get;}

		/// <summary> Gets or sets a value indicating whether this instance is auto created.
		/// </summary>
		/// <value> <c>true</c> if this instance is auto created; otherwise, <c>false</c>. </value>
		public bool IsAutoCreated {
			get { return m_IsAutoCreated==true; }
			set {
				MemberAccessUtil.DemandWriteOnce(!m_IsAutoCreated.HasValue,"The property can only be written once!",this,"IsAutoCreated","{E7279D65-F0FA-42BE-812F-45BA404524C8}");
				m_IsAutoCreated = value;
			}
		}

		public bool IsInUse { get; set; }

		/// <summary> Gets or sets the parent of this provider.
		/// </summary>
		/// <value>The parent of this provider.</value>
		public object Parent {
			[CanBeNull]
			get {return m_Parent;}
			[NotNull]
			set {
				if(value==null) throw new InvalidOperationException("Parent cannot be null!");
				MemberAccessUtil.DemandWriteOnce(this.m_Parent==null,null,this,"Parent","{B673604D-F920-4C88-80C7-416EDC0EB027}");
				this.m_Parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{DE125E06-06F1-4A38-93A0-216E3ED97CE0}");
				EventManager.Raise<EventHandler,EventArgs>(m_LazyWeakEventStore,"ParentChangedEvent", EventArgs.Empty);
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

		#endregion

		#region Implementation of IDataProvider

		/// <summary> Occurs when <see cref="Data"/>-property has been changed.
		/// </summary>
		[Obsolete("Use DataChangedEvent")]
		public event EventHandler<DataChangedEventArgs> DataChanged;

		/// <summary> Gets the event source for the event which occurs when the <see cref="Data"/> property has been changed.
		/// </summary>
		/// <value>The event source.</value>
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
			EventManager.Raise<EventHandler<DataChangedEventArgs>,DataChangedEventArgs>(m_LazyWeakEventStore, "DataChangedEvent", new DataChangedEventArgs(previousData,newData));
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
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent {
			get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{B82994F3-CE86-4C05-8F34-8CED8399CDAC}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventStore,"PropertyChangedEvent", args);
		}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {
			//TODO implement Dispose
		}
	}

	/// <summary> Generic DataProvider base class
	/// </summary>
	public abstract class DataProvider<T>:IDataProvider<T>,IDataProvider {

		private bool m_IsDisposed;
		private object m_Parent;
		private bool? m_IsAutoCreated;
		protected Lazy<EventSourceStore> m_LazyWeakEventStore;

		/// <summary> Initializes a new instance of the <see cref="DataProvider{T}" /> class.
		/// </summary>
		protected DataProvider() {
			m_LazyWeakEventStore=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		protected EventSourceStore EventStore{get { return m_LazyWeakEventStore.Value; }}
		protected Lazy<EventSourceStore> LazyWeakEventStore{get { return m_LazyWeakEventStore; }}

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
			get {return m_Parent;}
			[NotNull]
			set {
				MemberAccessUtil.DemandNotNull(value,"Parent cannot be null!",this,"{28F3510D-FB94-44ED-BE16-B6A16B9F0C91}");
				MemberAccessUtil.DemandWriteOnce(m_Parent==null,null,this,"Parent","{65A545AD-0209-4F59-9593-10B97243E955}");
				m_Parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{05300EF3-E6DA-42C6-9592-D374725E8913}");
				EventManager.Raise<EventHandler,EventArgs>(LazyWeakEventStore,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ParentChanged;

		public IEventSource<EventHandler> ParentChangedEvent { get { return EventStore.Get<EventHandler>("ParentChangedEvent"); } }

		/// <summary> Gets or sets a value indicating whether this instance is auto created.
		/// </summary>
		/// <value> <c>true</c> if this instance is auto created; otherwise, <c>false</c>. </value>
		public bool IsAutoCreated {
			get { return m_IsAutoCreated==true; }
			set {
				MemberAccessUtil.DemandWriteOnce(!m_IsAutoCreated.HasValue,"The property can only be written once!",this,"IsAutoCreated","{8E2584E1-C321-4DD8-98F1-FEDC25B402FB}");
				m_IsAutoCreated = value;
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
			if(m_IsDisposed)return;m_IsDisposed=true;
			if (m_LazyWeakEventStore.IsValueCreated) m_LazyWeakEventStore.Value.Dispose();
			m_LazyWeakEventStore = null;
			m_Parent = null;
		}
	}

	public interface ILocalDataProvider {
		
	}

	/// <summary> Provides local data
	/// </summary>
	public class LocalDataProvider : DataProvider,ILocalDataProvider {

		private object m_Data;

		/// <summary> Initializes a new instance of the <see cref="LocalDataProvider"/> class.
		/// </summary>
		public LocalDataProvider() {}

		#region IViewModelProvider

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value>	<see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		public override bool IsSupported {get {return true;}}

		#endregion

		#region Implementation of IDataProvider

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override object Data {
			get {
				return m_Data;
			}
			set {
				if(Equals(value,PreviousData)) return;
				Validate(value);
				m_Data=value;
				OnDataChanged(PreviousData,m_Data);
				PreviousData = value;
			}
		}

		#endregion

	}

	/// <summary> Provides local data
	/// </summary>
	/// <typeparam name="TData">Type of data</typeparam>
	public class LocalDataProvider<TData>:DataProvider<TData>,IDataProvider,ILocalDataProvider {

		private TData m_Data;

		/// <summary> Initializes a new instance of the <see cref="LocalDataProvider{TData}"/> class.
		/// </summary>
		public LocalDataProvider() {}

		#region IViewModelProvider

		/// <summary> Gets a value indicating whether this instance is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		public override bool IsSupported {get {return true;}}

		#endregion

		#region Implementation of IDataProvider<T>

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override TData Data {
			get {return m_Data;}
			set {
				if (Equals(PreviousData, value)) return;
				Validate(value);
				m_Data = value;
				OnDataChanged(PreviousData, m_Data);
				PreviousData = m_Data;
			}
		}

		#endregion

		#region Implementation of IDataProvider
		
		Exception IDataProvider.Validate(object data) { return Validate((TData) data); }

		#endregion
	}

	public interface ICustomDataProvider:IDataProvider {
		
	}

	/// <summary> Provides custom data
	/// </summary>
	public class CustomDataProvider:DataProvider,ICustomDataProvider {

		private readonly Func<object> m_GetValue;
		private readonly Action<object> m_SetValue;

		/// <summary> Initializes a new instance of the <see cref="CustomDataProvider"/> class.
		/// </summary>
		/// <param name="getValue">The getter.</param>
		/// <param name="setValue">The setter.</param>
		public CustomDataProvider(Func<object> getValue, Action<object> setValue) {
//			if (getValue == null) throw new ArgumentNullException("getValue");
//			if (setValue == null) throw new ArgumentNullException("setValue");

			m_GetValue = getValue;
			m_SetValue = setValue;
		}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>. </value>
		public override bool IsSupported {get {return true;}}

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override object Data {
			get {
				if(m_GetValue==null) throw new InvalidOperationException("Data has not getter!");
				return m_GetValue();
			}
			set {
				if(m_SetValue==null) throw new InvalidOperationException("Data has not setter!");
				if(Equals(value,PreviousData)) return;
				Validate(value);
				m_SetValue(value);
				//NotifyDataChanged();
				OnDataChanged(PreviousData, value);
				PreviousData = value;
			}
		}
	}

	/// <summary> Provides custom data
	/// </summary>
	/// <typeparam name="TData">Type of data</typeparam>
	public class CustomDataProvider<TData> : DataProvider<TData>, ICustomDataProvider {

		private readonly Func<TData> m_GetValue;
		private readonly Action<TData> m_SetValue;

		/// <summary> Initializes a new instance of the <see cref="CustomDataProvider&lt;TData&gt;"/> class.
		/// </summary>
		/// <param name="getValueFunc">The getter.</param>
		/// <param name="setValueFunc">The setter.</param>
		public CustomDataProvider(Func<TData> getValueFunc, Action<TData> setValueFunc){
//			if (getValueFunc == null) throw new ArgumentNullException("getValueFunc");
//			if (setValueFunc == null) throw new ArgumentNullException("setValueFunc");

			m_GetValue = getValueFunc;
			m_SetValue = setValueFunc;
		}

		public override bool IsSupported {get {	return true;}}

		protected IObjectVM ParentVM {
			get {
				var metadata = Parent as IParentSupport; if( metadata==null) return null;
				var vm = metadata.Parent as IObjectVM;
				return vm;
			}
		}

		/// <summary>  Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override TData Data {
			get {
				if(m_GetValue==null) throw new NotSupportedException("Property get method not defined! UniqueID: {6708C25A-A2CF-4C60-98C5-C129A1FABD19}");
				//possible NullReferenceException, is Data is null!
				//ex.: DataProvider = new CustomDataProvider<String>(() => this.Data.Author, value => this.Data.Author = value)
				return m_GetValue();
			}
			set {
				if(m_SetValue==null) throw new NotSupportedException("Property set method not defined! UniqueID: {D880F93C-25D5-4170-B803-415A38013118}");
				if(Equals(value,PreviousData)) return;
				Validate(value);
				m_SetValue(value);
				OnDataChanged(PreviousData, value);
				PreviousData = value;
			}
		}

		#region Implementation of ICustomDataProvider

		#endregion

	}

	/// <summary> [EXPERIMENTAL] Provides data using reflection
	/// </summary>
	/// <typeparam name="TData">Type of data</typeparam>
	/// <remarks>
	/// Draft Limitation:<br/>
	/// * path must be a name of a property
	/// </remarks>
	public class ReflectionDataProvider<TData> : DataProvider<TData>, IDataProvider {

		private readonly Func<object> m_GetObject;
		private readonly string m_Path;
		private readonly object m_Obj;

			/// <summary> Initializes a new instance of the <see cref="ReflectionDataProvider{TData}"/> class.
		/// </summary>
		/// <param name="getObjectFunc">The getter.</param>
		/// <param name="path">The setter.</param>
		public ReflectionDataProvider(Func<object> getObjectFunc, string path){
			if (getObjectFunc == null) throw new ArgumentNullException("getObjectFunc");
			if (path == null) throw new ArgumentNullException("path");

			m_GetObject = getObjectFunc;
			m_Path      = path;
		}

		/// <summary> Initializes a new instance of the <see cref="ReflectionDataProvider{TData}"/> class.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="path">The path.</param>
		public ReflectionDataProvider(object obj, string path){
			if (obj == null) throw new ArgumentNullException("obj");
			if (path == null) throw new ArgumentNullException("path");

			m_Obj       = obj;
			m_GetObject = () => m_Obj;
			m_Path      = path;
		}

		/// <summary> Initializes a new instance of the <see cref="ReflectionDataProvider{TData}"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		public ReflectionDataProvider(string path){
			if (path == null) throw new ArgumentNullException("path");

			m_GetObject = () => ((IParentSupport)this.Parent).Parent; // we assume ValueVM.Metadata.Dataprovider structure
			m_Path      = path;
		}
		public override bool IsSupported {get {	return true;}		}

		/// <summary>  Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override TData Data {
			get {
				var obj=this.m_GetObject();
				if(obj==null) throw new NullReferenceException("{14942C32-C47A-48A9-A8D9-FA455244F2B4}");//TODO
//				var t = obj.GetType();
//				var prop = t.GetProperty(m_Path);
//				var value = prop.GetValue(obj, null);
				
				var value=new Json(obj, false, true)[m_Path].NativeValue;

				return (TData)value;
			}
			set {
				if(Equals(value,PreviousData)) return;
				Validate(value);

				var obj=this.m_GetObject();
//				var t = obj.GetType();
//				var prop = t.GetProperty(m_Path);
//				prop.SetValue(obj,value,null);

				new Json(obj, false, false)[m_Path] = new Json(value,false,true);

				OnDataChanged(PreviousData, value);
				PreviousData = value;
			}
		} 

		#region Implementation of ICustomDataProvider

		#endregion

	}

	/// <summary> Provides no data
	/// </summary>
	public sealed class NoDataProvider:IDataProvider {

		private object m_Parent;
		private bool? m_IsAutoCreated;
		private Lazy<EventSourceStore> m_LazyWeakEventProperties;

		public NoDataProvider() {
			m_LazyWeakEventProperties=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}
		public EventSourceStore EventSources{get { return m_LazyWeakEventProperties.Value; }}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>. </value>
		public bool IsSupported {get {return false;}}

		/// <summary> Gets or sets the parent of this provider.
		/// </summary>
		/// <value>The parent of this provider.</value>
		public object Parent {
			[CanBeNull]
			get {return m_Parent;}
			[NotNull]
			set {
				MemberAccessUtil.DemandNotNull(value,"Property cannot be null!",this,"{8D3472D8-B6FD-45BE-A63F-79FA56577E24}");
				MemberAccessUtil.DemandWriteOnce(this.m_Parent==null,null,this,"Parent","{734D4ADC-52CF-4ED5-AA58-5274F4E66911}");
				m_Parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{C86F40A2-9A2A-46F2-9005-2C5FD5140823}");
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

		/// <summary> Gets or sets a value indicating whether this instance is auto created.
		/// </summary>
		/// <value> <c>true</c> if this instance is auto created; otherwise, <c>false</c>. </value>
		public bool IsAutoCreated {
			get { return m_IsAutoCreated==true; }
			set {
				MemberAccessUtil.DemandWriteOnce(!m_IsAutoCreated.HasValue,"The property can only be written once!",this,"IsAutoCreated","{8E2584E1-C321-4DD8-98F1-FEDC25B402FB}");
				m_IsAutoCreated = value;
			}
		}

		public bool IsInUse { get; set; }

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		event EventHandler<DataChangedEventArgs> IDataProvider.DataChanged {add {}remove {}}

		/// <summary> Gets the event source for the event which occurs when the <see cref="Data"/> property has been changed.
		/// </summary>
		/// <value>The event source.</value>
		public IEventSource<EventHandler<DataChangedEventArgs>> DataChangedEvent { get { return EventSources.Get<EventHandler<DataChangedEventArgs>>("DataChangedEvent"); } }

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		DataValidatingCallbackHandler IDataProvider.DataValidatingCallback {get {throw new NotSupportedException();}set {throw new NotSupportedException();}}
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object IDataProvider.Data {get {throw new NotSupportedException();}set {throw new NotSupportedException();}}
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object IDataProvider.TryGetData(out Exception exception) {exception = new NotSupportedException();return null;}

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		Exception IDataProvider.Validate(object data) { throw new NotSupportedException(); }
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void IDataProvider.NotifyDataChanged() { }

		/// <summary> Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary> Get the event source for the event which occurs when a property value changes.
		/// </summary>
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent {
			get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{A0C86ABF-E7AF-427C-AFFA-3FF446E2F6C0}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventProperties,"PropertyChangedEvent", args);
		}

		public void Dispose() { }
	}


	/// <summary> Provides arguments for the DataChanged event
	/// </summary>
	public class DataChangedEventArgs: EventArgs {

		private readonly object m_PreviousData;
		private readonly object m_NewData;

		/// <summary> Initializes a new instance of the <see cref="DataChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousData">The previous data if unknown</param>
		/// <param name="newData">The new data.</param>
		public DataChangedEventArgs(object previousData, object newData) {
			m_PreviousData = previousData;
			m_NewData = newData;
		}

		/// <summary> Gets the previous data or null if previous data is unknown
		/// </summary>
		/// <value>The previous data or null.</value>
		public object PreviousData {get {return m_PreviousData;}}

		/// <summary> Gets the new data.
		/// </summary>
		/// <value>The new data.</value>
		public object NewData {get {return m_NewData;}}

		public string Cause { get; set; }
	}

	/// <summary> Provides arguments for the DataChanged event
	/// </summary>
	public class DataChangedEventArgs<TData>: EventArgs {

		private readonly TData m_PreviousData;
		private readonly TData m_NewData;

		/// <summary> Initializes a new instance of the <see cref="DataChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousData">The previous data if unknown</param>
		/// <param name="newData">The new data.</param>
		public DataChangedEventArgs(TData previousData, TData newData) {
			m_PreviousData = previousData;
			m_NewData = newData;
		}

		/// <summary> Gets the previous data or null if previous data is unknown
		/// </summary>
		/// <value>The previous data or null.</value>
		public TData PreviousData {get {return m_PreviousData;}}

		/// <summary> Gets the new data.
		/// </summary>
		/// <value>The new data.</value>
		public TData NewData {get {return m_NewData;}}

		public string Cause { get; set; }
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
