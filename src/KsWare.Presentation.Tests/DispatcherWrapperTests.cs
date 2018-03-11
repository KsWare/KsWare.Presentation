using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests {

	[TestClass]
	public class DispatcherWrapperTests {

		[TestMethod]
		public void ParamsTest() {
//			Assert.AreEqual(-1,MethodWithParams());
			Assert.AreEqual( 0,MethodWithParams(             ));
			Assert.AreEqual( 2,MethodWithParams(0,1          ));
			Assert.AreEqual(-1,MethodWithParams(null         )); // !!!  null      as single parameter, unexpected beavior
			Assert.AreEqual( 0,MethodWithParams(new object[0])); // !!!  object[0] as single parameter, unexpected beavior
			Assert.AreEqual( 1,MethodWithParams(new object[1])); // !!!  object[1] as single parameter, unexpected beavior
			Assert.AreEqual( 2,MethodWithParams(new object[2])); // !!!  object[2] as single parameter, unexpected beavior
			Assert.AreEqual( 2,MethodWithParams(null,null    ));
			Assert.AreEqual( 2,MethodWithParams(null,new object[0]));

			// specifying null or object[] as single parameter replaces the params array and will NOT interpreted as 1 parameter
		}

		private int MethodWithParams(params object[] args) {
			if (args == null) return -1;
			return args.Length;
		}

	}

}