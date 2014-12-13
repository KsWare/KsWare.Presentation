using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> Interface for all ValueSourceProviders
	/// </summary>
	/// <remarks></remarks>
	public interface IValueSourceProvider:IProvider {

		/// <summary> Gets or sets the source list.
		/// </summary>
		/// <value>The source list.</value>
		IEnumerable SourceList{get;set;}
	}

	/// <summary> Provides a collection e.g. used to generate the content of the System.Windows.Controls.ItemsControl.
	/// </summary>
	/// <remarks></remarks>
	public class ValueSourceProvider:IValueSourceProvider {

		protected Lazy<EventSourceStore> m_LazyWeakEventProperties;
		private IEnumerable m_SourceList;
		private object m_Parent;

		/// <summary> Initializes a new instance of the <see cref="ValueSourceProvider"/> class.
		/// </summary>
		public ValueSourceProvider() {
			m_LazyWeakEventProperties=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}
		public EventSourceStore EventSources{get { return m_LazyWeakEventProperties.Value; }}

		/// <summary> Gets or sets the source list.
		/// </summary>
		/// <value>The source list.</value>
		/// <remarks> The 'source list' is a collection e.g. used to generate the content of the System.Windows.Controls.ItemsControl.</remarks>
		/// <example>
		/// <code>
		/// &lt;ItemsControl ItemsSource="{Binding ValueSourceProvider.SourceList}"/>
		/// </code>
		/// </example>
		/// <example>
		/// <code>
		/// &lt;Style x:Key="MyComboBoxStyle" TargetType="ComboBox" BasedOn="{StaticResource {x:Type ComboBox}}" >
		///     &lt;Setter Property="ItemsSource" Value="{Binding ValueSourceProvider.SourceList, Mode=OneWay}"/>
		/// &lt;/Style>
		/// </code>
		/// </example>
		public IEnumerable SourceList {
			get {return m_SourceList;}
			set {
				m_SourceList=value;
				OnPropertyChanged("SourceList");
			}
		}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public bool IsSupported {get {return true;}}

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
				EventManager.Raise<EventHandler,EventArgs>(m_LazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ParentChanged;
		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); }}

		/// <summary> Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary> Gets the event source for the event which occurs when a property value changes.
		/// </summary>
		/// <value>The property changed event.</value>
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent{get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }}

		[NotifyPropertyChangedInvocator]
