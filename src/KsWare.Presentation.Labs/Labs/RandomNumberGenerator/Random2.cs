using System.Collections.Generic;
using KsWare.Presentation;

namespace KsWare {

	public static class Random {

		private static Random2 R=new Random2();

		public static int Next() { return R.Next(); }

		public static int Next(int minValue, int maxValue) { return R.Next(minValue, maxValue); }

		public static int Next(int maxValue) { return R.Next(maxValue); }

		public static double NextDouble() { return R.NextDouble(); }

		public static void NextBytes(byte[] buffer) { R.NextBytes(buffer); }

		public static void Shuffle<T>(List<T> list) { R.Shuffle(list);}

		public static T Next<T>(IList<T> list, bool remove) { return R.Next(list, remove); }

		public static byte[] NextBytes(int count) {
			var buffer = new byte[count];
			R.NextBytes(buffer);
			return buffer;
		}
	}

	public class Random2 : System.Random {

		public T Next<T>(IList<T> list, bool remove) {
			var r=Next(list.Count);
			var item = list[r];
			if(remove) list.RemoveAt(r);
			return item;
		}

		public void Shuffle<T>(List<T> list) {
			for (int i = 0; i < list.Count; i++) {
				var r = Next(list.Count);
				list.Move(i,r);
			}
			for (int i = 0; i < list.Count; i++) {
				var r = Next(list.Count);
				list.Move(r,i);
			}
			for (int i = 0; i < list.Count*10; i++) {
				var r1 = Next(list.Count);
				var r2 = Next(list.Count);
				if(r1==r2) continue;
				list.Move(r1,r2);
			}
		}
	}
}
