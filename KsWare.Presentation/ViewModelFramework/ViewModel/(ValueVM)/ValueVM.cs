using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Documentation;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core.Logging;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	//TODO why we need the ActionVM ErrorInfoAction?

	/// <summary> Interface for all ValueVMs
	/// </summary>
	/// <remarks></remarks>
	public interface IValueVM:IObjectVM {

		/// <summary> Gets a value indicating whether this instance has value.
		/// </summary>
		/// <remarks></remarks>
		bool HasValue{get;}

		/// <summary> Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		/// <remarks></remarks>
		object Value{get;set;}

		/// <summary> Tries to get the value.
		/// </summary>
		/// <param name="exception">The exception if failed; else <see langword="null"/></param>
		/// <value>The value or if failed <see langword="null"/> (or the default value of current type. dependent on implementation)</value>
		/// <remarks></remarks>
		object TryGetValue(out Exception exception); // ADDED: [xgksc 2013-01-25] 

		/// <summary> Gets the type of the value.
		/// </summary>
		/// <remarks></remarks>
		Type ValueType{get;}

		/// <summary> Occurs when value has been changed.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Use ValueChangedEvent")]
		event EventHandler<ValueChangedEventArgs> ValueChanged;

		/// <summary> Gets the event source for the event which occurs when <see cref="Value"/> changed.
		/// </summary>
		/// <value>The value changed event source.</value>
		IEventSource<EventHandler<ValueChangedEventArgs>> ValueChangedEvent { get; }

		/// <summary> Gets the display value provider.
		/// </summary>
		/// <remarks></remarks>
		IDisplayValueProvider DisplayValueProvider{get;}

		/// <summary> Gets the edit value provider.
		/// </summary>
		/// <remarks></remarks>
		IEditValueProvider EditValueProvider{get;}

		/// <summary> Gets the value source provider.
		/// </summary>
		/// <remarks></remarks>
		IValueSourceProvider ValueSourceProvider{get;}

		ActionVM ErrorInfoAction { get; set; }

	}
	
	/// <summary> Generic interface for all ValueVMs
	/// </summary>
	/// <remarks></remarks>
	public interface IValueVM<T>:IObjectVM {

		/// <summary> Gets a value indicating whether this instance has value.
		/// </summary>
		/// <remarks></remarks>
		bool HasValue{get;}

		/// <summary> Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		/// <remarks></remarks>
		T Value{get;set;}

		/// <summary> Tries to get the value.
		/// </summary>
		/// <param name="exception">The exception if failed; else <see langword="null"/></param>
		/// <value>The value or if failed the default value of current type.</value>
		/// <remarks></remarks>
		T TryGetValue(out Exception exception); // ADDED: [xgksc 2013-01-25] 

		/// <summary> Occurs when value has been changed.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Use ValueChangedEvent")]
		event EventHandler<ValueChangedEventArgs> ValueChanged;

		IEventSource<EventHandler<ValueChangedEventArgs>> ValueChangedEvent { get; }

		/// <summary> Gets the display value provider.
		/// </summary>
		/// <remarks></remarks>
		IDisplayValueProvider DisplayValueProvider{get;}

		/// <summary> Gets the edit value provider.
		/// </summary>
		/// <remarks></remarks>
		IEditValueProvider EditValueProvider{get;}

		/// <summary> Gets the value source provider.
		/// </summary>
		/// <remarks></remarks>
		IValueSourceProvider ValueSourceProvider{get;}

		ActionVM ErrorInfoAction { get; set; }

	}

	/// <summary> Generic ValueVM. Base class for all value view models
	/// </summary>
	/// <typeparam name="T">Type of the value</typeparam>
	/// <remarks></remarks>
	public partial class ValueVM<T>:ObjectVM,IValueVM<T>,IValueVM {
		
		private T m_CachedValue;
		private T m_PreviousRaisedValue=default(T);
		private bool m_EnableBusinessModelFeatures;
		private WeakReference m_WeakLastBusinessObject;

		/// <summary> Initializes a new instance of the <see cref="ValueVM{T}"/> class.
		/// </summary>
		public ValueVM() {
			base.DebuggerFlags=new ClassDebuggerFlags();
			//ErrorInfoAction = RegisterChild("ErrorInfoAction", new ActionVM());
		}

		/// <summary> Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ValueVM{T}"/> is reclaimed by garbage collection.
		/// </summary>
		~ValueVM(){Dispose(false);}

		/// <summary> Occurs when <see cref="Value"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		[Obsolete("Use ValueChangedEvent")]
		public event EventHandler<ValueChangedEventArgs> ValueChanged;

		public IEventSource<EventHandler<ValueChangedEventArgs>> ValueChangedEvent {
			get { return EventSources.Get<EventHandler<ValueChangedEventArgs>>("ValueChangedEvent"); }
		}

		protected override ViewModelMetadata CreateDefaultMetadata() {
			 if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var attr = propertyInfo.GetCustomAttributes(typeof(ViewModelMetadataAttribute),true).Cast<ViewModelMetadataAttribute>().FirstOrDefault();
					if (attr != null) {
						if (attr.MetadataType != null) {
							var metaData = (ViewModelMetadata)Activator.CreateInstance(attr.MetadataType);
							return metaData;
						}				        
					}
				}
			}
			var metadata=new ValueMetadata<T>();
			if (m_EnableBusinessModelFeatures) metadata.EnableBusinessModelFeatures = true;
			return metadata;
		}

		public ActionVM ErrorInfoAction { get; set; }

		/// <summary> Gets or sets the <see cref="docːObjectVM.underlyingˑdata"/> (Alias for Metadata.DataProvider.Data)
		/// </summary>
		/// <value>The <see cref="docːObjectVM.underlyingˑdata"/>.</value>
		public override object MːData {
			get {return Metadata.DataProvider.Data;}
			set {
				if (value is IValueBM) MːBusinessObject = (IValueBM) value; 
				else Metadata.DataProvider.Data=value;
			}
		}


		/// <summary>  [EXPERIMENTAL] Gets or set the underlying business object
		/// </summary>
		/// <value>The underlying business object or <c>null</c>.</value>
		public override IObjectBM MːBusinessObject {
			get {
				if (!HasMetadata) return null;
				if (!Metadata.HasDataProvider) return null;
				var provider=Metadata.DataProvider as IBusinessValueDataProvider;
				return provider==null ? null : provider.BusinessValue;
			}
			set {
				BusinessValueDataProvider<T> provider;
				if      (!HasMetadata             ) m_EnableBusinessModelFeatures = true;
				else if (!Metadata.HasDataProvider) Metadata.EnableBusinessModelFeatures = true;
				
				if (Metadata.DataProvider is BusinessValueDataProvider<T>) provider = (BusinessValueDataProvider<T>) Metadata.DataProvider;
				else if (!Metadata.DataProvider.IsAutoCreated) throw new InvalidOperationException("DataProvider is no BusinessValueDataProvider");
				else Metadata.ChangeDataProvider(provider= new BusinessValueDataProvider<T>());
				provider.BusinessValue = (IValueBM<T>) value;

				//TODO verify this
				var p2 = Metadata.ValueSourceProvider as BusinessValueSourceProvider;
				if (p2 != null) p2.BusinessValue = (IValueBM) value;
			}
		}

		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);

			if (e.NewData == null) {
				//TODO REVISE e.NewData == null
				//HasValue=false;
				return;
			}

			RaiseValueChanged((T)e.NewData);

