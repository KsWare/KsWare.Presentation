using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using KsWare.Presentation.Providers;
using KsWare.Presentation.Core;
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "KsWare.Presentation.MemberAccessUtil.Demand(System.Boolean,System.String,System.Object,System.String)")]
		protected override void OnMetadataChanging(BusinessMetadata newMetadata) {
			MemberAccessUtil.Demand(newMetadata is BusinessActionMetadata, "Invalid type of Metadata! Type of BusinessActionMetadata expected.",this,"{F84C7FD2-E8B6-45F5-8C12-29F490CBA569}");
			if(newMetadata.DataProvider==null) newMetadata.DataProvider = new NoDataProvider();
			base.OnMetadataChanging(newMetadata);
		}

		/// <summary>
		/// Called after the Metadata-property has been changed.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "KsWare.Presentation.MemberAccessUtil.Demand(System.Boolean,System.String,System.Object,System.String)")]
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
