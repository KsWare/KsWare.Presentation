using System;
using System.Windows.Markup;

namespace KsWare.Presentation.ViewFramework {

	// Was soll diese Markup Erweiterung eigentlich machen?
	// vom Namen her soll sie etwas anzeigen zum Finden von Fehlern
	// TODO suchen wo diese Markup Erweiterung bereits verwendet wurde.
	// Kann sonst gelöscht werden wegen zu wenig funktionalität.

	/// <summary>
	/// [OBSOLETE] Class DebugViewerExtension.
	/// </summary>
	/// <seealso cref="System.Windows.Markup.MarkupExtension" />
	[Obsolete("Still required?",true)]
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
