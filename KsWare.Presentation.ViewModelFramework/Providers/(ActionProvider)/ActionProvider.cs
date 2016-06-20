using System;
using System.ComponentModel;
using System.Diagnostics;
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
			ApplicationDispatcher.BeginInvoke(OnAfterExecute);
		}

		/// <summary> Called before calling <see cref="ExecutedCallback"/>. Raises the <see cref="BeforeExecute"/> event.
		/// </summary>
		protected virtual void OnBeforeExecute() {
			EventUtil.Raise(BeforeExecute,this,EventArgs.Empty,"{BC52B4FE-518C-4D36-9C03-B9CE12E6DA02}");
		}

		/// <summary> Called after calling <see cref="ExecutedCallback"/>. Raises the <see cref="AfterExecute"/> event.
		/// </summary>
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

		/// <summary> Occurs before calling <see cref="ExecutedCallback"/>.
		/// </summary>
		public event EventHandler BeforeExecute;
		
		/// <summary> Occurs after calling <see cref="ExecutedCallback"/>.
		/// </summary>
		public event EventHandler AfterExecute;

		/// <summary> Gets or sets the execute callback.
		/// </summary>
		/// <value>The execute callback.</value>
		/// <remarks></remarks>
		public EventHandler<ExecutedEventArgs> ExecutedCallback{get;set;}

		/// <summary> Gets or sets the callback method to implement a custom CanExecute logic.
		/// </summary>
		/// <value>The can execute callback.</value>
		public EventHandler<CanExecuteEventArgs> CanExecuteCallback { get; set; }

		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		/// <remarks></remarks>
		public abstract object BusinessObject{get;set;}
		
	}

}
