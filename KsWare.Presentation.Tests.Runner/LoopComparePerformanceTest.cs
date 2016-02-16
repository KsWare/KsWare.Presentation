using System.Runtime.CompilerServices;

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
