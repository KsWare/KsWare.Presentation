using System;
using System.Collections.Generic;
using System.Windows;

namespace KsWare.Presentation.ViewFramework {

	public class SharedResourceDictionary : ResourceDictionary {

		private static readonly Dictionary<Uri, WeakReference> SharedCache = new Dictionary<Uri, WeakReference>();

		private Uri _sourceCore;

		public SharedResourceDictionary() {
			
		}

		public bool DisableCache { get; set; }

		public new Uri Source {
			get => _sourceCore;
			set {
				_sourceCore = value;
				if (!SharedCache.ContainsKey(_sourceCore) || DisableCache) {
					base.Source = _sourceCore;
					if (DisableCache) return;
					CacheSource();
				}
				else {
					ResourceDictionary target = (ResourceDictionary) SharedCache[_sourceCore].Target;
					if (target != null) {
						MergedDictionaries.Add(target);
						_sourceCore = target.Source;
					}
					else {
						base.Source = _sourceCore;
						CacheSource();
					}
				}
			}
		}

		private void CacheSource() {
			if (SharedCache.ContainsKey(_sourceCore)) SharedCache.Remove(_sourceCore);
			SharedCache.Add(_sourceCore, new WeakReference((object) this, false));
		}
	}
}