using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KsWare.Presentation {

	public static class ListExtension {

		public static void Move<T>(this IList<T> list, int oldIndex, int newIndex) {
			T obj = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, obj);
		}

		public static T Next<T>(this IList<T> list, T current) {
			var idx = list.IndexOf(current);
			if (idx < 0 /*not found*/ || idx==list.Count-1 /*last*/) return default(T);
			return list[idx + 1];
		}

		public static T Previous<T>(this IList<T> list, T current) {
			var idx = list.IndexOf(current);
			if (idx < 0 /*not found*/ ||  idx==0 /*first*/) return default(T);
			return list[idx - 1];
		}

		public static bool Contains<T>(this IEnumerable<T> collection, T item, Func<T,T,bool> comparer) {
			return collection.Any(x => comparer(x,item));
		}
	}
}
