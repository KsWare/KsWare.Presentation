using System.Diagnostics.CodeAnalysis;
using KsWare.Presentation.BusinessFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable TestClassNameDoesNotMatchFileNameWarning

namespace KsWare.Presentation.Tests.BusinessFramework {

    /// <summary> Test Class
    /// </summary>
    [TestClass]
    [SuppressMessage("ReSharper", "TestClassNameDoesNotMatchFileNameWarning")]
    public class MetricBusinessValueQubicMeterTests {
    	private MetricValueBM value;

    	/// <summary> Setup this instance.
        /// </summary>
        [TestInitialize]
        public void Setup() {
			this.value=new MetricValueBM {Unit = Unit.QubicMeter};
			this.value.Metadata.DataProvider.Data = 1.0;
        }

        /// <summary>
        /// Teardowns this instance.
        /// </summary>
        [TestCleanup]
        public void Teardown(){

        }

//		 [TestMethod]
//       public void Nano(){Assert.AreEqual(this.value.Nano,Math.Pow(10.0,9.0*3.0));}

		[TestMethod]
        public void Micro(){Assert.AreEqual(this.value.Micro,System.Math.Pow(10.0,6.0*3.0));}

        [TestMethod]
        public void Milli(){Assert.AreEqual(this.value.Milli,System.Math.Pow(10.0,3.0*3.0));}

		[TestMethod]
        public void Centi(){Assert.AreEqual(this.value.Centi,System.Math.Pow(10.0,2.0*3.0));}

		[TestMethod]
        public void Deci(){Assert.AreEqual(this.value.Deci,System.Math.Pow(10.0,1.0*3.0));}

		[TestMethod]
        public void Value(){Assert.AreEqual(this.value.Value,System.Math.Pow(10.0,0.0*3.0));}

		[TestMethod]
        public void Deca(){Assert.AreEqual((float)this.value.Deca,(float)System.Math.Pow(10.0,-1.0*3.0));}

		[TestMethod]
        public void Hecto(){Assert.AreEqual((float)this.value.Hecto,(float)System.Math.Pow(10.0,-2.0*3.0));}

		[TestMethod]
        public void Kilo(){Assert.AreEqual(this.value.Kilo,System.Math.Pow(10.0,-3.0*3.0));}

		[TestMethod]
        public void Mega(){Assert.AreEqual((float)this.value.Mega,(float)System.Math.Pow(10.0,-6.0*3.0));}

//		[TestMethod]
//        public void Giga(){Assert.AreEqual(this.value.Giga,Math.Pow(10.0,-9.0*3.0));}
    }

    /// <summary>
    /// Test Class
    /// </summary>
    [TestClass]
    public class MetricBusinessValueSquareMeterTests {

    	private MetricValueBM value;

    	/// <summary>
        /// Setup this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
			this.value=new MetricValueBM {Unit = Unit.SquareMeter};
			this.value.Metadata.DataProvider.Data = 1.0;
        }

        /// <summary>
        /// Teardowns this instance.
        /// </summary>
        [TestCleanup]
        public void Teardown(){

        }

//		 [TestMethod]
//       public void Nano(){Assert.AreEqual(this.value.Nano,Math.Pow(10.0,9.0*2.0));}

		[TestMethod]
        public void Micro(){Assert.AreEqual(this.value.Micro,System.Math.Pow(10.0,6.0*2.0));}

        [TestMethod]
        public void Milli(){Assert.AreEqual(this.value.Milli,System.Math.Pow(10.0,3.0*2.0));}

		[TestMethod]
        public void Centi(){Assert.AreEqual(this.value.Centi,System.Math.Pow(10.0,2.0*2.0));}

		[TestMethod]
        public void Deci(){Assert.AreEqual(this.value.Deci,System.Math.Pow(10.0,1.0*2.0));}

		[TestMethod]
        public void Value(){Assert.AreEqual(this.value.Value,System.Math.Pow(10.0,0.0*2.0));}

		[TestMethod]
        public void Deca(){Assert.AreEqual((float)this.value.Deca,(float)System.Math.Pow(10.0,-1.0*2.0));}

		[TestMethod]
        public void Hecto(){Assert.AreEqual(this.value.Hecto,System.Math.Pow(10.0,-2.0*2.0));}

		[TestMethod]
        public void Kilo(){Assert.AreEqual(this.value.Kilo,System.Math.Pow(10.0,-3.0*2.0));}

		[TestMethod]
        public void Mega(){Assert.AreEqual(this.value.Mega,System.Math.Pow(10.0,-6.0*2.0));}

//		[TestMethod]
//      public void Giga(){Assert.AreEqual(this.value.Giga,Math.Pow(10.0,-9.0*2.0));}

    }

    /// <summary>
    /// Test Class
    /// </summary>
    [TestClass]
    public class MetricBusinessValueMeterTests {

    	private MetricValueBM value;

    	/// <summary>
        /// Setup this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
			this.value=new MetricValueBM {Unit = Unit.Meter};
			this.value.Metadata.DataProvider.Data = 1.0;
        }

        /// <summary>
        /// Teardowns this instance.
        /// </summary>
        [TestCleanup]
        public void Teardown(){

        }

//		 [TestMethod]
//       public void Nano(){Assert.AreEqual(this.value.Nano,Math.Pow(10.0,9.0*1.0));}

		[TestMethod]
        public void Micro(){Assert.AreEqual(this.value.Micro,System.Math.Pow(10.0,6.0*1.0));}

        [TestMethod]
        public void Milli(){Assert.AreEqual(this.value.Milli,System.Math.Pow(10.0,3.0*1.0));}

		[TestMethod]
        public void Centi(){Assert.AreEqual(this.value.Centi,System.Math.Pow(10.0,2.0*1.0));}

		[TestMethod]
        public void Deci(){Assert.AreEqual(this.value.Deci,System.Math.Pow(10.0,1.0*1.0));}

		[TestMethod]
        public void Value(){Assert.AreEqual(this.value.Value,System.Math.Pow(10.0,0.0*1.0));}

		[TestMethod]
        public void Deca(){Assert.AreEqual(this.value.Deca,System.Math.Pow(10.0,-1.0*1.0));}

		[TestMethod]
        public void Hecto(){Assert.AreEqual(this.value.Hecto,System.Math.Pow(10.0,-2.0*1.0));}

		[TestMethod]
        public void Kilo(){Assert.AreEqual(this.value.Kilo,System.Math.Pow(10.0,-3.0*1.0));}

		[TestMethod]
        public void Mega(){Assert.AreEqual(this.value.Mega,System.Math.Pow(10.0,-6.0*1.0));}

//		[TestMethod]
//        public void Giga(){Assert.AreEqual(this.value.Giga,Math.Pow(10.0,-9.0*1.0));}
    }
}
