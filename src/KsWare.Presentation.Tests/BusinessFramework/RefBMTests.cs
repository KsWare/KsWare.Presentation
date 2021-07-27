using System;
using KsWare.Presentation.BusinessFramework;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.BusinessFramework {

	[TestFixture]
	public class RefBMTests {

		[Test]
		public void IRefBM_TargetChangedEvent_get() {
			var bm = new RefBM<Int32BM>();

			var ibm = (IRefBM) bm;
//			IWeakEventSource<EventHandler<ValueChangedEventArgs>> targetChangedEvent = ibm.TargetChangedEvent;
			IEventSource targetChangedEvent = ibm.TargetChangedEvent;

			Assert.AreNotEqual(null,targetChangedEvent);
			var e1= (IEventSource<EventHandler<ValueChangedEventArgs<Int32BM>>>) targetChangedEvent;
		}
	}

}