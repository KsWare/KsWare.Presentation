using System.Diagnostics;
using KsWare.Presentation.BusinessFramework;
using NUnit.Framework;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.BusinessFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="BusinessObjectTreeHelper"/>-class
	/// </summary>
	[TestFixture]
	public class BusinessObjectTreeHelperTests {

		/// <summary> 
		/// </summary>
		[Test]
		public void CommonA() {
			int treeChangesCount = 0;
			BusinessObjectTreeHelper.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCount++; };
			var a = new ObjectA();
			a.B.C.Change();
			Debug.WriteLine("=>TreeChangesCount: " + treeChangesCount);
		}

		[Test]
		public void CommonB() {
			//LOG: using(this.LogBlock(Flow.Enter)) 
			{
				int treeChangesCount = 0;
				BusinessObjectTreeHelper.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCount++; };
				var a = new ObjectA();
				using(BusinessObjectTreeHelper.PauseTreeChangedEvents()) {
						a.B.C.Change();
				}
				Debug.WriteLine("=>TreeChangesCount: " + treeChangesCount);				
			}
		}

		[Test]
		public void CommonC() {
			int treeChangesCount = 0;
			var a = new ObjectA();
			a.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCount++; };
			a.B.C.Change();
			Debug.WriteLine("=>TreeChangesCount: " + treeChangesCount);
		}
		[Test]
		public void CommonD() {
			int treeChangesCountA=0,treeChangesCountB=0,treeChangesCountC = 0;
			var a = new ObjectA();
			a.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCountA++; };
			a.B.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCountB++; };
			a.B.C.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCountC++; };
			using(BusinessObjectTreeHelper.PauseTreeChangedEvents()) {
				a.B.C.Change();
			}
			Debug.WriteLine("=>TreeChangesCountA: " + treeChangesCountA);
			Debug.WriteLine("=>TreeChangesCountB: " + treeChangesCountB);
			Debug.WriteLine("=>TreeChangesCountC: " + treeChangesCountC);
		}

		[Test]
		public void CommonE() {
			int treeChangesCountA=0,treeChangesCountB=0,treeChangesCountC = 0;
			var a = new ObjectA();
			a.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCountA++; };
			a.B.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCountB++; };
			a.B.C.TreeChanged+=delegate(object sender, TreeChangedEventArgs args) { treeChangesCountC++; };
			using(BusinessObjectTreeHelper.PauseTreeChangedEvents()) {
				a.B.C.Change();
			}
			Debug.WriteLine("=>TreeChangesCountA: " + treeChangesCountA);
			Debug.WriteLine("=>TreeChangesCountB: " + treeChangesCountB);
			Debug.WriteLine("=>TreeChangesCountC: " + treeChangesCountC);
		}
		/// <summary> 
		/// </summary>
		[Test]
		public void RaiseTreeChangedEventDirect() {
			BusinessObjectTreeHelper.TreeChanged += delegate(object sender, TreeChangedEventArgs args) { Debug.WriteLine("=>" + new StackTrace()); };
			BusinessObjectTreeHelper.RaiseTreeChangedEventDirect();
		}

		[Test]
		public void RaiseTreeChangedEventWithHelper() {
			BusinessObjectTreeHelper.TreeChanged += delegate(object sender, TreeChangedEventArgs args) { Debug.WriteLine("=>" + new StackTrace()); };
			BusinessObjectTreeHelper.RaiseTreeChangedEventWithHelper();
		}

		private class ObjectA:ObjectBM
		{
			public ObjectA() {
				RegisterChild(B=new ObjectB{MemberName = "B"});
				B.TreeChanged+=If_ObjectC_Changed_Then_Change_ObjectB;
			}

			private void If_ObjectC_Changed_Then_Change_ObjectB(object sender, TreeChangedEventArgs args) {
				//LOG: using(this.LogBlock(Flow.Enter.EventHandler.Current.Parameter("Source",args.OriginalSource.GetType().Name))) 
				{
					if (args.OriginalSource == this.B.C) {
						using (BusinessObjectTreeHelper.PauseTreeChangedEvents()) {
							this.B.Change();
						}
					}					
				}

			}

			public ObjectB B{get;private set;}

			public void Change() {
				//LOG: using(this.LogBlock(Flow.Enter)) 
				{
					OnTreeChanged();
				}
			}
		}

		private class ObjectB:ObjectBM
		{
			public ObjectB() {
				RegisterChild(C=new ObjectC{MemberName = "C"});
			}

			public ObjectC C{get;private set;}

			public void Change() {
				//LOG: using(this.LogBlock(Flow.Enter)) 
				{
					OnTreeChanged();
				}
			}
		}

		private class ObjectC:ObjectBM
		{
			public void Change() {
				//LOG: using(this.LogBlock(Flow.Enter)) 
				{
					OnTreeChanged();
				}
			}
		}

	}

	// ReSharper restore InconsistentNaming
}