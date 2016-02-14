using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KsWare.Presentation.ViewFramework {

	public abstract class GroupStyleSelector/*:DataTemplateSelector*/ {

		public abstract GroupStyle SelectTemplate(CollectionViewGroup @group, int level);

	}

	public class ResourceGroupStyleSelector:GroupStyleSelector {

		public override GroupStyle SelectTemplate(CollectionViewGroup @group, int level) {

			//'group' is the parent of current group item (and null for level=0). current group item is not available!
//			string groupKey = "GroupName:"+(@group==null ? "{Null}" : (@group.Name==null ? "{Null}" : @group.Name.ToString()));
//			var levelKey = "Level:"+level;
//
//			string key = groupKey+"+"+levelKey;
//			if(!Resources.Contains(key)) {
//				key = groupKey;
//				if(!Resources.Contains(key)) {
//					key = levelKey;
//					if(!Resources.Contains(key)) return null;
//				}
//			}

			var key = level.ToString(CultureInfo.InvariantCulture);
			if(!Resources.Contains(key)) return null;

			var groupStyle = Resources[key] as GroupStyle;
			return groupStyle;
		}

		public ResourceDictionary Resources { get; set; }

		public ResourceGroupStyleSelector() {
			Resources = new ResourceDictionary();
		}
	}

}