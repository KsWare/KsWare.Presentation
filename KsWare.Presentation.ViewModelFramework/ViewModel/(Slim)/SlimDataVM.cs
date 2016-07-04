using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KsWare.Presentation.ViewModelFramework {

	public interface ISlimDataVM : ISlimObjectVM {

		object Data { get; set; }
	}

	public interface ISlimDataVM<TData> : ISlimObjectVM {

		TData Data { get; set; }
	}

	public class SlimDataVM {

//		public static readonly Dictionary<Type, Type> Cache = new Dictionary<Type, Type>();
//		public static readonly List<Assembly> AssemblyCache = new List<Assembly>();
//		private static bool s_IsCurrentAssemblyScanned;
//		private static bool s_IsEntryAssemblyScanned;
//
//		public object Create(object data, Assembly assembly=null) {
//			if (!s_IsCurrentAssemblyScanned) {Scan(Assembly.GetExecutingAssembly());s_IsCurrentAssemblyScanned=true;}
//			if (!s_IsEntryAssemblyScanned  ) {Scan(Assembly.GetEntryAssembly()    );s_IsEntryAssemblyScanned=true;}
//			if (assembly != null && !AssemblyCache.Contains(assembly)) Scan(assembly);
//			if (assembly == null && !AssemblyCache.Contains(Assembly.GetCallingAssembly())) Scan(Assembly.GetCallingAssembly());
//		}
//
//		private void Scan(Assembly assembly) {
//			if(AssemblyCache.Contains(assembly)) return;
//			AssemblyCache.Add(assembly);
//			var allTypes = assembly.GetTypes();
//			var i = typeof (ISlimDataVM);
//			var filteredTypes = allTypes.Where(i.IsAssignableFrom).ToArray();
//			foreach (var type in filteredTypes) {
//				type.GetProperty("Data",BindingFlags.Instance|BindingFlags.Public)
//			}
//
//		}
	}

	public class SlimDataVM<TData>:SlimObjectVM,ISlimDataVM {

		protected static readonly Dictionary<Type,List<string>> PropertyNameCache=new Dictionary<Type, List<string>>(); 

		private TData _data;

		public SlimDataVM() {}
		public SlimDataVM(TData data) { _data = data; }

		public TData Data {
			get { return _data; }
			set {
				if(Equals(_data,value)) return;
				var prev = _data;
				_data = value;
				OnDataChanged(prev, _data);
				OnPropertyChanged(nameof(Data));
			}
		}

		object ISlimDataVM.Data { get { return Data; } set { Data = (TData) value; } }

		protected virtual void OnDataChanged(TData previousData, TData newData) {
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