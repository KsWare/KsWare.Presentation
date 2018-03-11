using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides a business action
	/// </summary>
	public class ActionBM:ObjectBM {

		/// <summary> Occurs when <see cref="CanExecute"/> property has been changed.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary> Occurs when the action has been executed.
		/// </summary>
		public event EventHandler<ExecutedEventArgs> Executed;

		/// <summary> Gets or sets the business object metadata.
		/// </summary>
		/// <value>The business object metadata.</value>
		public new BusinessActionMetadata Metadata{get{return (BusinessActionMetadata) base.Metadata;} set {base.Metadata = value;}}

		/// <summary> Creates the default metadata for the current type of business object .
		/// </summary>
		/// <returns>business object metadata</returns>
		protected override BusinessMetadata CreateDefaultMetadata() {
			return new BusinessActionMetadata {DataProvider = new LocalDataProvider()};
		}

		/// <summary>
		/// Called before the Metadata-property is changed.
		/// </summary>
		/// <param name="newMetadata">The metadata.</param>
		protected override void OnMetadataChanging(BusinessMetadata newMetadata) {
			MemberAccessUtil.Demand(newMetadata is BusinessActionMetadata, "Invalid type of Metadata! Type of BusinessActionMetadata expected.",this,"{F84C7FD2-E8B6-45F5-8C12-29F490CBA569}");
			if(newMetadata.DataProvider==null) newMetadata.DataProvider = new NoDataProvider();
			base.OnMetadataChanging(newMetadata);
		}

		/// <summary>
		/// Called after the Metadata-property has been changed.
		/// </summary>
		protected override void OnMetadataChanged() {
			MemberAccessUtil.Demand(base.Metadata is BusinessActionMetadata, "Invalid type of Metadata! Type of BusinessActionMetadata expected.",this,"{67FB63AE-0DB4-4F28-832E-9E75B7C979BD}");
			base.OnMetadataChanged();
			((BusinessActionMetadata)Metadata).CanExecuteChangedCallback=OnMetadataCanExecuteChangedCallback;
		}

		private void OnMetadataCanExecuteChangedCallback(object sender, EventArgs args) {
			//?? OnPropertyChanged("CanExecute");
			EventUtil.Raise(this.CanExecuteChanged, this, EventArgs.Empty, "{D659AEF7-CD8C-46AB-970E-EE20F7E3AECE}");
			OnTreeChanged();
		}

		protected override void OnParentChanged(ValueChangedEventArgs e) {
			base.OnParentChanged(e);
			RegisterActionMethod(e);
		}

		private void RegisterActionMethod(ValueChangedEventArgs e) {
			if (e.NewValue != null) {
				// MemberName: "EditAction" or "Edit"
				// MethodName: "DoEdit"
				var name = MemberName.EndsWith("Action") ? MemberName.Substring(0, MemberName.Length - 6) : MemberName;
				string methodName= "Do"+name;
				var methods=e.NewValue.GetType().GetMembers(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic)
					.Where(m=>m.Name==methodName && m.MemberType==MemberTypes.Method)
					.OfType<MethodInfo>()
					.ToArray();
				if (methods.Length == 0) { /*TODO LOG: No method {methodName} found. */}
				else if (methods.Length == 1) {
					var parameters = methods[0].GetParameters();
					if (parameters.Length == 0) {
						var body   = Expression.Call(Expression.Constant(e.NewValue), methods[0]);
						var lambda = Expression.Lambda<Action>(body);
						MːDoAction = lambda.Compile();
					} else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object)) {
						var param   = Expression.Parameter(parameters[0].ParameterType);
						var body    = Expression.Call(Expression.Constant(e.NewValue), methods[0],param);
						var lambda  = Expression.Lambda<Action<object>>(body, param);
						MːDoActionP = lambda.Compile();
					}
					else {
						 /*TODO LOG: No method {methodName} with matching signature found. */
						 // see http://stackoverflow.com/questions/2933221/can-you-get-a-funct-or-similar-from-a-methodinfo-object
					}
				}
			}
		}
		/// <summary> Executes the action.
		/// </summary>
		/// <param name="parameter">The action parameter.</param>
		[UsedImplicitly]
		public void Execute(object parameter) {
			if(!CanExecute) return;
			if(((BusinessActionMetadata)Metadata).ExecuteCallback!=null) {
				((BusinessActionMetadata)Metadata).ExecuteCallback(this, new ExecutedEventArgs(parameter));
			}
			EventUtil.Raise(Executed,this,new ExecutedEventArgs(parameter),"{0A1FEFD8-509C-4B9F-B6FC-D5E4478761A8}");
		}

		/// <summary> Gets a value indicating whether this action can be executed.
		/// </summary>
		/// <value><see langword="true"/> if this action can be executed; otherwise, <see langword="false"/>.</value>
		[UsedImplicitly]
		public bool CanExecute {get {return ((BusinessActionMetadata)Metadata).CanExecute;}}

		/// <summary> [DRAFT] sets the Metadata.ActionProvider.ExecutedCallback
		/// </summary>
		/// <value></value>
		public Action MːDoAction{set { Metadata.ExecuteCallback = (s,e)=>value(); }}

		/// <summary> [DRAFT] sets the Metadata.ActionProvider.ExecutedCallback
		/// </summary>
		/// <value></value>
		public Action<object> MːDoActionP {
			set { Metadata.ExecuteCallback = (s, e) => value(e.Parameter); }
		}

		/// <summary> [DRAFT 2014-05-06] sets the Metadata.ActionProvider.ExecutedCallback
		/// </summary>
		/// <value></value>
		public Action MːDoTryAction {
			set {
				Metadata.ExecuteCallback = (s, e) => {
					if(Debugger.IsAttached) value();
					else try { value(); } catch (Exception ex) {RequestUserFeedback(new ExceptionFeedbackEventArgs(ex));}
				};
			}
		}

	}

}
