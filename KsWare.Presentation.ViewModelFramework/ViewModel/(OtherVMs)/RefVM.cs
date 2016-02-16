using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	public interface IRefVM:IObjectVM {

		/// <summary> Gets or sets the object (the target) referenced by the current <see cref="IRefVM"/> object.
		/// </summary>
		object Target { get; set; }

		/// <summary> Occurs when <see cref="Target"/> changed.
		/// </summary>
		[Obsolete("Use TargetChangedEvent")]
		event EventHandler TargetChanged;

		/// <summary> Gets the event source for the event which occurs when <see cref="Target"/> changed.
		/// </summary>
		IEventSource TargetChangedEvent { get; }
	}

	public interface IRefVM<TRef>:IObjectVM where TRef : class, IObjectVM {

		/// <summary> Gets or sets the object (the target) referenced by the current <see cref="IRefVM"/> object.
		/// </summary>
		TRef Target { get; set; }

		/// <summary> Occurs when <see cref="Target"/> changed.
		/// </summary>
		[Obsolete("Use TargetChangedEvent")]
		event EventHandler TargetChanged;

		/// <summary> Gets the event source for the event which occurs when <see cref="Target"/> changed.
		/// </summary>
		IEventSource<EventHandler<ValueChangedEventArgs<TRef>>> TargetChangedEvent { get; }
	}

	//TODO use MetadataAttribute/Itemtemplate to initialize new instances of TRef

	/*\ 
	 * Linked behavior (Data is IRefBM or IRefVM):
	 * RefVM<Int32VM>, Data=RefBM<Int32BM>
	 * RefVM<Int32VM>, Data=RefVM<Int32VM>
	 * 
	 * a.Data=b
	 * When the a.Target changes: the data of the b.Target will be the data of the a.Target
	 * b.Target=a.Target.Data
	 * When the b.Target changes: the a.Target will be a new instance with b.Target as data
	 * a.Target=new A{Data=b}
	 * 
	\*/

	/// <summary> Provides a reference for a view model object
	/// </summary>
	/// <typeparam name="TRef">The type of the reference</typeparam>
	public class RefVM<TRef> : ObjectVM, IRefVM<TRef>,IRefVM where TRef : class, IObjectVM {

		private TRef m_ItemTemplate;
		private TRef m_Target;

		/// <summary> Initializes a new instance of the <see cref="RefVM{TRef}" /> class.
		/// </summary>
		public RefVM() {
			
		}

		/// <summary> Gets or sets the metadata for this object.
		/// </summary>
		/// <value>The metadata.</value>
		/// <remarks>The metadata provides additional configuration for this view model object</remarks>
		public new ReferenceViewModelMetadata Metadata{get { return (ReferenceViewModelMetadata) base.Metadata; } set { base.Metadata = value; }}

		protected override ViewModelMetadata CreateDefaultMetadata() {
			var businessType = BusinessObjectVM.GetBusinessType(typeof (TRef));
			if(businessType==null) return new ReferenceViewModelMetadata();

			//var tbref=typeof (RefBM<>).MakeGenericType(businessType);
			//var tmeta = typeof (BusinessObjectMetadata<>).MakeGenericType(tref);
			var tmeta = typeof (BusinessReferenceViewModelMetadata<,>).MakeGenericType(typeof(TRef), businessType);
			var metadata = (ViewModelMetadata)Activator.CreateInstance(tmeta);

			return metadata;
		}

		/// <summary> Gets or sets the object (the target) referenced by the current <see cref="RefVM{TRef}"/> object.
		/// </summary>
		/// <remarks>
		/// If Target is set to the same value/reference as before, no changes will occur.
		/// </remarks>
		public TRef Target {
			get { return m_Target; }
			set {
				if(Equals(m_Target,value)) return;
				var e=new ValueChangingEventArgs<TRef>(m_Target,value);
				EventUtil.Raise(TargetChanging,this,e,"{ABE618BB-DAC8-4557-9BA9-0BFA5116D5E8}" );
				//WeakEventManager.Raise(LazyWeakEventProperties,()=>TargetChangingEvent, e);
				var prevTarget=m_Target;
				var prevHasTarget = HasTarget;
				m_Target = value;

				#region Sync with Data/BusinessObject
				var data =  Metadata.DataProvider.Data;
				if (data == null) {
					
				}else if(data is IRefBM) {
					((IRefBM)data).Target = m_Target==null?null:((IObjectBM) m_Target.Metadata.DataProvider.Data);
				} else if (data is IRefVM) {
					((IRefVM) data).Target = m_Target == null ? null : ((IObjectVM) m_Target.Metadata.DataProvider.Data);
				} else {
					throw new NotImplementedException("{30E67041-CBE5-4674-9695-BC36982C36C7}");
				}
				#endregion

				var ea=new ValueChangedEventArgs<TRef>(prevTarget,m_Target);
				EventUtil.Raise(TargetChanged,this,ea,"{0ABD9487-DD5C-45A1-B997-A4E5C3188320}" );
				EventManager.Raise<EventHandler<ValueChangedEventArgs<TRef>>,ValueChangedEventArgs<TRef>>(LazyWeakEventStore,"TargetChangedEvent", ea);
				OnPropertyChanged("Target");
				if(prevHasTarget!=HasTarget) OnPropertyChanged("HasTarget");
			}
		}

		/// <summary> Gets a value indicating whether <see cref="Target"/> is not null.
		/// </summary>
		/// <value><c>true</c> if <see cref="Target"/> is not null; otherwise, <c>false</c>.</value>
		public bool HasTarget { get { return m_Target != null; } }

		object IRefVM.Target {get { return Target; }set { Target = (TRef) value; }}

		/// <summary> [EXPERIMENTAL] Gets or sets the item template.
		/// </summary>
		/// <value>The item template.</value>
		/// <exception cref="System.ArgumentNullException">The template must not be null!</exception>
		/// <remarks>The item template is used when the underlying/linked <see cref="DataProvider.Data"/> changes its target</remarks>
		public TRef ItemTemplate {
			get {
				if(m_ItemTemplate==null) m_ItemTemplate=Metadata.NewItemProvider.CreateItem<TRef>(null);
				return m_ItemTemplate;
			}
			set {
				if(value==null) throw new ArgumentNullException("value");
				m_ItemTemplate=value;
				OnPropertyChanged("ItemTemplate");
			}
		}
		
		/// <summary> Occurs before <see cref="Target"/> changed.
		/// </summary>
		public event EventHandler<ValueChangingEventArgs> TargetChanging;

		/// <summary> Occurs when <see cref="Target"/> changed.
		/// </summary>
		[Obsolete("Use TargetChangedEvent")]
		public event EventHandler TargetChanged;


		/// <summary> Gets the event source for the event which occurs when <see cref="Target"/> changed.
		/// </summary>
		public IEventSource<EventHandler<ValueChangedEventArgs<TRef>>> TargetChangedEvent {
			get { return EventSources.Get<EventHandler<ValueChangedEventArgs<TRef>>>("TargetChangedEvent"); }
		}
		
		IEventSource IRefVM.TargetChangedEvent { get { return TargetChangedEvent; } }

		[Obsolete("Use TargetChangedEvent")]
		public EventHandler EːTargetChanged {set { TargetChanged+=value; }}

		/// <summary> [EXPERIMENTAL] Gets or sets the previous target.
		/// </summary>
		/// <value> The previous target. </value>
		/// <remarks>By default, this property is not managed by this view model. It can be used to store the previous target with own logic.</remarks>
		[Bindable(false)]
		public TRef PreviousTarget { get; set; }


		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);

			//TODO: only IRefBM/IRefVM is implemented
			if(e.NewData!=null) {
				if(e.NewData is IRefBM) {
					var bm = (IRefBM) e.NewData;
					bm.TargetChanged+=AtBusinessTargetChanged;
					EventUtil.Register(this, "{65AA2714-2E1C-44F1-ADEB-A7CB7B5F668C}", bm);
					AtBusinessTargetChanged(null, null);
				} else if(e.NewData is IRefVM) {
					var vm = (IRefVM) e.NewData;
					vm.TargetChanged+=AtViewModelTargetChanged;
					EventUtil.Register(this, "{65AA2714-2E1C-44F1-ADEB-A7CB7B5F668C}", vm);
					AtViewModelTargetChanged(null, null);
				} else {
					throw new NotImplementedException("{BC673A31-6787-42EB-AFE9-4367C878CAE6}");
				}
			} else {
				var o=EventUtil.Release(this, "{65AA2714-2E1C-44F1-ADEB-A7CB7B5F668C}");
				if(o is IRefBM) ((IRefBM) o).TargetChanged -= AtBusinessTargetChanged;
				if(o is IRefVM            ) ((IRefVM            ) o).TargetChanged -= AtViewModelTargetChanged;
			}

		}

		/// <summary> Occurs when the target of underlaying business model changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void AtBusinessTargetChanged(object sender, EventArgs e) {
			var bm = (IRefBM) Metadata.DataProvider.Data;

