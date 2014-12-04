using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
	/// <summary> Extension for LINQ
	/// </summary>
	public static class LinqExtension
	{

		/// <summary> Creates a List{T} from an IEnumerable.
		/// </summary>
		/// <typeparam name="TResult">The type to convert the elements of source to.</typeparam>
		/// <param name="source">The IEnumerable to create a List{T} from.</param>
		/// <returns>A List{T} that contains elements from the input sequence.</returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public static List<TResult> ToList<TResult>(this IEnumerable source) { return source.Cast<TResult>().ToList(); }

		/// <summary> Returns the number of elements in a sequence.
		/// </summary>
		/// <param name="enumerable">The IEnumerable that contains the elements to be count</param>
		/// <returns></returns>
		public static int Count(this IEnumerable enumerable) {return enumerable.Cast<object>().Count(); }

		/// <summary> Performs the specified action on each element of the <see cref="IEnumerable{T}"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable">The enumerable.</param>
		/// <param name="action">The action.</param>
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
			foreach (var o in enumerable) action(o);
		}

		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> enumerable, TSource first) {
			return enumerable.Except(new TSource[] {first});
		}
		public static IEnumerable<TSource> ExceptMultiple<TSource>(this IEnumerable<TSource> enumerable, params TSource[] except) {
			return enumerable.Except(except);
		}
   
	}
}