using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using KsWare.Presentation.Documentation;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Interface for business object
	/// </summary>
	public interface IObjectBM : IModel, IHierarchical<IObjectBM> {

		/// <summary> Gets or sets the metadata.
		/// </summary>
		/// <value>The metadata.</value>
		BusinessMetadata Metadata { get; set; }

		/// <summary> Occurs when user feedback is requested.
		/// </summary>
		event EventHandler<UserFeedbackEventArgs> UserFeedbackRequested;

		/// <summary> Gets the event source for the event which occurs when user feedback is requested.
		/// </summary>
		/// <value>The user feedback requested event.</value>
		/// <remarks>Not available in <see cref="ObjectSlimBM"> slim objects.</see> </remarks>
		/// [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IEventSource<EventHandler<UserFeedbackEventArgs>> UserFeedbackRequestedEvent { get; }

		/// <summary> Occurs when a business property has been changed.
		/// </summary>
		event EventHandler<BusinessPropertyChangedEventArgs> BusinessPropertyChanged;

		/// <summary> Occurs when the business tree has been changed.
		/// </summary>
		event EventHandler<TreeChangedEventArgs> TreeChanged;


		/// <summary> Requests a user feedback.
		/// </summary>
		/// <param name="args">The <see cref="KsWare.Presentation.UserFeedbackEventArgs"/> instance containing the event data.</param>
		void RequestUserFeedback(UserFeedbackEventArgs args);


		/// <summary> Gets a value indicating whether this business object is applicable/usable at present.
		/// </summary>
		/// <remarks></remarks>
		bool IsApplicable { get; }

		/// <summary> Occurs when <see cref="IsApplicable"/>-property has been changed.
		/// </summary>
		/// <remarks></remarks>
		event EventHandler IsApplicableChanged;


		/// <summary> Gets a value indicating whether this instance is a slim object.
		/// </summary>
		/// <value><c>true</c> if this instance is a slim object; otherwise, <c>false</c>.</value>
		/// <remarks> A slim object is a object with limited functionality. It has the same interface but the functionalty of some members are not implemented.</remarks>
		bool IsSlim { get; }
	}

	/// <summary> Business object
	/// </summary>
	public partial class ObjectBM : IObjectBM {

		private static bool? s_isInDesignMode;

		private BusinessMetadata _metadata;
		private readonly List<IDisposable> _disposableObjects = new List<IDisposable>();
		private readonly Lazy<EventSourceStore> _lazyWeakEventPropertyStore;
		private int _isDisposed;

		public ObjectBM() {
			Interlocked.Increment(ref StatisticsːNumberOfCreatedInstances);
			Interlocked.Increment(ref StatisticsːNumberOfInstances);

			_lazyWeakEventPropertyStore=new Lazy<EventSourceStore>(()=>new EventSourceStore(this));
			InitPartHierarchy();
			InitPartFields();
			InitPartReflection();
		}

		~ObjectBM() {
			Interlocked.Increment(ref StatisticsːMethodInvocationːDestructorːCount);
			Interlocked.Decrement(ref StatisticsːNumberOfInstances);
		}

		public bool IsSlim => false;

		protected Lazy<EventSourceStore> LazyWeakEventStore => _lazyWeakEventPropertyStore;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected EventSourceStore EventSources => _lazyWeakEventPropertyStore.Value;

		/// <summary> Creates the default metadata for the current type of business object .
		/// </summary>
		/// <returns>business object metadata</returns>
		protected virtual BusinessMetadata CreateDefaultMetadata() { return new BusinessObjectMetadata {DataProvider = new LocalDataProvider()}; }

		/// <summary> Gets or sets the business object metadata.
		/// </summary>
		/// <value>The business object metadata.</value>
		public BusinessMetadata Metadata {
			get {
			    if (_metadata == null) {
			        lock (this) {
			            if (_metadata == null) {
//LOG  						Debug.WriteLine("=>WARNING: Create default metadata! Type='"+this.GetType().NameT()+"', Name='"+this.MemberName+"', ParentType='"+(Parent!=null?Parent.GetType().Name:"")+"'", "ObjectBM");
			                Metadata = CreateDefaultMetadata();
			            }
			        }
			    }
			    return _metadata;
			}
			set {
			    //				Debug.WriteLine("=>ObjectBM: INFO: Set Metadata. "+(value==null?"{null}":value.GetType().FullName));
			    if (_metadata != null) throw new InvalidOperationException("Cannot set a metadata property once it is applied to a business value property operation.");
			    if (value == null) return;
			    OnMetadataChanging(value);
			    _metadata = value;
			    _metadata.BusinessObject = this; // Metadata is read-only now
			    OnMetadataChanged();
			}
		}

		/// <summary> Called before the <see cref="Metadata"/>-property is changed.
		/// </summary>
		/// <param name="newMetadata">The metadata.</param>
		protected virtual void OnMetadataChanging(BusinessMetadata newMetadata) {
			// <- Metadata{set;}
		}

		/// <summary> Called after the <see cref="Metadata"/>-property has been changed.
		/// </summary>
		protected virtual void OnMetadataChanged() {
			// <- Metadata{set;}

			//OPTIONAL: validations
			if (_metadata.DataProvider == null) Debug.WriteLine("=>ObjectBM: WARNING: No data provider specified");

			//OPTIONAL:	EventUtil.Raise(MetadataChanged,this, EventArgs.Empty,"{24FEDB67-B002-4844-A31A-80E8028D7E7F}");
			//OPTIONAL:	WeakEventManager.Raise(MetadataChangedEvent, EventArgs.Empty);

			//TODO revise. use: _Metadata.HasDataProvider / _Metadata.DataProvider changed
			if (_metadata.DataProvider != null) _metadata.DataProvider.DataChanged += (sender, e) => OnDataChanged(e);
		}

		/// <summary> Occurs when the Metadata.DataProvider.Data has been changed
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDataChanged(DataChangedEventArgs e) {
//			//TODO Verify
//			//StackOverflow!
////			Debug.WriteLine("=>INFO: Recursive notify DataProvider.Data has been changed. UniqueID: {8CAA2DBC-2AC0-4846-800F-7805148B7456}");
//			foreach (var businessObject in Children) {
//			    //if(ObjectBM is IListBM) ObjectBM.Metadata.DataProvider.NotifyDataChanged();
//			    businessObject.Metadata.DataProvider.NotifyDataChanged();
//			}
		}

		/// <summary> Requests a user feedback.
		/// </summary>
		/// <param name="args">The <see cref="UserFeedbackEventArgs"/> instance containing the event data.</param>
		/// <param name="callback"> The callback method which is called after feedback </param>
		/// <param name="state"> a user-defined object that qualifies or contains information about an feedback operation. </param>
		public void RequestUserFeedback(UserFeedbackEventArgs args, Action<UserFeedbackEventArgs> callback=null, object state=null) {
			if (callback != null || state != null) {
				args.AsyncCallback = callback;
				args.AsyncCallbackParameters = state;
			}
			RequestUserFeedbackCore(args);
		}

		public void RequestUserFeedback(UserFeedbackEventArgs args) { RequestUserFeedbackCore(args); }

		protected virtual void RequestUserFeedbackCore(UserFeedbackEventArgs args) {
			if (args.Source == null) args.Source = this;

			//Routing strategy: bubble to root until first handler found which sets the IsHandled/IsHandledAsync to true
			
			if (UserFeedbackRequested != null) {
				EventUtil.Raise(UserFeedbackRequested,this,args,"{B85E48C5-EBC3-401C-A4F9-D89E930D7278}");
				if(args.AsyncHandled) return;
				if(args.Handled) return;
			}

			if (LazyWeakEventStore.IsValueCreated) {
				EventManager.Raise<EventHandler<UserFeedbackEventArgs>,UserFeedbackEventArgs>(LazyWeakEventStore,"UserFeedbackRequestedEvent",args);
				if(args.AsyncHandled) return;
				if(args.Handled) return;
			}

			if (Parent is ObjectBM) ((ObjectBM)Parent).RequestUserFeedbackCore(args);
			else if (Parent != null) Parent.RequestUserFeedback(args);
			else {
				throw new NotImplementedException("UserFeedbackRequested handler not registered and no parent present!"+
					"\n\t"+"Model: "     + DebugUtil.FormatTypeName(this)+
					"\n\t"+"OriganalSource: "     + DebugUtil.FormatTypeName(args.Source)+
					"\n\t"+"EventArgs: " + DebugUtil.FormatTypeName(args)+
					"\n\t"+"ErrorID: {8502A3BA-6131-436C-9D2F-627391E060D8}");
			}
		}

		/// <summary> Raises the <see cref="BusinessPropertyChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="KsWare.Presentation.BusinessFramework.BusinessPropertyChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnBusinessPropertyChanged(BusinessPropertyChangedEventArgs e) {
			EventUtil.Raise(BusinessPropertyChanged, this, e, "{3364EB50-71E9-4EBF-BBB0-14F5AFB84AA4}");
			EventManager.Raise<EventHandler<BusinessPropertyChangedEventArgs>,BusinessPropertyChangedEventArgs>(LazyWeakEventStore,"PropertyChangedEvent", e);
		}

		/// <summary> Occurs when user feedback is requested.
		/// </summary>
		[Obsolete("Use UserFeedbackRequestedEvent")]
		public event EventHandler<UserFeedbackEventArgs> UserFeedbackRequested;

		/// <summary> Gets the event source for the event which occurs when an user feedback is requested.
		/// </summary>
		/// <value>The user feedback requested event source.</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler<UserFeedbackEventArgs>> UserFeedbackRequestedEvent => EventSources.Get<EventHandler<UserFeedbackEventArgs>>("UserFeedbackRequestedEvent");

		/// <summary> Occurs when a business property has been changed.
		/// </summary>
		public event EventHandler<BusinessPropertyChangedEventArgs> BusinessPropertyChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler<BusinessPropertyChangedEventArgs>> PropertyChangedEvent => EventSources.Get<EventHandler<BusinessPropertyChangedEventArgs>>("PropertyChangedEvent");

		#region IModel,IDisposable

		/// <summary> Occurs when this instance is disposed.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
			Dispose(true);
			//GC.SuppressFinalize(this); Interlocked.Decrement(ref StatisticsːNumberOfInstances);
		}

		/// <summary> Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="explicitDisposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool explicitDisposing) {
			if (explicitDisposing) {
				if(Interlocked.Exchange(ref _isDisposed, 1)>0) return;
				Interlocked.Increment(ref StatisticsːMethodInvocationːDisposeːCount);
			    foreach (var child in _children) { child.Dispose(); }
			    foreach (var disposable in _disposableObjects) { disposable.Dispose(); }
			    _children.Clear();
			    _disposableObjects.Clear();
				if (LazyWeakEventStore.IsValueCreated) LazyWeakEventStore.Value.Dispose();
			    EventUtil.RaiseDisposedEvent(Disposed, this);
			}
		}

		#endregion

		/// <summary> Occurs when the business tree has been changed.
		/// </summary>
		public event EventHandler<TreeChangedEventArgs> TreeChanged;

		/// <summary> Raises the <see cref="TreeChanged"/> event in this and all overlying instances.
		/// </summary>
		protected void OnTreeChanged() {
			var eventArgs = new TreeChangedEventArgs(this);
			OnTreeChanged(eventArgs);
			BusinessObjectTreeHelper.OnTreeChanged(this, eventArgs);
		}

		/// <summary> Raises the <see cref="TreeChanged"/> event in this and all overlying instances.
		/// </summary>
		/// <param name="e">The <see cref="TreeChangedEventArgs"/> instance containing the event data.</param>
		/// <seealso cref="docːObjectBM.TreeChangedˑexample1"/>
		protected virtual void OnTreeChanged(TreeChangedEventArgs e) {
			if (TreeChanged != null) { BusinessObjectTreeHelper.OnTreeChanged(TreeChanged, this, e); }

			var p = this.Parent;
			while (p != null) {
			    if (p is ObjectBM) {
			        ((ObjectBM) Parent).OnTreeChanged(e);
			        break;
			    }
			    else {
			        //EventUtil.Raise(p.TreeChanged,..);
			        throw new NotImplementedException("{D070E402-AD00-4653-8B60-350EC9E2FAF4}");
			    }
			    //p = (IObjectBM) p.Parent; 
			}
		}

		#region IsApplicable

		private readonly List<object> _IsApplicableObjections = new List<object>();

		/// <summary> Gets a value indicating whether this instance is enabled.
		/// </summary>
		/// <remarks></remarks>
		[DefaultValue(true)]
		public bool IsApplicable => this._IsApplicableObjections.Count == 0;

		/// <summary> Occurs when <see cref="IsApplicable"/>-property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler IsApplicableChanged;

		/// <summary>  Sets the applicable state.
		/// </summary>
		/// <param name="token">The token to change the state</param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool SetApplicable(object token, bool value) {
			bool prevValue = this._IsApplicableObjections.Count == 0;

			//Clear is optional not in use at the moment
			//if(false && token is string && (((string)token=="CLEAR")||((string)token=="RESET"))) {
			//    this._IsApplicableObjections.Clear();
			//} else 
			{
			    // store/remove objections
			    if (value == false) { if (!this._IsApplicableObjections.Contains(token)) _IsApplicableObjections.Add(token); }
			    else { this._IsApplicableObjections.Remove(token); }
			}

			bool newValue = this._IsApplicableObjections.Count == 0;
			if (prevValue != newValue) {

			    // set the value for children - this will raise events for the children
			    foreach (var child in Children) {
			        var bo = child as ObjectBM;
			        if (bo == null) throw new NotImplementedException("{26B9FBCC-38FD-4220-8BCB-E1C2445C8F3D}"); //REVIEW  throw or ignore?
			        bo.SetApplicable("Parent is not applicable", newValue);
			    }

			    // raise event for this
			    //TODO ??? OnTreeChanged(new TreeChangedEventArgs(this));
			    if (IsApplicableChanged != null) IsApplicableChanged(this, EventArgs.Empty);
			}

			// return current value
			return newValue;
		}

		#endregion

		#region IsInDesignMode

		/// <summary> Gets a value indicating whether the app is in design mode.
		/// </summary>
		/// <remarks>
		/// The value must be set manually to emulate design time behavior</remarks>
		public static bool IsInDesignMode {
			get => s_isInDesignMode == true;
			set => s_isInDesignMode = value;
		}

		#endregion

		public IDispatcher Dispatcher => ApplicationBM.ModelDispatcher;

		/// <summary> Completes the initialization. Call this method as last directive in constructor
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">always use: <c>_=>this</c></param>
		protected void EndInitialize<T>(Func<object, T> @this) {//ALIAS EndOfConstruction
			IsInitialized = true;
		}
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsInitialized { get => _lazyFields.IsValueCreated ? Fields.GetValue<bool>() : false; set => Fields.SetValue(value); }

		public void DoWhenInitialized(Action action) {
			if (IsInitialized) action();
			else Fields[_=>IsInitialized].ValueChanged+=(s,e) => action();
		}
	}

	public interface IDataBM : IObjectBM {
		object Data { get; set; }
	}

	public interface IDataBM<TData> : IObjectBM {

		/// <summary> Alias for Metadata.DataProvider.Data </summary>
		TData Data { get; set; }

	}

	/// <summary> Business object for data
	/// </summary>
	/// <typeparam name="TData">Type of data</typeparam>
	public class DataBM<TData> : ObjectBM, IDataBM<TData>, IDataBM {

		/// <summary> Alias for Metadata.DataProvider.Data </summary>
		public TData Data { get => (TData) Metadata.DataProvider.Data; set => Metadata.DataProvider.Data = value; }

		object IDataBM.Data { get => Metadata.DataProvider.Data; set => Metadata.DataProvider.Data = value; }

		protected override void OnDataChanged(DataChangedEventArgs e) { OnDataChanged(new DataChangedEventArgs<TData>((TData) e.PreviousData, (TData) e.NewData) {Cause = e.Cause}); }

		protected virtual void OnDataChanged(DataChangedEventArgs<TData> e) {
			
		}
	}

}