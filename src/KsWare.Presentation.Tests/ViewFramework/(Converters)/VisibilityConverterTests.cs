using System.Windows;
using KsWare.Presentation.ViewFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.ViewFramework {

	[TestClass]
	public class VisibilityConverterTests {

		[TestMethod]
		public void EqualVisibleElseCollapsed() {
			Assert.AreEqual(Visibility.Visible  , (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert('A',typeof(Visibility),'A',null));
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert('A',typeof(Visibility),'B',null));
		}

		[TestMethod]
		public void NotEqualVisibleElseCollapsed() {
			Assert.AreEqual(Visibility.Visible  , (Visibility)VisibilityConverter.NotEqualVisibleElseCollapsed.Convert('A',typeof(Visibility),'B',null));
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.NotEqualVisibleElseCollapsed.Convert('A',typeof(Visibility),'A',null));
		}

		[TestMethod]
		public void EqualVisibleElseCollapsed_Double() {
			double a = 1;
			double b1 = 0.99999999;
			double b2 = 0.9999999;
			Assert.AreEqual(Visibility.Visible  , (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),a,null));
			Assert.AreEqual(Visibility.Visible  , (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),b1,null));
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),b2,null));
		}

		[TestMethod]
		public void EqualVisibleElseCollapsed_Double_String() {
			double a = 1;
			var b1 = "0.99999999";
			var b2 = "0.9999999";
			Assert.AreEqual(Visibility.Visible  , (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),a,null));
			Assert.AreEqual(Visibility.Visible  , (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),b1,null));
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),b2,null));
		}

		[TestMethod]
		public void EqualVisibleElseCollapsed_Double_String_NaN() {
			double a = double.NaN;
			var b1 = "NaN";
			Assert.AreEqual(Visibility.Visible  , (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),a,null));
			Assert.AreEqual(Visibility.Visible  , (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),b1,null));
		}
		[TestMethod]
		public void EqualVisibleElseCollapsed_Double_String_InvalidNumber() {
			double a = 1;
			var b = "invalid";
			Assert.AreEqual(Visibility.Collapsed  , (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a,typeof(Visibility),b,null));
		}
	}
}
