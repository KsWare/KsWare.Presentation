using System;
using System.Diagnostics;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {

	//REVIEW make BusinessMetadata abstract

	/// <summary> Provides command metadata for a business object property, specifically adding framework-specific property system characteristics.
	/// </summary>
	public class BusinessMetadata:IMetadata,IParentSupport, IParentSupport<IObjectBM> {

		private Lazy<EventSourceStore> m_LazyWeakEventProperties;
		private IObjectBM m_BusinessObject;
		private IDataProvider m_DataProvider;
		private EventHandler m_ParentChanged;

		public BusinessMetadata() {
			m_LazyWeakEventProperties=new Lazy<EventSourceStore>(()=>new EventSourceStore(this));
		}

		/// <summary> Gets the ObjectBM which owns this metadata
		/// </summary>
		/// <remarks>
		/// After setting businessObject to value other as null all meta data properties will be write protected.
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "KsWare.Presentation.MemberAccessUtil.DemandWriteOnce(System.Boolean,System.String,System.Object,System.String,System.String)")]
		public IObjectBM BusinessObject {
			get{return m_BusinessObject;}
			set {
				MemberAccessUtil.DemandWriteOnce(m_BusinessObject==null,"Cannot set a metadata property once it is applied to a business value property operation.",this,"ObjectBM","{158703AC-3BC0-4CAE-AC88-ED6B3F9456E1}");
				m_BusinessObject = value;
				SetParentPattern.Execute(ref m_BusinessObject, value,"ObjectBM");
				//
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object IParentSupport.Parent {
			get {return BusinessObject;}
			set {
				BusinessObject=(IObjectBM) value;
				EventUtil.Raise(m_ParentChanged,this,EventArgs.Empty,"{F0954034-EBCA-412E-BE2A-38758FCA1FE9}");
				EventManager.Raise<EventHandler,EventArgs>(m_LazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		IObjectBM IParentSupport<IObjectBM>.Parent {get {return BusinessObject;}set {BusinessObject=value;}}

		
		event EventHandler IParentSupport.ParentChanged {add { m_ParentChanged += value; }remove { m_ParentChanged -= value; }}

		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); } }

		event EventHandler IParentSupport<IObjectBM>.ParentChanged {add { m_ParentChanged += value; }remove { m_ParentChanged -= value; }}

		/// <summary> Gets or sets the data provider.
		/// </summary>
		/// <value>The data provider.</value>
		public IDataProvider DataProvider {
			get {
				if(m_DataProvider==null) {
					Debug.WriteLine("=>WARNING: DataProvider not specified!" +
					"\r\n\t"+"ObjectBM: "+(m_BusinessObject!=null ? m_BusinessObject.GetType().FullName : "{null}")+
					(m_BusinessObject !=null ? ("\r\n\t"+"ObjectBM.Name: "+(m_BusinessObject.MemberName ?? "{null}")):"")
					);
				}
				return m_DataProvider;
			}
			set {
				DemandWrite();
				m_DataProvider = value;
			}
		}

		protected Lazy<EventSourceStore> LazyWeakEventProperties { get { return m_LazyWeakEventProperties; } }

		protected EventSourceStore EventSources { get { return m_LazyWeakEventProperties.Value; } }

		/// <summary> Demands a write operation. 
		/// If a write operation is not allowed a <see cref="InvalidOperationException"/> is throwed.
		/// </summary>
		protected void DemandWrite() { 
			if(BusinessObject!=null) throw new InvalidOperationException("Cannot set a metadata property once it is applied to a business value property operation.");
		}
	}

	/// <summary> Provides metadata for a business object property, specifically adding framework-specific property system characteristics.
	/// </summary>
	public class BusinessObjectMetadata:BusinessMetadata {

	}


	/// <summary>
	/// 
	/// </summary>
	public delegate Exception ValidateValueCallback(IValueBM businesssValue, object value);
	//public delegate ValidateValueCallback(IValueBM businessValue, ValidateBusinessValueEventArgs e);

	/// <summary> Represents the callback that is invoked when the effective property value of a IValueBM changes.
	/// </summary>
	/// <param name="businessValue">The IValueBM on which the value has changed.</param>
	/// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
	public delegate void BusinessValueChangedCallback(IValueBM businessValue, BusinessValueChangedEventArgs e);

	/// <summary>
	/// 
	/// </summary>
	public delegate object CoerceValueCallback(IValueBM businessValue, object baseValue);

	/// <summary> Provides arguments for the BusinessValueChanged-event
	/// </summary>
	public class BusinessValueChangedEventArgs:EventArgs {

		private readonly object m_PreviousValue;
		private readonly object m_NewValue;

		/// <summary> Initializes a new instance of the <see cref="BusinessValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousValue">The previous value.</param>
		/// <param name="newValue">The new value.</param>
		public BusinessValueChangedEventArgs(object previousValue, object newValue) {
			m_PreviousValue = previousValue;
			m_NewValue = newValue;
		}

		/// <summary> Gets the previous value.
		/// </summary>
		/// <value>The previous value.</value>
		public object PreviousValue{get {return m_PreviousValue;}}

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value.</value>
		public object NewValue{get {return m_NewValue;}}
	}
}