//			var oldTarget = _target;
			if(bm.Target==null){
				Target=null;
			}else {
				var bmTarget = bm.Target;
				TRef vmTarget = null;

				//NEW 2014-05-18 using SourceList 
				if (Metadata.ValueSourceProvider.SourceList != null) {
					var a = Metadata.ValueSourceProvider.SourceList.OfType<TRef>();
					vmTarget = a.FirstOrDefault(x => ReferenceEquals(x.Metadata.DataProvider.Data, bmTarget));
				}

				if (vmTarget == null) {
					vmTarget = Metadata.NewItemProvider.CreateItem<TRef>(bm.Target);
				}
				Target =vmTarget;
			}			
		}

		/// <summary> Occurs when the target of underlaying view model changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void AtViewModelTargetChanged(object sender, EventArgs e) {
			var vm = (IRefVM) Metadata.DataProvider.Data;

//			var oldTarget = _target;
			if(vm.Target==null){
				Target=null;
			}else{
				var bmTarget = vm.Target;
				object vmTarget = null;

				//NEW 2014-05-18 using SourceList 
				if (Metadata.ValueSourceProvider.SourceList != null) {
					var a = Metadata.ValueSourceProvider.SourceList.OfType<TRef>();
					vmTarget = a.FirstOrDefault(x => ReferenceEquals(x.Metadata.DataProvider.Data, bmTarget));
				}

				if (vmTarget == null) {
					vmTarget = Metadata.NewItemProvider.CreateItem<TRef>(bmTarget);
				}
				Target = (TRef) vmTarget;
			}			
		}

		/// <summary> [EXPERIMENTAL] Gets or sets the list of available targets.
		/// </summary>
		/// <value>The list of available targets</value>
		/// <remarks>uses the ValueSourceProvider: <c>Metadata.ValueSourceProvider.SourceList</c></remarks>
		public IList<TRef> MːSourceList { get { return (IList<TRef>) Metadata.ValueSourceProvider.SourceList; } set{Metadata.ValueSourceProvider.SourceList=value;} }
	}

	/// <summary> Provides a metadata implementation for references (RefVM) 
	/// </summary>
	public class ReferenceViewModelMetadata : ViewModelMetadata {

		private INewItemProvider m_NewItemProvider;

		/// <summary> Gets or sets the "NewItemProvider".
		/// </summary>
		/// <value>The "NewItemProvider".</value>
		/// <remarks>
		/// The NewItemProvider is provider which provides a method to create a new item. This allows to change the logic how the item is created, w/o to write a new ListVM.
		/// </remarks>
		public INewItemProvider NewItemProvider {
			get {
				if(m_NewItemProvider==null) {
//LOG				Debug.WriteLine("=>WARNING: Create default NewItemProvider!");
					m_NewItemProvider=new DefaultNewItemProvider();
				}
				return m_NewItemProvider;
			}
			set {
				DemandPropertySet();
				m_NewItemProvider = value;
			}
		}
	}

	/// <summary> Provides meta data for a <see cref="RefVM{TRef}"/> with underlying <see cref="RefBM{TRef}"/>
	/// </summary>
	/// <typeparam name="TRef">Type of RefVM{T}-item</typeparam>
	/// <typeparam name="TBusinessRef">Type of RefBM{T}-item</typeparam>
	public class BusinessReferenceViewModelMetadata<TRef,TBusinessRef>
				          :ReferenceViewModelMetadata 
		where TRef        :class,IObjectVM,new()
		where TBusinessRef:class,IObjectBM/*,new()*/
	{
		/// <summary> Initializes a new instance of the <see cref="BusinessReferenceViewModelMetadata{TRef,TBusinessRef}"/> class.
		/// </summary>
		public BusinessReferenceViewModelMetadata() {
			DataProvider = new LocalDataProvider<RefBM<TBusinessRef>>();
			NewItemProvider=new CustomNewItemProvider(CreateNewItemCallback);
		}

		private object CreateNewItemCallback(object data) {
			//NewViewModel = delegate { return new StringVM{Metadata = new BusinessValueMetadata<string>()}; }

			var vm = (TRef)Activator.CreateInstance<TRef>();
			if(ValueVM.IsValueVM(typeof(TRef))) vm.Metadata = CreateBusinessValueMetadata(typeof (TBusinessRef));
			else vm.Metadata = new BusinessObjectMetadata<TBusinessRef>();
			vm.Metadata.DataProvider.Data = data;
			return vm;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		private ViewModelMetadata CreateBusinessValueMetadata(Type businessType) {
			var dataType = ValueVM.GetDataType(businessType);
			var metadataType=typeof (BusinessValueMetadata<>).MakeGenericType(dataType);
			var metaData = (ViewModelMetadata)Activator.CreateInstance(metadataType);
			return metaData;
		}

//		/// <summary> Gets or sets the data provider.
//		/// </summary>
//		/// <value>The data provider.</value>
//		/// <remarks></remarks>
//		public new BusinessObjectDataProvider<RefBM<TBusinessRef>> DataProvider{
//			get {
//				return (BusinessObjectDataProvider<RefBM<TBusinessRef>>) base.DataProvider;
//			}
//		}
	}


	public class NullableVM<T> : RefVM<T> where T : class, IObjectVM{

		//TODO seperate from RefVM
		// replace Target with Value and HasTarget with HasValue

		public NullableVM() {
			TargetChangedEvent.add= (s, e) => {
				if (e.PreviousValue != null) e.PreviousValue.Parent = null;
				if (e.NewValue      != null) e.NewValue     .Parent = this;
			};
		}


	}
}
