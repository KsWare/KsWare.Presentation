using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Baml2006;
using System.Xaml;

namespace KsWare.Presentation.ViewFramework {
	
	public class ConcatResourceDictionary: ResourceDictionary {
		private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

		private readonly ObservableCollection<ResourceDictionary> _concatDictionaries;
		private readonly ObservableCollection<Uri> _concatUris;

		public ConcatResourceDictionary() {
			_concatDictionaries=new ObservableCollection<ResourceDictionary>();
			_concatDictionaries.CollectionChanged += (s, e) => Concat(e.NewItems?.Cast<ResourceDictionary>());
			_concatUris=new ObservableCollection<Uri>();
			_concatUris.CollectionChanged += (s, e) => Concat(e.NewItems?.Cast<Uri>());
		}

		public bool DesignModeOnly { get; set; }

		public Collection<ResourceDictionary> ConcatDictionaries => (Collection<ResourceDictionary>) _concatDictionaries;

		public Collection<Uri> ConcatUris => (Collection<Uri>) _concatUris;

		private void Concat(IEnumerable<Uri> uris) {
			if (uris == null) return;
			if(DesignModeOnly && IsInDesignMode) return;
			foreach (var uri in uris) {
				var                              info   = Application.GetResourceStream(uri);
				var                              rd     = ReadBaml<ResourceDictionary>(info.Stream);
				Concat(new[] {rd});
			}
		}

		private static T ReadBaml<T>(Stream stream) {
			var bamlReader = new Baml2006Reader(stream);
			var writer     = new XamlObjectWriter(bamlReader.SchemaContext);
			while (bamlReader.Read()) writer.WriteNode(bamlReader);
			return (T)writer.Result;
		}

		private void Concat(IEnumerable<ResourceDictionary> dictionaries) {
			if(dictionaries==null) return;
			foreach (var resourceDictionary in dictionaries) {
				if (resourceDictionary.Source != null) {
					var info = Application.GetResourceStream(resourceDictionary.Source);
					var rd = ReadBaml<ResourceDictionary>(info.Stream);
					Concat(new []{rd});
				} else if (resourceDictionary.MergedDictionaries.Count > 0) {
					Concat(resourceDictionary.MergedDictionaries);
				}
				else {
					foreach (DictionaryEntry entry in resourceDictionary) {
						base.Remove(entry.Key);
						base.Add(entry.Key,entry.Value);
					}					
				}
			}
		}


	}
}
