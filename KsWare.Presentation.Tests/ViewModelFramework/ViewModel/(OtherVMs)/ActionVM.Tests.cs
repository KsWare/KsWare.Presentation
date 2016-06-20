using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.Presentation.ViewModelFramework;
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

	}

	// ReSharper restore InconsistentNaming
}