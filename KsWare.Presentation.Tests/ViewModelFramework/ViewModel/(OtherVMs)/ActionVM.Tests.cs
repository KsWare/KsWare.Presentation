using System;
using JetBrains.Annotations;
using KsWare.Presentation;
using KsWare.Presentation.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.ViewModelFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="ActionVM"/>-class
	/// </summary>
	[TestClass]
	public class ActionVMTests {

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
		public void Common() {
			var c = 0;
			var vm=new ActionVM {
				Metadata = {
					ActionProvider = {
						ExecutedCallback =delegate(object sender, ExecutedEventArgs args) {
							Assert.AreEqual("Call2",args.Parameter);
							c++;
						}
					}
				}
			};
			
			vm.SetCanExecute("test",false);
			Assert.IsFalse(vm.CanExecute);
			Assert.Throws<NotSupportedException>(() => vm.Execute("Call1"));
			Assert.AreEqual(0,c);
			vm.SetCanExecute("test",true);
			Assert.IsTrue(vm.CanExecute);
			vm.Execute("Call2");
			Assert.AreEqual(1,c);
		}

		[TestMethod]
		public void AutoActionMethodAssignment() {
			var sut = new TestVM();
			sut.EditAction.Execute();
			Assert.That(sut.TestResult, Is.EqualTo("DoEdit"));
			sut.EditParameterAction.Execute("test");
			Assert.That(sut.TestResult, Is.EqualTo("DoEditParameter test"));
		}

		public class TestVM:ObjectVM {

			public TestVM() {
				RegisterChildren(()=>this);
			}

			public ActionVM EditAction { get; [UsedImplicitly] private set; }
			public ActionVM EditParameterAction { get; [UsedImplicitly] private set; }

			private void DoEdit() { TestResult = "DoEdit"; }
			private void DoEditParameter(object p) { TestResult = "DoEditParameter "+p; }

			public string TestResult { get; set; }
		}

	}

	// ReSharper restore InconsistentNaming
}