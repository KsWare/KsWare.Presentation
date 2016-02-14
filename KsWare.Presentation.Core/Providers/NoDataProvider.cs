using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace KsWare.Presentation.Core.Providers {

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

}