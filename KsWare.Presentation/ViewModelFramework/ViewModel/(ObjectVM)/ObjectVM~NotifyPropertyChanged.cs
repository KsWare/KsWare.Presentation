using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core;

namespace KsWare.Presentation.ViewModelFramework {
	//
	// Implements "NotifyPropertyChanged" functionality for ObjectVM
	//  

	public partial class ObjectVM: INotifyPropertyChanged {

		private void InitPartNotifyPropertyChanged() {
			
		}

		private PropertyChangedEventHandler m_INotifyPropertyChangedPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {add{m_INotifyPropertyChangedPropertyChanged+=value;}remove {m_INotifyPropertyChangedPropertyChanged-=value;}}
		private List<PropertyChangedHandlerEntry> m_PropertyChangedEventHandlersByName = new List<PropertyChangedHandlerEntry>(); 

		/// <summary> Occurs when a property has been changed.
		/// </summary>
		public event EventHandler<ViewModelPropertyChangedEventArgs> PropertyChanged;

		public IEventSource<EventHandler<ViewModelPropertyChangedEventArgs>> PropertyChangedEvent {
			get { return EventSources.Get<EventHandler<ViewModelPropertyChangedEventArgs>>("PropertyChangedEvent"); }
		}

		/// <summary> Called when a property has been changed.
		/// </summary>
		/// <param name="propertyName">Name of the changed property.</param>
		/// <remarks></remarks>
		protected virtual void OnPropertyChanged([LocalizationRequired(false)] string propertyName) {
			OnPropertyChangedInternal(null, propertyName);
		}

		protected void OnPropertyChanged(Expression<Func<object,object>> propertyExpression ) {
			var name = MemberNameUtil.GetPropertyName(propertyExpression);
			OnPropertyChangedInternal(null, name);
		}

		/// <summary> Called when a property has been changed.
		/// </summary>
		/// <param name="viewModelProperty">The view model property.</param>
		/// <remarks></remarks>
		protected void OnPropertyChanged(ViewModelProperty viewModelProperty) {
			OnPropertyChangedInternal(viewModelProperty, null);
		}

		private void OnPropertyChangedInternal(ViewModelProperty viewModelProperty, string propertyName) { 
			if (SuppressAnyEvents != 0) return;

			//INFO: propertyName may be "Item[]" e.g. for indexer in a list

			if(propertyName!=null) {
				var property = PropertyCache.GetProperty(propertyName, this.GetType(), autoRegister:true);
				viewModelProperty = property;
			}

			//IObjectVM.PropertyChanged
			EventUtil.Raise(PropertyChanged, this, new ViewModelPropertyChangedEventArgs(viewModelProperty), "{68BDD817-933C-4F19-ABEC-E4EB06EDCD98}");

			//INotifyPropertyChanged.PropertyChanged
			//INFO: use propertyName if available because "Item[]"
			EventUtil.Raise(m_INotifyPropertyChangedPropertyChanged, this, new PropertyChangedEventArgs(propertyName ?? viewModelProperty.Name), "{2E8D6B35-3ED2-4E56-A5FA-398AA02FBACB}");
			EventManager.Raise<EventHandler<ViewModelPropertyChangedEventArgs>,ViewModelPropertyChangedEventArgs>(LazyWeakEventStore,"PropertyChangedEvent", new ViewModelPropertyChangedEventArgs(viewModelProperty));

			//PropertyChangedEventHandlers
			foreach (var item in m_PropertyChangedEventHandlers) {
				if (item.Item1 == viewModelProperty) {
					EventUtil.Raise(item.Item2, this, EventArgs.Empty, "{8BADE8AD-6CE3-4AD0-A8F6-02667B42CEA0}");
				}
			}

			//filtered
			foreach (var tuple in m_PropertyChangedEventHandlersByName) {
				if ((propertyName ?? viewModelProperty.Name) == tuple.PropertyName) {
					tuple.EventHandler.Invoke(this,EventArgs.Empty);
				}
			}
		}

		protected void AddPropertyChangedHandler(Expression<Func<object, object>> propertyExpression, EventHandler eventHandler) {
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			var d=new PropertyChangedHandlerEntry(this, memberName, eventHandler);
			m_PropertyChangedEventHandlersByName.Add(d);
		}

		private class PropertyChangedHandlerEntry : IDisposable {
			private readonly WeakReference m_VM;

			public PropertyChangedHandlerEntry(ObjectVM vm, string propertyName, EventHandler eventHandler) {
				m_VM = new WeakReference(vm);
				PropertyName = propertyName;
				EventHandler = eventHandler;
			}

			public EventHandler EventHandler { get; private set; }
			public string PropertyName { get; private set; }

			public void Dispose() {
				EventHandler = null;
				if (m_VM.IsAlive) {
					((ObjectVM) m_VM.Target).m_PropertyChangedEventHandlersByName.Remove(this);
					m_VM.Target = null;
				}
			}
		}
		
	}
}
