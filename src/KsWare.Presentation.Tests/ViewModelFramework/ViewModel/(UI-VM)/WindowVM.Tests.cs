﻿using System;
using System.Threading;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;
using Assert=NUnit.Framework.Assert;
using Is=NUnit.Framework.Is;
namespace KsWare.Presentation.Tests.ViewModelFramework {

	[TestFixture]
	public class WindowVMTests:ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public override void TestInitialize() {
			base.TestInitialize();
		}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public override void TestCleanup() {
			if(TestSubject!=null) {TestSubject.Dispose(); TestSubject=null;}
			base.TestCleanup();
		}

		public WindowVM TestSubject { get; set; }

		[Test][Ignore("TODO") /*this would create a real window, test this as standalone*/]
		public void Show() {
			TestSubject = new WindowVM();
			TestSubject.UIAccess.IsDirectAccessEnabled = true;
			TestSubject.Show();

			Assert.AreEqual(1,ApplicationVM.Current.Windows.Count);
			Assert.AreEqual(TestSubject,ApplicationVM.Current.MainWindow);

			Assert.That(TestSubject.UIAccess.HasWindow      ,Is.True, "HasWindow");

			//in automated unit test this will be not allays true
			Assert.That(TestSubject.UIAccess.Window.IsActive,Is.True, "Window.IsActive");

			Thread.Sleep(500);
			TestSubject.CloseAction.Execute(null);
		}

		[Test][Apartment(ApartmentState.STA)]
		public void CloseWindow() {
			TestSubject = new WindowVM();
			TestSubject.UIAccess.IsDirectAccessEnabled = true;
			TestSubject.Show();

			TestSubject.UIAccess.Window.Close();
		}

	}
}
