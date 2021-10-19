using System;
using System.Threading.Tasks;
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

		[Test]
		public void RegisterActionMethod() {
			var x = new SampleVM();
			x.TestAction.Execute(); Assert.That(x.Result, Is.EqualTo("SampleBaseVM.DoTest"));
			x.TestPAction.Execute("A"); Assert.That(x.Result, Is.EqualTo("SampleBaseVM.DoTestP-A"));
			x.Test3Action.Execute(); Assert.That(x.Result, Is.EqualTo("SampleVM.DoTest3"));
			x.Test4Action.Execute(); Assert.That(x.Result, Is.EqualTo("SampleVM.DoTest4"));
		}

		private class SampleVM : SampleBaseVM{
			/// <inheritdoc />
			public SampleVM() {
				RegisterChildren(()=>this);
			}

			public ActionVM Test3Action { get; private set; }
			private void DoTest3(){ Result = "SampleVM.DoTest3";}

			protected override void DoTest4(){ Result = "SampleVM.DoTest4";}
		}

		private class SampleBaseVM : ObjectVM {

			/// <inheritdoc />
			public SampleBaseVM() {
				RegisterChildren(()=>this);
			}

			public string Result { get; set; }
			
			public ActionVM TestAction { get; private set; }
			public ActionVM TestPAction { get; private set; }
			public ActionVM Test2Action { get; private set; }
			public ActionVM Test2PAction { get; private set; }
			public ActionVM Test4Action { get; private set; }

			private void DoTest() { Result = "SampleBaseVM.DoTest";}	
			private void DoTestP(object parameter){Result=$"SampleBaseVM.DoTestP-{parameter}";}

			private Task DoTest2Async() { throw new Exception();}
			private Task DoTest2PAsync(object parameter){ throw new Exception();}

			protected virtual void DoTest4() { Result = "SampleBaseVM.DoTest4";}	
		}
	}

	// ReSharper restore InconsistentNaming
}