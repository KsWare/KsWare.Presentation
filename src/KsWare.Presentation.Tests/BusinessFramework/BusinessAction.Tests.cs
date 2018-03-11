using System;
using KsWare.Presentation.BusinessFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.BusinessFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="ActionBM"/>-class
	/// </summary>
	[TestClass]
	public class BusinessActionTests {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() { }

		/// <summary> 
		/// </summary>
		[TestMethod]
		public void UsageTest() {
			ExecutedEventArgs executeCallbackArgs = null;
			int executeCallbackCount = 0;
			int canExecuteChangedCount = 0;

			//Business setup:
			var action = new ActionBM {
				Metadata = new BusinessActionMetadata {
					ExecuteCallback = delegate(object sender, ExecutedEventArgs args) {
						executeCallbackArgs = args;
						executeCallbackCount++;
					}
				}
			};

			//View setup:
			action.CanExecuteChanged+=delegate(object sender, EventArgs args) {
				canExecuteChangedCount++;
			};

			//Test
			((BusinessActionMetadata)action.Metadata).SetCanExecute("A",false);
			Assert.IsFalse(action.CanExecute);
			action.Execute("Test1");
			Assert.IsNull(executeCallbackArgs);

			((BusinessActionMetadata)action.Metadata).SetCanExecute("A",true);
			Assert.IsTrue(action.CanExecute);
			action.Execute("Test2");
			Assert.IsNotNull(executeCallbackArgs);
			Assert.AreEqual("Test2",executeCallbackArgs.Parameter);
		}
	}

	// ReSharper restore InconsistentNaming
}