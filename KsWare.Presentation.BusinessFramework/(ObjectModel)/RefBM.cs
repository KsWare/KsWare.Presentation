using System;
using System.Collections.Generic;

namespace KsWare.Presentation.BusinessFramework {
	
	/// <summary> Interface for a business model object reference
	/// </summary>
	public interface IRefBM {

		/// <summary> Gets or sets the object (the target) referenced by the current <see cref="RefBM{TRef}"/> object.
		/// </summary>
		IObjectBM Target { get; set; }

		/// <summary> Occurs when the <see cref="Target"/> property changes.
		/// </summary>
		[Obsolete("Use TargetChangedEvent")]
		event EventHandler TargetChanged;

		/// <summary> Event source for the event which occurs when the <see cref="Target"/> property changes.
		/// </summary>
		IEventSource TargetChangedEvent { get; }
		//IWeakEventSource<EventHandler<ValueChangedEventArgs>> TargetChangedEvent { get; }

	}

	/// <summary> Interface for a business model object reference
	/// </summary>
	/// <typeparam name="TRef">The type of the reference</typeparam>
	public interface IRefBM<TRef> where TRef:class,IObjectBM {

		/// <summary> Gets or sets the object (the target) referenced by the current <see cref="RefBM{TRef}"/> object.
		/// </summary>
		TRef Target { get; set; }

		/// <summary> Occurs when the <see cref="Target"/> property changes.
		/// </summary>
		[Obsolete("Use TargetChangedEvent")]
		event EventHandler TargetChanged;

		/// <summary> Event source for the event which occurs when the <see cref="Target"/> property changes.
		/// </summary>
		IEventSource<EventHandler<ValueChangedEventArgs<TRef>>> TargetChangedEvent { get; }

	}

	/// <summary> Provides a reference for a business model object
	/// </summary>
	/// <typeparam name="TRef">The type of the reference</typeparam>
	public class RefBM<TRef>:ObjectBM,IRefBM<TRef>,IRefBM where TRef:class,IObjectBM {

		private TRef _target;

		public RefBM() {
			
		}

		/// <summary> Gets or sets the object (the target) referenced by the current <see cref="RefBM{TRef}"/> object.
		/// </summary>
		public TRef Target {
			get {
// 				if(_Target==null && Metadata.DataProvider.Data!=null) {
//					//m_Target=new TRef(){Metadata = {DataProvider = {Data = Metadata.DataProvider.Data}}};
// 					//Metadata.NewItemProvider
// 					_Target=(TRef) Activator.CreateInstance(typeof (TRef));
// 					_Target.Metadata.DataProvider.Data = Metadata.DataProvider.Data;
// 				}
				if(Metadata.DataProvider.Data!=null) throw new NotImplementedException("Metadata.DataProvider.Data is not null!");
				return _target; 
			}
			set {
				if (_target == value) return;
				var old = _target;
				_target = value;

//				if (_Target != null) {
//					Metadata.DataProvider.Data=_Target.Metadata.DataProvider.Data; //???
//				}
				if(Metadata.DataProvider.Data!=null) throw new NotImplementedException("Metadata.DataProvider.Data is not null!");

				var e=new ValueChangedEventArgs<TRef>(old,_target);
				EventUtil.Raise(TargetChanged,this,e,"{0524E629-61C7-4517-BB1F-3B46BE3FD5C8}");
				EventManager.Raise<EventHandler<ValueChangedEventArgs<TRef>>,ValueChangedEventArgs<TRef>>(LazyWeakEventStore,"TargetChangedEvent",e);
				OnBusinessPropertyChanged(new BusinessPropertyChangedEventArgs("Target"));
			}
		}

		IObjectBM IRefBM.Target {get { return Target; }set { Target = (TRef) value; }}


		/// <summary> [EXPERIMENTAL] Gets or sets the list of available targets.
		/// </summary>
		/// <value>The list of available targets</value>
		public IList<TRef> ListSource { get; set; }

		/// <summary> Occurs when the <see cref="Target"/> property changes.
		/// </summary>
		[Obsolete("Use TargetChangedEvent")]
		public event EventHandler TargetChanged;


		/// <summary> Event source for the event which occurs when the <see cref="Target"/> property changes.
		/// </summary>
		public IEventSource<EventHandler<ValueChangedEventArgs<TRef>>> TargetChangedEvent {
			get { return EventSources.Get<EventHandler<ValueChangedEventArgs<TRef>>>("TargetChangedEvent"); }
		}
//		IWeakEventSource<EventHandler<ValueChangedEventArgs>> IRefBM.TargetChangedEvent { get { return (IWeakEventSource<EventHandler<ValueChangedEventArgs>>)TargetChangedEvent; } }
		IEventSource IRefBM.TargetChangedEvent { get { return TargetChangedEvent; } }

	}
}
