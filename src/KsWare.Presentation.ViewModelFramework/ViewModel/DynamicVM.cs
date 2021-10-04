using System;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides a dynamic typed view model
	/// </summary>
	/// <remarks></remarks>
	public class DynamicVM:ValueVM<object>,IValueVM {

		public Type ValueType { get => Fields.GetValue<Type>(); set => Fields.SetValue(value); }

		protected override void OnBusinessObjectChanged(ValueChangedEventArgs<IObjectBM> e) {
			base.OnBusinessObjectChanged(e);
			
			#region ValueType = {Binding ValueType, Source= MːBusinessObject, Mode=TwoWay}
			if (e.OldValue is DynamicBM) {
				((DynamicBM)e.OldValue).Fields["ValueType"].ValueChangedEvent.Release(this,"{16A4A018-0D9A-41B9-95C6-11F754037AA4}");
				Fields["ValueType"].ValueChangedEvent.Release(this,"{03BBE793-8175-4BAE-BD94-BC4F2B707324}");
			}
			if (e.NewValue  is DynamicBM) {
				var updateDestination = new Action(() => ValueType = ((DynamicBM) MːBusinessObject).ValueType);
				((DynamicBM)e.NewValue).Fields["ValueType"].ValueChangedEvent.Register(this,"{16A4A018-0D9A-41B9-95C6-11F754037AA4}",(s1,e1) => updateDestination());
				updateDestination();
				var updateSource = new Action(() => ((DynamicBM) MːBusinessObject).ValueType=ValueType);
				Fields["ValueType"].ValueChangedEvent.Register(this,"{03BBE793-8175-4BAE-BD94-BC4F2B707324}",(s1,e1) => updateDestination());
				updateSource();
			}
			#endregion
		}

		Type IValueVM.ValueType => ValueType;
	}	

}
