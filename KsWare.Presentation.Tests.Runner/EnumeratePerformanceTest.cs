using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace KsWare.Presentation.Tests.Runner {

	public class EnumeratePerformanceTest {
		List<int> m_List=new List<int>(); 

		public void Run() {
			for (int i = 0; i < 10000000; i++) {
				m_List.Add(i);
			}

			For();
			ForReverse();
			ForEach();
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		public void For() {
			int a;
			for (int i = 0; i < 10000000; i++) {
				var v = m_List[i];
				a = v;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		public void ForReverse() {
			int a;
			for (int i = 1000000-1; i>=0 ; i--) {
				var v = m_List[10000000-1-i];
				a = v;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization|MethodImplOptions.NoInlining)]
		public void ForEach() {
			int a;
			foreach (var v in m_List) {
				a = v;
			}
		}

	}
}
