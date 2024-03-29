﻿using System;
using System.Reflection;
using NUnit.Framework;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.Core{
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="MultiTargetInvocationException"/>-class
	/// </summary>
	[TestFixture]
	public class MultiTargetInvocationExceptionTests {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public void Teardown() { }

		/// <summary> ConstructorA
		/// </summary>
		[Test]
		public void ConstructorA() {
			Assert.Throws<MultiTargetInvocationException>(delegate {
				throw new MultiTargetInvocationException(new[]{new TargetInvocationException(new Exception("Test"))});
			});
		}

		/// <summary> ConstructorB
		/// </summary>
		[Test]
		public void ConstructorB() {
			Assert.Throws<MultiTargetInvocationException>(delegate {
				throw new MultiTargetInvocationException("Test Message", new[]{new TargetInvocationException(new Exception("Test"))});
			});
		}

		/// <summary> ConstructorB
		/// </summary>
		[Test]
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