//			((IErrorProviderWriter)ErrorProvider).ResetError();

			#region (optional) DataProvider-Validate);
			if (Metadata.DataProvider != null && Metadata.DataProvider.IsSupported) {
				try {
					Metadata.DataProvider.Validate(m_CachedValue);
					((IErrorProviderController)ErrorProvider).ResetError();
				}
				catch (Exception ex) {
					((IErrorProviderController)ErrorProvider).SetError(ex.Message);//TODO localize message
				}
			}
			#endregion
		}

		protected override void OnDataProviderChanged(ValueChangedEventArgs<IDataProvider> e) {
			base.OnDataProviderChanged(e);
			if (e.NewValue is IBusinessValueDataProvider) {
				var provider = (IBusinessValueDataProvider) e.NewValue;
				provider.BusinessValueChanged += (s1, e1) => {
					var lastBusinessObject=m_WeakLastBusinessObject == null ? null : (!m_WeakLastBusinessObject.IsAlive ? null : m_WeakLastBusinessObject.Target);
					m_WeakLastBusinessObject = e1.NewData == null ? null : new WeakReference(e1.NewData);
					OnBusinessObjectChanged(new ValueChangedEventArgs<IObjectBM>((IObjectBM)lastBusinessObject, (IObjectBM) e1.NewData));
				};
			}
		}

		/// <summary> Called when underlying business object changed.
		/// </summary>
		/// <param name="e">The arguments for the value changed event.</param>
		/// <remarks>If the prevoius business object is disposed (not more alive) then <c>null</c> is returned by <see cref="ValueChangedEventArgs{T}.PreviousValue"/></remarks>
		protected virtual void OnBusinessObjectChanged(ValueChangedEventArgs<IObjectBM> e) {
			
		}

		#region Provider properties

		/// <summary> Provides the display value
		/// </summary>
		public IDisplayValueProvider DisplayValueProvider{get {return Metadata.DisplayValueProvider;}}

		/// <summary> Provides the edit value
		/// </summary>
		public IEditValueProvider EditValueProvider{get {return Metadata.EditValueProvider;}}

		/// <summary> Provides a list of valid values
		/// </summary>
		/// <remarks>
		/// Used as ItemsSource 
		/// </remarks>
		public virtual IValueSourceProvider ValueSourceProvider{get {return Metadata.ValueSourceProvider;}}

		#endregion

		/// <summary> Sets the value changed callback handler. Should be used only for initializer syntax
		/// </summary>
		/// <value>The value changed callback.</value>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
		[Obsolete("Use ValueChangedEventAdd")]
		public EventHandler<ValueChangedEventArgs> ValueChangedAdd {set {ValueChanged += value;}}
		
		/// <summary> Sets the value changed callback handler. Should be used only for initializer syntax
		/// </summary>
		/// <value>The value changed callback.</value>
		public EventHandler<ValueChangedEventArgs> ValueChangedEventAdd {set {ValueChangedEvent.Register(this,"ValueChanged", value);}}

		/// <summary> Gets a value indicating whether this instance has a value.
		/// </summary>
		/// <remarks></remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		[Obsolete("Functionality not completely implemented! Implement on demand.")]
		public bool HasValue {
			get {
				#region Debug Breakpoint
				if(this.DebuggerFlags.Breakpoints.HasValueGet && Debugger.IsAttached) Debugger.Break();
				#endregion

				// if T is not a reference type always true is returned;  
				if(Equals(default(T), null)) {
					return true;
				} else {
					try{return Metadata.DataProvider.Data!=null;}
					catch (Exception ex) {
						string text = "=>WARNING: " + ex.Message + "\n\t" + "ErrorID: {63C2A2C3-298D-4161-B756-FA9D8C9DAD8D}";
						//Debug.WriteLine(text);
						this.Log(Flow.Output(text));
						//### Break if (Debugger.IsAttached) Debugger.Break(););
						return false;
					}
				}
			}
		}

		/// <summary> Gets the value of the current <see cref="ValueVM{T}"/> value. 
		/// </summary>
		/// <value>The value of the current <see cref="ValueVM{T}"/> object if the <see cref="HasValue"/> property is true. </value>
		public virtual T Value {
			get {
				Exception exception;
				var v=TryGetValue(out exception);
				if(exception!=null) throw exception;
				return v;
			}
			set {
				#region Debug Breakpoint
				if(this.DebuggerFlags.Breakpoints.ValueSet && Debugger.IsAttached) Debugger.Break(); 
				#endregion 
				//Debug.WriteLine("=>Trace: ValueVM.Value{set;} Name='"+MemberName+"' Value="+value);

				#region (optional) DataProvider-Validate);
				if(Metadata.DataProvider!=null && Metadata.DataProvider.IsSupported) {
					try {
						var exception= Metadata.DataProvider.Validate(value);
						if(exception!=null){
							if(Metadata.ErrorProvider!=null && Metadata.ErrorProvider.IsSupported)
								((IErrorProviderController)ErrorProvider).SetError(exception.Message);//TODO localize message
						}else{
							if(Metadata.ErrorProvider!=null && Metadata.ErrorProvider.IsSupported)
								((IErrorProviderController)ErrorProvider).ResetError();
						}
					} catch (Exception ex) {
						if(Metadata.ErrorProvider!=null && Metadata.ErrorProvider.IsSupported)
							((IErrorProviderController)ErrorProvider).SetError(ex.Message);//TODO localize message
						this.Log(Flow.Output("ERROR: Could not set value! Validation failed."));
						//### Break 
						if(Debugger.IsAttached) Debugger.Break();
						// throw;
					}
				}
				#endregion

				if (Equals(m_CachedValue, value)) return;
				m_CachedValue = value;

				if (Metadata.DataProvider!=null && Metadata.DataProvider.IsSupported) {
					#region (optional) DataProvider-SetData
					try {
						Metadata.DataProvider.Data=value;
						if(Metadata.ErrorProvider!=null && Metadata.ErrorProvider.IsSupported)
							((IErrorProviderController)ErrorProvider).ResetError();
					} catch (Exception ex) {
						if(Metadata.ErrorProvider!=null && Metadata.ErrorProvider.IsSupported)
							((IErrorProviderController)ErrorProvider).SetError(ex.Message);//TODO localize message
						this.Log(Flow.Output("ERROR: Could not set value! Forbidden."));
						//### Break
						if(Debugger.IsAttached) Debugger.Break();
						// throw;
					}
					#endregion
				} else {
					throw new NotImplementedException("{EB5466A5-492F-417B-90A3-5BA0D3B8B16A}");
				}
			}
		}

		/// <summary> Tries to get the value.
		/// </summary>
		/// <param name="exception">The exception if failed; else <see langword="null"/></param>
		/// <value>The value or if failed the default value of current type.</value>
		/// <remarks></remarks>
		public T TryGetValue(out Exception exception) {
			#region Debug Breakpoint
			if(this.DebuggerFlags.Breakpoints.ValueGet && Debugger.IsAttached) Debugger.Break();
			#endregion

			exception = null;
			if(!HasMetadata) return default(T);
			if(!Metadata.HasDataProvider) return default(T);
			if(!Metadata.DataProvider.IsSupported) return default(T);
			
			var data = Metadata.DataProvider.TryGetData(out exception);
			if(exception!=null) return default(T);
			if(data==null) return default(T);

			try {
				T dataT = (T) (data);
				if(!Equals(m_CachedValue,data)){/*TODO if(Debugger.IsAttached) Debugger.Break();*//*DEBUG uups*/}
				m_CachedValue = dataT;
				return m_CachedValue;
			}catch (Exception ex) {
				string text = "=>" + ex.Message + "\r\n\t" + "UniqueID: {8F538E40-842F-4151-BA12-75DEB4BE6B77}";
				//Debug.WriteLine(text);
				this.Log(Flow.Output(text));
				//### Break if (Debugger.IsAttached) Debugger.Break();
				return default(T);
			}	
		}

		object IValueVM.TryGetValue(out Exception exception ){return TryGetValue(out exception);}

		#region IValueVM

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		Type IValueVM.ValueType{get {return typeof(T);}}

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object IValueVM.Value{get {return Value;}set {Value = (T) value;}}

		#endregion

		public override string ToString() {
			return string.Format("{{Value: {0} {{{1}<{2}>}}}}", Value, this.GetType().Name, typeof(T).Name);
		}
		
		private void RaiseValueChanged(T newValue) {
			OnPropertyChanged("HasValue");
			OnPropertyChanged("Value");

			var args=new ValueChangedEventArgs(m_PreviousRaisedValue,newValue);
			EventUtil.Raise(ValueChanged,this,args,"{B86ED179-8BE0-4702-A352-5E77A884195C}");
			EventManager.Raise<EventHandler<ValueChangedEventArgs>,ValueChangedEventArgs>(LazyWeakEventStore,"ValueChangedEvent", args);
			m_PreviousRaisedValue = newValue;
		}


		#region Debug

		/// <summary> Gets the debugger flags.
		/// </summary>
		/// <value>The debugger flags.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
		public new ClassDebuggerFlags DebuggerFlags{get { return (ClassDebuggerFlags) base.DebuggerFlags; }}


		/// <summary> Provides flags for class debugging
		/// </summary>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public new class ClassDebuggerFlags:ObjectVM.ClassDebuggerFlags
		{

			/// <summary> Initializes a new instance of the <see cref="ValueVM&lt;T&gt;.ClassDebuggerFlags"/> class.
			/// </summary>
			/// <remarks></remarks>
			public ClassDebuggerFlags() {
				base.Breakpoints=new ClassDebuggerFlagsBreakpoints();
			}

			/// <summary> Gets the breakpoints.
			/// </summary>
			/// <remarks></remarks>
			public new ClassDebuggerFlagsBreakpoints Breakpoints{get { return (ClassDebuggerFlagsBreakpoints) base.Breakpoints; }}
		}

		/// <summary> provides availabe breakpoints for <see cref="ClassDebuggerFlags"/>
		/// </summary>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public new class ClassDebuggerFlagsBreakpoints:ObjectVM.ClassDebuggerFlagsBreakpoints
		{
			/// <summary> Gets or sets the Value{get} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Value{get} breakpoint; otherwise, <see langword="false"/> disable the Value{get} breakpoint.
			/// </value>
			public bool ValueGet{get;set;}

			/// <summary> Gets or sets the Value{get} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Value{set} breakpoint; otherwise, <see langword="false"/> disable the Value{set} breakpoint.
			/// </value>
			public bool ValueSet{get;set;}

			/// <summary> Gets or sets the Value{get} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the HasValue{get} breakpoint; otherwise, <see langword="false"/> disable the HasValue{get} breakpoint.
			/// </value>
			public bool HasValueGet{get;set;}
		}

		#endregion
	
	}	

	public class ValueMetadata: ViewModelMetadata {

		private Type m_ValueType;

		protected ValueMetadata(Type valueType) { m_ValueType = valueType; }

		protected override IDataProvider CreateDefaultDataProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var attr = propertyInfo.GetCustomAttributes(typeof(ViewModelMetadataAttribute),true).Cast<ViewModelMetadataAttribute>().FirstOrDefault();
					if (attr != null) {
						if (attr.DataProvider != null) {
							var provider = (IDataProvider)Activator.CreateInstance(attr.DataProvider);
							return provider;
						}				        
					}
				}
			}

			if (EnableBusinessModelFeatures) {
				var provider=(IDataProvider) Activator.CreateInstance(typeof(BusinessValueDataProvider<>).MakeGenericType(m_ValueType));
				return provider;
			}

			return new LocalDataProvider();
		}
	}

	public class ValueMetadata<T> : ValueMetadata {

		public ValueMetadata():base(typeof (T)) {
			
		}

	}

	public class ValueMetadataAttribute : ViewModelMetadataAttribute {

		public ValueMetadataAttribute() {}
		public ValueMetadataAttribute(Type metadataType) :base(metadataType) { }

	}

	public class LinkedValueMetadata : ViewModelMetadata {

		public LinkedValueMetadata() {
			BindingProvider=new ValueBindingProvider();
		}

	}
}
