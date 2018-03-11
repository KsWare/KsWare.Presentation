using System;
using System.Linq;

namespace KsWare.Presentation
{
	/// <summary> Compare any values is true or false
	/// </summary>
	public static class BComp
	{

		/// <summary>
		/// ORs the specified value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		/// <param name="trueValues">The true values.</param>
		/// <returns></returns>
		public static bool OR<T>(T value, params T[] trueValues) where T:IComparable{
			return trueValues.Any(type => type.CompareTo(value)==0);
		}

//		public static bool NOR<T>(T value, params T[] falseValues) where T:IComparable{
//			return !falseValues.Any(type => type.CompareTo(value)==0);
//		}
	}
}
