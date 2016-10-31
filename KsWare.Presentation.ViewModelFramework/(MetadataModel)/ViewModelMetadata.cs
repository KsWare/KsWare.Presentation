/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : ViewModelMetadata.cs
 * OriginalNamespace: KsWare.Presentation.ViewModelFramework
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	public interface IViewModelMetadata:IMetadata,IParentSupport{}

	/// <summary> Provides metadata for viewmodel objects (xxxVM)
	/// </summary>
	/// <remarks>Use the <see cref="ViewModelMetadataAttribute"/> to configure the creation of ViewModelMetadata for each view model</remarks>
	/// <example>
	/// <code>
	/// 
	/// </code>
	/// </example>
	public class ViewModelMetadata:IViewModelMetadata,INotifyPropertyChanged,IParentSupport {

		private IDataProvider         _dataProvider        ;
		private IErrorProvider        _errorProvider       ;
		private IDisplayValueProvider _displayValueProvider;
		private IEditValueProvider    _editValueProvider   ;
		private IValueSourceProvider  _valueSourceProvider ;
		private IBindingProvider      _bindingProvider     ;
		private IObjectVM             _parent              ;
		private readonly Lazy<EventSourceStore> _lazyWeakEventProperties;
		private bool _enableBusinessModelFeatures;

		/// <summary> Initializes a new instance of the <see cref="ViewModelMetadata"/> class.
		/// </summary>
		public ViewModelMetadata() {
			_lazyWeakEventProperties = new Lazy<EventSourceStore>(() => new EventSourceStore(this));

			//m_dataProvider         = new LocalDataProvider();
			//m_errorProvider        = new ErrorProvider();
			//m_editValueProvider    = new EditValueProvider();
			//m_displayValueProvider = new DisplayValueProvider();
			//m_valueSourceProvider  = new ValueSourceProvider();
		}

		/// <summary>Gets or sets the <see cref="ReflectedPropertyInfo"/> from <see cref="ObjectVM"/>
		/// </summary>
		internal ReflectedPropertyInfo Reflection { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected EventSourceStore EventStore { get { return _lazyWeakEventProperties.Value; }}
		protected Lazy<EventSourceStore> LazyWeakEventStore { get { return _lazyWeakEventProperties; }}

		/// <summary> Gets the view model object which holds this metadata.
		/// </summary>
		/// <value>The view model object which holds this metadata or null if not assigned.</value>
		/// <remarks>
		/// After setting <see cref="Parent"/> to value other as null all meta data properties will be read-only.
		/// <blockquote><b>Note: </b> Only the parent itself should set this property.</blockquote>
		/// </remarks>
		public IObjectVM Parent {
			[CanBeNull]
			get{return _parent;}
			[NotNull] 
			set {
				MemberAccessUtil.DemandNotNull(value,"Property can not be null!",this,"Parent","{0317A98F-B4CD-4E89-AE9C-0A8A1ABBC196}");
				MemberAccessUtil.DemandWriteOnce(_parent==null,"Property can only set one time!",this,nameof(Parent),"{968E415C-F520-4267-BC27-303EE01A5591}");
				_parent = value;
				OnParentChanged();
				EndInitalize();
			}
		}

		object IParentSupport.Parent { get { return Parent; } set { Parent = (IObjectVM) value; } }

		/// <summary> Occurs when <see cref="Parent"/> property has been changed.
		/// </summary>
		public event EventHandler ParentChanged;

		public IEventSource<EventHandler>  ParentChangedEvent  { get { return EventStore.Get<EventHandler>("ParentChangedEvent"); } }

		/// <summary> Called when <see cref="Parent"/>-property has been changed.
		/// This indicates metadata has been assigned to an view model object and all metadata properties are now read-only.
		/// Raises the <see cref="ParentChanged"/>-event.
		/// </summary>
		protected virtual void OnParentChanged() {
			EventUtil.Raise(ParentChanged, this, EventArgs.Empty, "{F65518B0-4D15-4F8C-AE33-DC789AFA3AAB}");
			EventManager.Raise<EventHandler,EventArgs>(_lazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);

//			if (Parent != null) {
//				Parent.ParentChangedEvent.RegisterChild(this, "{8F72EC5D-93B3-426D-841B-BF6B54BC6F79}", delegate(object sender, EventArgs args) {
//					OnModelParentChanged();
//				});
//			}
		}

		public bool EnableBusinessModelFeatures {
			get { return _enableBusinessModelFeatures; }
			set {
				MemberAccessUtil.DemandWriteOnce(_enableBusinessModelFeatures==false,null,this,nameof(EnableBusinessModelFeatures),"{6BAD9F66-E6DF-4556-B4CF-3A7D91DDD666}");
				_enableBusinessModelFeatures = value;
			}
		}


		/// <summary> Called after <see cref="Parent"/> has been specified.
		/// </summary>
		/// <remarks></remarks>
		[SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "provider")]
		protected virtual void EndInitalize() {
			//if (state != ComponentState.Initializing) return;

//			IProvider provider;
//			provider = DataProvider;
//			provider = ErrorProvider;
//			provider = DisplayValueProvider;
//			provider = EditValueProvider;
//			provider = ValueSourceProvider;
		}

		#region DataProvider

		/// <summary> Gets a value indicating whether this instance has a data provider assigned.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has data provider assigned; otherwise, <c>false</c>.
		/// </value>
		public bool HasDataProvider {get { return _dataProvider != null; }}

		/// <summary> Gets or sets the data provider.
		/// </summary>
		/// <value>The data provider.</value>
		public IDataProvider DataProvider {
			[NotNull] 
			get {
				if(_dataProvider==null) {
					_dataProvider = CreateDefaultDataProvider();
					_dataProvider.IsAutoCreated = true;
					_dataProvider.Parent = this;
					OnDataProviderChanged(new ValueChangedEventArgs<IDataProvider>(null,_dataProvider));
					OnPropertyChanged("HasDataProvider");
				}
				return _dataProvider;
			}
			[NotNull] 
			set {
				DemandNotNull(value);
				DemandPropertySet();
				if(Equals(_dataProvider,value)) return;
				
				var oldProvider = _dataProvider;
				_dataProvider = value;
				_dataProvider.Parent = this;
				OnDataProviderChanged(new ValueChangedEventArgs<IDataProvider>(oldProvider,_dataProvider));
				OnPropertyChanged("HasDataProvider");
			}
		}

		public void ChangeDataProvider(IDataProvider dataProvider) {
			DemandNotNull(dataProvider);
			if(Equals(_dataProvider,dataProvider)) return;

			var oldProvider = _dataProvider;
			if(oldProvider!=null && oldProvider.Data!=null) throw new InvalidOperationException("DataProvider is in use! ErrorID: {3F43DE4B-737C-4D62-B764-A5B23957D813}");

			_dataProvider = dataProvider;
			_dataProvider.Parent = this;
			OnDataProviderChanged(new ValueChangedEventArgs<IDataProvider>(oldProvider,dataProvider));
			OnPropertyChanged("HasDataProvider");

			if(oldProvider!=null) oldProvider.Dispose();
		}

		/// <summary> Called when <see cref="DataProvider"/> has been changed.
		/// </summary>
		protected virtual void OnDataProviderChanged(ValueChangedEventArgs<IDataProvider> e) { 
			//OnPropertyChanged("DataProvider");
			EventUtil.Raise(DataProviderChanged,this,e,"{869F3686-8B2C-4AFE-86BF-93AD6D99AA29}");
			EventManager.Raise<EventHandler<ValueChangedEventArgs<IDataProvider>>,ValueChangedEventArgs<IDataProvider>>(_lazyWeakEventProperties,"DataProviderChangedEvent", e);
		}

		[Obsolete("Use DataProviderChangedEvent")]
		public event EventHandler<ValueChangedEventArgs<IDataProvider>>  DataProviderChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler<ValueChangedEventArgs<IDataProvider>>> DataProviderChangedEvent {
			get { return EventStore.Get<EventHandler<ValueChangedEventArgs<IDataProvider>>>(nameof(DataProviderChangedEvent)); }
		}

		/// <summary> Creates the default data provider.
		/// </summary>
		/// <returns></returns>
		protected virtual IDataProvider CreateDefaultDataProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ViewModelMetadataAttribute),true).Cast<ViewModelMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.DataProvider != null) {
							var provider = (IDataProvider)Activator.CreateInstance(a.DataProvider);
							return provider;
						}				        
					}
				}
			}
			return new LocalDataProvider();
		}

		#endregion

		#region ErrorProvider

		/// <summary> Gets or sets the error provider.
		/// </summary>
		/// <value>The error provider.</value>
		public IErrorProvider ErrorProvider {
			get {
				if(_errorProvider==null) {
					_errorProvider = CreateDefaultErrorProvider();
//					_ErrorProvider.IsAutoCreated = true;
					_errorProvider.Parent = this;
				}
				return _errorProvider;
			}
			set {
				DemandNotNull(value);
				DemandPropertySet();
				_errorProvider = value;
				_errorProvider.Parent = this;
			}
		}

		/// <summary> Creates the default error provider.
		/// </summary>
		/// <returns></returns>
		protected virtual IErrorProvider CreateDefaultErrorProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ViewModelMetadataAttribute),true).Cast<ViewModelMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.ErrorProvider != null) {
							var provider = (IErrorProvider)Activator.CreateInstance(a.ErrorProvider);
							return provider;
						}				        
					}
				}
			}
			return new ErrorProvider();
		}

		///// <summary> Raises the PropertyChanged event for ErrorProvider
		///// </summary>
		//protected virtual void OnErrorProviderChanged() { 
		//    OnPropertyChanged("ErrorProvider");
		//}

		#endregion

		#region DisplayValueProvider
		/// <summary> Gets or sets the display value provider.
		/// </summary>
		/// <value>The display value provider.</value>
		public IDisplayValueProvider DisplayValueProvider {
			get {
				if (_displayValueProvider == null) {
					_displayValueProvider = CreateDefaultDisplayValueProvider();
//					_DisplayValueProvider.IsAutoCreated = true;
					_displayValueProvider.Parent = this;
				}
				return _displayValueProvider;
			}
			set {
				DemandNotNull(value);
				DemandPropertySet();
				_displayValueProvider = value;
				_displayValueProvider.Parent = this;
			}
		}

		///// <summary>
		///// Called when [display value provider changed].
		///// </summary>
		//protected virtual void OnDisplayValueProviderChanged() { 
		//    OnPropertyChanged("DisplayValueProvider");
		//}

		/// <summary> Creates the default display value provider.
		/// </summary>
		/// <returns></returns>
		protected virtual IDisplayValueProvider CreateDefaultDisplayValueProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ViewModelMetadataAttribute),true).Cast<ViewModelMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.DisplayValueProvider != null) {
							var provider = (IDisplayValueProvider)Activator.CreateInstance(a.DisplayValueProvider);
							return provider;
						}				        
					}
				}
			}
			return new DisplayValueProvider();
		}

		#endregion

		#region EditValueProvider

		/// <summary> Gets or sets the edit value provider.
		/// </summary>
		/// <value>The edit value provider.</value>
		/// <remarks></remarks>
		public IEditValueProvider EditValueProvider {
			get {
				if(_editValueProvider==null) {
					_editValueProvider = CreateDefaultEditValueProvider();
//					_EditValueProvider.IsAutoCreated = true;
					_editValueProvider.Parent = this;
				}
				return _editValueProvider;
			}
			set {
				DemandNotNull(value);
				DemandPropertySet();
				_editValueProvider = value;
				_editValueProvider.Parent = this;
			}
		}

		/// <summary> Creates the default edit value provider.
		/// </summary>
		/// <returns></returns>
		protected virtual IEditValueProvider CreateDefaultEditValueProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ViewModelMetadataAttribute),true).Cast<ViewModelMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.EditValueProvider != null) {
							var provider = (IEditValueProvider)Activator.CreateInstance(a.EditValueProvider);
							return provider;
						}				        
					}
				}
			}
			return new EditValueProvider();
		}

		///// <summary> Raises the PropertyChanged event for EditValueProvider
		///// </summary>
		//protected virtual void OnEditValueProviderChanged() { 
		//    OnPropertyChanged("EditValueProvider");
		//}

		#endregion
		
		#region ValueSourceProvider

		/// <summary> Gets or sets the value source provider.
		/// </summary>
		/// <value>The value source provider.</value>
		/// <remarks></remarks>
		public IValueSourceProvider ValueSourceProvider {
			get {
				if(_valueSourceProvider==null) {
					_valueSourceProvider = CreateDefaultValueSourceProvider();
//					_ValueSourceProvider.IsAutoCreated = true;
					_valueSourceProvider.Parent = this;
				}
				return _valueSourceProvider;
			}
			set {
				DemandNotNull(value);
				DemandPropertySet();
				_valueSourceProvider = value;
				_valueSourceProvider.Parent = this;
			}
		}

		/// <summary> Creates the default edit value provider.
		/// </summary>
		/// <returns></returns>
		protected virtual IValueSourceProvider CreateDefaultValueSourceProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ViewModelMetadataAttribute),true).Cast<ViewModelMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.ValueSourceProvider != null) {
							var provider = (IValueSourceProvider)Activator.CreateInstance(a.ValueSourceProvider);
							return provider;
						}				        
					}
				}
			}
			return new ValueSourceProvider();
		}

		///// <summary> Raises the PropertyChanged event for ValueSourceProvider
		///// </summary>
		//protected virtual void OnValueSourceProviderChanged() { 
		//    OnPropertyChanged("ValueSourceProvider");
		//}

		#endregion

		#region LocalizationProvider

		public ILocalizationProvider LocalizationProvider { get; set; }

		#endregion

		#region BindingProvider

		public IBindingProvider BindingProvider {
			get {
				if(_bindingProvider==null) {
					_bindingProvider = CreateDefaultBindingProvider();
//					_BindingProvider.IsAutoCreated = true;
					_bindingProvider.Parent = this;
				}
				return _bindingProvider;
			}
			set {
				DemandNotNull(value);
				DemandPropertySet();
				_bindingProvider = value;
				_bindingProvider.Parent = this;
			}
		}

		protected virtual IBindingProvider CreateDefaultBindingProvider() {
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
			return null;
		}

		#endregion

		/// <summary> Demands a write-once operation for a metadata property
		/// </summary>
		/// <remarks></remarks>
		[AssertionMethod]
		protected void DemandPropertySet() { 
			MemberAccessUtil.Demand (
				_parent==null,
				()=>"Cannot set a metadata property once it is applied to a view model object!"+
					DebugUtil.P("MemberPath",()=>_parent.MemberPath),
				this,
				"{92971A8E-4C86-402F-8CDE-FCC15FE60C85}"
			);
		}

		[AssertionMethod]
		protected void DemandNotNull(object value) {MemberAccessUtil.DemandNotNull(value,"Metadata property can not be null!",this,1,"{A8766806-1BB7-4328-9779-15E6E39E619C}");}

		/// <summary> Occurs when a property value changes.
		/// </summary>
		/// <remarks></remarks>
		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<PropertyChangedEventHandler>  PropertyChangedEvent { get { return EventStore.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); } }

		protected void OnPropertyChanged(string propertyName) {
			var ea = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,ea,"{3ED247AA-CBA5-4D2E-B0DE-2BE3CBAF61B7}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(_lazyWeakEventProperties,"PropertyChangedEvent", ea);
		}

	}

	/// <summary> Provides configuration settings for <see cref="ViewModelMetadata"/>
	/// </summary>
	public class ViewModelMetadataAttribute : MetadataAttribute {

		private Type _metadataType;
		private Type _dataProvider;
		private Type _errorProvider;
		private Type _displayValueProvider;
		private Type _editValueProvider;
		private Type _valueSourceProvider;
		private Type _localizationProvider;
		private Type _bindingProvider;

		public ViewModelMetadataAttribute() {}

		public ViewModelMetadataAttribute(Type metadataType) {
			_metadataType = metadataType;
		}

		/// <summary> Gets or sets the type of the metadata.
		/// </summary>
		/// <value>The type of the metadata.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type is not derived from ViewModelMetadata!</exception>
		public Type MetadataType {
			get { return _metadataType; }
			set {
				if (!typeof (ViewModelMetadata).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type is not derived from ViewModelMetadata!");
				_metadataType = value;
			}
		}

		/// <summary> Gets or sets the type of data provider. 
		/// The data provider must implement <see cref="IDataProvider"/>
		/// </summary>
		/// <value>The type of data provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IDataProvider"/>!</exception>
		public Type DataProvider {
			get { return _dataProvider; }
			set {
				if (!typeof (IDataProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement IDataProvider!");
				_dataProvider = value;
			}
		}

		/// <summary> Gets or sets the type of error provider. 
		/// The error provider must implement <see cref="IErrorProvider"/>
		/// </summary>
		/// <value>The type of action provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IErrorProvider"/>!</exception>
		public Type ErrorProvider {
			get { return _errorProvider; }
			set {
				if (!typeof (IErrorProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement IErrorProvider!");
				_errorProvider = value;
			}
		}

		/// <summary> Gets or sets the type of display provider. 
		/// The display provider must implement <see cref="IDisplayValueProvider"/>
		/// </summary>
		/// <value>The type of display provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IDisplayValueProvider"/>!</exception>
		public Type DisplayValueProvider {
			get { return _displayValueProvider; }
			set {
				if (!typeof (IDisplayValueProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement IDisplayValueProvider!");
				_displayValueProvider = value;
			}
		}

		/// <summary> Gets or sets the type of edit provider. 
		/// The edit provider must implement <see cref="IEditValueProvider"/>
		/// </summary>
		/// <value>The type of edit provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IEditValueProvider"/>!</exception>
		public Type EditValueProvider {
			get { return _editValueProvider; }
			set {
				if (!typeof (IEditValueProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement IEditValueProvider!");
				_editValueProvider = value;
			}
		}

		/// <summary> Gets or sets the type of value source provider. 
		/// The value source provider must implement <see cref="IValueSourceProvider"/>
		/// </summary>
		/// <value>The type of value soure provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IValueSourceProvider"/>!</exception>
		public Type ValueSourceProvider {
			get { return _valueSourceProvider; }
			set {
				if (!typeof (IValueSourceProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement IValueSourceProvider!");
				_valueSourceProvider = value;
			}
		}

		/// <summary> Gets or sets the type of localization provider. 
		/// The localization provider must implement <see cref="ILocalizationProvider"/>
		/// </summary>
		/// <value>The type of localization provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="ILocalizationProvider"/>!</exception>
		public Type LocalizationProvider {
			get { return _localizationProvider; }
			set {
				if (!typeof (ILocalizationProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement ILocalizationProvider!");
				_localizationProvider = value;
			}
		}
	
		/// <summary> Gets or sets the type of binding provider. 
		/// The binding provider must implement <see cref="IBindingProvider"/>
		/// </summary>
		/// <value>The type of binding provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IBindingProvider"/>!</exception>
		public Type BindingProvider {
			get { return _bindingProvider; }
			set {
				if (!typeof (IBindingProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement IBindingProvider!");
				_bindingProvider = value;
			}
		}
	}

}