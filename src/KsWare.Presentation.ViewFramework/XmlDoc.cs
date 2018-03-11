using System;
using System.Windows;
using System.Windows.Markup;

namespace KsWare.Presentation.ViewFramework {

	public class XmlDoc {

/* http://msdn.microsoft.com/de-de/library/5ast78ax.aspx
 <c>
<para>
<see>*
<code>
<param>*
<seealso>*
<example>
<paramref>
<summary>
<exception>*
<permission>*
<typeparam>*
<include>*
<remarks>
<typeparamref>
<list>
<returns>
<value> 
*/
		public static readonly DependencyProperty SummaryProperty = DependencyProperty.RegisterAttached(
			"Summary", typeof (string), typeof (XmlDoc), new PropertyMetadata(default(string)));

		public static void SetSummary(DependencyObject element, string value) { element.SetValue(SummaryProperty, value); }

		public static string GetSummary(DependencyObject element) { return (string) element.GetValue(SummaryProperty); }

		public static readonly DependencyProperty RemarksProperty = DependencyProperty.RegisterAttached(
			"Remarks", typeof (string), typeof (XmlDoc), new PropertyMetadata(default(string)));

		public static void SetRemarks(DependencyObject element, string value) { element.SetValue(RemarksProperty, value); }

		public static string GetRemarks(DependencyObject element) { return (string) element.GetValue(RemarksProperty); }

		public static readonly DependencyProperty ReturnsProperty = DependencyProperty.RegisterAttached(
			"Returns", typeof (string), typeof (XmlDoc), new PropertyMetadata(default(string)));

		public static void SetReturns(DependencyObject element, string value) { element.SetValue(ReturnsProperty, value); }

		public static string GetReturns(DependencyObject element) { return (string) element.GetValue(ReturnsProperty); }

		public static readonly DependencyProperty SeeAlsoProperty = DependencyProperty.RegisterAttached(
			"SeeAlso", typeof (string), typeof (XmlDoc), new PropertyMetadata(default(string)));

		public static void SetSeeAlso(DependencyObject element, string value) { element.SetValue(SeeAlsoProperty, value); }

		public static string GetSeeAlso(DependencyObject element) { return (string) element.GetValue(SeeAlsoProperty); }


	}

	namespace XmlDocMarkup {

		[MarkupExtensionReturnType(typeof (string))]
		public class doc : MarkupExtension {

			//DOESN'T WORK public Doc(params string[]args) {}
			public doc(string p00) {}
			public doc(string p00,string p01) {}
			public doc(string p00,string p01,string p02) {}
			public doc(string p00,string p01,string p02,string p03) {}
			public doc(string p00,string p01,string p02,string p03,string p04) {}
			public doc(string p00,string p01,string p02,string p03,string p04,string p05) {}
			public doc(string p00,string p01,string p02,string p03,string p04,string p05,string p06) {}
			public doc(string p00,string p01,string p02,string p03,string p04,string p05,string p06,string p07) {}
			public doc(string p00,string p01,string p02,string p03,string p04,string p05,string p06,string p07,string p08) {}

			public override object ProvideValue(IServiceProvider serviceProvider) { return null; }

			string P00 { get; set; }
			string P01 { get; set; }
			string P02 { get; set; }
			string P03 { get; set; }
			string P04 { get; set; }
			string P05 { get; set; }
			string P06 { get; set; }
		}

		[MarkupExtensionReturnType(typeof (string))]
		public class see : MarkupExtension {
			public override object ProvideValue(IServiceProvider serviceProvider) { return href; }
			public string href {get; set; }
		}

		[MarkupExtensionReturnType(typeof (string))]
		public class code : MarkupExtension {

			public code(string content) { Content = content; }

			public override object ProvideValue(IServiceProvider serviceProvider) { return Content; }

			[ConstructorArgument("content")] 
			public string Content {get; set; }
		}
		
	}

}
