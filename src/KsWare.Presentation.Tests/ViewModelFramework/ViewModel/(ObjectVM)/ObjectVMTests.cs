

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using KsWare.Presentation.Testing;
using KsWare.Presentation.ViewModelFramework.UIProperties;
using NUnit.Framework;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using Assert=NUnit.Framework.Assert;

#pragma warning disable 67 // “[some event] never used” compiler warning

namespace KsWare.Presentation.Tests.ViewFramework {
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="ObjectVM"/>-class
	/// </summary>
	[TestFixture]
	public class ObjectVMTests: ApplicationVMTestBase {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public override void TestInitialize() {
			base.TestInitialize();
			//...do anything here...
		}

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public override void TestCleanup() {
			//...do anything here...
			base.TestCleanup();
		}

		/// <summary> 
		/// </summary>
		[Test]
		public void SetEnabled_IsEnabled() {
			var vm = new ObjectVM();
			vm.SetEnabled("Test", false);
			Assert.IsFalse(vm.IsEnabled);
			vm.SetEnabled("Test1", false);
			Assert.IsFalse(vm.IsEnabled);
			vm.SetEnabled("Test", true);
			Assert.IsFalse(vm.IsEnabled);
			vm.SetEnabled("Test1", true);
			Assert.IsTrue(vm.IsEnabled);
		}

		/// <summary> 
		/// </summary>
		[Test]
		public void SetReadOnly_IsReadOnly() {
			var vm = new ObjectVM();
			vm.SetReadOnly("Test", false);
			Assert.IsFalse(vm.IsReadOnly);
			vm.SetReadOnly("Test1", false);
			Assert.IsFalse(vm.IsReadOnly);
			vm.SetReadOnly("Test", true);
			Assert.IsFalse(vm.IsReadOnly);
			vm.SetReadOnly("Test1", true);
			Assert.IsTrue(vm.IsReadOnly);
		}

		/// <summary> 
		/// </summary>
		[Test]
		public void RegisterChild() {
			var parent = new TestVM();
		}

		/// <summary> TestDescription
		/// </summary>
		[Test,Ignore("TODO")]
		public void ProperName() {
			var vm=new ObjectVM();
			vm.MemberName = "blubber";
			Assert.Throws<InvalidOperationException>(delegate { vm.MemberName = "other"; });
		}

		[Test]
		public void PropertyChanged() {
			var vm = new Test2VM();
			vm.PropertyChanged+=OnTest2PropertyChanged;
			vm.TestProperty = "test";
			Assert.AreEqual("TestProperty",vm.TestProperty2);
			Assert.AreEqual(1,vm.PropertyChangedCount);
			vm.PropertyChanged-=OnTest2PropertyChanged;
		}

		[Test]
		public void Dispose() {
			var vm = new Test2VM();
			vm.Disposed+=OnTestDisposed;
			vm.Dispose();
			Assert.AreEqual(1,vm.DisposeCount);
		}

		private void OnTestDisposed(object sender, EventArgs eventArgs) { 
			((Test2VM) sender).DisposeCount++;
		}

		private void OnTest2PropertyChanged(object sender, ViewModelPropertyChangedEventArgs e) { 
			((Test2VM)sender).TestProperty2=e.PropertyName;
			((Test2VM) sender).PropertyChangedCount++;
		}

		[Test]
		public void PropertyChangedEvent() {
			var vm = new ObjectVM();

			var count = 0;
			vm.PropertyChangedEvent.Register(this,"{D9FCCC5F-79B6-4DC2-B4EC-EC734EA2EC11}",(sender, args) => count++);
			vm.SetEnabled("test", false);vm.SetEnabled("test", true);
			Assert.AreEqual(2,count);

			vm.PropertyChangedEvent.Release(this,"{D9FCCC5F-79B6-4DC2-B4EC-EC734EA2EC11}");
			count = 0;
			vm.SetEnabled("test", false);vm.SetEnabled("test", true);
			Assert.AreEqual(0,count);
		}

		

		private class TestVM:ObjectVM {

