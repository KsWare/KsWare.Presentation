using System;
using System.ComponentModel;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Interface for view models which uses business objects (<see cref="IObjectBM"/>) as underlying data.
	/// </summary>
	public interface IBusinessObjectVM : IObjectVM {
		Type GetBusinessObjectType();
		IObjectBM BusinessObject { get; set; }
	}

	/// <summary> Interface for view models which uses business objects (<see cref="IObjectBM"/>) as underlying data.
	/// </summary>
	public interface IBusinessObjectVM<TBusinessObject> : IBusinessObjectVM where TBusinessObject:class,IObjectBM {
		new Type GetBusinessObjectType();
		new TBusinessObject BusinessObject { get; set; }
	}

	/// <summary> Base class for view models which uses business objects (<see cref="IObjectBM"/>) as underlying data.
	/// </summary>
	/// <typeparam name="TBusinessObject">The type of the business object.</typeparam>
	public class BusinessObjectVM<TBusinessObject>:ObjectVM,IBusinessObjectVM where TBusinessObject:class,IObjectBM {

		protected const string NoBusinessObjectToken="BusinessObject is null";

		/// <summary> Initializes a new instance of the <see cref="BusinessObjectVM{TBusinessObject}" /> class.
		/// </summary>
		public BusinessObjectVM() {
			SetEnabled(NoBusinessObjectToken, false);
		}

		/// <summary> Gets the type of the business object.
		/// </summary>
		/// <returns>The type of the business object</returns>
		Type IBusinessObjectVM.GetBusinessObjectType() {return typeof (TBusinessObject);}

		/// <summary> Gets or sets the business object.
		/// </summary>
		/// <value>The business object.</value>
		[Bindable(true,BindingDirection.TwoWay)]
		public TBusinessObject BusinessObject { get { return (TBusinessObject)Metadata.DataProvider.Data; } set { Metadata.DataProvider.Data = value; } }

		IObjectBM IBusinessObjectVM.BusinessObject { get { return BusinessObject; } set { BusinessObject = (TBusinessObject) value; } }

		/// <summary> Creates the default metadata (<see cref="BusinessObjectMetadata{TBusinessObject}"/>).
		/// </summary>
		/// <returns><see cref="BusinessObjectMetadata{TBusinessObject}"/></returns>
		protected override ViewModelMetadata CreateDefaultMetadata() {
			var metadata = new BusinessObjectMetadata<TBusinessObject>();
			return metadata;
		}

		/// <summary> Called when underlying business object changed.
		/// </summary>
		/// <param name="o">The current business object.</param>
		/// <param name="p">The previous business object.</param>
		/// <remarks>
		/// * Sets the <see cref="ObjectVM.IsEnabled"/> property to false, when the business object is null.<br/>
		/// * User feedback requests are passed on from business object to view model.<br/>
		/// </remarks>
		protected virtual void OnBusinessObjectChanged(TBusinessObject o, TBusinessObject p) {
			SetEnabled(NoBusinessObjectToken, o != null);

			//slim objects does not support UserFeedbackRequestedEvent, so we have to check this
			if (p != null && !p.IsSlim) p.UserFeedbackRequestedEvent.Release(this, "RequestUserFeedback");
			if (o != null && !o.IsSlim) o.UserFeedbackRequestedEvent.Register(this,"RequestUserFeedback", (s, e) => RequestUserFeedback(e));

			OnPropertyChanged("BusinessObject");
		}

		/// <summary> [override deprecated] Called when <see cref="docːObjectVM.underlyingˑdata"/> changed.
		/// </summary>
		/// <param name="e">The <see cref="DataChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Under normal circumstances you don't need to override this method. Override <see cref="OnBusinessObjectChanged"/> instead.</remarks>
		[Obsolete("Override OnBusinessObjectChanged")]
		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);
			var ndata = (TBusinessObject)e.NewData;
			var pdata = e.PreviousData is DBNull ?default(TBusinessObject) : (TBusinessObject)e.PreviousData;
			if(ndata==null && pdata==null) return; //PERFORMANCE: do we need this? if the condition is always false, remove this.
			OnBusinessObjectChanged(ndata,pdata);
		}
		
	}

	public static class BusinessObjectVM {

		public static Type GetBusinessType(Type vmType) {
			//TODO if(vmType.IsInterface) {}
			var t = vmType;
			while (t!=typeof(object)) {
				if(t==null) break;
				if(t==typeof(ObjectVM)) break;
				if(t.Name.Contains("`")) {
					var tg=t.GetGenericTypeDefinition();
					if(tg==typeof(BusinessObjectVM<>)) {
						var genericArguments = t.GetGenericArguments();
						return genericArguments[0];
					}
				}
				t = t.BaseType;
			}
			return null;
		}

		public static bool IsBusinessObjectVM(Type vmType) {
			var t = vmType;
			while (t!=typeof(object)) {
				if(t==typeof(ObjectVM)) break;
				if(t.Name.Contains("`")) {
					var tg=t.GetGenericTypeDefinition();
					if(tg==typeof(BusinessObjectVM<>)) {
						return true;
					}
				}
				t = t.BaseType;
			}
			return false;
		}
	}
}