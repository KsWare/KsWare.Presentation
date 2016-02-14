using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core.Logging;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides a dynamic typed view model
	/// </summary>
	/// <remarks></remarks>
	public class DynamicVM:ValueVM<object>,IValueVM {

		public Type ValueType { get { return Fields.Get<Type>("ValueType"); } set { Fields.Set("ValueType", value); } }

		protected override void OnBusinessObjectChanged(ValueChangedEventArgs<IObjectBM> e) {
			base.OnBusinessObjectChanged(e);
			
			#region ValueType = {Binding ValueType, Source= MːBusinessObject, Mode=TwoWay}
			if (e.PreviousValue is DynamicBM) {
				((DynamicBM)e.PreviousValue).Fields["ValueType"].ValueChangedEvent.Release(this,"{16A4A018-0D9A-41B9-95C6-11F754037AA4}");
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

		Type IValueVM.ValueType{get {return ValueType;}}
	}	

}
