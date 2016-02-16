using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace KsWare.Presentation.BusinessFramework {

	/*\ Features
	 *  ‾‾‾‾‾‾‾‾
	 * IDisposable
	 * INotifyPropertyChanged
	 * Fields {BackingFieldsStore}
	\*/

	/// <summary>
	/// Slim business object interface
	/// </summary>
	public interface IObjectSlimBM : INotifyPropertyChanged,IDisposable {

		BackingFieldsStore Fields { get; }
// D	event PropertyChangedEventHandler PropertyChanged;
// D	void Dispose();
		IObjectBM Parent { get; set; }
		event EventHandler ParentChanged;
		IEventSource<EventHandler> ParentChangedEvent { get; }

	}

	/// <summary> Slim business object
	/// </summary>
	public class ObjectSlimBM:IObjectSlimBM,IObjectBM {

		// The WeakEventPropertyStore is only created if any body accesses one of the weak event sources (e.g. ParentChangedEvent)

		private static string NotAvailable="Not available in slim objects";
		private static ICollection<IObjectBM> s_EmptyChildrenCollection=new List<IObjectBM>().AsReadOnly();

		private readonly Lazy<BackingFieldsStore> m_LazyFields;
		private IObjectBM m_Parent;
		private EventSourceStore m_EventSourceStore;

		public ObjectSlimBM() {
			m_LazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		public BackingFieldsStore Fields { get { return m_LazyFields.Value; } }

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool explicitDispose) {
			if (explicitDispose) {
				Fields.Dispose();
			}
		}

		[Obsolete("Not available in slim objects",true)]
		public event EventHandler Disposed;

		public IObjectBM Parent {
			get { return m_Parent; }
			set {
				if (Equals(value, m_Parent)) return;
				m_Parent = value;
				OnPropertyChanged("Parent");
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{D8F0EBFB-94B8-4431-A272-4F76D854A600}");

				// raises the event (w/o side effects which would create the store or the event source)
				if(m_EventSourceStore!=null && m_EventSourceStore.Has("ParentChangedEvent"))
					EventManager.Raise<EventHandler,EventArgs>(ParentChangedEvent,EventArgs.Empty);
			}
		}

		private EventSourceStore EventSources {
			get {
				if (m_EventSourceStore == null) 
					m_EventSourceStore = new EventSourceStore(this);
				return m_EventSourceStore;
			}
		}

		public bool IsSlim { get { return true;} }

		public event EventHandler ParentChanged;

		public IEventSource<EventHandler> ParentChangedEvent {get { return EventSources.Get<EventHandler>("ParentChangedEvent"); }}

		[Obsolete("Not available in slim objects. Always returning empty collection.")]
		public ICollection<IObjectBM> Children { get{return s_EmptyChildrenCollection;} }

		[Obsolete("Not available in slim objects",true)]
		public string MemberName { get { return "?"; } set{throw new NotSupportedException(NotAvailable);} }

		[Obsolete("Not available in slim objects",true)]
		public string MemberPath { get { return "?"; } set{throw new NotSupportedException(NotAvailable);} }

		[Obsolete("Not available in slim objects",true)]
		public BusinessMetadata Metadata { get{throw new NotSupportedException(NotAvailable);} set { throw new NotSupportedException(NotAvailable); } }
		
		[Obsolete("Not available in slim objects",true)]
		public event EventHandler<UserFeedbackEventArgs> UserFeedbackRequested;

		[Obsolete("Not available in slim objects",true)]
		public IEventSource<EventHandler<UserFeedbackEventArgs>> UserFeedbackRequestedEvent { get { throw new NotImplementedException();} }
			
		[Obsolete("Not available in slim objects",true)]
		public event EventHandler<BusinessPropertyChangedEventArgs> BusinessPropertyChanged;
		
		[Obsolete("Not available in slim objects",true)]
		public event EventHandler<TreeChangedEventArgs> TreeChanged;
		
		[Obsolete("Not available in slim objects",true)]
		public void RequestUserFeedback(UserFeedbackEventArgs args) { throw new NotSupportedException(NotAvailable); }
		
		[Obsolete("Not available in slim objects. Always returning true.")]
		public bool IsApplicable { get{return true;} }
		
		[Obsolete("Not available in slim objects",true)]
		public event EventHandler IsApplicableChanged;

	}

}