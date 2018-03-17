using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using KsWare.Presentation.ViewModelFramework.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides metadata for <see cref="ActionVM"/>
	/// </summary>
	/// <remarks></remarks>
	public class ActionMetadata:ViewModelMetadata {

		private IActionProvider _actionProvider;

		/// <summary> Initializes a new instance of the <see cref="ActionMetadata"/> class.
		/// </summary>
		/// <remarks></remarks>
		public ActionMetadata() {

		}

		/// <summary> Creates the default action provider.
		/// </summary>
		/// <returns></returns>
		protected virtual IActionProvider CreateDefaultActionProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ActionMetadataAttribute),true).Cast<ActionMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.ActionProvider != null) {
							var actionProvider = (IActionProvider)Activator.CreateInstance(a.ActionProvider);
							return actionProvider;
						}				        
					}
				}
			}
			return new LocalActionProvider();
		}

		/// <summary> Gets or sets the action provider.
		/// </summary>
		/// <value>The action provider.</value>
		[Bindable(true)]
		public IActionProvider ActionProvider {
			get {
				// lazy initialization
				if(_actionProvider==null) {
					_actionProvider = CreateDefaultActionProvider();
					_actionProvider.Parent = this;
					OnActionProviderChanged(new ValueChangedEventArgs<IActionProvider>(null,_actionProvider));
					OnPropertyChanged("HasActionProvider");
				}
				return _actionProvider;
			}
			set {
				var oldHasActionProvider = HasActionProvider;
				var oldActionProvider = _actionProvider;
				if(value==null) throw new InvalidOperationException("ActionProvider must not be null!");
				if(_actionProvider!=null) DemandPropertySet();
				_actionProvider = value;
				_actionProvider.Parent = this;
				OnActionProviderChanged(new ValueChangedEventArgs<IActionProvider>(oldActionProvider,_actionProvider));
				if(HasActionProvider!=oldHasActionProvider) OnPropertyChanged("HasActionProvider");
			}
		}

		/// <summary> Gets a value indicating whether this instance has an action provider assigned.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has action provider; otherwise, <c>false</c>.
		/// </value>
		public bool HasActionProvider => _actionProvider != null;

		protected virtual void OnActionProviderChanged(ValueChangedEventArgs<IActionProvider> e) {
			//OnPropertyChanged("ActionProvider");
			EventUtil.Raise(ActionProviderChanged,this,e,"{B8F22E4D-0288-40A9-A28E-1D2D4DACA7A1}");
			EventManager.Raise<EventHandler,EventArgs>(LazyWeakEventStore, "ActionProviderChangedEvent",e);
		}

		public event ValueChangedEventHandler<IActionProvider> ActionProviderChanged;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<ValueChangedEventHandler<IActionProvider>> ActionProviderChangedEvent => EventStore.Get<ValueChangedEventHandler<IActionProvider>>("ActionProviderChangedEvent");

		public void ChangeActionProvider(IActionProvider actionProvider) {
			DemandNotNull(actionProvider);
			if(Equals(_actionProvider,actionProvider)) return;

			var oldProvider = _actionProvider;
//			if(oldProvider!=null && oldProvider.Data!=null) throw new InvalidOperationException("DataProvider is in use! ErrorID: {3F43DE4B-737C-4D62-B764-A5B23957D813}");

			_actionProvider = actionProvider;
			_actionProvider.Parent = this;
			OnActionProviderChanged(new ValueChangedEventArgs<IActionProvider>(oldProvider,actionProvider));
			OnPropertyChanged("HasDataProvider");

			if(oldProvider!=null) oldProvider.Dispose();
		}

		#region Overrides of ViewModelMetadata

		protected override IBindingProvider CreateDefaultBindingProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ViewModelMetadataAttribute),true).Cast<ViewModelMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.BindingProvider != null) {
							var provider = (IBindingProvider)Activator.CreateInstance(a.BindingProvider);
							return provider;
						}				        
					}
				}
			}
			return new ActionBindingProvider();
		}

		#endregion
	}

	public class ActionMetadataAttribute : ViewModelMetadataAttribute {

		private Type _actionProvider;

		/// <summary> Gets or sets the type of action provider. 
		/// The action provider must implement <see cref="IActionProvider"/>
		/// </summary>
		/// <value>The type of action provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IActionProvider"/>!</exception>
		public Type ActionProvider {
			get => _actionProvider;
			set {
				if (!typeof (IActionProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement IActionProvider!");
				_actionProvider = value;
			}
		}
	}
}