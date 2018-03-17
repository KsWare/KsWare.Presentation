using System;
using System.ComponentModel;
using System.Diagnostics;
using JetBrains.Annotations;

namespace KsWare.Presentation.Providers {

	public abstract class Provider:IProvider {

		protected Lazy<EventSourceStore> _LazyWeakEventStore;
		private bool? _isAutoCreated;
		private object _parent;

		protected Provider() {
			_LazyWeakEventStore=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		public EventSourceStore EventSources => _LazyWeakEventStore.Value;

		public abstract bool IsSupported { get; }

		/// <summary> Gets or sets a value indicating whether this instance is auto created.
		/// </summary>
		/// <value> <c>true</c> if this instance is auto created; otherwise, <c>false</c>. </value>
		public bool IsAutoCreated {
			get => _isAutoCreated ==true;
			set {
				MemberAccessUtil.DemandWriteOnce(!_isAutoCreated.HasValue,"The property can only be written once!",this,nameof(IsAutoCreated),"{E7279D65-F0FA-42BE-812F-45BA404524C8}");
				_isAutoCreated = value;
			}
		}

		public bool IsInUse { get; set; }

		#region Parent

		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		/// <remarks></remarks>
		public object Parent {
			get => _parent;
			set {
				MemberAccessUtil.DemandNotNull(value,"Parent cannot be null!",this,"{DE9F4789-BD94-46A6-8238-F5988092A30C}");
				MemberAccessUtil.DemandWriteOnce(_parent==null,null,this,nameof(Parent),"{F02EA960-5BBA-412D-A777-766413010026}");
				_parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{92587EBC-9EF5-4000-A6CE-72E4AF8AF010}");
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
		public IEventSource<EventHandler> ParentChangedEvent => EventSources.Get<EventHandler>("ParentChangedEvent");

		#endregion

		#region PropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent => EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent");

		[NotifyPropertyChangedInvocator]
//		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{B410DCA0-779B-4F54-9718-B3651E8E79C7}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(_LazyWeakEventStore,"PropertyChangedEvent", args);
		}

		#endregion

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool explicitDispose) {
			if (explicitDispose) {
				if(_LazyWeakEventStore.IsValueCreated) _LazyWeakEventStore.Value.Dispose();
			}
		}

	}
}
