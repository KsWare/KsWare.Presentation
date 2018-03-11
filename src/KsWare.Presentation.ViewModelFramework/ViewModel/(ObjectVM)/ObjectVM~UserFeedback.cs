using System;
using System.Diagnostics;

namespace KsWare.Presentation.ViewModelFramework {

	partial interface IObjectVM /* Part: UserFeedback*/ {

//		void RequestUserFeedback(UserFeedbackEventArgs args);
		void RequestUserFeedback(UserFeedbackEventArgs args, Action<UserFeedbackEventArgs> callback=null, object state=null);

	}

	partial class ObjectVM /* Part: UserFeedback*/ {


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

		protected virtual void RequestUserFeedbackCore(UserFeedbackEventArgs args) {
			if (args.Source == null) args.Source = this;
			
			//Routing strategy: bubble to root until first handler found

			if (UserFeedbackRequested != null) UserFeedbackRequested(this, args);
			if(args.AsyncHandled) return;
			if(args.Handled) {if(args.AsyncCallback!=null) args.AsyncCallback(args); return;} //TODO revise calling of AsyncCallback

			EventManager.Raise<EventHandler<UserFeedbackEventArgs>,UserFeedbackEventArgs>(LazyWeakEventStore, "UserFeedbackRequestedEvent", args);
			if(args.AsyncHandled) return;
			if (args.Handled) {if(args.AsyncCallback!=null) args.AsyncCallback(args);return;} //TODO revise calling of AsyncCallback

			if(Parent is ObjectVM) ((ObjectVM)Parent).RequestUserFeedbackCore(args);
			else if(Parent!=null) Parent.RequestUserFeedback(args);
			else {
				//TODO: revise no UserFeedbackRequested handler registered 
				if (args is BusyUserFeedbackEventArgs && ((BusyUserFeedbackEventArgs)args).IsBusy==true) {
					//if no one in the hierarchy uses  e.g. BusyAdornerBehavior ignore this RequestUserFeedback
					Debug.WriteLine("WARNING: UserFeedbackRequested handler (for BusyUserFeedbackEventArgs) not registered and no parent present!"+
						"\n\t"+"ViewModel: " + DebugUtil.FormatTypeName(this)+
						"\n\t"+"ErrorID: {58358DEA-4028-413D-9BCD-641A8AA07BE5}"
						);
					return;
				}
				throw new NotImplementedException("UserFeedbackRequested handler not registered and no parent present!"+
					"\n    "+"Model: "     + DebugUtil.FormatTypeName(this)+
					"\n    "+"OriginalSource: "     +  DebugUtil.FormatTypeName(args.Source)+
					"\n    "+"EventArgs: " + DebugUtil.FormatTypeName(args)+
					"\n    "+"ErrorID: {EA0BF43D-740F-4543-B67C-4E1D755252E1}");
			}
		}

		/// <summary> Occurs when user feedback is requested.
		/// </summary>
		public event EventHandler<UserFeedbackEventArgs> UserFeedbackRequested;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public IEventSource<EventHandler<UserFeedbackEventArgs>> UserFeedbackRequestedEvent {
			get { return EventSources.Get<EventHandler<UserFeedbackEventArgs>>("UserFeedbackRequestedEvent"); }
		}

	}

}
