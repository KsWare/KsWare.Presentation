using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Testing.Tests {

	[TestFixture]
	public class ApplicationVMTestBaseTests:ApplicationVMTestBase {


		[SetUp]
		public override void TestInitialize() { base.TestInitialize(); }

		[TearDown]
		public override void TestCleanup() { base.TestCleanup(); }

		[Test]
		public void Dispatcher_Invoke() {
			var invokeCount = 0;
			System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(new Action(() => invokeCount++));
			Assert.AreEqual(1,invokeCount);
		}

		[Test]
		[SuppressMessage("ReSharper", "AsyncConverter.AsyncWait")]
		public void Dispatcher_BeginInvoke() {
			var invokeCount = 0;
			var dispatcherOperation = System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => invokeCount++));
			var dispatcherOperationStatus = dispatcherOperation.Wait(TimeSpan.FromSeconds(1));
			Assert.AreEqual(1,invokeCount);
		}

		[Test]
		public void ApplicationDispatcher_Invoke() {
			var invokeCount = 0;
			ApplicationDispatcher.Instance.Invoke(new Action(() => invokeCount++));
			Assert.AreEqual(1,invokeCount);
		}

		[Test]
		[SuppressMessage("ReSharper", "AsyncConverter.AsyncWait")]
		public void ApplicationDispatcher_BeginInvoke() {
			var invokeCount = 0;
			var dispatcherOperation = ApplicationDispatcher.Instance.BeginInvoke(new Action(() => invokeCount++));
			var dispatcherOperationStatus = dispatcherOperation.Wait(TimeSpan.FromSeconds(1));
			Assert.AreEqual(1,invokeCount);
		}

	}

}