			public TestVM() {
				RegisterChild(new ObjectVM{MemberName = "A"});
				RegisterChild("B", new ObjectVM());
				RegisterChild("inital", new ObjectVM{MemberName = "inital"}); 
				RegisterChild("C", new TextChildVM());

				Assert.AreEqual(4,Children.Count);

				Assert.Throws<InvalidOperationException>(delegate { RegisterChild(new ObjectVM()); });
				Assert.Throws<InvalidOperationException>(delegate { RegisterChild(new ObjectVM{MemberName = ""}); });
				Assert.Throws<InvalidOperationException>(delegate { RegisterChild("",new ObjectVM()); });
				//TODO Assert.Throws<InvalidOperationException>(delegate { RegisterChild(null,new ObjectVM()); });
				Assert.Throws<ArgumentNullException>(delegate { RegisterChild<ObjectVM>(null); });
				Assert.Throws<ArgumentNullException>(delegate { RegisterChild<TextChildVM>("any",null); });

				Assert.Throws<InvalidOperationException>(delegate { RegisterChild("second", new ObjectVM{MemberName = "inital"}); });
			}

			public ObjectVM A{get;private set;}
			public ObjectVM B{get;private set;}
			public ObjectVM inital{get;private set;}
			public IObjectVM C{get;private set;}
		}

		private class TextChildVM:IObjectVM,IHierarchical<IObjectVM> {

			private readonly List<IObjectVM> _Children=new List<IObjectVM>();
			private PropertyChangedEventHandler _notifyPropertyChangedPropertyChanged;

			public TextChildVM() {

			}

			public void Dispose() { Dispose(true); }

			private void Dispose(bool explicitDisposing) {
				if (explicitDisposing) {
					
				}
			}

			public event EventHandler Disposed;

			
			event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {add => _notifyPropertyChangedPropertyChanged+=value; remove => _notifyPropertyChangedPropertyChanged-=value; }

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public IEventSource<EventHandler<ViewModelPropertyChangedEventArgs>> PropertyChangedEvent { get; private set; }
			public IEventSource<EventHandler<ValueChangedEventArgs<ViewModelMetadata>>> MetadataChangedEvent { get; private set; }
			public bool IsEnabled { get; private set; }
			public event EventHandler<UserFeedbackEventArgs> UserFeedbackRequested;
			public event RoutedPropertyChangedEventHandler<bool> IsEnabledChanged;

			public event EventHandler<ViewModelPropertyChangedEventArgs> PropertyChanged;

			public IErrorProvider ErrorProvider => throw new NotImplementedException();

			public bool HasMetadata => false;

			public event EventHandler<ValueChangedEventArgs<ViewModelMetadata>> MetadataChanged;

			public void RequestUserFeedback(UserFeedbackEventArgs args, Action<UserFeedbackEventArgs> callback=null, object state=null) {	 }

			public void NotifyActivated(IObjectVM refferer) {throw new NotImplementedException();}

			public void NotifyDeactivated() {throw new NotImplementedException();}

			public UIPropertiesRoot UI { get; private set; }
			public ViewModelMetadata Metadata {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

			

			public string PropertyName{get;set;}

			public IObjectVM Parent{get;set;} //TODO implement ParentChanged

			public event EventHandler ParentChanged;

			public IEventSource<EventHandler> ParentChangedEvent { get; private set; }
			ICollection<IObjectVM> IHierarchical<IObjectVM>.Children => Children;

			public string MemberName{get;set;}

			IObjectVM IHierarchical<IObjectVM>.Parent { get => Parent; set => Parent = value; }

			public ICollection<IObjectVM> Children => this._Children;

			public string MemberPath {
				get {
					var path = new StringBuilder();
					IHierarchical<IObjectVM> h = this;
					while (h!=null) {
						if(path.Length>0) path.Insert(0, ".");
						path.Insert(0, h.MemberName ?? "?");
						h = h.Parent as IHierarchical<IObjectVM>;
					}
					return path.ToString();
				}
			}


			#region Implementation of ISelectable
			public bool IsSelected { get; set; }
			[Obsolete("Not implemented",true)] public event EventHandler IsSelectedChanged;
			[Obsolete("Not implemented",true)] public IEventSource<EventHandler> IsSelectedChangedEvent { get; private set; }
			#endregion
		}

		private class Test2VM:ObjectVM {
			private string testProperty;

			public string TestProperty {
				get => this.testProperty;
				set {
					if (Equals(this.testProperty, value)) return;
					this.testProperty = value;
					OnPropertyChanged("TestProperty");
				}
			}

			public string TestProperty2{get;set;}

			public int PropertyChangedCount{get;set;}

			public int DisposeCount{get;set;}
		}
	}

	// ReSharper restore InconsistentNaming
}