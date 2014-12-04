using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary>
	///     Interface for DisplayValueProvider (provides a value for display)
	/// </summary>
	/// <remarks></remarks>
	public interface IDisplayValueProvider : IViewModelProvider {

		/// <summary> Gets the display value (a string representation of the value)
		/// </summary>
		[Obsolete("Use String")]
		string Value { get; }

//		string Millimeter { get; } REMOVED [xgksc 2013-01-25]
//		string Centimeter { get; } REMOVED [xgksc 2013-01-25]

		/// <summary> Gets the value as an enum value (if possible; else null)
		/// </summary>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		object AsEnum { get; }

		/// <summary> Gets the value as bool (if possible; else null)
		/// </summary>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		bool? AsBooleanNull { get; }

		/// <summary> Gets the value as string
		/// </summary>
		string String { get; }

		/// <summary> Gets a value indicating the value is equal to 'True'
		/// </summary>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		bool IsTrue { get; }

		/// <summary> Gets a value indicating the value is equal to 'False'
		/// </summary>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		bool IsFalse { get; }
	}

	/// <summary> Provides a value for display
	/// </summary>
	public class DisplayValueProvider : ViewModelValueProvider, IDisplayValueProvider {

		private string m_DisplayValue;
		private bool m_DisplayValueIsInitialized;
		private object m_EnumValue;
		private bool? m_BooleanNullValue;
		private bool m_IsFalse;	
		private string m_StringValue;
		private TypeConverter m_TypeConverter = new ValueProviderStringConverter();
		private IValueVM m_ViewModelIsEnabledChangedIsObserved;
		private IValueVM m_ViewModelCultureChangedIsObserved;

		/// <summary> Gets or sets the type converter to convert a value into different types.
		/// </summary>
		/// <value>The type converter.</value>
		/// <remarks></remarks>
		public virtual TypeConverter TypeConverter {
			get { return m_TypeConverter; }
			set {
				MemberAccessUtil.DemandNotNull(value, null, this, "TypeConverter", "{431F8775-4ACD-42C5-AD23-F5963BBB162F}");
				MemberAccessUtil.DemandWriteOnce(Parent == null, null, this, "TypeConverter", "{A6A9D618-7ED4-454E-B7B1-D9153DE2AD52}");
				m_TypeConverter = value;
			}
		}

		/// <summary> Gets a value indicating whether the parent value view model is enabled.
		/// </summary>
		/// <value><c>true</c> if parent value view model is enabled; otherwise, <c>false</c>.</value>
		/// <remarks>
		/// safe alias for: <c>this.Parent.Parent.IsEnabled</c>
		/// </remarks>
		private bool ParentValueVMIsEnabled {
			get {
				var parentValueVM = (ObjectVM) ViewModel;
				return parentValueVM != null && parentValueVM.IsEnabled;
			}
		}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value>
		///     <see langword="true" /> if this instance is supported; otherwise, <see langword="false" />.
		/// </value>
		/// <remarks></remarks>
		public override bool IsSupported {
			get { return true; }
		}

		/// <summary> Gets the display value
		/// </summary>
		/// <value>The display value</value>
		[Obsolete("Use String")]
		public string Value {
			get {
				if (!m_DisplayValueIsInitialized) {
					if (UpdateDisplayValues(false)) m_DisplayValueIsInitialized = true;
				}
				return m_DisplayValue;
			}
		}

		/// <summary> Gets the value as string if applicable else null;.
		/// </summary>
		/// <remarks></remarks>
		public string String {
			get {
				if (!m_DisplayValueIsInitialized) {
					if (UpdateDisplayValues(false)) m_DisplayValueIsInitialized = true;
				}
				if (!ParentValueVMIsEnabled) return null;
				return m_StringValue;
			}
		}

		#region Boolean

		/// <summary> Gets the value as boolean if applicable else false;
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public bool? AsBooleanNull {
			get {
				if (!m_DisplayValueIsInitialized) {
					if (UpdateDisplayValues(false)) m_DisplayValueIsInitialized = true;
				}
				if (!ParentValueVMIsEnabled) return false;
				return m_BooleanNullValue;
			}
		}

		/// <summary> Gets a value indicating the value is applicable and equal to 'True'
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public bool IsTrue {
			get {
				if (!m_DisplayValueIsInitialized) {
					if (UpdateDisplayValues(false)) m_DisplayValueIsInitialized = true;
				}
				if (!ParentValueVMIsEnabled) return false;
				return m_BooleanNullValue == true;
			}
		}

		/// <summary> Gets a value indicating the value is applicable and equal to 'False'
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public bool IsFalse {
			get {
				if (!m_DisplayValueIsInitialized) {
					if (UpdateDisplayValues(false)) m_DisplayValueIsInitialized = true;
				}
				if (!ParentValueVMIsEnabled)
					return false;
				return m_IsFalse;
			}
		}

		#endregion

		/// <summary> Gets the value as enum if applicable; else Enum(0);.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public object AsEnum {
			get {
				if (!m_DisplayValueIsInitialized) {
					if (UpdateDisplayValues(false)) m_DisplayValueIsInitialized = true;
				}
				if (ViewModel.GetType().Name.Contains("EnumVM")) {
					Type t = ViewModel.GetType().GetGenericArguments()[0];
					if (!((ObjectVM) ViewModel).IsEnabled) {
						return Enum.ToObject(t, 0);
					}
				}

				return m_EnumValue;
			}
		}

		/// <summary> Called if ValueVM.Value has been changed
		/// </summary>
		/// <remarks></remarks>
		protected override void OnParentValueVMValueChanged() {
			base.OnParentValueVMValueChanged();
			UpdateDisplayValues(true);
		}

		private bool UpdateDisplayValues(bool raiseEvents) {
			if (ViewModel == null) return false;

			if(m_ViewModelIsEnabledChangedIsObserved!=ViewModel) {
				m_ViewModelIsEnabledChangedIsObserved = ViewModel;
				ViewModel.IsEnabledChanged+= delegate { UpdateDisplayValues(true); };
			}
			if(m_ViewModelCultureChangedIsObserved!=ViewModel) {
				m_ViewModelCultureChangedIsObserved = ViewModel;
				ObjectVM.CultureChanged+= delegate { UpdateDisplayValues(true); };
			}

			CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
			string tmp = null;

			#region Double
			if (ViewModel is DoubleVM) {
				var dataProvider = ViewModel.Metadata.DataProvider as IBusinessValueDataProvider;
				if(dataProvider!=null) {
					IValueBM bm = (dataProvider).BusinessValue;
					var decimalAccuracySpecified = bm!=null && /*bm.HasMetadata &&*/ ((BusinessValueMetadata)bm.Metadata).Settings.DecimalAccuracySpecified;
					if(decimalAccuracySpecified) {
						var da = (double)((BusinessValueMetadata)bm.Metadata).Settings.DecimalAccuracy;

						#region Round with DecimalAccuracy

						double doubleValue = double.NaN;
						if(System.Math.Abs(da-1.0)>double.Epsilon) // 'da' equals not 1.0
							doubleValue = System.Math.Round((double)ViewModel.Value/da, 0)*da;
						else
							doubleValue = System.Math.Round((double)ViewModel.Value, 0);

						#endregion

						tmp = (string)TypeConverter.ConvertFrom(null, culture, doubleValue);
					}
				}
			}
			#endregion

			#region string
			if(tmp==null && ViewModel is StringVM) {
				Exception exception;
				tmp = (string)ViewModel.TryGetValue(out exception);
				if(exception!=null) return false;
				if(ViewModel.HasMetadata) {
					var loc = ViewModel.Metadata.LocalizationProvider;
					if(loc!=null) {
						tmp = loc.GetString(tmp);
					}
				}
			}
			#endregion

			if (tmp==null) {
				Exception exception;
				var v = ViewModel.TryGetValue(out exception);
				if(exception!=null) return false;
				tmp = (string) TypeConverter.ConvertFrom(null, culture, v);
			}
			m_StringValue = ParentValueVMIsEnabled ? tmp : null;

			#region BoolVM

			if (ViewModel is BoolVM && ParentValueVMIsEnabled) {
				m_BooleanNullValue = (bool) ViewModel.Value;
				m_IsFalse = (m_BooleanNullValue == false);
			} else {
				m_BooleanNullValue = false;
				m_IsFalse = false;
			}

			#endregion

			#region EnumVM

			if (ViewModel.GetType().Name.Contains("EnumVM")) {
				Type t = ViewModel.GetType().GetGenericArguments()[0];
				if (!((ObjectVM) ViewModel).IsEnabled) {
					m_EnumValue = Enum.ToObject(t, 0);
				} else {
					m_EnumValue = ViewModel.Value;
				}
			}

			#endregion

			// if (Equals(tmp, m_DisplayValue)) return true;
			m_DisplayValue = tmp;
			if (raiseEvents) RaiseEvents();
			return true;
		}


		private void RaiseEvents() {
			OnPropertyChanged("Value");
			OnPropertyChanged("String");
			OnPropertyChanged("AsBooleanNull");
			OnPropertyChanged("AsEnum");
			OnPropertyChanged("IsTrue");
			OnPropertyChanged("IsFalse");
		}

		/// <summary> [WORKAROUND] Notifies this instance, the value has been changed. This will raise PropertyChanged events.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("WORKAROUND! Avoid using this!")]
		public void NotifyValueChanged() {
			RaiseEvents();
		}
	}

	/// <summary> Provides a DisplayValueProvider for enums
	/// </summary>
	/// <remarks></remarks>
	public class EnumDisplayValueProvider : ViewModelValueProvider, IDisplayValueProvider {

		private string m_DisplayValue;

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value>
		///     <see langword="true" /> if this instance is supported; otherwise, <see langword="false" />.
		/// </value>
		public override bool IsSupported {get { return true; }}

		/// <summary> Gets the display value
		/// </summary>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public string Value {get { return m_DisplayValue; }}

		/// <summary> Gets the value as string
		/// </summary>
		/// <remarks></remarks>
		public string String {get { return m_DisplayValue; /*TODO implement use of IsEnabled*/ }}

		/// <summary> Returns allways <see langword="null" />
		/// </summary>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public object AsEnum {get { return null; }}

		/// <summary> Returns allways <see langword="null" />
		/// </summary>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public bool? AsBooleanNull {get { return null; }}

		/// <summary> Gets a value indicating the value is equal to 'True'
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public bool IsTrue {get { return false; }}

		/// <summary> Gets a value indicating the value is equal to 'False'
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Obsolete [xgksc 2013-03-07]",true)]
		public bool IsFalse {get { return false; }}

		/// <summary> Sets the display value manually
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(String value) {
			m_DisplayValue = value;
			OnPropertyChanged("Value");
			OnPropertyChanged("String");
		}
	}
}