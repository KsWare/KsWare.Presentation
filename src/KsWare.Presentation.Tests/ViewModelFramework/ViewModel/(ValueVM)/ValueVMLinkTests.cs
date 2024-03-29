﻿using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;
using NUnit.Framework;

namespace KsWare.Presentation.Tests.ViewModelFramework {

	[TestFixture]
	public class ValueVMLinkTests {

		[Test]
		public void Instance() {
			var b = new BxVM();
			var a = new AxVM{Data = b};
		}

		[Test]
		public void Instance2() {
			var b = new BxVM();
			b.Int32.Value = 1;
			var a = new AxVM{Data = b};
			Assert.AreEqual(1,a.Int32.Value);
		}

		[Test]
		public void DataChanged_BA() {
			var b = new BxVM();
			var a = new AxVM{Data = b};
			b.Int32.Value = 1;
			Assert.AreEqual(1,a.Int32.Value);
		}

		[Test]
		public void DataChanged_AB() {
			var b = new BxVM();
			var a = new AxVM{Data = b};
			a.Int32.Value = 1;
			Assert.AreEqual(1,b.Int32.Value);
		}
	}

	public class AxVM : DataVM<BxVM> {

		public AxVM() {RegisterChildren(()=>this);}

		[ValueMetadata(typeof(LinkedValueMetadata))]
		public Int32VM Int32 { get; private set; }

		#region Overrides of ObjectVM

		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e); if(e.NewData==null) return;
			Int32.Metadata.BindingProvider.Source = Data.Int32;
		}

		#endregion
	}

	public class BxVM : ObjectVM {

		public BxVM() {RegisterChildren(()=>this);}

		public Int32VM Int32 { get; private set; }
	}
}
