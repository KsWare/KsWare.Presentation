using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework {

	public interface ISlimBusinessObjectVM : ISlimObjectVM {

		object BusinessObject { get; set; }
	}

	public class SlimBusinessObjectVM<TBusinessObject>:SlimObjectVM,ISlimBusinessObjectVM where TBusinessObject:class,IObjectBM {

		protected static readonly Dictionary<Type,List<string>> PropertyNameCache=new Dictionary<Type, List<string>>(); 

		private TBusinessObject m_BusinessObject;

		public SlimBusinessObjectVM() {}
		public SlimBusinessObjectVM(TBusinessObject BusinessObject) { m_BusinessObject = BusinessObject; }

		public TBusinessObject BusinessObject {
			get { return m_BusinessObject; }
			set {
				if(Equals(m_BusinessObject,value)) return;
				var prev = m_BusinessObject;
				m_BusinessObject = value;
				OnBusinessObjectChanged(prev, m_BusinessObject);
				OnPropertyChanged("BusinessObject");
			}
		}
		object ISlimBusinessObjectVM.BusinessObject { get { return BusinessObject; } set { BusinessObject = (TBusinessObject) value; } }

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