using System;
using KsWare.Presentation.BusinessFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.BusinessFramework {

	[TestClass]
	public class RefBMTests {

		[TestMethod]
		public void IRefBM_TargetChangedEvent_get() {
			var bm = new RefBM<Int32BM>();

			var ibm = (IRefBM) bm;
//			IWeakEventSource<EventHandler<ValueChangedEventArgs>> targetChangedEvent = ibm.TargetChangedEvent;
			IWeakEventSource targetChangedEvent = ibm.TargetChangedEvent;

			Assert.AreNotEqual(null,targetChangedEvent);
			var e1= (IWeakEventSource<EventHandler<ValueChangedEventArgs<Int32BM>>>) targetChangedEvent;
		}
	}

}