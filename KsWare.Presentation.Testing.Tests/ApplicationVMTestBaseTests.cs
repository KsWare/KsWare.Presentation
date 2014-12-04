using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Testing.Tests {

	[TestClass]
	public class ApplicationVMTestBaseTests:ApplicationVMTestBase {


		[TestInitialize]
		public override void TestInitialize() { base.TestInitialize(); }

		[TestCleanup]
		public override void TestCleanup() { base.TestCleanup(); }

		private volatile int InvokeCount;

		[TestMethod]
		public void Dispatcher_Invoke() {
			System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(new Action(() => InvokeCount++));
			Assert.AreEqual(1,InvokeCount);
		}

		[TestMethod]
		public void Dispatcher_BeginInvoke() {
			var dispatcherOperation = System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => InvokeCount++));
			var dispatcherOperationStatus = dispatcherOperation.Wait(TimeSpan.FromSeconds(1));
			Assert.AreEqual(1,InvokeCount);
		}

		[TestMethod]
		public void ApplicationDispatcher_Invoke() {
			ApplicationDispatcher.CurrentDispatcher.Invoke(new Action(() => InvokeCount++));
			Assert.AreEqual(1,InvokeCount);
		}
		[TestMethod]
		public void ApplicationDispatcher_BeginInvoke() {
			var dispatcherOperation = ApplicationDispatcher.CurrentDispatcher.BeginInvoke(new Action(() => InvokeCount++));
			var dispatcherOperationStatus = dispatcherOperation.Wait(TimeSpan.FromSeconds(1));
			Assert.AreEqual(1,InvokeCount);
		}

	}

}

