using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KsWare.Presentation.Tests.Core;

namespace KsWare.Presentation.Tests {

	public static class Program {

		[STAThread]
		public static void Main(string[] args) {
//			new WeakEventManagerTests().PerformanceTest1();
//			new WeakEventManagerTests().PerformanceTest1();
//			new WeakEventManagerTests().PerformanceTest2();
//			new WeakEventManagerTests().PerformanceTest2();
			new EventManagerˑIntegrationTests().MassiveListenerGCTest();
		}
	}
}
