using System;
using System.ComponentModel;

namespace KsWare.Presentation.ViewModelFramework {

	//
	// this file implements the ISelectable part of ObjectVM
	//
	partial class ObjectVM:ISelectable {

		private bool m_IsSelected;

		/// <summary> Gets or sets a value indicating whether this object is selected.
		/// </summary>
		/// <value><c>true</c> if this object is selected; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
		[Bindable(BindableSupport.Yes,BindingDirection.TwoWay)]
		public bool IsSelected {
			get {
				DebuggerːBreak(DebuggerFlags.Breakpoints.IsSelectedGet);
				return m_IsSelected;
			}
			set {
				DebuggerːBreak(DebuggerFlags.Breakpoints.IsSelectedSet);
				if (m_IsSelected == value) return;
				m_IsSelected = value;
				OnPropertyChanged("IsSelected");
				if (SuppressAnyEvents==0) {
					EventUtil.Raise(IsSelectedChanged,this,EventArgs.Empty,"{800B0C9A-92AA-4FC6-9C25-135AE3197665}");
					EventUtil.WeakEventManager.Raise(IsSelectedChangedEvent, EventArgs.Empty);
				}
			}
		}

		/// <summary> Occurs when the selected state of this object (<see cref="IsSelected"/>) has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler IsSelectedChanged;

		public IWeakEventSource<EventHandler> IsSelectedChangedEvent { get { return WeakEventProperties.Get<EventHandler>("IsSelectedChangedEvent"); } }

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

		IWeakEventSource<EventHandler> IsSelectedChangedEvent { get; }

//		IObservable<bool> IsSelectedObservable { get; }
	}
}
