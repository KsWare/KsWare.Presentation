namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides metadata for a business value property, specifically adding framework-specific property system characteristics.
	/// </summary>
	public class BusinessValueMetadata:BusinessMetadata {
		
		private IValueSettings _settings;
		private ValidateValueCallback _validateValueCallback;
		private BusinessValueChangedCallback _valueChangedCallback;
		private CoerceValueCallback _coerceValueCallback;

		/// <summary> Initializes a new instance of the <see cref="BusinessValueMetadata"/> class.
		/// </summary>
		public BusinessValueMetadata():this(null,null,null,null) {}

		/// <summary> Initializes a new instance of the BusinessValueMetadata class with the specified settings and callbacks.  
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="validateValueCallback">A reference to a callback that should perform any custom validation of the business value value beyond typical type validation.</param>
		/// <param name="valueChangedCallback">A reference to a handler implementation that the property system will call whenever the effective value of the property changes.</param>
		/// <param name="coerceValueCallback">A reference to a handler implementation will be called whenever the property system calls CoerceValue for the business value.</param>
		public BusinessValueMetadata(IValueSettings settings, ValidateValueCallback validateValueCallback, BusinessValueChangedCallback valueChangedCallback, CoerceValueCallback coerceValueCallback) {
			_settings              = settings;
			_validateValueCallback = validateValueCallback;
			_valueChangedCallback  = valueChangedCallback;
			_coerceValueCallback   = coerceValueCallback;
		}



		/// <summary> Gets or sets the validate value callback.
		/// </summary>
		/// <value>The validate value callback.</value>
		public ValidateValueCallback ValidateValueCallback {
			get {
				if(this._validateValueCallback!=null) return this._validateValueCallback;

				//if no custom validator is specified and Settings is specified return the Settings validator
				if(this._settings!=null) return this.Settings.Validate;

				//else return null
				return null;
			}
			set {
				DemandWrite();
				_validateValueCallback = value;
			}
		}

		/// <summary> Gets or sets a reference to a PropertyChangedCallback implementation specified in this metadata.
		/// </summary>
		/// <value>A PropertyChangedCallback implementation reference.</value>
		public BusinessValueChangedCallback ValueChangedCallback {
			get {return _valueChangedCallback;}
			set {
				DemandWrite();
				_valueChangedCallback = value;
			}
		}

		/// <summary> Gets or sets a reference to a CoerceValueCallback implementation specified in this metadata.
		/// </summary>
		/// <value>A CoerceValueCallback implementation reference.</value>
		public CoerceValueCallback CoerceValueCallback {
			get {return _coerceValueCallback;}
			set {
				DemandWrite();
				_coerceValueCallback = value;
			}
		}

		/// <summary> Gets or sets the settings for the business value.
		/// </summary>
		/// <value>The settings.</value>
		public IValueSettings Settings {
			get {return _settings;}
			set {
				DemandWrite();
				_settings = value;
			}
		}
	}
}