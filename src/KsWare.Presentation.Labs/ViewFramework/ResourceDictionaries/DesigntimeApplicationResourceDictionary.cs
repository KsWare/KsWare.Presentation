using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Baml2006;
using System.Xaml;
using System.Xml;

namespace KsWare.Presentation.ViewFramework {
	
	public class DesigntimeApplicationResourceDictionary : ResourceDictionary {
		private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
		private Uri _source;

		public DesigntimeApplicationResourceDictionary() {
			if(!IsInDesignMode) return;
			throw new WarningException($"{Application.Current.Resources.GetType().Name}");
		}

		public new Uri Source {
			get => _source;
			set {
				_source = value;
//				if (!IsInDesignMode) return;
				//TODO continue here
				var rd=ApplicationDefinition2ResourceDictionary(_source);
			}
		}

		private ResourceDictionary ApplicationDefinition2ResourceDictionary(Uri applicationDefinition) {
			var info=Application.GetResourceStream(applicationDefinition);
			var xamlDoc = Baml2Xaml(info.Stream);
			var appresources = xamlDoc.DocumentElement.FirstChild;
			

			return null;
		}

		private static XmlDocument Baml2Xaml(Stream stream) {
			var bamlReader = new Baml2006Reader(stream);
			var xamlStream=new MemoryStream();
			var writer     = new XamlXmlWriter(xamlStream, bamlReader.SchemaContext);
			while (bamlReader.Read()) writer.WriteNode(bamlReader);
			writer.Flush();
			xamlStream.Position = 0;
			var doc=new XmlDocument();
			doc.Load(xamlStream);
			return doc;
		}
	}
}
