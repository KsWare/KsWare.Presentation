using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KsWare.Presentation.ViewFramework.Controls
{

	/// <summary> Interaction logic for ResourcePresenter.xaml
	/// </summary>
	/// <remarks>
	/// The ResourcePresenter shows object from resource file
	/// </remarks>
	public sealed partial class ResourcePresenter 
	{

		#region Source Property

		/// <summary> Provides the SourceProperty
		/// </summary>
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
			"Source", typeof (Uri), typeof (ResourcePresenter), new PropertyMetadata(null));

		/// <summary> Gets or sets the source.
		/// </summary>
		/// <value>The source.</value>
		/// <example>
		/// Source="pack://application:,,,/Resources/AFolder/AFile.xaml"/>
		/// </example>
		public Uri Source {
			get {return (Uri) GetValue(SourceProperty);}
			set {SetValue(SourceProperty, value);}
		}

		public static readonly DependencyProperty SourceFormatProperty =
			DependencyProperty.Register("SourceFormat", typeof(Uri), typeof(ResourcePresenter), new PropertyMetadata(null,
				(d, e) => ((ResourcePresenter) d).UpdateSource()));

		#endregion

		#region SourceFormat Property

		public Uri SourceFormat {
			get { return (Uri)GetValue(SourceFormatProperty); }
			set { SetValue(SourceFormatProperty, value); }
		}

		public static readonly DependencyProperty SourceParameter0Property =
			DependencyProperty.Register("SourceParameter0", typeof(string), typeof(ResourcePresenter), new PropertyMetadata(null,
			(d, e) => ((ResourcePresenter) d).UpdateSource()));

		public string SourceParameter0 {
			get { return (string)GetValue(SourceParameter0Property); }
			set { SetValue(SourceParameter0Property, value); }
		}

		public static readonly DependencyProperty SourceParameter1Property =
			DependencyProperty.Register("SourceParameter1", typeof(string), typeof(ResourcePresenter), new PropertyMetadata(null,
			(d, e) => ((ResourcePresenter) d).UpdateSource()));

		public string SourceParameter1 {
			get { return (string)GetValue(SourceParameter1Property); }
			set { SetValue(SourceParameter1Property, value); }
		}

		public static readonly DependencyProperty SourceParameter2Property =
			DependencyProperty.Register("SourceParameter2", typeof(string), typeof(ResourcePresenter), new PropertyMetadata(null,
			(d, e) => ((ResourcePresenter) d).UpdateSource()));

		public string SourceParameter2 {
			get { return (string)GetValue(SourceParameter2Property); }
			set { SetValue(SourceParameter2Property, value); }
		}

		private void UpdateSource() {UpdateSource(SourceFormat, new []{SourceParameter0,SourceParameter1,SourceParameter2});}
		
		private void UpdateSource(Uri sourceFormat, string[] sourceParameters) {
			if(sourceFormat==null) {
				ClearValue(SourceProperty);
			} else {
				var v = sourceFormat;
				string s = sourceFormat.OriginalString;
				for(int i = 0; i<sourceParameters.Length; i++) {
					string p = sourceParameters[i];
					if(!string.IsNullOrEmpty(p)) {
						s=s.Replace("{"+i+"}", p);
					}
				}
				v=new Uri(s,sourceFormat.IsAbsoluteUri?UriKind.Absolute :UriKind.Relative);
				Source = v;
			}
		}

		#endregion

		/// <summary> Initializes a new instance of the <see cref="ResourcePresenter"/> class.
		/// </summary>
		/// <remarks></remarks>
		public ResourcePresenter() {
			InitializeComponent();
		}
	}
}
