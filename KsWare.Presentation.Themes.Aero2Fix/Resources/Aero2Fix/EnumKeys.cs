using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace KsWare.Presentation.Themes.Aero2Fix.Resources.Aero2Fix {

	internal class ResourceDictionaryUtil {

		public static void EnumKeys() {
			//This runs only in a WPF application

			ResourceDictionary res = Application.LoadComponent(
 new Uri("/KsWare.Presentation.Themes.Aero2Fix;component/Resources/Aero2Fix/+Aero2FixOverride.xaml", 
 UriKind.RelativeOrAbsolute)) as ResourceDictionary;

			var skin1=new ResourceDictionary {Source = new Uri("/KsWare.Presentation.Themes.Aero2Fix;component/Resources/Aero2Fix/+Aero2FixOverride.xaml", UriKind.Relative)};

			var allKeys=GetKeys(skin1).ToList();
			allKeys.Sort((key1, key2) => key1.DistinctName.CompareTo(key2.DistinctName));

			foreach (var key in allKeys) {
				Debug.WriteLine("{0} {1,-64} {2} "," ", key.DisplayName, key.Source);
			}

			Debug.WriteLine(@"
//┌─ OK in both
//│ ┌─ Dark.xaml
//│ │ ┌─ Light.xaml");
//			foreach (var key in allKeys) {
//				var ok1 = skin1.Contains(key.Value);
//				var ok2 = skin2.Contains(key.Value);
//
////				Debug.WriteLine("{0} {1} {2} {3} {4}",ok1&&ok2?"·":"!", ok1?"·":"?",ok2?"·":"?",key.DisplayName, ok1&&ok2?"":" in "+key.Source);
//				Debug.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",ok1&&ok2?"·":"!", ok1?"·":"?",ok2?"·":"?",key.DisplayName, ok1&&ok2?"":key.Source);
//			}
		}

		private static List<Key> Distinct(List<Key> allKeys) {
			var allKeyNames = allKeys.Distinct(new KeyEqualityComparer()).Select(x=>x.DistinctName).ToList();
			var distinctList = new List<Key>();
			foreach (var keyName in allKeyNames) {
				var l=allKeys.Where(x => x.DistinctName == keyName).ToList();
				distinctList.Add(new Key(l[0].Value,null){All=l,Source = l.Last().Source});

			}
			return distinctList;
		}

		private static IEnumerable<Key> GetKeys(ResourceDictionary rd) {
			var list = rd.Keys.OfType<object>().Select(x => new Key(x,rd)).ToList();
			foreach (var md in rd.MergedDictionaries) {
				list.AddRange(GetKeys(md));
			}
			return list;
		}

		internal class Key {

			public Key(object key, ResourceDictionary resourceDictionary) {
				Value = key;

				if (key is Type) { }
				else if (key is string) { } 
		        else if (key.GetType().FullName=="System.Windows.SystemThemeKey") { } 
		        else if (key.GetType().FullName=="System.Windows.SystemResourceKey") { } 
		        else if (key.GetType().FullName=="System.Windows.ComponentResourceKey") { } 
				else { Debugger.Break();}

				DisplayName = key.ToString() + " {" + key.GetType().Name+"}" ;
				DistinctName= key.ToString();//TODO
				OrderName = key.ToString();//TODO

				if (resourceDictionary != null) {
					Source = resourceDictionary.Source.OriginalString;
//					Value2 = resourceDictionary[key];
//					Value2TypeName = Value2.GetType().Name;
				}
			}

			public object Value { get; set; }
			public string DisplayName { get; set; }
			public string OrderName { get; set; }
			public string DistinctName { get; set; }
			public string Source { get; set; }
			public List<Key> All { get; set; }

			public object Value2 { get; set; }
			public string Value2TypeName { get; set; }
		}

		internal class KeyEqualityComparer : IEqualityComparer<Key> {

			public bool Equals(Key x, Key y) { return Equals(x.DistinctName, y.DistinctName); }
			public int GetHashCode(Key obj) { return obj.DistinctName.GetHashCode(); }

		}
	}
}