//		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{B410DCA0-779B-4F54-9718-B3651E8E79C7}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventProperties,"PropertyChangedEvent", args);
		}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {
			//TODO implement Dispose
		}
	}

	/// <summary> Provides a list with valid values 
	/// using the IncludeValues from underlying business layer value settings.
	/// </summary>
	public class BusinessValueSourceProvider:IValueSourceProvider,IWeakEventListener {

		protected Lazy<EventSourceStore> m_LazyWeakEventProperties;
		private IValueBM m_BusinessValue;
		private readonly ObservableNotifyableCollection<object> m_Values=new ObservableNotifyableCollection<object>();
		private object m_Parent;
		private IValueSettings m_BusinessValueSettings;

		/// <summary> Initializes a new instance of the <see cref="BusinessValueSourceProvider"/> class.
		/// </summary>
		public BusinessValueSourceProvider() {
			m_LazyWeakEventProperties=new Lazy<EventSourceStore>(() => new EventSourceStore(this));
		}

		public EventSourceStore EventSources{get { return m_LazyWeakEventProperties.Value; }}

		/// <summary> Gets or sets the business layer value.
		/// </summary>
		/// <value>The business value.</value>
		public IValueBM BusinessValue {
			get {return m_BusinessValue;}
			set {
				if(m_BusinessValue!=null) {
					//m_BusinessValue.Disposed-=AtBusinessValueDisposed;
					m_BusinessValue.SettingsChanged-=AtBusinessValueSettingsChanged;
					if(m_BusinessValue.Settings.IncludeValues is INotifyCollectionChanged) {
						CollectionChangedEventManager.RemoveListener(((INotifyCollectionChanged) m_BusinessValue.Settings.IncludeValues), this);
					}
				}

				m_BusinessValue = value;
				
				if(m_BusinessValue!=null){
					//m_BusinessValue.Disposed+=AtBusinessValueDisposed;
					m_BusinessValue.SettingsChanged+=AtBusinessValueSettingsChanged;
				}
				AtBusinessValueSettingsChanged(value,new ValueSettingsChangedEventArgs(ValueSettingName.All));
			}
		}

		/// <summary> Gets the view model value.
		/// </summary>
		/// <value>The view model value.</value>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private IValueVM ViewModelValue {
			get {
				var md = Parent as ViewModelMetadata; if(md==null) return null;
				var vm = md.Parent as IValueVM; if(vm==null) return null;
				return vm;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void AtBusinessValueDisposed(object sender, EventArgs eventArgs) { 
			if(sender!=m_BusinessValue) return;
			BusinessValue = null;
		}

		private void AtBusinessValueSettingsChanged(object sender, ValueSettingsChangedEventArgs args) {
			// <- ValueBM{set;}
			// <- ValueBM.SettingsChanged-event
			var newSettings = m_BusinessValue != null ? m_BusinessValue.Settings : null;
			if(m_BusinessValueSettings!=newSettings) {
				if(m_BusinessValueSettings!=null) {
					if(m_BusinessValueSettings.IncludeValues is INotifyCollectionChanged) {
						CollectionChangedEventManager.RemoveListener(((INotifyCollectionChanged) m_BusinessValueSettings.IncludeValues), this);
					}
					if(m_BusinessValueSettings.ExcludeValues is INotifyCollectionChanged) {
						CollectionChangedEventManager.RemoveListener(((INotifyCollectionChanged) m_BusinessValueSettings.ExcludeValues), this);
					}
				}

				m_BusinessValueSettings = newSettings;

				if(m_BusinessValueSettings!=null) {
					if(m_BusinessValueSettings.IncludeValues is INotifyCollectionChanged) {
						CollectionChangedEventManager.AddListener(((INotifyCollectionChanged) m_BusinessValueSettings.IncludeValues), this);
					}				
					if(m_BusinessValueSettings.ExcludeValues is INotifyCollectionChanged) {
						CollectionChangedEventManager.AddListener(((INotifyCollectionChanged) m_BusinessValueSettings.ExcludeValues), this);
					}				
				}
			}

			AtListChanged();
		}

		private void AtListChanged() {
			// <- OnBusinessValueSettingsChanged()
			// <- businessValueSettings.ExcludeValues.CollectionChanged-event

			ApplicationDispatcher.CurrentDispatcher.InvokeIfRequired(new Action(FillValueList));
		}

		private void FillValueList() {
			m_Values.Clear();
			
			if (m_BusinessValue == null) return;
			if (m_BusinessValue.Settings == null || !m_BusinessValue.Settings.IncludeValuesSpecified || m_BusinessValue.Settings.IncludeValues == null) return;
			
			foreach (var value in m_BusinessValue.Settings.IncludeValues) {
				if (value.GetType().IsEnum) {
					var vm = EnumVM.GetVM((Enum)value);
					m_Values.Add(vm);
				} else {
					m_Values.Add(value);
				}
			}
		}

		/// <summary> Gets or sets the a list of available values.
		/// </summary>
		/// <value>The list of available values.</value>
		public IEnumerable SourceList {get {return m_Values;}set {throw new InvalidOperationException("The SourceList is managed by underlying business object only!");}}

		/// <summary> Receives events from the centralized event manager.
		/// </summary>
		/// <returns> true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle. </returns>
		/// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param><param name="sender">Object that originated the event.</param><param name="e">Event data.</param>
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			AtListChanged();
			return true;
		}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public bool IsSupported {get {return true;}}

		/// <summary> Gets or sets the parent of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		/// <remarks></remarks>
		public object Parent {
			get {return m_Parent;}
			set {
				MemberAccessUtil.DemandNotNull(value,"Parent cannot be null!",this,"{9852266A-54E4-4EDB-9A81-41B7F7F48CB9}");
				MemberAccessUtil.DemandWriteOnce(m_Parent==null,null,this,"Parent","{8A92663A-CC1A-4914-9AB8-E08602EB330D}");
				m_Parent = value;
				EventUtil.Raise(ParentChanged,this,EventArgs.Empty,"{87C3EE1D-5BA3-4F93-95EA-1EC5D45D5C2C}");
				EventManager.Raise<EventHandler,EventArgs>(m_LazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
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

		//
		/// <summary> [WORKAROUND] Notifies the source list changed. This will raise PropertyChanged events.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("WORKAROUND! Avoid using this!")]
		public void NotifySourceListChanged() {
			((IDoNotifyCollectionChanged)SourceList).NotifyCollectionChanged();
			if(Parent!=null) ((DisplayValueProvider)((ViewModelMetadata) Parent).DisplayValueProvider).NotifyValueChanged();
			//AtListChanged();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public IEventSource<PropertyChangedEventHandler> PropertyChangedEvent{get { return EventSources.Get<PropertyChangedEventHandler>("PropertyChangedEvent"); }}

		[NotifyPropertyChangedInvocator]
//		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
		protected virtual void OnPropertyChanged(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			EventUtil.Raise(PropertyChanged,this,args,"{AA1E098D-2138-4537-9E35-85122D4CD7A3}");
			EventManager.Raise<PropertyChangedEventHandler,PropertyChangedEventArgs>(m_LazyWeakEventProperties,"PropertyChangedEvent", args);
		}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {
			//TODO implement Dispose
		}
	}
}
