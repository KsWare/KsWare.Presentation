using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Rx;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> Interface vor action providers
	/// </summary>
	public interface IActionProvider:IViewModelProvider {

		/// <summary> Gets a value whether the action can be executed or not
		/// </summary>
		bool CanExecute{get;}

		/// <summary> Sets a value whether the action can be executed or not
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <remarks></remarks>
		void SetCanExecute(object token, bool value);

		/// <summary> Occurs when <see cref="CanExecute"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		event EventHandler CanExecuteChanged;

		IObservable<bool> CanExecuteObservable { get; }


		/// <summary> Executes the action.
		/// </summary>
		/// <param name="parameter">The action parameter.</param>
		/// <remarks></remarks>
		void Execute(object parameter);

		/// <summary> Gets or sets the execute callback.
		/// </summary>
		/// <value>The execute callback.</value>
		/// <remarks></remarks>
		EventHandler<ExecutedEventArgs> ExecutedCallback{get;set;}

		EventHandler<CanExecuteEventArgs> CanExecuteCallback { get; set; }

		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks></remarks>
		object BusinessObject{get;set;}

		/// <summary> [EXPERIMENTAL] Gets or sets a value indicating whether exceptions are catched and forwarded as user feedback.
		/// </summary>
		/// <value><c>true</c> if exceptions are catched and forwarded as user feedback; otherwise, <c>false</c>.</value>
		/// <seealso cref="ObjectVM.RequestUserFeedback"/>
		/// <remarks> The property name is preliminary and subject to change.</remarks>
		bool CatchExceptionsAndRequestUserFeedback { get; set; /*ADDED 2014-06-03*/} 


		/// <summary> [EXPERIMENTAL] Gets or sets the exception default message.
		/// </summary>
		/// <value>The exception default message.</value>
		/// <remarks> The property name is preliminary and subject to change.</remarks>
		string ExceptionDefaultMessage { get; set; /*ADDED 2014-09-11*/} 

		event EventHandler BeforeExecute;

		event EventHandler AfterExecute;

		void NotifyCanExecuteChanged();

	}

	/// <summary> Base class for action providers
	/// </summary>
	public abstract class ActionProvider:ViewModelProvider,IActionProvider {

		private bool? m_UseAsync;
		private Lazy<SimpleObservable<bool>> m_LazyCanExecuteObservable=new Lazy<SimpleObservable<bool>>(() => new SimpleObservable<bool>());

		protected ActionProvider() {
			
		}

		/// <summary> Gets a value whether the action can be executed or not
		/// </summary>
		public abstract bool CanExecute{get;}

		/// <summary> Sets a value whether the action can be executed or not
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <remarks></remarks>
		public abstract void SetCanExecute(object token, bool value);

		/// <summary>  Occurs when <see cref="CanExecute"/> property has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler CanExecuteChanged;

		public IObservable<bool> CanExecuteObservable { get { return m_LazyCanExecuteObservable.Value; }} 

		public void NotifyCanExecuteChanged() {OnCanExecuteChanged();}

		/// <summary> Raises the <see cref="CanExecuteChanged"/>-event.
		/// </summary>
		protected void OnCanExecuteChanged() { 
			//if(canExecute!=CanExecute) OnPropertyChanged("CanExecute");
			EventUtil.Raise(CanExecuteChanged,this, EventArgs.Empty,"{5992B5C0-13B1-4921-8176-E0EA4B005C1B}");
			if(m_LazyCanExecuteObservable.IsValueCreated) m_LazyCanExecuteObservable.Value.Raise(CanExecute);
		}

		[Bindable(false)]
		public bool Async {
			get { return m_UseAsync==true; }
			set {
				MemberAccessUtil.DemandWriteOnce(m_UseAsync==null,null,this,"Async","{50EA3CCF-2609-44F9-B023-1E450350E774}");
				m_UseAsync = value;
			}
		}

		/// <summary> Executes the action.
		/// </summary>
		/// <param name="parameter">The action parameter.</param>
		/// <remarks></remarks>
		public virtual void Execute(object parameter) {
			if(!CanExecute) throw new NotSupportedException(
				"Execute not supported. CanExecute returns false."
				+"\r\n\t"+"ViewModel: " +(ViewModel!=null?ViewModel.GetType().ToString():"{Null}")
				+"\r\n\t"+"MemberName: "+ViewModelMemberName
//				+"\r\n\t"+"Property: "+this.PropertyName
			);
			OnExecute(parameter);
		}

		private string ViewModelMemberName {get {var vm = ViewModel;return vm==null?null:vm.MemberName;}}

		/// <summary> Calls the <see cref="ExecutedCallback"/> 
		/// </summary>
		/// <param name="parameter">The action parameter.</param>
		/// <remarks></remarks>
		protected void OnExecute(object parameter) { 
			if(ExecutedCallback==null) return;
			
			if(m_UseAsync!=true) { 
				//Blocking ExecutedCallback
				OnBeforeExecute();
				TryAndCatchExecute(parameter);
				OnAfterExecute();
			} else { 
				//NonBlocking ExecutedCallback
				OnBeforeExecute();
				var cb = new AsyncCallback(CallBackProc);
				var am = new Action<object>(TryAndCatchExecute);
				var ar = am.BeginInvoke(parameter, cb, null);
				this.DoNothing(ar);
				//done.WaitOne(); 				
			}
		}

		private void TryAndCatchExecute(object parameter) {
			if(Debugger.IsAttached) ExecutedCallback(this, new ExecutedEventArgs(parameter));
			else try { ExecutedCallback(this, new ExecutedEventArgs(parameter)); }
				catch (Exception ex) {
					var ofe=string.IsNullOrWhiteSpace(ExceptionDefaultMessage) 
						? new OperationFailedException(ex,new StackTrace(true)) 
						: new OperationFailedException(ExceptionDefaultMessage, ex,new StackTrace(true));
//					ViewModel.RequestUserFeedback(new ExceptionFeedbackEventArgs(ofe));
					ExceptionManager.ExceptionCaught(ofe);
				}
		}

		private void CallBackProc(IAsyncResult ar) {
			var action = (EventHandler<ExecutedEventArgs>)ar.AsyncState; 
			action.EndInvoke(ar); 
			//done.Set();
			ApplicationDispatcher.CurrentDispatcher.BeginInvoke(OnAfterExecute);
		}

		protected virtual void OnBeforeExecute() {
			EventUtil.Raise(BeforeExecute,this,EventArgs.Empty,"{BC52B4FE-518C-4D36-9C03-B9CE12E6DA02}");
		}

		protected virtual void OnAfterExecute() {
			EventUtil.Raise(AfterExecute,this,EventArgs.Empty,"{994B0FDF-6220-4802-8864-798AE483334D}");
		}

		/// <summary> [EXPERIMENTAL] Gets or sets a value indicating whether exceptions are catched and forwarded as user feedback.
		/// </summary>
		/// <value><c>true</c> if exceptions are catched and forwarded as user feedback; otherwise, <c>false</c>.</value>
		/// <seealso cref="ObjectVM.RequestUserFeedback"/>
		public bool CatchExceptionsAndRequestUserFeedback { get; set; }

		/// <summary> [EXPERIMENTAL] Gets or sets the exception default message.
		/// </summary>
		/// <value>The exception default message.</value>
		/// <remarks> The property name is preliminary and subject to change.</remarks>
		public string ExceptionDefaultMessage { get; set; /*ADDED 2014-09-11*/} 

		public event EventHandler BeforeExecute;
		
		public event EventHandler AfterExecute;

		/// <summary> Gets or sets the execute callback.
		/// </summary>
		/// <value>The execute callback.</value>
		/// <remarks></remarks>
		public EventHandler<ExecutedEventArgs> ExecutedCallback{get;set;}

		public EventHandler<CanExecuteEventArgs> CanExecuteCallback { get; set; }

		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks></remarks>
		public abstract object BusinessObject{get;set;}
		
	}


	/// <summary> 
	/// </summary>
	public class LocalActionProvider: ActionProvider  {

		private readonly ArrayList m_CanExecuteObjections=new ArrayList();

		/// <summary> Initializes a new instance of the <see cref="DisplayValueProvider"/> class.
		/// </summary>
		public LocalActionProvider() {}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported{get {return true;}}

		/// <summary> Gets a value whether the action can be executed or not
		/// </summary>
		/// <value>The value whether the action can be executed or not</value>
		public override bool CanExecute {
			get {
				if(CanExecuteCallback!=null) {
					var e=new CanExecuteEventArgs(null);
					CanExecuteCallback(this, e);
					if(!e.CanExecute) return false;
				}
				return m_CanExecuteObjections.Count==0;
			}
		}

		/// <summary> Sets a value whether the action can be executed or not
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <remarks></remarks>
		public override void SetCanExecute(object token, bool value) {
			var canExecute = this.CanExecute;
			if(value) m_CanExecuteObjections.Remove(token);
			else if(!m_CanExecuteObjections.Contains(token)) m_CanExecuteObjections.Add(token);
			if(CanExecute != canExecute) OnCanExecuteChanged();
		}

		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks></remarks>
		public override object BusinessObject {get {throw new NotSupportedException();}set {throw new NotSupportedException();}}
	}

	/// <summary> 
	/// </summary>
	public class CommandActionProvider: ActionProvider {

		private readonly ArrayList m_CanExecuteObjections=new ArrayList();
		private ICommand m_Command;

		/// <summary> Initializes a new instance of the <see cref="DisplayValueProvider"/> class.
		/// </summary>
		public CommandActionProvider() {}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported{get {return true;}}

		/// <summary> Gets a value whether the action can be executed or not
		/// </summary>
		/// <value>The value whether the action can be executed or not</value>
		public override bool CanExecute {
			get {
				//return m_CanExecuteObjections.Count==0;
//				return m_Command!=null && m_Command.CanExecute(null);
				return true;
			}
		}

		/// <summary> Sets a value whether the action can be executed or not
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <remarks></remarks>
		public override void SetCanExecute(object token, bool value) {
			var canExecute = this.CanExecute;
			if(value) m_CanExecuteObjections.Remove(token);
			else if(!m_CanExecuteObjections.Contains(token)) m_CanExecuteObjections.Add(token);
			if(CanExecute != canExecute) OnCanExecuteChanged();
		}

		public ICommand Command {
			get { return m_Command; }
			set {
				if(m_Command==value)return;

				if(m_Command!=null) {
					m_Command.CanExecuteChanged-=AtCommandOnCanExecuteChanged;
					ExecutedCallback = null;
				}
				m_Command = value;
				if(m_Command!=null) {
					m_Command.CanExecuteChanged+=AtCommandOnCanExecuteChanged;
					ExecutedCallback= (s,e) => m_Command.Execute(e.Parameter);
				}
			}
		}

		private void AtCommandOnCanExecuteChanged(object sender, EventArgs eventArgs) {
			OnPropertyChanged("CanExecute");
		}

		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks></remarks>
		public override object BusinessObject {get {throw new NotSupportedException();}set {throw new NotSupportedException();}}
	}

	/// <summary>  <see cref="ActionProvider"/> for actions in business layer
	/// </summary>
	/// <remarks></remarks>
	public class BusinessActionProvider: ActionProvider {

		private ActionBM m_ActionBM;

		/// <summary> Initializes a new instance of the <see cref="DisplayValueProvider"/> class.
		/// </summary>
		public BusinessActionProvider() {}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported{get {return m_ActionBM!=null;}}

		/// <summary> Gets a value whether the action can be executed or not
		/// </summary>
		/// <value>The value whether the action can be executed or not</value>
		public override bool CanExecute {
			get {
				if(m_ActionBM==null) return false;
				return m_ActionBM.CanExecute;
			}
		}

		/// <summary> Sets a value whether the action can be executed or not
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <remarks></remarks>
		public override void SetCanExecute(object token, bool value) {
			if(m_ActionBM==null) throw new NotSupportedException("Underlying object not initialized!");
			//throw new NotSupportedException("Action not supported by underlying object! {8FFF5FFA-8E8E-471A-A302-E9DC0F9EAC70}");
			((BusinessFramework.BusinessActionMetadata) m_ActionBM.Metadata).SetCanExecute(token,value);
		}

		/// <summary> Executes the action.
		/// </summary>
		/// <param name="parameter">The action parameter.</param>
		/// <remarks></remarks>
		public override void Execute(object parameter) {
			if(m_ActionBM==null) throw new NotSupportedException("Underlying object not initialized!");
			m_ActionBM.Execute(parameter);
			//### base.Execute(parameter);
		}


		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks></remarks>
		public override object BusinessObject {
			get {return m_ActionBM;}
			set {
				if(m_ActionBM!=null) {
					m_ActionBM.CanExecuteChanged-=AtActionCanExecuteChanged;
				}

				m_ActionBM=(ActionBM) value;

				if(m_ActionBM!=null) {
					m_ActionBM.CanExecuteChanged+=AtActionCanExecuteChanged;
				}

				OnCanExecuteChanged();//TODO: call only when value of CanExecute has been changed
			}
		}

		/// <summary> Called when this.action.CanExecute-property has been changed
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void AtActionCanExecuteChanged(object sender, EventArgs eventArgs) { 
			OnCanExecuteChanged();
		}
	}

