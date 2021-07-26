using System.Diagnostics.CodeAnalysis;
using KsWare.Presentation.BusinessFramework;
using NUnit.Framework;

// ReSharper disable TestClassNameDoesNotMatchFileNameWarning

namespace KsWare.Presentation.Tests.BusinessFramework {

    /// <summary> Test Class
    /// </summary>
    [TestFixture]
    [SuppressMessage("ReSharper", "TestClassNameDoesNotMatchFileNameWarning")]
    public class MetricBusinessValueQubicMeterTests {
    	private MetricValueBM value;

    	/// <summary> Setup this instance.
        /// </summary>
        [SetUp]
        public void Setup() {
			this.value=new MetricValueBM {Unit = Unit.QubicMeter};
			this.value.Metadata.DataProvider.Data = 1.0;
        }

        /// <summary>
        /// Teardowns this instance.
        /// </summary>
        [TearDown]
        public void Teardown(){

        }

//		 [Test]
//       public void Nano(){Assert.AreEqual(this.value.Nano,Math.Pow(10.0,9.0*3.0));}

		[Test]
        public void Micro(){Assert.AreEqual(this.value.Micro,System.Math.Pow(10.0,6.0*3.0));}

        [Test]
        public void Milli(){Assert.AreEqual(this.value.Milli,System.Math.Pow(10.0,3.0*3.0));}

		[Test]
        public void Centi(){Assert.AreEqual(this.value.Centi,System.Math.Pow(10.0,2.0*3.0));}

		[Test]
        public void Deci(){Assert.AreEqual(this.value.Deci,System.Math.Pow(10.0,1.0*3.0));}

		[Test]
        public void Value(){Assert.AreEqual(this.value.Value,System.Math.Pow(10.0,0.0*3.0));}

		[Test]
        public void Deca(){Assert.AreEqual((float)this.value.Deca,(float)System.Math.Pow(10.0,-1.0*3.0));}

		[Test]
        public void Hecto(){Assert.AreEqual((float)this.value.Hecto,(float)System.Math.Pow(10.0,-2.0*3.0));}

		[Test]
        public void Kilo(){Assert.AreEqual(this.value.Kilo,System.Math.Pow(10.0,-3.0*3.0));}

		[Test]
        public void Mega(){Assert.AreEqual((float)this.value.Mega,(float)System.Math.Pow(10.0,-6.0*3.0));}

//		[Test]
//        public void Giga(){Assert.AreEqual(this.value.Giga,Math.Pow(10.0,-9.0*3.0));}
    }

    /// <summary>
    /// Test Class
    /// </summary>
    [TestFixture]
    public class MetricBusinessValueSquareMeterTests {

    	private MetricValueBM value;

    	/// <summary>
        /// Setup this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
			this.value=new MetricValueBM {Unit = Unit.SquareMeter};
			this.value.Metadata.DataProvider.Data = 1.0;
        }

        /// <summary>
        /// Teardowns this instance.
        /// </summary>
        [TearDown]
        public void Teardown(){

        }

//		 [Test]
//       public void Nano(){Assert.AreEqual(this.value.Nano,Math.Pow(10.0,9.0*2.0));}

		[Test]
        public void Micro(){Assert.AreEqual(this.value.Micro,System.Math.Pow(10.0,6.0*2.0));}

        [Test]
        public void Milli(){Assert.AreEqual(this.value.Milli,System.Math.Pow(10.0,3.0*2.0));}

		[Test]
        public void Centi(){Assert.AreEqual(this.value.Centi,System.Math.Pow(10.0,2.0*2.0));}

		[Test]
        public void Deci(){Assert.AreEqual(this.value.Deci,System.Math.Pow(10.0,1.0*2.0));}

		[Test]
        public void Value(){Assert.AreEqual(this.value.Value,System.Math.Pow(10.0,0.0*2.0));}

		[Test]
        public void Deca(){Assert.AreEqual((float)this.value.Deca,(float)System.Math.Pow(10.0,-1.0*2.0));}

		[Test]
        public void Hecto(){Assert.AreEqual(this.value.Hecto,System.Math.Pow(10.0,-2.0*2.0));}

		[Test]
        public void Kilo(){Assert.AreEqual(this.value.Kilo,System.Math.Pow(10.0,-3.0*2.0));}

		[Test]
        public void Mega(){Assert.AreEqual(this.value.Mega,System.Math.Pow(10.0,-6.0*2.0));}

//		[Test]
//      public void Giga(){Assert.AreEqual(this.value.Giga,Math.Pow(10.0,-9.0*2.0));}

    }

    /// <summary>
    /// Test Class
    /// </summary>
    [TestFixture]
    public class MetricBusinessValueMeterTests {

    	private MetricValueBM value;

    	/// <summary>
        /// Setup this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
			this.value=new MetricValueBM {Unit = Unit.Meter};
			this.value.Metadata.DataProvider.Data = 1.0;
        }

        /// <summary>
        /// Teardowns this instance.
        /// </summary>
        [TearDown]
        public void Teardown(){

        }

//		 [Test]
//       public void Nano(){Assert.AreEqual(this.value.Nano,Math.Pow(10.0,9.0*1.0));}

		[Test]
        public void Micro(){Assert.AreEqual(this.value.Micro,System.Math.Pow(10.0,6.0*1.0));}

        [Test]
        public void Milli(){Assert.AreEqual(this.value.Milli,System.Math.Pow(10.0,3.0*1.0));}

		[Test]
        public void Centi(){Assert.AreEqual(this.value.Centi,System.Math.Pow(10.0,2.0*1.0));}

		[Test]
        public void Deci(){Assert.AreEqual(this.value.Deci,System.Math.Pow(10.0,1.0*1.0));}

		[Test]
        public void Value(){Assert.AreEqual(this.value.Value,System.Math.Pow(10.0,0.0*1.0));}

		[Test]
        public void Deca(){Assert.AreEqual(this.value.Deca,System.Math.Pow(10.0,-1.0*1.0));}

		[Test]
        public void Hecto(){Assert.AreEqual(this.value.Hecto,System.Math.Pow(10.0,-2.0*1.0));}

		[Test]
        public void Kilo(){Assert.AreEqual(this.value.Kilo,System.Math.Pow(10.0,-3.0*1.0));}

		[Test]
        public void Mega(){Assert.AreEqual(this.value.Mega,System.Math.Pow(10.0,-6.0*1.0));}

//		[Test]
//        public void Giga(){Assert.AreEqual(this.value.Giga,Math.Pow(10.0,-9.0*1.0));}
    }
}
