using System;
using System.Windows.Markup;

namespace KsWare.Presentation.ViewFramework {

	public class DebugViewerExtension:MarkupExtension {

		public DebugViewerExtension() {}

		public DebugViewerExtension(object datacontext) {
			DataContext=datacontext;
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			if(DataContext==null) return "{Null}";
			return "{" + DataContext.GetType().Name + "}";
		}

		public object DataContext { get; set; }
	}
}
