using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.Core{
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="MultiTargetInvocationException"/>-class
	/// </summary>
	[TestClass]
	public class MultiTargetInvocationExceptionTests {

		/// <summary> Setup this instance.
		/// </summary>
		[TestInitialize]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TestCleanup]
		public void Teardown() { }

		/// <summary> ConstructorA
		/// </summary>
		[TestMethod]
		public void ConstructorA() {
			Assert.Throws<MultiTargetInvocationException>(delegate {
				throw new MultiTargetInvocationException(new[]{new TargetInvocationException(new Exception("Test"))});
			});
		}

		/// <summary> ConstructorB
		/// </summary>
		[TestMethod]
		public void ConstructorB() {
			Assert.Throws<MultiTargetInvocationException>(delegate {
				throw new MultiTargetInvocationException("Test Message", new[]{new TargetInvocationException(new Exception("Test"))});
			});
		}

		/// <summary> ConstructorB
		/// </summary>
		[TestMethod]
		public void ReadExceptionsProperty() {
			try {
				throw new MultiTargetInvocationException("Test Message", new[]{new TargetInvocationException(new Exception("Test"))});
			}catch (MultiTargetInvocationException ex) {
				Assert.AreEqual("Test Message", ex.Message);
				Assert.AreEqual(1,ex.Exceptions.Count);
				Assert.AreEqual("Test",ex.Exceptions[0].InnerException.Message);
			}

		}

	}

	// ReSharper restore InconsistentNaming
}