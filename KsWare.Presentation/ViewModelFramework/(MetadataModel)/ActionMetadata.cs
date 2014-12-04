using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using KsWare.Presentation.ViewModelFramework.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides metadata for <see cref="ActionVM"/>
	/// </summary>
	/// <remarks></remarks>
	public class ActionMetadata:ViewModelMetadata {

		private IActionProvider m_ActionProvider;

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
				if(m_ActionProvider==null) {
					m_ActionProvider = CreateDefaultActionProvider();
					m_ActionProvider.Parent = this;
					OnActionProviderChanged();
					OnPropertyChanged("HasActionProvider");
				}
				return m_ActionProvider;
			}
			set {
				if(value==null) throw new InvalidOperationException("ActionProvider cannot be null!");
				if(m_ActionProvider!=null) DemandPropertySet();
				m_ActionProvider = value;
				m_ActionProvider.Parent = this;
				OnActionProviderChanged();
				OnPropertyChanged("HasActionProvider");
			}
		}

		/// <summary> Gets a value indicating whether this instance has an action provider assigned.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has action provider; otherwise, <c>false</c>.
		/// </value>
		public bool HasActionProvider { get { return m_ActionProvider != null; } }

		protected virtual void OnActionProviderChanged() {
			//OnPropertyChanged("ActionProvider");
			EventUtil.Raise(ActionProviderChanged,this,EventArgs.Empty,"{B8F22E4D-0288-40A9-A28E-1D2D4DACA7A1}");
			EventUtil.WeakEventManager.Raise(ActionProviderChangedEvent,EventArgs.Empty);
		}

		public event EventHandler ActionProviderChanged;
		public IWeakEventSource<EventHandler> ActionProviderChangedEvent { get { return WeakEventProperties.Get<EventHandler>("ActionProviderChangedEvent"); }}

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

		private Type m_ActionProvider;

		/// <summary> Gets or sets the type of action provider. 
		/// The action provider must implement <see cref="IActionProvider"/>
		/// </summary>
		/// <value>The type of action provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IActionProvider"/>!</exception>
		public Type ActionProvider {
			get { return m_ActionProvider; }
			set {
				if (!typeof (IActionProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException("value","The type does not implement IActionProvider!");
				m_ActionProvider = value;
			}
		}
	}
}