namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides metadata for a business value property, specifically adding framework-specific property system characteristics.
	/// </summary>
	public class BusinessValueMetadata:BusinessMetadata {
		
		private IValueSettings m_Settings;
		private ValidateValueCallback m_ValidateValueCallback;
		private BusinessValueChangedCallback m_ValueChangedCallback;
		private CoerceValueCallback m_CoerceValueCallback;

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
			m_Settings              = settings;
			m_ValidateValueCallback = validateValueCallback;
			m_ValueChangedCallback  = valueChangedCallback;
			m_CoerceValueCallback   = coerceValueCallback;
		}



		/// <summary> Gets or sets the validate value callback.
		/// </summary>
		/// <value>The validate value callback.</value>
		public ValidateValueCallback ValidateValueCallback {
			get {
				if(this.m_ValidateValueCallback!=null) return this.m_ValidateValueCallback;

				//if no custom validator is specified and Settings is specified return the Settings validator
				if(this.m_Settings!=null) return this.Settings.Validate;

				//else return null
				return null;
			}
			set {
				DemandWrite();
				m_ValidateValueCallback = value;
			}
		}

		/// <summary> Gets or sets a reference to a PropertyChangedCallback implementation specified in this metadata.
		/// </summary>
		/// <value>A PropertyChangedCallback implementation reference.</value>
		public BusinessValueChangedCallback ValueChangedCallback {
			get {return m_ValueChangedCallback;}
			set {
				DemandWrite();
				m_ValueChangedCallback = value;
			}
		}

		/// <summary> Gets or sets a reference to a CoerceValueCallback implementation specified in this metadata.
		/// </summary>
		/// <value>A CoerceValueCallback implementation reference.</value>
		public CoerceValueCallback CoerceValueCallback {
			get {return m_CoerceValueCallback;}
			set {
				DemandWrite();
				m_CoerceValueCallback = value;
			}
		}

		/// <summary> Gets or sets the settings for the business value.
		/// </summary>
		/// <value>The settings.</value>
		public IValueSettings Settings {
			get {return m_Settings;}
			set {
				DemandWrite();
				m_Settings = value;
			}
		}
	}
}