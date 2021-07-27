using System;
using NUnit.Framework;
using KsWare.Presentation.ViewModelFramework;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.ViewModelFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="ActionVM"/>-class
	/// </summary>
	[TestFixture]
	public class ActionVMTests {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public void Teardown() { }

		/// <summary> 
		/// </summary>
		[Test]
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