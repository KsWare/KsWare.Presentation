using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework {

	public interface ISlimBusinessObjectVM : ISlimObjectVM {

		object BusinessObject { get; set; }
	}

	public class SlimBusinessObjectVM<TBusinessObject>:SlimObjectVM,ISlimBusinessObjectVM where TBusinessObject:class,IObjectBM {

		protected static readonly Dictionary<Type,List<string>> PropertyNameCache=new Dictionary<Type, List<string>>(); 

		private TBusinessObject _businessObject;

		public SlimBusinessObjectVM() {}
		public SlimBusinessObjectVM(TBusinessObject BusinessObject) { _businessObject = BusinessObject; }

		public TBusinessObject BusinessObject {
			get => _businessObject;
			set {
				if(Equals(_businessObject,value)) return;
				var prev = _businessObject;
				_businessObject = value;
				OnBusinessObjectChanged(prev, _businessObject);
				OnPropertyChanged(nameof(BusinessObject));
			}
		}
		object ISlimBusinessObjectVM.BusinessObject { get => BusinessObject; set => BusinessObject = (TBusinessObject) value; }

		protected virtual void OnBusinessObjectChanged(TBusinessObject previousBusinessObject, TBusinessObject newBusinessObject) {
			List<string> properties;
			var type = GetType();
			if (!PropertyNameCache.TryGetValue(type, out properties)) {
				var allPropertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
				var filteredPropertyInfos = allPropertyInfos.Where(x => x.GetIndexParameters().Length == 0).ToArray();
				properties = filteredPropertyInfos.Select(x => x.Name).ToList();
				PropertyNameCache.Add(type,properties);
			}
			properties.ForEach(OnPropertyChanged);
		}

	}

}