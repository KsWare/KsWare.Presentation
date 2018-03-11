using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IEventSource<EventHandler> ParentChangedEvent { get; }

	}

	/// <summary> Slim business object
	/// </summary>
	public class ObjectSlimBM:IObjectSlimBM,IObjectBM {

		// The WeakEventPropertyStore is only created if any body accesses one of the weak event sources (e.g. ParentChangedEvent)

		private static string NotAvailable="Not available in slim objects";
		private static ICollection<IObjectBM> s_EmptyChildrenCollection=new List<IObjectBM>().AsReadOnly();

		private readonly Lazy<BackingFieldsStore> _lazyFields;
		private IObjectBM _parent;
		private EventSourceStore _eventSourceStore;

		public ObjectSlimBM() {
			_lazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public BackingFieldsStore Fields { get { return _lazyFields.Value; } }
		public BackingFieldsStore FieldsːDebug { get { return _lazyFields.IsValueCreated ? _lazyFields.Value : null; } }

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

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
			get { return _parent; }
			set {
				if (Equals(value, _parent)) return;
				_parent = value;
				OnPropertyChanged(nameof(Parent));
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{D8F0EBFB-94B8-4431-A272-4F76D854A600}");

				// raises the event (w/o side effects which would create the store or the event source)
				if(_eventSourceStore!=null && _eventSourceStore.Has("ParentChangedEvent"))
					EventManager.Raise<EventHandler,EventArgs>(ParentChangedEvent,EventArgs.Empty);
			}
		}

		private EventSourceStore EventSources {
			get {
				if (_eventSourceStore == null) 
					_eventSourceStore = new EventSourceStore(this);
				return _eventSourceStore;
			}
		}

		public bool IsSlim { get { return true;} }

		public event EventHandler ParentChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
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

		[Obsolete("Not available in slim objects",true)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
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