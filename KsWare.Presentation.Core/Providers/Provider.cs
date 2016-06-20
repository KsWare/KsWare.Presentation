using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace KsWare.Presentation.Providers {

	public abstract class Provider:IProvider {

		protected Lazy<EventSourceStore> m_LazyWeakEventStore;
		private bool? m_IsAutoCreated;
		private object m_Parent;

		protected Provider() {
			m_LazyWeakEventStore=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		public EventSourceStore EventSources{get { return m_LazyWeakEventStore.Value; }}

		public abstract bool IsSupported { get; }

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

		#region Parent

		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		/// <remarks></remarks>
		public object Parent {
			get {return m_Parent;}
			set {
				MemberAccessUtil.DemandNotNull(value,"Parent cannot be null!",this,"{DE9F4789-BD94-46A6-8238-F5988092A30C}");
				MemberAccessUtil.DemandWriteOnce(m_Parent==null,null,this,"Parent","{F02EA960-5BBA-412D-A777-766413010026}");
				m_Parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{92587EBC-9EF5-4000-A6CE-72E4AF8AF010}");
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

		#region PropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent { get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); } }

		[NotifyPropertyChangedInvocator]
//		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{B410DCA0-779B-4F54-9718-B3651E8E79C7}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventStore,"PropertyChangedEvent", args);
		}

		#endregion

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool explicitDispose) {
			if (explicitDispose) {
				if(m_LazyWeakEventStore.IsValueCreated) m_LazyWeakEventStore.Value.Dispose();
			}
		}

	}
}
