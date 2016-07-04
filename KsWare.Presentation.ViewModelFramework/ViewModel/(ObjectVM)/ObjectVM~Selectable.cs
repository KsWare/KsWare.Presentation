using System;
using System.ComponentModel;

namespace KsWare.Presentation.ViewModelFramework {

	//
	// this file implements the ISelectable part of ObjectVM
	//
	partial class ObjectVM:ISelectable {

		private bool _isSelected;

		/// <summary> Gets or sets a value indicating whether this object is selected.
		/// </summary>
		/// <value><c>true</c> if this object is selected; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
		[Bindable(BindableSupport.Yes,BindingDirection.TwoWay)]
		public bool IsSelected {
			get {
				DebuggerːBreak(DebuggerFlags.Breakpoints.IsSelectedGet);
				return _isSelected;
			}
			set {
				DebuggerːBreak(DebuggerFlags.Breakpoints.IsSelectedSet);
				if (_isSelected == value) return;
				_isSelected = value;
				OnPropertyChanged("IsSelected");
				if (SuppressAnyEvents==0) {
					EventUtil.Raise(IsSelectedChanged,this,EventArgs.Empty,"{800B0C9A-92AA-4FC6-9C25-135AE3197665}");
					EventManager.Raise<EventHandler,EventArgs>(LazyWeakEventStore, "IsSelectedChangedEvent", EventArgs.Empty);
				}
			}
		}

		/// <summary> Occurs when the selected state of this object (<see cref="IsSelected"/>) has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler IsSelectedChanged;

		public IEventSource<EventHandler> IsSelectedChangedEvent { get { return EventSources.Get<EventHandler>("IsSelectedChangedEvent"); } }

//		IObservable<bool> IsSelectedObservable { get; }
	}

	/// <summary> Provides properties for selectable objects
	/// </summary>
	public interface ISelectable {

		/// <summary> Gets or sets a value indicating whether this object is selected.
		/// </summary>
		/// <value><c>true</c> if this object is selected; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
		bool IsSelected { get;set; }

		event EventHandler IsSelectedChanged;

		IEventSource<EventHandler> IsSelectedChangedEvent { get; }

//		IObservable<bool> IsSelectedObservable { get; }
	}
}
