using System;
using System.Collections.ObjectModel;
using KsWare.Presentation.ViewModelFramework.DesignTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.ViewModelFramework.Tests {

	[TestClass]
	public class ViewModelCodeGenerator4DataObjectsTests {


		[TestMethod]
		public void GenarateMissingProperties() {
			ViewModelCodeGenerator4DataObjects.GenerateMissingProperties(typeof (TestData), typeof (TestVM));
		}


		private class TestData {

			public string[] ArrayOfString { get; set; }
			public string[][] JaggedArrayOfString { get; set; }
			public string[,] MultiDimArrayOfString { get; set; }
			public Array Array { get; set; }
			public ReadOnlyCollection<string> ReadOnlyCollectionOfString { get; set; }
		}

		private class TestVM:DataVM<TestData>{}
	}
}
