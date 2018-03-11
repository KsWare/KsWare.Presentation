using System;
using MSTest=Microsoft.VisualStudio.TestTools.UnitTesting;


namespace KsWare.Presentation.Testing {

	public partial class NUnit {

		public static class Assert {

			public static void Throws<T>(Action action) {Throws(typeof (T), action);}

			public static void Throws(Type type, Action action) {
				try {
					action();
					MSTest.Assert.IsInstanceOfType(null, type);
				} catch (Exception ex) {
					MSTest.Assert.IsInstanceOfType(ex, type);
				}
			}

			public static void Catch<T>(Action func) {
				try {
					func();
				} catch (Exception ex) {
					MSTest.Assert.IsInstanceOfType(ex, typeof(T));
				}
			}

		}
		

	}


}