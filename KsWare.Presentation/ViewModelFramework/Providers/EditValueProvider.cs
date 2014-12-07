using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using JetBrains.Annotations;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> Interface for edit value providers (***EditValueProvider)
	/// </summary>
	public interface IEditValueProvider:IViewModelProvider,INotifyPropertyChanged {

//		/// <summary> Gets or sets the edit value.
//		/// </summary>
//		/// <value>The edit value.</value>
//		[Obsolete("Use String",true)]
//		string Value{get;set;}

		/// <summary> Gets or sets the edit value.
		/// </summary>
		/// <value>The edit value.</value>
		string String{get;set;}
	
		/// <summary> Gets or sets the edit value.
		/// </summary>
		/// <value>The edit value.</value>
		bool? BoolNullable { get; set; }

		IEditValueProviderTimeSpanExtension TimeSpan { get; }

		IEditValueProviderDateTimeExtension DateTime { get; }

		IEditValueProviderHexNumberExtension HexNumber { get; }

		IEditValueProviderMetricExtension Metric { get; }

		IEditValueProviderQuantizationExtension Quantization { get; }
	}


	/// <summary> Provides the editable value
	/// </summary>
	public sealed partial class EditValueProvider:ViewModelValueProvider,IEditValueProvider,INotifyPropertyChanged {

		private TypeConverter m_TypeConverter      = new ValueProviderStringConverter();

		private Lazy<StringExtension      > m_LazyString;
		private Lazy<HexNumberExtension   > m_LazyHexNumber;
		private Lazy<QuantizationExtension> m_LazyQuantization;
		private Lazy<TimeSpanExtension    > m_LazyTimeSpan;
		private Lazy<DateTimeExtension    > m_LazyDateTime;
		private Lazy<MetricExtension      > m_LazyMetric;
		private Lazy<NullableBoolExtension> m_LazyNullableBool;

		/// <summary> Initializes a new instance of the <see cref="EditValueProvider"/> class.
		/// </summary>
		public EditValueProvider() {
			m_LazyString       = new Lazy<StringExtension      >(()=>new StringExtension      (this));
			m_LazyDateTime     = new Lazy<DateTimeExtension    >(()=>new DateTimeExtension    (this));
			m_LazyTimeSpan     = new Lazy<TimeSpanExtension    >(()=>new TimeSpanExtension    (this));
			m_LazyHexNumber    = new Lazy<HexNumberExtension   >(()=>new HexNumberExtension   (this));
			m_LazyQuantization = new Lazy<QuantizationExtension>(()=>new QuantizationExtension(this));
			m_LazyMetric       = new Lazy<MetricExtension      >(()=>new MetricExtension      (this));
			m_LazyNullableBool = new Lazy<NullableBoolExtension>(()=>new NullableBoolExtension(this));
		}

		/// <summary> Gets a value indicating whether this instance is supported.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is supported; otherwise, <c>false</c>.
		/// </value>
		public override bool IsSupported{get {return true;}}

		/// <summary> Gets the type converter.
		/// </summary>
		/// <value>The type converter.</value>
		/// <seealso cref="ValueProviderStringConverter"/>
		public TypeConverter TypeConverter{get{return m_TypeConverter;} private set{m_TypeConverter = value;}}

		/// <summary> Gets or sets the editable value as string.
		/// </summary>
		/// <value>The editable value.</value>
		public string String {get { return m_LazyString.Value.Value; } set { m_LazyString.Value.Value=value; }}

		/// <summary> Gets or sets the editable value as <see cref="Nullable{Boolean}"/>.
		/// </summary>
		/// <value>The editable value.</value>
		/// <remarks>Use <see cref="BoolNullable"/> to bind e.g. <see cref="CheckBox.IsChecked"/> property</remarks>
		public bool? BoolNullable {get { return m_LazyNullableBool.Value.Value; }set { m_LazyNullableBool.Value.Value = value; }}

		public IEditValueProviderStringExtension StringExt { get { return m_LazyString.Value; } }
		public IEditValueProviderNullableBoolExtension BoolNullableExt { get { return m_LazyNullableBool.Value; } }
		public IEditValueProviderHexNumberExtension HexNumber { get { return m_LazyHexNumber.Value; } }
		public IEditValueProviderTimeSpanExtension TimeSpan { get { return m_LazyTimeSpan.Value; } }
		public IEditValueProviderDateTimeExtension DateTime { get { return m_LazyDateTime.Value; } }
		public IEditValueProviderMetricExtension Metric { get { return m_LazyMetric.Value; }}
		public IEditValueProviderQuantizationExtension Quantization { get { return m_LazyQuantization.Value; } }

		/// <summary> Called if ValueVM.Value has been changed
		/// </summary>
		/// <remarks></remarks>
		protected override void OnParentValueVMValueChanged() {
			base.OnParentValueVMValueChanged();
			UpdateValue(true);
		}

		/// <summary> Updates the editable value
		/// </summary>
		/// <param name="raiseEvents">if set to <c>true</c> raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.</param>
		/// <remarks></remarks>
		private void UpdateValue(bool raiseEvents) {
			if(ViewModel==null) return;

			if(m_LazyString      .IsValueCreated) if(((StringExtension      ) StringExt      ).UpdateValue(raiseEvents)) OnPropertyChanged(()=>String      );
			if(m_LazyNullableBool.IsValueCreated) if(((NullableBoolExtension) BoolNullableExt).UpdateValue(raiseEvents)) OnPropertyChanged(()=>BoolNullable);
			if(m_LazyMetric      .IsValueCreated)    ((MetricExtension      ) Metric         ).UpdateValue(raiseEvents);
			if(m_LazyQuantization.IsValueCreated)    ((QuantizationExtension) Quantization   ).UpdateValue(raiseEvents);
			if(m_LazyHexNumber   .IsValueCreated)    ((HexNumberExtension   ) HexNumber      ).UpdateValue(raiseEvents);
			if(m_LazyDateTime    .IsValueCreated)    ((DateTimeExtension    ) DateTime       ).UpdateValue(raiseEvents);
			if(m_LazyTimeSpan    .IsValueCreated)    ((TimeSpanExtension    ) TimeSpan       ).UpdateValue(raiseEvents);
		}

		/// <summary> Updates the source.
		/// </summary>
		/// <param name="newValue">The new value.</param>
		/// <exception cref="System.InvalidOperationException">Update source failed!</exception>
		private void UpdateSource(object newValue) {
			#region Validate
			try {
				if(Metadata!=null && Metadata.DataProvider!=null && Metadata.DataProvider.IsSupported) {
					var exception=Metadata.DataProvider.Validate(newValue);
					if(exception!=null) {
						((IErrorProviderController) Metadata.ErrorProvider).SetError(exception.Message);//TODO localize message; 
						return;
					}
				}
			} catch (Exception ex) {
				((IErrorProviderController) Metadata.ErrorProvider).SetError(ex.Message);//TODO localize message
				return;
			}
			#endregion

			#region Set
			try {
				var prevValue = ViewModel.Value;
				ViewModel.Value = newValue;

				var ea = new ValueChangedEventArgs(prevValue, newValue);
				EventUtil.Raise(SourceUpdated,this,ea,"{F5D63124-F178-4467-AAEF-B43D9A072A07}");
				EventUtil.WeakEventManager.Raise<EventHandler<ValueChangedEventArgs>>(LazyWeakEventProperties,"SourceUpdatedEvent",ea);

				#region WORKARROUND
				if (!Equals(ViewModel.Value,newValue)) throw new InvalidOperationException("Update source failed!"+"\n\t"+"ErrorID:{05434B88-D178-4007-9785-0C69F3892B0A}");
				#endregion
			}catch (Exception ex) {
				((IErrorProviderController) ViewModel.Metadata.ErrorProvider).SetError(ex.Message);//TODO localize message
				return;
			}
			#endregion

			((IErrorProviderController) ViewModel.Metadata.ErrorProvider).ResetError();
		}

		public EventHandler<ValueChangedEventArgs> SourceUpdated;
		public IWeakEventSource<EventHandler<ValueChangedEventArgs>> SourceUpdatedEvent;


		/// <summary> Determines whether the specified value is an integer value.
		/// </summary>
		/// <param name="value">The value or <see cref="Type"/></param>
		/// <returns><c>true</c> if the specified value is an integer; otherwise, <c>false</c>.</returns>
		private static bool IsInteger(object value) {
			switch (value==null?TypeCode.Empty:Type.GetTypeCode(value is Type?(Type)value:value.GetType())) {
				case TypeCode.SByte:case TypeCode.Int16:case TypeCode.Int32:case TypeCode.Int64:
				case TypeCode.Byte:case TypeCode.UInt16:case TypeCode.UInt32:case TypeCode.UInt64: return true;
				default: return false;
			}
		}

		private static bool IsNumeric(object value) {
			switch (value==null?TypeCode.Empty:Type.GetTypeCode(value is Type?(Type)value:value.GetType())) {
				case TypeCode.SByte:case TypeCode.Int16: case TypeCode.Int32: case TypeCode.Int64: 
				case TypeCode.Byte:case TypeCode.UInt16: case TypeCode.UInt32: case TypeCode.UInt64: 
				case TypeCode.Single:case TypeCode.Double:case TypeCode.Decimal: return true;
				default: return false;
			}
		}
	}

	partial class EditValueProvider : IDataErrorInfo {

		string IDataErrorInfo.this[string columnName] {
			get {
				return ViewModel.Metadata.ErrorProvider.HasError == false ? "" : ViewModel.Metadata.ErrorProvider.ErrorMessage;
			}
		}

		string IDataErrorInfo.Error {
			get {
				return ViewModel.Metadata.ErrorProvider.HasError == false ? "" : ViewModel.Metadata.ErrorProvider.ErrorMessage;
			}
		}
	}

	partial class EditValueProvider {

		private string m_StringOnGotFocus;

		public void NotifyGotFocus() {
			m_StringOnGotFocus = String;
		}

		public void NotifyLostFocus() {
			this.DoNothing(m_StringOnGotFocus);
		}

		public void NotifyKeyDown(KeyEventArgs e, string textPreview) {
			this.DoNothing();
		}

		public void NotifyKeyUp(KeyEventArgs e) {
			this.DoNothing();
		}

		public void NotifyTextInput(TextCompositionEventArgs e) {
			this.DoNothing();
			//e.Handled = true;
		}

		public void NotifyTextChanged(TextChangedEventArgs e, string text) {
			var ea = new ValueChangingEventArgs(text);
			if(StringChanging!=null) StringChanging(this, ea);
		}

		public event TextChangedEventHandler StringChanging;

		public delegate void TextChangedEventHandler(object sender, ValueChangingEventArgs args);

	}
}

