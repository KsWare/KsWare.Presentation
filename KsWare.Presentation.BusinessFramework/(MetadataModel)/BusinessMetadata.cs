using System;
using System.Diagnostics;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {

	//REVIEW make BusinessMetadata abstract

	/// <summary> Provides command metadata for a business object property, specifically adding framework-specific property system characteristics.
	/// </summary>
	public class BusinessMetadata:IMetadata,IParentSupport, IParentSupport<IObjectBM> {

		private Lazy<EventSourceStore> _lazyWeakEventProperties;
		private IObjectBM _businessObject;
		private IDataProvider _dataProvider;
		private EventHandler _parentChanged;

		public BusinessMetadata() {
			_lazyWeakEventProperties=new Lazy<EventSourceStore>(()=>new EventSourceStore(this));
		}

		/// <summary> Gets the ObjectBM which owns this metadata
		/// </summary>
		/// <remarks>
		/// After setting businessObject to value other as null all meta data properties will be write protected.
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "KsWare.Presentation.MemberAccessUtil.DemandWriteOnce(System.Boolean,System.String,System.Object,System.String,System.String)")]
		public IObjectBM BusinessObject {
			get{return _businessObject;}
			set {
				MemberAccessUtil.DemandWriteOnce(_businessObject==null,"Cannot set a metadata property once it is applied to a business value property operation.",this,"ObjectBM","{158703AC-3BC0-4CAE-AC88-ED6B3F9456E1}");
				_businessObject = value;
				SetParentPattern.Execute(ref _businessObject, value,"ObjectBM");
				//
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object IParentSupport.Parent {
			get {return BusinessObject;}
			set {
				BusinessObject=(IObjectBM) value;
				EventUtil.Raise(_parentChanged,this,EventArgs.Empty,"{F0954034-EBCA-412E-BE2A-38758FCA1FE9}");
				EventManager.Raise<EventHandler,EventArgs>(_lazyWeakEventProperties,"ParentChangedEvent", EventArgs.Empty);
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		IObjectBM IParentSupport<IObjectBM>.Parent {get {return BusinessObject;}set {BusinessObject=value;}}

		
		event EventHandler IParentSupport.ParentChanged {add { _parentChanged += value; }remove { _parentChanged -= value; }}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler> ParentChangedEvent { get { return EventSources.Get<EventHandler>("ParentChangedEvent"); } }

		event EventHandler IParentSupport<IObjectBM>.ParentChanged {add { _parentChanged += value; }remove { _parentChanged -= value; }}

		/// <summary> Gets or sets the data provider.
		/// </summary>
		/// <value>The data provider.</value>
		public IDataProvider DataProvider {
			get {
				if(_dataProvider==null) {
					Debug.WriteLine("=>WARNING: DataProvider not specified!" +
					"\r\n\t"+"ObjectBM: "+(_businessObject!=null ? _businessObject.GetType().FullName : "{null}")+
					(_businessObject !=null ? ("\r\n\t"+"ObjectBM.Name: "+(_businessObject.MemberName ?? "{null}")):"")
					);
				}
				return _dataProvider;
			}
			set {
				DemandWrite();
				_dataProvider = value;
			}
		}

		protected Lazy<EventSourceStore> LazyWeakEventProperties { get { return _lazyWeakEventProperties; } }

		protected EventSourceStore EventSources { get { return _lazyWeakEventProperties.Value; } }

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

		private readonly object _PreviousValue;
		private readonly object _NewValue;

		/// <summary> Initializes a new instance of the <see cref="BusinessValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousValue">The previous value.</param>
		/// <param name="newValue">The new value.</param>
		public BusinessValueChangedEventArgs(object previousValue, object newValue) {
			_PreviousValue = previousValue;
			_NewValue = newValue;
		}

		/// <summary> Gets the previous value.
		/// </summary>
		/// <value>The previous value.</value>
		public object PreviousValue{get {return _PreviousValue;}}

		/// <summary> Gets the new value.
		/// </summary>
		/// <value>The new value.</value>
		public object NewValue{get {return _NewValue;}}
	}
}
