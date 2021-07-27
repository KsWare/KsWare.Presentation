using System;
using KsWare.Presentation.BusinessFramework;
using NUnit.Framework;
using Assert=NUnit.Framework.Assert;
using Is=NUnit.Framework.Is;

namespace KsWare.Presentation.Tests.BusinessFramework {

	[TestFixture]
	public class ObjectBMːHierarchyTests {

		[Test]
		public void RegisterChildːBoolBMˑCustomDataProviderˑTest() {
			var data = new TestClass1Data();
			var bm = new TestClass1BM();
			bm.Data = data;
			Assert.That(data.MyBool,Is.EqualTo(false));
			Assert.That(bm.MyBool.Value,Is.EqualTo(false));

			bm.MyBool.Value = true;

			Assert.That(data.MyBool,Is.EqualTo(true));
			Assert.That(bm.MyBool.Value,Is.EqualTo(true));

			bm.MyBool.Value = false;

			Assert.That(data.MyBool,Is.EqualTo(false));
			Assert.That(bm.MyBool.Value,Is.EqualTo(false));

			data.MyBool = true; //this will not raise PropertyChanged

			Assert.That(data.MyBool,Is.EqualTo(true));
			Assert.That(bm.MyBool.Value,Is.EqualTo(true));
		}

		[Test]
		public void RegisterChildːEnumBMˑCustomDataProviderˑTest() {
			var data = new TestClass2Data();
			var bm = new TestClass2BM();
			bm.Data = data;

			Assert.That(data.MyEnum,Is.EqualTo(default(DayOfWeek)));
			Assert.That(bm.MyEnum.Value,Is.EqualTo(default(DayOfWeek)));

			bm.MyEnum.Value = DayOfWeek.Wednesday;

			Assert.That(data.MyEnum,Is.EqualTo(DayOfWeek.Wednesday));
			Assert.That(bm.MyEnum.Value,Is.EqualTo(DayOfWeek.Wednesday));

			bm.MyEnum.Value = DayOfWeek.Thursday;

			Assert.That(data.MyEnum,Is.EqualTo(DayOfWeek.Thursday));
			Assert.That(bm.MyEnum.Value,Is.EqualTo(DayOfWeek.Thursday));

			data.MyEnum = DayOfWeek.Friday; //this will not raise PropertyChanged

			Assert.That(data.MyEnum,Is.EqualTo(DayOfWeek.Friday));
			Assert.That(bm.MyEnum.Value,Is.EqualTo(DayOfWeek.Friday));
		}

		[Test]
		public void RegisterChildːInvalidBMˑCustomDataProviderˑTest() {
			Assert.Throws<ArgumentOutOfRangeException>(() => new TestClass3BM());
		}

		public class TestClass1BM : DataBM<TestClass1Data> {

			public TestClass1BM() {
				MyBool=RegisterChild<BoolBM,bool>("MyBool",() => Data.MyBool, b => Data.MyBool=b);
			}

			public BoolBM MyBool { get; private set; }

		}

		public class TestClass2BM : DataBM<TestClass2Data> {

			public TestClass2BM() {
				MyEnum=RegisterChild<EnumBM<DayOfWeek>,DayOfWeek>("MyEnum",() => Data.MyEnum, b => Data.MyEnum=b);
			}

			public EnumBM<DayOfWeek> MyEnum { get; private set; }

		}

		public class TestClass3BM : DataBM<Object> {

			public TestClass3BM() {
				MyObject=RegisterChild<ObjectBM,object>("MyObject",() => null, b => b=b);
			}

			public ObjectBM MyObject { get; private set; }

		}

		public class TestClass1Data {
			public bool MyBool { get; set; }
		}

		public class TestClass2Data {
			public DayOfWeek MyEnum { get; set; }
		} 
	}

}