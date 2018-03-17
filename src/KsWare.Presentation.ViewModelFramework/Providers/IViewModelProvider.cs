using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;
using KsWare.Presentation.Providers;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> Interface for all ViewModelProviders
	/// </summary>
	public interface IViewModelProvider:IProvider,INotifyPropertyChanged {

		/// <summary> Gets or sets the parent metadata (<see cref="IViewModelMetadata"/>) of this instance.
		/// </summary>
		/// <value>The parent metadata of this instance.</value>
		/// <remarks></remarks>
		new IViewModelMetadata Parent { get; set; }
	}

	/// <summary> Base class for ViewModelProvider
	/// </summary>
	public abstract class ViewModelProvider:IViewModelProvider {

		private Lazy<EventSourceStore> _lazyWeakEventStore;
		private IViewModelMetadata _parent;
		private bool? _isAutoCreated;

		protected ViewModelProvider() {
			_lazyWeakEventStore=new Lazy<EventSourceStore>(()=>new EventSourceStore(this));
		}

		protected Lazy<EventSourceStore> LazyWeakEventStore => _lazyWeakEventStore;
		protected EventSourceStore EventStore => _lazyWeakEventStore.Value;

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public abstract bool IsSupported{get;}

		public bool IsAutoCreated {
			get => _isAutoCreated ==true;
			set {
				MemberAccessUtil.DemandWriteOnce(!_isAutoCreated.HasValue,"The property can only be written once!",this,nameof(IsAutoCreated),"{8E2584E1-C321-4DD8-98F1-FEDC25B402FB}");
				_isAutoCreated = value;
			}
		}

		public bool IsInUse { get; set; }

		/// <summary> Gets or sets the parent metadata (<see cref="IViewModelMetadata"/>) of this instance.
		/// </summary>
		/// <value>The parent of this instance.</value>
		/// <remarks></remarks>
		public IViewModelMetadata Parent {
			[CanBeNull]
			get {return _parent;}
			[NotNull]
			set {
				MemberAccessUtil.DemandNotNull(value,"Parent cannot be null!",this,"{99788DA6-EB8A-44DF-974D-1E5CA567B9CF}");
				MemberAccessUtil.DemandWriteOnce(_parent==null,null,this,nameof(Parent),"{5B977C30-B3E4-4A50-8BAB-7A7EF8E789FA}");
				_parent = value;
				((ViewModelMetadata) _parent).ParentChanged+=AtMetadataParentChanged;
				OnParentChanged();
			}
		}

		object IParentSupport.Parent {get => Parent; set => Parent = (IViewModelMetadata)value; }

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ParentChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler> ParentChangedEvent => EventStore.Get<EventHandler>("ParentChangedEvent");

		/// <summary> Gets parent metadata (<see cref="IViewModelMetadata"/>) of this instance.
		/// </summary>
		/// <value>The parent metadata of this instance.</value>
		/// <remarks></remarks>
		protected IViewModelMetadata Metadata => Parent;

		/// <summary> Gets parent viewmodel (<see cref="IObjectVM"/>) of this instance.
		/// </summary>
		/// <value>The parent viewmodel of this instance.</value>
		/// <remarks></remarks>
//		protected IObjectVM ViewModel{get { return (IObjectVM) this.ʘ(o=>o.Parent).ʘ(o=>o.Parent); }}
		protected IObjectVM ViewModel => (IObjectVM) Parent?.Parent;

		/// <summary> Called when <see cref="Parent"/>-property has been changed. 
		/// This indicates provider has been assigned to an metadata object and all provider configuration properties are now read-only.
		/// Raises the <see cref="ParentChanged"/>-event.
		/// </summary>
		protected virtual void OnParentChanged() {
			EventUtil.Raise(ParentChanged, this, EventArgs.Empty, "{8AC4DF0E-2DCE-491E-A2CB-DA7AEA029A4A}");
			EventManager.Raise<EventHandler,EventArgs>(_lazyWeakEventStore,"ParentChangedEvent", EventArgs.Empty);
			OnPropertyChanged("Parent");
		}

		/// <summary> Called when <see cref="ViewModelMetadata"/>.<see cref="ViewModelMetadata.Parent"/>-property has been changed. 
		/// This indicates metadata has been assigned to an view model object and all metadata properties are now read-only.
		/// </summary>
		protected virtual void AtMetadataParentChanged(object sender, EventArgs e) { 
			
		}

		/// <summary> Raises the <see cref="PropertyChanged"/>-event.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected virtual void OnPropertyChanged(string propertyName) {
			OnPropertyChangedInternal(propertyName);
		}

		// _ => MyProperty
		protected virtual void OnPropertyChanged(Expression<Func<object, object>> propertyExpression) {
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			OnPropertyChangedInternal(memberName);
		}

		// () => MyProperty
		protected virtual void OnPropertyChanged(Expression<Func<object>> propertyExpression) {
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			OnPropertyChangedInternal(memberName);
		}

		private void OnPropertyChangedInternal(string propertyName) {
			EventUtil.Raise(PropertyChanged,this,new PropertyChangedEventArgs(propertyName),"{931FADE1-B6D5-4D35-827A-6988B893AA05}");
		}

		#region Implementation of INotifyPropertyChanged
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		
#endregion

		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak() {DebugUtil.Break();}

		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak(string message) {DebugUtil.Break(message);}

		public void Dispose() {Dispose(true); }

		protected virtual void Dispose(bool explicitDispose) {
			//TODO implement Dispose
		}
	}

	/// <summary> Abstract base class vor all providers used in ValueVM metadata
	/// </summary>
	public abstract class ViewModelValueProvider:ViewModelProvider {

		/// <summary> Gets the value view model object which holds this provider.
		/// </summary>
		/// <remarks>safe alias for: <c>this.Parent.Parent</c></remarks>
		protected new IValueVM ViewModel {[CanBeNull]get {return (IValueVM)base.ViewModel;}}

		/// <summary> Gets the overlying metadata (<see cref="ViewModelMetadata.Parent"/>) for this provider. 
		/// </summary>
		protected new ViewModelMetadata Metadata{[CanBeNull] get {return ((ViewModelMetadata) base.Metadata);}}

		/// <summary> Called when <see cref="ViewModelProvider.Parent"/>-property has been changed.
		/// This indicates provider has been assigned to an metadata object and all provider configuration properties are now read-only.
		/// Raises the <see cref="ViewModelProvider.ParentChanged"/>-event.
		/// </summary>
		/// <remarks></remarks>
		protected override void OnParentChanged() {
			base.OnParentChanged();

			if(Metadata!=null) {
				if( Metadata.Parent is IValueVM) RegisterValueChanged();
			} else {
				//wait for OnMetadataParentChanged
			}
		}

		/// <summary> Called when <see cref="ViewModelMetadata"/>.<see cref="ViewModelMetadata.Parent"/>-property has been changed.
		/// This indicates metadata has been assigned to an view model object and all metadata properties are now read-only.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks></remarks>
		protected override void AtMetadataParentChanged(object sender, EventArgs e) {
			base.AtMetadataParentChanged(sender, e);

			if(Metadata!=null) {
				if( Metadata.Parent is IValueVM) RegisterValueChanged();
			} else {
				//Uups
				throw new NotImplementedException("{B84BA14D-7244-480D-B71E-219E87E46441}");
			}
		}


		private void RegisterValueChanged() { 
			ViewModel.ValueChanged+=AtParentValueVMValueChanged;
		}

		/// <summary> Provides facilitating access to overlying ValueVM.ValueChanged event (using <c>this.Parent.Parent.ValueChanged</c>)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AtParentValueVMValueChanged(object sender, EventArgs e) {OnParentValueVMValueChanged();}

		/// <summary>
		/// Provides facilitating access to overlying ValueVM.ValueChanged event (using <c>this.Parent.Parent.ValueChanged</c>)
		/// </summary>
		protected virtual void OnParentValueVMValueChanged() {
			
		}
	}
}