//	/// <summary>  <see cref="ActionProvider"/> for linked actions
//	/// </summary>
//	/// <remarks></remarks>
//	public class LinkedActionProvider: ActionProvider {
//
//		private ActionVM m_LinkedAction;
//
//		/// <summary> Initializes a new instance of the <see cref="DisplayValueProvider"/> class.
//		/// </summary>
//		public LinkedActionProvider() {}
//
//		/// <summary> Gets a value indicating whether the provider is supported.
//		/// </summary>
//		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
//		/// <remarks></remarks>
//		public override bool IsSupported{get {return m_LinkedAction!=null;}}
//
//		/// <summary> Gets a value whether the action can be executed or not
//		/// </summary>
//		/// <value>The value whether the action can be executed or not</value>
//		public override bool CanExecute {
//			get {
//				if(m_LinkedAction==null) return false;
//				return m_LinkedAction.CanExecute;
//			}
//		}
//
//		/// <summary> Sets a value whether the action can be executed or not
//		/// </summary>
//		/// <param name="token">The token.</param>
//		/// <param name="value">if set to <c>true</c> [value].</param>
//		/// <remarks></remarks>
//		public override void SetCanExecute(object token, bool value) {
//			if(m_LinkedAction==null) throw new NotSupportedException("Underlying object not initalized!");
//			m_LinkedAction.Metadata.ActionProvider.SetCanExecute(token,value);
//		}
//
//		/// <summary> Executes the action.
//		/// </summary>
//		/// <param name="parameter">The action parameter.</param>
//		/// <remarks></remarks>
//		public override void Execute(object parameter) {
//			if(m_LinkedAction==null) throw new NotSupportedException("Underlying object not initalized!");
//			m_LinkedAction.Execute(parameter);
//		}
//
//
//		/// <summary> Gets or sets the business object.
//		/// </summary>
//		/// <value>The business object.</value>
//		/// <remarks></remarks>
//		public override object BusinessObject {
//			get {return m_LinkedAction;}
//			set {
//				if(m_LinkedAction!=null) {
//					m_LinkedAction.CanExecuteChanged-=AtActionCanExecuteChanged;
//				}
//
//				m_LinkedAction=(ActionVM) value;
//
//				if(m_LinkedAction!=null) {
//					m_LinkedAction.CanExecuteChanged+=AtActionCanExecuteChanged;
//				}
//
//				OnCanExecuteChanged();//TODO: call only when value of CanExecute has been changed
//			}
//		}
//
//		/// <summary> Called when this.action.CanExecute-property has been changed
//		/// </summary>
//		/// <param name="sender">The sender.</param>
//		/// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
//		private void AtActionCanExecuteChanged(object sender, EventArgs eventArgs) { 
//			OnCanExecuteChanged();
//		}
//	}

}
