using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace KsWare.Presentation.Tests.Runner {

	public class LoopComparePerformanceTest {

		public void Run() {
			For();
			ForReverse();
			While();
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		public void For() {
			for (int i = 0; i < 1000000000; i++) {
				
			}
		}
		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		public void ForReverse() {
			for (int i = 1000000000-1; i>=0 ; i--) {
				
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		public void While() {
			var i = 1000000000;
			while (i-->=0) {
				
			}
		}

	}
}
