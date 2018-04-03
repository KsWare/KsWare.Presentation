using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KsWare.Presentation.ViewModelFramework {

	public class ViewResolver {
		
		public static ViewResolver Default = new ViewResolver();

		public Type Resolve(Type viewModelType, bool throwOnError = true) {
			
			if (viewModelType == typeof(WindowVM)) return typeof(Window);

			var defView = viewModelType.GetCustomAttributes(typeof(DefaultViewAttribute), false).OfType<DefaultViewAttribute>()
				.Select(x => x.View).FirstOrDefault();
			if (defView != null) return defView;

			var n     = viewModelType.Name;     //"MainWindowVM"
			var fn    = viewModelType.FullName; //"ProDevis.UI.ViewModels.MainWindowVM"
			var vmExt = new[] {"VM", "ViewModel"};
			var bn    = n;
			var bfn   = fn;
			foreach (var ext in vmExt) {
				if (n.EndsWith(ext)) {
					bn  = n.Substring(0, n.Length   - ext.Length); //"MainWindow"
					bfn = fn.Substring(0, fn.Length - ext.Length); //"ProDevis.UI.ViewModels.MainWindow"
					break;
				}
			}
			var viewExt = "View";
			var vn      = bn  + viewExt; //"MainWindowView"
			var fvn     = bfn + viewExt; //"ProDevis.UI.ViewModels.MainWindowView"

			Type t = null;
			t = viewModelType.Assembly.GetType(fvn, false);
			if (t != null) return t;
			t = viewModelType.Assembly.GetType(bfn, false);
			if (t != null) return t;

			var allTypes = viewModelType.Assembly.GetTypes();
			var m        = allTypes.Where(x => x.Name == vn).ToArray();
			if (m.Length == 1) return m[0];
			m = allTypes.Where(x => x.Name == bn).ToArray();
			if (m.Length == 1) return m[0];

			if(throwOnError) throw new TypeLoadException("View not found! Name: " + vn + "|" + bn + " ViewModel: " + viewModelType.FullName);
			return null;
		}
	}
}
