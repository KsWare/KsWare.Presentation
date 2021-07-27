using System;
using NUnit.Framework;
using KsWare.Presentation.Core.Providers;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.Core {
	// ReSharper disable InconsistentNaming

	/// <summary> Test Class
	/// </summary>
	[TestFixture]
	public class DataProviderTests {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public void Teardown() { }

		[Test]
		public void NoDataProvider() {
			var provider = new NoDataProvider();
			
			Assert.AreEqual(false,provider.IsSupported);
//			Assert.That(((IDataProvider)provider).Data=null                  ,Throws.TypeOf(typeof(NotSupportedException)));
//			Assert.That(((IDataProvider)provider).Data                       ,Throws.Exception);
//			Assert.That(((IDataProvider)provider).DataChangedCallback=null   ,Throws.TypeOf(typeof(NotSupportedException)));
//			Assert.That(((IDataProvider)provider).DataChangedCallback        ,Throws.TypeOf(typeof(NotSupportedException)));
//			Assert.That(((IDataProvider)provider).DataValidatingCallback=null,Throws.TypeOf(typeof(NotSupportedException)));
//			Assert.That(((IDataProvider)provider).DataValidatingCallback     ,Throws.TypeOf(typeof(NotSupportedException)));
//			Assert.That(((IDataProvider)provider).Validate(null)             ,Throws.TypeOf(typeof(NotSupportedException)));
//			Assert.That(((IDataProvider)provider).Data                  ,Throws.TypeOf(typeof(NotSupportedException)));
//			Assert.That(((IDataProvider)provider).SetData(null)              ,Throws.TypeOf(typeof(NotSupportedException)));

			var et = typeof (NotSupportedException);
			Assert.Throws(et, delegate{         ((IDataProvider)provider).Data = null                 ;});
			Assert.Throws(et, delegate{ var d = ((IDataProvider)provider).Data                        ;});
//			Assert.Throws(et, delegate{         ((IDataProvider)provider).DataChangedCallback=null    ;});
//			Assert.Throws(et, delegate{ var d = ((IDataProvider)provider).DataChangedCallback         ;});
			Assert.Throws(et, delegate{         ((IDataProvider)provider).DataValidatingCallback=null ;});
			Assert.Throws(et, delegate{ var d = ((IDataProvider)provider).DataValidatingCallback      ;});
			Assert.Throws(et, delegate{         ((IDataProvider)provider).Validate(null)              ;});
			Assert.Throws(et, delegate{ var d = ((IDataProvider)provider).Data                        ;});
			Assert.Throws(et, delegate{         ((IDataProvider)provider).Data=null                   ;});

		}
	}

	// ReSharper restore InconsistentNaming
}
