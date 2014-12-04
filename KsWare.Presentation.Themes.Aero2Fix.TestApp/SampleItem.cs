using System.Dynamic;

namespace KsWare.Presentation.Themes.Aero2Fix.TestApp {
	public class SampleItem {

		public SampleItem() {
			MyBoolTrue = true;
			MyBoolFalse = false;
			MyInt32 = 123;
			MyString = "Text";
			MyLongString = "Text Text2 Text3 Text4 Text5 Text6 Text7 Text8 Text9 Text0 Text1 Text2 Text3 Text4 Text5 Text6";
			MyLongStringWithLineBreaks = "Line 1\r\nLine 2\r\nLine 3";
		}

		public bool MyBoolTrue { get; set; }
		public bool MyBoolFalse { get; set; }
		public int MyInt32 { get; set; }
		public string MyString { get; set; }
		public string MyLongString { get; set; }
		public string MyLongStringWithLineBreaks { get; set; }

		public static string[] StaticStrings { get { return new[] {"Text", "Text 2", "Text 3"}; } }

	}
}
