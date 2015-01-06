using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using KsWare.Presentation.Tests.Runner;

namespace KsWare.Presentation1.Tests.Runner {

	static class Program {

		[STAThread]
		static void Main(string[] args) {
//			KsWare.Presentation.Tests.Program.Main(args);

//			Trace.WriteLine(typeof(EventHandler<PropertyChangedEventArgs>).FullName);
//			Trace.WriteLine(typeof(EventHandler<NotifyCollectionChangedEventArgs>).FullName);
//			Trace.WriteLine(typeof(PropertyChangedEventHandler).FullName);
//			Trace.WriteLine(typeof(NotifyCollectionChangedEventHandler).FullName);

			new  TypeComparePerformanceTest().Run();
			new LoopComparePerformanceTest().Run();
			new EnumeratePerformanceTest().Run();
		}



	}
}
