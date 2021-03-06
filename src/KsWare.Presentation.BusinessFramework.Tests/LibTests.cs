﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.BusinessFramework.Tests {

	[TestClass]
	public class LibTests {

		[TestMethod]
		public void NamespaceMustMatchAssemblyName() {
			var t = typeof (KsWare.Presentation.BusinessFramework.AssemblyInfo);
			var assemblyName = KsWare.Presentation.BusinessFramework.AssemblyInfo.Assembly.GetName(false).Name;
			Assert.That(t.Namespace, Is.EqualTo(assemblyName));
		}
	}
}
