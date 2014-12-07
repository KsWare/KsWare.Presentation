using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using KsWare.Presentation.BusinessFramework.Providers;
using KsWare.Presentation.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Interface for business value
	/// </summary>
	public interface IValueBM: IObjectBM {

		/// <summary> Occurs when <see cref="Value"/> has been changed.
		/// </summary>
		event EventHandler<ValueChangedEventArgs> ValueChanged;

		/// <summary> Occurs when <see cref="Settings"/> or a Settings property has been changed.
		/// </summary>
		event EventHandler<ValueSettingsChangedEventArgs> SettingsChanged;

//		event EventHandler<ValueErrorEventArgs> ValueErrorChanged;

		/// <summary> Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		object Value{get;set;}

		/// <summary> Gets a value indicating whether this instance has a value.
		/// </summary>
		/// <value> <see langword="true"/> if this instance has a value; otherwise, <see langword="false"/>.
		/// </value>
		bool HasValue{get;}

		/// <summary> Gets the type of the value.
		/// </summary>
		/// <value>The type of the value.</value>
		Type ValueType{get;}

		/// <summary> Gets the business value settings.
		/// </summary>
		/// <value>The business value settings.</value>
		IValueSettings Settings{get;}

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns> <see langword="true"/> if the value is valid; else  <see langword="false"/></returns>
		Exception Validate(object value, bool throwOnInvalid);

	}

	/// <summary> Generic interface for business value
	/// </summary>
	/// <typeparam name="TValue">Type of value</typeparam>
	public interface IValueBM<TValue>: IObjectBM {

		/// <summary> Occurs when <see cref="Value"/> has been changed.
		/// </summary>
		event EventHandler<ValueChangedEventArgs> ValueChanged;

		/// <summary> Occurs when <see cref="Settings"/> or a Settings property has been changed.
		/// </summary>
		event EventHandler<ValueSettingsChangedEventArgs> SettingsChanged;

//		event EventHandler<ValueErrorEventArgs> ValueErrorChanged;

		/// <summary> Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		TValue Value{get;set;}

		/// <summary> Gets a value indicating whether this instance has a value.
		/// </summary>
		/// <value> <see langword="true"/> if this instance has a value; otherwise, <see langword="false"/>.
		/// </value>
		bool HasValue{get;}

		/// <summary> Gets the business value settings.
		/// </summary>
		/// <value>The business value settings.</value>
		IValueSettings<TValue> Settings{get;}

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns> <see langword="true"/> if the value is valid; else  <see langword="false"/></returns>
		Exception Validate(TValue value, bool throwOnInvalid);

	}


	/// <summary> Provides a strong typed business value
	/// </summary>
	/// <typeparam name="TValue">Type of business value</typeparam>
	public class ValueBM<TValue>: ObjectBM, IValueBM<TValue>, IValueBM {

		/// <summary> Initializes a new instance of the <see cref="ValueBM{TValue}"/> class.
		/// </summary>
		public ValueBM() { }

		/// <summary> Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="explicitDisposing"><c>true</c> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
		protected override void Dispose(bool explicitDisposing) {
			base.Dispose(explicitDisposing);
		}

		#region IValueBM<T>

		/// <summary> Occurs when <see cref="Value"/> has been changed.
		/// </summary>
		public event EventHandler<ValueChangedEventArgs> ValueChanged;

		/// <summary> Occurs when <see cref="Settings"/> or a Settings property has been changed.
		/// </summary>
		public event EventHandler<ValueSettingsChangedEventArgs> SettingsChanged;

//		/// <summary> Occurs when <see cref="ValueError"/>-property has been changed.
//		/// </summary>
//		public event EventHandler<ValueErrorEventArgs> ValueErrorChanged;


		/// <summary> Gets or sets the business object metadata.
		/// </summary>
		/// <value>The business object metadata.</value>
		public new BusinessValueMetadata Metadata {get {return (BusinessValueMetadata) base.Metadata;}set {base.Metadata = value;}}

		/// <summary> Called before the <see cref="Metadata"/>-property is changed.
		/// </summary>
		/// <param name="newMetadata">The metadata.</param>
		protected override void OnMetadataChanging(BusinessMetadata newMetadata) {
			var vmd = (BusinessValueMetadata) newMetadata;
			if(vmd.Settings==null) vmd.Settings=new ValueSettings<TValue>();
			base.OnMetadataChanging(newMetadata);
		}

		/// <summary> Called after the <see cref="Metadata"/>-property has been changed.
		/// </summary>
		protected override void OnMetadataChanged() {
			base.OnMetadataChanged();
			if (Metadata.Settings == null) Debug.WriteLine("=>ObjectBM: WARNING: No Settings specified!");
			Settings.PropertyChanged += AtPropertyChanged;
			Metadata.DataProvider.DataChanged+=AtDataProviderDataChanged;
		}

		private void AtDataProviderDataChanged(object sender, DataChangedEventArgs e) { 
			OnValueChanged(new ValueChangedEventArgs(e.PreviousData,e.NewData));
		}

		/// <summary> Gets the value settings.
		/// </summary>
		/// <value>The value settings.</value>
		public IValueSettings<TValue> Settings {get {return (IValueSettings<TValue>) Metadata.Settings;}}

		private object GetValueHelper() {
			object baseValue = this.Metadata.DataProvider.Data;
			var value = this.Metadata.CoerceValueCallback != null ? this.Metadata.CoerceValueCallback(this, baseValue) : baseValue;
			return value;
		}

		/// <summary> Gets or sets the value of the current <see cref="ValueBM{TValue}"/> value.
		/// </summary>
		/// <value>The value of the current <see cref="ValueBM{TValue}"/> object.
		///  An exception is thrown if the <see cref="HasValue"/> property is false.</value>
		public TValue Value {
			//TODO implement support for Nullable-types
			get {
				object value = GetValueHelper();
//TODO			if(value==null && Nullable.GetUnderlyingType(typeof(T))==null && typeof(T).IsValueType) throw new InvalidOperationException("");
				if(value==null && Nullable.GetUnderlyingType(typeof(TValue))==null && typeof(TValue).IsValueType) return default(TValue);
				return (TValue) value;
			}
			set {
				//Debug.WriteLine("=>Trace: ValueBM.Value{set;} Name='"+Name+"' Value="+value);
				object oldValue = Metadata.DataProvider.Data;
				if(Equals(oldValue,value)) return;
				Validate(value, true);
				Metadata.DataProvider.Data=value;
			    //OnValueChanged(); should be done by Dataprovider
			}
		}

		//### Empty
		//### Plausibility
		/// <summary> Sets the value of the current property without validate the value
		/// Therfore the value can set to null for the plausibility control to reset <see cref="ValueBM{TValue}"/> value.
		/// </summary>
		public void SetValue(TValue value, bool validate){
			object oldValue = Metadata.DataProvider.Data;
			if(Equals(oldValue,value)) return;
			if (validate) Validate(value, true);
			Metadata.DataProvider.Data = value;
			 //OnValueChanged(); should be done by Dataprovider
		}

		/// <summary> Gets a value indicating whether this instance has a value.
		/// </summary>
		/// <value>true if the current <see cref="ValueBM{TValue}"/> object has a value; false if the current <see cref="ValueBM{TValue}"/> object has no value.</value>
		/// <remarks>
		/// If the HasValue property is true, the value of the current <see cref="ValueBM{TValue}"/> can be accessed with the <see cref="Value"/> property.
		/// </remarks>
		public bool HasValue {
			get {
				object value=null;
				try{value= GetValueHelper();}catch(Exception ex){
					this.DoNothing(ex); 
					Debug.WriteLine("WARNING: Exception caught and ignored in BusinessValues.HasValue; returns with HasValue=false; UniqueID: {C44E88F0-876F-4E8B-B3FF-786B7DD711E7}");
					return false;
				}
				return value != null;
			}
		}

		/// <summary> Validates the specified value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <param name="throwOnInvalid">if set to <see langword="true"/> throws on invalid value.</param>
		/// <returns> <see langword="true"/> if the value is valid; else  <see langword="false"/></returns>
		public virtual Exception Validate(TValue value, bool throwOnInvalid) {
			if(Metadata.ValidateValueCallback!=null) {
				var result = this.Metadata.ValidateValueCallback(this, value);
				if(!throwOnInvalid) return result;
				if (result != null)
					throw result; //### Empty
				return result;
			}
			return null;
		}
		
		#endregion

		/// <summary> Creates the default metadata for the current type of business object .
		/// </summary>
		/// <returns>Business object metadata</returns>
		protected override BusinessMetadata CreateDefaultMetadata() {
			return new BusinessValueMetadata {
				DataProvider = new LocalDataProvider<TValue>(),
				Settings = new ValueSettings<TValue>()
			};
		}

		/// <summary> Raises the <see cref="ValueChanged"/>- and  TreeChanged-event.
		/// </summary>							
		protected virtual void OnValueChanged(ValueChangedEventArgs e) {
			if (ValueChanged != null) ValueChanged(this, e);
			OnTreeChanged();
			//REVISE ??? Metadata.DataProvider.NotifyDataChanged();
		}

//		/// <summary> Raises the <see cref="ValueErrorChanged"/> event.
//		/// </summary>
//		/// <param name="e">The <see cref="KsWare.Presentation.BusinessFramework.ValueErrorEventArgs"/> instance containing the event data.</param>
//		protected virtual void OnValueErrorChanged(ValueErrorEventArgs e) { 
//			if (ValueErrorChanged != null) ValueErrorChanged(this, e); 
//		}

		/// <summary> Called if <see cref="OnMetadataChanged"/>. Raises the <see cref="SettingsChanged"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="KsWare.Presentation.BusinessFramework.ValueSettingsChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void AtPropertyChanged(object sender, ValueSettingsChangedEventArgs e) {
			if (SettingsChanged != null) SettingsChanged(this, e);

			//revalidate
//            try {
//                Validate(Value, true);
//                OnValueErrorChanged(new ValueErrorEventArgs(BusinessModelError.None, this));
//            } catch (BusinessModelException ex) {
//                OnValueErrorChanged(new ValueErrorEventArgs(ex));
//            } catch (Exception ex) {
//                OnValueErrorChanged(new ValueErrorEventArgs(new ValueValidationException(Value, this, ex)));
//            }
		}


		#region IValueBM (explicit implementation for invariant compatibility)

		object IValueBM.Value{get {return this.Value;}set {this.Value = (TValue) value;}}
		
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		Type IValueBM.ValueType{get {return typeof(TValue);}}

		IValueSettings IValueBM.Settings{get {return (IValueSettings) this.Settings;}}

		Exception IValueBM.Validate(object value, bool throwOnInvalid) { return this.Validate((TValue) value, throwOnInvalid); }

		#endregion

	}
}