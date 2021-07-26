using System;
using System.Collections.ObjectModel;
using KsWare.Presentation.ViewModelFramework.DesignTime;
using NUnit.Framework;

namespace KsWare.Presentation.ViewModelFramework.Tests {

	[TestFixture]
	public class ViewModelCodeGenerator4DataObjectsTests {


		[Test]
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
