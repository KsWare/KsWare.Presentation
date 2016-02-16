using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Providers;

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
	public class ValueSourceProvider:Provider,IValueSourceProvider {

		
		private IEnumerable m_SourceList;


		/// <summary> Initializes a new instance of the <see cref="ValueSourceProvider"/> class.
		/// </summary>
		public ValueSourceProvider() {
			
		}

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
		public override bool IsSupported {get {return true;}}

	}

	/// <summary> Provides a list with valid values 
	/// using the IncludeValues from underlying business layer value settings.
	/// </summary>
	public class BusinessValueSourceProvider:Provider,IValueSourceProvider,IWeakEventListener {

		private IValueBM m_BusinessValue;
		private readonly ObservableNotifyableCollection<object> m_Values=new ObservableNotifyableCollection<object>();
		private IValueSettings m_BusinessValueSettings;

		/// <summary> Initializes a new instance of the <see cref="BusinessValueSourceProvider"/> class.
		/// </summary>
		public BusinessValueSourceProvider() {
		}

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
		public override bool IsSupported {get {return true;}}


		/// <summary> [WORKAROUND] Notifies the source list changed. This will raise PropertyChanged events.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("WORKAROUND! Avoid using this!")]
		public void NotifySourceListChanged() {
			((IDoNotifyCollectionChanged)SourceList).NotifyCollectionChanged();
			if(Parent!=null) ((DisplayValueProvider)((ViewModelMetadata) Parent).DisplayValueProvider).NotifyValueChanged();
			//AtListChanged();
		}


	}
}
